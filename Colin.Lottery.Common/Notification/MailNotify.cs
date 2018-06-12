using System;
using System.IO;
using System.Threading.Tasks;

using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;

using Colin.Lottery.Models;
using Colin.Lottery.Models.Notification;
using Colin.Lottery.Utils;

namespace Colin.Lottery.Common.Notification
{
    public static class MailNotify
    {
        public static async Task NotifyAsync(MailNotifyModel model)
        {
            var config = ConfigUtil.GetAppSettings<MailNotifyConfig>("MailNotify");
            SendGridConfig sendgridApiKey = ConfigUtil.GetAppSettings<SendGridConfig>("SendGridConfig");

            var subject = $"{model.Lottery}-{model.Rule}-{model.Plan}-{model.CurrentPeriod.KeepGuaCnt}连挂-第{model.CurrentPeriod.ChaseTimes}期";
            string content;
            switch (config.ContentType)
            {
                case MailContentType.Html:
                    content = GetEmailContent(config.Template, model);
                    break;
                case MailContentType.Plain:
                    content = config.Content;
                    break;
                default:
                    LogUtil.Error("邮件通知内容格式未知，请检查配置文件");
                    return;
            }

            await MailUtil.MailAsync(sendgridApiKey.ApiKey,
                                     config.From, config.To.Split(','), subject, content, config.ContentType);
        }

        private static string GetEmailContent(string templateFile, MailNotifyModel model)
        {
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, templateFile);
            if (!File.Exists(file))
            {
                LogUtil.Error($"邮件通知模板文件({templateFile})不存在，请检查配置文件或模板文件");
                return null;
            }

            try
            {
                var engine = new VelocityEngine();
                engine.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, AppDomain.CurrentDomain.BaseDirectory);
                engine.Init();
                var template = engine.GetTemplate(templateFile);
                var context = new VelocityContext();
                context.Put("Model", model);

                var writer = new StringWriter();
                template.Merge(context, writer);
                return writer.GetStringBuilder().ToString();
            }
            catch (Exception ex)
            {
                LogUtil.Warn($"通知邮件内容生成失败,错误消息:{ex.Message}堆栈内容:{ex.StackTrace}");
                return null;
            }
        }
    }
}