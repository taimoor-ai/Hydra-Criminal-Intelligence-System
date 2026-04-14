// ============================================================
// Utilities/Helpers.cs
// Password hashing, ID generation, display utilities
// ============================================================
using System.Security.Cryptography;
using System.Text;

namespace CriminalRecordMS.Utilities
{
    public static class PasswordHelper
    {
        public static string Hash(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password + "CRMS_SALT_2024"));
            return Convert.ToHexString(bytes);
        }

        public static bool Verify(string password, string hash)
            => Hash(password) == hash;
    }

    public static class IdGenerator
    {
        private static int _criminalCounter = 100;
        private static int _officerCounter = 200;
        private static int _caseCounter = 300;
        private static int _evidenceCounter = 400;
        private static int _victimCounter = 500;
        private static int _crimeCounter = 600;

        public static string NewCriminalId() => $"CRM-{++_criminalCounter}";
        public static string NewOfficerId() => $"OFC-{++_officerCounter}";
        public static string NewCaseId() => $"CSE-{++_caseCounter}";
        public static string NewEvidenceId() => $"EVD-{++_evidenceCounter}";
        public static string NewVictimId() => $"VCT-{++_victimCounter}";
        public static string NewCrimeId() => $"CRC-{++_crimeCounter}";

        public static void SetCounters(int cr, int of, int cs, int ev, int vi, int crc)
        {
            _criminalCounter = cr;
            _officerCounter = of;
            _caseCounter = cs;
            _evidenceCounter = ev;
            _victimCounter = vi;
            _crimeCounter = crc;
        }
    }

    public static class ConsoleUI
    {
        public static void PrintHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine();
            Console.WriteLine("  ╔" + new string('═', 50) + "╗");
            Console.WriteLine($"  ║  {title.PadRight(48)}║");
            Console.WriteLine("  ╚" + new string('═', 50) + "╝");
            Console.ResetColor();
        }

        public static void PrintSuccess(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ✔  {msg}");
            Console.ResetColor();
        }

        public static void PrintError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ✘  ERROR: {msg}");
            Console.ResetColor();
        }

        public static void PrintWarning(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  ⚠  {msg}");
            Console.ResetColor();
        }

        public static void PrintInfo(string msg)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"  ℹ  {msg}");
            Console.ResetColor();
        }

        public static void PrintDivider()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("  " + new string('─', 52));
            Console.ResetColor();
        }

        public static string PromptInput(string label)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"  » {label}: ");
            Console.ResetColor();
            return Console.ReadLine()?.Trim() ?? "";
        }

        public static string PromptPassword(string label = "Password")
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"  » {label}: ");
            Console.ResetColor();

            var pass = new StringBuilder();
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
                {
                    pass.Remove(pass.Length - 1, 1);
                    Console.Write("\b \b");
                }
                else if (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.Backspace)
                {
                    pass.Append(key.KeyChar);
                    Console.Write("*");
                }
            } while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            return pass.ToString();
        }

        public static void PressAnyKey()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n  Press any key to continue...");
            Console.ResetColor();
            Console.ReadKey(true);
        }

        public static bool Confirm(string question)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"  » {question} (y/n): ");
            Console.ResetColor();
            var answer = Console.ReadLine()?.Trim().ToLower();
            return answer == "y" || answer == "yes";
        }

        public static void PrintMenuItem(int number, string label)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"  [{number}] ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(label);
            Console.ResetColor();
        }
    }

    public static class Validator
    {
        public static bool IsValidCnic(string cnic)
            => !string.IsNullOrWhiteSpace(cnic) && cnic.Length >= 13;

        public static bool IsValidPhone(string phone)
            => !string.IsNullOrWhiteSpace(phone) && phone.Length >= 10;

        public static bool TryParseDate(string input, out DateTime date)
            => DateTime.TryParseExact(input, "yyyy-MM-dd",
               System.Globalization.CultureInfo.InvariantCulture,
               System.Globalization.DateTimeStyles.None, out date);

        public static bool TryParseEnum<T>(string input, out T result) where T : struct
            => Enum.TryParse(input, true, out result);
    }
}