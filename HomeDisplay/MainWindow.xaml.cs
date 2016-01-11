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
using System.Net;
using System.IO;

namespace HomeDisplay
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer dispatcherTimer1 = new DispatcherTimer();
        private DispatcherTimer dispatcherTimer2 = new DispatcherTimer();
        private string currTemp;
        private string currCond;
        private string currHumid;
        private string currWind;
        private string location;
        private string foreCond;
        private string foreHigh;
        private string foreLow;
        private string imgCode;
        private string pubDate;
        private BitmapImage sourceImg = new BitmapImage();
        
        public MainWindow()
        {
            InitializeComponent();
            dispatcherTimer1.Tick += dispatcherTimer1_Tick;
            dispatcherTimer1.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer1.Start();

            label.Content = DateTime.Now.ToLongDateString();
            label1.Content = DateTime.Now.ToString("HH:mm:ss");

            dispatcherTimer2.Tick += dispatcherTimer2_Tick;
            dispatcherTimer2.Interval = new TimeSpan(0, 0, 1);
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

        public void mainPageDisable()
        {
            label3.Visibility = Visibility.Hidden;
            label4.Visibility = Visibility.Hidden;
            label6.Visibility = Visibility.Hidden;
            label8.Visibility = Visibility.Hidden;
            label10.Visibility = Visibility.Hidden;
            label11.Visibility = Visibility.Hidden;
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

            currWind = chan.SelectSingleNode("yweather:wind", dataMan).Attributes["chill"].Value;

            location = chan.SelectSingleNode("yweather:location", dataMan).Attributes["city"].Value;

            foreCond = chan.SelectSingleNode("item").SelectSingleNode("yweather:forecast", dataMan).Attributes["text"].Value;

            foreHigh = chan.SelectSingleNode("item").SelectSingleNode("yweather:forecast", dataMan).Attributes["high"].Value;

            foreLow = chan.SelectSingleNode("item").SelectSingleNode("yweather:forecast", dataMan).Attributes["low"].Value;

            imgCode = chan.SelectSingleNode("item").SelectSingleNode("yweather:condition", dataMan).Attributes["code"].Value;

            pubDate = chan.SelectSingleNode("item").SelectSingleNode("yweather:condition", dataMan).Attributes["date"].Value;

            label3.Content = currTemp + " C";
            label4.Content = currCond;
            label6.Content = currHumid;
            label8.Content = foreHigh + " C";
            label10.Content = foreLow + " C";
            label11.Content = "Last Updated: " + pubDate;

            retrieveWeatherIconFromWeb();
            //retrieveWeatherIconFromFolder();
            image.Source = sourceImg;
            if(sourceImg != null)
            {
                sourceImg = new BitmapImage();
            }

        }

        private void retrieveWeatherIconFromFolder()
        {
            if (imgCode.Equals("3200"))
            {
                sourceImg = new BitmapImage(new Uri(sourceImg.BaseUri, "Images/na.png"));
            }
            else
            {
                sourceImg = new BitmapImage(new Uri(sourceImg.BaseUri, "Images/" + imgCode + ".png"));
            }
        }

        private void retrieveWeatherIconFromWeb()
        {
            int bytesToRead = 100;

            WebRequest req = WebRequest.Create(new Uri("http://l.yimg.com/a/i/us/we/52/" + imgCode + ".gif", UriKind.Absolute));
            req.Timeout = -1;
            WebResponse resp = req.GetResponse();
            Stream respStr = resp.GetResponseStream();
            BinaryReader reader = new BinaryReader(respStr);
            MemoryStream memStr = new MemoryStream();

            byte[] bytebuf = new byte[bytesToRead];
            int bytesRead = reader.Read(bytebuf, 0, bytesToRead);

            while (bytesRead > 0)
            {
                memStr.Write(bytebuf, 0, bytesRead);
                bytesRead = reader.Read(bytebuf, 0, bytesToRead);
            }

            sourceImg.BeginInit();
            memStr.Seek(0, SeekOrigin.Begin);
            sourceImg.StreamSource = memStr;
            sourceImg.EndInit();
        }
    }
}
