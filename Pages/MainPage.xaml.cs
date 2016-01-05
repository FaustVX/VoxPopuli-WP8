using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using VoxPopuli.Annotations;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace VoxPopuli.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
	    public event PropertyChangedEventHandler PropertyChanged;

		public MainPage()
        {
			_logsPage = new LogsPage();
			NoGamePage = new NoGamePage();
			LogsPage.DoBack += () => OptionPage = null;
#if DEBUG
			ShowLog = true;
#endif

			Options.Default.RoomChanged += (sender, roomId) =>
			{
				Options.Default.RegenerateGameSocket();
				GamePage = string.IsNullOrWhiteSpace(roomId) ? null : new GamePage();
			};

			if (!Options.IsConnected)
			{
				var loginPage = new LoginPage();
				loginPage.Connected += () => OptionPage = null;
				OptionPage = loginPage;
			}

	        DataContext = this;
	        this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
		}

	    private GamePage _gamePage;
		public GamePage GamePage
	    {
		    get { return _gamePage; }
		    private set
		    {
			    if (Equals(value, _gamePage)) return;
			    GamePage?.CloseSocket();
			    _gamePage = value;
			    OnPropertyChanged();
				OnPropertyChanged(nameof(ShowGamePage));
		    }
	    }

	    public NoGamePage NoGamePage { get; }

	    private LogsPage _logsPage;
		public LogsPage LogsPage
	    {
		    get { return _logsPage; }
		    private set
		    {
			    if (Equals(value, _logsPage)) return;
			    _logsPage = value;
			    OnPropertyChanged();
		    }
	    }

	    private IOptionPage _optionPage;
		public IOptionPage OptionPage
	    {
		    get { return _optionPage; }
		    private set
		    {
			    if (Equals(value, _optionPage)) return;
			    _optionPage = value;
			    OnPropertyChanged();
			    OnPropertyChanged(nameof(ShowOptionPage));
		    }
	    }

	    public bool ShowOptionPage
			=> OptionPage != null;

		public bool ShowGamePage
			=> GamePage != null;

	    public bool ShowLog { get; } = false;

	    public Options Options { get; } = Options.Default;

	    /// <summary>
	    /// Invoked when this page is about to be displayed in a Frame.
	    /// </summary>
	    /// <param name="e">Event data that describes how this page was reached.
	    /// This parameter is typically used to configure the page.</param>
	    protected override void OnNavigatedTo(NavigationEventArgs e)
        {
			// TODO: Prepare page for display here.

			// TODO: If your application contains multiple pages, ensure that you are
			// handling the hardware Back button by registering for the
			// Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
			// If you are using the NavigationHelper provided by some templates,
			// this event is handled for you.
		    Windows.Phone.UI.Input.HardwareButtons.BackPressed += (sender, args) =>
		    {
			    args.Handled = true;
			    if (ShowOptionPage)
				    OptionPage = null;
			    else if (ShowGamePage)
				    Options.Default.RoomID = null;
			    else
				    Application.Current.Exit();
		    };

        }

	    [NotifyPropertyChangedInvocator]
	    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	    private void Login_OnClick(object sender, RoutedEventArgs e)
	    {
			var loginPage = new LoginPage();
		    loginPage.Connected += () => OptionPage = null;
			OptionPage = loginPage;
	    }

	    private void Logout_OnClick(object sender, RoutedEventArgs e)
	    {
		    Options.Default.IsConnected = false;
	    }

	    private void Logs_OnClick(object sender, RoutedEventArgs e)
	    {
		    OptionPage = LogsPage;
	    }
    }
}
