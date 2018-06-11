using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Colin.Lottery.Models;
using MailKit.Net.Smtp;
using MimeKit;
using SendGrid;
using SendGrid.Helpers.Mail;

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

        public static async Task MailAsync(string from, IEnumerable<string> to, string subject, string content, MailContentType contentType, string smtpHost, int smtpPort, string userName, string password)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(from));
            to.ToList().ForEach(addr => message.To.Add(new MailboxAddress(addr)));
            message.Subject = subject;

            var builder = new BodyBuilder();
            if (contentType == MailContentType.Plain)
                builder.TextBody = content;
            else
                builder.HtmlBody = content;
            message.Body = builder.ToMessageBody();

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtpHost, smtpPort, true);
                    await client.AuthenticateAsync(userName, password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Warn($"邮件发送失败，请检查配置.错误消息:{ex.Message}\r\n堆栈错误:{ex.StackTrace}");
            }
        }

        /// <summary>
        /// 使用 SendGrid 发送邮件 (Google Cloud 支持的服务之一)
        /// </summary>
        /// <returns>发送回执</returns>
        /// <param name="sendGridApiKey">Sendgrid API key.</param>
        /// <param name="from">From.</param>
        /// <param name="tos">Tos.</param>
        /// <param name="subject">Subject.</param>
        /// <param name="content">Content.</param>
        /// <param name="contentType">Content type.</param>
        public static async Task MailAsync(string sendGridApiKey, string from, IEnumerable<string> tos, string subject, string content, MailContentType contentType)
        {
            if (tos == null || !tos.Any())
            {
                LogUtil.Warn($"邮件发送失败，错误消息: 收件人不能为空！");
                return;
            }

            var sendgrid = new SendGridClient(sendGridApiKey);
            var emailAddresses = new List<EmailAddress>();
            foreach (var to in tos)
            {
                emailAddresses.Add(new EmailAddress(to));
            }

            var sendMessage = new SendGridMessage()
            {
                From = new EmailAddress(from),
                Subject = subject,
                HtmlContent = "测试 SendGrid " + DateTime.Now,
                Personalizations = new List<Personalization> {
                    new Personalization(){
                        Tos = emailAddresses
                    }
                }
            };
            SendGrid.Response response =  await sendgrid.SendEmailAsync(sendMessage);

            LogUtil.Info($"Sendgrid 邮件发送状态: {response.StatusCode}");
        }
    }
}
