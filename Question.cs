using System.ComponentModel;
using System.Runtime.CompilerServices;
using VoxPopuli.Annotations;

namespace VoxPopuli
{
	public class Question : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public Question(string question, string answerA, string answerB, string answerC)
		{
			QuestionString = question;
			ResponseA = new Response(answerA);
			ResponseB = new Response(answerB);
			ResponseC = new Response(answerC);
		}

		public string QuestionString { get; } 

		public Response ResponseA { get; }

		/*
		public string AnswerA { get; }

		private int _responseA;
		public int ResponseA
		{
			get { return _responseA; }
			set
			{
				if (value == _responseA) return;
				_responseA = value;
				OnPropertyChanged();
			}
		}

		private bool _majorityA;
		public bool MajorityA
		{
			get { return _majorityA; }
			set
			{
				if (value == _majorityA) return;
				_majorityA = value;
				OnPropertyChanged();
			}
		}*/

		public Response ResponseB { get; }

		/*
		public string AnswerB { get; }

		private int _responseB;
		public int ResponseB
		{
			get { return _responseB; }
			set
			{
				if (value == _responseB) return;
				_responseB = value;
				OnPropertyChanged();
			}
		}

		private bool _majorityB;
		public bool MajorityB
		{
			get { return _majorityB; }
			set
			{
				if (value == _majorityB) return;
				_majorityB = value;
				OnPropertyChanged();
			}
		}*/
		public Response ResponseC { get; }

		/*
		public string AnswerC { get; }

		private int _responseC;
		public int ResponseC
		{
			get { return _responseC; }
			set
			{
				if (value == _responseC) return;
				_responseC = value;
				OnPropertyChanged();
			}
		}

		private bool _majorityC;
		public bool MajorityC
		{
			get { return _majorityC; }
			set
			{
				if (value == _majorityC) return;
				_majorityC = value;
				OnPropertyChanged();
			}
		}*/

		private bool _showVote;
		public bool ShowVote
		{
			get { return _showVote; }
			set
			{
				if (value == _showVote) return;
				_showVote = value;
				OnPropertyChanged();
			}
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}