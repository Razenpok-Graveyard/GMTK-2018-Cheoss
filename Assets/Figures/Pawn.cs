using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Figures
{
    public class Pawn : ChessFigure
    {
        public override IEnumerable<BoardCell> MovementCells
        {
            get
            {
                var direction = Player.IsFirst ? 1 : -1;
                var attackOffsets = new[]
                {
                    new Vector2Int(-1, 1),
                    new Vector2Int(1, 1),
                };
                var attackPositions = attackOffsets
                    .Select(offset => Position + offset * direction);
                var movementCells = AvailableMovementCells(attackPositions)
                    .Where(cell => cell.HasFigure)
                    .ToList();

                BoardCell movementCell;
                var canMoveForward =
                    Board.TryGetCell(Position + Vector2Int.up * direction, out movementCell) &&
                    !movementCell.HasFigure;
                if (!canMoveForward)
                {
                    return movementCells;
                }
                movementCells.Add(movementCell);
                var startingRow = Player.IsFirst ? 1 : 6;
                var canMakeAdditionalMove =
                    Position.y == startingRow &&
                    Board.TryGetCell(Position + Vector2Int.up * direction * 2, out movementCell) &&
                    !movementCell.HasFigure;
                if (canMakeAdditionalMove)
                {
                    movementCells.Add(movementCell);
                }
                return movementCells;
            }
        }

        public int FinishRow
        {
            get { return Player.IsFirst ? 7 : 0; }
        }

        public override int Value
        {
            get { return 10; }
        }
    }
}