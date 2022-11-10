using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace TspShared;

public class UserInputConsumer : DefaultBasicConsumer
{
    private SolverDataTransferer _dataTransferer;

    public UserInputConsumer(IModel model, SolverDataTransferer dataTransferer) : base(model)
    {
        _dataTransferer = dataTransferer;
    }

    public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange,
        string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
    {
        if (properties.Headers != null)
        {
            string serialized = Encoding.UTF8.GetString(body.ToArray());
            string type = Encoding.UTF8.GetString((byte[])properties.Headers["type"]);
            if (type == "data")
            {
                InputData? data = JsonConvert.DeserializeObject<InputData>(serialized);
                if (data != null)
                {
                    Console.WriteLine("Received input data:");
                    Console.WriteLine($"Phase1Seocnds: {data.Phase1Seconds}; Phase2Seconds: {data.Phase2Seconds}; " +
                                      $"ParallelExecutionsCount: {data.ParallelExecutionsCount}; EpochsCount: {data.EpochsCount}");
                }

               _dataTransferer.InitSolver(data);
            }
            else if (type == "token")
            {
                ActionToken? token = JsonConvert.DeserializeObject<ActionToken>(serialized);
                if (token != null)
                {
                    Console.WriteLine("Received token");
                    if (token.Pause && !token.Stop)
                        _dataTransferer.PauseSolver();
                    if (!token.Pause && !token.Stop)
                        _dataTransferer.UnpauseSolver();
                    if (token.Stop)
                        _dataTransferer.StopSolver();
                }
            }
        }

        System.Threading.Thread.Sleep(2000);
        Model.BasicAck(deliveryTag, false);
    }
}