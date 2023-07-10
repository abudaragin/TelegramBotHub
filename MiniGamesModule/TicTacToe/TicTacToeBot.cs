using System.Drawing;
using System.Drawing.Imaging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotHub.MiniGamesModule.TicTacToe;

public class TicTacToeBot : MyBot
{
    private static Dictionary<long, TicTacToeGame> _games = new ();
    private static Dictionary<long, List<Message>> _userTurnsSafe = new ();
    private static Dictionary<long, List<Message>> _botTurnsSafe = new ();

    private static String _rules = "Правила: 💥HIT💥" +
                                   "\nИгра ведется на поле 9 на 9 клеток, разделенном на 9 блоков размером 3 на 3." +
                                   "\nДля победы в игре нужно образовать линию (вертикальную, горизонтальную" +
                                   "или диагональную) из 3 \"закрытых\" своим значением блоков." +
                                   "\nДля \"закрытия\" блока нужно внутри блока выиграть в классические крестики-нолики." +
                                   "\n \nПравила хода:" +
                                   "\nПервый игрок ходит в любую клетку центрального блока." +
                                   "\nСледующий игрок обязан походить в тот блок, в какую клетку внутри блока ходили до него." +
                                   "Например, если первый игрок походил в верхнюю левую клетку блока, " +
                                   "то второй игрок должен сделать ход в верхнем левом блоке." +
                                   "\nЕсли блок, в который нужно походить, \"закрыт\", или в нем нет свободных клеток," +
                                   "то игрок может сделать ход в любую свободную клетку на поле.";

    protected override string GetToken()
    {
        return Tokens.LeNat82_MegaTicTacToeBotToken;
    }

    public override string GetName()
    {
        return "Мега-крестики-нолики";
    }

    protected override void PreRunPreparation()
    {
        _games = new Dictionary<long, TicTacToeGame>();
        _userTurnsSafe = new Dictionary<long, List<Message>>();
        _botTurnsSafe = new Dictionary<long, List<Message>>();
    }

    protected override async Task HandleMessageAsync(ITelegramBotClient botClient, Message message)
    {
        switch (message.Text.ToLower())
        {
            case "/new_game":
            case "/start":
                var game = new TicTacToeGame();
                _games[message.From.Id] = game;
                await botClient.SendTextMessageAsync(message.Chat, "Новая игра.");
                await SendGameReport(botClient, message.Chat, game, true, message.From.Id);
                await SendTextWithKeyboard(botClient, message.Chat, game, message.From.Id);
                return;
            case "/rules":
                await botClient.SendTextMessageAsync(message.Chat, _rules);
                return;
            default:
                if (_games.ContainsKey(message.From.Id) &&
                    !_games[message.From.Id].IsEnd)
                {
                    await botClient.SendTextMessageAsync(message.Chat, "играем...");
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

    private InlineKeyboardMarkup CreateTableKeyboard(bool[,] possibleChoices, bool canMoveUndo)
    {
        int lines = canMoveUndo ? 4 : 3;
        var buttons = new InlineKeyboardButton[lines][];
        for (int y = 0; y < 3; y++)
        {
            buttons[y] = new InlineKeyboardButton[3];
            for (int x = 0; x < 3; x++)
            {
                if (possibleChoices[x, y])
                    buttons[y][x] = InlineKeyboardButton.WithCallbackData("X", "" + x + y);
                else
                    buttons[y][x] = InlineKeyboardButton.WithCallbackData(" ");
            }
        }

        if (canMoveUndo)
            buttons[3] = new[] { InlineKeyboardButton.WithCallbackData("undo", "undo") };

        return new InlineKeyboardMarkup(buttons);
    }

    protected override async Task HandleCallbackAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
        var game = _games[callbackQuery.From.Id];
        var needAITurn = !game.IsChoiceBlock && callbackQuery.Data != "undo";

        game.SetUserChoice(callbackQuery.Data);
        await DeleteMessages(botClient, callbackQuery.From.Id, _userTurnsSafe);
        await SendGameReport(botClient, callbackQuery.Message.Chat, game, true, callbackQuery.From.Id);

        if (needAITurn && !game.IsEnd)
        {
            await Task.Delay(500);
            game.CreateAITurn();
            await DeleteMessages(botClient, callbackQuery.From.Id, _botTurnsSafe);
            await SendGameReport(botClient, callbackQuery.Message.Chat, game, false, callbackQuery.From.Id);
        }

        if (!game.IsEnd) await SendTextWithKeyboard(botClient, callbackQuery.Message.Chat, game, callbackQuery.From.Id);
    }

    private async Task SendTextWithKeyboard(ITelegramBotClient botClient, Chat chat, TicTacToeGame game,
        long userID)
    {
        var text = game.IsChoiceBlock ? "Выберите блок:" : "Выберите ячейку:";
        var inlineKeyboard = CreateTableKeyboard(game.GetUserChoices(), game.CanUserMoveUndo());
        var dictionary = PrepareDictionary(userID, true);
        dictionary[userID].Add(
            await botClient.SendTextMessageAsync(chat, text, replyMarkup: inlineKeyboard));
    }

    private async Task SendGameReport(ITelegramBotClient botClient, Chat chat, TicTacToeGame game,
        bool isUserTurn, long userID)
    {
        await SendGameView(botClient, chat, game, isUserTurn, userID);
        if (game.IsEnd)
            await botClient.SendTextMessageAsync(chat, game.GameResult);
    }

    private async Task SendGameView(ITelegramBotClient botClient, Chat chat, TicTacToeGame game,
        bool isUserTurn, long userID)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        using (Bitmap finalImage = game.DrawView())
        {
            finalImage.Save(memoryStream, ImageFormat.Png);
            memoryStream.Position = 0;

            var dictionary = PrepareDictionary(userID, isUserTurn);
            /*var text = isUserTurn ? "Ваш ход:" : "Ход бота:";
            dictionary[userID].Add(
                await botClient.SendTextMessageAsync(chat, text));*/
            dictionary[userID].Add(
                await botClient.SendPhotoAsync(chat, new InputFile(memoryStream, "image.png"), 0));
        }
    }

    private async Task DeleteMessages(ITelegramBotClient botClient, long userID,
        Dictionary<long, List<Message>> messeges)
    {
        if (!messeges.ContainsKey(userID)) return;
        foreach (var message in messeges[userID])
        {
            await botClient.DeleteMessageAsync(message.Chat, message.MessageId);
        }

        messeges[userID].Clear();
    }

    private Dictionary<long, List<Message>> PrepareDictionary(long id, bool isUserTurn)
    {
        var dictionary = _botTurnsSafe;
        if (isUserTurn) dictionary = _userTurnsSafe;
        if (!dictionary.ContainsKey(id))
            dictionary[id] = new List<Message>();
        return dictionary;
    }

    protected override async Task HandlePreEventAsync(ITelegramBotClient botClient, Update update)
    {
    }

    protected override async Task HandlePostEventAsync(ITelegramBotClient botClient, Update update)
    {
    }
}