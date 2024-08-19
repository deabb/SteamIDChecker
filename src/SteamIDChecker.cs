/*
Copyright (C) 2024 Dea Brcka

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System.Net.Http;
using System.Xml.Linq;

partial class SteamIDChecker
{
    static async Task Main(string[] args)
    {
        bool checkThreeLetterCombinations = args.Length > 0 && args[0] == "-3";

        var combinations = ShitOutCombinations(checkThreeLetterCombinations);

        Console.WriteLine($"Checking {(checkThreeLetterCombinations ? "3 letter" : "2 letter")} Steam IDs...");
        Console.WriteLine("Please wait...");

        using HttpClient client = new();
        foreach (var id in combinations)
        {
            string url = $"https://steamcommunity.com/id/{id}/?xml=1";
            string response = await client.GetStringAsync(url);

            string status = IsNotValid(response) ? "\u001b[32mFree\u001b[0m" : "\u001b[91mTaken\u001b[0m";
            Console.WriteLine($"{id} - {status}");
        }
    }

    static IEnumerable<string> ShitOutCombinations(bool checkThreeLetterCombinations)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        foreach (char first in chars)
        {
            foreach (char second in chars)
            {
                if (checkThreeLetterCombinations)
                {
                    foreach (char third in chars)
                    {
                        yield return $"{first}{second}{third}";
                    }
                }
                else
                {
                    yield return $"{first}{second}";
                }
            }
        }
    }

    static bool IsNotValid(string response)
    {
        var xml = XDocument.Parse(response);
        return xml.Root?.Element("error")?.Value.Contains("The specified profile could not be found") ?? false;
    }
}
