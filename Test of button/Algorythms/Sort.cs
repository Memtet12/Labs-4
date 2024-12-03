using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Documents;
using Spectre.Console;
using static System.Collections.Specialized.BitVector32;

namespace Algorithms
{
    class Sort
    {
        public static void BubbleSort(int[] array, int delay, ref string log, ref int actionCounter)
        {
            int n = array.Length;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    string action = $"Сравнение {j + 1}-ого элемента и {j + 2}-ого => {array[j]} > {array[j + 1]}";
                    log += $"{actionCounter}. {action}\n";
                    actionCounter++;
                    Console.Clear();
                    AnsiConsole.Write(new BarChart()
                        .Width(60)
                        .Label($"[green bold underline]Текущий массив (Сравнение)[/]")
                        .CenterLabel()
                        .AddItems(array.Select((value, index) => new BarChartItem($"{value}", value, index == j || index == j + 1 ? Color.Red : Color.Blue))));
                    AnsiConsole.MarkupLine($"[yellow]{action}[/]");
                    Thread.Sleep(delay);

                    if (array[j] > array[j + 1])
                    {
                        action = $"Перестановка {j + 1}-ого и {j + 2}-ого элементов (Номер действия перестановки: {actionCounter})";
                        log += $"{actionCounter}. {action}\n";
                        actionCounter++;
                        int temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;

                        Console.Clear();
                        AnsiConsole.Write(new BarChart()
                            .Width(60)
                            .Label($"[green bold underline]Текущий массив (Перестановка)[/]")
                            .CenterLabel()
                            .AddItems(array.Select((value, index) => new BarChartItem($"{value}", value, index == j || index == j + 1 ? Color.Green : Color.Blue))));
                        AnsiConsole.MarkupLine($"[green]{action}[/]");
                        Thread.Sleep(delay);
                    }
                    else
                    {
                        action = $"Не требуется перестановка";
                        log += $"{actionCounter}. {action}\n";
                        actionCounter++;
                        Console.Clear();
                        AnsiConsole.Write(new BarChart()
                            .Width(60)
                            .Label($"[green bold underline]Текущий массив (Не требуется перестановка)[/]")
                            .CenterLabel()
                            .AddItems(array.Select((value, index) => new BarChartItem($"{value}", value, index == j || index == j + 1 ? Color.Red : Color.Blue))));
                        AnsiConsole.MarkupLine($"[red]{action}[/]");
                        Thread.Sleep(delay);
                    }
                }
            }
        }


       public static void MergeSort(int[] array, int left, int right, int delay, ref string log, ref int actionCounter)
{
    if (left < right)
    {
        int middle = (left + right) / 2;

        // Логирование начала разбиения массива
        string splitAction = $"Разбиение массива на подмассивы [{left}..{middle}] и [{middle + 1}..{right}]";
        log += $"{actionCounter}. {splitAction}\n";
        actionCounter++;
        Console.Clear();
        AnsiConsole.Write(new BarChart()
            .Width(60)
            .Label($"[green bold underline]Текущий массив (Разбиение)[/]")
            .CenterLabel()
            .AddItems(array.Select((value, index) => new BarChartItem($"{value}", value,
                index >= left && index <= right ?
                    (index <= middle ? Color.Blue : Color.Yellow) :
                    Color.Grey))));
        AnsiConsole.Markup($"[blue]{Markup.Escape(splitAction)}[/]\n");
        Thread.Sleep(delay*2);

        // Рекурсивный вызов для левого подмассива
        MergeSort(array, left, middle, delay, ref log, ref actionCounter);

        // Рекурсивный вызов для правого подмассива
        MergeSort(array, middle + 1, right, delay, ref log, ref actionCounter);

        // Слияние подмассивов
        Merge(array, left, middle, right, delay, ref log, ref actionCounter);
    }
}

        private static void Merge(int[] array, int left, int middle, int right, int delay, ref string log, ref int actionCounter)
        {
            int n1 = middle - left + 1;
            int n2 = right - middle;

            int[] leftArray = new int[n1];
            int[] rightArray = new int[n2];

            for (int i = 0; i < n1; i++)
            {
                leftArray[i] = array[left + i];
            }
            for (int j = 0; j < n2; j++)
            {
                rightArray[j] = array[middle + 1 + j];
            }

            int i1 = 0, i2 = 0;
            int k = left;

            // Начало слияния
            string mergeStartAction = $"Начало слияния подмассивов [{left}..{middle}] и [{middle + 1}..{right}]";
            log += $"{actionCounter}. {mergeStartAction}\n";
            actionCounter++;
            Console.Clear();
            AnsiConsole.Write(new BarChart()
                .Width(60)
                .Label($"[green bold underline]Текущий массив (Начало слияния)[/]")
                .CenterLabel()
                .AddItems(array.Select((value, index) => new BarChartItem($"{value}", value,
                    index >= left && index <= right ? Color.Green : Color.Grey))));
            AnsiConsole.Markup($"[green]{Markup.Escape(mergeStartAction)}[/]\n");
            Thread.Sleep(delay);

            while (i1 < n1 && i2 < n2)
            {
                string action = $"Сравнение {left + i1 + 1}-ого элемента ({leftArray[i1]}) и {middle + i2 + 2}-ого элемента ({rightArray[i2]}) => {leftArray[i1]} > {rightArray[i2]}";
                log += $"{actionCounter}. {action}\n";
                actionCounter++;
                Console.Clear();
                AnsiConsole.Write(new BarChart()
                    .Width(60)
                    .Label($"[green bold underline]Текущий массив (Сравнение)[/]")
                    .CenterLabel()
                    .AddItems(array.Select((value, index) => new BarChartItem($"{value}", value,
                        index >= left && index <= right ?
                            (index == k ? Color.Red : (index >= left && index <= middle ? Color.Blue : Color.Yellow)) :
                            Color.Grey))));
                AnsiConsole.Markup($"[yellow]{Markup.Escape(action)}[/]\n");
                Thread.Sleep(delay);

                if (leftArray[i1] <= rightArray[i2])
                {
                    array[k] = leftArray[i1];
                    i1++;
                    string reason = $"элемент {left + i1 - 1} ({leftArray[i1 - 1]}) меньше элемента {middle + i2} ({rightArray[i2]}), поэтому переставляем";
                    string swapAction = $"Перестановка {left + i1 - 1}-ого элемента в {k + 1}-ый => {array[k]} ({reason})";
                    log += $"{actionCounter}. {swapAction}\n";
                    actionCounter++;
                    Console.Clear();
                    AnsiConsole.Write(new BarChart()
                        .Width(60)
                        .Label($"[green bold underline]Текущий массив (Перестановка)[/]")
                        .CenterLabel()
                        .AddItems(array.Select((value, index) => new BarChartItem($"{value}", value,
                            index >= left && index <= right ?
                                (index == k ? Color.Green : (index >= left && index <= middle ? Color.Blue : Color.Yellow)) :
                                Color.Grey))));
                    AnsiConsole.Markup($"[green]{Markup.Escape(swapAction)}[/]\n");
                    Thread.Sleep(delay);
                }
                else
                {
                    array[k] = rightArray[i2];
                    i2++;
                    string reason = $"элемент {middle + i2 - 1} ({rightArray[i2 - 1]}) больше элемента {left + i1} ({leftArray[i1]}), поэтому переставляем";
                    string swapAction = $"Перестановка {middle + i2 - 1}-ого элемента в {k + 1}-ый => {array[k]} ({reason})";
                    log += $"{actionCounter}. {swapAction}\n";
                    actionCounter++;
                    Console.Clear();
                    AnsiConsole.Write(new BarChart()
                        .Width(60)
                        .Label($"[green bold underline]Текущий массив (Перестановка)[/]")
                        .CenterLabel()
                        .AddItems(array.Select((value, index) => new BarChartItem($"{value}", value,
                            index >= left && index <= right ?
                                (index == k ? Color.Green : (index >= left && index <= middle ? Color.Blue : Color.Yellow)) :
                                Color.Grey))));
                    AnsiConsole.Markup($"[green]{Markup.Escape(swapAction)}[/]\n");
                    Thread.Sleep(delay);
                }

                k++;
            }

            while (i1 < n1)
            {
                array[k] = leftArray[i1];
                i1++;
                k++;
                string action = $"Перестановка {left + i1 - 1}-ого элемента в {k}-ый => {array[k - 1]}";
                log += $"{actionCounter}. {action}\n";
                actionCounter++;
                Console.Clear();
                AnsiConsole.Write(new BarChart()
                    .Width(60)
                    .Label($"[green bold underline]Текущий массив (Перестановка)[/]")
                    .CenterLabel()
                    .AddItems(array.Select((value, index) => new BarChartItem($"{value}", value,
                        index >= left && index <= right ?
                            (index == k - 1 ? Color.Green : (index >= left && index <= middle ? Color.Blue : Color.Yellow)) :
                            Color.Grey))));
                AnsiConsole.Markup($"[green]{Markup.Escape(action)}[/]\n");
                Thread.Sleep(delay);
            }

            while (i2 < n2)
            {
                array[k] = rightArray[i2];
                i2++;
                k++;
                string action = $"Перестановка {middle + i2 - 1}-ого элемента в {k}-ый => {array[k - 1]}";
                log += $"{actionCounter}. {action}\n";
                actionCounter++;
                Console.Clear();
                AnsiConsole.Write(new BarChart()
                    .Width(60)
                    .Label($"[green bold underline]Текущий массив (Перестановка)[/]")
                    .CenterLabel()
                    .AddItems(array.Select((value, index) => new BarChartItem($"{value}", value,
                        index >= left && index <= right ?
                            (index == k - 1 ? Color.Green : (index >= left && index <= middle ? Color.Blue : Color.Yellow)) :
                            Color.Grey))));
                AnsiConsole.Markup($"[green]{Markup.Escape(action)}[/]\n");
                Thread.Sleep(delay);
            }

            // Конец слияния
            string mergeEndAction = $"Конец слияния подмассивов [{left}..{middle}] и [{middle + 1}..{right}]";
            log += $"{actionCounter}. {mergeEndAction}\n";
            actionCounter++;
            Console.Clear();
            AnsiConsole.Write(new BarChart()
                .Width(60)
                .Label($"[green bold underline]Текущий массив (Конец слияния)[/]")
                .CenterLabel()
                .AddItems(array.Select((value, index) => new BarChartItem($"{value}", value,
                    index >= left && index <= right ? Color.Green : Color.Grey))));
            AnsiConsole.Markup($"[green]{Markup.Escape(mergeEndAction)}[/]\n");
            Thread.Sleep(delay);
        }

        public static void QuickSort(int[] array, int left, int right, int delay, ref string log, ref int actionCounter)
        {
            if (left < right)
            {
                int pivotIndex = Partition(array, left, right, delay, ref log, ref actionCounter);
                QuickSort(array, left, pivotIndex - 1, delay, ref log, ref actionCounter);
                QuickSort(array, pivotIndex + 1, right, delay, ref log, ref actionCounter);
            }
        }

        private static int Partition(int[] array, int left, int right, int delay, ref string log, ref int actionCounter)
        {
            int pivot = array[right];
            int i = left - 1;

            for (int j = left; j < right; j++)
            {
                string action = $"Сравнение {j + 1}-ого элемента и опорного ({pivot}) => {array[j]} > {pivot}";
                log += $"{actionCounter}. {action}\n";
                actionCounter++;
                Console.Clear();
                AnsiConsole.Write(new BarChart()
                    .Width(60)
                    .Label($"[green bold underline]Текущий массив (Сравнение)[/]")
                    .CenterLabel()
                    .AddItems(array.Select((value, index) => new BarChartItem($"{value}", value, index == j ? Color.Red : index == right ? Color.Green : Color.Blue))));
                AnsiConsole.MarkupLine($"[yellow]{action}[/]");
                Thread.Sleep(delay);

                if (array[j] < pivot)
                {
                    i++;
                    action = $"Перестановка {i + 1}-ого и {j + 1}-ого элементов (Номер действия перестановки: {actionCounter})";
                    log += $"{actionCounter}. {action}\n";
                    actionCounter++;
                    int temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;

                    Console.Clear();
                    AnsiConsole.Write(new BarChart()
                        .Width(60)
                        .Label($"[green bold underline]Текущий массив (Перестановка)[/]")
                        .CenterLabel()
                        .AddItems(array.Select((value, index) => new BarChartItem($"{value}", value, index == i || index == j ? Color.Green : index == right ? Color.Green : Color.Blue))));
                    AnsiConsole.MarkupLine($"[green]{action}[/]");
                    Thread.Sleep(delay);
                }
                else
                {
                    action = $"Не требуется перестановка";
                    log += $"{actionCounter}. {action}\n";
                    actionCounter++;
                    Console.Clear();
                    AnsiConsole.Write(new BarChart()
                        .Width(60)
                        .Label($"[green bold underline]Текущий массив (Не требуется перестановка)[/]")
                        .CenterLabel()
                        .AddItems(array.Select((value, index) => new BarChartItem($"{value}", value, index == j ? Color.Red : index == right ? Color.Green : Color.Blue))));
                    AnsiConsole.MarkupLine($"[red]{action}[/]");
                    Thread.Sleep(delay);
                }
            }

            string action2 = $"Перестановка {i + 2}-ого и {right + 1}-ого элементов (Номер действия перестановки: {actionCounter})";
            log += $"{actionCounter}. {action2}\n";
            actionCounter++;
            int temp2 = array[i + 1];
            array[i + 1] = array[right];
            array[right] = temp2;

            Console.Clear();
            AnsiConsole.Write(new BarChart()
                .Width(60)
                .Label($"[green bold underline]Текущий массив (Перестановка)[/]")
                .CenterLabel()
                .AddItems(array.Select((value, index) => new BarChartItem($"{value}", value, index == i + 1 || index == right ? Color.Green : Color.Blue))));
            AnsiConsole.MarkupLine($"[green]{action2}[/]");
            Thread.Sleep(delay);

            return i + 1;
        }

        public static void InsertionSort(int[] array, int delay, ref string log, ref int actionCounter)
        {
            int n = array.Length;
            for (int i = 1; i < n; i++)
            {
                int key = array[i];
                int j = i - 1;

                string action = $"Сравнение {j + 1}-ого элемента и {i + 1}-ого => {array[j]} > {key}";
                log += $"{actionCounter}. {action}\n";
                actionCounter++;
                Console.Clear();
                AnsiConsole.Write(new BarChart()
                    .Width(60)
                    .Label($"[green bold underline]Текущий массив (Сравнение)[/]")
                    .CenterLabel()
                    .AddItems(array.Select((value, index) => new BarChartItem($"{value}", value, index == j || index == i ? Color.Red : Color.Blue))));
                AnsiConsole.MarkupLine($"[yellow]{action}[/]");
                Thread.Sleep(delay);

                while (j >= 0 && array[j] > key)
                {
                    action = $"Перестановка {j + 1}-ого и {j + 2}-ого элементов (Номер действия перестановки: {actionCounter})";
                    log += $"{actionCounter}. {action}\n";
                    actionCounter++;
                    array[j + 1] = array[j];
                    j--;

                    Console.Clear();
                    AnsiConsole.Write(new BarChart()
                        .Width(60)
                        .Label($"[green bold underline]Текущий массив (Перестановка)[/]")
                        .CenterLabel()
                        .AddItems(array.Select((value, index) => new BarChartItem($"{value}", value, index == j + 1 || index == j + 2 ? Color.Green : Color.Blue))));
                    AnsiConsole.MarkupLine($"[green]{action}[/]");
                    Thread.Sleep(delay);
                }

                array[j + 1] = key;

                action = $"Вставка {key} на позицию {j + 2}";
                log += $"{actionCounter}. {action}\n";
                actionCounter++;
                Console.Clear();
                AnsiConsole.Write(new BarChart()
                    .Width(60)
                    .Label($"[green bold underline]Текущий массив (Вставка)[/]")
                    .CenterLabel()
                    .AddItems(array.Select((value, index) => new BarChartItem($"{value}", value, index == j + 1 ? Color.Green : Color.Blue))));
                AnsiConsole.MarkupLine($"[green]{action}[/]");
                Thread.Sleep(delay);
            }
        }

        public static void PrintArrayInBarChart(int[] array)
        {
            AnsiConsole.Write(new BarChart()
                .Width(60)
                .Label("[green bold underline]Массив[/]")
                .CenterLabel()
                .AddItems(array.Select((value, index) => new BarChartItem($"{value}", value, Color.Blue))));
        }
    }
}