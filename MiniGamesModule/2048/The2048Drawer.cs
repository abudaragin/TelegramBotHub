using System.Drawing;

namespace TelegramBotHub.MiniGamesModule._2048;

public class The2048Drawer: IThe2048Drawer
{
    private const int CellSize = 100;
    private static int FieldSize = CellSize * 4 + 1;
    private const int XTextShift = 5;
    private const int YTextShift = 5;
    
    public Bitmap DrawView(int[,] numbers)
    {
        Bitmap result = new Bitmap(FieldSize, FieldSize);
        Graphics graphics = Graphics.FromImage(result);
        graphics.Clear(Color.White);
        
        DrawEmptyTable(graphics);
        DrawCellValues(graphics, numbers);
        
        return result;
    }
    
    private void DrawEmptyTable(Graphics graphics)
    {
        var pen = new Pen(Color.Black);
        for (int i = 0; i < 5; i++)
        {
            var position = i * CellSize;
            graphics.DrawLine(pen, new Point(position, 0), new Point(position, FieldSize));
            graphics.DrawLine(pen, new Point(0, position), new Point(FieldSize, position));
        }
    }

    private void DrawCellValues(Graphics graphics, int[,] numbers)
    {
        Font font = new Font("Arial", 16, FontStyle.Bold);
        SolidBrush brush = new SolidBrush(Color.Black);

        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                var number = numbers[x, y];
                if(number == 0) continue;
                
                graphics.DrawString(
                    number.ToString(),
                    new Font("Arial", GetFontSize(number), FontStyle.Bold),
                    brush,
                    new PointF(
                        GetXShift(number) + x * CellSize,
                        GetYShift(number) + y * CellSize));
            }
        }
    }

    #region STATIC SIZES
    private static int GetFontSize(int number)
    {
        return number.ToString().Length switch
        {
            1 => 70,
            2 => 50,
            3 => 35,
            _ => 25
        };
    }
    
    private static int GetXShift(int number)
    {
        return number.ToString().Length switch
        {
            1 => 10,
            2 => 0,
            3 => 3,
            _ => 10
        };
    }
    
    private static int GetYShift(int number)
    {
        return number.ToString().Length switch
        {
            1 => 0,
            2 => 15,
            3 => 20,
            _ => 30
        };
    }
    #endregion
}