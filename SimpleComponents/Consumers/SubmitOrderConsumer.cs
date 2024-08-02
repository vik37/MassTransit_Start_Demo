using SimpleContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace SimpleComponents.Consumers
{
    public class SubmitOrderConsumer : IConsumer<SubmitOrder>
    {
        private readonly ILogger<SubmitOrderConsumer> _logger;

        public SubmitOrderConsumer(ILogger<SubmitOrderConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<SubmitOrder> context)
        {
            _logger.LogInformation("Submit Order For Customer: {SubmitOrder}", context.Message.CustomerNumber);

            if (context.Message.CustomerNumber.Contains("TEST"))
            {
                if (context.RequestId != null) 
                {
                    await context.RespondAsync<OrderSubmitionRejected>(new
                    {
                        InVar.Timestamp,
                        context.Message.CustomerNumber,
                        context.Message.OrderId,
                        Reason = $"Invalid Customer Number: {context.Message.CustomerNumber}"
                    });
                }

                return;
            }

            if (context.RequestId != null) 
            {
                await context.RespondAsync<OrderSubmitionAccepted>(new
                {
                    InVar.Timestamp,
                    context.Message.CustomerNumber,
                    context.Message.OrderId
                });
            }
        }
    }
}
