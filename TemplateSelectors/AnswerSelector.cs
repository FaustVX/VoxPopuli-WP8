using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace VoxPopuli.TemplateSelectors
{
	public class AnswerSelector : DataTemplateSelector
	{
		public DataTemplate NormalAnswerTemplate { get; set; }
		public DataTemplate MajorityAnswerTemplate { get; set; }

		protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
		{
			return MajorityAnswerTemplate;
		}
	}
}