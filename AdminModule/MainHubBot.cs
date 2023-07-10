using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotHub.AdminModule;

public class MainHubBot : MyBot
{
    private static Message _messageNeedDelete;

    protected override string GetToken()
    {
        return Tokens.LeNat80_HubBotToken;
    }

    public override string GetName()
    {
        return "Хаб";
    }

    protected override async Task HandleMessageAsync(ITelegramBotClient botClient, Message message)
    {
        switch (message.Text.ToLower())
        {
            case "/start":
                await botClient.SendTextMessageAsync(message.Chat, "Hello.");
                break;
        }

        if (_messageNeedDelete != null)
        {
            await botClient.DeleteMessageAsync(_messageNeedDelete.Chat,
                _messageNeedDelete.MessageId);
            _messageNeedDelete = null;
        }

        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[] // first row
            {
                InlineKeyboardButton.WithUrl("💥HIT💥 Мега-крестики-нолики", "t.me/lenat82_bot")
            },
            new[] // second row
            {
                InlineKeyboardButton.WithUrl("🐂🐄 Быки и коровы", "t.me/lenat84_bot")
            },
            new[]
            {
                InlineKeyboardButton.WithUrl("🏬 Города", "t.me/lenat83_bot"),
            },
            new[] 
            {
            InlineKeyboardButton.WithUrl("🎲 Зонк", "t.me/lenat86_bot"),
            InlineKeyboardButton.WithUrl("2️⃣0️⃣4️⃣8️⃣", "t.me/lenat85_bot")
            }
        });
        _messageNeedDelete =
            await botClient.SendTextMessageAsync(message.Chat, "Выберите игру:", replyMarkup: inlineKeyboard);
    }

    protected override void PreRunPreparation()
    {
    }

    protected override async Task HandlePreEventAsync(ITelegramBotClient botClient, Update update)
    {
    }

    protected override async Task HandleCallbackAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
    }

    protected override async Task HandlePostEventAsync(ITelegramBotClient botClient, Update update)
    {
    }
}