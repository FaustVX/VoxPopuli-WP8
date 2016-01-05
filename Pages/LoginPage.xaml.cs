using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace VoxPopuli.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LoginPage : Page, IOptionPage
	{
		public event Action DoBack;
		public event Action Connected;

		public LoginPage()
		{
			DataContext = this;
			this.InitializeComponent();

		}

		public string URL { get; } = Options.LoginURL;

		private static async void OnCompleted(WebView webView, Uri uri)
		{
			if (uri.ToString() == Options.HostName)
				webView.Source = new Uri(Options.GetUserJsonURL);
			else if (uri.ToString() == Options.GetUserJsonURL)
			{
				var json = await webView.InvokeScriptAsync("eval", new[] {"document.getElementsByTagName('body')[0].innerHTML"});
				var jUser = JsonObject.Parse(json);
				Options.Default.UserId = jUser.GetNamedString("user_id");
				Options.Default.UserSession = jUser.GetNamedString("user_session");
				Options.Default.IsConnected = true;
				Options.Default.RoomID = null;
				(webView.Parent as LoginPage)?.Connected?.Invoke();
				//await new MessageDialog("Appuyez sur 'Back' ", "Connecté").ShowAsync();
			}
		}

		private void WebView_OnDOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
		{
			OnCompleted(sender, args.Uri);
		}
	}
}
