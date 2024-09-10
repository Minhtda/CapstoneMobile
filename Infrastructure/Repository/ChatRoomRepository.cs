using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.ViewModel.ChatRoomModel;
using Application.ViewModel.UserModel;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class ChatRoomRepository : GenericRepository<ChatRoom>, IChatRoomRepository
    {
        private readonly AppDbContext _appDbContext;
        public ChatRoomRepository(AppDbContext appDbContext, IClaimService claimService, ICurrentTime currentTime) : base(appDbContext, claimService, currentTime)
        {
            _appDbContext = appDbContext;
        }
        public async Task<List<ChatRoomWithOrder>> GetByUserIdAsync(Guid userId)
        {
            // Fetch only the required fields using projection to reduce memory overhead
            var rooms = await _appDbContext.ChatRooms
                .Where(m => (m.SenderId == userId || m.ReceiverId == userId) && m.IsDelete == false)
                .Select(room => new
                {
                    room.Id,
                    room.SenderId,
                    room.ReceiverId,
                    SenderUserName = room.Sender.UserName,
                    ReceiverUserName = room.Receiver.UserName,
                    SenderAvatar = room.Sender.ProfileImage,
                    ReceiverAvatar = room.Receiver.ProfileImage,
                    Messages = room.Messages
                        .Select(m => new
                        {
                            m.Id,
                            m.MessageContent,
                            m.CreatedBy,
                            m.CreationDate
                        }).ToList(),
                    Orders = _appDbContext.Orders
                        .Where(o => o.Post.CreatedBy == room.ReceiverId && o.UserId == room.SenderId)
                        .Select(o => new
                        {
                            o.Id,
                            o.OrderStatusId
                        }).ToList(),
                    LastMessageDate = room.Messages.Max(m => m.CreationDate) // Get the latest message creation date
                })
                .OrderByDescending(room => room.LastMessageDate) // Order by the latest message creation date descending
                .AsNoTracking() // Improves performance for read-only data
                .ToListAsync();

            // Map entities to DTOs
            var roomDtos = rooms.Select(room => new ChatRoomWithOrder
            {
                roomId = room.Id,
                SenderId = room.SenderId,
                ReceiverId = room.ReceiverId,
                SenderName = room.SenderUserName,
                ReceiverName = room.ReceiverUserName,
                SenderAvatar = room.SenderAvatar,
                ReceiverAvatar = room.ReceiverAvatar,
                Messages = room.Messages.Select(message => new MessageDto
                {
                    messageId = message.Id,
                    Content = message.MessageContent,
                    CreatedBy = message.CreatedBy,
                    CreatedDate = message.CreationDate?.ToShortDateString(),
                    CreatedTime = message.CreationDate?.ToShortTimeString()
                }).ToList(),
                Order = room.Orders.Select(order => new OrderDto
                {
                    OrderId = order.Id,
                    OrderStatusId = order.OrderStatusId
                }).ToList()
            }).ToList();

            return roomDtos;
        }

        public async Task<ChatRoomWithOrder> GetMessagesByRoomId(Guid roomId)
        {
            var chatRoom = await _appDbContext.ChatRooms.Where(m => m.Id == roomId).
                                                     Where(x => x.IsDelete == false).
                                                     Include(m => m.Messages)
                                                     .Include(c => c.Sender)
                                                     .Include(c => c.Receiver)
                                                     .FirstOrDefaultAsync();
            if (chatRoom == null)
            {
                return null; 
            }
            var userIds = chatRoom.Messages
                .Where(m => m.CreatedBy.HasValue)
                .Select(m => m.CreatedBy.Value)
                .Distinct()
                .ToList();

            var users = await _appDbContext.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => new { u.UserName, u.ProfileImage });
            var orders = await _appDbContext.Orders
                .Where(o => o.Post.CreatedBy == chatRoom.ReceiverId)
                .Where(o => o.UserId == chatRoom.SenderId).ToListAsync();
            var roomDto = new ChatRoomWithOrder
            {
                roomId = chatRoom.Id,
                SenderId = chatRoom.SenderId,
                ReceiverId = chatRoom.ReceiverId,
                SenderName = chatRoom.Sender.UserName,
                ReceiverName = chatRoom.Receiver.UserName,
                SenderAvatar = chatRoom.Sender.ProfileImage,  
                ReceiverAvatar = chatRoom.Receiver.ProfileImage, 
                Messages = chatRoom.Messages.Select(message => new MessageDto
                {
                    messageId = message.Id,
                    Content = message.MessageContent,
                    CreatedBy = message.CreatedBy,
                    CreatedByUserName = message.CreatedBy.HasValue && users.ContainsKey(message.CreatedBy.Value)
                                        ? users[message.CreatedBy.Value].UserName
                                        : "Unknown User",
                    Avatar = message.CreatedBy.HasValue && users.ContainsKey(message.CreatedBy.Value)
                                        ? users[message.CreatedBy.Value].ProfileImage
                                        : "Unknown Avatar",  
                    CreatedDate = message.CreationDate.Value.ToShortDateString(),
                    CreatedTime = message.CreationDate.Value.ToShortTimeString()
                }).OrderBy(m => m.CreatedDate)
                       .ThenBy(m => m.CreatedTime).ToList(),
                Order = orders.Select(order => new OrderDto
                {
                    OrderId = order.Id,
                    OrderStatusId = order.OrderStatusId
                }).ToList()
            };
            return roomDto;
        }
        
        public async Task<ChatRoomWithOrder> GetRoomBy2UserId(Guid user1, Guid user2)
        {
            var chatRoom = await _appDbContext.ChatRooms.Where(m => (m.SenderId == user1 && m.ReceiverId == user2) ||
                                                                    (m.SenderId == user2 && m.ReceiverId == user1))
                                                                    .Include(m => m.Messages)
                                                                    .Include(c => c.Sender)
                                                                    .Include(c => c.Receiver).FirstOrDefaultAsync();
            if (chatRoom == null)
            {
                return null; // Or handle the case when the chat room is not found
            }

            // Fetch users for all CreatedBy user IDs in the messages
            var userIds = chatRoom.Messages
                .Where(m => m.CreatedBy.HasValue)
                .Select(m => m.CreatedBy.Value)
                .Distinct()
                .ToList();

            var users = await _appDbContext.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => new { u.UserName, u.ProfileImage });
            var orders = await _appDbContext.Orders
                .Where(o => o.Post.CreatedBy == chatRoom.ReceiverId)
                .Where(o => o.UserId == chatRoom.SenderId).ToListAsync();
            var roomDto = new ChatRoomWithOrder
            {
                roomId = chatRoom.Id,
                SenderId = chatRoom.SenderId,
                ReceiverId = chatRoom.ReceiverId,
                SenderName = chatRoom.Sender.UserName,
                ReceiverName = chatRoom.Receiver.UserName,
                SenderAvatar = chatRoom.Sender.ProfileImage,  // Add this line
                ReceiverAvatar = chatRoom.Receiver.ProfileImage,  // Add this line
                Messages = chatRoom.Messages.Select(message => new MessageDto
                {
                    messageId = message.Id,
                    Content = message.MessageContent,
                    CreatedBy = message.CreatedBy,
                    CreatedByUserName = message.CreatedBy.HasValue && users.ContainsKey(message.CreatedBy.Value)
                                        ? users[message.CreatedBy.Value].UserName
                                        : "Unknown User",
                    Avatar = message.CreatedBy.HasValue && users.ContainsKey(message.CreatedBy.Value)
                                        ? users[message.CreatedBy.Value].ProfileImage
                                        : "Unknown Avatar",  // Add this line
                    CreatedDate = message.CreationDate.Value.ToShortDateString(),
                    CreatedTime = message.CreationDate.Value.ToShortTimeString()
                }).OrderBy(m => m.CreatedDate).ToList(),
                Order = orders.Select(order => new OrderDto
                {
                    OrderId = order.Id,
                    OrderStatusId = order.OrderStatusId
                }).ToList()
            };
            return roomDto;
        }
    }
}
