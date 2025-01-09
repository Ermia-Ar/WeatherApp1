using System.Windows;
using System.Net;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using Newtonsoft.Json;
using WeatherApp.JSON;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Runtime.InteropServices.JavaScript.JSType;



namespace WeatherApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string cityname;
        public static WebClient Client = new WebClient();
        public PlotModel MyModel { get; private set; }
        private const string IpApiUrl = "https://ep.api.getfastah.com/whereis/v1/json/auto?fastah-key=638eec4613e64929aa0a4d7505b6d8cb";
        public static string URL = $"https://api.weatherbit.io/v2.0/forecast/daily?&lat={GetLocationFromIpAsync().locationData.lat}&lon={GetLocationFromIpAsync().locationData.lng}&key=985f222ea9f64461ad09ecac58ab694e&units=I";
        public List<Datum> ai = new List<Datum>(WeatherInfo().data);
        

        public MainWindow()
        {
            InitializeComponent();
            createModel();
            City.Text = cityname;
            DataContext = this;

        }


        public void createModel()
        {
            MyModel = new PlotModel();

            var lineSeries = new LineSeries();


            string[] ali1 = ai[0].valid_date.Split("-");
            string[] ali2 = ai[1].valid_date.Split("-");
            string[] ali3 = ai[2].valid_date.Split("-");
            string[] ali4 = ai[3].valid_date.Split("-");
            string[] ali5 = ai[4].valid_date.Split("-");
            string[] ali6 = ai[5].valid_date.Split("-");
            string[] ali7 = ai[6].valid_date.Split("-");

            // Sample Weekly Data (Start Date, Value)
            var weeklyData = new List<Tuple<DateTime, double>>
            {
                new Tuple<DateTime, double>(new DateTime(int.Parse(ali1[0]) , int.Parse(ali1[1]) ,int.Parse(ali1[2])),(ai[0].temp - 32 ) /1.8),
                new Tuple<DateTime, double>(new DateTime(int.Parse(ali2[0]) , int.Parse(ali2[1]) ,int.Parse(ali2[2])),(ai[1].temp - 32 ) /1.8),
                new Tuple<DateTime, double>(new DateTime(int.Parse(ali3[0]) , int.Parse(ali3[1]) ,int.Parse(ali3[2])),(ai[2].temp - 32 ) /1.8),
                new Tuple<DateTime, double>(new DateTime(int.Parse(ali4[0]) , int.Parse(ali4[1]) ,int.Parse(ali4[2])),(ai[3].temp - 32 ) /1.8),
                new Tuple<DateTime, double>(new DateTime(int.Parse(ali5[0]) , int.Parse(ali5[1]) ,int.Parse(ali5[2])),(ai[4].temp - 32 ) /1.8),
                new Tuple<DateTime, double>(new DateTime(int.Parse(ali6[0]) , int.Parse(ali6[1]) ,int.Parse(ali6[2])),(ai[5].temp - 32 ) /1.8),
                new Tuple<DateTime, double>(new DateTime(int.Parse(ali7[0]) , int.Parse(ali7[1]) ,int.Parse(ali7[2])),(ai[6].temp - 32 ) /1.8),

            };
            // Fill data
            Des.Text = (ai[0].weather.description);
            string ali = ai[0].weather.icon;
            var uriSource = new Uri($"Images\\{ali}.png", UriKind.Relative);
            Weather_Image.Source = new BitmapImage(uriSource);
            Temp.Text = Math.Round((ai[0].temp -32) /1.8) + "°C";

            // Add data points to the series
            foreach (var dataPoint in weeklyData)
            {
                lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(dataPoint.Item1), dataPoint.Item2));
            }

            lineSeries.Color = OxyColor.Parse("#FFFFAD5B");
            lineSeries.Background = OxyColor.Parse("#FFFDF1C4");
            lineSeries.MouseDown += LineSeries_MouseDown;

            MyModel.Series.Add(lineSeries);

            // Create a DateTimeAxis for the X-axis
            var dateAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "yyyy-MM-dd", // Format the date as needed
            };


            var dateAxis1 = new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = (WeatherInfo().data[0].min_temp - 32) / 1.8 - 3,
                Maximum = (WeatherInfo().data[0].max_temp - 32) / 1.8 + 3,
                Title = "°C",


                // IntervalType = DateTimeIntervalType.Weeks, // Set interval to weeks
            };


            MyModel.Axes.Add(dateAxis);
            MyModel.Axes.Add(dateAxis1);

        }

        private void LineSeries_MouseDown(object? sender, OxyMouseDownEventArgs e)
        {
            if (e.HitTestResult != null && e.HitTestResult.Item != null)
            {
                DataPoint hitPoint = (DataPoint)e.HitTestResult.Item;

                // Convert the X value (which is a double representing the date) back to a DateTime
                DateTime date = DateTimeAxis.ToDateTime(hitPoint.X);

                // Access the Y value (temperature)
                //double temperature = hitPoint.Y;



                //Find the datum that corresponds to the clicked point

                foreach (Datum datum in ai)
                {
                    if (DateTime.Parse(datum.valid_date).Date == date.Date)
                    {
                        Des.Text = (datum.weather.description);
                        //Icons\pencil.png
                        string ali = datum.weather.icon;
                        var uriSource = new Uri($"Images\\{ali}.png", UriKind.Relative);
                        Weather_Image.Source = new BitmapImage(uriSource);
                        Temp.Text = Math.Round( (datum.temp -32) / 1.8) + "°C";
                    }
                }
            }

        }
        public static Root1 GetLocationFromIpAsync()
        {
            string json = Client.DownloadString(IpApiUrl);
            Root1 ali = JsonConvert.DeserializeObject<Root1>(json);

            return ali;


        }
        public static Root WeatherInfo()
        {
            string json = Client.DownloadString(URL);
            Root ali = JsonConvert.DeserializeObject<Root>(json);
            cityname = ali.city_name;

            return ali;
        }
    }
}