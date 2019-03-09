using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid
{
    private readonly HashSet<Cell> _alive = new HashSet<Cell>();
    private readonly HashSet<Cell> _toAdd = new HashSet<Cell>();
    private readonly HashSet<Cell> _toKill = new HashSet<Cell>();

    // Used when checking if any cells should be added, cached to avoid allocations
    private readonly List<Cell> _cachedAddCheckList = new List<Cell>() { Capacity = 8 };
    
    /// <summary>
    /// The more cells are alive, the less likely this will be O(1) due to collisions.
    /// </summary>
    public bool IsAlive(long x, long y)
    {
        // Because Cell is a struct, this allocation happens on the stack
        var cell = new Cell(x, y);

        return _alive.Contains(cell);
    }

    public void Seed(string path)
    {
        _alive.Clear();
        var file = new System.IO.StreamReader(@path);
        string line;
        while ((line = file.ReadLine()) != null)
        {
            var nums = line.Split(new char[] {' ', ',', '(', ')'});

            long x = 0;
            long y = 0;
            var gotX = false;
            var gotBoth = false;
            foreach (var num in nums)
            {
                long n;
                if (long.TryParse(num, out n))
                {
                    if (!gotX)
                    {
                        gotX = true;
                        x = n;
                    }
                    else
                    {
                        y = n;
                        gotBoth = true;
                        break;
                    }
                }
            }

            if (gotBoth)
            {
                _alive.Add(new Cell(x, y));
            }
        }
        
    }

    public void Tick()
    {
        _toKill.Clear();
        _toAdd.Clear();

        var neighbors = new List<Cell> { Capacity = 8 };

        foreach (var cell in _alive)
        {
            GetNeighbors(cell, neighbors);

            // if alive, check if it should die
            if (_alive.Contains(cell) && ShouldDie(cell, neighbors))
            {
                _toKill.Add(cell);
            }

            // check if any neighbors should live
            foreach (var neighbor in neighbors)
            {
                if (!_alive.Contains(neighbor) && ShouldAdd(neighbor))
                {
                    _toAdd.Add(neighbor);
                }
            }
        }

        foreach (var toKill in _toKill)
        {
            _alive.Remove(toKill);
        }

        foreach (var toAdd in _toAdd)
        {
            _alive.Add(toAdd);
        }
    }

    private bool ShouldDie(Cell cell, List<Cell> neighbors)
    {
        var aliveCount = 0;
        foreach (var neighbor in neighbors)
        {
            if (_alive.Contains(neighbor))
            {
                aliveCount++;
            }
        }

        return aliveCount < 2 || aliveCount > 3;
    }

    private bool ShouldAdd(Cell cell)
    {
        GetNeighbors(cell, _cachedAddCheckList);


        var aliveCount = 0;
        foreach (var neighbor in _cachedAddCheckList)
        {
            if (_alive.Contains(neighbor))
            {
                aliveCount++;
            }
        }

        return aliveCount == 3;
    }

    private static void GetNeighbors(Cell cell, List<Cell> neighbors)
    {
        neighbors.Clear();

        // North
        if (cell.Y < long.MaxValue)
        {
            neighbors.Add(new Cell(cell.X, cell.Y + 1));
        }

        // South
        if (cell.Y > long.MinValue)
        {
            neighbors.Add(new Cell(cell.X, cell.Y - 1));
        }

        if (cell.X < long.MaxValue)
        {
            // East
            neighbors.Add(new Cell(cell.X + 1, cell.Y));

            // North-East
            if (cell.Y < long.MaxValue)
            {
                neighbors.Add(new Cell(cell.X + 1, cell.Y + 1));
            }

            // South-East
            if (cell.Y > long.MinValue)
            {
                neighbors.Add(new Cell(cell.X + 1, cell.Y - 1));
            }
        }

        if (cell.X > long.MinValue)
        {
            // West
            neighbors.Add(new Cell(cell.X - 1, cell.Y));

            // North-West
            if (cell.Y < long.MaxValue)
            {
                neighbors.Add(new Cell(cell.X - 1, cell.Y + 1));
            }

            // South-West
            if (cell.Y > long.MinValue)
            {
                neighbors.Add(new Cell(cell.X - 1, cell.Y - 1));
            }
        }
    }
}
