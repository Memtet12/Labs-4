using System;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.WriteLine("Введите элементы массива через пробел:");
        string[] input = Console.ReadLine().Split(' ');
        int[] array = Array.ConvertAll(input, int.Parse);

        Console.WriteLine("Введите задержку в секундах:");
        int delayInSeconds = int.Parse(Console.ReadLine());
        int delayInMilliseconds = delayInSeconds * 1000; // Преобразуем секунды в миллисекунды

        Console.WriteLine("Исходный массив:");
        PrintArray(array);

        Console.WriteLine("Выберите алгоритм сортировки:");
        Console.WriteLine("1. Сортировка пузырьком (нажмите 1)");
        Console.WriteLine("2. Сортировка слиянием (нажмите 2)");
        Console.WriteLine("3. Быстрая сортировка (нажмите 3)");

        ConsoleKeyInfo keyInfo = Console.ReadKey();
        int choice = int.Parse(keyInfo.KeyChar.ToString());

        if (choice == 1)
        {
            BubbleSort(array, delayInMilliseconds);
        }
        else if (choice == 2)
        {
            MergeSort(array, 0, array.Length - 1, delayInMilliseconds);
        }
        else if (choice == 3)
        {
            QuickSort(array, 0, array.Length - 1, delayInMilliseconds);
        }
        else
        {
            Console.WriteLine("Неверный выбор алгоритма.");
            return;
        }

        Console.WriteLine("Отсортированный массив:");
        PrintArray(array);

        Console.WriteLine("Лог сравнений и перестановок:");
        Console.WriteLine(log);
    }

    static string log = "";

    static void BubbleSort(int[] array, int delay)
    {
        int n = array.Length;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                log += $"Сравнение: {array[j]} и {array[j + 1]}\n";
                Console.Clear();
                Console.WriteLine("Текущий массив:");
                PrintArray(array, j, j + 1, "Сравнение");
                Thread.Sleep(delay);

                if (array[j] > array[j + 1])
                {
                    log += $"Перестановка: {array[j]} и {array[j + 1]}\n";
                    int temp = array[j];
                    array[j] = array[j + 1];
                    array[j + 1] = temp;

                    Console.Clear();
                    Console.WriteLine("Текущий массив:");
                    PrintArray(array, j, j + 1, "Перестановка");
                    Thread.Sleep(delay);
                }
            }
        }
    }

    static void MergeSort(int[] array, int left, int right, int delay)
    {
        if (left < right)
        {
            int middle = (left + right) / 2;

            MergeSort(array, left, middle, delay);
            MergeSort(array, middle + 1, right, delay);

            Merge(array, left, middle, right, delay);
        }
    }

    static void Merge(int[] array, int left, int middle, int right, int delay)
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

        int k = left;
        int i1 = 0, i2 = 0;

        while (i1 < n1 && i2 < n2)
        {
            log += $"Сравнение: {leftArray[i1]} и {rightArray[i2]}\n";
            Console.Clear();
            Console.WriteLine("Текущий массив:");
            PrintArrayWithDivider(array, left, middle, right, i1, i2, "Сравнение");
            Thread.Sleep(delay);

            if (leftArray[i1] <= rightArray[i2])
            {
                array[k] = leftArray[i1];
                i1++;
            }
            else
            {
                array[k] = rightArray[i2];
                i2++;
            }
            k++;
        }

        while (i1 < n1)
        {
            array[k] = leftArray[i1];
            i1++;
            k++;
        }

        while (i2 < n2)
        {
            array[k] = rightArray[i2];
            i2++;
            k++;
        }

        Console.Clear();
        Console.WriteLine("Текущий массив:");
        PrintArrayWithDivider(array, left, middle, right, -1, -1, "Слияние");
        Thread.Sleep(delay);
    }

    static void QuickSort(int[] array, int left, int right, int delay)
    {
        if (left < right)
        {
            int pivotIndex = Partition(array, left, right, delay);
            QuickSort(array, left, pivotIndex - 1, delay);
            QuickSort(array, pivotIndex + 1, right, delay);
        }
    }

    static int Partition(int[] array, int left, int right, int delay)
    {
        int pivot = array[right];
        int i = left - 1;

        for (int j = left; j < right; j++)
        {
            log += $"Сравнение: {array[j]} и {pivot}\n";
            Console.Clear();
            Console.WriteLine("Текущий массив:");
            PrintArrayWithPivot(array, left, right, j, pivot, "Сравнение");
            Thread.Sleep(delay);

            if (array[j] < pivot)
            {
                i++;
                log += $"Перестановка: {array[i]} и {array[j]}\n";
                int temp = array[i];
                array[i] = array[j];
                array[j] = temp;

                Console.Clear();
                Console.WriteLine("Текущий массив:");
                PrintArrayWithPivot(array, left, right, j, pivot, "Перестановка");
                Thread.Sleep(delay);
            }
        }

        log += $"Перестановка: {array[i + 1]} и {array[right]}\n";
        int temp2 = array[i + 1];
        array[i + 1] = array[right];
        array[right] = temp2;

        Console.Clear();
        Console.WriteLine("Текущий массив:");
        PrintArrayWithPivot(array, left, right, right, pivot, "Перестановка");
        Thread.Sleep(delay);

        return i + 1;
    }

    static void PrintArray(int[] array, int index1 = -1, int index2 = -1, string action = "")
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (i == index1 || i == index2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.Write(array[i] + " ");
        }
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine();
        Console.WriteLine($"Действие: {action}");
    }

    static void PrintArrayWithDivider(int[] array, int left, int middle, int right, int index1 = -1, int index2 = -1, string action = "")
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (i == index1 + left || i == index2 + middle + 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (i == middle + 1)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("| ");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.Write(array[i] + " ");
        }
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine();
        Console.WriteLine($"Действие: {action}");
    }

    static void PrintArrayWithPivot(int[] array, int left, int right, int index1 = -1, int pivot = -1, string action = "")
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (i == index1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            else if (i == right)
            {
                Console.ForegroundColor = ConsoleColor.Green;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.Write(array[i] + " ");
        }
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine();
        Console.WriteLine($"Действие: {action}");
        Console.WriteLine($"Опорный элемент: {pivot}");
    }
}