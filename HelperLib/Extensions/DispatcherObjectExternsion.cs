using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Zehong.CShart.Solution.HelperLib
{
  public static class DispatcherObjectExternsion
  {
    public static T SafeCreateInstance<T>(this DispatcherObject dispatcherObject, params object[] args)
    {
      try
      {
        var type = typeof(T);
        const BindingFlags bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.NonPublic;
        if (dispatcherObject.CheckAccess())
          return (T)Activator.CreateInstance(type, bf, null, args, null);

        T ret = default(T);
        dispatcherObject.Dispatcher.Invoke(new Action(delegate { ret = (T)Activator.CreateInstance(type, bf, null, args, null); }));
        return ret;
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
        throw;
      }
    }
    public static object SafeConvertInvariantStringTo(this string text, Type type)
    {
      if (Application.Current == null || Application.Current.CheckAccess() || !type.IsSubclassOf(typeof(DispatcherObject)))
        return System.ComponentModel.TypeDescriptor.GetConverter(type).ConvertFromInvariantString(text);

      object ret = null;
      Application.Current.Dispatcher.Invoke(
        new Action(() => ret = System.ComponentModel.TypeDescriptor.GetConverter(type).ConvertFromInvariantString(text)));
      return ret;
    }
    public static T SafeConvertInvariantStringTo<T>(this string text)
    {
      return (T)text.SafeConvertInvariantStringTo(typeof(T));
    }

  }
}
