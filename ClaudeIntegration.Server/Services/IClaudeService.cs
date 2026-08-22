namespace ClaudeIntegration.Server.Services
{
    public interface IClaudeService
    {
        Task<string> GetClaudeResponseAsync(string prompt, int maxTokens = 1000, double temperature = 0.7);

        Task<string> GetExpertResponseAsync(string systemPrompt, string userQuestion, int maxTokens = 1000);
    }
}
