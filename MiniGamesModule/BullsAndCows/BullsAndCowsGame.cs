using System.Text;

namespace TelegramBotHub.MiniGamesModule.BullsAndCows;

public class BullsAndCowsGame
{
    private readonly BullsAndCowsSettings _settings;
    public bool IsEnd { get; private set; }
    private readonly List<int> _values;

    private int _turns;
    private long _startTime;

    public BullsAndCowsGame(BullsAndCowsSettings settings)
    {
        _settings = settings;
        IsEnd = false;
        _values = CreateValues();

        _turns = 0;
        _startTime = 0L;
    }

    private List<int> CreateValues()
    {
        var allNumbers = new List<int>()
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9
        };
        var result = new List<int>();
        var random = new Random();

        for (int i = 0; i < _settings.Count; i++)
        {
            var index = random.Next(allNumbers.Count);
            result.Add(allNumbers[index]);
            if (!_settings.CanRepeated)
                allNumbers.RemoveAt(index);
        }

        return result;
    }

    public String GetTurnResult(String userTurn, DateTime time)
    {
        if (userTurn == "/give_up")
            return GiveUp();
        
        if (!IsCorrectFormat(userTurn))
            return "Вы должны написать число";

        if (userTurn.Length != _values.Count)
            return "Число должно быть из " + _settings.Count + " знаков";

        if (!CheckRepeatsAreCorrect(userTurn))
            return "Цыфры не должны повторятьься";

        if (_turns == 0)
            _startTime = time.Ticks;
        _turns++;
        var result = CountBullsAndCows(userTurn);
        if (result.Item1 == _values.Count)
        {
            IsEnd = true;
            var finishTime = time.Ticks;
            DateTime date = new DateTime(finishTime - _startTime);
            string timeString = date.ToString("mm:ss.fff");
            return "Да, именно это число я загадал! Вы победили!" +
                   "\nХодов: " + _turns +
                   "\nПотрачено времеи: " + timeString;
        }
        
        return CreateResultString(result);
    }

    private String GiveUp()
    {
        IsEnd = true;
        var builder = new StringBuilder();
        builder.Append("Игра окончена. Было загадано число ");
        foreach (var number in _values)
        {
            builder.Append(number);
        }
        return builder.ToString();
    }

    #region Check userTurn
    private bool IsCorrectFormat(String userTurn)
    {
        try
        {
            var number = Int32.Parse(userTurn);
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    private Tuple<int, int> CountBullsAndCows(String userTurn)
    {
        int bulls = 0;
        int cows = 0;

        for (int i = 0; i < userTurn.Length; i++)
        {
            var number = Int32.Parse(userTurn.Substring(i, 1));
            if (number == _values[i])
            {
                bulls++;
            }else if (_values.Contains(number))
            {
                cows++;
            }
        }

        return Tuple.Create(bulls, cows);
    }

    private bool CheckRepeatsAreCorrect(String userTurn)
    {
        if (_settings.CanRepeated) return true;

        var chars = new List<char>();

        foreach (var oneChar in userTurn)
        {
            if (chars.Contains(oneChar))
            {
                return false;
            }
            else
            {
                chars.Add(oneChar);
            }
        }

        return true;
    }
    #endregion

    private String CreateResultString(Tuple<int, int> result)
    {
        return "" + result.Item1 + "Б " + result.Item2 + "K";
    }
}