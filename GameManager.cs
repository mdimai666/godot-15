using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Godot;
using Godot15;

public partial class GameManager : Control
{
    [Export]
    public Control PuzzlesArea { get; set; }

    [Export]
    public NumberTile NumberTile { get; set; }

    private GameBoard _gameBoard = new();

    [Export]
    public NumberTile EmptyTile { get; set; }

    [Export]
    public ModalWin WinModal { get; set; }

    public override void _Ready()
    {
        CallDeferred(nameof(ApplicationStart));
    }

    public override void _Process(double delta)
    {

    }

    void ApplicationStart()
    {
        _gameBoard.CreateBoard(NumberTile, EmptyTile, PuzzlesArea);
        _gameBoard.Resize();
        Resized += _gameBoard.Resize;
        GameStart();

        _gameBoard.OnWin += OnWin;
        _gameBoard.OnMoveTile = OnMove;
    }

    public void GameStart()
    {
        GD.Print("GameStart()");
        _gameBoard.Start();
    }

    public void OnClickReset()
    {
        GameStart();
    }

    public void CloseWinModal()
    {
        WinModal.Hide();
        GameStart();
    }

    void OnMove(int tileIndex)
    {
    }

    async void OnWin()
    {
        await Task.Delay(120);
        var durationStr = (_gameBoard.EndTime == null) ? "0"
                            : _gameBoard.EndTime.Value.Subtract(_gameBoard.StartTime).ToString(@"mm\:ss");

        var scoreText = $"Puzzle solved!\n {_gameBoard.MoveCount} moves, time: {durationStr}";
        WinModal.SetModalTextsandScore("You Win!", scoreText);
        WinModal.Show();
    }
}
