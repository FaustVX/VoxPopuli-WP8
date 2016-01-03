using System;
using Windows.Data.Json;
using Quobject.SocketIoClientDotNet.Client;

namespace VoxPopuli.Pages
{
	public interface IOptionPage
	{
		event Action DoBack;
	}

	public interface IGamePage
	{
		Game Game { get; set; }
		void Action(GameAction action, JsonObject json);
	}
}