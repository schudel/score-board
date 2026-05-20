using Microsoft.Extensions.DependencyInjection;
using ScoreBoard.Infrastructure.Repositories;
using ScoreBoard.Services.Adapters;
using ScoreBoard.Services.Helpers;
using ScoreBoard.Services.UseCases;

namespace ScoreBoard.API
{
    public class DependencyInjectionManager
    {
        public static void RegisterBindings(IServiceCollection services)
        {
            // register services
            services.AddTransient<IPasswordService, PasswordService>();
            services.AddTransient<IEmailService, EmailService>();

            services.AddTransient<IChatService, ChatService>();
            services.AddTransient<ILiveMatchService, LiveMatchService>();
            services.AddTransient<ISettingsService, SettingsService>();
            services.AddTransient<ITeamService, TeamService>();
            services.AddTransient<IPlayerService, PlayerService>();
            services.AddTransient<IGameService, GameService>();
            services.AddTransient<IMatchService, MatchService>();
            services.AddTransient<IRatingService, RatingService>();
            services.AddTransient<IRatingHistoryService, RatingHistoryService>();
            services.AddTransient<IPasswordResetRequestService, PasswordResetRequestService>();

            // register repositories
            services.AddTransient<IChatRepository, ChatRepository>();
            services.AddTransient<ILiveMatchRepository, LiveMatchRepository>();
            services.AddTransient<ITeamRepository, TeamRepository>();
            services.AddTransient<IPlayerRepository, PlayerRepository>();
            services.AddTransient<IGameRepository, GameRepository>();
            services.AddTransient<IMatchRepository, MatchRepository>();
            services.AddTransient<IRatingRepository, RatingRepository>();
            services.AddTransient<IRatingHistoryRepository, RatingHistoryRepository>();
            services.AddTransient<IPasswordResetRequestRepository, PasswordResetRequestRepository>();
        }
    }
}
