using System;
using Windows.Data.Json;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace VoxPopuli
{
	public static class Helper
	{
		public static void RunDispatcher(this DependencyObject obj, DispatchedHandler action)
			=> obj.Dispatcher.RunAsync(CoreDispatcherPriority.High, action).AsTask().Wait();

		public static Player CreatePlayerFromJSON(JsonObject player)
		{
			JsonObject stats = null;
			try
			{
				stats = player.GetNamedObject("stats");
			}
			catch
			{ }

			return new Player(player.GetNamedString("user_id"), player.GetNamedString("screen_name"), (int?)stats?.GetNamedNumber("nbWins") ?? 0, player.GetNamedString("avatar_url"), (int)player.GetNamedNumber("nbLives")) {HasVoted = player.GetNamedBoolean("hasVoted") };
		}
	}
}