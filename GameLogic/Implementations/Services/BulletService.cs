using System.Collections.Generic;
using System.Linq;
using GameLogic.Enums;
using GameLogic.Implementations.GameObjects;
using GameLogic.Implementations.Public;
using GameLogic.Interfaces.Public;
using GameLogic.Interfaces.Services;

namespace GameLogic.Implementations.Services
{
	internal sealed class BulletService : IBulletService
	{
		private readonly Dictionary<string, Bullet> activeBulletsByUsers = new Dictionary<string, Bullet>();
		private readonly IMapAdapter mapAdapter;
		private readonly byte bulletActionPointCount;

		public IReadOnlyCollection<IBulletInfo> Bullets => this.activeBulletsByUsers
			.Select(x => new BulletInfo(x.Value.Coordinates, x.Value.Direction, x.Key))
			.Cast<IBulletInfo>()
			.ToList()
			.AsReadOnly();
		
		public void Process()
		{
			for (byte i = 0; i < this.bulletActionPointCount; ++i)
			{
				this.CheckHits();
				this.MoveBullets();
				this.CheckHits();
			}
		}

		public void CreateBullet(string userId, Direction direction)
		{
			var moveDirection = this.mapAdapter.MoveDirection(userId, direction);

			if (this.activeBulletsByUsers.ContainsKey(userId))
			{
				// На одного игрока одна пуля. Просто игнорируем.
				return;
			}
			
			this.activeBulletsByUsers.Add(userId, new Bullet(direction, moveDirection.ToCell.Coordinates));
		}

		private void MoveBullets()
		{
			foreach (var bullet in this.activeBulletsByUsers.Values)
			{
				bullet.Move();
			}
		}

		private void CheckHits()
		{
			var collidedBulletsKeys = new LinkedList<string>();
			foreach (var bulletRecord in this.activeBulletsByUsers)
			{
				// как видно, пересечение пуль не считаем, они пролетаются мимо друг друга
				var onCell = this.mapAdapter.GetCell(bulletRecord.Value.Coordinates);
				if (!onCell.IsEmpty)
				{
					var collisionResult = onCell.Content.ProcessShoot();
					if (collisionResult == BulletCollisionResult.Destroy)
					{
						// Как было бы удобно удалять внутри foreach
						collidedBulletsKeys.AddFirst(bulletRecord.Key);	
					}
				}
			}

			foreach (var collidedBulletKey in collidedBulletsKeys)
			{
				this.activeBulletsByUsers.Remove(collidedBulletKey);
			}
			
			this.mapAdapter.ClearDeadCells();
		}

		public BulletService(IMapAdapter mapAdapter, byte bulletActionPointCount)
		{
			this.mapAdapter = mapAdapter;
			this.bulletActionPointCount = bulletActionPointCount;
		}
	}
}