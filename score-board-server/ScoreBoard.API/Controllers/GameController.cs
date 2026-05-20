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
    public class GameController : ControllerBase
    {
        private readonly ILogger<GameController> logger;
        private readonly IGameService gameService;

        public GameController(IOptions<AppSettings> appSettings, ILogger<GameController> logger, IGameService gameService)
        {
            AppSettings settings = appSettings.Value;
            this.logger = logger;
            this.gameService = gameService;
            gameService.Initialize(settings.DbConnectionString);
        }

        /// <summary>
        /// Get all Games.
        /// </summary>
        /// <returns>All Games</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ICollection<GameDto>>> Get()
        {
            try
            {
                ICollection<GameDto> gameDtos = new List<GameDto>();
                ICollection<Game> games = await gameService.GetAll().ConfigureAwait(false);
                if (games == null)
                {
                    return Ok(gameDtos);
                }
                foreach (Game game in games)
                {
                    gameDtos.Add(GameDto.Create(game));
                }
                return Ok(gameDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get Game by Id
        /// </summary>
        /// <param name="id">Id of Game</param>
        /// <returns>Game with Id</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<GameDto>> Get(string id)
        {
            try
            {
                return Ok(GameDto.Create(await gameService.GetById(id).ConfigureAwait(false)));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Add new Game
        /// </summary>
        /// <param name="gameDto">New Game</param>
        /// <returns>Ok</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin)]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> Post([FromBody]GameDto gameDto)
        {
            try
            {
                await gameService.Add(gameDto.GetGame()).ConfigureAwait(false);
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
        /// Update an existing Game
        /// </summary>
        /// <param name="id">Game Id</param>
        /// <param name="gameDto">Updated Game</param>
        /// <returns>Ok</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin)]
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> Put(string id, [FromBody]GameDto gameDto)
        {
            try
            {
                await gameService.Update(id, gameDto.GetGame()).ConfigureAwait(false);
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
        /// Delete Game
        /// </summary>
        /// <param name="id">Game Id</param>
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
                await gameService.Remove(id).ConfigureAwait(false);
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
        /// Count all Games
        /// </summary>
        /// <returns>Game Count</returns>
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
                return Ok(await gameService.Count().ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get all Game Genres
        /// </summary>
        /// <returns>Game Genres</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("Genres")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ICollection<string>>> GetGenres()
        {
            try
            {
                return Ok(await gameService.GetGenres().ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get all Game Types
        /// </summary>
        /// <returns>Game Types</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("Types")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ICollection<string>>> GetTypes()
        {
            try
            {
                return Ok(await gameService.GetTypes().ConfigureAwait(false));
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