using AppGhiChuWinStore.Common;
using AppGhiChuWinStore.Helper;
using Facebook;
using Parse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

namespace AppGhiChuWinStore.View
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SyncPage : Page
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

        ParseUser userfb;
        public SyncPage()
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
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (!localSettings.Values.ContainsKey("username"))
            {
                try
                {
                    userfb = await ParseFacebookUtils.LogInAsync(browser, new[] { "user_about_me", "email" });
                    browser.Visibility = Visibility.Collapsed;
                    var usersPosts = await ParseObject.GetQuery("GhiChu").WhereEqualTo("user", userfb).FindAsync();
                    if (usersPosts.Count() > 0)
                    {
                        ccc.Text = "Tài khoản facebook của bạn có " + usersPosts.Count().ToString() + " ghi chú.";
                    }
                    else
                    {
                        ccc.Text = "Tài khoản facebook của bạn chưa có dữ liệu.";
                        btnDown.Visibility = Visibility.Collapsed;
                    }
                    var fb = new FacebookClient();
                    fb.AccessToken = ParseFacebookUtils.AccessToken;
                    dynamic me = await fb.GetTaskAsync("me");
                    ProfilePic.Source = new BitmapImage(new Uri("https://graph.facebook.com/" + me["id"] + "/picture"));
                    FBName.Text = me["name"];
                    localSettings.Values["name"] = me["name"];
                    localSettings.Values["username"] = userfb.Username;
                    localSettings.Values["image"] = "https://graph.facebook.com/" + me["id"] + "/picture";
                    Content.Visibility = Visibility.Visible;
                    ProgressRingGrid.Visibility = Visibility.Collapsed;
                }
                catch
                {

                }
            }
            else
            {
                var user = await ParseUser.Query.WhereEqualTo("username", localSettings.Values["username"].ToString()).FindAsync();
                userfb = user.First();
                var usersPosts = await ParseObject.GetQuery("GhiChu").WhereEqualTo("user", userfb).FindAsync();
                if (usersPosts.Count() > 0)
                {
                    ccc.Text = "Tài khoản facebook của bạn có " + usersPosts.Count().ToString() + " ghi chú.";
                }
                else
                {
                    ccc.Text = "Tài khoản facebook của bạn chưa có dữ liệu.";
                    btnDown.Visibility = Visibility.Collapsed;
                }
                FBName.Text = localSettings.Values["name"].ToString();
                ProfilePic.Source = new BitmapImage(new Uri(localSettings.Values["image"].ToString()));
                Content.Visibility = Visibility.Visible;
                ProgressRingGrid.Visibility = Visibility.Collapsed;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog msgDialog = new MessageDialog("Dữ liệu liên kết với facebook của bạn sẽ bị ghi đè. Bạn có muốn đồng bộ dữ liệu lên.", "Lưu ý");

            //OK Button
            UICommand okBtn = new UICommand("OK");
            okBtn.Invoked = OkBtnClick1;
            msgDialog.Commands.Add(okBtn);

            //Cancel Button
            UICommand cancelBtn = new UICommand("Cancel");
            cancelBtn.Invoked = CancelBtnClick1;
            msgDialog.Commands.Add(cancelBtn);

            //Show message
            msgDialog.ShowAsync();
        }

        private async void btnDown_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog msgDialog = new MessageDialog("Dữ liệu hiện tại của bạn sẽ bị ghi đè. Bạn có muốn đồng bộ dữ liệu xuống.", "Lưu ý");

            //OK Button
            UICommand okBtn = new UICommand("OK");
            okBtn.Invoked = OkBtnClick;
            msgDialog.Commands.Add(okBtn);

            //Cancel Button
            UICommand cancelBtn = new UICommand("Cancel");
            cancelBtn.Invoked = CancelBtnClick;
            msgDialog.Commands.Add(cancelBtn);

            //Show message
            msgDialog.ShowAsync();
        }
        private void CancelBtnClick(IUICommand command)
        {
            return;
        }

        private void OkBtnClick(IUICommand command)
        {
            Frame.Navigate(typeof(HubPage), userfb);
        }
        private void CancelBtnClick1(IUICommand command)
        {
            return;
        }

        private async void OkBtnClick1(IUICommand command)
        {
            ProgressRingGrid1.Visibility = Visibility.Visible;
            state.Visibility = Visibility.Visible;
            state.Text = "Đang đồng bộ...";
            backButton.Visibility = Visibility.Collapsed;
            try
            {
                var query = await ParseObject.GetQuery("GhiChu").Where(x => x["user"] == userfb).FindAsync();
                foreach (var gchu in query.ToList())
                {
                    await gchu.DeleteAsync();
                }
                DatabaseHelper help = new DatabaseHelper();
                var listghichu = help.ReadAllContacts();
                foreach (var gc in listghichu)
                {
                    ParseObject gameScore = new ParseObject("GhiChu");
                    gameScore["Title"] = gc.Title;
                    gameScore["Content"] = gc.Content;
                    gameScore["Time"] = gc.Time;
                    gameScore["Remind"] = gc.Remind;
                    gameScore["Complete"] = gc.Complete;
                    gameScore["user"] = userfb;
                    gameScore["Id"] = gc.Id;
                    await gameScore.SaveAsync();
                }
                var usersPosts = await ParseObject.GetQuery("GhiChu").WhereEqualTo("user", userfb).FindAsync();
                if (usersPosts.Count() > 0)
                {
                    ccc.Text = "Tài khoản facebook của bạn có " + usersPosts.Count().ToString() + " ghi chú.";
                    btnDown.Visibility = Visibility.Visible;
                }
                else
                {
                    ccc.Text = "Tài khoản facebook của bạn chưa có dữ liệu.";
                    btnDown.Visibility = Visibility.Collapsed;
                }
                state.Text = "Đồng bộ thành công.";
            }
            catch
            {
                state.Text = "Đồng bộ thất bại.";
            }
            ProgressRingGrid1.Visibility = Visibility.Collapsed;
            backButton.Visibility = Visibility.Visible;
        }
    }
}
