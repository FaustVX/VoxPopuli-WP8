#if DEBUG
	//#define UDP
#endif
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace VoxPopuli.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class HomePage : Page
	{
		public ObservableCollection<Game> Waitings { get; }
		public ObservableCollection<Game> Playings { get; }
		//private readonly Socket _socket;
#if UDP
		private readonly UdpClient _udpClient;
		private bool _isRunning;
#endif


		public HomePage()
		{
			Options.Default.RoomID = null;
			Waitings = new ObservableCollection<Game>();
			Playings = new ObservableCollection<Game>();

#if UDP
			_udpClient = new UdpClient(new IpAddress("box.faustvx.fr", 80));
			Task.Run(async () =>
			{
				_isRunning = true;
				await _udpClient.SendAsync(Encoding.Unicode.GetBytes("connect"));
				while (_isRunning)
				{
					Tuple<IpAddress, byte[]> response;
					if (_udpClient.TryGetIncomingMessage(out response))
					{
						var json = Encoding.Unicode.GetString(response.Item2, 0, response.Item2.Length);
						this.RunDispatcher(() => OnTickList(JsonObject.Parse(json)));
						_isRunning = false;
					}
					else
						await Task.Delay(250);
				}
			});
#endif


			//_socket = IO.Socket(Options.SocketURL, new IO.Options() { ForceNew = true, QueryString = Options.Default.HomeHeader });
			Options.Default.HomeSocket.On("tickList", obj =>
			{
				var json = JsonObject.Parse((obj as JObject)?.ToString() ?? "{}");
				if (json == null)
					return;

				//Options.Logs.Add(obj.ToString());
				//Debug.WriteLine(obj.ToString());

				this.RunDispatcher(() => OnTickList(json));
			});

			DataContext = this;
			this.InitializeComponent();
		}

		private void OnTickList(JsonObject json)
		{
			Waitings.Clear();
			Playings.Clear();

			var waitings = json.GetNamedArray("waiting");
			foreach (var game in waitings.Select(waiting => waiting.GetObject()))
				Waitings.Add(CreateGame(game));

			var playings = json.GetNamedArray("playing");
			foreach (var game in playings.Select(waiting => waiting.GetObject()).Select(CreateGame))//.Where(game => game.GetPlayerById(Options.Default.UserId) != null))
				Playings.Add(game);
		}

		private static Game CreateGame(JsonObject game1)
		{
			var mode = game1.GetNamedObject("gamemode");
			var gameMode = new GameMode(mode.GetNamedString("name"),
				mode.GetNamedBoolean("loosersCanVote"),
				mode.GetNamedBoolean("loosersCanVoteFinal"),
				(int) mode.GetNamedNumber("nbLiveStart"),
				(int) mode.GetNamedNumber("nbSafeQuestions"),
				(int) mode.GetNamedNumber("nbSecToVote"),
				mode.GetNamedBoolean("noQuestions"),
				(int) mode.GetNamedNumber("gainLifeWhenSafe"),
				false);
			var players = game1.GetNamedObject("players");
			var playersId = players.Keys;
			var playersEnum = playersId.Select(players.GetNamedObject).Select(Helper.CreatePlayerFromJSON);
			var game = new Game(game1.GetNamedString("room_id"), gameMode, (int)game1.GetNamedNumber("minPlayers"), (int)game1.GetNamedNumber("maxPlayers"), playersEnum) {AlivePlayers = (int) game1.GetNamedNumber("nbAlivePlayers")};
			return game;
		}

		private void Connect_OnClick(object sender, RoutedEventArgs e)
		{
			var game = (sender as FrameworkElement)?.Tag as Game;
			if (game == null)
				return;
#if UDP
			_isRunning = false;
			_udpClient?.Dispose();
#endif

			Options.Default.RoomID = game.ID;
		}

		public void CloseSocket()
		{
			Options.Default.HomeSocket.Off();
			Options.Default.HomeSocket.Close();
		}
	}
}
