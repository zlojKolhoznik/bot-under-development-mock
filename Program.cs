using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var botClient = new TelegramBotClient(System.Configuration.ConfigurationManager.AppSettings["BotToken"]);
using var cts = new CancellationTokenSource();
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>()
};

botClient.StartReceiving(HandleUpdatesAsync, HandleErrorsAsync, receiverOptions, cts.Token);

Task HandleErrorsAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    botClient.DeleteWebhookAsync(true, cts.Token).Wait();
    var updates = botClient.GetUpdatesAsync(cancellationToken: cancellationToken).Result;
    if (updates.Length > 0)
    {
    }
    Console.WriteLine($"[{DateTime.Now}] Error: {exception.Message}");
    return Task.CompletedTask;
}

async Task HandleUpdatesAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
{
    var botUser = await client.GetMeAsync(cancellationToken: cancellationToken);

    if (update is { Type: UpdateType.Message, Message.Type: MessageType.Text } &&
        update.Message?.Text == $"@{botUser.Username}")
    {
        await client.SendTextMessageAsync(chatId: update.Message.Chat.Id,
            text: "Бот знаходиться на технічному обслуговуванні. Спробуйте знову за декілька годин",
            cancellationToken: cancellationToken);
    }
}