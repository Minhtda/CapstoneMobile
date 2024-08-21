using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class MobileToken:BaseEntity
    {
        public string DeviceName { get; set; }
        public string TokenString { get; set; }
    }
}
