using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Figures
{
    public class Queen : ChessFigure
    {
        public override IEnumerable<BoardCell> MovementCells
        {
            get
            {
                return AvailableRayMovementCells(Vector2Int.up)
                    .Concat(AvailableRayMovementCells(Vector2Int.down))
                    .Concat(AvailableRayMovementCells(Vector2Int.left))
                    .Concat(AvailableRayMovementCells(Vector2Int.right))
                    .Concat(AvailableRayMovementCells(Vector2Int.right + Vector2Int.up))
                    .Concat(AvailableRayMovementCells(Vector2Int.right + Vector2Int.down))
                    .Concat(AvailableRayMovementCells(Vector2Int.left + Vector2Int.down))
                    .Concat(AvailableRayMovementCells(Vector2Int.left + Vector2Int.up));
            }
        }

        public override int Value
        {
            get { return 100; }
        }
    }
}