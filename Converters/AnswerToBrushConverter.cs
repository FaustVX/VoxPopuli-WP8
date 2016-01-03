using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace VoxPopuli.Converters
{
	public class AnswerToBrushConverter : IValueConverter
	{
		public Brush Null { get; set; }
		public Brush A { get; set; }
		public Brush B { get; set; }
		public Brush C { get; set; }

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is Answer)
			{
				switch ((Answer)value)
				{
					case Answer.A:
						return A;
					case Answer.B:
						return B;
					case Answer.C:
						return C;
					default:
						throw new ArgumentOutOfRangeException(nameof(value), value, null);
				}
			}
			else if (value == null)
				return Null;
			else
				throw new InvalidCastException(nameof(value));
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}