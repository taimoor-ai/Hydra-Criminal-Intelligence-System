using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Text;
using CriminalRecordMS.Services;
using CriminalRecordMS.Utilities;

namespace CriminalRecordMS
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Splash Screen
            ShowHydraSplashScreen();

            // 2. System Settings
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "Criminal Record Management System — CRMS v1.0";

            var store = new DataStore();
            var menu = new MenuController(store);

            try
            {
                store.LoadAll();
                if (store.Criminals.Count == 0 && store.Cases.Count == 0)
                {
                    store.SeedSampleData();
                }

                if (menu.ShowLoginScreen())
                {
                    menu.ShowMainMenu();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.ReadKey();
            }
        }

        // --- SPLASH SCREEN METHOD ---
        //static void ShowHydraSplashScreen()
        //{
        //    Console.Clear();
        //    Console.CursorVisible = false;
        //    string path = "logo.jpeg";

        //    if (File.Exists(path))
        //    {
        //        using (Bitmap bmp = new Bitmap(path))
        //        {
        //            int width = 45;
        //            int height = 16;
        //            Bitmap resized = new Bitmap(bmp, new Size(width, height));

        //            int padLeft = (Console.WindowWidth - width) / 2;
        //            int padTop = 2;

        //            for (int y = 0; y < resized.Height; y++)
        //            {
        //                Console.SetCursorPosition(Math.Max(0, padLeft), padTop + y);
        //                for (int x = 0; x < resized.Width; x++)
        //                {
        //                    Color p = resized.GetPixel(x, y);
        //                    int gray = (int)(p.R * 0.3 + p.G * 0.59 + p.B * 0.11);
        //                    char[] ramp = { ' ', '.', ':', '-', '+', '*', '#', '@' };
        //                    int index = (gray * (ramp.Length - 1)) / 255;
        //                    Console.Write($"\u001b[38;2;0;{gray};0m{ramp[index]}");
        //                }
        //            }
        //        }
        //    }

        //    Console.ResetColor();
        //    int statusPadding = (Console.WindowWidth - 35) / 2;
        //    Console.SetCursorPosition(statusPadding, Console.CursorTop + 2);

        //    // Calling the helper method
        //    PrintSmallStatus("Connecting to Hydra Mainframe...", statusPadding);
        //    PrintSmallStatus("Bypassing Encryption...", statusPadding);
        //    PrintSmallStatus("Access Granted.", statusPadding);

        //    Console.ForegroundColor = ConsoleColor.Cyan;
        //    string title = ">>> HYDRA CRIMINAL INTELLIGENCE  SYSTEM <<<";
        //    Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2, Console.CursorTop + 1);

        //    foreach (char l in title) { Console.Write(l); Thread.Sleep(25); }

        //    Console.WriteLine("\n");
        //    Console.ForegroundColor = ConsoleColor.DarkGray;
        //    string prompt = "[ Press any key to initialize login ]";
        //    Console.SetCursorPosition((Console.WindowWidth - prompt.Length) / 2, Console.CursorTop + 1);
        //    Console.Write(prompt);

        //    Console.ReadKey(true);
        //    Console.Clear();
        //    Console.CursorVisible = true;
        //}
        static void ShowHydraSplashScreen()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            string[] logo =
            {
        @"██╗  ██╗██╗   ██╗██████╗ ██████╗  █████╗ ",
        @"██║  ██║╚██╗ ██╔╝██╔══██╗██╔══██╗██╔══██╗",
        @"███████║ ╚████╔╝ ██║  ██║██████╔╝███████║",
        @"██╔══██║  ╚██╔╝  ██║  ██║██╔══██╗██╔══██║",
        @"██║  ██║   ██║   ██████╔╝██║  ██║██║  ██║",
        @"╚═╝  ╚═╝   ╚═╝   ╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝",
        @"",
        @"      CRIMINAL INTELLIGENCE SYSTEM"
    };

            int top = (Console.WindowHeight - logo.Length) / 2;

            for (int i = 0; i < top; i++)
                Console.WriteLine();

            foreach (var line in logo)
            {
                int left = (Console.WindowWidth - line.Length) / 2;
                Console.WriteLine(new string(' ', Math.Max(0, left)) + line);
                Thread.Sleep(80);
            }

            Console.ResetColor();
            Thread.Sleep(1000);
            Console.Clear();
        }

        // --- HELPER METHOD (Must be outside ShowHydraSplashScreen) ---
        static void PrintSmallStatus(string msg, int left)
        {
            Console.SetCursorPosition(left, Console.CursorTop);
            Console.Write($"\u001b[32m>\u001b[0m {msg}");
            Thread.Sleep(400);
            Console.WriteLine();
        }
    }
}