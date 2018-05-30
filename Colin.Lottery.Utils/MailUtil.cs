using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;

using Colin.Lottery.Models;
using System.Collections.Generic;

namespace Colin.Lottery.Utils
{
    public static class MailUtil
    {
        /// <summary>
        /// 发送Email(强制启用SSL)
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="subject">Subject.</param>
        /// <param name="content">Content.</param>
        /// <param name="contentType">Content type.</param>
        /// <param name="smtpHost">Smtp host.</param>
        /// <param name="smtpPort">Smtp port.</param>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        public static async Task MailAsync(string from, string to, string subject, string content, MailContentType contentType, string smtpHost, int smtpPort, string userName, string password)
        {
            await MailAsync(from, new List<string> { to }, subject, content, contentType, smtpHost, smtpPort, userName, password);
        }

        public static async Task MailAsync(string from, List<string> to, string subject, string content, MailContentType contentType, string smtpHost, int smtpPort, string userName, string password)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(from));
            to.ForEach(addr => message.To.Add(new MailboxAddress(addr)));
            message.Subject = subject;

            var builder = new BodyBuilder();
            if (contentType == MailContentType.Plain)
                builder.TextBody = content;
            else
                builder.HtmlBody = content;
            message.Body = builder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(smtpHost, smtpPort, true);
                await client.AuthenticateAsync(userName, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
