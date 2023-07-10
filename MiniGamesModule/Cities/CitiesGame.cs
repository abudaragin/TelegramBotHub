namespace TelegramBotHub.MiniGamesModule.Cities;

internal class CitiesGame
{
    private readonly Dictionary<String, List<String>> _allCitys;
    private readonly Dictionary<String, List<String>> _remainingCitys;
    public bool IsEnd { get; private set; }
    private int _remainingHelps;

    private String _nextFirstLetter;
    private String _possibleNextWord;

    public CitiesGame()
    {
        _allCitys = SplitCitys(CitiesNames.CityStrings);
        _remainingCitys = SplitCitys(CitiesNames.CityStrings);
        IsEnd = false;
        _remainingHelps = 3;
        _nextFirstLetter = "";
        _possibleNextWord = "";
    }

    private Dictionary<string, List<string>> SplitCitys(String[] citiesNames)
    {
        var result = new Dictionary<string, List<string>>();
        foreach (var city in citiesNames)
        {
            var firstLetter = city.Substring(0, 1).ToUpper();
            if (!result.ContainsKey(firstLetter))
            {
                result.Add(firstLetter, new List<string>());
            }

            result[firstLetter].Add(city);
        }

        return result;
    }

    public String GetNextTurn(String userTurn)
    {
        if (userTurn == "/help")
        {
            return HelpTurn();
        }

        if (userTurn == "/give_up")
        {
            return GiveUp();
        }

        var checkResult = CheckUserTurn(userTurn);
        if (checkResult != "") return checkResult;

        var nextCity = FindNextCity(userTurn);
        if (nextCity != "") return nextCity;

        IsEnd = true;
        return "ПОЗДРАВЛЯЮ!!!\n" +
               "Вы победили, я не знаю больше городов на " + _nextFirstLetter;
    }

    private String HelpTurn()
    {
        if (_nextFirstLetter == "")
        {
            return "Вы еще не сделали первый ход.\n" +
                   "Вы можете написать название любого города.";
        }

        if (_remainingHelps > 0)
        {
            _remainingHelps--;
            if (_possibleNextWord == "") return "Я и сам не знаю таких городов. Увы.";

            var help = _possibleNextWord.Substring(0, 2).PadRight(_possibleNextWord.Length, '*');
            return "Подсказка: " + help;
        }

        return "У вас кончились подсказки.\n" +
               "Вам на " + _nextFirstLetter.ToUpper();
    }

    private String GiveUp()
    {
        IsEnd = true;
        var text = "Игра закончена.";
        if (_possibleNextWord != "") text += "\nВы могли бы продолжить, если бы назвали " + _possibleNextWord;
        return text;
    }

    private String CheckUserTurn(String userTurn)
    {
        if (userTurn == "")
            return "Напишите что-нибудь";

        if(!IsFirstLetterMatches(userTurn))
            return "Город должен начинаться на " + _nextFirstLetter;

        if(!IsCityExist(userTurn)) 
            return "Такого города не существует или он написан неправильно";
        
        if(!IsCityRemaining(userTurn)) 
            return "Такой город уже был в этой игре, напишите другой";

        return "";
    }

    #region Check Is...
    private bool IsFirstLetterMatches(String cityName)
    {
        var userFirstLetter = cityName.Substring(0, 1).ToUpper();
        return _nextFirstLetter == "" || userFirstLetter == _nextFirstLetter;
    }
    
    private bool IsCityExist(String cityName)
    {
        var userFirstLetter = cityName.Substring(0, 1).ToUpper();
        if (!_allCitys.ContainsKey(userFirstLetter)) return false;
        
        foreach (var city in _allCitys[userFirstLetter])
        {
            if (city.ToLower() == cityName.ToLower()) return true;
        }

        return false;
    }

    private bool IsCityRemaining(String cityName)
    {
        var userFirstLetter = cityName.Substring(0, 1).ToUpper();
        var remainingCitys = _remainingCitys[userFirstLetter];
        for (int i = 0; i < remainingCitys.Count; i++)
        {
            if (remainingCitys[i].ToLower() == cityName.ToLower())
            {
                remainingCitys.RemoveAt(i);
                return true;
            }
        }
        
        return false;
    }
    #endregion

    #region Finder

    private String FindNextCity(String userTurn)
    {
        FindNextFirstLetter(userTurn);

        if (_remainingCitys[_nextFirstLetter].Count == 0) return "";

        var nextCity = FindPossibleNextCity();
        _remainingCitys[_nextFirstLetter].Remove(nextCity);
        FindNextFirstLetter(nextCity);
        _possibleNextWord = FindPossibleNextCity();

        return nextCity + "\nВам на " + _nextFirstLetter;
    }

    private void FindNextFirstLetter(String currentCity)
    {
        _nextFirstLetter = currentCity.Substring(currentCity.Length - 1, 1).ToUpper();
        if (_nextFirstLetter == "Ь" || _nextFirstLetter == "Ы" || _nextFirstLetter == "Ъ")
            _nextFirstLetter = currentCity.Substring(currentCity.Length - 2, 1).ToUpper();
        if (_nextFirstLetter == "Ё") _nextFirstLetter = "Е";
    }

    private String FindPossibleNextCity()
    {
        if (_remainingCitys[_nextFirstLetter].Count == 0) return "";
        var random = new Random();
        var index = random.Next(_remainingCitys[_nextFirstLetter].Count);
        return _remainingCitys[_nextFirstLetter][index];
    }

    #endregion
}