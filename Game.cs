using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VoxPopuli.Annotations;

namespace VoxPopuli
{
	public class Game : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public ObservableCollection<Player> Players { get; }
		public string ID { get; }
		public GameMode GameMode { get; }
		public int MinPlayers { get; }
		public int MaxPlayers { get; }
		public Player Player { get; }
		public bool CanJoin
			=> Player != null && !Player.IsEliminated;

		private Question _currentQuestion;
		public Question CurrentQuestion
		{
			get { return _currentQuestion; }
			set
			{
				if (Equals(value, _currentQuestion)) return;
				_currentQuestion = value;
				OnPropertyChanged();
			}
		}

		private int _timer;
		public int Timer
		{
			get { return _timer; }
			set
			{
				if (value == _timer) return;
				_timer = value;
				OnPropertyChanged();
			}
		}

		private int _alivePlayers;
		public int AlivePlayers
		{
			get { return _alivePlayers; }
			set
			{
				if (value == _alivePlayers) return;
				_alivePlayers = value;
				OnPropertyChanged();
			}
		}

		public Game(string id, GameMode gameMode, int minPlayers, int maxPlayers, IEnumerable<Player> players)
		{
			ID = id;
			GameMode = gameMode;
			MinPlayers = minPlayers;
			MaxPlayers = maxPlayers;
			Players = new ObservableCollection<Player>(players);
			Player = GetPlayerById(Options.Default.UserId);
			Timer = 0;
		}

		public Player GetPlayerById(string id)
			=> Players.FirstOrDefault(player => player.ID == id);

		public void AddPlayer(Player player)
		{
			Players.Add(player);
		}

		public void RemovePlayer(Player player)
		{
			Players.Remove(player);
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}