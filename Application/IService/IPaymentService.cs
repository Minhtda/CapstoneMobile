using Application.VnPay.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.InterfaceService
{
    public interface IPaymentService
    {
        public string GetPayemntUrl(int choice);
        public Task<bool> HandleIpn(VnPayResponse vnPayResponse);
        public Task<bool> BuySubscription(Guid subscriptionId);
        public Task<bool> ExtendSubscriptionByUserId(Guid userId);
    }
}
