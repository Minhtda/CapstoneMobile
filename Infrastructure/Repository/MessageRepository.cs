using Application.InterfaceRepository;
using Application.InterfaceService;
using Application.Service;
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
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ICurrentTime _currentTime;
        public MessageRepository(AppDbContext appDbContext, IClaimService claimService, ICurrentTime currentTime) : base(appDbContext, claimService, currentTime)
        {
            _appDbContext = appDbContext;
            _currentTime = currentTime;
        }

        public async Task<Message> getByContent(string messageContent)
        {
            var messages = await _appDbContext.Messages
                .Where(m => m.MessageContent == messageContent)
                .Where(m => m.IsDelete == false).FirstOrDefaultAsync();
            return messages;
        }

        public async Task<List<Message>> GetMessagesBy2UserId(Guid user1, Guid user2)
        {
           /* var messages = await _appDbContext.Messages.Where(m => (m.SenderId == user1 && m.ReceiverId == user2) ||
                                                                    (m.SenderId == user2 && m.ReceiverId == user1)).
                                                                    Where(x => x.IsDelete == false).ToListAsync();
            return messages;*/
           throw new NotImplementedException();
        }
        public async Task AddMessageAsync(Message entity)
        {
            entity.CreationDate = _currentTime.GetCurrentTime();
            entity.IsDelete = false;
            await _dbSet.AddAsync(entity);
        }
    }
}