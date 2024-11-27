using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TextSortings
{
    public static class Misc
    {
        public static string[] GetTexts(string textsPath)
        {
            //string textsPath = "..\\..\\Texts";

            if (!Directory.Exists(textsPath))
                Directory.CreateDirectory(textsPath);

            return Directory.GetFiles(textsPath);
        }

        private static string[] ReadFile(string file)
        {
            try
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    List<string> wordsList = new List<string>();
                    StringBuilder buffer = new StringBuilder();

                    while (sr.Peek() != -1)
                    {
                        string input = sr.ReadLine().ToLower();
                        Regex reg = new Regex("\\b[\\w-']+\\b"); //Создаёт регулярное выражение : \\начало слова \\любые символы слов или - ' \\конец слова
                        var matches = reg.Matches(input);
                        foreach (Match match in matches)
                        {
                            wordsList.Add(match.Value.ToLower());
                        }
                    }
                    return wordsList.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            throw new Exception("Method does not return result");
        }

        public static List<(string, int)> CountWords(string file)
        {
            string[] words = ReadFile(file);
            TextSorting.RadixSort(words);
            var result = new List<(string, int)>();
            string current = words[0];
            int count = 0;
            foreach (string word in words)
            {
                if (word == current)
                    count++;
                else
                {
                    result.Add((current, count));

                    current = word;
                    count = 1;
                }
            }
            return result;
        }

        public static List<(int, double, double)> TestTime(string[] files)
        {
            var result = new List<(int, double, double)>();
            foreach (var file in files)
            {
                string[] words = ReadFile(file);
                Stopwatch sw1 = Stopwatch.StartNew();
                TextSorting.BubbleSort(words);
                sw1.Stop();
                Stopwatch sw2 = Stopwatch.StartNew();
                TextSorting.RadixSort(words);
                sw2.Stop();


                long frequancy = Stopwatch.Frequency / 1000;
                double time1 = sw1.ElapsedTicks / frequancy;
                double time2 = sw2.ElapsedTicks / frequancy;

                result.Add((words.Length, time1, time2));
            }
            return result;
        }


    }
}
