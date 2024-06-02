using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace BoltBot
{
    internal class BotService
    {
        TelegramBotClient botClient = new TelegramBotClient("7478941975:AAF9nPwf8FWy2033T1XGD35M4Z-dhv4SeUM");

        public BotService()
        {
            botClient.StartReceiving(OnUpdate, OnError);
        }

        private async Task OnUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;
            // Only process text messages
            if (message.Text is not { } messageText)
                return;

            var chatId = message.Chat.Id;

            Console.WriteLine($"Received a '{messageText}' message in chat {chatId} from {message.Chat.FirstName}.");

            // Echo received message text
            Message sentMessage = await botClient.SendTextMessageAsync
                (
                chatId: chatId,
                text: "You said:\n" + messageText,
                cancellationToken: cancellationToken
                );
        }

        private Task OnError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}