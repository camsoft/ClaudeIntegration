using Anthropic.Models.Messages;

namespace ClaudeIntegration.Server.Services
{
    public class ClaudeService : IClaudeService
    {
        private readonly Anthropic.AnthropicClient _anthropicClient;

        public ClaudeService(Anthropic.AnthropicClient anthropicClient)
        {
            _anthropicClient = anthropicClient;
        }

        public async Task<string> GetClaudeResponseAsync(string prompt, int maxTokens = 1000, double temperature = 0.7)
        {
            // Note: `temperature` is intentionally not forwarded to the request below.
            // The Anthropic SDK marks MessageCreateParams.Temperature as [Obsolete] -
            // models released after Claude Opus 4.6 reject any value other than 1.0
            // with an HTTP 400, so setting it is no longer functional for current models.
            var messages = new List<MessageParam>
            {
                new MessageParam
                {
                    Role = Role.User,
                    Content = prompt
                }
            };

            var tools = new List<ToolUnion>
            {
                new WebSearchTool20250305 { MaxUses = 5 }
            };

            var parameters = new MessageCreateParams
            {
                Messages = messages,
                MaxTokens = maxTokens,
                Model = Model.ClaudeOpus4_8,
                Tools = tools,
            };

            var response = await _anthropicClient.Messages.Create(parameters);

            var returnOutput = string.Empty;

            foreach (var contentBlock in response.Content)
            {
                if (contentBlock.TryPickText(out var textBlock))
                {
                    returnOutput += textBlock.Text;
                }
            }

            return returnOutput;
        }

        /// <summary>
        /// Demonstrates a "Workbench-style" prompt: a system prompt that assigns Claude
        /// a persona/role and instructions, paired with a separate user question. This
        /// mirrors the System Prompt + User Message template used in Claude's Workbench.
        /// </summary>
        /// <param name="systemPrompt">
        /// Sets Claude's role/expertise and behavior, e.g. "You are an expert nutritionist.
        /// Answer concisely and cite sources when possible."
        /// </param>
        /// <param name="userQuestion">The question or task for Claude to respond to.</param>
        public async Task<string> GetExpertResponseAsync(string systemPrompt, string userQuestion, int maxTokens = 1000)
        {
            var messages = new List<MessageParam>
            {
                new MessageParam
                {
                    Role = Role.User,
                    Content = userQuestion
                }
            };

            var parameters = new MessageCreateParams
            {
                System = systemPrompt,
                Messages = messages,
                MaxTokens = maxTokens,
                Model = Model.ClaudeOpus4_8,
                //Model = Model.ClaudeFable5
                //Model = Model.ClaudeSonnet5
            };

            var response = await _anthropicClient.Messages.Create(parameters);

            var returnOutput = string.Empty;

            foreach (var contentBlock in response.Content)
            {
                if (contentBlock.TryPickText(out var textBlock))
                {
                    returnOutput += textBlock.Text;
                }
            }

            return returnOutput;
        }
    }
}
