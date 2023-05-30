using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;

namespace CourseFEM
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var _sigma = double.Parse(SigmaTextBox.Text);
            var _ro = double.Parse(RoTextBox.Text);
            var _c = double.Parse(CTextBox.Text);
            var _e = double.Parse(ETextBox.Text);
            var _g = double.Parse(GTextBox.Text);
            var _d = double.Parse(DTextBox.Text);
            var _n = int.Parse(NTextBox.Text);
            var _l = double.Parse(LTextBox.Text);
            FEMSolver fEMSolver = new FEMSolver(_sigma, _ro, _c, _e, _g, _d, _n, _l);

            fEMSolver.Configure();
            double[] u;
            double[] p;
            fEMSolver.SplitVector(out u, out p);

            PlotVector(u, plotView1, "u", _l);
            PlotVector(p, plotView2, "p", _l);
        }
        public static void PlotVector(double[] vector, PlotView plotView, string name, double L)
        {
            // Create a new plot model
            var plotModel = new PlotModel();

            // Create a line series to represent the vector values
            var lineSeries = new LineSeries();

            // Add the data points to the line series
            for (int i = 0; i < vector.Length; i++)
            {
                double x = i * (L / (vector.Length - 1));
                double y = vector[i];
                lineSeries.Points.Add(new DataPoint(x, y));
            }
            //customization
            switch (name)
            {
                case "u":
                    plotModel.Title = "U(x) Plot";
                    plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X" });
                    plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "u(x)" });
                    break;
                case "p":
                    plotModel.Title = "P(x) Plot";
                    plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X" });
                    plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "p(x)" });
                    break;
                default:
                    break;
            }



            // Add the line series to the plot model
            plotModel.Series.Add(lineSeries);

            // Set up the plot axes


            // Set the plot model to the plot view
            plotView.Model = plotModel;
        }
    }
}