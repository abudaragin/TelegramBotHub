using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBotHub.MiniGamesModule.BullsAndCows;

public class BullsAndCowsBot : MyBot
{
    private static readonly Dictionary<long, BullsAndCowsGame> Games = new();
    private static readonly Dictionary<long, BullsAndCowsSettings> Settings = new();
    private static readonly Dictionary<long, BullsAndCowsSettings> EditSettings = new();
    private static readonly Dictionary<long, Message> MessagesToDelete = new();

    private static String _rules = "Правила:" +
                                   "\nВ классическом варианте игра рассчитана на двух игроков. " +
                                   "Каждый из игроков задумывает и записывает тайное 4-значное число с " +
                                   "неповторяющимися цифрами. Игрок, который начинает игру по жребию, " +
                                   "делает первую попытку отгадать число. " +
                                   "\nПопытка — это 4-значное число с неповторяющимися цифрами, сообщаемое противнику. " +
                                   "Противник сообщает в ответ, сколько цифр угадано без совпадения " +
                                   "с их позициями в тайном числе (то есть количество коров) и сколько угадано " +
                                   "вплоть до позиции в тайном числе (то есть количество быков). Например:" +
                                   "\nЗадумано тайное число «3219»." +
                                   "\nПопытка: «2310»." +
                                   "\nРезультат: две «коровы» (две цифры: «2» и «3» — угаданы на неверных позициях) " +
                                   "и один «бык» (одна цифра «1» угадана вплоть до позиции)." +
                                   "Игроки начинают по очереди угадывать число соперника. Побеждает тот," +
                                   "кто угадает число первым, при условии, что он не начинал игру. " +
                                   "\nЕсли же отгадавший начинал игру — его противнику предоставляется последний шанс " +
                                   "угадать последовательность." +
                                   "\nПри игре против бота игрок вводит комбинации одну за другой, " +
                                   "пока не отгадает всю последовательность.";

    protected override string GetToken()
    {
        return Tokens.LeNat84_BullsAndCowsBotToken;
    }

    public override string GetName()
    {
        return "Быки и коровы";
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
                await StartNewGame(botClient, message.Chat, message.From.Id);
                return;
            case "/settings":
                EditSettings[message.From.Id] = new BullsAndCowsSettings();
                MessagesToDelete[message.From.Id] = await botClient.SendTextMessageAsync(
                    message.Chat, "Выберите кол-во цыфр в числе:",
                    replyMarkup: CreateFirstSettingKeyboard());
                return;
            default:
                if (EditSettings.ContainsKey(message.From.Id) &&
                    EditSettings[message.From.Id] != null)
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Игра остановлена. Вернитесь к настройкам.");
                    return;
                }

                if (Games.ContainsKey(message.From.Id) &&
                    !Games[message.From.Id].IsEnd)
                {
                    await botClient.SendTextMessageAsync(message.Chat,
                        Games[message.From.Id].GetTurnResult(message.Text, message.Date));
                    return;
                }

                await botClient.SendTextMessageAsync(message.Chat,
                    "Игра еще не начата." +
                    "\nВыберите в меню пункт 'Новая игра'");
                return;
        }
    }

    private async Task StartNewGame(ITelegramBotClient botClient, Chat chat, long userID)
    {
        if (!Settings.ContainsKey(userID))
            Settings[userID] = new BullsAndCowsSettings();
        Games[userID] = new BullsAndCowsGame(Settings[userID]);
        await botClient.SendTextMessageAsync(chat, "Новая игра." +
                                                   "\nВаш ход");
    }

    protected override async Task HandleCallbackAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
    {
        DeleteMessage(botClient, callbackQuery.From.Id);

        if (callbackQuery.Data == "back")
        {
            EditSettings[callbackQuery.From.Id] = null;
            return;
        }

        var editSetting = EditSettings[callbackQuery.From.Id];
        if (callbackQuery.Data.Substring(0, 1) == "1")
        {
            editSetting.Count = Int32.Parse(callbackQuery.Data.Substring(2, 1));
            MessagesToDelete[callbackQuery.From.Id] =
                await botClient.SendTextMessageAsync(
                    callbackQuery.Message.Chat, "Могут ли цыфры в числе повторяться:",
                    replyMarkup: CreateSecondSettingKeyboard());
            return;
        }

        editSetting.CanRepeated = callbackQuery.Data == "2_yes";
        Settings[callbackQuery.From.Id] = editSetting;
        EditSettings[callbackQuery.From.Id] = null;
        StartNewGame(botClient, callbackQuery.Message.Chat, callbackQuery.From.Id);
    }

    private async Task DeleteMessage(ITelegramBotClient botClient, long userID)
    {
        if (!MessagesToDelete.ContainsKey(userID)) return;
        if (MessagesToDelete[userID] == null) return;
        await botClient.DeleteMessageAsync(MessagesToDelete[userID].Chat, MessagesToDelete[userID].MessageId);
        MessagesToDelete[userID] = null;
    }


    #region Keyboards

    private InlineKeyboardMarkup CreateFirstSettingKeyboard()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[] // first row
            {
                InlineKeyboardButton.WithCallbackData("4", "1_4"),
                InlineKeyboardButton.WithCallbackData("5", "1_5"),
                InlineKeyboardButton.WithCallbackData("6", "1_6"),
            },
            new[] // second row
            {
                InlineKeyboardButton.WithCallbackData("Вернуться к игре без изменений", "back"),
            }
        });
    }

    private InlineKeyboardMarkup CreateSecondSettingKeyboard()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[] // first row
            {
                InlineKeyboardButton.WithCallbackData("да", "2_yes"),
                InlineKeyboardButton.WithCallbackData("нет", "2_no"),
            },
            new[] // second row
            {
                InlineKeyboardButton.WithCallbackData("Вернуться к игре без изменений", "back"),
            }
        });
    }

    #endregion


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