using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace TspShared;

public abstract class SolverDataTransferer
{
    protected IModel? _channel;

    protected ITspSolver _solver;
    protected volatile bool _solverStopped;
    protected volatile bool _solverPaused;
    protected volatile bool _solverInitialized;

    public SolverDataTransferer()
    {
        _solverStopped = false;
        _solverPaused = false;
        _solverInitialized = false;
    }

    public void SendResults(IModel channel, TspResults results)
    {
        string serializedResults = JsonConvert.SerializeObject(results);
        IBasicProperties properties = channel.CreateBasicProperties();
        properties.Headers = new Dictionary<string, object>();
        properties.Headers.Add("type", "data");
        channel.BasicPublish(
            "output",
            "output.1",
            properties,
            Encoding.UTF8.GetBytes(serializedResults)
        );
    }

    private void SendProcessReady()
    {
        string ok = "ok";
        IBasicProperties properties = _channel.CreateBasicProperties();
        properties.Headers = new Dictionary<string, object>();
        properties.Headers.Add("type", "start");
        _channel.BasicPublish(
            "output",
            "output.1",
            properties,
            Encoding.UTF8.GetBytes(ok)
        );
    }
    
    public void Run()
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

        _channel.ExchangeDeclare("input", "topic");
        var queueName6 = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queueName6, "input", "input.#");

        var consumer = new UserInputConsumer(_channel, this);
        _channel.BasicConsume(queueName6, false, consumer);

        _channel.ExchangeDeclare("output", "topic");

        Console.WriteLine("Sending ok signal.");
        SendProcessReady();
        
        while (!_solverInitialized)
        {
            Thread.Sleep(1000);
        }

        while (!_solverInitialized)
        {
        }
    
        RunSolver();
    }

    protected InputData _data;

    protected abstract void RunSolver();

    public void InitSolver(InputData data)
    {
        _data = data;
        Console.WriteLine("Solver initialized");
        _solverInitialized = true;
    }

    public void PauseSolver()
    {
        TspResults tspResults = _solver.Pause();
        SendResults(_channel, tspResults);
        Console.WriteLine("Solver paused");
        _solverPaused = true;
    }

    public void UnpauseSolver()
    {
        _solver.Unpause();
        Console.WriteLine("Solver unpaused");
        _solverPaused = false;
    }

    public void StopSolver()
    {
        _solverStopped = true;
        TspResults tspResults = _solver.Stop();
        SendResults(_channel, tspResults);
        Console.WriteLine("Solver stopped");
    }
    
}