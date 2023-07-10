using System.Drawing;

namespace TelegramBotHub.MiniGamesModule.TicTacToe.Additionals;

public class Cell
{
    public String Value { set; get; }
    public Point CellInTablePosition { get; }
    public Point CellInBlockPosition { get; private set; }
    public Point BlockPosition { get; private set; }

    public Cell(int x, int y)
    {
        CellInTablePosition = new Point(x, y);
        Value = "";
    }

    public Cell(String value, Point blockPosition, Point cellInBlockPosition)
    {
        CellInTablePosition = new Point(
            blockPosition.X * 3 + cellInBlockPosition.X,
            blockPosition.Y * 3 + cellInBlockPosition.Y);
        Value = value;
        BlockPosition = blockPosition;
        CellInBlockPosition = cellInBlockPosition;
    }

    public void AttachToBlock(Point blockPosition, Point cellPosition)
    {
        CellInBlockPosition = cellPosition;
        BlockPosition = blockPosition;
    }

    public Cell Clone()
    {
        var newCell = new Cell(this.CellInTablePosition.X, this.CellInTablePosition.Y);
        newCell.BlockPosition = this.BlockPosition;
        newCell.CellInBlockPosition = this.CellInBlockPosition;
        newCell.Value = this.Value;
        return newCell;
    }
}