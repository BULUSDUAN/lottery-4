using System.Collections.Generic;

namespace Colin.Lottery.Models.Notification
{
    public class MailNotifyConfig
    {
        public string ApiKey { get; set; }
        
        public string From { get; set; }

        public string To { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public string Template { get; set; }

        public MailContentType ContentType { get; set; }
    }
}