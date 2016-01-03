using System.ComponentModel;
using System.Runtime.CompilerServices;
using VoxPopuli.Annotations;

namespace VoxPopuli
{
	public class Player : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public string ID { get; }
		public string Name { get; }
		public int WinnedGames { get; }
		public string AvatarURL { get; }

		public bool CanVote
			=> !HasVoted && !IsEliminated;

		private int _life;
		public int Life
		{
			get { return _life; }
			set
			{
				if (value == _life) return;
				_life = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(StringLife));
			}
		}

		public string StringLife
			=> new string(LifeChar, Life);
		public char LifeChar { get; } = '❣';

		private bool _isEliminated;
		public bool IsEliminated
		{
			get { return _isEliminated; }
			set
			{
				if (value == _isEliminated) return;
				_isEliminated = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(CanVote));
			}
		}

		private bool _hasVoted;
		public bool HasVoted
		{
			get { return _hasVoted; }
			set
			{
				if (value == _hasVoted) return;
				_hasVoted = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(CanVote));
			}
		}

		private Answer? _selectedAnswer;
		public Answer? SelectedAnswer
		{
			get { return _selectedAnswer; }
			set
			{
				if (value == _selectedAnswer) return;
				_selectedAnswer = value;
				OnPropertyChanged();
			}
		}

		public Player(string id, string name, int winnedGames, string avatarURL, int nbLive)
		{
			ID = id;
			Name = name;
			WinnedGames = winnedGames;
			AvatarURL = avatarURL;
			Life = nbLive;
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}