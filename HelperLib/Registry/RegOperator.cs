using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Zehong.CShart.Solution.HelperLib
{
  public static class RegHandler
  {
    public static void SetRegValue<T>(String relativeKey, String name, T value)
    {
      if (String.IsNullOrWhiteSpace(name))
        return;

      System.Globalization.CultureInfo oldCi = Thread.CurrentThread.CurrentCulture;
      try
      {
        Thread.CurrentThread.CurrentCulture = Helper.DefaultCultureInfo;

        var subKey = GetKey(relativeKey);
        var regKey = Registry.CurrentUser.CreateSubKey(subKey);
        if (regKey != null)
        {
          if (value == null)
            regKey.SetValue(name, "");
          else
            regKey.SetValue(name, value);
        }

        if (Helper.IsAdmin())
        {
          subKey = ".DEFAULT\\" + subKey;
          regKey = Registry.Users.CreateSubKey(subKey);
          if (regKey != null)
          {
            if (value == null)
              regKey.SetValue(name, "");
            else
              regKey.SetValue(name, value);
          }
        }
      }
      catch (Exception e)
      {
        ExceptionHandler.ThrowException(e);
      }
      finally
      {
        System.Threading.Thread.CurrentThread.CurrentCulture = oldCi;
      }
    }
    public static void SetRegValueSST(String relativeKey, String name, string value)
    {
      value = string.Format("SST:{0}", SSTCryptographer.Boxing(value, "WebReporter"));
      SetRegValue(relativeKey, name, value);
    }

    public static object GetRegValue(String relativeKey, String name)
    {
      try
      {
        if (String.IsNullOrWhiteSpace(name))
          return null;

        object regValue = null;

        var subKey = GetKey(relativeKey);
        var regKey = Registry.CurrentUser.OpenSubKey(subKey);
        if (regKey != null)
          regValue = regKey.GetValue(name);
        if (regValue != null)
          return regValue;

        if (!Helper.IsAdmin())
          return null;

        subKey = ".DEFAULT\\" + subKey;
        regKey = Registry.Users.CreateSubKey(subKey);
        if (regKey != null)
          regValue = regKey.GetValue(name);
        return regValue;
      }
      catch (Exception e)
      {
        ExceptionHandler.ThrowException(e);
        return null;
      }
    }
    public static T GetRegValue<T>(String relativeKey, String name, T defaultValue)
    {
      if (String.IsNullOrWhiteSpace(name))
        return defaultValue;

      var regValue = GetRegValue(relativeKey, name);
      if (regValue == null)
        return defaultValue;

      var regValueString = regValue.ToString();
      if (!string.IsNullOrWhiteSpace(regValueString) && regValueString.StartsWith("SST:"))
        regValueString = SSTCryptographer.Unboxing(regValueString.Substring(4), "WebReporter");

      if (regValueString != null)
        return regValueString.SafeConvertInvariantStringTo<T>();
      return defaultValue;
    }

    public static void DeleteRegValue(String relativeKey, String name)
    {
      try
      {
        var subKey = GetKey(relativeKey);
        RegistryKey regKey = Registry.CurrentUser.OpenSubKey(subKey, true);
        if (regKey != null)
        {
          var valueNames = regKey.GetValueNames();
          if (valueNames.Contains(name))
            regKey.DeleteValue(name);
          regKey.Close();
        }
        if (Helper.IsAdmin())
        {
          subKey = ".DEFAULT\\" + subKey;
          regKey = Registry.Users.OpenSubKey(subKey, true);
          if (regKey == null)
            return;

          var valueNames = regKey.GetValueNames();
          if (valueNames.Contains(name))
            regKey.DeleteValue(name);
          regKey.Close();
        }
      }
      catch (Exception e)
      {
        ExceptionHandler.ThrowException(e);
      }
    }

    public static void RemoveExcelOpeningWarning()
    {
      RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Office\\12.0\\Excel\\Security", true);
      if (regKey != null)
      {
        regKey.SetValue("ExtensionHardening", 0, RegistryValueKind.DWord);
        regKey.Close();
      }
    }
    public static void DeleteSubKeyTree(String relativeKey, String subName = "")
    {
      var subKey = GetKey(relativeKey);
      RegistryKey regKey = Registry.CurrentUser.OpenSubKey(subKey, true);
      if (regKey != null)
      {
        var names = regKey.GetSubKeyNames();
        if (names.Contains(subName))
          regKey.DeleteSubKeyTree(subName);
        regKey.Close();
      }

      if (Helper.IsAdmin())
      {
        subKey = ".DEFAULT\\" + subKey;
        regKey = Registry.Users.OpenSubKey(subKey, true);
        if (regKey != null)
        {
          var names = regKey.GetSubKeyNames();
          if (names.Contains(subName))
            regKey.DeleteSubKeyTree(subName);
          regKey.Close();
        }
      }
    }
    public static string[] GetSubKeyNames(String relativeKey)
    {
      try
      {
        var result = default(String[]);
        var subKey = GetKey(relativeKey);
        RegistryKey regKey = Registry.CurrentUser.OpenSubKey(subKey);
        if (regKey != null)
        {
          result = regKey.GetSubKeyNames();
          regKey.Close();
        }
        else if (Helper.IsAdmin())
        {
          subKey = ".DEFAULT\\" + subKey;
          regKey = Registry.Users.OpenSubKey(subKey);
          if (regKey == null)
            return null;

          result = regKey.GetSubKeyNames();
          regKey.Close();
        }
        return result;
      }
      catch (Exception e)
      {
        ExceptionHandler.ThrowException(e);
        return null;
      }
    }
    public static string[] GetValueNames(String relativeKey)
    {
      try
      {
        RegistryKey regKey = Registry.CurrentUser.OpenSubKey(GetKey(relativeKey));
        if (regKey == null)
          return null;

        var result = regKey.GetValueNames();
        regKey.Close();
        return result;
      }
      catch (Exception e)
      {
        ExceptionHandler.ThrowException(e);
        return null;
      }
    }
    public static bool RegistryKeyExists(String relativeKey)
    {
      try
      {
        RegistryKey regKey = Registry.CurrentUser.OpenSubKey(GetKey(relativeKey));
        if (regKey == null)
          return false;

        regKey.Close();
        return true;
      }
      catch (Exception e)
      {
        ExceptionHandler.ThrowException(e);
        return false;
      }
    }
    public static void DeleteRedundantSubKey(String relativeKey, List<String> subKeyNamesExist, List<String> valueNamesExist)
    {
      if (String.IsNullOrWhiteSpace(relativeKey))
        return;

      // Delete extra key
      var subKey = GetKey(relativeKey);
      var regKey = Registry.CurrentUser.OpenSubKey(subKey, true);
      if (regKey != null)
      {
        DeleteRedundantSubKey(regKey, subKeyNamesExist, valueNamesExist);
        regKey.Close();
      }
      if (Helper.IsAdmin())
      {
        subKey = ".DEFAULT\\" + subKey;
        regKey = Registry.Users.OpenSubKey(subKey, true);
        if (regKey != null)
        {
          DeleteRedundantSubKey(regKey, subKeyNamesExist, valueNamesExist);
          regKey.Close();
        }
      }
    }

    private static String GetKey(String relativeKey)
    {
      String subKey = "Software\\ZZH_Solution";
      if (!String.IsNullOrWhiteSpace(relativeKey))
      {
        relativeKey = relativeKey.Trim();
        if (!relativeKey.StartsWith("\\"))
          subKey += "\\";
        subKey += relativeKey;
      }
      return subKey;
    }
    private static void DeleteRedundantSubKey(RegistryKey regKey, List<String> currentSubKeyNames, List<String> currentValueNames)
    {
      if (regKey == null)
        return;

      try
      {
        // Delete extra keys
        var exstingNames = regKey.GetSubKeyNames();
        if (exstingNames != null)
        {
          foreach (var existingName in exstingNames)
          {
            if (!currentSubKeyNames.Contains(existingName))
              regKey.DeleteSubKeyTree(existingName);
          }
        }

        // Delete extra value names
        exstingNames = regKey.GetValueNames();
        if (exstingNames != null)
        {
          foreach (var existingName in exstingNames)
          {
            if (!currentValueNames.Contains(existingName))
              regKey.DeleteValue(existingName);
          }
        }
      }
      catch (Exception ex)
      {
        ExceptionHandler.LogFile(ex.Message);
      }
    }
  }
}
