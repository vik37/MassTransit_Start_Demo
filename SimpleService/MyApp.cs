using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleService
{
    public class MyApp
    {
        private  ILogger<MyApp> _logger;
        private  IConfiguration _configuration;

        public MyApp(ILogger<MyApp> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public Task<int> StartAsync()
        {
            Console.WriteLine($"Hello World Viktor {_configuration["App:Value1"]} - {_configuration["App:Value2"]}");
            _logger.LogInformation("This is a log message.");

            return Task.FromResult(0);
        }
    }
}
