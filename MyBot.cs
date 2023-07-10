using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotHub;

/*  NEW AMPTY BOT:
    protected override string GetToken()
    {
        return "";
    }
    
    public override string GetName()
    {
        return "";
    }

    protected override void PreRunPreparation() { }
    protected override async Task HandleMessageAsync(ITelegramBotClient botClient, Message message) { }
    protected override async Task HandleCallbackAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery) { }
    protected override async Task HandlePreEventAsync(ITelegramBotClient botClient, Update update) { }
    protected override async Task HandlePostEventAsync(ITelegramBotClient botClient, Update update) { }
 */

public abstract class MyBot
{
    #region Abstract zone
    protected abstract String GetToken();
    public abstract String GetName();
    protected abstract void PreRunPreparation();
    protected abstract Task HandleMessageAsync(ITelegramBotClient botClient, Message message);
    protected abstract Task HandleCallbackAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery);
    protected abstract Task HandlePreEventAsync(ITelegramBotClient botClient, Update update);
    protected abstract Task HandlePostEventAsync(ITelegramBotClient botClient, Update update);
    #endregion

    #region Work zone
    private List<Chat> _chats;
    private ITelegramBotClient _botClient;

    public void Run()
    {
        _chats = new List<Chat>();
        var bot = new TelegramBotClient(GetToken());
        Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);
        PreRunPreparation();
        
        var cancellationToken = new CancellationTokenSource().Token;
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }, // receive all update types
        };
        bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );
        
        Console.ReadLine();
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(GetName() + ": " + JsonConvert.SerializeObject(update));
        _botClient = botClient;

        if (CheckSuperGroup(update))
        {
            if(update.Message != null) await botClient.SendTextMessageAsync(update.Message.Chat, "Удалите бота из группы.");
            return;
        }

        if(update.Message != null && !_chats.Contains(update.Message.Chat))
            _chats.Add(update.Message.Chat);

        try
        {
            await HandlePreEventAsync(botClient, update);
            if (update.Type == UpdateType.Message
                && update.Message != null
                && update.Message.Text != null)
            {
                await HandleMessageAsync(botClient, update.Message);
            }
            if (update.Type == UpdateType.CallbackQuery 
                && update.CallbackQuery != null 
                && update.CallbackQuery.Data != null)
            {
                await HandleCallbackAsync(botClient, update.CallbackQuery);
            }
            await HandlePostEventAsync(botClient, update);
        }
        catch (Exception exception)
        {
            Console.WriteLine(JsonConvert.SerializeObject(exception));
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(JsonConvert.SerializeObject(exception));
        return Task.CompletedTask;
    }

    public void Update(String adminMessage)
    {
        if(_botClient == null) return;
        
        for (int i = 0; i < _chats.Count; i++)
        {
            try
            {
                _botClient.SendTextMessageAsync(_chats[i], adminMessage);
            }
            catch (Exception exception)
            {
                _chats.RemoveAt(i);
                i--;
            }
        }
    }
    #endregion

    private bool CheckSuperGroup(Update update)
    {
        if (update.Message != null && update.Message.Chat.Type == ChatType.Supergroup) return true;
        if (update.EditedMessage != null && update.EditedMessage.Chat.Type == ChatType.Supergroup) return true;
        return false;
    }
}
