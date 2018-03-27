using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

        List<FileDownloadModel> downloadList = new List<FileDownloadModel>();

        private async void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            downloadList = GetDownloadListFromExcel();


            using (WebClient client = new WebClient())
            {
                client.DownloadProgressChanged += client_DownloadProgressChanged;
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                await StartDownload(client);

                //    client.DownloadFileAsync(new Uri("http://i.ebayimg.com/00/s/MTYwMFgxMjcw/z/4SIAAOSwM6NZsZ6u/$_12.JPG")
                //        , @"f:\image35.jpg");

                //    client.DownloadFileCompleted += Client_DownloadFileCompleted;
            }
        }

        private List<FileDownloadModel> GetDownloadListFromExcel()
        {
            List<FileDownloadModel> downloadList = new List<FileDownloadModel>();
            var fileExtensionsToDownload = new string[] { ".jpg", ".png" };

            ExcelPackage ep = new ExcelPackage();
            using (var fs = new FileStream(txtExcelFile.Text, FileMode.Open, FileAccess.Read))
                ep.Load(fs);

            var worksheet = ep.Workbook.Worksheets[1];

            int columnIndex = 1;
            while (!string.IsNullOrWhiteSpace(worksheet.Cells[1, columnIndex].Value?.ToString()))
            {
                var folderName = worksheet.Cells[1, columnIndex].Value?.ToString().Replace('/', '_');

                int rowIndex = 2;
                while (!string.IsNullOrWhiteSpace(worksheet.Cells[rowIndex, columnIndex].Value?.ToString()))
                {
                    var url = worksheet.Cells[rowIndex, columnIndex].Value?.ToString();

                    if (url.LastIndexOf('.') > 0)
                    {
                        var fileExt = url.Substring(url.LastIndexOf('.')).ToLower();
                        if (fileExtensionsToDownload.Contains(fileExt))
                        {
                            var fileName = folderName + "__" + (rowIndex - 1) + fileExt;

                            downloadList.Add(new FileDownloadModel
                            {
                                Url = url,
                                FolderName = folderName,
                                FileName = fileName,
                                ExcelCellAddress = worksheet.Cells[rowIndex, columnIndex].FullAddressAbsolute
                            });
                        }
                    }

                    rowIndex++;
                }


                columnIndex++;
            }

            dtGrid.ItemsSource = downloadList;

            labelTotal.Content = $"Total files to download : {downloadList.Count}";
            labelProgress.Content = $"Download completed : {0}";
            return downloadList;
        }


        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            downloadList = GetDownloadListFromExcel();
            dtGrid.ItemsSource = downloadList;

        }

        private TaskCompletionSource<bool> _tcs;


        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //labelProgress.Content = String.Format("Downloaded {0} of {1} bytes", e.BytesReceived, e.TotalBytesToReceive);
        }

        void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // whatever else you have in this event handler, and then...
            if (e.Error == null)
            {
                _tcs.SetResult(true);
            }
            else _tcs.SetResult(false);
        }

        private async Task StartDownload(WebClient client)
        {
            int currentItem = 1;
            foreach (var item in downloadList)
            {
                _tcs = new TaskCompletionSource<bool>();

                var folderPath = txtRootFolder.Text + "\\" + item.FolderName;
                Directory.CreateDirectory(folderPath);
                var filePath = folderPath + "\\" + item.FileName;

                client.DownloadFileAsync(new Uri(item.Url), filePath);
                var downloaded = await _tcs.Task;

                if (downloaded)
                {
                    labelProgress.Content = String.Format("{0} of {1} Items Downloaded", currentItem++, downloadList.Count);
                    item.IsDownloaded = true;
                }
            }
            _tcs = null;
        }
    }

    public class FileDownloadModel
    {
        public string Url { get; set; }
        //public string FileExtension { get; set; }
        public string FolderName { get; set; }
        public string FileName { get; set; }
        public bool IsDownloaded { get; set; }
        public string ExcelCellAddress { get; set; }
    }
}
