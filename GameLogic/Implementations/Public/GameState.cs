﻿using System.Collections.Generic;
using GameLogic.Interfaces.Public;

namespace GameLogic.Implementations.Public
{
	internal sealed class GameState : IGameState
	{
		public IReadOnlyCollection<ICellContentInfo> ContentsInfo { get; }
		public IReadOnlyCollection<IBulletInfo> BulletsInfo { get; }

		public GameState(IReadOnlyCollection<ICellContentInfo> contentsInfo, IReadOnlyCollection<IBulletInfo> bulletsInfo)
		{
			this.ContentsInfo = contentsInfo;
			this.BulletsInfo = bulletsInfo;
		}
	}
}