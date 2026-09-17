using Microsoft.AspNetCore.Hosting;

namespace Scholar.Common.Email
{
    public interface IEmailTemplateRenderer
    {
        Task<string> RenderAsync(string templateName, IReadOnlyDictionary<string, string> values);
    }

    public class EmailTemplateRenderer : IEmailTemplateRenderer
    {
        private readonly string _templateDir;

        public EmailTemplateRenderer(IWebHostEnvironment env)
        {
            _templateDir = Path.Combine(env.ContentRootPath, "Templates", "Email");
        }

        public async Task<string> RenderAsync(string templateName, IReadOnlyDictionary<string, string> values)
        {
            string path = Path.Combine(_templateDir, templateName);
            string html = await File.ReadAllTextAsync(path);

            foreach (KeyValuePair<string, string> pair in values)
            {
                html = html.Replace("{{" + pair.Key + "}}", pair.Value);
            }

            return html;
        }
    }
}
