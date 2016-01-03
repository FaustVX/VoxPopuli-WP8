using System;
using Windows.UI.Xaml.Data;

namespace VoxPopuli.Converters
{
	public class DebugConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
			=> value;

		public object ConvertBack(object value, Type targetType, object parameter, string language)
			=> value;
	}
}