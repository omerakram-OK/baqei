using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Baqei.Infrastructure.RabbitMq;

public class RabbitMqService : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _exchangeName = "baqei.exchange";
    private readonly string _queueName = "baqei.queue";
    private readonly string _routingKey = "baqei.key";

    public RabbitMqService(string hostName, int port, string userName, string password)
    {
        var factory = new ConnectionFactory
        {
            HostName = hostName,
            Port = port,
            UserName = userName,
            Password = password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Direct, durable: true, autoDelete: false);
        _channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(queue: _queueName, exchange: _exchangeName, routingKey: _routingKey);
    }

    public Task PublishAsync(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        _channel.BasicPublish(exchange: _exchangeName, routingKey: _routingKey, basicProperties: properties, body: body);
        return Task.CompletedTask;
    }

    public void StartConsumer(Func<string, Task> handleMessage, CancellationToken cancellationToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                await handleMessage(message);
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception)
            {
                _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
    }

    public void Dispose()
    {
        try
        {
            _channel?.Close();
            _connection?.Close();
        }
        catch
        {
            // ignore
        }
    }
}
