using AppGhiChuWinStore.Common;
using AppGhiChuWinStore.DataModel;
using AppGhiChuWinStore.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace AppGhiChuWinStore.View.Add
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class AddPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public AddPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            TimeBinding();
        }
        private void TimeBinding()
        {
            currentTime.Text = DateTime.Now.ToString();
            var timer = new DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Start();
        }

        void timer_Tick(object sender, object e)
        {
            currentTime.Text = DateTime.Now.ToString();
        }
        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
        private async void Image_Tapped1(object sender, TappedRoutedEventArgs e)
        {
            MessageDialogHelper ms = new MessageDialogHelper();
            ms.Show("Bạn có muốn thoát chương trình", "");
        }
        private async void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (tbxTitle.Text == "")
            {
                MessageDialog ms = new MessageDialog("Vui lòng nhập tiêu đề!");
                await ms.ShowAsync();
                return;
            }
            if (tbxContent.Text == "")
            {
                MessageDialog ms = new MessageDialog("Vui lòng nhập nội dung!");
                await ms.ShowAsync();
                return;
            }
            var t = time.Time;
            var d = String.Format("{0:d}", date.Date);
            var k = DateTime.ParseExact(d + " " + t, "M/d/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None);
            if (k <= DateTime.Now)
            {
                MessageDialog ms = new MessageDialog("Hạn chót phải sau thời điểm hiện tại!");
                await ms.ShowAsync();
                return;
            }
            GhiChu u = new GhiChu();
            u.Title = tbxTitle.Text;
            u.Content = tbxContent.Text;
            u.Time = k.ToString();
            u.Remind = (bool)cbNhacNho.IsChecked;
            u.Complete = false;
            DatabaseHelper help = new DatabaseHelper();
            help.Insert(u);
            if ((bool)cbNhacNho.IsChecked)
            {
                ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText01;
                XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
                XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
                ((XmlElement)toastImageAttributes[0]).SetAttribute("src", "/Assets/noteicon.png");
                ((XmlElement)toastImageAttributes[0]).SetAttribute("alt", "alt text");
                XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                toastTextElements[0].AppendChild(toastXml.CreateTextNode(u.Title));
                var dueTime = new DateTimeOffset(k);
                var toast = new Windows.UI.Notifications.ScheduledToastNotification(toastXml, dueTime);
                toast.Id = "T.note" + u.Id.ToString();
                Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier().AddToSchedule(toast);
            }
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
        private void btnLuu_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            btnLuu.Source = new BitmapImage(new Uri("ms-appx:/Assets/Save1.png"));
        }

        private void btnLuu_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            btnLuu.Source = new BitmapImage(new Uri("ms-appx:/Assets/Save.png"));
        }

    }
}
