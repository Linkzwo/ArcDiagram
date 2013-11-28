using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramTest
{
    class MainViewModel : Observable
    {
        private PlotModel model;

        public MainViewModel()
        {
            // Create the plot model
            var tmp = new PlotModel("Simple Arc Diagram Example", "using OxyPlot");
            
            // Draw Arcs
            drawSimpleArcLine(tmp, 4, 2);
            drawArc(tmp, 10,5,3);
            drawArc(tmp, 8, 1, 0.5);
            drawArc(tmp, 6, 1, 0.5);
 
            // Axes are created automatically if they are not defined
            var xaxis = new CategoryAxis(AxisPosition.Bottom, "Names", new String[] { "Yannick", "Sabrina", "Tine", "Hallo" });
            xaxis.Angle = -90;
            xaxis.Minimum = 0;
            //xaxis.Maximum = 10;
            xaxis.MaximumPadding = 0;
            xaxis.MinimumPadding = 0;

            var yaxis = new LinearAxis();
            yaxis.IsAxisVisible = false;
            yaxis.Position = AxisPosition.Left;
            yaxis.Minimum = 0;
           

            tmp.Axes.Add(xaxis);
            tmp.Axes.Add(yaxis);
            // Set the Model property, the INotifyPropertyChanged event will make the WPF Plot control update its content
            Model = tmp;
        }

        public void drawArc(PlotModel pm, double center, double outerR, double innerR)
        {
            var areaSeries = new AreaSeries();
            var data = new Collection<ArcData>();

            for (double i = 0; i <= Math.PI; i += 0.001)
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

            pm.Series.Add(areaSeries);
        }

        public void drawSimpleArcLine(PlotModel pm, double center, double radius)
        {
            var funcSeries = new FunctionSeries(t => center + radius * Math.Cos(t), t => radius * Math.Sin(t), 0, Math.PI, 0.001);
            funcSeries.Color = OxyColor.FromAColor(80, OxyColors.LightBlue);
            pm.Series.Add(funcSeries);
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

    }

    public class ArcData
    {
        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }
    }
}

