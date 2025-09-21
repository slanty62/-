using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using var cts = new CancellationTokenSource();
var bot = new TelegramBotClient("8205253796:AAEdfaPgxTi76Xta5F-fWcfb-ezoC9sRet4");

Console.WriteLine("Шеф-повар бот запущен! Enter для остановки!");

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

bot.StartReceiving(updateHandler: HandleUpdateAsync,
                   HandlePollingErrorAsync,
                   receiverOptions: receiverOptions,
                   cancellationToken: cts.Token);

Console.ReadLine();
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Message is not { } message || message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;
    var userName = message.From.FirstName;

    switch (messageText.ToLower())
    {
        case "/start":
        case "привет":
        case "здравствуй":
            // 1. Приветствие
            await botClient.SendMessage(
                chatId: chatId,
                text: $"Здравствуй, {userName}! Я твой шеф-повар!",
                cancellationToken: cancellationToken);
            break;

        case "рецепт":
        case "картинка":
        case "фото":
            // Картинка блюда
            await botClient.SendPhoto(
                chatId: chatId,
                photo: InputFile.FromUri("https://s00.yaplakal.com/pics/pics_original/8/3/2/101238.jpg"),
                caption: "Вот изысканное блюдо из остатков! Яички с кепчуком!",
                cancellationToken: cancellationToken);
            break;

        case "видео":
        case "видеорецепт":
            // Видео рецепт
            await botClient.SendVideo(
                chatId: chatId,
                video: InputFile.FromUri("https://t.me/Kulinariya_retsept/370?t=0"),
                caption: "Видео-рецепт: Как приготовить шедевр из остатков",
                cancellationToken: cancellationToken);
            break;

        case "стикер":
        case "смайлик":
            // Стикер
            await botClient.SendSticker(
                chatId: chatId,
                sticker: InputFile.FromString("CAACAgIAAxkBAAESezFoz-SZmng8PJwSpaAtCYJs4-UMCwACNRwAAhK6sEjQlLasrSHkyDYE"),
                cancellationToken: cancellationToken);
            break;

        case "меню":
        case "кнопки":
        case "выбор":
            // Кнопки
            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "Рецепт", "Видео" },
                new KeyboardButton[] { "Стикер", "Меню" }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };

            await botClient.SendMessage(
                chatId: chatId,
                text: $"{userName}, выбери что тебе интересно:",
                replyMarkup: keyboard,
                cancellationToken: cancellationToken);
            break;

        default:
            await botClient.SendMessage(
                chatId: chatId,
                text: $"{userName}, не совсем понимаю. Попробуй: привет, рецепт, видео, стикер или меню",
                cancellationToken: cancellationToken);
            break;
    }
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var errorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Ошибка Telegram API: {apiRequestException.ErrorCode} - {apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(errorMessage);
    return Task.CompletedTask;
}


