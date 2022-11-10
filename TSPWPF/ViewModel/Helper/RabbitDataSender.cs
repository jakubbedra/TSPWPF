using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using TspShared;

namespace TSPWPF.ViewModel.Helper;

public class RabbitDataSender
{
    private ResultsConsumer _consumer;

    private IModel _channel;   
    
    public RabbitDataSender(MainViewModel _mainViewModel)
    {
        var factory = new ConnectionFactory()
        {
            UserName = "guest",
            Password = "guest",
            HostName = "localhost",
            VirtualHost = "tsp"
        };
        var connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        
        _channel.ExchangeDeclare("output", "topic");
        var queueName7 = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queueName7, "output", "output.#");
        
        _consumer = new ResultsConsumer(_channel, _mainViewModel);
        _channel.BasicConsume(queueName7, false, _consumer);
        
        _channel.ExchangeDeclare("input", "topic");
    }
    
    public void SendStart(InputData data)
    {
        string serializedData = JsonConvert.SerializeObject(data);
        IBasicProperties prop = _channel.CreateBasicProperties();
        prop.Headers = new Dictionary<string, object>();
        prop.Headers.Add("type", "data");
        _channel.BasicPublish(
            "input",
            "input.1",
            prop,
            Encoding.UTF8.GetBytes(serializedData)
        );
    }
    
    public void SendSwitchPause(bool pause)
    {
        SendToken(pause, false);
    }

    public void SendEnd()
    {
        SendToken(true, true);
    }
    
    private void SendToken(bool pause, bool stop)
    {
        ActionToken actionToken = new ActionToken()
        {
            Pause = pause,
            Stop = stop
        };
        string serialized = JsonConvert.SerializeObject(actionToken);
        IBasicProperties properties = _channel.CreateBasicProperties();
        properties.Headers = new Dictionary<string, object>();
        properties.Headers.Add("type", "token");
        _channel.BasicPublish(
            "input",
            "input.1",
            properties,
            Encoding.UTF8.GetBytes(serialized)
        );
    }
    
}