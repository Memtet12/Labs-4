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
            }
        }

        private async void StartSort_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FilePath.Text) || string.IsNullOrWhiteSpace(SortColumn.Text))
            {
                MessageBox.Show("Укажите файл и ключ сортировки!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(SortColumn.Text, out int sortColumn) || sortColumn < 1)
            {
                MessageBox.Show("Ключ сортировки должен быть положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Размер буфера по умолчанию равен 2
            int bufferSize = 2;

            string inputFile = FilePath.Text;
            string outputFile = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(inputFile), $"{System.IO.Path.GetFileNameWithoutExtension(inputFile)}_sorted.txt");

            Logs.Text = "Начинается сортировка...\n";
            try
            {
                await Task.Run(() => ExternalSort(inputFile, outputFile, sortColumn - 1, bufferSize));
                AddLog($"Сортировка завершена! Результат сохранён в файле: {outputFile}");
            }
            catch (Exception ex)
            {
                AddLog($"Ошибка: {ex.Message}");
            }
        }

        private void ClearLogs_Click(object sender, RoutedEventArgs e)
        {
            logs.Clear();
            Logs.Text = string.Empty;
        }

        private void ExternalSort(string inputFile, string outputFile, int sortColumn, int bufferSize)
        {
            AddLog($"Читаем данные из файла: {inputFile}");
            var tempFiles = SplitFileIntoSortedChunks(inputFile, sortColumn, bufferSize);
            AddLog($"Создано {tempFiles.Count} временных файла для слияния.");

            MergeChunks(tempFiles, outputFile, sortColumn);

            foreach (var tempFile in tempFiles)
            {
                File.Delete(tempFile); // Удаляем временные файлы
                AddLog($"Удален временный файл: {tempFile}");
            }
        }

        private List<string> SplitFileIntoSortedChunks(string inputFile, int sortColumn, int bufferSize)
        {
            var tempFiles = new List<string>();
            var linesBuffer = new List<string>();

            AddLog("Начинаем разбиение файла на блоки...");

            using (var reader = new StreamReader(inputFile))
            {
                while (!reader.EndOfStream)
                {
                    linesBuffer.Add(reader.ReadLine());
                    if (linesBuffer.Count == bufferSize)
                    {
                        tempFiles.Add(SortAndSaveChunk(linesBuffer, sortColumn));
                        linesBuffer.Clear();
                    }
                }

                if (linesBuffer.Count > 0)
                {
                    tempFiles.Add(SortAndSaveChunk(linesBuffer, sortColumn));
                }
            }

            AddLog("Разбиение файла завершено.");
            return tempFiles;
        }

        private string SortAndSaveChunk(List<string> lines, int sortColumn)
        {
            AddLog("Сортируем текущий блок строк...");

            for (int i = 0; i < lines.Count - 1; i++)
            {
                for (int j = 0; j < lines.Count - i - 1; j++)
                {
                    // Получаем значения для сравнения
                    string value1 = lines[j].Split(',')[sortColumn].Trim();
                    string value2 = lines[j + 1].Split(',')[sortColumn].Trim();
                    AddLog($"Сравниваем: {value1} <=> {value2}");

                    if (CompareLines(lines[j], lines[j + 1], sortColumn) > 0)
                    {
                        // Если порядок неверный, меняем местами
                        AddLog($"Обмен значениями: \"{lines[j]}\" и \"{lines[j + 1]}\".");
                        var temp = lines[j];
                        lines[j] = lines[j + 1];
                        lines[j + 1] = temp;
                    }
                }
            }

            string tempFile = System.IO.Path.GetTempFileName();
            File.WriteAllLines(tempFile, lines);
            AddLog($"Сохранён отсортированный блок в файл: {tempFile}");

            return tempFile;
        }

        private void MergeChunks(List<string> tempFiles, string outputFile, int sortColumn)
        {
            AddLog("Начинаем слияние временных файлов...");

            var readers = tempFiles.Select(file => new StreamReader(file)).ToList();
            var priorityQueue = new SortedList<string, int>(new ChunkComparer(sortColumn));

            // Инициализация очереди
            for (int i = 0; i < readers.Count; i++)
            {
                if (!readers[i].EndOfStream)
                {
                    string line = readers[i].ReadLine();
                    priorityQueue.Add(line, i);
                }
            }

            using (var writer = new StreamWriter(outputFile))
            {
                while (priorityQueue.Count > 0)
                {
                    var smallest = priorityQueue.First();
                    writer.WriteLine(smallest.Key);
                    priorityQueue.RemoveAt(0);

                    int fileIndex = smallest.Value;
                    if (!readers[fileIndex].EndOfStream)
                    {
                        string line = readers[fileIndex].ReadLine();
                        priorityQueue.Add(line, fileIndex);
                    }
                }
            }

            foreach (var reader in readers)
            {
                reader.Dispose();
            }

            AddLog("Слияние временных файлов завершено.");
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

        private class ChunkComparer : IComparer<string>
        {
            private readonly int _sortColumn;

            public ChunkComparer(int sortColumn)
            {
                _sortColumn = sortColumn;
            }

            public int Compare(string x, string y)
            {
                var fieldsX = x.Split(',');
                var fieldsY = y.Split(',');

                string valueX = fieldsX[_sortColumn].Trim();
                string valueY = fieldsY[_sortColumn].Trim();

                if (double.TryParse(valueX, out double numX) && double.TryParse(valueY, out double numY))
                {
                    return numX.CompareTo(numY);
                }

                return string.Compare(valueX, valueY, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
