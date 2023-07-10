using System.Drawing;

namespace TelegramBotHub.MiniGamesModule._2048;

public class The2048Game
{
    private IThe2048Drawer _drawer;
    private int[,] _field;
    public bool IsEnd { private set; get; }

    public The2048Game(IThe2048Drawer drawer)
    {
        _drawer = drawer;
        IsEnd = false;
        _field = new int[4, 4];

        CreateNewNumber();
        CreateNewNumber();
    }

    public Bitmap DrawView()
    {
        return _drawer.DrawView(_field);
    }

    public void SetTurn(String turn)
    {
        bool cellsMoved;
        switch (turn)
        {
            case "up":
                cellsMoved = MoveCells(0, 1);
                break;
            case "left":
                cellsMoved = MoveCells(1, 0);
                break;
            case "rigth":
                cellsMoved = MoveCells(-1, 0);
                break;
            case "down":
                cellsMoved = MoveCells(0, -1);
                break;
            default:
                throw new Exception();
        }

        if (!cellsMoved) return;
        CreateNewNumber();
        CheckIsGameEnd();
    }

    private void CreateNewNumber()
    {
        var emptys = new List<Point>();
        for (int x = 0; x < _field.GetLength(0); x++)
        for (int y = 0; y < _field.GetLength(1); y++)
        {
            if (_field[x, y] == 0) emptys.Add(new Point(x, y));
        }

        var random = new Random();
        var point = emptys[random.Next(emptys.Count)];
        var percent = random.Next(100);

        _field[point.X, point.Y] = percent >= 90 ? 4 : 2;
    }

    #region MOVE

    private bool MoveCells(int xShift, int yShift)
    {
        var result = MoveEmptys(xShift, yShift);
        result = ConnectPairs(xShift, yShift) | result;
        MoveEmptys(xShift, yShift);
        ChangeNegatives();
        return result;
    }


    private bool MoveEmptys(int xShift, int yShift)
    {
        var result = false;

        var startIndex = Math.Max(xShift, yShift) > 0 ? 0 : 3;
        var gap = Math.Max(xShift, yShift) > 0 ? 1 : -1;

        for (int i = 0; i < 4; i++)
        for (int x = startIndex; IsIndexInField(x); x += gap)
        for (int y = startIndex; IsIndexInField(y); y += gap)
        {
            if (IsPointInField(x + xShift, y + yShift) &&
                _field[x, y] == 0 &&
                _field[x + xShift, y + yShift] > 0)
            {
                var temp = _field[x, y];
                _field[x, y] = _field[x + xShift, y + yShift];
                _field[x + xShift, y + yShift] = temp;
                result = true;
            }
        }

        return result;
    }

    private bool ConnectPairs(int xShift, int yShift)
    {
        var result = false;

        var startIndex = Math.Max(xShift, yShift) > 0 ? 0 : 3;
        var gap = Math.Max(xShift, yShift) > 0 ? 1 : -1;

        for (int i = 0; i < 4; i++)
        for (int x = startIndex; IsIndexInField(x); x += gap)
        for (int y = startIndex; IsIndexInField(y); y += gap)
        {
            if (IsPointInField(x + xShift, y + yShift) &&
                _field[x, y] != 0 &&
                _field[x, y] == _field[x + xShift, y + yShift])
            {
                _field[x, y] *= -2;
                _field[x + xShift, y + yShift] = 0;
                result = true;
            }
        }

        return result;
    }

    private void ChangeNegatives()
    {
        for (int x = 0; x < _field.GetLength(0); x++)
        for (int y = 0; y < _field.GetLength(1); y++)
        {
            if (_field[x, y] < 0) _field[x, y] *= -1;
        }
    }

    #endregion

    #region CHECK

    private void CheckIsGameEnd()
    {
        if (!CheckHasEmpty() && !CheckCanConnect())
            IsEnd = true;
    }

    private bool CheckHasEmpty()
    {
        for (int x = 0; x < _field.GetLength(0); x++)
        for (int y = 0; y < _field.GetLength(1); y++)
            if (_field[x, y] == 0)
                return true;

        return false;
    }

    private bool CheckCanConnect()
    {
        IsEnd = true;
        for (int x = 0; x < _field.GetLength(0); x++)
        for (int y = 0; y < _field.GetLength(1); y++)
        {
            if (x > 0 && _field[x, y] == _field[x - 1, y])
                return true;

            if (y > 0 && _field[x, y] == _field[x, y - 1])
                return true;
        }

        return false;
    }

    #endregion

    private bool IsIndexInField(int index)
    {
        return index >= 0 && index <= 3;
    }

    private bool IsPointInField(int x, int y)
    {
        return x >= 0 && x <= 3 &&
               y >= 0 && y <= 3;
    }
}