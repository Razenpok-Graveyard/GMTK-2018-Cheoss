﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Figures;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    private const int Width = 8;
    private const int Height = 8;

    [SerializeField] private ChessFigure pawn;
    [SerializeField] private ChessFigure rook;
    [SerializeField] private ChessFigure knight;
    [SerializeField] private ChessFigure bishop;
    [SerializeField] private ChessFigure queen;
    [SerializeField] private ChessFigure king;

    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private GameObject figureContainer;

    private bool isGameOverYet;

    private readonly BoardCell[,] cells = new BoardCell[Height, Width];
    private readonly Player firstPlayer = new Player { IsFirst = true, IsBot = true };
    private readonly Player secondPlayer = new Player { IsBot = true };

    private void Start()
    {
        ResetPositions();
        mainMenu.StartPressed += MainMenuStartPressed;
        mainMenu.FadedOut += MainMenuFadedOut;
    }

    private void MainMenuStartPressed()
    {
        firstPlayer.God = mainMenu.FirstPlayerSettings.SelectedGod;
        firstPlayer.IsBot = mainMenu.FirstPlayerSettings.IsBot;
        secondPlayer.God = mainMenu.SecondPlayerSettings.SelectedGod;
        secondPlayer.IsBot = mainMenu.SecondPlayerSettings.IsBot;
        ResetBoard();
    }

    private void MainMenuFadedOut()
    {
        StartCoroutine(StartNextTurn(firstPlayer, true));
    }

    private void ResetBoard()
    {
        isGameOverYet = false;
        firstPlayer.ClearFigures();
        secondPlayer.ClearFigures();
        SpawnFigures();
    }

    private void SpawnFigures()
    {
        SpawnRoyalRow(0, firstPlayer);
        SpawnPawnRow(1, firstPlayer);
        SpawnPawnRow(6, secondPlayer);
        SpawnRoyalRow(7, secondPlayer);
    }

    private void SpawnRoyalRow(int row, Player player)
    {
        Spawn(rook, row, 0, player);
        Spawn(knight, row, 1, player);
        Spawn(bishop, row, 2, player);
        Spawn(queen, row, 3, player);
        Spawn(king, row, 4, player);
        Spawn(bishop, row, 5, player);
        Spawn(knight, row, 6, player);
        Spawn(rook, row, 7, player);
    }

    private void SpawnPawnRow(int row, Player player)
    {
        for (int i = 0; i < Width; i++)
        {
            Spawn(pawn, row, i, player);
        }
    }

    private SfxPlayer SfxPlayer { get { return FindObjectOfType<SfxPlayer>(); } }

    private void Spawn(ChessFigure figure, int row, int column, Player player)
    {
        var cell = cells[column, row];
        var go = Instantiate(figure, figureContainer.transform);
        cell.Figure = go;
        player.AddActiveFigure(go);
        go.Player = player;
        go.DragStarted += () => Figure_DragStarted(go);
        go.DragEnded += () => Figure_DragEnded(go);
    }

    private void Figure_DragEnded(ChessFigure figure)
    {
        var targetCell = cells.Cast<BoardCell>()
            .FirstOrDefault(cell => cell.IsMouseOver);
        if (targetCell == null || targetCell == figure.Cell || !figure.MovementCells.Contains(targetCell))
        {
            figure.Player.IsActive = true;
            return;
        }

        Move(figure, targetCell);
        StartCoroutine(StartNextTurn(OppositePlayer(figure)));
    }

    private void Move(ChessFigure figure, BoardCell targetCell)
    {
        figure.Cell.Figure = null;
        if (targetCell.HasFigure)
        {
            var targetFigure = targetCell.Figure;
            targetFigure.Player.AddDeadFigure(targetFigure);
            if (targetFigure is King)
            {
                isGameOverYet = true;
                SfxPlayer.PlayGameOver();
            }
            else if (targetFigure.Player.IsBot)
            {
                SfxPlayer.PlayPlayerKill();
            }
            else
            {
                SfxPlayer.PlayBotKill();
            }
            Destroy(targetFigure.gameObject);
        }
        else
        {
            SfxPlayer.PlayDragStopped();
        }
        targetCell.Figure = figure;
    }

    private IEnumerator ShuffleFigures(Player player)
    {
        SfxPlayer.PlayWarp();
        var teamFigures = player.ActiveFigures.Where(f => !(f is King)).ToList();
        var teamCells = teamFigures.Select(f => f.Cell).ToList();
        foreach (var f in Easing.Linear(0, 1, 0.3f))
        {
            foreach (var figure in teamFigures)
            {
                figure.transform.localScale = Vector3.one * (1 - f);
                figure.transform.rotation = Quaternion.Euler(0, 0, f * 180 + figure.Rotation);
            }

            yield return null;
        }

        Shuffle(teamFigures);
        Shuffle(teamCells);
        for (var i = 0; i < teamFigures.Count; i++)
        {
            teamCells[i].Figure = teamFigures[i];
        }

        foreach (var f in Easing.Linear(0, 1, 0.3f))
        {
            foreach (var figure in teamFigures)
            {
                figure.transform.localScale = Vector3.one * f;
                figure.transform.rotation = Quaternion.Euler(0, 0, 180 + f * 180 + figure.Rotation);
            }

            yield return null;
        }
    }

    private static readonly System.Random random = new System.Random();

    private static void Shuffle<T>(IList<T> list)
    {
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = random.Next(n + 1);
            var value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void Figure_DragStarted(ChessFigure figure)
    {
        SfxPlayer.PlayDragStarted();
        figure.Player.IsActive = false;
    }

    private IEnumerator StartNextTurn(Player player, bool isFirstTurn = false)
    {
        player.IsActive = false;
        OppositePlayer(player).IsActive = false;
        while (true)
        {
            if (isGameOverYet)
            {
                yield return new WaitForSeconds(2f);
                mainMenu.Show();
                yield break;
            }

            if (!isFirstTurn)
            {
                yield return new WaitForSeconds(0.6f);
                yield return StartCoroutine(ShuffleFigures(OppositePlayer(player)));
            }
            if (!player.IsBot) break;
            yield return StartCoroutine(FakeThinking(player));
            yield return StartCoroutine(MakeBotTurn(player));
            player = OppositePlayer(player);
        }

        player.IsActive = true;
    }

    private static IEnumerator FakeThinking(Player player)
    {
        var thinking = player.King.Thinking;
        thinking.gameObject.SetActive(true);
        foreach (var f in Easing.OutSquare(1, 0, 2 + Random.value))
        {
            thinking.fillAmount = f;
            yield return null;
        }
        thinking.gameObject.SetActive(false);
    }

    private IEnumerator MakeBotTurn(Player player)
    {
        var attackingFigures = player.ActiveFigures
            .Where(f => f.MovementCells.Any(c => c.HasFigure))
            .ToList();
        if (attackingFigures.Any())
        {
            var mostValuableAttacker = attackingFigures
                .OrderByDescending(f => f.MovementCells
                    .Where(c => c.HasFigure)
                    .Select(c => c.Figure.Value)
                    .Max())
                .First();
            var mostValuableTarget = mostValuableAttacker.MovementCells
                .Where(c => c.HasFigure)
                .OrderByDescending(c => c.Figure.Value)
                .First();
            yield return StartCoroutine(AnimateMovement(mostValuableAttacker, mostValuableTarget));
            Move(mostValuableAttacker, mostValuableTarget);
        }
        else
        {
            var figures = player.ActiveFigures
                .Where(c => c.MovementCells.Any())
                .ToList();
            Shuffle(figures);
            var figure = figures.First();
            var moves = figure.MovementCells.ToList();
            Shuffle(moves);
            var targetCell = moves.First();
            yield return StartCoroutine(AnimateMovement(figure, targetCell));
            Move(figure, targetCell);
        }
    }

    private IEnumerator AnimateMovement(ChessFigure figure, BoardCell targetCell)
    {
        var currentCell = figure.Cell;
        figure.Cell = null;
        while (Distance(figure, targetCell) > 0.4f)
        {
            figure.transform.position = Vector3.MoveTowards(
                figure.transform.position,
                targetCell.transform.position,
                0.3f
            );
            yield return null;
        }
        figure.Cell = currentCell;
    }

    private static float Distance(ChessFigure figure, BoardCell cell)
    {
        return Vector3.Distance(figure.transform.position, cell.transform.position);
    }

    private Player OppositePlayer(Player player)
    {
        return firstPlayer == player ? secondPlayer : firstPlayer;
    }

    private void ResetPositions()
    {
        var children = GetComponentsInChildren<BoardCell>();
        for (var i = 0; i < children.Length; i++)
        {
            var row = i / 8;
            var column = i % 8;
            children[i].Position = new Vector2Int(column, row);
            children[i].AffectingGod = null;
            cells[column, row] = children[i];
        }
    }

    public bool TryGetCell(Vector2Int position, out BoardCell cell)
    {
        if (position.x >= 0 && position.x < Width && position.y >= 0 && position.y < Height)
        {
            cell = cells[position.x, position.y];
            return true;
        }

        cell = null;
        return false;
    }
}

public class Player
{
    private readonly List<ChessFigure> activeFigures = new List<ChessFigure>();
    private readonly List<ChessFigure> deadFigures = new List<ChessFigure>();
    private bool isActive;

    public void AddActiveFigure(ChessFigure figure)
    {
        activeFigures.Add(figure);
    }

    public IEnumerable<ChessFigure> ActiveFigures
    {
        get { return activeFigures; }
    }

    public King King
    {
        get { return activeFigures.OfType<King>().First(); }
    }

    public bool IsActive
    {
        get { return isActive; }
        set
        {
            isActive = value;
            foreach (var figure in activeFigures)
            {
                figure.IsInteractable = value;
            }
        }
    }

    public God God { get; set; }

    public bool IsBot { get; set; }

    public bool IsFirst { get; set; }

    public void AddDeadFigure(ChessFigure figure)
    {
        activeFigures.Remove(figure);
        deadFigures.Add(figure);
    }

    public void ClearFigures()
    {
        foreach (var figure in activeFigures)
        {
            Object.Destroy(figure.gameObject);
        }
        activeFigures.Clear();
        deadFigures.Clear();
    }
}

public abstract class God
{
    public abstract Color Color { get; }

    public static implicit operator God(GodName name)
    {
        switch (name)
        {
            case GodName.Khorne:
                return new Khorne();
            case GodName.Tzeentch:
                return new Tzeentch();
            case GodName.Nurgle:
                return new Nurgle();
            case GodName.Slaanesh:
                return new Slaanesh();
            default:
                throw new ArgumentOutOfRangeException("name", name, null);
        }
    }
}

public class Khorne : God
{
    public override Color Color
    {
        get { return Color.red; }
    }
}

public class Nurgle : God
{
    public override Color Color
    {
        get { return Color.green; }
    }
}

public class Tzeentch : God
{
    public override Color Color
    {
        get { return Color.cyan; }
    }
}

public class Slaanesh : God
{
    public override Color Color
    {
        get { return Color.magenta; }
    }
}