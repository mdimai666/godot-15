using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Godot15;

internal class GameBoard
{
    NumberTile[] _tiles;

    const int TILE_COUNT = 16;
    public int Size = TILE_COUNT / 4;

    public int MoveCount { get; private set; } = 0;
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    bool IsGameProcess => EndTime != null;

    NumberTile EmptyTile;
    Control PuzzlesArea { get; set; }

    public Action OnWin { get; set; }
    public Action<int> OnMoveTile { get; set; }

    public void CreateBoard(NumberTile firstTile, NumberTile emptyTile, Control puzzlesArea)
    {
        //PackedScene packedScene = GD.Load("res://NumberTile.tscn");
        EmptyTile = emptyTile;
        PuzzlesArea = puzzlesArea;

        _tiles = new NumberTile[TILE_COUNT];
        //_tiles[0] = NumberTile.GetNode<NumberTile>("NinePatchRect");
        _tiles[0] = firstTile;
        _tiles[TILE_COUNT - 1] = EmptyTile;

        int rowCount = Size;

        var w = PuzzlesArea.Size.X / rowCount;
        for (int i = 1; i < TILE_COUNT - 1; i++)
        {
            var tileIndex = i;
            var tile = (NumberTile)firstTile.Duplicate();
            PuzzlesArea.AddChild(tile);
            _tiles[i] = tile;
            var label = tile.GetNode<Label>("NinePatchRect/Label");
            label.Text = $"{i + 1}";
        }

        for (int i = 0; i < TILE_COUNT; i++)
        {
            var tileIndex = i;
            var tile = _tiles[i];
            //var y = i / rowCount;
            //var x = i - (y * rowCount);
            //tile.Position = new Vector2(x * w, y * w);
            tile.Position = IndexToVectorPosition(i);
            tile.Size = new Vector2(w, w);
            tile.OnClick = e => OnClickTile(tileIndex);
            tile.PositionIndex = i;
        }

        Start();
    }

    public void Start()
    {
        MoveCount = 0;
        StartTime = DateTime.Now;
        EndTime = null;
        Shuffle();
    }

    //void OnClickTile(int tileIndex)
    //{
    //    GD.Print($"tile-{tileIndex}");

    //    var tile = _tiles[tileIndex];
    //    (tile.Position, EmptyTile.Position) = (EmptyTile.Position, tile.Position);
    //    var k = EmptyTile.PositionIndex;
    //    EmptyTile.PositionIndex = tile.PositionIndex;
    //    tile.PositionIndex = k;
    //}

    public void Resize()
    {
        GD.Print("Resize");
        int rowCount = Size;
        var w = PuzzlesArea.Size.X / rowCount;

        for (int i = 0; i < TILE_COUNT; i++)
        {
            var tile = _tiles[i];
            var y = tile.PositionIndex / rowCount;
            var x = tile.PositionIndex - (y * rowCount);
            tile.Size = new Vector2(w, w);
            tile.Position = new Vector2(x * w, y * w);
        }
    }

    bool IsAdjacent(int index1, int index2)
    {
        int x1 = index1 % Size, y1 = index1 / Size;
        int x2 = index2 % Size, y2 = index2 / Size;

        return (Math.Abs(x1 - x2) + Math.Abs(y1 - y2)) == 1;
    }

    void OnClickTile(int tileIndex)
    {
        //int emptyIndex = Array.IndexOf(_tiles, EmptyTile);
        int emptyIndex = TILE_COUNT - 1;
        if (tileIndex == emptyIndex) return;

        var tilePositionIndex = _tiles[tileIndex].PositionIndex;
        var emptyPositionIndex = EmptyTile.PositionIndex;

        bool onlyAdjacent = false;

        if (onlyAdjacent)
        {
            // Разрешаем ход только если тайл соседний с пустой клеткой
            if (!IsAdjacent(tilePositionIndex, emptyPositionIndex))
                return;

            MoveTile(tileIndex, emptyIndex);
            AddTileMove();
        }
        else
        {
            var tiles = GetTilesForShift(tileIndex, out var xshift);
            if (!tiles.Any()) return;
            for (int i = 0; i < tiles.Length - 1; i++)
            {
                var index = Array.IndexOf(_tiles, tiles[i]);
                var nextIndex = Array.IndexOf(_tiles, tiles[i + 1]);
                MoveTile(index, nextIndex);
            }
            OnMoveTile.Invoke(tileIndex);
            AddTileMove();
        }
    }

    void AddTileMove()
    {
        MoveCount++;
        if (MoveCount == 1) StartTime = DateTime.Now;
    }

    void MoveTile(int fromTileIndex, int toTileIndex)
    {
        var movingTile = _tiles[fromTileIndex];
        var secondTile = _tiles[toTileIndex];

        var tempIdx = movingTile.PositionIndex;
        movingTile.PositionIndex = secondTile.PositionIndex;
        secondTile.PositionIndex = tempIdx;

        Vector2 targetPos = IndexToVectorPosition(movingTile.PositionIndex);

        // Анимация через Tween
        using var tween = PuzzlesArea.CreateTween();
        tween.TweenProperty(movingTile, "position", targetPos, 0.25f)
             .SetTrans(Tween.TransitionType.Quint)
             .SetEase(Tween.EaseType.Out);

        Vector2 targetPos2 = IndexToVectorPosition(secondTile.PositionIndex);

        using var tween2 = PuzzlesArea.CreateTween();
        tween2.TweenProperty(secondTile, "position", targetPos2, 0.25f)
             .SetTrans(Tween.TransitionType.Quint)
             .SetEase(Tween.EaseType.Out);

        tween.Finished += CheckWin;
    }

    Vector2 IndexToVectorPosition(int index)
    {
        int targetX = index % Size;
        int targetY = index / Size;
        float tileSize = PuzzlesArea.Size.X / Size;
        Vector2 targetPos = new(targetX * tileSize, targetY * tileSize);
        return targetPos;
    }

    bool IsTileOnEmptyTileLine(int tileIndex)
    {
        var (x1, y1) = PositionIndexToXYPosition(_tiles[tileIndex].PositionIndex);
        var (x2, y2) = PositionIndexToXYPosition(EmptyTile.PositionIndex);

        return x1 == x2 || y1 == y2;
    }

    (int x, int y) PositionIndexToXYPosition(int positionIndex)
    {
        int targetX = positionIndex % Size;
        int targetY = positionIndex / Size;
        return (targetX, targetY);
    }

    int PositionIndexToX(int positionIndex) => positionIndex % Size;
    int PositionIndexToY(int positionIndex) => positionIndex / Size;

    Dictionary<(int, int), NumberTile> TilesPosDict()
        => _tiles.ToDictionary(tile => PositionIndexToXYPosition(tile.PositionIndex));

    NumberTile[] GetTilesForShift(int clickedTileIndex, out bool XShift)
    {
        var (x1, y1) = PositionIndexToXYPosition(_tiles[clickedTileIndex].PositionIndex);
        var (x2, y2) = PositionIndexToXYPosition(EmptyTile.PositionIndex);

        //var tiles = _tiles.Where(s => PositionIndexToX(s.PositionIndex) == x1).OrderBy(s => PositionIndexToY(s.PositionIndex)).ToArray();
        var tilesDict = TilesPosDict();

        if (x1 == x2)
        {
            XShift = true;
            var tiles = y1 < y2
                            ? Enumerable.Range(y1, y2 - y1 + 1).Select(y => tilesDict[(x1, y)])
                            : Enumerable.Range(y2, y1 - y2 + 1).Select(y => tilesDict[(x1, y)]).Reverse();

            return tiles.ToArray();
        }
        else if (y1 == y2)
        {
            XShift = false;
            var tiles = x1 < x2
                            ? Enumerable.Range(x1, x2 - x1 + 1).Select(x => tilesDict[(x, y1)])
                            : Enumerable.Range(x2, x1 - x2 + 1).Select(x => tilesDict[(x, y1)]).Reverse();

            return tiles.ToArray();
        }

        XShift = false;
        return [];
    }

    public void Shuffle()
    {
        var shuffle = BoardTools.GenerateShuffle(Size, shuffleMoves: 400);
        foreach (var (position, tileIndex) in shuffle)
        {
            var tile = _tiles[tileIndex];
            var positionIndex = position.y * Size + position.x;
            tile.Position = IndexToVectorPosition(positionIndex);
            tile.PositionIndex = positionIndex;
        }
        GD.Print("Shuffle");
    }

    void CheckWin()
    {
        for (int i = 0; i < _tiles.Length; i++)
        {
            if (_tiles[i].PositionIndex != i)
            {
                return;
            }
        }
        GameFinished();
    }

    void GameFinished()
    {
        EndTime = DateTime.Now;
        OnWin?.Invoke();
    }

}
