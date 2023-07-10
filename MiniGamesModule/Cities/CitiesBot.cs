using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotHub.MiniGamesModule.Cities;

public class CitiesBot : MyBot
{
    private static readonly Dictionary<long, CitiesGame> Games = new ();

    protected override string GetToken()
    {
        return Tokens.LeNat83_CitysBotToken;
    }
    
    private static String _rules = "Правила: " +
                                   "\nВы начинаете первым." +
                                   "\nКаждый следующий город должен начинаться на последнюю букву предидущего." +
                                   "\nЕсли город заканчивается на 'ь', 'ъ' или 'ы', то следующий город должен " +
                                   "начинаться на предпоследнюю букву." +
                                   "\nЕсли город заканчивается на 'ё', то она меняется на 'е'." +
                                   "\nВ игре участвуют только Русские города.";

    public override string GetName()
    {
        return "Города";
    }

    protected override async Task HandleMessageAsync(ITelegramBotClient botClient, Message message)
    {
        switch (message.Text.ToLower())
        {
            case "/rules":
                await botClient.SendTextMessageAsync(
                    message.Chat, _rules);
                return;
            case "/new_game":
                Games[message.From.Id] = new CitiesGame();
                await botClient.SendTextMessageAsync(message.Chat, "Новая игра." +
                                                                   "\nВаш ход");
                return;
            default:
                if (Games.ContainsKey(message.From.Id) &&
                    !Games[message.From.Id].IsEnd)
                {
                    await botClient.SendTextMessageAsync(message.Chat,
                        Games[message.From.Id].GetNextTurn(message.Text));
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat,
                        "Игра еще не начата." +
                        "\nВыберите в меню пункт 'Новая игра'");
                }
                return;
        }
    }

    protected override void PreRunPreparation() { }
    protected override async Task HandleCallbackAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery) { }
    protected override async Task HandlePreEventAsync(ITelegramBotClient botClient, Update update) { }
    protected override async Task HandlePostEventAsync(ITelegramBotClient botClient, Update update) { }
}