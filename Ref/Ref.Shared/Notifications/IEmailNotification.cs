using Ref.Shared.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ref.Shared.Notifications
{
    public interface IEmailNotification
    {
    }

    public class EmailNotification : IEmailNotification
    {
        private readonly IEmailProvider _emailProvider;

        public EmailNotification(
            IEmailProvider emailProvider)
        {
            _emailProvider = emailProvider;
        }
    }
}