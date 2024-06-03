using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BoltBot
{
    internal class BotService
    {
        private readonly TelegramBotClient botClient = new("7478941975:AAF9nPwf8FWy2033T1XGD35M4Z-dhv4SeUM");

        public BotService()
        {
            botClient.StartReceiving(OnUpdate, OnError);
        }

        private async Task OnUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type != UpdateType.Message || update?.Message?.Text is null)
                return;

            var message = update.Message;
            var chatId = message.Chat.Id;

            Console.WriteLine($"Received a '{message.Text}' message in chat {chatId} from {message.Chat.FirstName}.");

            if (message.Text.StartsWith("/"))
            {
                await HandleCommandAsync(botClient, message, cancellationToken);
            }
        }

        static async Task SetBotCommandsAsync(ITelegramBotClient botClient)
        {
            var commands = new List<BotCommand>
            {
                new() { Command = "start", Description = "Запустити бота" },
                new() { Command = "help", Description = "Показати допомогу" },
                new() { Command = "info", Description = "Інформація про бота" },
                new() { Command = "share", Description = "Поділитись номером телефону" }
            };

            await botClient.SetMyCommandsAsync(commands);
        }

        private async Task HandleCommandAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;

            switch (message?.Text?.Split(' ')[0]) // Отримуємо команду без аргументів
            {
                case "/start":
                    await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: $"Вітаю! 👋\nЯ готовий вам допомогти орендувати різні види транспорту. Введіть /help, щоб дізнатися більше.", cancellationToken: cancellationToken);

                    Message message1 = await botClient.SendPhotoAsync(
                    chatId: chatId,
                    photo: InputFile.FromUri("https://img.freepik.com/free-vector/eco-transport-isometric-set-with-electric-car-scooter-bicycle-segway-gyro-isolated-decorative-icons_1284-26725.jpg"),
                    cancellationToken: cancellationToken);

                    break;
                case "/help":
                    await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Доступні команди:\n/start - Запустити бота\n/help - Показати допомогу\n/info - Інформація про бота\n/share - Поділитись контактами", cancellationToken: cancellationToken);
                    break;
                case "/info":
                    await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Я інформаційний бот.", cancellationToken: cancellationToken);
                    break;
                case "/share":
                    await botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: "Виберіть один з наявних варіантів...", cancellationToken: cancellationToken);
                    if (message.Text == "/share")
                    {
                        // Створюємо клавіатуру з кнопкою "Share Contact"
                        ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
                        {
                        new KeyboardButton[] { KeyboardButton.WithRequestContact("Поділитись контактним номером") }
                    });
                        replyKeyboardMarkup.ResizeKeyboard = true;

                        // Відправляємо повідомлення з клавіатурою
                        await botClient.SendTextMessageAsync
                        (
                            chatId: message.Chat.Id,
                            text: "Поділіться контактними даними, щоб ми могли з вами зв'язатись!",
                            replyMarkup: replyKeyboardMarkup,
                            cancellationToken: cancellationToken
                        );
                    }
                    break;
                default:
                    await botClient.SendTextMessageAsync(chatId: message.Chat.Id, replyMarkup: null, text: "Невідома команда.", cancellationToken: cancellationToken);
                    break;
            }
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