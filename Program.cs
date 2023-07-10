using TelegramBotHub.AdminModule;
using TelegramBotHub.MiniGamesModule._2048;
using TelegramBotHub.MiniGamesModule.BullsAndCows;
using TelegramBotHub.MiniGamesModule.Cities;
using TelegramBotHub.MiniGamesModule.TicTacToe;
using TelegramBotHub.MiniGamesModule.Zonk;

namespace TelegramBotHub
{
    class Program
    {
        //Главная точка инициализации всех ботов
        private static MyBot[] _bots = new MyBot[]
        {
            new MainHubBot(),
            new CitiesBot(),
            new TicTacToeBot(),
            new BullsAndCowsBot(),
            new The2048Bot(),
            new ZonkBot(),
        };

        static void Main(string[] args)
        {
            var adminBot = new AdminBot();
            adminBot.SetBots(_bots);
            
            foreach (var bot in _bots)
                new Thread(() => bot.Run()).Start();
            new Thread(() => adminBot.Run()).Start();
        }
    }
}