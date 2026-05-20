using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScoreBoard.API.Dtos;
using ScoreBoard.API.Models;
using ScoreBoard.Domain.Enums;
using ScoreBoard.Domain.Models;
using ScoreBoard.Services.UseCases;

namespace ScoreBoard.API.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly ILogger<RatingController> logger;
        private readonly IRatingService ratingService;
        private readonly IMatchService matchService;
        private readonly IRatingHistoryService ratingHistoryService;

        public RatingController(IOptions<AppSettings> appSettings, ILogger<RatingController> logger, IRatingService ratingService, IMatchService matchService, IRatingHistoryService ratingHistoryService)
        {
            AppSettings settings = appSettings.Value;
            this.logger = logger;
            this.ratingService = ratingService;
            ratingService.Initialize(settings.DbConnectionString);
            this.matchService = matchService;
            matchService.Initialize(settings.DbConnectionString);
            this.ratingHistoryService = ratingHistoryService;
            ratingHistoryService.Initialize(settings.DbConnectionString);
        }

        /// <summary>
        /// Get all existing Ratings
        /// </summary>
        /// <param name="slim">Load light weight version of ratings</param>
        /// <returns>All Ratings</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ICollection<RatingDto>>> Get([FromQuery]bool slim = false)
        {
            try
            {
                ICollection<RatingDto> ratingDto = new List<RatingDto>();
                ICollection<Rating> ratings = await ratingService.GetAll(slim).ConfigureAwait(false);
                if (ratings == null)
                {
                    return Ok(ratingDto);
                }
                foreach (Rating rating in ratings)
                {
                    ratingDto.Add(RatingDto.Create(rating));
                }
                return Ok(ratingDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get Rating by Id
        /// </summary>
        /// <param name="id">Rating Id</param>
        /// <param name="slim">Load light weight version of ratings</param>
        /// <returns>Rating</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<RatingDto>> Get(string id, [FromQuery]bool slim = false)
        {
            try
            {
                return Ok(RatingDto.Create(await ratingService.GetById(id, slim).ConfigureAwait(false)));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// (Re)Calc all existing Ratings and save to the DB
        /// </summary>
        /// <returns>Ok</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin)]
        [HttpPut("CalcAllAndSave")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> CalcAllAndSave()
        {
            try
            {
                await ratingService.CalcAllRatings(true).ConfigureAwait(false);
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
        /// (Re)Calc all existing Ratings
        /// </summary>
        /// <returns>Ok</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin)]
        [HttpPut("CalcAll")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> CalcAll()
        {
            try
            {
                await ratingService.CalcAllRatings().ConfigureAwait(false);
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
        /// Calc Rating for new Match
        /// </summary>
        /// <param name="match">Match for Rating calc</param>
        /// <returns>Ratings for Match</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpPost("Calc")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<RatingDto>> Calc([FromBody]MatchDto match)
        {
            try
            {
                IDictionary<Guid, Rating> calcRating = await ratingService.CalcRating(match.GetMatch()).ConfigureAwait(false);
                if (calcRating != null && calcRating.Count > 0)
                {
                    return Ok(RatingDto.Create(calcRating.Values.ElementAt(0)));
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
        /// Calc Rating for one new Match and save to DB
        /// </summary>
        /// <param name="match">Match for Rating calc</param>
        /// <returns>Ratings for Match</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpPost("CalcAndSave")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<RatingDto>> CalcAndSave([FromBody]MatchDto match)
        {
            try
            {
                Match m = match.GetMatch();
                if (m.State != MatchState.Done)
                {
                    return StatusCode(500, "Match State has to be \"Done\"");
                }
                IDictionary<Guid, Rating> calcRating = await ratingService.CalcRating(m, null, true).ConfigureAwait(false);
                if (calcRating != null && calcRating.Count > 0)
                {
                    return Ok(RatingDto.Create(calcRating.Values.ElementAt(0)));
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
        /// (Re)Calc existing Rating for one new Match and save to DB
        /// </summary>
        /// <param name="id">Match Id for Rating calc</param>
        /// <returns>Ratings for Match</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpPut("ReCalcAndSave/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<RatingDto>> ReCalcAndSave(string id)
        {
            try
            {
                Match m = await matchService.GetById(id).ConfigureAwait(false);
                if (m.State != MatchState.Done)
                {
                    return StatusCode(500, "Match State has to be \"Done\"");
                }
                IDictionary<Guid, Rating> calcRating = await ratingService.CalcRating(m, null, true).ConfigureAwait(false);
                if (calcRating != null && calcRating.Count > 0)
                {
                    return Ok(RatingDto.Create(calcRating.Values.ElementAt(0)));
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
        /// Calc Quality for new Match
        /// </summary>
        /// <param name="match">Match for Quality calc</param>
        /// <returns>Quality for Match</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpPost("CalcMatchQuality")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<double>> CalcMatchQuality([FromBody]MatchDto match)
        {
            try
            {
                return Ok(await ratingService.CalcMatchQuality(match.GetMatch()).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Calc Quality for new Match and save to DB
        /// </summary>
        /// <param name="match">Match for Quality calc</param>
        /// <returns>Quality for Match</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpPost("CalcMatchQualityAndSave")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<double>> CalcMatchQualityAndSave([FromBody]MatchDto match)
        {
            try
            {
                Match m = match.GetMatch();
                return m.State != MatchState.Done ? 
                    StatusCode(500, "Match State has to be \"Done\"") : 
                    Ok(await ratingService.CalcMatchQuality(m, null, true).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// (Re)Calc Quality for existing Match and save to DB
        /// </summary>
        /// <param name="id">Match id for Quality calc</param>
        /// <returns>Quality for Match</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpPut("ReCalcMatchQualityAndSave/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<double>> ReCalcMatchQualityAndSave(string id)
        {
            try
            {
                Match m = await matchService.GetById(id).ConfigureAwait(false);
                return m.State != MatchState.Done ? 
                    StatusCode(500, "Match State has to be \"Done\"") : 
                    Ok(await ratingService.CalcMatchQuality(m, null, true).ConfigureAwait(false));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// (Re)Calc all existing Match Qualities and save to the DB
        /// </summary>
        /// <returns>Ok</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin)]
        [HttpPut("CalcAllMatchQualitiesAndSave")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> CalcAllMatchQualitiesAndSave()
        {
            try
            {
                await ratingService.CalcAllMatchQualities().ConfigureAwait(false);
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
        /// Get Ratings by Player Id
        /// </summary>
        /// <param name="playerId">Player Id</param>
        /// <returns>Ratings for Player Id</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("Player")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ICollection<RatingDto>>> GetByPlayerId([FromQuery]string playerId)
        {
            try
            {
                ICollection<RatingDto> ratingDtos = new List<RatingDto>();
                ICollection<Rating> ratings = await ratingService.GetRatingsByPlayerId(playerId).ConfigureAwait(false);
                if (ratings == null)
                {
                    return Ok(ratingDtos);
                }
                foreach (Rating rating in ratings)
                {
                    ratingDtos.Add(RatingDto.Create(rating));
                }
                return Ok(ratingDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Calculates and returns complete Rating History
        /// </summary>
        /// <returns>Rating History List</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin)]
        [HttpGet("CalcAllRatingHistories")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ICollection<RatingHistoryDto>>> CalcAllRatingHistories()
        {
            try
            {
                ICollection<RatingHistory> ratingHistories = await ratingService.CalcAllRatingHistories().ConfigureAwait(false);
                ICollection<RatingHistoryDto> ratingHistoryDtos = new List<RatingHistoryDto>();
                foreach (RatingHistory ratingDetail in ratingHistories)
                {
                    ratingHistoryDtos.Add(RatingHistoryDto.Create(ratingDetail));
                }
                return Ok(ratingHistoryDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Calculates and returns complete Rating History and save them to DB
        /// </summary>
        /// <returns>Rating History List</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin)]
        [HttpGet("CalcAllRatingHistoriesAndSave")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ICollection<RatingHistoryDto>>> CalcAllRatingHistoriesAndSave()
        {
            try
            {
                ICollection<RatingHistory> ratingHistories = await ratingService.CalcAllRatingHistories(true).ConfigureAwait(false);
                ICollection<RatingHistoryDto> ratingHistoryDtos = new List<RatingHistoryDto>();
                foreach (RatingHistory ratingDetail in ratingHistories)
                {
                    ratingHistoryDtos.Add(RatingHistoryDto.Create(ratingDetail));
                }
                return Ok(ratingHistoryDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get the complete Rating History
        /// </summary>
        /// <returns>Rating History List</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("GetAllRatingHistories")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ICollection<RatingHistoryDto>>> GetAllRatingHistories()
        {
            try
            {
                ICollection<RatingHistory> ratingHistories = await ratingHistoryService.GetAll(true).ConfigureAwait(false);
                ratingHistories = ratingHistories.OrderBy(m => m.DateTime).ToList();
                ICollection<RatingHistoryDto> ratingHistoryDtos = new List<RatingHistoryDto>();
                foreach (RatingHistory ratingDetail in ratingHistories)
                {
                    ratingHistoryDtos.Add(RatingHistoryDto.Create(ratingDetail));
                }
                return Ok(ratingHistoryDtos);
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