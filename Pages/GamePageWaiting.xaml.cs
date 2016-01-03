using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Quobject.SocketIoClientDotNet.Client;
using VoxPopuli.Annotations;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace VoxPopuli.Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class GamePageWaiting : Page, IGamePage
	{
		public GamePageWaiting()
		{
			this.InitializeComponent();
		}
		
		public Game Game { get; set; }

		public async void Action(GameAction action, JsonObject json)
		{
			switch (action)
			{
				//case GameAction.RoomData:
				//	for (var i = 1; i <= 25; i++)
				//		Game.Players.Add(new Player(i.ToString(), "Player_" + i, 3, ""));
				//	break;
				case GameAction.AddPlayer:
					Game.AddPlayer(Helper.CreatePlayerFromJSON(json.GetNamedObject("player")));
					break;
				case GameAction.RemovePlayer:
					var playerID = json.GetNamedString("player");
					var player = Game.GetPlayerById(playerID);
					Game.RemovePlayer(player);
					break;
			}
		}
	}
}
