using System;
using Windows.UI.Xaml.Data;

namespace VoxPopuli.Converters
{
	public class InvertedBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is bool)
				return !(bool) value;

			throw new InvalidCastException(nameof(value));
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			if (value is bool)
				return !(bool)value;

			throw new InvalidCastException(nameof(value));
		}
	}
}