using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Windows.Forms.Integration;
using System.Windows.Forms.DataVisualization.Charting;
using MahApps.Metro.Controls;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();

            // 画像用
            this.setChartControl();
        }

        private double x = 0.0;
        private void setChartControl()
        {
            var windowsFormsHost = grid.FindName("WindowsFormsHost1") as WindowsFormsHost;
            //windowsFormsHost.Background = Brushes.Red;
            //windowsFormsHost.Opacity = 0;

            var chart = windowsFormsHost.Child as Chart;
            chart.BackColor = System.Drawing.Color.Transparent;

            // ChartArea追加
            chart.ChartAreas.Add("ChartArea1");
            chart.ChartAreas[0].BackColor = System.Drawing.Color.Transparent;

            // Seriesの作成と値の追加
            Series seriesSin = new Series();
            seriesSin.ChartType = SeriesChartType.Line;
            seriesSin.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;

            Series seriesCos = new Series();
            seriesCos.ChartType = SeriesChartType.Line;
            seriesCos.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;

            for (double x = this.x; x <= 2 * Math.PI; x = x + 0.1)
            {
                seriesSin.Points.AddXY(x, Math.Sin(x));
                seriesCos.Points.AddXY(x, Math.Cos(x));
            }

            chart.Series.Add(seriesSin);
            chart.Series.Add(seriesCos);

            this.x += 0.1;
        }
        private void updateSeries()
        {

        }
    }
}
