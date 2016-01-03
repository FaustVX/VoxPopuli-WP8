using System;
using Windows.UI.Xaml.Data;

namespace VoxPopuli.Converters
{
	public class AnswerConverter : EnumConverter
	{
		public AnswerConverter()
		{
			EnumType = typeof (Answer);
		}
	}

	public class EnumConverter : IValueConverter
	{
		public bool AllowNullValue { get; set; }
		public bool AllowNullParameter { get; set; }
		public Type EnumType { get; set; }

		/// <summary>
		/// Converts a value. 
		/// </summary>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		/// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
		public object Convert(object value, Type targetType, object parameter, string culture)
		{
			if (!AllowNullValue && !(value is Enum))
				throw new InvalidCastException();
			if (!AllowNullParameter && !(parameter is string))
				throw new InvalidCastException();

			if (value == null && parameter == null)
				return true;

			if (value == null || parameter == null)
				return false;

			var btn = (Enum)value;
			var param = (Enum)Enum.Parse(EnumType, (string) parameter);

			return btn.Equals(param);
		}

		/// <summary>
		/// Converts a value. 
		/// </summary>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		/// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
		public object ConvertBack(object value, Type targetType, object parameter, string culture)
		{
			if (!(value is bool) || (!AllowNullParameter && !(parameter is string)))
				throw new InvalidCastException();

			var btn = (bool)value;
			if (!btn || !Enum.IsDefined(EnumType, (string) parameter))
				return null;

			return Enum.Parse(EnumType, (string) parameter);
		}
	}
}