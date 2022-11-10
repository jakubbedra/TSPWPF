using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using TspShared;

namespace TSPWPF.ViewModel.Helper;

public class ResultsConsumer : DefaultBasicConsumer
{
    private MainViewModel _mainViewModel;

    public ResultsConsumer(IModel model, MainViewModel mainViewModel) : base(model)
    {
        _mainViewModel = mainViewModel;
    }

    public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange,
        string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
    {
        string serialized = Encoding.UTF8.GetString(body.ToArray());
        string type = Encoding.UTF8.GetString((byte[])properties.Headers["type"]);
        if (type == "data")
        {
            TspResults? results = JsonConvert.DeserializeObject<TspResults>(serialized);
            if (results != null)
            {
                _mainViewModel.UpdateResults(results);
            }
        }
        else if (type == "start")
        {
            _mainViewModel.ChildProcessStarted();
        }
        
        System.Threading.Thread.Sleep(100);
        Model.BasicAck(deliveryTag, false);
    }
}