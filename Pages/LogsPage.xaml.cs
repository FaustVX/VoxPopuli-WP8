using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using VoxPopuli.Annotations;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace VoxPopuli.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LogsPage : Page, INotifyPropertyChanged, IOptionPage
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public event EventHandler OnBack;

		public LogsPage()
		{
			//DataContext = Options.Logs.Reverse<string>();
			DataContext = this;
			this.InitializeComponent();
		}

		private ObservableCollection<string> _logs = Options.Logs;

		public ObservableCollection<string> Logs
		{
			get { return _logs; }
			private set
			{
				if (Equals(value, _logs)) return;
				_logs = value;
				OnPropertyChanged();
			}
		}

		private void Back_OnClick(object sender, RoutedEventArgs e)
			=> OnBack?.Invoke(this, EventArgs.Empty);

		private void Clear_OnClick(object sender, RoutedEventArgs e)
		{
			Options.Logs.Clear();
		}

		private void Delete_OnClick(object sender, RoutedEventArgs e)
		{
			var log = (sender as FrameworkElement)?.Tag as string;
			if (log == null)
				return;
			Options.Logs.Remove(log);
		}
		
		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		//private void Refresh_OnClick(object sender, RoutedEventArgs e)
		//{
		//	Logs = null;
		//	Logs = Options.Logs;
		//}
		public event Action DoBack;
	}
}
