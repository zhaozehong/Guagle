using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Zehong.CShart.Solution.HelperLib;

namespace Zehong.CSharp.Solution.DBHandler
{
  public partial class DatabaseDialog : Window
  {
    public DatabaseDialog()
    {
      this.Model = new DatabaseDialogModel();
      InitializeComponent();
    }

    public DatabaseDialogModel Model { get; private set; }

    private void btnDatabaseFolder_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        var folderDlg = new System.Windows.Forms.FolderBrowserDialog();
        folderDlg.Description = Zehong.CSharp.Solution.AppStrings.Properties.Resources.DatabaseDialog_DirectoryBrowser_Prompt;
        if (!String.IsNullOrWhiteSpace(this.Model.DatabaseFolder) && System.IO.Directory.Exists(this.Model.DatabaseFolder))
          folderDlg.SelectedPath = this.Model.DatabaseFolder;

        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
          Helper.Launch(folderDlg.SelectedPath, String.Empty, false, false);
          return;
        }

        if (folderDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
          this.Model.DatabaseFolder = folderDlg.SelectedPath;
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
      }
      ExceptionHandler.ShowErrorMessageBox();
    }

    private void btnOK_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        this.CreateLocalDatabase();
        this.DialogResult = true;
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
        this.DialogResult = false;
      }
    }

    public Boolean CreateLocalDatabase()
    {
      var mdfFilePath = this.Model.DatabaseFilePath;
      var logFilePath = this.Model.DatabaseLogFilePath;
      if (File.Exists(mdfFilePath) || File.Exists(logFilePath))
      {
        if (!this.Model.NeedCreateLocalInstance)
          throw new Exception(AppStrings.Properties.Resources.DB_Warning_DatabaseFileAlreadyExist);
      }
      else
      {
        var mdfStream = Helper.GetResourceStream("DBHandler;Component/Database/Demo.mdf");
        var logStream = Helper.GetResourceStream("DBHandler;Component/Database/Demo_log.ldf");
        if (mdfStream == null || logStream == null)
          throw new Exception(AppStrings.Properties.Resources.DB_Warning_DatabaseSourceNotExist);

        using (var mdfFileStream = new FileStream(mdfFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
        {
          Helper.Pump(mdfStream, mdfFileStream);
          mdfFileStream.Close();
        }
        using (var logFileStream = new FileStream(logFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
        {
          Helper.Pump(logStream, logFileStream);
          logFileStream.Close();
        }
      }

      return true;
    }
  }
  public class DatabaseDialogModel : NotifyPropertyChangedImp
  {
    public List<String> AvailableDatabases
    {
      get
      {
        var files = new List<String>();
        var directoryInfo = new DirectoryInfo(Helper.ForceCreateDirectory(this.DatabaseFolder));
        var fileInfos = directoryInfo.GetFiles("*.mdf");
        foreach (var fileInfo in fileInfos)
        {
          files.Add(System.IO.Path.GetFileNameWithoutExtension(fileInfo.Name));
        }
        return files;
      }
    }

    public String DatabaseFolder
    {
      get { return _databaseFolder; }
      set
      {
        if (!object.Equals(_databaseFolder, value))
        {
          _databaseFolder = value;
          this.SendPropertyChanged(() => DatabaseFolder);
          this.SendPropertyChanged(() => AvailableDatabases);
        }
      }
    }
    public String DatabaseName
    {
      get { return _databaseName; }
      set
      {
        if (!object.Equals(_databaseName, value))
        {
          _databaseName = value;
          this.SendPropertyChanged(() => DatabaseName);
        }
      }
    }
    public String DatabaseFilePath { get { return String.Format("{0}{1}.mdf", this.DatabaseFolder, this.DatabaseName); } }
    public String DatabaseLogFilePath { get { return String.Format("{0}{1}_log.ldf", this.DatabaseFolder, this.DatabaseName); } }
    public Boolean NeedCreateLocalInstance { get { return true; } }
    public String LocalInstanceName
    {
      get { return _localInstanceName; }
      set
      {
        if (_localInstanceName != value)
        {
          _localInstanceName = value;
          this.SendPropertyChanged(() => LocalInstanceName);
        }
      }
    }

    private String _databaseFolder = null;
    private String _databaseName = null;
    private String _localInstanceName = null;
  }
}
