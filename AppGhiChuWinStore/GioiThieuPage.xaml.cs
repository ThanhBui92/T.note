using AppGhiChuWinStore.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace AppGhiChuWinStore
{
    public class GioiThieu
    {
        public GioiThieu(string image, string title, string link)
        {
            Image = image;
            Title = title;
            Link = link;
        }
        public string Image { get; set; }
        public string Link { get; set; }
        public string Title { get; set; }
    }
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class GioiThieuPage : Page
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


        public GioiThieuPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
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
            List<GioiThieu> lh = new List<GioiThieu>();
            lh.Add(new GioiThieu("/Assets/Item-53e88f6f-42e4-4d2c-ab41-a8d568894434.png", "SHTP-TRAINING", ""));
            lh.Add(new GioiThieu("/Assets/location.png", "Địa chỉ: Lô E1 – Khu Công nghệ cao, Xa lộ Hà Nội, Phường Hiệp Phú, Quận 9, TP.HCM", "bingmaps:?cp=10.855064~106.785569&lvl=17"));
            lh.Add(new GioiThieu("/Assets/phoneicon.png", "Điện thoại: (84-8) 39.322.230 (17) ", "tel:(84-8) 39.322.230 (17)"));
            lh.Add(new GioiThieu("/Assets/Reminder-Window.png", "Giúp người dùng lưu trữ các ghi chú và nhắc lịch.", ""));
            lh.Add(new GioiThieu("/Assets/Download.png", "Phiên bản: 1.0.0", ""));
            AboutUs.ItemsSource = lh;
        }
        private async void AboutUs_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (((GioiThieu)e.ClickedItem).Link != "")
            {
                await Launcher.LaunchUriAsync(new Uri(((GioiThieu)e.ClickedItem).Link, UriKind.Absolute));
            }
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
    }
}
