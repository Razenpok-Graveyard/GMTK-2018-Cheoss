using Figures;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardCell :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private Image image;
    [SerializeField] private ChessFigure figure;

    public Vector2Int Position { get; set; }

    public God AffectingGod { get; set; }

    public bool IsMouseOver { get; private set; }

    public ChessFigure Figure
    {
        get { return figure; }
        set
        {
            figure = value;
            if (figure != null)
            {
                figure.Cell = this;
            }
        }
    }

    public bool HasFigure
    {
        get { return Figure != null; }
    }

    private int Row
    {
        get { return Position.x; }
    }

    private int Column
    {
        get { return Position.y; }
    }

    private void Update()
    {
        if (AffectingGod != null)
        {
            if (IsDark)
            {
                image.color = Color.gray * AffectingGod.Color;
            }
            else
            {
                float h, s, v;
                Color.RGBToHSV(AffectingGod.Color, out h, out s, out v);
                s *= 0.75f;
                image.color = Color.HSVToRGB(h, s, v);
            }
            return;
        }
        image.color = IsDark ? Color.black : Color.white;
    }

    private bool IsDark
    {
        get
        {
            return Row % 2 == Column % 2;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsMouseOver = false;
    }
}