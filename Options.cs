using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.Data.Json;
using Windows.Storage;
using Quobject.SocketIoClientDotNet.Client;
using VoxPopuli.Annotations;

namespace VoxPopuli
{
	public class Options : INotifyPropertyChanged
	{
		public event EventHandler<string> RoomChanged;

		public static Options Default { get; }

		//public static StorageFile LoginFile { get; }

		private string _userId;// = "144625865";
		public string UserId
		{
			get { return _userId; }
			set
			{
				_userId = value;
				Save(nameof(UserId), UserId);
			}
		}

		private string _userSession;// = "27a39ca153848cd072e044e355560790";
		public string UserSession
		{
			get { return _userSession; }
			set
			{
				_userSession = value;
				Save(nameof(UserSession), UserSession);
			}
		}

		private bool _isConnected;
		public bool IsConnected
		{
			get { return _isConnected; }
			set
			{
				_isConnected = value;
				if (!IsConnected)
				{
					UserId = null;
					UserSession = null;
					RoomID = null;
				}
				OnPropertyChanged();
				Save(nameof(IsConnected), IsConnected.ToString());
			}
		}

		private string _roomID;
		public string RoomID
		{
			get { return _roomID; }
			set
			{
				if (value == _roomID)
					return;
				if (value != null && _roomID != null)
				{
					_roomID = null;
					OnRoomChanged(RoomID);
				}
				_roomID = value;
				OnRoomChanged(RoomID);
				OnPropertyChanged();
				OnPropertyChanged(nameof(GameHeader));
			}
		}

		private string Header
			=> (IsConnected ? $"user_id={UserId}&user_session={UserSession}&" : "") + "page=";
		public string HomeHeader
			=> Header + "index";
		public string GameHeader
			=> Header + $"game&room={RoomID}";

		public Socket HomeSocket { get; }
		public Socket GameSocket { get; private set; }

		public void RegenerateGameSocket()
		{
			//GameSocket?.Off();
			GameSocket?.Close();
			GameSocket = string.IsNullOrWhiteSpace(RoomID) ? null : (IO.Socket(SocketURL, new IO.Options() {ForceNew = true, QueryString = GameHeader}));
		}

		public void RegenerateGameSocket(string roomID)
		{
			RoomID = roomID;
			RegenerateGameSocket();
		}

		public static string HostName { get; } = "https://vox-populi.richie.fr/";
		public static string SocketURL { get; } = HostName + "lldpgn";
		public static string LoginURL { get; } = HostName + "login";
		public static string GetUserJsonURL { get; } = HostName + "getuserjson";

		public static ObservableCollection<string> Logs { get; } = new ObservableCollection<string>();

		public static void Save(string key, string value)
			=> ApplicationData.Current.LocalSettings.Values[key] = value;

		public static string Load(string key)
			=> ApplicationData.Current.LocalSettings.Values[key] as string;

		static Options()
		{
			Default = new Options();
		}

		public static ApplicationDataContainer Settings { get; }

		public Options()
		{
			//UserId = "4626812663";
			//UserSession = "50bcf2e6a1dfc3b4ae0e9e392a7e62a4";
			bool connected;
			if (bool.TryParse(Load(nameof(IsConnected)) ?? bool.FalseString, out connected) && connected)
			{
				UserId = Load(nameof(UserId)) ?? "";
				UserSession = Load(nameof(UserSession)) ?? "";
			}
			IsConnected = connected;
			
			HomeSocket = IO.Socket(SocketURL, new IO.Options() {ForceNew = true, QueryString = "page=index"});
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		protected virtual void OnRoomChanged(string room)
			=> RoomChanged?.Invoke(this, room);
	}
}