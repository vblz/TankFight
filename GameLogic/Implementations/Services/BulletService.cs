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
		
		public IDestroyedInfo Process()
		{
			var destroyedBullets = new Dictionary<string, Coordinates>();
			var destroyedObjects = new List<ICellContentInfo>();
			
			for (byte i = 0; i < this.bulletActionPointCount; ++i)
			{
				var firstObjects = this.CheckHits();
				this.MoveBullets();
				var secondObjects = this.CheckHits();

				destroyedBullets = destroyedBullets
					.Concat(firstObjects.DestroyedBullets)
					.Concat(secondObjects.DestroyedBullets)
					.GroupBy(x => x.Key)
					.ToDictionary(x => x.Key, x => x.First().Value);
				
				destroyedObjects.AddRange(firstObjects.DestroyedObjects);
				destroyedObjects.AddRange(secondObjects.DestroyedObjects);
			}
			
			return new DestroyedInfo(destroyedBullets, destroyedObjects);
		}

		public void UserShoot(string userId, Direction direction)
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

		private IDestroyedInfo CheckHits()
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

			var destroyedBullets = collidedBulletsKeys
				.ToDictionary(x => x, x => this.activeBulletsByUsers[x].Coordinates);

			foreach (var collidedBulletKey in collidedBulletsKeys)
			{
				this.activeBulletsByUsers.Remove(collidedBulletKey);
			}
			
			var destroyedObjects = this.mapAdapter.ClearDeadCells();
			
			return new DestroyedInfo(destroyedBullets, destroyedObjects.ToList());
		}

		public BulletService(IMapAdapter mapAdapter, byte bulletActionPointCount)
		{
			this.mapAdapter = mapAdapter;
			this.bulletActionPointCount = bulletActionPointCount;
		}
	}
}