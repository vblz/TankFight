using System;
using System.Linq;
using GameLogic.Enums;
using GameLogic.Implementations.Exceptions;
using GameLogic.Implementations.GameObjects;
using GameLogic.Implementations.Map;
using GameLogic.Implementations.Public;
using GameLogic.Implementations.Services;
using GameLogic.Interfaces.Public;
using GameLogic.Interfaces.Services;

namespace GameLogic.Implementations.Game
{
	public sealed class Game
	{
		private readonly byte actionPoints;

		private readonly IMoveService moveService;
		private readonly IBulletService bulletService;
		private readonly IMapAdapter mapAdapter;

		public IGameState State => new GameState(this.mapAdapter.GetState(),
			this.bulletService.Bullets);

		public void Tick(IUserMove[] moves)
		{
			for (int i = 0; i < this.actionPoints; ++i)
			{
				var stageMoves = moves
					.Where(x => x.Actions.Count > i && x.Actions[i] != null)
					.Select(x => new { x.UserId, Action = x.Actions[i] })
					.ToArray();

				foreach (var move in stageMoves)
				{
					try
					{
						this.ProcessUserMove(move.UserId, move.Action);
					}
					catch (UserNotFoundException)
					{
						// обработали мертвого пользователя
						// TODO log
					}
				}
			
				this.bulletService.Process();
			}
		}

		private void ProcessUserMove(string userId, IAction action)
		{
			if (action == null)
			{
				throw new ArgumentNullException(nameof(action));
			}

			switch (action.Type)
			{
				case UserActionType.Move:
					this.moveService.Move(userId, action.Direction);
					break;
				
				case UserActionType.Shoot:
					this.bulletService.CreateBullet(userId, action.Direction);
					break;
				
				default:
					throw new NotSupportedException();
			}
		}

		public Game(string[] userIds, IMapInfo mapInfo, IGameSettings settings)
		{
			if (userIds == null)
			{
				throw new ArgumentNullException(nameof(userIds));
			}
			if (mapInfo == null)
			{
				throw new ArgumentNullException(nameof(mapInfo));
			}
			if (settings == null)
			{
				throw new ArgumentNullException(nameof(settings));
			}

			this.actionPoints = settings.ActionPoints;
			
			var tanks = userIds.Select(x => new Tank(new Health(settings.TankHealthPoint), x)).ToArray();
			
			var map = new Map(new MapCreationData(mapInfo, tanks), new BattlefieldBuilder(),
				new RandomSpawnService());
			// намерено скрываем типы используемых интерфейсов внутри класса игры
			this.mapAdapter = new MapAdapter(map);
			this.bulletService = new BulletService(this.mapAdapter, settings.BulletActionPoints);
			this.moveService = new MoveService(this.mapAdapter);
		}
	}
}