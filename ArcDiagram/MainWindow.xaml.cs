using OxyPlot;
using OxyPlot.Axes;
using System;
using System.IO;
using System.Collections.Generic;
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
using System.Text.RegularExpressions;

namespace ArcDiagram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private MainViewModel mvm = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = mvm;
            this.txtInput.Text = "abcd111110000011111abcd";
            DrawAllArcs();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DrawAllArcs();
        }

        private void DrawAllArcs()
        {
            mvm.Model.Series.Clear();
            string s = this.txtInput.Text;
            var lstDups = getDuplicates(s);
            List<RepetitionArea> repAreas = FindRepetitionAreas(s);
            Dictionary<string, List<int>> lstStringOcc = new Dictionary<string, List<int>>();


            foreach (var item in lstDups)
            {
                if (!lstDups.Any(x => !x.Equals(item) && x.Contains(item)))
                {
                    List<int> occurences = AllIndexesOf(s, item);
                    if (occurences.Count > 1 && !repAreas.Any(x => x.Substring.Contains(item)))
                    {
                        lstStringOcc.Add(item, occurences);
                        EnterArc(item, occurences);
                    }
                }
            }

            var areasToDraw = new List<RepetitionArea>();
            foreach (var item in repAreas)
            {
                if (repAreas.FindAll(x => x.Substring.Equals(item.Substring)).Count > 1)
                {
                    if (!areasToDraw.Any(x => x.Substring.Equals(item.Substring)))
                        areasToDraw.Add(item);
                }
            }

            areasToDraw.ForEach(x => EnterArc(x.Substring, AllIndexesOf(s, x.Substring)));

            foreach (var item in repAreas)
            {
                var lstOcc = AllIndexesOf(s, item.MinRepetition);
                lstOcc.RemoveAll(x => x < item.Start || x > (item.Start + item.Substring.Length));
                EnterArc(item.MinRepetition, lstOcc);
            }

            if (!sldrMinArcLength.IsEnabled) sldrMinArcLength.IsEnabled = true;
            sldrMinArcLength.Value = 0;
            mvm.Model.RefreshPlot(true);

        }

        private List<RepetitionArea> FindRepetitionAreas(string s)
        {
            List<RepetitionArea> reps = new List<RepetitionArea>();
            Regex r = new Regex(@"(.+?)\1+");

            foreach (Match item in r.Matches(s))
            {
                RepetitionArea ra = new RepetitionArea()
                {
                    Start = item.Index,
                    Parent = s,
                    Substring = item.Groups[0].Value,
                    MinRepetition = item.Groups[1].Value
                };
                reps.Add(ra);
            }
            return reps;
        }

        private List<string> getDuplicates(string s)
        {

            List<string> returnList = new List<string>();

            //.get all possible combinations of a string
            for (int i = 0; i < s.Length; i++)
            {
                for (int j = 1; j != s.Length - i + 1; j++)
                {
                    returnList.Add(s.Substring(i, j));
                }
            }
            return returnList.GroupBy(x => x)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();
        }
        
        private static List<int> AllIndexesOf(string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        private void EnterArc(string s, List<int> lst)
        {
            
            var overlappingArcs = true; //true if you want to have overlapping arcs

            var q = new Queue<int>(lst);
            double center, innerR, outerR;
            int first = q.Dequeue();

            while (q.Count > 0)
            {
                center = ((double)((first + s.Length - 1) + q.Peek()) / 2);
                if((q.Peek() - (first + s.Length - 1)) != 1){                    
                    innerR = center - (first + s.Length - 1) ;
                    outerR = center - (first + s.Length - 1) + s.Length - 1;
                    if (s.Length == 1)
                    {
                        innerR = outerR - 0.5;
                        if (overlappingArcs)outerR += 0.125;
                        if (overlappingArcs) innerR += 0.125;
                    }
                    else
                    {
                        if (overlappingArcs) outerR += 0.125;
                        if (overlappingArcs) innerR -= 0.125;
                    }
                }else {
                    innerR = 0.25;
                    outerR = 0.5;
                    if (overlappingArcs) outerR += 0.125;
                    if (overlappingArcs) innerR += 0.125;
                }
                
                mvm.drawArc(center, outerR , innerR);
                first = q.Dequeue();
            }
        }

        private void sldrMinArcLength_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mvm.Model.Series.ToList().ForEach(x => x.IsVisible = true);
            var list = mvm.Model.Series.Where(x => (double)x.Tag < (double)sldrMinArcLength.Value);
            list.ToList().ForEach(x => x.IsVisible = false);
            
            mvm.Model.RefreshPlot(true);
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) DrawAllArcs();
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.DefaultExt = ".pdf";
            sfd.Filter = "Portable Document File (*.pdf)|*.pdf";

            if (sfd.ShowDialog() == true)
            {
                using (var stream = File.Create(sfd.FileName))
                {
                    OxyPlot.Pdf.PdfExporter.Export(this.mvm.Model, stream, 600, 400);
                }
            }
        }

    }

    class RepetitionArea
    {
        public int Start { get; set; }
        public string Substring { get; set; }
        public string Parent { get; set; }
        public string MinRepetition { get; set; }
    }
}