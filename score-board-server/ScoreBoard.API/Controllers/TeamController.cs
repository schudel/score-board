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
    public class TeamController : ControllerBase
    {
        private readonly ILogger<TeamController> logger;
        private readonly ITeamService teamService;

        public TeamController(IOptions<AppSettings> appSettings, ILogger<TeamController> logger, ITeamService teamService)
        {
            AppSettings settings = appSettings.Value;
            this.logger = logger;
            this.teamService = teamService;
            teamService.Initialize(settings.DbConnectionString);
        }

        /// <summary>
        /// Get all Teams
        /// </summary>
        /// <returns>All Teams</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ICollection<TeamDto>>> Get()
        {
            try
            {
                ICollection<TeamDto> teamDtos = new List<TeamDto>();
                ICollection<Team> teams = await teamService.GetAll().ConfigureAwait(false);
                if (teams == null)
                {
                    return Ok(teamDtos);
                }
                foreach (Team team in teams)
                {
                    teamDtos.Add(TeamDto.Create(team));
                }
                return Ok(teamDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get Team by Id
        /// </summary>
        /// <param name="id">Team Id</param>
        /// <returns>Team by Id</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<TeamDto>> Get(string id)
        {
            try
            {
                return Ok(TeamDto.Create(await teamService.GetById(id).ConfigureAwait(false)));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get all Team-Names
        /// </summary>
        /// <returns>All Team-Names</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("Names")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ICollection<string>>> GetTeamNames()
        {
            try
            {
                return Ok(await teamService.GetNames().ConfigureAwait(false));
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