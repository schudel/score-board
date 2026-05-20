using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ScoreBoard.API.Dtos;
using ScoreBoard.API.Models;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.UseCases;

namespace ScoreBoard.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AppSettings appSettings;
        private readonly ILogger<AuthenticationController> logger;
        private readonly IPlayerService playerService;
        private readonly IPasswordResetRequestService passwordResetRequestService;

        public AuthenticationController(IOptions<AppSettings> appSettings, ILogger<AuthenticationController> logger, IPlayerService playerService, IPasswordResetRequestService passwordResetRequestService)
        {
            this.appSettings = appSettings.Value;
            this.logger = logger;
            this.playerService = playerService;
            this.passwordResetRequestService = passwordResetRequestService;
            playerService.Initialize(this.appSettings.DbConnectionString);
            passwordResetRequestService.Initialize(this.appSettings.DbConnectionString);
        }

        /// <summary>
        /// Authenticate with PlayerName and Password
        /// </summary>
        /// <param name="auth">Authentication Credentials</param>
        /// <returns>Authenticated Player</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        [AllowAnonymous]
        [HttpPost("Authenticate")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<PlayerDto>> Authenticate([FromBody]AuthenticationDto auth)
        {
            try
            {
                // authenticate
                Player authPlayer = await playerService.Authenticate(auth.PlayerName, auth.Password).ConfigureAwait(false);
                if (authPlayer == null)
                {
                    return StatusCode(500, "Authentication failed");
                }

                // authentication successful so generate jwt token
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                byte[] key = Encoding.ASCII.GetBytes(appSettings.Secret);
                SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, authPlayer.Id.ToString()),
                        new Claim(ClaimTypes.Name, authPlayer.PlayerName),
                        new Claim(ClaimTypes.Role, authPlayer.Role.Name)
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = appSettings.Issuer,
                    Audience = appSettings.Audience,
                    IssuedAt = DateTime.Now
                };
                SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
                // create playerDto
                PlayerDto playerDto = PlayerDto.Create(authPlayer, tokenHandler.WriteToken(token));

                // return basic playerDto info (without password) and token to store client side
                return Ok(playerDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Register a new Player
        /// </summary>
        /// <param name="playerDto">New Player</param>
        /// <returns>Ok</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        [AllowAnonymous]
        [HttpPost("Register")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> Register([FromBody]PlayerDto playerDto)
        {
            try
            {
                Player player = playerDto.GetPlayer();
                bool registrationSuccessful = await playerService.Register(player, playerDto.Password).ConfigureAwait(false);
                if (registrationSuccessful)
                {
                    return Ok();
                }
                return StatusCode(500, "Registration failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Activates the Account by Player Id - This i necessary to complete the registration
        /// </summary>
        /// <param name="id">Player Id</param>
        /// <returns>Ok</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        [AllowAnonymous]
        [HttpGet("Activate/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> Activate(string id)
        {
            try
            {
                bool activationSuccessful = await playerService.Activate(id).ConfigureAwait(false);
                if (activationSuccessful)
                {
                    return Ok();
                }
                return StatusCode(500, "Activation failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Re-Sends the Registration Email again (in case of faults)
        /// </summary>
        /// <param name="id">Player Id</param>
        /// <returns>Ok</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        [AllowAnonymous]
        [HttpGet("ResendEmail/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> ResendEmail(string id)
        {
            try
            {
                bool activationSuccessful = await playerService.ResendEmail(id).ConfigureAwait(false);
                if (activationSuccessful)
                {
                    return Ok();
                }
                return StatusCode(500, "Resend Email failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Request a Password Reset of a Player
        /// </summary>
        /// <param name="email">Email Address from Player</param>
        /// <returns>Ok</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        [AllowAnonymous]
        [HttpPost("RequestPasswordReset")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> RequestPasswordReset([FromBody]string email)
        {
            try
            {
                // Get Player by Email Address
                Player player = await playerService.GetByEmail(email).ConfigureAwait(false);
                if (player == null)
                {
                    return StatusCode(500, "No Player found with this E-Mail Address");
                }
                player.Email = email;
                // Create a Password Reset Request
                PasswordResetRequest passwordResetRequest = new PasswordResetRequest
                {
                    Player = player,
                    TimeStamp = DateTime.Now
                };
                await passwordResetRequestService.Add(passwordResetRequest).ConfigureAwait(false);
                // Send Password Reset Request to Email
                bool sendPasswordResetSuccessful = await passwordResetRequestService.SendPasswordResetEmail(passwordResetRequest).ConfigureAwait(false);
                if (sendPasswordResetSuccessful)
                {
                    return Ok();
                }
                return StatusCode(500, "Request Password Reset failed");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Reset Password of a Player
        /// </summary>
        /// <param name="resetPasswordDto">Reset Password Dto</param>
        /// <returns>Ok</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        [AllowAnonymous]
        [HttpPut("ResetPassword")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult> ResetPassword([FromBody]ResetPasswordDto resetPasswordDto)
        {
            try
            {
                // Get Password Reset Request
                PasswordResetRequest passwordResetRequest = await passwordResetRequestService.GetById(resetPasswordDto.PasswordRequestId).ConfigureAwait(false);
                if (passwordResetRequest == null)
                {
                    return StatusCode(500, "Cannot reset the Password. Please Request the Password Reset again.");
                }
                // Reset Password
                await playerService.ResetPassword(passwordResetRequest.Player.Id.ToString(), resetPasswordDto.Password).ConfigureAwait(false);
                // Delete Password Reset Request
                await passwordResetRequestService.Remove(passwordResetRequest).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }
    }
}