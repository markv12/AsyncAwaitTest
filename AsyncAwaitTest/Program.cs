using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncAwaitTest
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintWithThreadID("Starting Program");
            Console.WriteLine();

            //Fully Synchronous
            string result = ReadFileHierarchy("file1a");
            Console.WriteLine("____Got Result: " + result);
            Console.WriteLine();

            //Explicitly spawn another thread
            Func<string> simpleReadDelegate = () => ReadFileHierarchy("file1a");
            Task<string> simpleReadTask = Task.Run(simpleReadDelegate);
            string simpleTaskResult = simpleReadTask.GetAwaiter().GetResult();
            Console.WriteLine("____Got Simple Task Result: " + simpleTaskResult);
            Console.WriteLine();

            //Use Async/Await
            Task<string> asyncReadTaskA = ReadFileHierarchyAsync("file1a");
            Task<string> asyncReadTaskB = ReadFileHierarchyAsync("file1b");

            Task<string[]> combinedTask = Task.WhenAll(asyncReadTaskA, asyncReadTaskB);
            string[] combinedTaskResult = combinedTask.GetAwaiter().GetResult();

            for (int i = 0; i < combinedTaskResult.Length; i++)
            {
                Console.WriteLine("____Combined Task Result: " + combinedTaskResult[i]);
            }
        }

        private static string ReadFileHierarchy(string fileOneName)
        {
            PrintWithThreadID("Starting Read");
            fileOneName += ".txt";

            string fileTwoName = File.ReadAllText(fileOneName);
            fileTwoName += ".txt";

            PrintWithThreadID("Got Second File Name: " + fileTwoName);

            string fileThreeName = File.ReadAllText(fileTwoName);
            fileThreeName += ".txt";

            PrintWithThreadID("Got Third File Name: " + fileThreeName);

            string fileThreeContents = File.ReadAllText(fileThreeName);

            return fileThreeContents;
        }

        private static async Task<string> ReadFileHierarchyAsync(string fileOneName)
        {
            PrintWithThreadID("Starting Async Read");
            fileOneName += ".txt";

            Task<string> fileReadTask = File.ReadAllTextAsync(fileOneName);
            string fileTwoName = await fileReadTask;
            //string fileTwoName = await fileReadTask.ConfigureAwait(false); //Don't necessarily return to the same thread/Context
            //string fileTwoName = await Task.Run(() => File.ReadAllText("file1.txt")); //Explicitly spawn another thread
            fileTwoName += ".txt";

            PrintWithThreadID("Got Second File Name Async: " + fileTwoName);

            string fileThreeName = await File.ReadAllTextAsync(fileTwoName);
            fileThreeName += ".txt";

            PrintWithThreadID("Got Third File Name Async: " + fileThreeName);

            string fileThreeContents = await File.ReadAllTextAsync(fileThreeName);

            return fileThreeContents;
        }

        private static void PrintWithThreadID(string text)
        {
            Console.WriteLine(text + " on thread: " + Thread.CurrentThread.ManagedThreadId);
        }
    }
}
