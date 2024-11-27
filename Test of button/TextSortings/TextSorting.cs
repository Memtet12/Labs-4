using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Spectre.Console;

namespace TextSortings

{
    public static class TextSorting
    {

        public static void BubbleSort(string[] words)
        {
            int size = words.Length;
            for (int i = 1; i < size; i++)
            {
                bool swaped = false;
                for (int j = 0; j < size - 1; j++)
                {
                    if (string.Compare(words[j], words[j + 1]) > 0)
                    {
                        string temp = words[j];
                        words[j] = words[j + 1];
                        words[j + 1] = temp;
                        swaped = true;
                    }
                }
                if (!swaped)
                    return;
            }
        }

        public static void RadixSort(string[] array)
        {
            int maxLength = GetMaxLength(array);

            for (int digitIndex = maxLength - 1; digitIndex >= 0; digitIndex--)
            {
                CountingSort(array, digitIndex);
            }
        }

        private static void CountingSort(string[] array, int digitIndex)
        {
            const int alphabetSize = char.MaxValue; // Размер алфавита (UTF-16)
            int n = array.Length;
            string[] output = new string[n];
            int[] count = new int[alphabetSize];

            // Подсчет количества вхождений каждого символа
            for (int i = 0; i < n; i++)
            {
                int charIndex = digitIndex < array[i].Length ? array[i][digitIndex] : 0;
                count[charIndex]++;
            }

            // Преобразуем count в индексы
            for (int i = 1; i < alphabetSize; i++)
            {
                count[i] += count[i - 1];
            }

            // Построение выходного массива
            for (int i = n - 1; i >= 0; i--)
            {
                int charIndex = digitIndex < array[i].Length ? array[i][digitIndex] : 0;
                output[count[charIndex] - 1] = array[i];
                count[charIndex]--;
            }

            // Копируем выходной массив в исходный
            for (int i = 0; i < n; i++)
            {
                array[i] = output[i];
            }
        }

        private static int GetMaxLength(string[] array)
        {
            int maxLength = 0;
            foreach (string word in array)
            {
                if (word.Length > maxLength)
                {
                    maxLength = word.Length;
                }
            }
            return maxLength;
        }


    }
}
