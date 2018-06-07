namespace Colin.Lottery.Models.Notification
{
    public class MailNotifyConfig
    {
        public string From { get; set; }

        public string To { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public string Template { get; set; }

        public MailContentType ContentType { get; set; }

        public string SmtpHost { get; set; }

        public int SmtpPort { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}