using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace Example_931
{
    class Program
    {
        static List<string> files = new List<string>();
        static string path = AppDomain.CurrentDomain.BaseDirectory + @"\FilesDownloads\";
        static TelegramBotClient bot;
        const string token = "";

        static void Main(string[] args)
        {
            LoadList();
            bot = new TelegramBotClient(token);
            bot.OnMessage += MessageListener;
            bot.StartReceiving();
            Console.ReadKey();
        }

        private static void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string text = $"{DateTime.Now.ToLongTimeString()}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";

            Console.WriteLine($"{text} TypeMessage: {e.Message.Type.ToString()}");
            

            switch (e.Message.Text)
            {
                case "/start":
                    {
                        bot.SendTextMessageAsync(e.Message.Chat.Id, "Добро пожаловать на борт, добрый путник!");
                        bot.SendTextMessageAsync(e.Message.Chat.Id, "Введите /random чтобы получить рандомное число" +
                            "Введите /content чтобы посмотреть список загруженных файлов" +
                            "Отправьте файл и я его сохраню" +
                            "Введите /file + его имя и я вам его отправлю");
                        break;
                    }
                case "/random":
                    {
                        Random rnd = new Random();
                        bot.SendTextMessageAsync(e.Message.Chat.Id, $"Выпало число {rnd.Next(-1000, 1000)}");
                        break;
                    }
                case "/content":
                    {
                        
                        foreach (var fileInfo in files)
                        {
                            bot.SendTextMessageAsync(e.Message.Chat.Id, $"{fileInfo}");
                        }
                        break;
                    }
                case null:
                    {
                        break;
                    }
            }
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Document)
            {
                Console.WriteLine(e.Message.Document.FileId);
                Console.WriteLine(e.Message.Document.FileName);
                Console.WriteLine(e.Message.Document.FileSize);


                DownLoad(e.Message.Document.FileId, e.Message.Document.FileName);
                bot.SendTextMessageAsync(e.Message.Chat.Id, $"Файл загружен!!!");
                files.Add(e.Message.Document.FileName);
            }

            if (files.Contains(e.Message.Text))
            {
                SendFiles(e.Message.Chat.Id, e.Message.Text);
            }
        }
        static async void LoadList()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            FileInfo[] fileInfoGroup = dirInfo.GetFiles();
            foreach (var fileInfo in fileInfoGroup)
            {
                files.Add(fileInfo.Name);
            }
        }
        static async void DownLoad(string fileId, string fileName)
        {
            var file = await bot.GetFileAsync(fileId);
            FileStream fs = new FileStream(path + fileName, FileMode.OpenOrCreate);
            await bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();

            fs.Dispose();
        }
        static async void SendFiles(long id, string fileName)
        {
            using (var stream = System.IO.File.OpenRead(path + fileName))
            {
                InputOnlineFile iof = new InputOnlineFile(stream);
                iof.FileName = fileName;
                var send = await bot.SendDocumentAsync(id, iof, "Текст сообщения под файлом");
            }
        }
    }
}
