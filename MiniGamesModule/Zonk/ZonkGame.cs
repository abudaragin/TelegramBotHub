using System.Text;

namespace TelegramBotHub.MiniGamesModule.Zonk;

public class ZonkGame
{
    private int[] _roundResults;
    private int _roundNum;
    private ZonkRound _zonkRound;

    public ZonkGame()
    {
        _roundResults = new int[10];
        _roundNum = 1;
        _zonkRound = new ZonkRound();
    }

    public String GetReport()
    {
        var builder = new StringBuilder();
        builder.Append(GetResultText());
        builder.Append("Раунд №" + _roundNum);
        builder.Append("\n");
        builder.Append(_zonkRound.GetReport());
        return builder.ToString();
    }

    #region OPTIONS

    public bool CanStop()
    {
        return _zonkRound.CanStop();
    }

    public void StopRound()
    {
        _roundResults[_roundNum - 1] = _zonkRound.Result;
        _roundNum++;
        _zonkRound = new ZonkRound();
    }

    public void Continue()
    {
        if (_zonkRound.CanContinue()) _zonkRound.Continue();
        else
        {
            _roundResults[_roundNum - 1] = 0;
            if (_roundNum >= 3 && _roundResults[_roundNum - 2] == 0 && _roundResults[_roundNum - 3] == 0)
                _roundResults[_roundNum - 1] = -500;
            _roundNum++;
            _zonkRound = new ZonkRound();
        }
    }

    #endregion

    public bool IsEnd()
    {
        return _roundNum == 11;
    }

    #region TEXT

    private String GetResultText()
    {
        var builder = new StringBuilder();
        if (_roundNum > 1)
        {
            builder.Append("Результаты:");
            builder.Append("\n");
            int sum = 0;
            for (int i = 0; i < _roundNum - 1; i++)
            {
                builder.Append("Раунд " + (i + 1) + ": " + _roundResults[i]);
                builder.Append("\n");
                sum += _roundResults[i];
            }

            builder.Append("Итог: " + sum);
            builder.Append("\n");
            builder.Append("\n");
        }

        return builder.ToString();
    }

    #endregion
}