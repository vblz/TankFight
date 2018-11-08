using System;
using System.Threading;
using Newtonsoft.Json;
using RandomBot.Models;

namespace RandomBot
{
    class Program
    {
        private const int MovePercent = 70;
        private static readonly Random random = new Random();

        private static readonly CancellationTokenSource cts = new CancellationTokenSource();
        
        private static void Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eventArgs) => cts.Cancel();
            var ct = cts.Token;
            int moveNumber = 0;
            
            while (!ct.IsCancellationRequested)
            {
                var state = Console.ReadLine();
                Thread.Sleep(50);
                Console.WriteLine(JsonConvert.SerializeObject(new UserAction[]
                {
                    new UserAction()
                    {
                        Direction = (Direction)random.Next(4),
                        Type = random.Next(100) > MovePercent ? UserActionType.Shoot : UserActionType.Move
                    }
                }));
                Console.Error.WriteLine("Move number {0}", moveNumber++);
            }
        }
    }
}