using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SimpleAPI.OptionsModel;
using SimpleComponents.Consumers;
using SimpleContracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<RabbitMQConfig>().Bind(builder.Configuration.GetSection("RabbitMQConfig"));

//builder.Services.AddMediator(cf =>
//{
//    cf.AddConsumer<SubmitOrderConsumer>();
//    cf.AddRequestClient<OrderSubmitionAccepted>();
//});

builder.Services.AddMassTransit(cfg =>
{ 
    cfg.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
    cfg.UsingRabbitMq((context, conf) =>
    {
        
        var rabbit = context.GetService<IOptions<RabbitMQConfig>>().Value;
        conf.Host(rabbit.HostAddress,  rabbit.VirtualHost,h =>
        {
            h.Username(rabbit.Username);
            h.Password(rabbit.Password);
        });

        conf.ConfigureEndpoints(context);
    });

    //cfg.AddConsumer<SubmitOrderConsumer>();
    cfg.AddRequestClient<OrderSubmitionAccepted>(new Uri($"exchange:{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
