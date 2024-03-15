using System;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace eCampusGuard.API.Extensions
{
    public class XEnumNamesDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var enums = swaggerDoc.Components.Schemas.Where(s => !s.Value.Enum.IsNullOrEmpty());
            var assembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "eCampusGuard.Core");
            foreach (var e in enums)
            {
                var type = assembly.ExportedTypes.FirstOrDefault(t => t.Name == e.Key);
                var names = type.GetEnumNames();
                var enumArray = new OpenApiArray();
                enumArray.AddRange(names.OfType<string>().Select(p => new OpenApiString(p)).ToList<IOpenApiAny>());
                e.Value.Extensions.Add("x-enum-varnames", enumArray);
            }
        }
    }
}

