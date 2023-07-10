using System.Drawing;

namespace TelegramBotHub.MiniGamesModule.TicTacToe.Additionals;

public class Block
{
    private Cell[,] _cells;
    public Point Position { get; }
    public String Value { private set; get; }
    public bool IsClose { private set; get; }
    //public bool CanUserEndGame { set; get; }
    //public bool CanAIEndGame { set; get; }

    public Block(int blockX, int blockY, Cell[,] cellsTable)
    {
        Position = new Point(blockX, blockY);
        Value = "";
        IsClose = false;
        //CanUserEndGame = false;
        //CanAIEndGame = false;

        _cells = new Cell[3, 3];
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                _cells[x, y] = cellsTable[
                    Position.X * 3 + x,
                    Position.Y * 3 + y
                ];
                _cells[x, y].AttachToBlock(Position, new Point(x, y));
            }
        }
    }

    public List<Cell> GetPossibleTurns()
    {
        if (IsClose) return new List<Cell>();

        var result = new List<Cell>();
        foreach (var cell in _cells)
        {
            if (cell.Value == "")
                result.Add(cell.Clone());
        }

        return result;
    }

    public void SetValue(String value, Point position)
    {
        _cells[position.X, position.Y].Value = value;
        CheckIsClose(value, position);
    }

    private void CheckIsClose(String value, Point position)
    {
        var lines = PossibleLineFinder.FindLines(position);
        foreach (var line in lines)
        {
            if (value == _cells[line.Item1.X, line.Item1.Y].Value &&
                value == _cells[line.Item2.X, line.Item2.Y].Value)
            {
                Value = value;
                IsClose = true;
            }
        }
        if (IsClose) return;

        var hasEmptyCell = false;
        foreach (var cell in _cells)
        {
            if (cell.Value == "")
            {
                hasEmptyCell = true;
                break;
            }
        }
        if (!hasEmptyCell) IsClose = true;
    }

    public String GetValue(Point cellPosition)
    {
        return _cells[cellPosition.X, cellPosition.Y].Value;
    }
}