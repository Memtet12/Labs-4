using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;

namespace Test_of_button._3rd_task
{
    public static class Visualizer
    {
        public static void Run()
        {
            DisplayHeader();
            string regime = DisplayMainSelection();
            MainSelectionHandler(regime);

        }

        public static string DisplayMainSelection()
        { 
          return AnsiConsole.Prompt(
                new SelectionPrompt <string>()
                .Title("Выберите функцию:")
                .AddChoices(new[]
                {"Подсчёт слов в тексте",
                "Сравнение производительности BubbleSort и RadixSort"}
                ).HighlightStyle(Color.Green3)
                );
              
        }

        public static string DisplayFileSelection()
        {
            string[] files = Misc.GetTexts("..\\..\\Texts");

            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("Выберите файл (Все файлы хранятся в папке Texts):")
                .AddChoices( files
                ).HighlightStyle(Color.Green3)
                );
        }

        public static void DisplayHeader()
        {
            Console.Clear();
            AnsiConsole.Write(
                new FigletText("Text Sortings")
                    .Centered()
                    .Color(Color.CornflowerBlue));
        }

        public static void MainSelectionHandler(string regime)
        {
            if (regime == "Подсчёт слов в тексте")
            {
                string file =  DisplayFileSelection();
                DrawWordsCount(file);
            }
            else
            {
                DrawTimeTable();
            }
        }

        public static void DrawWordsCount(string file)
        {
            var data = Misc.CountWords(file);

            var table = new Table();
            table.AddColumn("Слово");
            table.AddColumn("Количество упоминаний");

            int i = 0;
            foreach (var word in data)
            {
                table.AddRow(new[] {word.Item1, word.Item2.ToString()});
            }
            AnsiConsole.Write(table);
        }

        private static void DrawTimeTable()
        {
            string[] files = Misc.GetTexts("..\\..\\Texts\\TestData");
            var data = Misc.TestTime(files);

            var table = new Table();

            table.AddColumn("");
            table.AddColumn("Время BubbleSort, мс");
            table.AddColumn("Вреия RadixSort, мс");

            foreach (var element in data)
            {
                table.AddRow(new[] {element.Item1.ToString() + " слов", element.Item2.ToString(), element.Item3.ToString()});
            }
            AnsiConsole.Write(table);
        }

    }
}
