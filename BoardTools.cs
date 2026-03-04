using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace Godot15;

internal class BoardTools
{
    /// <summary>
    /// Возвращает словарь: (координаты сетки) -> (индекс тайла 0-15).
    /// Индекс 15 — это всегда пустая клетка.
    /// </summary>
    /// <param name="gridSize">Размер сетки (например, 4 для 4x4)</param>
    /// <param name="shuffleMoves">Количество случайных ходов для перемешивания</param>
    public static Dictionary<(int x, int y), int> GenerateShuffle(int gridSize, int shuffleMoves = 100)
    {
        var rng = new RandomNumberGenerator();
        var board = new Dictionary<(int x, int y), int>();

        // 1. Инициализация "собранного" состояния
        // Заполняем сетку по порядку: 0, 1, 2... 14, 15(пустой)
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                int tileIndex = y * gridSize + x;
                board[(x, y)] = tileIndex;
            }
        }

        // Начальная позиция пустого тайла (правый нижний угол)
        int emptyX = gridSize - 1;
        int emptyY = gridSize - 1;

        // 2. Делаем случайные валидные ходы
        for (int i = 0; i < shuffleMoves; i++)
        {
            var neighbors = GetNeighbors(emptyX, emptyY, gridSize);
            var randomNeighbor = neighbors[rng.RandiRange(0, neighbors.Count - 1)];

            // Меняем значения местами (swap)
            int movingTileIndex = board[randomNeighbor]; // Тайл, который сдвигаем

            board[randomNeighbor] = 15;                  // Пустой переходит к соседу
            board[(emptyX, emptyY)] = movingTileIndex;   // Сдвинутый тайл занимает место пустого

            // Обновляем координаты "пустоты"
            emptyX = randomNeighbor.x;
            emptyY = randomNeighbor.y;
        }

        return board;
    }

    /// <summary>
    /// Возвращает список допустимых координат для хода из позиции (x, y)
    /// </summary>
    private static List<(int x, int y)> GetNeighbors(int x, int y, int gridSize)
    {
        var neighbors = new List<(int x, int y)>(4);

        if (x > 0) neighbors.Add((x - 1, y)); // Left
        if (x < gridSize - 1) neighbors.Add((x + 1, y)); // Right
        if (y > 0) neighbors.Add((x, y - 1)); // Up
        if (y < gridSize - 1) neighbors.Add((x, y + 1)); // Down

        return neighbors;
    }
}
