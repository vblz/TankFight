using System;
using GameLogic.Implementations.Public;
using GameLogic.Interfaces.Map;

namespace GameLogic.Implementations.Map
{
	internal sealed class Cell : ICell
	{
		public ICellContent Content { get; private set; }
		public Coordinates Coordinates { get; }
		public bool IsEmpty => this.Content == null;

		public void Put(ICellContent content)
		{
			if (!this.IsEmpty)
			{
				throw new InvalidOperationException();
			}

			this.Content = content;
		}

		public ICellContent Pop()
		{
			if (this.IsEmpty)
			{
				throw new InvalidOperationException();
			}

			var result = this.Content;
			this.Content = null;
			return result;
		}

		public Cell(ICellContent content, Coordinates coords)
		{
			this.Coordinates = coords;
			this.Put(content);
		}

		public Cell(Coordinates coords) : this(null, coords)
		{
		}
	}
}