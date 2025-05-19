namespace DXAgent
{
    public interface IPromptTemplate
    {
        public string FromRaw(string prompt, PromptTemplateType templateType);
    }

    public enum PromptTemplateType
    {
        Chat,
        CodeExplain,
        Rewrite
    }
}
