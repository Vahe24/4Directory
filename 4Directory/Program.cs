using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

class Program
{
    static async Task Main()
    {
        Console.WriteLine("=== 4Directory ===");

        Console.Write("Enter the base URL : ");
        string baseUrl = Console.ReadLine().Trim();

        Console.Write("Enter full path to your wordlist file (e.g. C:\\Users\\hp\\Documents\\doc.txt): ");
        string wordlistPath = Console.ReadLine().Trim();

        if (!File.Exists(wordlistPath))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"File not found: {wordlistPath}");
            Console.ResetColor();
            return;
        }

        List<string> wordlist = new List<string>(File.ReadAllLines(wordlistPath));

        List<string> extensions = new List<string>
        {
            "",         
            "/",        
            ".html",
            ".php",
            ".js",
            ".asp",
            ".aspx",
            "/index.html",
            "/index.php"
        };

        using (HttpClient client = new HttpClient())
        {
            foreach (var word in wordlist)
            {
                string path = word.Trim();
                if (string.IsNullOrWhiteSpace(path)) continue;

                foreach (var ext in extensions)
                {
                    string fullUrl = baseUrl.TrimEnd('/') + "/" + path + ext;

                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(fullUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"[+] Found: {fullUrl}");
                        }
                        else if ((int)response.StatusCode == 403)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"[-] Forbidden (403): {fullUrl}");
                        }
                        else if ((int)response.StatusCode == 401)
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"[!] Requires authentication (401): {fullUrl}");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine($"[ ] Not found ({(int)response.StatusCode}): {fullUrl}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[Error] {fullUrl} - {ex.Message}");
                    }
                }
            }
        }

        Console.ResetColor();
        Console.WriteLine("Scan completed.");
    }
}
