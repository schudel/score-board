using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScoreBoard.API.Dtos;
using ScoreBoard.API.Models;
using ScoreBoard.Services.UseCases;

namespace ScoreBoard.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> logger;
        private readonly IChatService chatService;

        public ChatHub(IOptions<AppSettings> appSettings, ILogger<ChatHub> logger, IChatService chatService)
        {
            AppSettings settings = appSettings.Value;
            this.logger = logger;
            this.chatService = chatService;
            chatService.Initialize(settings.DbConnectionString);
        }

        // ReSharper disable once UnusedMember.Global
        public async Task SendMessage(ChatDto message)
        {
            try
            {
                await Clients.All.SendAsync("ReceiveMessage", message).ConfigureAwait(false);
                await chatService.Add(message.GetChat()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }

        // ReSharper disable once UnusedMember.Global
        public async Task SendTypingState(ChatTypingStateDto typingState)
        {
            try
            {
                await Clients.All.SendAsync("ReceiveTypingState", typingState).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }
    }
}
