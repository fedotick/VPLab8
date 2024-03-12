using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;

namespace lab7
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string filePath = "";
        List<WeatherModel> weather = new List<WeatherModel>();

        public MainWindow()
        {
            InitializeComponent();
            save.IsEnabled = false;
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    foreach (var data in weather)
                    {
                        sw.WriteLine($"{data.Date.ToString()},{data.Temp},{data.Humidity},{data.Weather}");
                    }
                }

                MessageBox.Show("Changes saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void open_file_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    filePath = openFileDialog.FileName;

                    var lines = System.IO.File.ReadAllLines(filePath);

                    if (weather.Count() > 0)
                    {
                        weather.Clear();
                    }

                    foreach (var line in lines)
                    {
                        var values = line.Split(',');

                        if (values.Length == 4)
                        {
                            WeatherModel weatherData = new WeatherModel
                            {
                                Date = DateOnly.Parse(values[0]),
                                Temp = double.Parse(values[1]),
                                Humidity = int.Parse(values[2]),
                                Weather = values[3]
                            };

                            weather.Add(weatherData);
                        }
                    }

                    file_data.ItemsSource = weather;
                    file_data.Items.Refresh();
                    save.IsEnabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void DataGridRow_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                file_data.SelectedItem = row.DataContext;
            }
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (file_data.SelectedItem != null)
                {
                    var selectedRow = file_data.SelectedItem as WeatherModel;
                    weather.Remove(selectedRow);
                    file_data.Items.Refresh();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void chart_Click(object sender, RoutedEventArgs e)
        {
            ChartWindow chartw = new ChartWindow(weather);
            chartw.weather = weather;
            chartw.Show();
        }

        private void close_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
