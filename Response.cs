using System.ComponentModel;
using System.Runtime.CompilerServices;
using VoxPopuli.Annotations;

namespace VoxPopuli
{
	public class Response : INotifyPropertyChanged
	{
		public Response(string answer)
		{
			Answer = answer;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public string Answer { get; }

		private int _vote;
		public int Vote
		{
			get { return _vote; }
			set
			{
				if (value == _vote) return;
				_vote = value;
				OnPropertyChanged1();
			}
		}

		private bool _majority;
		public bool Majority
		{
			get { return _majority; }
			set
			{
				if (value == _majority) return;
				_majority = value;
				OnPropertyChanged1();
			}
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged1([CallerMemberName] string propertyName = null)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}