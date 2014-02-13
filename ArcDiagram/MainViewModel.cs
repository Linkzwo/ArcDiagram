using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcDiagram
{
    class MainViewModel : Observable
    {
        private PlotModel model;
        private Axis xaxis;
        private Axis yaxis;

        public MainViewModel()
        {
            // Create the plot model
            var tmp = new PlotModel("Simple Arc Diagram Example", "using OxyPlot");
            tmp.PlotAreaBorderColor = OxyColors.Transparent;
            tmp.IsLegendVisible = false;

            model = tmp;
        }

        public void setAxis(string s)
        {
            Console.WriteLine(s.Length);
            string[] items = s.Select(c => c.ToString()).ToArray();
            model.Axes.Clear();
            // Axes are created automatically if they are not defined
            var xaxis = new CategoryAxis("", items);
            if (s.Length > 200) {xaxis = new CategoryAxis(); } //do not show content if it is too much
            xaxis.TickStyle = TickStyle.None;
            xaxis.IsAxisVisible = true;
            xaxis.MinimumRange = 1;
            //xaxis.PositionAtZeroCrossing = true;

            var yaxis = new LinearAxis();
            yaxis.IsAxisVisible = false;
            yaxis.Position = AxisPosition.Left;
            yaxis.TickStyle = TickStyle.None;
            yaxis.MinimumRange = 1;
            //yaxis.IsPanEnabled = false;
            yaxis.AxislineColor = OxyColors.Transparent;

            this.Xaxis = xaxis;
            this.Yaxis = yaxis;

            model.Axes.Add(xaxis);
            model.Axes.Add(yaxis);
        }

        public void drawArc(double center, double outerR, double innerR, string title)
        {
            var areaSeries = new AreaSeries();
            var data = new Collection<ArcData>();
            areaSeries.Title = title;
            areaSeries.Tag = outerR - innerR;

            for (double i = 0; i <= Math.PI; i += 0.01)
            {             

                data.Add(new ArcData
                {
                  X1 = center  + outerR * Math.Cos(i),
                  X2 = center + innerR * Math.Cos(i),
                  Y1 = outerR * Math.Sin(i),
                  Y2 = innerR * Math.Sin(i)
                });
            }
            
            areaSeries.ItemsSource = data;
            areaSeries.DataFieldX = "X1";
            areaSeries.DataFieldY = "Y1";
            areaSeries.DataFieldX2 = "X2";
            areaSeries.DataFieldY2 = "Y2";
            areaSeries.Fill = OxyColor.FromAColor(80, OxyColors.LightBlue);
            areaSeries.StrokeThickness = 0;

            this.model.Series.Add(areaSeries);
        }

        public void drawSimpleArcLine(double center, double radius)
        {
            var funcSeries = new FunctionSeries(t => center + radius * Math.Cos(t), t => radius * Math.Sin(t), 0, Math.PI, 0.01);
            funcSeries.Color = OxyColor.FromAColor(80, OxyColors.LightBlue);
            this.model.Series.Add(funcSeries);
        }

        public PlotModel Model
        {
            get { return model; }
            set
            {
                if (model != value)
                {
                    model = value;
                    RaisePropertyChanged(() => Model);
                }
            }
        }

        public Axis Xaxis { 
            get { return this.xaxis; }
            set { this.xaxis = value; }
        }

        public Axis Yaxis
        {
            get { return this.yaxis; }
            set { this.yaxis = value; }
        }

    }

    public class ArcData
    {
        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }
    }
}

