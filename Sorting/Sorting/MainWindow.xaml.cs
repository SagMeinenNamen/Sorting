using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
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

namespace Sorting
{
    public partial class MainWindow : Window
    {
        List<int> data = new List<int>();
        readonly List<Rectangle> rectangles = new List<Rectangle>();
        int amount = 100;

        public MainWindow()
        {
            InitializeComponent();
            ScrambleData();
            ShowData();
        }

        private void ScrambleData()
        {
            for (int i = 1; i <= amount; i++)
            {
                data.Add(i);
                Grid.RowDefinitions.Add(new RowDefinition());
                Grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            Random rnd = new Random();
            data = data.OrderBy(item => rnd.Next()).ToList();
        }

        private void ShowData()
        {
            for (int i = 0; i < data.Count; i++)
            {
                Rectangle rectangle = new Rectangle
                {
                    Fill = Brushes.White,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Uid = "Rectangle_" + (data[i])
                };
                rectangles.Add(rectangle);
                Grid.Children.Add(rectangle);
                Grid.SetColumn(rectangle, i);
                Grid.SetRow(rectangle, data.Count - data[i]);
                Grid.SetRowSpan(rectangle, data[i] + 1);
            }
        }



        private void BubbleSort()
        {
            for (int j = data.Count - 1; j > 0; j--)
            {
                for (int i = 0; i < j; i++)
                {
                    if (data[i] > data[i + 1])
                    {
                        int temporary = data[i];
                        data[i] = data[i + 1];
                        data[i + 1] = temporary;
                        Task task = new Task(VisualizeSorting, i);
                        task.Start();
                    }
                }
            }

        }

        private void QuickSort(List<int> data, int start, int end)
        {
            if (start < end)
            {
                int piv_pos = Partition(data, start, end);
                QuickSort(data, start, piv_pos - 1);
                QuickSort(data, piv_pos + 1, end);
                Task task = new Task(VisualizeSorting, start);
                task.Start();
                Task task2 = new Task(VisualizeSorting, end);
                task2.Start();
            }
        }
        private int Partition(List<int> arr, int start, int end)
        {
            int i = start - 1;
            int piv = arr[end];
            for (int j = start; j <= end - 1; j++)
            {
                if (arr[j] <= piv)
                {
                    i++;
                    (arr[i], arr[j]) = (arr[j], arr[i]);

                }
            }
            (arr[end], arr[i + 1]) = (arr[i + 1], arr[end]);
            return i + 1;
        }


        private async void VisualizeSorting(object _num)
        {
            try
            {
                int num = (int)_num;
                for (int j = 0; j < 2; j++)
                {
                    num += j;
                    if (data.Contains(num))
                    {
                        Thread.Sleep(10);
                        Rectangle rectangle = null;
                        await Dispatcher.BeginInvoke(new Action(() =>
                        {
                            rectangle = rectangles.Where(item => item.Uid == "Rectangle_" + num).FirstOrDefault();
                            rectangle.Fill = Brushes.Red;
                            Grid.SetColumn(rectangle, num - 1);
                        }));
                        await Dispatcher.BeginInvoke(new Action(() => rectangle.Fill = Brushes.White));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (amount > 2000)
            {
                var result = MessageBox.Show("The number you entered is bigger than 2000. This will maybe cause the GUI to freeze, therefore not being able to see the progress. Do you want to continue?", "Continue?", MessageBoxButton.YesNo);
                if (result != MessageBoxResult.Yes)
                    return;
            }
            Task.Run(StartQuickSort);
        }

        private void StartQuickSort()
        {
            QuickSort(data, 0, data.Count - 1);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.IsLoaded)
            {
                if (Input.Text.Length > 4)
                {
                    Input.TextChanged -= TextBox_TextChanged;
                    Input.Text = Input.Text.Substring(0, 4);
                    Input.CaretIndex = Input.Text.Length;
                    Input.TextChanged += TextBox_TextChanged;
                    return;
                }
                try
                {
                    if (string.IsNullOrWhiteSpace(Input.Text) || ContainsOnlyZero(Input.Text))
                        amount = 1;
                    else
                        amount = Convert.ToInt32(Input.Text);

                    ResetVariables();
                    ScrambleData();
                    Grid.SetColumnSpan(btn, amount);
                    ShowData();

                }
                catch
                {
                    MessageBox.Show("Not a valid number");
                }
            }
        }

        private void ResetVariables()
        {
            Grid.Children.Clear();
            Grid.RowDefinitions.Clear();
            Grid.ColumnDefinitions.Clear();
            data.Clear();
            rectangles.Clear();
        }

        private bool ContainsOnlyZero(string str)
        {
            foreach (var c in str)
            {
                if (c != '0')
                {
                    return false;
                }
            }
            return true;
        }
    }
}
