using System;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using RuBot.Models;

namespace RuBot
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
            const string botName = "vblz/tanks:rubot";

            while (!ct.IsCancellationRequested)
            {
                var state = Console.ReadLine();
                var gameState = JsonConvert.DeserializeObject<GameState>(state);
                var myTank = gameState.ContentsInfo.First(c => c.Type == CellContentType.Tank && c.UserId == botName).Coordinates;
                var enemyTank = gameState.ContentsInfo.First(c => c.Type == CellContentType.Tank && c.UserId != botName).Coordinates;
                var dX = myTank.X - enemyTank.X;
                var dY = myTank.Y - enemyTank.Y;
                var type = UserActionType.Move;

                var direction = Math.Abs(dX) > Math.Abs(dY)
                    ? (dX < 0 ? Direction.Right : Direction.Left)
                    : (dY < 0 ? Direction.Up : Direction.Down);

                if (random.Next(100) < 70 && (myTank.X == enemyTank.X || myTank.Y == enemyTank.Y))
                {
                    type = UserActionType.Shoot;
                }
                else
                {
                    if ((gameState.ContentsInfo.FirstOrDefault(c =>
                             c.Coordinates.X == (myTank.X - 1) && c.Coordinates.Y == myTank.Y) != null) &&
                        (direction == Direction.Left)) direction = Direction.Down;
                    if ((gameState.ContentsInfo.FirstOrDefault(c =>
                             c.Coordinates.X == myTank.X && c.Coordinates.Y == (myTank.Y - 1)) != null) &&
                        (direction == Direction.Down)) direction = Direction.Right;
                    if ((gameState.ContentsInfo.FirstOrDefault(c =>
                             c.Coordinates.X == (myTank.X + 1) && c.Coordinates.Y == myTank.Y) != null) &&
                        (direction == Direction.Right)) direction = Direction.Up;
                    if ((gameState.ContentsInfo.FirstOrDefault(c =>
                             c.Coordinates.X == myTank.X && c.Coordinates.Y == (myTank.Y + 1)) != null) &&
                        (direction == Direction.Up)) direction = Direction.Left;
                    if (random.Next(100) > 70) direction = (Direction) random.Next(4);
                    type = random.Next(100) > MovePercent ? UserActionType.Shoot : UserActionType.Move;
                }

                Console.WriteLine(JsonConvert.SerializeObject(new UserAction[]
                {
                    new UserAction()
                    {
                        Direction = direction,
                        Type = type
                    }
                }));
            }
        }
    }
}