using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotHub.MiniGamesModule.Zonk;

public class ZonkBot: MyBot
{
    private static Dictionary<long, ZonkGame> _games = new();
    private static Dictionary<long, Message> _messageWithButtons = new();

    private static String _rules =
        "Правила:" +
        "\nВ зонк играют 2 игрока и более, в таком случае счет ведется до 5 000 или 10 000 очков. " +
        "Если играет один игрок, он стремится набрать как можно больше очков за 10 раундов, " +
        "зачастую до определенной планки (например, 5 000 очков)." +
        "\nВ начале каждого раунда игрок бросает все кости." +
        "\nПосле каждого броска он должен оставить кости в определенных комбинациях, которые принесут ему очки." +
        "Игрок может остановить свой ход (если за раунд он набрал 300 очков или более). Тогда" +
        "его очки записываются в таблицу. Но у него есть право продолжать бросать кости, однако со временем" +
        "вероятность выпадение зонка (отсутствие комбинаций из оставшихся костей) постепенно увеличивается." +
        "\nЕсли за раунд все 6 костей были собраны в различные комбинации, игрок получает право на призовой бросок, от" +
        "которого он имеет право отказаться." +
        "\nЕсли во время любого броска оставшиеся кости собрать в комбинации нельзя, - эта ситуация называется зонк" +
        " и все очки игрока, которые он заработал за раунд, сгорают. После трёх зонков подрядигрок теряет 500 очков." +
        "\nПосле того, как очки за раунд были сохранены и записаны, ход передается следующему игроку (" +
        "обычно по часовой стрелке)" +
        "\nКак только игрок добился количества очков, необходимых для победы, остальные игроки имеют право на еще" +
        "один раунд, чтобы превзойти эту планку. Если ни одному из противников сделать этого не удалось или они отказались" +
        "от этого права, этому игроку присуждается победа.";

    private static String _combinations = 
        "Комбинации: " +
        "\n1️⃣ - 100 очков за каждую" +
        "\n5️⃣ - 50 очков ща каждую" +
        "\n1️⃣1️⃣1️⃣ - 1000 очков" +
        "\n2️⃣2️⃣2️⃣ - 200 очков" +
        "\n3️⃣3️⃣3️⃣ - 300 очков" +
        "\n4️⃣4️⃣4️⃣ - 400 очков" +
        "\n5️⃣5️⃣5️⃣ - 500 очков" +
        "\n6️⃣6️⃣6️⃣ - 600 очков" +
        "\n3️⃣3️⃣3️⃣ + 3️⃣ - 300+300 (это касается и пятой, и шестой кости)" +
        "\n2️⃣2️⃣4️⃣4️⃣6️⃣6️⃣ - 750 очков" +
        "\n1️⃣2️⃣3️⃣4️⃣5️⃣6️⃣ - 1500 очков";

    protected override string GetToken()
    {
        return Tokens.LeNat86_ZonkBotToken;
    }
    
    public override string GetName()
    {
        return "Зонк";
    }

    protected override async Task HandleMessageAsync(ITelegramBotClient botClient, Message message)
    {
        switch (message.Text.ToLower())
        {
            case "/new_game":
            case "/start":
                var game = new ZonkGame();
                _games[message.From.Id] = game;
                _messageWithButtons[message.From.Id] = await botClient.SendTextMessageAsync(
                    message.Chat,
                    game.GetReport(),
                    replyMarkup: CreateKeyboard(game));
                return;
            case "/rules":
                await botClient.SendTextMessageAsync(message.Chat, _rules);
                return;
            case "/combinations":
                await botClient.SendTextMessageAsync(message.Chat, _combinations);
                return;
            default:
                await botClient.SendTextMessageAsync(message.Chat,
                    "Игра еще не начата." +
                    "\nВыберите в меню пункт 'Новая игра'");
                return;
        }
    }

    private InlineKeyboardMarkup CreateKeyboard(ZonkGame game)
    {
        var buttons = new List<InlineKeyboardButton>();
        if (game.CanStop()) buttons.Add(InlineKeyboardButton.WithCallbackData("Stop", "stop"));
        buttons.Add(InlineKeyboardButton.WithCallbackData("Continue", "continue"));
        return new InlineKeyboardMarkup(buttons);
    }

    protected override async Task HandleCallbackAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
        var message = _messageWithButtons[callbackQuery.From.Id];
        if (message != null)
            botClient.EditMessageReplyMarkupAsync(message.Chat.Id,
                message.MessageId, replyMarkup: null);
        ;

        var game = _games[callbackQuery.From.Id];
        switch (callbackQuery.Data)
        {
            case "stop":
                game.StopRound();
                break;
            case "continue":
                game.Continue();
                break;
            default:
                throw new Exception();
        }

        if (!game.IsEnd())
            _messageWithButtons[callbackQuery.From.Id] = await botClient.SendTextMessageAsync(
                callbackQuery.Message.Chat,
                game.GetReport(),
                replyMarkup: CreateKeyboard(game));
    }

    protected override void PreRunPreparation()
    {
    }

    protected override async Task HandlePreEventAsync(ITelegramBotClient botClient, Update update)
    {
    }

    protected override async Task HandlePostEventAsync(ITelegramBotClient botClient, Update update)
    {
    }
}