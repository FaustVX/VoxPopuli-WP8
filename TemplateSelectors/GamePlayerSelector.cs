using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VoxPopuli.TemplateSelectors
{
	public class GamePlayerSelector : DataTemplateSelector
	{
		public DataTemplate NormalPlayerTemplate { get; set; }
		public DataTemplate EliminatedPlayerTemplate { get; set; }
		public DataTemplate RedPlayerTemplate { get; set; }
		public DataTemplate GreenPlayerTemplate { get; set; }
		public DataTemplate BluePlayerTemplate { get; set; }

		protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
		{
			var player = item as Player;
			if (player == null)
				return null;

			if (player.IsEliminated)
				return EliminatedPlayerTemplate;

			switch (player.SelectedAnswer)
			{
				case Answer.A:
					return RedPlayerTemplate;
				case Answer.B:
					return GreenPlayerTemplate;
				case Answer.C:
					return BluePlayerTemplate;
			}

			return NormalPlayerTemplate;
		}
	}
}