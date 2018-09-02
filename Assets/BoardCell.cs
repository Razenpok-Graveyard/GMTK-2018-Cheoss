using System.Collections;
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
    [SerializeField] private Image sigil;

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
            image.color = AffectingGod.CellColor(this);
            return;
        }
        image.color = IsDark ? Color.black : Color.white;
    }

    public bool IsDark
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

    public void ShowSigil(Sprite sprite)
    {
        StartCoroutine(ShowSigilAndFadeOut(sprite));
    }

    private IEnumerator ShowSigilAndFadeOut(Sprite sprite)
    {
        sigil.sprite = sprite;
        var color = sigil.color;
        color.a = 1f;
        sigil.color = color;
        yield return new WaitForSeconds(1f);
        foreach (var f in Easing.Linear(1, 0, 1f))
        {
            color = sigil.color;
            color.a = f;
            sigil.color = color;
            yield return null;
        }
    }
}