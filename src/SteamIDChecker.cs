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

using System.Xml.Linq;
partial class SteamIDChecker
{
    static async Task Main()
    {
        bool checkThreeLetterCombinations = false;
        var combinations = ShitOutCombinations(checkThreeLetterCombinations);
        var invalidIDs = new List<string>();

        using HttpClient client = new();
        foreach (var id in combinations)
        {
            var url = $"https://steamcommunity.com/id/{id}/?xml=1";
            var response = await client.GetStringAsync(url);

            if (IsNotValid(response))
            {
                invalidIDs.Add(id);
                Console.WriteLine(id);
            }
        }
    }

    static IEnumerable<string> ShitOutCombinations(bool checkThreeLetterCombinations)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        foreach (char first in chars)
        {
            foreach (char second in chars)
            {
                yield return $"{first}{second}";

                if (checkThreeLetterCombinations)
                {
                    foreach (char third in chars)
                    {
                        yield return $"{first}{second}{third}";
                    }
                }
            }
        }
    }

    static bool IsNotValid(string response)
    {
        var xml = XDocument.Parse(response);
        var errorElement = xml.Root!.Element("error");
        if (errorElement != null && errorElement.Value.Contains("The specified profile could not be found"))
        {
            return true;
        }
        return false;
    }
}
