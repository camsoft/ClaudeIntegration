namespace ClaudeIntegration.Server.Models
{
    /// <summary>
    /// Request body for the Workbench-style "expert" prompt endpoint, pairing a
    /// system prompt (Claude's role/expertise) with a user question.
    /// </summary>
    public class ExpertPromptRequest
    {
        public string SystemPrompt { get; set; } = string.Empty;
        public string UserQuestion { get; set; } = string.Empty;
    }
}
