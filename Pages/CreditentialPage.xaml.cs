using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using VoxPopuli.Annotations;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace VoxPopuli.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class CreditentialPage : Page, INotifyPropertyChanged, IOptionPage
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler Validated;

		public CreditentialPage()
		{
			DataContext = this;
			this.InitializeComponent();
		}
		
		public string UserId { get; set; } = Options.Default.UserId;
		
		public string UserSession { get; set; } = Options.Default.UserSession;

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		private void Validate_OnClick(object sender, RoutedEventArgs e)
			=> Validated?.Invoke(this, EventArgs.Empty);

		public event Action DoBack;
	}
}
