// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

string input = "[14:49:52] INFO - | 0|  1060| 16.30 M|    |     20|     15|  0|  85|  72|    | 55|1835| 3802| 100| 191.8 K|";

string regex = "INFO\\s+\\-\\s+\\|([^|]+\\|){5}\\s*(?<rejectShare>\\d+)";
Regex rx = new Regex(regex, RegexOptions.Compiled | RegexOptions.IgnoreCase);




MatchCollection matches = rx.Matches(input);

Console.WriteLine("{0} matches found in:\n   {1}",
              matches.Count,
              input);

foreach (Match match in matches)
{
    GroupCollection groups = match.Groups;
    Console.WriteLine("'{0}' repeated at positions {1} and {2}",
                      groups["rejectShare"].Value,
                      groups[0].Index,
                      groups[1].Index);
}