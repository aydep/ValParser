using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using NLog;

namespace ValParser
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            cursTypeListBox.SelectedItem = cursTypeListBox.Items[0];
            pathTextBox.Text = $"C:\\Users\\{Environment.UserName}\\Documents";
        }

        public static Logger logger = LogManager.GetCurrentClassLogger();

        private string url;

        private void ParseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(pathTextBox.Text)) //Проверка задан ли путь, если нет то сохранение в Документы
            {
                pathTextBox.Text = $"C:\\Users\\{Environment.UserName}\\Documents";
            }

            CB cb = new CB();

            if (cursTypeListBox.SelectedIndex == 0)
            {
                if (!mainPicker.SelectedDate.HasValue) //Если дата не установлена, получить псоследние данные
                {
                    url = "http://www.cbr.ru/scripts/XML_daily.asp";
                }
                else //Иначе получит данные на заданную дату
                {
                    string slectedDate = mainPicker.SelectedDate.Value.Date.ToString().Substring(0, 10).Replace(".", "/");//Форматирование даты
                    url = "http://www.cbr.ru/scripts/XML_daily.asp?date_req=" + slectedDate;
                }
                cursDataGrid.DataContext = cb.Parse(url, "ValCurs", pathTextBox.Text).DefaultView;
            }
            else
            {
                if (mainPicker.SelectedDate.HasValue && secondPicker.SelectedDate.HasValue)
                {
                    string slectedFirstDate = mainPicker.SelectedDate.Value.ToString().Substring(0, 10).Replace(".", "/");
                    string slectedSecondDate = secondPicker.SelectedDate.Value.ToString().Substring(0, 10).Replace(".", "/");
                    url = "https://www.cbr.ru/scripts/xml_metall.asp?date_req1=" + slectedFirstDate + "&date_req2=" + slectedSecondDate;
                    cursDataGrid.DataContext = cb.Parse(url, "Metall", pathTextBox.Text).DefaultView;

                }
                else
                {
                    System.Windows.MessageBox.Show("Выберите даты");
                }
            }
        }

        private void todayButton_Click(object sender, RoutedEventArgs e)
        {
            mainPicker.SelectedDate = DateTime.Now;
            secondPicker.SelectedDate = DateTime.Now;
        }

        private void latestButton_Click(object sender, RoutedEventArgs e)
        {
            mainPicker.Text.Remove(0);
        }

        private void vals_Selected(object sender, RoutedEventArgs e)
        {
            secondPicker.Visibility = Visibility.Hidden;
            Grid.SetColumnSpan(mainPicker, 2);

            lastButton.Visibility = Visibility.Visible;
            Grid.SetRowSpan(todayButton, 1);
        }

        private void metalls_Selected(object sender, RoutedEventArgs e)
        {
            secondPicker.Visibility = Visibility.Visible;
            Grid.SetColumnSpan(mainPicker, 1);

            lastButton.Visibility = Visibility.Hidden;
            Grid.SetRowSpan(todayButton, 2);
        }

        private void pathBtn_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowDialog();
            if (dlg.SelectedPath != null)
                pathTextBox.Text = dlg.SelectedPath;
        }
    }
}