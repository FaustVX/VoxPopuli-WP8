using System;
using System.Linq;
using Windows.Data.Json;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace VoxPopuli.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class GamePageVoting : Page, IGamePage
	{
		public Game Game { get; set; }

		public GamePageVoting()
		{
			DataContext = this;
			this.InitializeComponent();
		}

		private void VoteButton_OnChecked(object sender, RoutedEventArgs e)
		{
			var btn = sender as ToggleButton;
			int id;

			if (!(btn?.Tag is string) || (!Game.Player.CanVote || Game.Player.SelectedAnswer != null || Game.CurrentQuestion == null) || !int.TryParse((string) btn.Tag, out id))
			{
				if (btn != null)
					btn.IsChecked = false;
				return;
			}

			var jsonObject = new JsonObject();
			jsonObject.SetNamedValue("action", JsonValue.CreateStringValue("vote"));
			jsonObject.SetNamedValue("voteid", JsonValue.CreateNumberValue(id));
			Options.Default.GameSocket.Emit("clientEvent", JObject.Parse(jsonObject.Stringify()));
		}

		public void Action(GameAction action, JsonObject json)
		{
			switch (action)
			{
				case GameAction.NewQuestion:
				{
					foreach (var player in Game.Players)
					{
						player.HasVoted = false;
						player.SelectedAnswer = null;
					}

					var jQuestion = json.GetNamedObject("question");
					var jAnswer = jQuestion.GetNamedArray("answers");
					var text = jQuestion.GetNamedString("content");
					var i = 0;
					Game.CurrentQuestion = new Question(text, jAnswer[i++].GetArray()[1].GetString(), jAnswer[i++].GetArray()[1].GetString(), jAnswer[i++].GetArray()[1].GetString());

					var roomData = json.GetNamedObject("roomData");
					Game.AlivePlayers = (int) roomData.GetNamedNumber("nbAlivePlayers");
					Game.Timer = ((int) roomData.GetNamedNumber("subTimer")) / 1000;
					Game.Player.SelectedAnswer = null;
					break;
				}
				case GameAction.UpdateTimer:
					Game.AlivePlayers = (int) json.GetNamedNumber("nbAlivePlayers");
					break;
				case GameAction.GainLife:
				case GameAction.LooseLife:
					//if (action == GameAction.LooseLife)
					//	new MessageDialog("Vous n'avez pas choisi la réponse majoritaire, vous perdez une vie.").ShowAsync();
					Game.Player.Life = (int)json.GetNamedNumber("newPoints");
					break;
				case GameAction.HasVoted:
				{
					var id = json.GetNamedString("player");
					var player = Game.GetPlayerById(id);
					player.HasVoted = true;
					break;
				}
				case GameAction.ShowVotes:
					{
						var votes = json.GetNamedArray("votes");
						var i = 0;
						var currentQuestion = Game.CurrentQuestion;
						currentQuestion.ResponseA.Vote = (int)votes[i++].GetNumber();
						currentQuestion.ResponseB.Vote = (int)votes[i++].GetNumber();
						currentQuestion.ResponseC.Vote = (int)votes[i++].GetNumber();

						var deadPlayers = json.GetNamedArray("deadPlayers");
						foreach (var player in deadPlayers.Select(id => id.GetString()).Select(Game.GetPlayerById))
							player.IsEliminated = true;

						var majoriries = json.GetNamedArray("majs");
						foreach (var maj in majoriries.Select(maj => maj.GetString()))
						{
							switch (maj)
							{
								case "0":
									currentQuestion.ResponseA.Majority = true;
									break;
								case "1":
									currentQuestion.ResponseB.Majority = true;
									break;
								case "2":
									currentQuestion.ResponseC.Majority = true;
									break;
								default:
									throw new ArgumentOutOfRangeException();
							}
						}

						var voteNamed = json.GetNamedObject("votesNamed");
						foreach (var id in voteNamed)
							Game.GetPlayerById(id.Key).SelectedAnswer = (Answer)(int)id.Value.GetNumber();

						currentQuestion.ShowVote = true;
						break;
					}
			}
		}
	}
}
