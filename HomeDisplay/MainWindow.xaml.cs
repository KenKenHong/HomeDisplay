using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace HomeDisplay
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer dispatcherTimer1 = new DispatcherTimer();
        DispatcherTimer dispatcherTimer2 = new DispatcherTimer();
        string currTemp;
        string currCond;
        string currHumid;
        string currWind;
        string location;
        string foreCond;
        string foreHigh;
        string foreLow;
        
        public MainWindow()
        {
            InitializeComponent();
            dispatcherTimer1.Tick += dispatcherTimer1_Tick;
            dispatcherTimer1.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer1.Start();

            label.Content = DateTime.Now.ToLongDateString();
            label1.Content = DateTime.Now.ToString("HH:mm:ss");

            dispatcherTimer2.Tick += dispatcherTimer2_Tick;
            dispatcherTimer2.Interval = new TimeSpan(1, 0, 0);
            dispatcherTimer2.Start();

            getWeather();
        }

        public void dispatcherTimer1_Tick(object Sender, EventArgs e)
        {
            label.Content = DateTime.Now.ToLongDateString();
            label1.Content = DateTime.Now.ToString("HH:mm:ss");
        }

        public void dispatcherTimer2_Tick(object Sender, EventArgs e)
        {
            getWeather();

        }

        public void getWeather()
        {
            string query = String.Format("http://weather.yahooapis.com/forecastrss?w=2972&u=c");

            XmlDocument currData = new XmlDocument();
            currData.Load(query);

            XmlNamespaceManager dataMan = new XmlNamespaceManager(currData.NameTable);
            dataMan.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");

            XmlNode chan = currData.SelectSingleNode("rss").SelectSingleNode("channel");
            XmlNodeList nodeList = currData.SelectNodes("/rss/channel/item/yweather:forecast", dataMan);

            currTemp = chan.SelectSingleNode("item").SelectSingleNode("yweather:condition", dataMan).Attributes["temp"].Value;

            currCond = chan.SelectSingleNode("item").SelectSingleNode("yweather:condition", dataMan).Attributes["text"].Value;

            currHumid = chan.SelectSingleNode("yweather:atmosphere", dataMan).Attributes["humidity"].Value;

            currWind = chan.SelectSingleNode("yweather:wind", dataMan).Attributes["speed"].Value;

            location = chan.SelectSingleNode("yweather:location", dataMan).Attributes["city"].Value;

            foreCond = chan.SelectSingleNode("item").SelectSingleNode("yweather:forecast", dataMan).Attributes["text"].Value;

            foreHigh = chan.SelectSingleNode("item").SelectSingleNode("yweather:forecast", dataMan).Attributes["high"].Value;

            foreLow = chan.SelectSingleNode("item").SelectSingleNode("yweather:forecast", dataMan).Attributes["low"].Value;

            label3.Content = currTemp + " C";
            label4.Content = currCond;
            label6.Content = currHumid;
            label8.Content = foreHigh + " C";
            label10.Content = foreLow + " C";
        }
    }
}
