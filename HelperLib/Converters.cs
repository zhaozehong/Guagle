using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Zehong.CShart.Solution.HelperLib
{
  public sealed class EnumToFriendlyNameConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Helper.GetLocalizedName(value);
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
  public sealed class ValueMinMaxToPointConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values == null || values.Count() < 3)
        return null;

      Double? value = values[0] as Double?;
      Double? minimum = values[1] as Double?;
      Double? maximum = values[2] as Double?;

      // Convert the value to one between 0 and 360
      Double? current = (maximum != minimum) ? (value / (maximum - minimum)) * 360 : 0;

      // Adjust the finished state so the ArcSegment gets drawn as a whole circle
      if (current == 360)
        current = 359.999;

      // Shift by 90 degrees so 0 starts at the top of the circle
      current = current - 90;

      // Convert the angle to radians
      current = current * 0.017453292519943295;

      // Calculate the circle's point
      Double x = 10 + 10 * Math.Cos((Double)current);
      Double y = 10 + 10 * Math.Sin((Double)current);

      return new Point(x, y);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
        CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
  public sealed class ValueMinMaxToIsLargeArcConverter : IMultiValueConverter
  {
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values == null || values.Count() < 3)
        return false;

      Double? value = values[0] as Double?;
      Double? minimum = values[1] as Double?;
      Double? maximum = values[2] as Double?;

      // Only return true if the value is 50% of the range or greater
      return ((value * 2) >= (maximum - minimum));
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }





}
