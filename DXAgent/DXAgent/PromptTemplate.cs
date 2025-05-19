namespace DXAgent
{
    public class PromptTemplate : IPromptTemplate
    {
        private readonly IReadOnlyDictionary<PromptTemplateType, string> _templates;

        public PromptTemplate(IConfiguration configuration)
        {
            Dictionary<PromptTemplateType, string> templates = new();

            templates.Add(PromptTemplateType.Chat, configuration["PromptTemplate:Chat"] ?? "{0}");
            templates.Add(PromptTemplateType.CodeExplain, configuration["PromptTemplate:CodeExplain"] ?? 
                "请你解释下面的代码：\n" +
                "'''\n" +
                "{0}\n" +
                "'''");

            templates.Add(PromptTemplateType.Rewrite, configuration["PromptTemplate:Rewrite"] ??
                "请你重写下面的内容：\n" +
                "\"\n" +
                "{0}\n" +
                "\"");

            this._templates = templates.AsReadOnly();
        }

        public string FromRaw(string prompt, PromptTemplateType templateType)
        {
            if (this._templates.TryGetValue(templateType, out string? template))
            {
                return string.Format(template, prompt);
            }
            else
            {
                throw new ArgumentException($"Invalid template type: {templateType}");
            }
        }
    }
}
