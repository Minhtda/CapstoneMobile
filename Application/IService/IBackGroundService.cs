using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IService
{
    public interface IBackGroundService
    {
        Task<bool> ExtendSubscription();
        Task<bool> RemovePostWhenSubscriptionExpire();
        Task<bool> ExtendSubscriptionByUserId(Guid userId);
        Task<bool> RemovePostWhenSubscriptionExpireByUserId(Guid userId);
    }
}
