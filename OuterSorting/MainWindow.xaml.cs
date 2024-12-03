using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OuterSorting
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> logs = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                FilePath.Text = dialog.FileName;
                AddLog($"Выбран файл: {dialog.FileName}");
            }
        }

        private async void StartSort_Click(object sender, RoutedEventArgs e)
        {
            Logs.Text = "Начинается проверка входных данных...\n";

            // Проверка на наличие пути к файлу
            if (string.IsNullOrWhiteSpace(FilePath.Text))
            {
                AddLog("Ошибка: путь к файлу не указан.");
                MessageBox.Show("Укажите файл для сортировки!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка на существование файла
            if (!File.Exists(FilePath.Text))
            {
                AddLog($"Ошибка: файл '{FilePath.Text}' не найден.");
                MessageBox.Show("Указанный файл не существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка на наличие ключа сортировки
            if (string.IsNullOrWhiteSpace(SortColumn.Text))
            {
                AddLog("Ошибка: ключ сортировки не указан.");
                MessageBox.Show("Укажите ключ сортировки!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка формата ключа сортировки
            if (!int.TryParse(SortColumn.Text, out int sortColumn) || sortColumn < 1)
            {
                AddLog("Ошибка: ключ сортировки должен быть положительным числом.");
                MessageBox.Show("Ключ сортировки должен быть положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // Проверка на наличие задержки 
            if (string.IsNullOrWhiteSpace(Delay.Text))
            {
                AddLog("Ошибка: задержки не указан.");
                MessageBox.Show("Укажите задержки!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка формата задержки
            if (!int.TryParse(Delay.Text, out int delay) || delay < 1)
            {
                AddLog("Ошибка: задержка должена быть положительным числом.");
                MessageBox.Show("Задержка должена быть положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string inputFile = FilePath.Text;

            // Проверка на пустоту файла
            if (new FileInfo(inputFile).Length == 0)
            {
                AddLog("Ошибка: файл пуст. Невозможно выполнить сортировку.");
                MessageBox.Show("Файл пуст. Сортировка невозможна!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка корректности индекса столбца
            bool isIndexValid = await Task.Run(() => ValidateIndex(inputFile, sortColumn - 1));
            if (!isIndexValid)
            {
                AddLog($"Ошибка: индекс столбца {sortColumn} выходит за пределы строк в файле.");
                MessageBox.Show("Индекс столбца выходит за пределы строк в файле. Проверьте ключ сортировки!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            AddLog("Все проверки пройдены успешно. Начинается сортировка...");

            try
            {
                await Task.Run(() => DirectMergeSort(inputFile, sortColumn - 1));
                AddLog("Сортировка завершена успешно. Исходный файл обновлён.");
            }
            catch (Exception ex)
            {
                AddLog($"Ошибка во время сортировки: {ex.Message}");
            }
        }

        private bool ValidateIndex(string filePath, int sortColumn)
        {
            AddLog("Проверка валидности индекса столбца...");
            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var fields = line.Split(',');
                    if (sortColumn >= fields.Length)
                    {
                        AddLog($"Ошибка: индекс {sortColumn + 1} превышает количество столбцов в строке: \"{line}\".");
                        return false;
                    }
                }
            }
            AddLog("Индекс столбца валиден.");
            return true;
        }



        private void DirectMergeSort(string inputFile, int sortColumn)
        {
            int delayMs = GetDelayFromUI();
            AddLog("Запуск прямого слияния...");
            int blockSize = 1; // Начальный размер блока для слияния

            while (true)
            {
                AddLog($"Текущий размер блока для слияния: {blockSize}");
                string fileB = System.IO.Path.GetTempFileName();
                string fileC = System.IO.Path.GetTempFileName();

                // Шаг 1: Разделение на два файла (B и C)
                using (var reader = new StreamReader(inputFile))
                using (var writerB = new StreamWriter(fileB))
                using (var writerC = new StreamWriter(fileC))
                {
                    bool toB = true;
                    while (!reader.EndOfStream)
                    {
                        for (int i = 0; i < blockSize && !reader.EndOfStream; i++)
                        {
                            var line = reader.ReadLine();
                            if (toB)
                            {
                                writerB.WriteLine(line);
                                AddLog($"Записано в файл B: {line}");
                            }
                            else
                            {
                                writerC.WriteLine(line);
                                AddLog($"Записано в файл C: {line}");
                            }
                            Task.Delay(delayMs).Wait(); // Задержка
                        }
                        toB = !toB;
                    }
                }
                AddLog("Разделение завершено.");

                // Шаг 2: Слияние блоков обратно в исходный файл
                using (var readerB = new StreamReader(fileB))
                using (var readerC = new StreamReader(fileC))
                using (var writer = new StreamWriter(inputFile))
                {
                    string[] bufferB = new string[blockSize];
                    string[] bufferC = new string[blockSize];
                    int countB, countC;

                    while (!readerB.EndOfStream || !readerC.EndOfStream)
                    {
                        countB = FillBuffer(readerB, bufferB);
                        countC = FillBuffer(readerC, bufferC);

                        int i = 0, j = 0;
                        while (i < countB && j < countC)
                        {
                            if (CompareLines(bufferB[i], bufferC[j], sortColumn) <= 0)
                            {
                                writer.WriteLine(bufferB[i]);
                                AddLog($"Из файла B записано: {bufferB[i]}");
                                i++;
                            }
                            else
                            {
                                writer.WriteLine(bufferC[j]);
                                AddLog($"Из файла C записано: {bufferC[j]}");
                                j++;
                            }
                            Task.Delay(delayMs).Wait(); // Задержка
                        }

                        while (i < countB)
                        {
                            writer.WriteLine(bufferB[i]);
                            AddLog($"Из файла B записано: {bufferB[i]}");
                            i++;
                            Task.Delay(delayMs).Wait(); // Задержка
                        }

                        while (j < countC)
                        {
                            writer.WriteLine(bufferC[j]);
                            AddLog($"Из файла C записано: {bufferC[j]}");
                            j++;
                            Task.Delay(delayMs).Wait(); // Задержка
                        }
                    }
                }

                File.Delete(fileB);
                File.Delete(fileC);

                // Проверка завершения сортировки
                if (IsSorted(inputFile, sortColumn))
                {
                    AddLog("Файл полностью отсортирован.");
                    break;
                }

                AddLog("Файл ещё не отсортирован. Увеличиваем размер блока и переходим к следующей итерации.");
                blockSize *= 2;
            }

            AddLog("Прямое слияние завершено. Исходный файл обновлён.");
        }

        // Вспомогательный метод для заполнения буфера
        private int FillBuffer(StreamReader reader, string[] buffer)
        {
            int delayMs = GetDelayFromUI();
            int count = 0;
            for (int i = 0; i < buffer.Length && !reader.EndOfStream; i++)
            {
                buffer[i] = reader.ReadLine();
                count++;
                Task.Delay(delayMs).Wait(); // Задержка
            }
            return count;
        }

        private bool IsSorted(string filePath, int sortColumn)
        {
            int delayMs = GetDelayFromUI();
            using (var reader = new StreamReader(filePath))
            {
                string prevLine = reader.ReadLine();
                string currLine;

                while ((currLine = reader.ReadLine()) != null)
                {
                    if (CompareLines(prevLine, currLine, sortColumn) > 0)
                    {
                        AddLog($"Алгоритм продолжается, так как нашлись строки в неверном порядке: \"{prevLine}\" > \"{currLine}\".");
                        return false;
                    }
                    prevLine = currLine;
                    Task.Delay(delayMs).Wait(); // Задержка
                }
            }
            return true;
        }


        private int GetDelayFromUI()
        {
            int delayMs = 0;

            // Используем Dispatcher.Invoke, чтобы безопасно получить доступ к элементу UI из другого потока
            Dispatcher.Invoke(() =>
            {
                // Пытаемся получить значение задержки из текстового поля
                if (int.TryParse(Delay.Text, out delayMs) && delayMs >= 0)
                {
                    // Если значение корректное, присваиваем его
                }
                else
                {
                    // Если значение некорректное, задержка остается равной 0
                    delayMs = 0;
                }
            });

            // Возвращаем значение задержки
            return delayMs;
        }
        private int CompareLines(string line1, string line2, int sortColumn)
        {
            var fields1 = line1.Split(',');
            var fields2 = line2.Split(',');

            if (sortColumn >= fields1.Length || sortColumn >= fields2.Length)
                throw new ArgumentException("Индекс ключа выходит за пределы строки.");

            string value1 = fields1[sortColumn].Trim();
            string value2 = fields2[sortColumn].Trim();

            if (double.TryParse(value1, out double num1) && double.TryParse(value2, out double num2))
            {
                return num1.CompareTo(num2);
            }

            return string.Compare(value1, value2, StringComparison.OrdinalIgnoreCase);
        }

        private void AddLog(string log)
        {
            Dispatcher.Invoke(() =>
            {
                logs.Add(log);
                Logs.Text = string.Join("\n", logs);
                Logs.ScrollToEnd();
            });
        }

        private void ClearLogs_Click(object sender, RoutedEventArgs e)
        {
            logs.Clear();
            Logs.Text = string.Empty;
        }
    }
}
