using System;
using eCampusGuard.Core.Entities;
using eCampusGuard.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace eCampusGuard.Services.NotificationServices
{
	public class EmailNotificationService : INotificationService<Response>
	{
        private readonly SendGridClient _sendGridClient;
        private readonly EmailAddress _from;
        private readonly IUnitOfWork _unitofWork;

        public EmailNotificationService(IConfiguration config, IUnitOfWork unitOfWork)
		{
            var apiKey = config["SENDGRID_API_KEY"];
            var fromAddress = config["SENDGRID_FROM_ADDRESS"];
            if (apiKey == null)
            {
                throw new Exception("Could not find 'SENDGRID_API_KEY' in user secrets");
            }

            if (fromAddress == null)
            {
                throw new Exception("Could not find 'SENDGRID_FROM_ADDRESS' in user secrets");
            }

            _unitofWork = unitOfWork;
            _from = new EmailAddress(fromAddress);
            _sendGridClient = new SendGridClient(apiKey);
            System.Console.Error.WriteLine("API Key: " + apiKey);
            System.Console.Error.WriteLine("Address: " + fromAddress);
        }

        public async Task<Response> SendApplicationAcceptedAsync(AppUser recipient, int applicationId)
        {
            var notification = new Notification
            {
                Title = "Application status update",
                Body = "Your application's status has been updated. Status: Awaiting Payment",
            };

            await _unitofWork.Notifications.AddAsync(notification);

            var userNotification = new UserNotification
            {
                User = recipient,
                Notification = notification
            };

            await _unitofWork.UserNotifications.AddAsync(userNotification);

            if (await _unitofWork.CompleteAsync() < 1)
            {
                throw new Exception("Could not create notification");
            }

            var to = new EmailAddress(recipient.Email);
            Dictionary<string, object> data = new()
            {
                { "id", applicationId },
                { "status", "Awaiting Payment" },
            };
            var email = MailHelper.CreateSingleTemplateEmail(_from, to, "d-35ff07954cb9420d87a5c9734ec00e4a", data);

            return await _sendGridClient.SendEmailAsync(email);
        }

        public async Task<Response> SendApplicationDeniedAsync(AppUser recipient, int applicationId)
        {
            var notification = new Notification
            {
                Title = "Application status update",
                Body = "Your application's status has been updated. Status: Denied",
                //User = recipient
            };

            await _unitofWork.Notifications.AddAsync(notification);

            var userNotification = new UserNotification
            {
                User = recipient,
                Notification = notification
            };

            await _unitofWork.UserNotifications.AddAsync(userNotification);

            if (await _unitofWork.CompleteAsync() < 1)
            {
                throw new Exception("Could not create notification");
            }

            var to = new EmailAddress(recipient.Email);
            Dictionary<string, object> data = new()
            {
                { "id", applicationId },
                { "status", "Denied" },
            };
            var email = MailHelper.CreateSingleTemplateEmail(_from, to, "d-35ff07954cb9420d87a5c9734ec00e4a", data);

            return await _sendGridClient.SendEmailAsync(email);
        }

        public async Task<Response> SendApplicationSubmittedAsync(AppUser recipient, int applicationId)
        {
            var notification = new Notification
            {
                Title = "Application status update",
                Body = "Your application's status has been updated. Status: Pending",
                //User = recipient
            };

            await _unitofWork.Notifications.AddAsync(notification);

            var userNotification = new UserNotification
            {
                User = recipient,
                Notification = notification
            };

            await _unitofWork.UserNotifications.AddAsync(userNotification);

            if (await _unitofWork.CompleteAsync() < 1)
            {
                throw new Exception("Could not create notification");
            }

            var to = new EmailAddress(recipient.Email);
            Dictionary<string, object> data = new()
            {
                { "id", applicationId },
                { "status", "Pending" },
            };
            var email = MailHelper.CreateSingleTemplateEmail(_from, to, "d-35ff07954cb9420d87a5c9734ec00e4a", data);

            return await _sendGridClient.SendEmailAsync(email);
        }

        public async Task<Response> SendGeneralNotificationAsync(IEnumerable<AppUser> recipients, string title, string content)
        {
            var notification = new Notification
            {
                Title = title,
                Body = content,
                //User = recipient
            };

            await _unitofWork.Notifications.AddAsync(notification);

            var userNotifications = recipients.Select(user => new UserNotification
            {
                User = user,
                Notification = notification
            });

            await _unitofWork.UserNotifications.AddRangeAsync(userNotifications);

            if (await _unitofWork.CompleteAsync() < 1)
            {
                throw new Exception("Could not create notification");
            }

            var tos = recipients.Select(user => new EmailAddress(user.Email)).ToList();
            Dictionary<string, object> data = new()
            {
                { "title", title },
                { "message", content },
            };
            var email = MailHelper.CreateSingleTemplateEmailToMultipleRecipients(_from, tos, "d-90e70651ec06469d9f9cdd97fc48c169", data);

            return await _sendGridClient.SendEmailAsync(email);
        }

        public async Task<Response> SendPaymentSuccessfulAsync(AppUser recipient, int applicationId)
        {
            var notification = new Notification
            {
                Title = "Application status update",
                Body = "Your application's status has been updated. Status: Paid",
                //User = recipient
            };

            await _unitofWork.Notifications.AddAsync(notification);

            var userNotification = new UserNotification
            {
                User = recipient,
                Notification = notification
            };

            await _unitofWork.UserNotifications.AddAsync(userNotification);

            if (await _unitofWork.CompleteAsync() < 1)
            {
                throw new Exception("Could not create notification");
            }

            var to = new EmailAddress(recipient.Email);
            Dictionary<string, object> data = new()
            {
                { "id", applicationId },
                { "status", "Paid" },
            };
            var email = MailHelper.CreateSingleTemplateEmail(_from, to, "d-35ff07954cb9420d87a5c9734ec00e4a", data);

            return await _sendGridClient.SendEmailAsync(email);
        }

    }
}

