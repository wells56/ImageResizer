using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageResizer
{
    class Program
    {
        static void Main(string[] args)
        {
            string sourcePath = Path.Combine(Environment.CurrentDirectory, "images");
            string destinationPath = Path.Combine(Environment.CurrentDirectory, "output"); ;

            ImageProcess imageProcess = new ImageProcess();

            imageProcess.Clean(destinationPath);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            // 原始同步程式
            //imageProcess.ResizeImages(sourcePath, destinationPath, 2.0);

            // 非同步程式
            //Task task = imageProcess.ResizeImagesAsync(sourcePath, destinationPath, 2.0);

            CancellationTokenSource cts = new CancellationTokenSource();
            #region 等候使用者輸入 取消 c 按鍵
            ThreadPool.QueueUserWorkItem(x =>
            {
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key == ConsoleKey.C)
                {
                    cts.Cancel();
                }
            });
            #endregion

            try
            {
                // 增加取消Token
                Task task = imageProcess.ResizeImagesAsyncCanceled(sourcePath, destinationPath, 2.0, cts.Token);
                task.Wait();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"{Environment.NewLine}轉圖已經取消");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Environment.NewLine}發現例外異常 {ex.Message}");
            }

            sw.Stop();

            Console.WriteLine($"花費時間: {sw.ElapsedMilliseconds} ms");
        }
    }
}
