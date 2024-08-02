using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleComponents.Consumers
{
    public interface OrderSubmitionAccepted
    {
        public Guid OrderId { get; }

        public DateTime Timestamp { get; }

        public string CustomerNumber { get; }
    }
}
