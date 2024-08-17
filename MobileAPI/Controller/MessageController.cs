using Application.InterfaceService;
using Application.Service;
using Application.ViewModel.MessageModel;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MobileAPI.Hubs;
using MobileAPI.MobileSevice;

namespace MobileAPI.Controllers
{
    public class MessageController : BaseController
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IOrderService _orderService;
        private readonly IPostService _postService;
        private readonly IClaimService _claimService;
        public MessageController(IMessageService messageService, IHubContext<ChatHub> hubContext, IOrderService orderService, 
            IPostService postService, IClaimService claimService)
        {
            _messageService = messageService;
            _hubContext = hubContext;
            _orderService = orderService;
            _postService = postService;
            _claimService = claimService;
        }
        /*[Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageModel message)
        {
            var isCreated = await _messageService.CreateMessage(message);
            if (isCreated)
            {
                return Ok();
            }
            return BadRequest();
        }*/
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            var userChatRooms = await _messageService.GetAllChatRoomsByUserIdAsync();
            return Ok(userChatRooms);
        }

        [Authorize]
        [HttpGet("{roomId}")]
        public async Task<IActionResult> GetPrivateMessagesByChatRoomId(Guid roomId)
        {
            var allMessages = await _messageService.GetMessagesByChatRoomId(roomId);
            return Ok(allMessages);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ContactNow(Guid userId, Guid postId)
        {
            var userChatRooms = await _messageService.GetOrCreateChatRoomAsync(userId, postId);
            if (userChatRooms == null)
            {
                return BadRequest("userId not exist");
            }
            return Ok(userChatRooms);
           
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> sendMessageTest(Guid roomId, string messageContent)
        {
            var userId = _claimService.GetCurrentUserId;
            if (userId == Guid.Empty)
            {
                throw new HubException("Invalid user ID.");
            }

            var room = await _messageService.GetChatRoomByIdAsync(roomId);
            if (room == null)
            {
                throw new HubException("Room does not exist.");
            }

            var createMessageModel = new CreateMessageModel
            {
                MessageContent = messageContent,
                RoomId = roomId,
                CreatedBy = userId
            };

            var message = await _messageService.CreateMessage(createMessageModel);
            if (message.CreatedBy == null)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}