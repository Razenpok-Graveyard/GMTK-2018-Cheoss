using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Figures
{
    public abstract class ChessFigure :
        MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerDownHandler,
        IPointerUpHandler
    {
        private bool isDragged;

        public event Action DragStarted;
        public event Action DragEnded;

        public BoardCell Cell { get; set; }

        public Player Player { get; set; }

        public int Rotation
        {
            get { return Player.IsFirst ? 0 : 180; }
        }

        public bool IsInteractable
        {
            get { return Image.raycastTarget; }
            set { Image.raycastTarget = value; }
        }

        private Image Image
        {
            get { return GetComponent<Image>(); }
        }

        protected Vector2Int Position
        {
            get { return Cell.Position; }
        }

        private void Update()
        {
            if (Cell != null)
            {
                transform.position = Cell.transform.position;
            }

            transform.rotation = Quaternion.Euler(0, 0, Rotation);
            Image.color = new Color(0.75f, 0.75f, 0.75f) * Player.God.Color;

            if (isDragged)
            {
                var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                position.z = transform.position.z;
                transform.position = position;
            }
        }

        public abstract IEnumerable<BoardCell> MovementCells { get; }

        protected IEnumerable<BoardCell> AvailableRayMovementCells(Vector2Int direction)
        {
            BoardCell cell;
            var distance = 1;
            do
            {
                if (!Board.TryGetCell(Position + direction * distance++, out cell))
                {
                    break;
                }

                if (cell.HasFigure && cell.Figure.Player == Player)
                {
                    break;
                }

                yield return cell;
            } while (!cell.HasFigure);
        }

        protected IEnumerable<BoardCell> AvailableMovementCells(IEnumerable<Vector2Int> positions)
        {
            foreach (var position in positions)
            {
                BoardCell cell;
                if (Board.TryGetCell(position, out cell) && !(cell.HasFigure && cell.Figure.Player == Player))
                {
                    yield return cell;
                }
            }
        }

        protected Board Board
        {
            get { return FindObjectOfType<Board>(); }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsInteractable)
            {
                return;
            }
            MarkPossibleMovement();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsInteractable)
            {
                return;
            }
            UnmarkPossibleMovement();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsInteractable && !isDragged)
            {
                return;
            }

            isDragged = true;
            transform.SetSiblingIndex(transform.childCount - 1);
            if (DragStarted != null) DragStarted();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!IsInteractable && !isDragged)
            {
                return;
            }

            UnmarkPossibleMovement();
            isDragged = false;
            if (DragEnded != null) DragEnded();
        }

        private void MarkPossibleMovement()
        {
            foreach (var cell in MovementCells)
            {
                cell.AffectingGod = Player.God;
            }
        }

        private void UnmarkPossibleMovement()
        {
            foreach (var cell in MovementCells)
            {
                cell.AffectingGod = null;
            }
        }

        public static implicit operator Player(ChessFigure figure)
        {
            return figure.Player;
        }

        public abstract int Value { get; }
    }
}