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
    public class LiveMatchController : ControllerBase
    {
        private readonly ILogger<LiveMatchController> logger;
        private readonly ILiveMatchService liveMatchService;

        public LiveMatchController(IOptions<AppSettings> appSettings, ILogger<LiveMatchController> logger, ILiveMatchService liveMatchService)
        {
            AppSettings settings = appSettings.Value;
            this.logger = logger;
            this.liveMatchService = liveMatchService;
            liveMatchService.Initialize(settings.DbConnectionString);
        }

        /// <summary>
        /// Get all Live Matches
        /// </summary>
        /// <returns>All Live Matches</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ICollection<LiveMatchDto>>> Get()
        {
            try
            {
                ICollection<LiveMatchDto> liveMatchDtos = new List<LiveMatchDto>();
                ICollection<LiveMatch> liveMatches = await liveMatchService.GetAll().ConfigureAwait(false);
                if (liveMatches == null)
                {
                    return Ok(liveMatchDtos);
                }
                foreach (LiveMatch liveMatch in liveMatches)
                {
                    liveMatchDtos.Add(LiveMatchDto.Create(liveMatch));
                }
                return Ok(liveMatchDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get all Live Matches distinct
        /// </summary>
        /// <returns>All Live Matches distinct</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("Distinct")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ICollection<LiveMatchDto>>> GetAllDistinct()
        {
            try
            {
                ICollection<LiveMatchDto> liveMatchDtos = new List<LiveMatchDto>();
                ICollection<LiveMatch> liveMatches = await liveMatchService.GetAllDistinct().ConfigureAwait(false);
                if (liveMatches == null)
                {
                    return Ok(liveMatchDtos);
                }
                foreach (LiveMatch liveMatch in liveMatches)
                {
                    liveMatchDtos.Add(LiveMatchDto.Create(liveMatch));
                }
                return Ok(liveMatchDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get Live Match by Id
        /// </summary>
        /// <param name="id">Match Id</param>
        /// <returns>Live Match by Id</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<LiveMatchDto>> Get(string id)
        {
            try
            {
                return Ok(LiveMatchDto.Create(await liveMatchService.GetById(id).ConfigureAwait(false)));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get Live Match by Match Id
        /// </summary>
        /// <param name="matchId">Match Id</param>
        /// <returns>Live Match by Match Id</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("Match")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ICollection<LiveMatchDto>>> GetByMatchId(string matchId)
        {
            try
            {
                ICollection<LiveMatchDto> liveMatchDtos = new List<LiveMatchDto>();
                ICollection<LiveMatch> liveMatches = await liveMatchService.GetByMatchId(matchId).ConfigureAwait(false);
                if (liveMatches == null)
                {
                    return Ok(liveMatchDtos);
                }
                foreach (LiveMatch liveMatch in liveMatches)
                {
                    liveMatchDtos.Add(LiveMatchDto.Create(liveMatch));
                }
                return Ok(liveMatchDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Delete all Live Matches
        /// </summary>
        /// <returns>Ok</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin)]
        [HttpDelete("")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> DeleteAll()
        {
            try
            {
                ICollection<LiveMatch> allChats = await liveMatchService.GetAll().ConfigureAwait(false);
                foreach (LiveMatch liveMatch in allChats)
                {
                    await liveMatchService.Remove(liveMatch).ConfigureAwait(false);
                }
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
        /// Invite Player to specific Match
        /// </summary>
        /// <param name="invitationDto">Invitation</param>
        /// <returns>Ok</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpPost("Invite")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> Invite([FromBody]InvitationDto invitationDto)
        {
            try
            {
                bool invitationSuccessful = await liveMatchService.InvitePlayer(invitationDto.GetInvitation()).ConfigureAwait(false);
                if (invitationSuccessful)
                {
                    return Ok();
                }
                return StatusCode(500, "Invitation failed");
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