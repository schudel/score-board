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
    public class MatchController : ControllerBase
    {
        private readonly ILogger<MatchController> logger;
        private readonly IMatchService matchService;

        public MatchController(IOptions<AppSettings> appSettings, ILogger<MatchController> logger, IMatchService matchService)
        {
            AppSettings settings = appSettings.Value;
            this.logger = logger;
            this.matchService = matchService;
            matchService.Initialize(settings.DbConnectionString);
        }

        /// <summary>
        /// Get all Matches
        /// </summary>
        /// <returns>All Matches</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ICollection<MatchDto>>> Get([FromQuery]bool slim = false)
        {
            try
            {
                ICollection<MatchDto> matchDtos = new List<MatchDto>();
                ICollection<Match> matches = await matchService.GetAll(slim).ConfigureAwait(false);
                if (matches == null)
                {
                    return Ok(matchDtos);
                }
                foreach (Match match in matches)
                {
                    matchDtos.Add(MatchDto.Create(match));
                }
                return Ok(matchDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get Match by Id
        /// </summary>
        /// <param name="id">Match Id</param>
        /// <param name="slim">Load light weight version of matches</param>
        /// <returns>Match by Id</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<MatchDto>> Get(string id, [FromQuery]bool slim = false)
        {
            try
            {
                return Ok(MatchDto.Create(await matchService.GetById(id, slim).ConfigureAwait(false)));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get Match by Game Id
        /// </summary>
        /// <param name="gameId">Game Id</param>
        /// <param name="slim">Load light weight version of matches</param>
        /// <returns>Match by Game Id</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("Game")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ICollection<MatchDto>>> GetByGameId(string gameId, [FromQuery]bool slim = false)
        {
            try
            {
                ICollection<MatchDto> matchDtos = new List<MatchDto>();
                ICollection<Match> matches = await matchService.GetMatchesByGameId(gameId, slim).ConfigureAwait(false);
                if (matches == null)
                {
                    return Ok(matchDtos);
                }
                foreach (Match match in matches)
                {
                    matchDtos.Add(MatchDto.Create(match));
                }
                return Ok(matchDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get Matches by Player Id
        /// </summary>
        /// <param name="playerId">Player Id</param>
        /// <param name="slim">Load light weight version of matches</param>
        /// <returns>Matches for Player Id</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("Player")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ICollection<MatchDto>>> GetByPlayerId([FromQuery]string playerId, [FromQuery]bool slim = false)
        {
            try
            {
                ICollection<MatchDto> matchDtos = new List<MatchDto>();
                ICollection<Match> matches = await matchService.GetMatchesByPlayerId(playerId, slim).ConfigureAwait(false);
                if (matches == null)
                {
                    return Ok(matchDtos);
                }
                foreach (Match match in matches)
                {
                    matchDtos.Add(MatchDto.Create(match));
                }
                return Ok(matchDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Add new Match
        /// </summary>
        /// <param name="matchDto">Net Match</param>
        /// <returns>Ok</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> Post([FromBody]MatchDto matchDto)
        {
            try
            {
                await matchService.Add(matchDto.GetMatch(), true, true).ConfigureAwait(false);
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
        /// Update existing Match
        /// </summary>
        /// <param name="id">Match Id</param>
        /// <param name="matchDto">Updated Match</param>
        /// <returns>Ok</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> Put(string id, [FromBody]MatchDto matchDto)
        {
            try
            {
                await matchService.Update(id, matchDto.GetMatch(), true, true).ConfigureAwait(false);
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
        /// Delete Match
        /// </summary>
        /// <param name="id">Match Id</param>
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
                await matchService.Remove(id).ConfigureAwait(false);
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
        /// Count all Matches
        /// </summary>
        /// <returns>Match Count</returns>
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
                return Ok(await matchService.Count().ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Count Matches by Game
        /// </summary>
        /// <param name="gameId">Game Id</param>
        /// <returns>Match Count</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("CountByGame")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<long>> CountByGame([FromQuery]string gameId)
        {
            try
            {
                return Ok(await matchService.CountByGame(gameId).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Count Matches by Game
        /// </summary>
        /// <param name="playerId">Player Id</param>
        /// <returns>Match Count</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("CountByPlayer")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<long>> CountByPlayer([FromQuery]string playerId)
        {
            try
            {
                return Ok(await matchService.CountByPlayer(playerId).ConfigureAwait(false));
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