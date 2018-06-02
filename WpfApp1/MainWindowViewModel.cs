using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace WpfApp1
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainViewCommand StartCommand { get; set; }
        public MainViewCommand StopCommand { get; set; }
        public MainWindowViewModel()
        {
            this.StartCommand = new MainViewCommand(new EventDelegate(start));
            this.StopCommand = new MainViewCommand(new EventDelegate(stop));
        }
        private bool isTriggered = false;

        private bool isProcessing = true;
        public bool IsProcessing
        {
            get
            {
                return this.isProcessing;
            }
            set
            {
                this.isProcessing = value;
                this.IsEnableCapture = !value;
                this.RaisePropertyChanged("IsProcessing");
            }
        }

        private bool isEnableCapture = false;
        public bool IsEnableCapture
        {
            get
            {
                return this.isEnableCapture;
            }
            set
            {
                this.isEnableCapture = value;
                this.RaisePropertyChanged("IsEnableCapture");
            }
        }

        private WriteableBitmap imageSource;
        public WriteableBitmap ImageSource
        {
            get
            {
                return this.imageSource;
            }
            set
            {
                this.imageSource = value;
                this.RaisePropertyChanged("ImageSource");
            }
        }

        #region ボタンイベント
        private async void start()
        {
            await this.CaptureAsync();
        }
        private void stop()
        {
            this.isTriggered = true;
        }
        #endregion

        #region INotifyPropertyChanged実装
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler CanExecuteChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region OpenCV
        private async Task CaptureAsync()
        {
            this.IsProcessing = false;

            var data1 = new List<Mat>();
            var data2 = new List<Mat>();

            using (var capture = new VideoCapture(0))
            {
                capture.Fps = 30;
                capture.FrameWidth = 640;
                capture.FrameHeight = 480;
                while (true)
                {
                    var mat = await this.RetrieveMatAsync(capture);
                    if (mat == null) break;

                    if (!this.isTriggered)
                    {
                        data1.Add(mat.Clone());
                        if (data1.Count() > 90)
                        {
                            data1.RemoveAt(0);
                        }
                    }
                    else
                    {
                        data2.Add(mat.Clone());
                        if (data2.Count() >= 90)
                        {
                            break;
                        }
                    }
                    this.ImageSource = mat.ToWriteableBitmap();
                    Cv2.WaitKey(1);
                }
            }
            this.isTriggered = false;

            var now = System.DateTime.Now.ToFileTimeUtc();
            using (var writer = new VideoWriter($"test-{now}.wmv", FourCC.MP43, 30, new OpenCvSharp.Size(640, 480)))
            {
                foreach (var mat in data1.Concat(data2))
                {
                    writer.Write(mat);
                }
            }
            Cv2.DestroyAllWindows();

            this.IsProcessing = true;
        }

        private async Task<Mat> RetrieveMatAsync(VideoCapture videoCapture)
        {
            return await Task.Run(() =>
            {
                return videoCapture.RetrieveMat();
            });
        }
        #endregion

    }

}
