using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace VoxPopuli.Converters
{
	public class BoolToTConverter<T> : DependencyObject, IValueConverter
	{
		//public T True { get; set; }
		//public T False { get; set; }

		public static readonly DependencyProperty TrueProperty = DependencyProperty.Register(
			nameof(True), typeof (T), typeof (BoolToTConverter<T>), new PropertyMetadata(default(T)));

		public T True
		{
			get { return (T) GetValue(TrueProperty); }
			set { SetValue(TrueProperty, value); }
		}

		public static readonly DependencyProperty FalseProperty = DependencyProperty.Register(
			"False", typeof (T), typeof (BoolToTConverter<T>), new PropertyMetadata(default(T)));

		public T False
		{
			get { return (T) GetValue(FalseProperty); }
			set { SetValue(FalseProperty, value); }
		}

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is bool)
				return ((bool) value) ? True : False;

			throw new InvalidCastException(nameof(value));
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}