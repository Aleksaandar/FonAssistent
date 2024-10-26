using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using IMPOLAssistant.API.Models;
using IMPOLAssistant.SemanticKernel;
using IMPOLAssistant.KernelMemory;
using MongoDB.Driver.Core.WireProtocol.Messages;


namespace IMPOLAssistant.API.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ISemanticKernelService semanticKernel;
        private readonly IKernelMemoryService kernelMemoryService;
        private readonly ITabelaLeguraKernelService tabelaLeguraKernelService;

        public ChatController(ISemanticKernelService semanticKernel, IKernelMemoryService kernelMemoryService,
            ITabelaLeguraKernelService tabelaLeguraKernelService)
        {
            this.kernelMemoryService = kernelMemoryService;
            this.tabelaLeguraKernelService = tabelaLeguraKernelService;
            this.semanticKernel = semanticKernel;
        }

        [HttpPost("send")]
        public async Task<List<Message>> SendMessage([FromBody] Message userMessage,  string index)
        {
            var responseMessages = new List<Message>();

            var odgovor = await this.kernelMemoryService.AskAsync(userMessage.Content,index);

            responseMessages.Add(new Message
            {
                Content = "Pitanje: " + userMessage.Content,
                Timestamp = DateTime.Now
            });

            responseMessages.Add(new Message
            {
                Content = "Odgovor: " + odgovor,
                Timestamp = DateTime.Now
            });

            return responseMessages;
        }


        [HttpPost("send-legura")]
        public async Task<IActionResult> SendLegura([FromBody] Message userMessage)
        {
            var responseMessages = new List<Message>();
            var result = await this.tabelaLeguraKernelService.ProcessUserQueryAsync(userMessage.Content);
            responseMessages.Add(new Message
            {
                Content = "Pitanje: " + userMessage.Content,
                Timestamp = DateTime.Now
            });

            responseMessages.Add(new Message
            {
                Content = "Odgovor: " + result,
                Timestamp = DateTime.Now
            });

            return Ok(responseMessages);
        }
    }
}
