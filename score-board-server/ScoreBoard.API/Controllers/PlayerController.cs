using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    public class PlayerController : ControllerBase
    {
        private readonly ILogger<PlayerController> logger;
        private readonly IPlayerService playerService;
        private readonly ISettingsService settingsService;

        public PlayerController(IOptions<AppSettings> appSettings, ILogger<PlayerController> logger, IPlayerService playerService, ISettingsService settingsService)
        {
            AppSettings settings = appSettings.Value;
            this.logger = logger;
            this.playerService = playerService;
            playerService.Initialize(settings.DbConnectionString);
            this.settingsService = settingsService;
            settingsService.Initialize(settings.DbConnectionString);
        }

        /// <summary>
        /// Get all Players
        /// </summary>
        /// <returns>All Players</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ICollection<PlayerDto>>> Get()
        {
            try
            {
                ICollection<PlayerDto> playerDtos = new List<PlayerDto>();
                ICollection<Player> players = await playerService.GetAll().ConfigureAwait(false);
                if (players == null)
                {
                    return Ok(playerDtos);
                }
                foreach (Player player in players)
                {
                    playerDtos.Add(PlayerDto.Create(player));
                }
                return Ok(playerDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get Player by Id
        /// </summary>
        /// <param name="id">Player Id</param>
        /// <returns>Player with Id</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<PlayerDto>> Get(string id)
        {
            try
            {
                return Ok(PlayerDto.Create(await playerService.GetById(id).ConfigureAwait(false)));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Add new Player
        /// </summary>
        /// <param name="playerDto">New Player</param>
        /// <returns>Ok</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin)]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> Post([FromBody]PlayerDto playerDto)
        {
            try
            {
                await playerService.Create(playerDto.GetPlayer(), playerDto.Password, true).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Update existing Player
        /// </summary>
        /// <param name="id">Player Id</param>
        /// <param name="playerDto">Updated Player</param>
        /// <returns>Ok</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> Put(string id, [FromBody]PlayerDto playerDto)
        {
            try
            {
                await playerService.Update(id, playerDto.GetPlayer()).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Delete Player
        /// </summary>
        /// <param name="id">Player Id</param>
        /// <returns>Ok</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin)]
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                await playerService.Remove(id).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Change the current Password
        /// </summary>
        /// <param name="id">Player Id</param>
        /// <param name="changePasswordDto">Current and New Password</param>
        /// <returns>Ok</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpPut("ChangePassword/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> ChangePassword(string id, [FromBody]ChangePasswordDto changePasswordDto)
        {
            try
            {
                await playerService.ChangePassword(id, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Update Settings of a Player
        /// </summary>
        /// <param name="id">Player Id</param>
        /// <param name="settingsDto">Updated Settings</param>
        /// <returns>Ok</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpPut("Settings/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> Settings(string id, [FromBody]SettingsDto settingsDto)
        {
            try
            {
                await settingsService.Update(id, settingsDto.GetSettings()).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Count all Players
        /// </summary>
        /// <returns>Player Count</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("Count")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<long>> Count()
        {
            try
            {
                return Ok(await playerService.Count().ConfigureAwait(false));
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