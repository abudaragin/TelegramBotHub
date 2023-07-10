using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotHub.AdminModule;

public class AdminBot: MyBot
{
    private MyBot[] _bots;
    private const int AdminID = 1701638606;

    private String _textToSend;
    private Message _messageToDelete;

    public void SetBots(MyBot[] bots)
    {
        _bots = bots;
    }
    
    protected override string GetToken()
    {
        return Tokens.LeNat81_AdminBotToken;
    }

    public override string GetName()
    {
        return "Админ";
    }

    protected override async Task HandleMessageAsync(ITelegramBotClient botClient, Message message)
    {
        if(_bots == null || message.From == null || message.From.Id != AdminID) return;

        _textToSend = message.Text;
        _messageToDelete = await botClient.SendTextMessageAsync(
            message.Chat, "Кому отправить?", replyMarkup: CreateKeyboard());
    }

    protected override async Task HandleCallbackAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
        if (_messageToDelete != null)
        {
            await botClient.DeleteMessageAsync(_messageToDelete.Chat, _messageToDelete.MessageId);
            _messageToDelete = null;
        }
        
        if (callbackQuery.Data == "back")
        {
            _textToSend = null;
            return;
        }
        
        if (callbackQuery.Data == "all")
        {
            foreach (var bot in _bots)
            {
                bot.Update(_textToSend);
            }
        }
        else
        {
            _bots[Int32.Parse(callbackQuery.Data)].Update(_textToSend);
        }
    }
    
    private InlineKeyboardMarkup CreateKeyboard()
    {
        var buttons = new InlineKeyboardButton[_bots.Length + 2][];
        for (int i = 0; i < _bots.Length; i++)
        {
            buttons[i + 1] = new[] { InlineKeyboardButton.WithCallbackData(_bots[i].GetName(), i.ToString()) };
        }
        buttons[0] = new[] { InlineKeyboardButton.WithCallbackData("Всем", "all") };
        buttons[_bots.Length + 1] = new[] { InlineKeyboardButton.WithCallbackData("Отмена", "back") };

        return new InlineKeyboardMarkup(buttons);
    }
    
    protected override void PreRunPreparation() { }
    protected override async Task HandlePreEventAsync(ITelegramBotClient botClient, Update update) { }
    protected override async Task HandlePostEventAsync(ITelegramBotClient botClient, Update update) { }
}