using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace VoxPopuli.Converters
{
	public class BoolToVisibility : BoolToTConverter<Visibility>
	{
		public BoolToVisibility()
		{
			True = Visibility.Visible;
			False = Visibility.Collapsed;
		}
	}
}