namespace VoxPopuli
{
	public class GameMode
	{
		public GameMode(string name, bool looserCanVote, bool looserCanVoteFinal, int nbLiveStart, int nbSafeQuestions, int nbSecsToVote, bool noQuestions, int gainLifeWhenSafe, bool isDebug)
		{
			Name = name;
			LooserCanVote = looserCanVote;
			LooserCanVoteFinal = looserCanVoteFinal;
			NbLiveStart = nbLiveStart;
			NbSafeQuestions = nbSafeQuestions;
			NbSecsToVote = nbSecsToVote;
			NoQuestions = noQuestions;
			GainLifeWhenSafe = gainLifeWhenSafe;
			IsDebug = isDebug;
		}

		public string Name { get; }
		public bool LooserCanVote { get; }
		public bool LooserCanVoteFinal { get; }
		public int NbLiveStart { get; }
		public int NbSafeQuestions { get; }
		public int NbSecsToVote { get; }
		public bool NoQuestions { get; }
		public int GainLifeWhenSafe { get; }
		public bool IsDebug { get; }
	}
}