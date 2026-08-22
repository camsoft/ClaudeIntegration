using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClaudeIntegration.Server.Services;
using ClaudeIntegration.Server.Models;

namespace ClaudeIntegration.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClaudeController : ControllerBase
    {
        private readonly IClaudeService _claudeService;

        public ClaudeController(IClaudeService claudeService)
        {
            _claudeService = claudeService;
        }

        [HttpPost("prompt")]
        public async Task<IActionResult> GetClaudeResponse([FromBody] string prompt)
        {
            var response = await _claudeService.GetClaudeResponseAsync(prompt);
            return Ok(new { response });
        }

        /// <summary>
        /// Demonstrates a Workbench-style System Prompt + User Message request, where
        /// Claude is given a persona/role via the system prompt before answering.
        /// </summary>
        [HttpPost("expert")]
        public async Task<IActionResult> GetExpertResponse([FromBody] ExpertPromptRequest request)
        {
            var response = await _claudeService.GetExpertResponseAsync(request.SystemPrompt, request.UserQuestion);
            return Ok(new { response });
        }
    }
}
