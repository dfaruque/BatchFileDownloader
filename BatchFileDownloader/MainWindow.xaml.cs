using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
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

namespace BatchFileDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {

            using (WebClient client = new WebClient())
            {
                client.DownloadFileAsync(new Uri("http://i.ebayimg.com/00/s/MTYwMFgxMjcw/z/4SIAAOSwM6NZsZ6u/$_12.JPG")
                    , @"f:\image35.jpg");

                client.DownloadFileCompleted += Client_DownloadFileCompleted;
            }
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            labelProgress.Content = "completed";
        }

        //private TaskCompletionSource<bool> _tcs;

        //private async void DownloadForm_Shown(object sender, EventArgs e)
        //{
        //    WebClient client = new WebClient();
        //    client.DownloadProgressChanged += client_DownloadProgressChanged;
        //    client.DownloadFileCompleted += client_DownloadFileCompleted;
        //    await startDownload(client);
        //}

        //void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        //{
        //    progressBar.Value = e.ProgressPercentage;
        //    labelProgress.Content = String.Format("Downloaded {0} of {1} bytes", e.BytesReceived, e.TotalBytesToReceive);
        //}

        //void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        //{
        //    // whatever else you have in this event handler, and then...
        //    _tcs.SetResult(true);
        //}

        //private async Task startDownload(WebClient client)
        //{
        //    //files contains all URL links
        //    foreach (string str in files)
        //    {
        //        //location is variable where file will be stored
        //        _tcs = new TaskCompletionSource<bool>();
        //        client.DownloadFileAsync(new Uri(str), location);
        //        await _tcs.Task;
        //    }
        //    _tcs = null;
        //}
    }
}
