using System;
using System.Linq;
using Spectre.Console;
using Test_of_button;

namespace Algorithms
{
    class MenuTask1
    {
        private static string log = "";
        private static int actionCounter = 1;

        public static void Run()
        {
            Console.Clear();
            DisplayHeader();
            string choice = DisplaySortingAlgorithmMenu();
            int[] array = GetInputArray();
            int delayInMilliseconds = GetDelay();

            DisplayOriginalArray(array);
            SortArray(choice, array, delayInMilliseconds);
            DisplaySortedArray(array);

            WaitForUserInput();
            DisplayLogIfRequested();

            // Добавляем кнопку "меню" для перезапуска программы
            AnsiConsole.MarkupLine("[blue]Нажмите любую клавишу для возврата в меню...[/]");
            Console.ReadKey();
            log = "";
            MainMenu.DisplayMainMenu(); // Выход в главное меню
        }

        private static void DisplayHeader()
        {
            Console.Clear();
            AnsiConsole.Write(
                new FigletText("Sorting Algorithms")
                    .Centered()
                    .Color(Color.Green));
        }

        private static string DisplaySortingAlgorithmMenu()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Выберите алгоритм сортировки:")
                    .AddChoices(new[] { "Сортировка пузырьком", "Сортировка простыми вставками", "Быстрая сортировка", "Сортировка слиянием" }));
        }

        private static int[] GetInputArray()
        {
            int[] array = null;
            bool isValidInput = false;

            while (!isValidInput)
            {
                var input = AnsiConsole.Ask<string>("Введите элементы массива через пробел:");
                try
                {
                    array = input.Split(' ').Select(int.Parse).ToArray();
                    isValidInput = true;
                }
                catch (FormatException)
                {
                    AnsiConsole.MarkupLine("[red]Неверный формат ввода. Попробуйте еще раз.[/]");
                }
            }

            return array;
        }

        private static int GetDelay()
        {
            var delay = AnsiConsole.Ask<int>("Введите задержку в миллисекундах:");
            return delay;
        }

        private static void DisplayOriginalArray(int[] array)
        {
            AnsiConsole.MarkupLine("[green]Исходный массив:[/]");
            Sort.PrintArrayInBarChart(array);
        }

        private static void SortArray(string choice, int[] array, int delayInMilliseconds)
        {
            if (choice == "Сортировка пузырьком")
            {
                Sort.BubbleSort(array, delayInMilliseconds, ref log, ref actionCounter);
            }
            else if (choice == "Сортировка слиянием")
            {
                Sort.MergeSort(array, 0, array.Length - 1, delayInMilliseconds, ref log, ref actionCounter);
            }
            else if (choice == "Быстрая сортировка")
            {
                Sort.QuickSort(array, 0, array.Length - 1, delayInMilliseconds, ref log, ref actionCounter);
            }
            else if (choice == "Сортировка простыми вставками")
            {
                Sort.InsertionSort(array, delayInMilliseconds, ref log, ref actionCounter);
            }
        }

        private static void DisplaySortedArray(int[] array)
        {
            AnsiConsole.MarkupLine("[green]Отсортированный массив:[/]");
            Sort.PrintArrayInBarChart(array);
        }

        private static void WaitForUserInput()
        {
            AnsiConsole.MarkupLine("[blue]Нажмите любую клавишу для продолжения...[/]");
            Console.ReadKey();
        }

        private static void DisplayLogIfRequested()
        {
            var logChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Вы хотите вывести лог сравнений и перестановок?")
                    .AddChoices(new[] { "Да", "Нет" }));

            if (logChoice == "Да")
            {
                AnsiConsole.MarkupLine("[green]Лог сравнений и перестановок:[/]");
                AnsiConsole.WriteLine(log);
            }
        }
    }
}