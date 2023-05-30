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
            FEMSolver.Configure();
            double[] u;
            double[] v;
            FEMSolver.SplitVector(out u, out v);

            PlotVector(u, plotView1);
            PlotVector(v, plotView2);
            // run calculations get vector u and p
            // plot results u to x and p to x
        }
        public static void PlotVector(double[] vector, PlotView plotView)
        {
            // Create a new plot model
            var plotModel = new PlotModel();

            // Create a line series to represent the vector values
            var lineSeries = new LineSeries();

            // Add the data points to the line series
            for (int i = 0; i < vector.Length; i++)
            {
                double x = (double)i / (vector.Length - 1);
                double y = vector[i];
                lineSeries.Points.Add(new DataPoint(x, y));
            }

            // Add the line series to the plot model
            plotModel.Series.Add(lineSeries);

            // Set up the plot axes
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = 0, Maximum = 1 });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left });

            // Set the plot model to the plot view
            plotView.Model = plotModel;
        }
    }
}