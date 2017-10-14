using System;
using ContentManagerModels.Models;

namespace ContentScriptCreator
{
    class Program
    {
        static async void Main(string[] args)
        {
            var model = new ContentsModel();
            if (args.Length < 1)
            {
                Console.WriteLine("コンテンツスクリプトクリエーター");
            }
        }
    }
}
