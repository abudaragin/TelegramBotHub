using System.Drawing;
using TelegramBotHub.MiniGamesModule.TicTacToe.Additionals;
using TelegramBotHub.MiniGamesModule.TicTacToe.Interfaces;

namespace TelegramBotHub.MiniGamesModule.TicTacToe.Drawer;

public class TicTacToeDrawer : ITicTacToeDrawer
{
    private const int CellSize = 30;
    private static int FieldSize = CellSize * 9 + 1;
    private const int XTextShift = 5;
    private const int YTextShift = 5;

    private Field _field;

    public TicTacToeDrawer(Field field)
    {
        _field = field;
    }

    public Bitmap DrawView(bool[,] markers, bool isChoiceBlock, Point chosenBlock, Cell lastTurn)
    {
        Bitmap result = new Bitmap(FieldSize, FieldSize);
        Graphics graphics = Graphics.FromImage(result);
        graphics.Clear(Color.White);

        if (isChoiceBlock) DrawBlocksMarkers(graphics, markers);
        else DrawCellsMarkers(graphics, markers, chosenBlock);
        DrawEmptyTable(graphics);
        DrawCellValues(graphics, lastTurn);
        DrawBlocksValue(graphics);

        return result;
    }

    private void DrawBlocksMarkers(Graphics graphics, bool[,] markers)
    {
        SolidBrush brush = new SolidBrush(Color.Yellow);
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (markers[x, y])
                    graphics.FillRectangle(brush,
                        x * CellSize * 3,
                        y * CellSize * 3,
                        CellSize * 3,
                        CellSize * 3);
            }
        }
    }

    private void DrawCellsMarkers(Graphics graphics, bool[,] markers, Point chosenBlock)
    {
        SolidBrush brush = new SolidBrush(Color.Yellow);
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                if (markers[x, y])
                    graphics.FillRectangle(brush,
                        x * CellSize + chosenBlock.X * CellSize * 3,
                        y * CellSize + chosenBlock.Y * CellSize * 3,
                        CellSize,
                        CellSize);
            }
        }
    }

    private void DrawEmptyTable(Graphics graphics)
    {
        var pen = new Pen(Color.Black);
        for (int i = 0; i < 10; i++)
        {
            var position = i * CellSize;
            graphics.DrawLine(pen, new Point(position, 0), new Point(position, FieldSize));
            graphics.DrawLine(pen, new Point(0, position), new Point(FieldSize, position));
        }

        for (int i = 0; i < 4; i++)
        {
            var position = i * CellSize * 3;
            graphics.DrawLine(pen, new Point(position - 1, 0), new Point(position - 1, FieldSize));
            graphics.DrawLine(pen, new Point(position + 1, 0), new Point(position + 1, FieldSize));
            graphics.DrawLine(pen, new Point(0, position - 1), new Point(FieldSize, position - 1));
            graphics.DrawLine(pen, new Point(0, position + 1), new Point(FieldSize, position + 1));
        }
    }

    private void DrawCellValues(Graphics graphics, Cell lastTurn)
    {
        Font font = new Font("Arial", 16, FontStyle.Bold);
        SolidBrush mainBrush = new SolidBrush(Color.Black);
        SolidBrush lastTurnBrush = new SolidBrush(Color.Red);
        
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                var brush = mainBrush;
                if (lastTurn.CellInTablePosition.X == x && lastTurn.CellInTablePosition.Y == y)
                    brush = lastTurnBrush;
                graphics.DrawString(
                    _field.CellsTable[x, y].Value,
                    font,
                    brush,
                    new PointF(
                        XTextShift + x * CellSize,
                        YTextShift + y * CellSize));
            }
        }
    }

    private void DrawBlocksValue(Graphics graphics)
    {
        Font font = new Font("Arial", 48, FontStyle.Bold);
        SolidBrush textBrush = new SolidBrush(Color.Black);
        SolidBrush backBrush = new SolidBrush(Color.White);

        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                var value = _field.Blocks[x, y].Value;
                if (value != "")
                {
                    graphics.FillRectangle(
                        backBrush,
                        x * CellSize * 3 + 2,
                        y * CellSize * 3 + 2,
                        CellSize * 3 - 3,
                        CellSize * 3 - 3);
                    graphics.DrawString(
                        value,
                        font,
                        textBrush,
                        new PointF(
                            XTextShift * 2 + x * CellSize * 3,
                            YTextShift * 2 + y * CellSize * 3));
                }
            }
        }
    }
}