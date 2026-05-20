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
    public class ChatController : ControllerBase
    {
        private readonly ILogger<ChatController> logger;
        private readonly IChatService chatService;

        public ChatController(IOptions<AppSettings> appSettings, ILogger<ChatController> logger, IChatService chatService)
        {
            AppSettings settings = appSettings.Value;
            this.logger = logger;
            this.chatService = chatService;
            chatService.Initialize(settings.DbConnectionString);
        }

        /// <summary>
        /// Get all Chats
        /// </summary>
        /// <returns>All Chats</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ICollection<ChatDto>>> Get()
        {
            try
            {
                ICollection<ChatDto> chatDtos = new List<ChatDto>();
                ICollection<Chat> chats = await chatService.GetAll().ConfigureAwait(false);
                if (chats == null)
                {
                    return Ok(chatDtos);
                }
                foreach (Chat chat in chats)
                {
                    chatDtos.Add(ChatDto.Create(chat));
                }
                return Ok(chatDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get Chat by Id
        /// </summary>
        /// <param name="id">Chat Id</param>
        /// <returns>Chat by Id</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ChatDto>> Get(string id)
        {
            try
            {
                return Ok(ChatDto.Create(await chatService.GetById(id).ConfigureAwait(false)));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get Chat by Player Id
        /// </summary>
        /// <param name="playerId">Player Id</param>
        /// <returns>Chat by Player Id</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("Player")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ICollection<ChatDto>>> GetByMatchId(string playerId)
        {
            try
            {
                ICollection<ChatDto> chatDtos = new List<ChatDto>();
                ICollection<Chat> chats = await chatService.GetByPlayerId(playerId).ConfigureAwait(false);
                if (chats == null)
                {
                    return Ok(chatDtos);
                }
                foreach (Chat chat in chats)
                {
                    chatDtos.Add(ChatDto.Create(chat));
                }
                return Ok(chatDtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                // return error message if there was an exception
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Delete all Chats
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
                ICollection<Chat> allChats = await chatService.GetAll().ConfigureAwait(false);
                foreach (Chat chat in allChats)
                {
                    await chatService.Remove(chat).ConfigureAwait(false);
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
        /// Get top n Chats
        /// </summary>
        /// <param name="amount">Select n Entries</param>
        /// <param name="skip">Skip n Entries</param>
        /// <returns>Top n Chats</returns>
        /// <response code="200">Ok</response>
        /// <response code="500">If there was an internal server error</response>
        /// <response code="401">If unauthorized</response>
        [Authorize(Roles = RoleDto.Admin + "," + RoleDto.User)]
        [HttpGet("Limit")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ICollection<ChatDto>>> GetTopEntries([FromQuery] int amount, [FromQuery] int skip)
        {
            try
            {
                ICollection<ChatDto> chatDtos = new List<ChatDto>();
                ICollection<Chat> topEntries = await chatService.GetSpecificEntries(amount, skip).ConfigureAwait(false);
                if (topEntries == null)
                {
                    return Ok(chatDtos);
                }
                foreach (Chat chat in topEntries)
                {
                    chatDtos.Add(ChatDto.Create(chat));
                }
                return Ok(chatDtos);
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