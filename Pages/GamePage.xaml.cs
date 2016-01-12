#if DEBUG
	//#define UDP
#endif
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using VoxPopuli.Annotations;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace VoxPopuli.Pages
{
	public enum GameAction
	{
		RoomData,
		AddPlayer,
		RemovePlayer,
		NewQuestion,
		AlertDisconnect,
		GainLife,
		LooseLife,
		HasVoted,
		ShowVotes,
		RemoveTimer,
		UpdateTimer,
		UpdateBeforeDelete,
		DeleteRoom,
		EndGame,
		AlertSpect
	}

	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class GamePage : Page, INotifyPropertyChanged
	{
		//private readonly Socket _socket;
		public event PropertyChangedEventHandler PropertyChanged;
		
		private IGamePage _page;
		public IGamePage Page
		{
			get { return _page; }
			set
			{
				if (Equals(value, _page)) return;
				_page = value;
				OnPropertyChanged();
			}
		}

		private PageRule _rules;
		public PageRule Rules
		{
			get { return _rules; }
			set
			{
				if (Equals(value, _rules)) return;
				_rules = value;
				OnPropertyChanged();
			}
		}

		private Game _game;
#if UDP
		private readonly UdpClient _udpClient;
#endif

		public Game Game
		{
			get { return _game; }
			set
			{
				if (Equals(value, _game)) return;
				_game = value;
				OnPropertyChanged();
			}
		}

		public GamePage()
		{
			//_socket = IO.Socket(Options.SocketURL, new IO.Options() { ForceNew = true, QueryString = Options.Default.GameHeader });
			Options.Default.GameSocket.On("gameEvent", obj =>
			{
				var json = JsonObject.Parse((obj as JObject)?.ToString() ?? "{}");
				if (json == null)
					return;

				this.RunDispatcher(() => Options.Logs.Add(obj.ToString()));
				try
				{
					this.RunDispatcher(() => OnGameEvent(json));
				}
				catch (Exception e)
				{
					Options.Logs.Add(e.ToString());
				}
			});


#if UDP
			_udpClient = new UdpClient(new IpAddress("box.faustvx.fr", 80));
			Task.Run(async () =>
			{
				await _udpClient.SendAsync(Encoding.Unicode.GetBytes("connect"));
				while (true)
				{
					Tuple<IpAddress, byte[]> response;
					if (_udpClient.TryGetIncomingMessage(out response))
					{
						var json = Encoding.Unicode.GetString(response.Item2, 0, response.Item2.Length);
						var jsonObject = JsonObject.Parse(json);
						this.RunDispatcher(() => OnGameEvent(jsonObject));
					}
					else
						await Task.Delay(250);
				}
			});
#endif


			DataContext = this;
			this.InitializeComponent();
		}

		public async void OnGameEvent(JsonObject json)
		{
			var jsonAction = json.GetNamedString("action");
			switch (jsonAction)
			{
				case "roomData":
					{
						var roomData = json.GetNamedObject("roomData");
						if (roomData.GetNamedString("status") != "waiting")
							break;

						var mode = roomData.GetNamedObject("gamemode");
						var gameMode = new GameMode(mode.GetNamedString("name"),
							mode.GetNamedBoolean("loosersCanVote"),
							mode.GetNamedBoolean("loosersCanVoteFinal"),
							(int)mode.GetNamedNumber("nbLiveStart"),
							(int)mode.GetNamedNumber("nbSafeQuestions"),
							(int)mode.GetNamedNumber("nbSecToVote"),
							mode.GetNamedBoolean("noQuestions"),
							(int) mode.GetNamedNumber("gainLifeWhenSafe"),
							roomData.GetNamedBoolean("isDebug"));

						var players = roomData.GetNamedObject("players");
						var playersId = players.Keys;
						var playersEnum = playersId.Select(playerId => Helper.CreatePlayerFromJSON(players.GetNamedObject(playerId)));

						var game = new Game(roomData.GetNamedString("room_id"),
							gameMode,
							(int)roomData.GetNamedNumber("minPlayers"),
							(int)roomData.GetNamedNumber("maxPlayers"),
							playersEnum);

						Game = game;
						(Page = new GamePageWaiting()).Game = Game;
						Rules = new PageRule() {GameMode = Game.GameMode};
						break;
					}
				case "newQuestion":
					{
						if (!(Page is GamePageVoting))
							(Page = new GamePageVoting()).Game = Game;
						break;
					}
				case "alertDisconnect":
				{
					Options.Default.RoomID = null;
						await new MessageDialog("Vous êtes connecté depuis un autre navigateur", "Déconnexion").ShowAsync();
						break;
					}
				case "updateTimer":
					Game.Timer = ((int)json.GetNamedNumber("newValue")) / 1000;
					break;
				case "updateBeforeDelete":
					Game.Timer = (int)json.GetNamedNumber("val");
					break;
				case "deleteRoom":
					await new MessageDialog("La partie vient de se terminer", "Partie Terminée").ShowAsync();
					Options.Default.RoomID = null;
					//CloseSocket();
					break;
				case "endGame":
					var winners = json.GetNamedArray("winners");
					await new MessageDialog($"Les gagnants sont: {string.Join(", ", winners.Select(name => name.GetString()))}", "Gagnants").ShowAsync();
					Options.Default.RoomID = null;
					break;
				case "alertSpect":
					await new MessageDialog(json.GetNamedString("message"), "Non Connecté").ShowAsync();
					break;
				default:
					break;
			}

			GameAction action;
			if (GameAction.TryParse(jsonAction, true, out action))
				Page?.Action(action, json);
			else 
				Options.Logs.Add(jsonAction +" n'est pas reconnu");
		}

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public void CloseSocket()
		{
#if UDP
			_udpClient?.Dispose();
#endif
			Options.Default.GameSocket.Off();
			Options.Default.GameSocket.Close();
		}
	}
}
