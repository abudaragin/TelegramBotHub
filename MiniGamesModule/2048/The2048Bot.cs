using System.Drawing;
using System.Drawing.Imaging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotHub.MiniGamesModule._2048;

public class The2048Bot : MyBot
{
    private static Dictionary<long, The2048Game> _games = new();
    private static Dictionary<long, Message> _messagesToDelete = new();

    private static String _rules = "Правила:";

    protected override string GetToken()
    {
        return Tokens.LeNat85_2048BotToken;
    }

    public override string GetName()
    {
        return "2048";
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
                var game = new The2048Game(new The2048Drawer());
                _games[message.From.Id] = game;
                await SendGameView(botClient, message.Chat, game, message.From.Id);
                return;
            default:
                await botClient.SendTextMessageAsync(message.Chat,
                    "Игра еще не начата." +
                    "\nВыберите в меню пункт 'Новая игра'");
                return;
        }
    }
    
    private async Task SendGameView(ITelegramBotClient botClient, Chat chat, The2048Game game, long userID)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        using (Bitmap finalImage = game.DrawView())
        {
            var keyboard = game.IsEnd ? null : CreateKeyboard();
            
            finalImage.Save(memoryStream, ImageFormat.Png);
            memoryStream.Position = 0;
            _messagesToDelete[userID] = await botClient.SendPhotoAsync(chat, new InputFile(memoryStream, "image.png"), 0, 
                replyMarkup: keyboard
            );
            
            if (game.IsEnd)
                await botClient.SendTextMessageAsync(chat, "Игра окончена");
        }
    }
    
    private InlineKeyboardMarkup CreateKeyboard()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[] // first row
            {
                InlineKeyboardButton.WithCallbackData("⬆️", "up"),
            },
            new[] // second row
            {
                InlineKeyboardButton.WithCallbackData("⬅️", "left"),
                InlineKeyboardButton.WithCallbackData("➡️", "rigth"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("⬇️", "down"),
            },
        });
    }

    protected override async Task HandleCallbackAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
        if (callbackQuery.Data == "back")
        {
            return;
        }
        DeleteMessage(botClient, callbackQuery.From.Id);
        var game = _games[callbackQuery.From.Id];
        game.SetTurn(callbackQuery.Data);
        await SendGameView(botClient, callbackQuery.Message.Chat, game, callbackQuery.From.Id);
    }

    private async Task DeleteMessage(ITelegramBotClient botClient, long userID)
    {
        if (!_messagesToDelete.ContainsKey(userID)) return;
        if (_messagesToDelete[userID] == null) return;
        await botClient.DeleteMessageAsync(_messagesToDelete[userID].Chat, _messagesToDelete[userID].MessageId);
        _messagesToDelete[userID] = null;
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