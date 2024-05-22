using System;
using eCampusGuard.Core.Entities;

namespace eCampusGuard.Core.Interfaces
{
	/// <summary>
	/// An interface for a notification service. Could be email or phone.
	/// </summary>
	/// <typeparam name="TResponse">Response type class</typeparam>
	public interface INotificationService<TResponse> where TResponse : class
	{
		Task<TResponse> SendApplicationSubmittedAsync(AppUser recipient, int applicationId);

		Task<TResponse> SendApplicationAcceptedAsync(AppUser recipient, int applicationId);

		Task<TResponse> SendApplicationDeniedAsync(AppUser recipient, int applicationId);

        Task<TResponse> SendPaymentSuccessfulAsync(AppUser recipient, int applicationId);

        Task<TResponse> SendGeneralNotificationAsync(IEnumerable<AppUser> recipients, string title, string content);
    }
}

