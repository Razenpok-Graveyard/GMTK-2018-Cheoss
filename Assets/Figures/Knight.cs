using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Figures
{
    public class Knight : ChessFigure
    {
        public override IEnumerable<BoardCell> MovementCells
        {
            get
            {
                var offsets = new[]
                {
                    new Vector2Int(-2, -1),
                    new Vector2Int(-1, -2),
                    new Vector2Int(2, -1),
                    new Vector2Int(1, -2),
                    new Vector2Int(-2, 1),
                    new Vector2Int(-1, 2),
                    new Vector2Int(2, 1),
                    new Vector2Int(1, 2),
                };
                var positions = offsets.Select(offset => Position + offset);
                return AvailableMovementCells(positions);
            }
        }

        public override int Value
        {
            get { return 20; }
        }
    }
}