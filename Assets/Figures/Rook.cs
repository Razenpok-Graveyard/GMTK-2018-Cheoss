using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Figures
{
    public class Rook : ChessFigure
    {
        public override IEnumerable<BoardCell> MovementCells
        {
            get
            {
                return AvailableRayMovementCells(Vector2Int.up)
                    .Concat(AvailableRayMovementCells(Vector2Int.down))
                    .Concat(AvailableRayMovementCells(Vector2Int.left))
                    .Concat(AvailableRayMovementCells(Vector2Int.right));
            }
        }

        public override int Value
        {
            get { return 40; }
        }
    }
}