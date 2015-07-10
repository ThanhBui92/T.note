using AppGhiChuWinStore.Common;
using AppGhiChuWinStore.Data;
using AppGhiChuWinStore.DataModel;
using AppGhiChuWinStore.Helper;
using AppGhiChuWinStore.View;
using AppGhiChuWinStore.View.Add;
using Parse;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Hub Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=321224

namespace AppGhiChuWinStore
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        public HubPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper.LoadState += navigationHelper_LoadState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            DatabaseHelper help = new DatabaseHelper();
            if (Frame.BackStack.Count > 0)
            {
                if (Frame.BackStack.Last().SourcePageType == typeof(SyncPage))
                {
                    ProgressRingGrid.Visibility = Visibility.Visible;
                    Content.Visibility = Visibility.Collapsed;
                    foreach (var gc in help.ReadAllContacts().ToList())
                    {
                        IReadOnlyList<ScheduledToastNotification> scheduled = ToastNotificationManager.CreateToastNotifier().GetScheduledToastNotifications();

                        foreach (ScheduledToastNotification notify in scheduled)
                        {
                            if (notify.Id == ("T.note" + gc.Id))
                            {
                                ToastNotificationManager.CreateToastNotifier().RemoveFromSchedule(notify);
                            }
                        }
                    }
                    help.DeleteAllContact();
                    Frame.BackStack.Remove(Frame.BackStack.Where(x => x.SourcePageType == typeof(SyncPage)).FirstOrDefault());
                    var query = await ParseObject.GetQuery("GhiChu").Where(x => x["user"] == ((ParseUser)e.NavigationParameter)).FindAsync();
                    foreach (var ghichu in query.OrderBy(x => x["Id"]).ToList())
                    {
                        var gc = new GhiChu(ghichu.Get<string>("Title"), ghichu.Get<string>("Content"), ghichu.Get<string>("Time"), ghichu.Get<bool>("Remind"), ghichu.Get<bool>("Complete"));
                        help.Insert(gc);
                        if (DateTime.ParseExact(gc.Time, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None) > DateTime.Now && gc.Remind == true)
                        {
                            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText01;
                            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
                            XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
                            ((XmlElement)toastImageAttributes[0]).SetAttribute("src", "/Assets/noteicon.png");
                            ((XmlElement)toastImageAttributes[0]).SetAttribute("alt", "alt text");
                            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                            toastTextElements[0].AppendChild(toastXml.CreateTextNode(gc.Title));
                            var dueTime = new DateTimeOffset(DateTime.ParseExact(gc.Time, "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None));
                            var toast = new Windows.UI.Notifications.ScheduledToastNotification(toastXml, dueTime);
                            toast.Id = "T.note" + gc.Id.ToString();
                            Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier().AddToSchedule(toast);
                        }
                    }
                    Content.Visibility = Visibility.Visible;
                    ProgressRingGrid.Visibility = Visibility.Collapsed;
                }
            }
            itemGridView.ItemsSource = help.ReadContacts();
            itemGridView1.ItemsSource = help.ReadContactsComplete();
        }

        /// <summary>
        /// Invoked when a HubSection header is clicked.
        /// </summary>
        /// <param name="sender">The Hub that contains the HubSection whose header was clicked.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            HubSection section = e.Section;
            var group = section.DataContext;
            this.Frame.Navigate(typeof(SectionPage), ((SampleDataGroup)group).UniqueId);
        }

        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        /// <param name="sender">The GridView or ListView
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((GhiChu)e.ClickedItem).Id;
            this.Frame.Navigate(typeof(DetailPage), itemId);
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

        private void btnLuu_PointerEntered2(object sender, PointerRoutedEventArgs e)
        {
            btnPre.Source = new BitmapImage(new Uri("ms-appx:/Assets/Arrowhead-Left1.png"));
        }

        private void btnLuu_PointerExited2(object sender, PointerRoutedEventArgs e)
        {
            btnPre.Source = new BitmapImage(new Uri("ms-appx:/Assets/Arrowhead-Left.png"));
        }
        private void btnLuu_PointerEntered3(object sender, PointerRoutedEventArgs e)
        {
            btnNext.Source = new BitmapImage(new Uri("ms-appx:/Assets/Arrowhead-Right1.png"));
        }

        private void btnLuu_PointerExited3(object sender, PointerRoutedEventArgs e)
        {
            btnNext.Source = new BitmapImage(new Uri("ms-appx:/Assets/Arrowhead-Right.png"));
        }
        private async void Image_Tapped2(object sender, TappedRoutedEventArgs e)
        {
            tbxTab.Text = "Ghi chú";
            flip.SelectedIndex = 0;
            btnNext.Visibility = Visibility.Visible;
            btnPre.Visibility = Visibility.Collapsed;
            btnClose.Visibility = Visibility.Visible;
        }
        private async void Image_Tapped3(object sender, TappedRoutedEventArgs e)
        {
            tbxTab.Text = "Lịch sử";
            flip.SelectedIndex = 1;
            btnPre.Visibility = Visibility.Visible;
            btnNext.Visibility = Visibility.Collapsed;
            btnClose.Visibility = Visibility.Collapsed;
        }
        private async void Image_Tapped4(object sender, TappedRoutedEventArgs e)
        {
        }
        private void flip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (flip == null) return;
            if (flip.SelectedIndex == 0)
            {
                tbxTab.Text = "Ghi chú";
                btnNext.Visibility = Visibility.Visible;
                btnPre.Visibility = Visibility.Collapsed;
                GridAdd.Visibility = Visibility.Visible;
            }
            else
            {
                tbxTab.Text = "Lịch sử";
                btnPre.Visibility = Visibility.Visible;
                btnNext.Visibility = Visibility.Collapsed;
                GridAdd.Visibility = Visibility.Collapsed;
            }
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddPage));
        }

        private void Grid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            btnClose.Source = new BitmapImage(new Uri("ms-appx:/Assets/Add-New1.png"));
            tbkAdd.Foreground = new SolidColorBrush(Color.FromArgb(255, 204, 204, 204));
        }

        private void Grid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            btnClose.Source = new BitmapImage(new Uri("ms-appx:/Assets/Add-New.png"));
            tbkAdd.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        }
        private void Grid_PointerEntered_1(object sender, PointerRoutedEventArgs e)
        {
            btnSync.Source = new BitmapImage(new Uri("ms-appx:/Assets/Data-Synchronize1.png"));
            tbkSync.Foreground = new SolidColorBrush(Color.FromArgb(255, 204, 204, 204));
        }

        private void Grid_PointerExited_1(object sender, PointerRoutedEventArgs e)
        {
            btnSync.Source = new BitmapImage(new Uri("ms-appx:/Assets/Data-Synchronize.png"));
            tbkSync.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        }

        private async void Grid_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            //var m = Windows.Security.Authentication.Web.WebAuthenticationBroker.GetCurrentApplicationCallbackUri().AbsoluteUri;
            //try
            //{
            //    ParseUser userfb = await ParseFacebookUtils.LogInAsync(new[] { "user_about_me", "email" });
            //}
            //catch { }
            this.Frame.Navigate(typeof(SyncPage));
            //FacebookClient _fb = new FacebookClient();
            //var loginUrl = _fb.GetLoginUrl(new
            //{
            //    client_id = "1594349534150866",
            //    redirect_uri = Windows.Security.Authentication.Web.WebAuthenticationBroker.GetCurrentApplicationCallbackUri().AbsoluteUri,
            //    scope = "user_about_me",
            //    display = "popup",
            //    response_type = "token"
            //});

            //WebAuthenticationResult WebAuthenticationResult = await WebAuthenticationBroker.AuthenticateAsync(
            //      WebAuthenticationOptions.None,
            //      loginUrl);

            //if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.Success)
            //{
            //    var callbackUri = new Uri(WebAuthenticationResult.ResponseData.ToString());
            //    var facebookOAuthResult = _fb.ParseOAuthCallbackUrl(callbackUri);

            //    // Retrieve the Access Token. You can now interact with Facebook on behalf of the user
            //    // using the Access Token.
            //    var accessToken = facebookOAuthResult.AccessToken;
            //}
            //else if (WebAuthenticationResult.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            //{
            //    // handle authentication failure
            //}
            //else
            //{
            //    // The user canceled the authentication
            //}
        }
        private void Grid_PointerEntered_2(object sender, PointerRoutedEventArgs e)
        {
            btnGT.Source = new BitmapImage(new Uri("ms-appx:/Assets/Business-Card-011.png"));
            tbkGT.Foreground = new SolidColorBrush(Color.FromArgb(255, 204, 204, 204));
        }

        private void Grid_PointerExited_2(object sender, PointerRoutedEventArgs e)
        {
            btnGT.Source = new BitmapImage(new Uri("ms-appx:/Assets/Business-Card-01.png"));
            tbkGT.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        }

        private async void Grid_Tapped_2(object sender, TappedRoutedEventArgs e)
        {
            //var m = Windows.Security.Authentication.Web.WebAuthenticationBroker.GetCurrentApplicationCallbackUri().AbsoluteUri;
            //try
            //{
            //    ParseUser userfb = await ParseFacebookUtils.LogInAsync(new[] { "user_about_me", "email" });
            //}
            //catch { }
            this.Frame.Navigate(typeof(GioiThieuPage));
        }
    }
}
