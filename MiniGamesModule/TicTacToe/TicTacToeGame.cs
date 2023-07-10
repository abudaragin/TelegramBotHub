using System.Drawing;
using TelegramBotHub.MiniGamesModule.TicTacToe.Additionals;
using TelegramBotHub.MiniGamesModule.TicTacToe.Drawer;
using TelegramBotHub.MiniGamesModule.TicTacToe.Interfaces;
using TelegramBotHub.MiniGamesModule.TicTacToe.OpponentAI;

namespace TelegramBotHub.MiniGamesModule.TicTacToe;

public class TicTacToeGame
{
    public const String UserValue = "X";
    public const String AIValue = "O";

    private Field _field;
    public bool IsEnd { get; private set; }
    private Cell _lastTurn;
    public String GameResult { get; private set; }
    
    private List<Cell> _possibleTurns;
    public bool IsChoiceBlock { get; private set; }
    private Point _choicenBlockPosition;

    private ITicTacToeDrawer _ticTacToeDrawer;
    private IOpponentAI _opponentAi;

    public TicTacToeGame()
    {
        IsEnd = false;
        _field = new Field();
        _lastTurn = _field.CellsTable[4, 4].Clone(); //Первый ход должен быть сделан в центральный блок
        GameResult = "";
        
        _possibleTurns = _field.GetPossibleTurns(_lastTurn.CellInBlockPosition);
        IsChoiceBlock = false;
        _choicenBlockPosition = new Point(1, 1);

        _ticTacToeDrawer = new TicTacToeDrawer(_field);
        _opponentAi = new TicTacToeAI(_field);
    }

    private void UpdateGameStatus()
    {
        if (_field.IsAnyWin())
        {
            IsEnd = true;
            _possibleTurns.Clear();
            if (_lastTurn.Value == UserValue)
            {
                GameResult = "User Win!!!";
            }
            else
            {
                GameResult = "Bot Win!!!";
            }
        }
        else if (!_field.HasAnyTurns())
        {
            IsEnd = true;
            _possibleTurns.Clear();
            GameResult = "Nobody win.";
        }
    }

    public void CreateAITurn()
    {
        _lastTurn = _opponentAi.ChoiceTurn(_possibleTurns);
        _lastTurn.Value = AIValue;
        ReactToTurn();
    }

    public Bitmap DrawView()
    {
        return _ticTacToeDrawer.DrawView(GetUserChoices(), IsChoiceBlock, _choicenBlockPosition, _lastTurn);
    }

    public bool[,] GetUserChoices()
    {
        var choices = new List<Point>();
        if (IsChoiceBlock)
        {
            choices = FilterPossibleBlocks();
        }
        else
        {
            choices = FilterPossibleTurns(_choicenBlockPosition);
        }

        var result = new bool[3, 3];
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                result[x, y] = false;
            }
        }

        foreach (var point in choices)
        {
            result[point.X, point.Y] = true;
        }

        return result;
    }

    public bool CanUserMoveUndo()
    {
        return !IsChoiceBlock && FilterPossibleBlocks().Count > 1;
    }

    public void SetUserChoice(String userChoise)
    {
        if (userChoise == "undo")
        {
            IsChoiceBlock = true;
            return;
        }

        var x = Int32.Parse(userChoise.Substring(0, 1));
        var y = Int32.Parse(userChoise.Substring(1, 1));
        if (IsChoiceBlock)
        {
            IsChoiceBlock = false;
            _choicenBlockPosition = new Point(x, y);
        }
        else
        {
            _lastTurn = new Cell(UserValue, _choicenBlockPosition, new Point(x, y));
            ReactToTurn();
        }
    }

    private List<Point> FilterPossibleTurns(Point blockPosition)
    {
        var result = new List<Point>();
        foreach (var turn in _possibleTurns)
        {
            if(turn.BlockPosition == blockPosition)
                result.Add(turn.CellInBlockPosition);
        }

        return result;
    }

    private List<Point> FilterPossibleBlocks()
    {
        var result = new List<Point>();
        foreach (var turn in _possibleTurns)
        {
            if(!result.Contains(turn.BlockPosition))
                result.Add(turn.BlockPosition);
        }

        return result;
    }

    private void ReactToTurn()
    {
        _field.SetValue(_lastTurn);
        UpdatePossibleTurns();
        UpdateGameStatus();
        _opponentAi.ReactToTurn(_lastTurn);
    }

    private void UpdatePossibleTurns()
    {
        _possibleTurns = _field.GetPossibleTurns(_lastTurn.CellInBlockPosition);
        if (FilterPossibleBlocks().Count > 1)
        {
            IsChoiceBlock = true;
        }
        else
        {
            IsChoiceBlock = false;
            _choicenBlockPosition = _lastTurn.CellInBlockPosition;
        }
    }
}