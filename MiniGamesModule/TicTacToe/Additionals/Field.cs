using System.Drawing;

namespace TelegramBotHub.MiniGamesModule.TicTacToe.Additionals;

public class Field
{
    public Block[,] Blocks { get; }
    public Cell[,] CellsTable { get; }
    public String WinningValue { get; private set; }

    public Field()
    {
        CellsTable = new Cell[9, 9];
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                CellsTable[x, y] = new Cell(x, y);
            }
        }

        Blocks = new Block[3, 3];
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                Blocks[x, y] = new Block(x, y, CellsTable);
            }
        }

        WinningValue = "";
    }

    public List<Cell> GetPossibleTurns(Point lastTurnCellInBlockPosition)
    {
        var result = new List<Cell>();

        var blocksList = new Block[1, 1];
        blocksList[0, 0] = Blocks[lastTurnCellInBlockPosition.X, lastTurnCellInBlockPosition.Y];
        if (Blocks[lastTurnCellInBlockPosition.X, lastTurnCellInBlockPosition.Y].IsClose)
            blocksList = Blocks;

        foreach (var block in blocksList)
        {
            var blockPossibleTurns = block.GetPossibleTurns();
            if (blockPossibleTurns.Count > 0)
                result.AddRange(blockPossibleTurns);
        }

        return result;
    }

    public void SetValue(Cell turn)
    {
        var block = Blocks[turn.BlockPosition.X, turn.BlockPosition.Y];
        block.SetValue(turn.Value, turn.CellInBlockPosition);
        if (block.IsClose)
        {
            CheckAnyWin();
        }
    }

    private void CheckAnyWin()
    {
        var checkedPositions = new Point[] { new (0, 0), new (1, 1), new (2, 2) };
        foreach (var checkedPosition in checkedPositions)
        {
            var block = Blocks[checkedPosition.X, checkedPosition.Y];
            if(block.Value == "") continue;

            var value = block.Value;
            var lines = PossibleLineFinder.FindLines(checkedPosition);
            foreach (var line in lines)
            {
                if (value == Blocks[line.Item1.X, line.Item1.Y].Value &&
                    value == Blocks[line.Item2.X, line.Item2.Y].Value)
                {
                    WinningValue = value;
                    return;
                }
            }
        }
    }

    public bool IsAnyWin()
    {
        return WinningValue != "";
    }

    public bool HasAnyTurns()
    {
        foreach (var block in Blocks)
            if (!block.IsClose)
                return true;
        return false;
    }
}