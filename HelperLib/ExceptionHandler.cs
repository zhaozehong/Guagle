using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Zehong.CShart.Solution.HelperLib
{
  public class ExceptionHandler
  {
    public static void ThrowException(Exception e)
    {
      if (e is System.Threading.ThreadAbortException)
        return;
      ExceptionManager.Current.ThrowException(e);
    }
    public static void ThrowException(String error, Boolean needDumpStackTrace = true)
    {
      ExceptionManager.Current.ThrowException(error, needDumpStackTrace);
    }
    public static void LogFile(String info)
    {
      ExceptionManager.Current.LogFile(info);
    }
    public static void SetExtraInfomation(String extraInfo)
    {
      ExceptionManager.Current.SetExtraInfomation(extraInfo);
    }

    public static void ShowErrorMessageBox()
    {
      ExceptionManager.Current.ShowErrorMessageBox();
    }
    public static void ShowErrorMessageBox(String caption)
    {
      ExceptionManager.Current.ShowErrorMessageBox(caption);
    }

    public static void ClearException(String exception)
    {
      ExceptionManager.Current.ClearException(exception);
    }
    public static void ClearAllExceptions()
    {
      ExceptionManager.Current.ClearAllExceptions();
    }

    public static Boolean IsIgnoreWarningMessage
    {
      get { return ExceptionManager.Current.IsIgnoreWarningMessage; }
      set { ExceptionManager.Current.IsIgnoreWarningMessage = value; }
    }

    public static String AllErrorMessage { get { return ExceptionManager.Current.AllErrorMessage; } }
    public static Boolean HasError { get { return ExceptionManager.Current.HasError; } }
    public static String LastErrorMessage { get { return ExceptionManager.Current.LastErrorMessage; } }
  }

  public class ExceptionManager : DispatcherObject
  {
    public void ThrowException(Exception e)
    {
      ThrowException(e.Message, false);
      if (e.InnerException != null)
        ThrowException(e.InnerException.Message);
      DumpIntoFile(e.StackTrace, false);
    }
    public void ThrowException(String error, Boolean needDumpStackTrace = true)
    {
      if (_latestErrorMessages.IndexOf(error) == -1)
        _latestErrorMessages.Add(error);

      DumpIntoFile(error, needDumpStackTrace);
    }
    public void LogFile(String info)
    {
      DumpIntoFile(info, false);
    }
    public void SetExtraInfomation(String extraInfo)
    {
      _extraInfomation = extraInfo;
    }

    public void ShowErrorMessageBox()
    {
      if (!this.IsIgnoreWarningMessage)
        this.ShowErrorMessageBox("Failed");
    }
    public void ShowErrorMessageBox(String caption)
    {
      if (this.CheckAccess() == false)
      {
        this.Dispatcher.BeginInvoke(new Action(() => ShowErrorMessageBox(caption)));
        return;
      }

      if (this.HasError)
      {
        var prompt = ShortErrorMessage;
        if (_extraInfomation != String.Empty)
          prompt += "\n\n" + _extraInfomation;
        var hasDetails = _latestErrorMessages.Count() > 12;
        var details = AllErrorMessage;

        _latestErrorMessages.Clear();
        _extraInfomation = String.Empty;

        using (new CursorSetter(null))
        {
          MessageBox.Show(prompt, caption, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.Yes);
        }
      }
      else
      {
        _latestErrorMessages.Clear();
        _extraInfomation = String.Empty;
      }
    }
    public void ShowFinishOperationMessageBox(String opName)
    {
      if (this.CheckAccess() == false)
      {
        this.Dispatcher.BeginInvoke(new Action(() => ShowFinishOperationMessageBox(opName)));
        return;
      }

      if (ExceptionHandler.HasError)
      {
        ExceptionHandler.ShowErrorMessageBox(opName);
      }
      else
      {
        String prompt = "Success";
        if (_extraInfomation != String.Empty)
          prompt += "\n\n" + _extraInfomation;
        _extraInfomation = String.Empty;

        System.Windows.MessageBox.Show(prompt, opName, MessageBoxButton.OK, MessageBoxImage.Information);
      }
    }

    public void ClearAllExceptions()
    {
      _latestErrorMessages.Clear();
    }
    public void ClearException(String exception)
    {
      var query = from p in _latestErrorMessages
                  where p != exception
                  select p;
      _latestErrorMessages = query.ToList();
    }

    private void DumpIntoFile(String info, Boolean needDumpStackTrace)
    {
      try
      {
        var logPath = Helper.ForceCreateDirectory(String.Format("{0}\\log", Helper.AppDataDirectory));
        var logFileName = String.Format("{0}\\{1}-{2}-{3}.log", logPath, DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
        var sw = File.Exists(logFileName) ? File.AppendText(logFileName) : File.CreateText(logFileName);
        sw.BaseStream.Seek(0, SeekOrigin.End);
        sw.WriteLine(String.Format("****{0}****{1}", DateTime.Now, this.AppVersionInfo));
        sw.WriteLine(info);
        if (needDumpStackTrace)
        {
          sw.WriteLine(String.Format("**{0}**", "StackTrace"));

          var trace = new StackTrace(true);
          var stackFrames = trace.GetFrames();
          if (stackFrames != null)
          {
            foreach (StackFrame frame in stackFrames)
            {
              var file = frame.GetFileName();
              if (String.IsNullOrWhiteSpace(file))
                continue;
              var methodName = frame.GetMethod().Name;
              if (methodName == "DumpIntoFile" || methodName == "ThrowException")
                continue;

              var line = frame.GetFileLineNumber();
              var column = frame.GetFileColumnNumber();
              if (line == 0 && column == 0)
                continue;

              sw.WriteLine(String.Format("{3} : {0}({1},{2})", Path.GetFileName(file), line, column, methodName));
            }
          }
        }
        sw.Close();

        // Delete old files if there are more than 15 log files
        var files = Directory.GetFiles(logPath).ToList();
        if (files.Count > 15)
        {
          var dictionary = new Dictionary<String, DateTime>();
          files.ForEach(p => dictionary.Add(p, File.GetCreationTime(p)));
          var items = dictionary.OrderBy(p => p.Value).Take(files.Count - 15);
          foreach (var item in items)
          {
            File.Delete(item.Key);
          }
        }
      }
      catch (Exception) { }
    }

    public String AllErrorMessage
    {
      get
      {
        var retVal = String.Empty;
        foreach (String tmp in _latestErrorMessages)
        {
          retVal += tmp + "\n";
        }
        return retVal.TrimEnd('\n');
      }
    }
    public String ShortErrorMessage
    {
      get
      {
        var retVal = String.Empty;
        for (int i = 0; i < 12 && i < _latestErrorMessages.Count; i++)
        {
          retVal += _latestErrorMessages[i] + "\n";
        }
        return retVal.TrimEnd('\n');
      }
    }
    public Boolean IsIgnoreWarningMessage
    {
      get { return _isIgnoreWarningMessage; }
      set { _isIgnoreWarningMessage = value; }
    }
    public String AppVersionInfo
    {
      get
      {
        if (String.IsNullOrWhiteSpace(_appVersionInfo))
          _appVersionInfo = "0.0.0.0(Unknown Version)";
        return _appVersionInfo;
      }
    }
    public List<String> LatestErrorMessages { get { return _latestErrorMessages; } }
    public Boolean HasError { get { return _latestErrorMessages.Count > 0; } }
    public Boolean ExtraInfomationIsEmpty { get { return _extraInfomation == String.Empty; } }
    public String LastErrorMessage { get { return _latestErrorMessages.LastOrDefault(); } }

    private Boolean _isIgnoreWarningMessage = false;
    private String _appVersionInfo = null;
    private List<String> _latestErrorMessages = new List<String>();
    private String _extraInfomation = String.Empty;


    private static ExceptionManager _current = null;
    public static ExceptionManager Current
    {
      get
      {
        if (_current == null)
        {
          if (Application.Current != null)
            _current = Application.Current.SafeCreateInstance<ExceptionManager>();
          else
            _current = new ExceptionManager();
        }
        return _current;
      }
    }
  }
}
