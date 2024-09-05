using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.OrderModel
{
    public class CreateOrderModel
    {
        public Guid PostId { get; set; }
        public Guid BuyerId { get; set; }
        public int Quantity { get; set; }
    }
}
