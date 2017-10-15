using System;
using System.Configuration;
using ContentManagerModels.Models;
using ContentScriptCreator.Properties;

namespace ContentScriptCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            var model = new ContentsModel();
            Console.Write("スピーカー情報出力：");
            Console.WriteLine(model.CreateSpeakersAsync(Settings.Default.SpeakerYml).Result ? "成功" : "失敗");
            Console.Write("セッション情報出力：");
            Console.WriteLine(model.CreateSessionsAsync(Settings.Default.SessionInfoPath).Result ? "成功" : "失敗");
            Console.ReadLine();
        }
    }
}
