using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ViewModel.SubcriptionModel
{
    public class UpdateSubscriptionModel
    {
        public Guid Id { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "Price must be greater than 1.")]
        public long Price { get; set; }
        public string SubcriptionType { get; set; }
        public int  ExpiryDay { get; set; }
    }
}
