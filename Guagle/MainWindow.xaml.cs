using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Zehong.CSharp.Solution.DBHandler;
using Zehong.CShart.Solution.HelperLib;
using Zehong.CShart.Solution.UserControls;

namespace CSharp.Solution.Guagle
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      this.Model = new GuagleWindowModel();
      InitializeComponent();
    }
    private void InitializeGuagle(IVerification verifier)
    {
      var grid = GuaguaGrid;
      //Clear
      grid.RowDefinitions.Clear();
      grid.ColumnDefinitions.Clear();
      grid.Children.Clear();
      // Initialize
      for (int i = 0; i < this.Model.RowCount; i++)
      {
        grid.RowDefinitions.Add(new RowDefinition());
      }
      for (int j = 0; j < this.Model.ColumnCount; j++)
      {
        grid.ColumnDefinitions.Add(new ColumnDefinition());
      }
      // Add Content
      var index = 0;
      var codes = verifier.CodeList.OfType<GuagleCode>().ToList();
      for (int i = 0; i < this.Model.RowCount; i++)
      {
        for (int j = 0; j < this.Model.ColumnCount; j++)
        {
          var ci = new CoveredItem(codes[index++].CodeText);
          ci.CodeChanged += ci_CodeChanged;
          Grid.SetRow(ci, i);
          Grid.SetColumn(ci, j);
          grid.Children.Add(ci);
        }
      }
    }

    private void ci_CodeChanged(string code)
    {
      var result = this.Model.Verify(code);
      if (result == null || !result.Result)
        MessageBox.Show("验证结果：假冒伪劣产品");
      else
        MessageBox.Show(String.Format("验证结果：正品\r\n{0}", result.Transaction.GetDetailedInformation()));
    }
    private void GuaguaGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      e.Handled = this.Model.HasVerified;
    }
    private void btnRequest_Click(object sender, RoutedEventArgs e)
    {
      IVerification verifier = null;
      var strRequestReturnd = this.Model.RandomRequest(out verifier);
      if (!String.IsNullOrWhiteSpace(strRequestReturnd))
      {
        this.InitializeGuagle(verifier);
        this.Model.Prompt = strRequestReturnd;
      }
      else
        this.Model.Prompt = null;
    }
    private void btnRequestAgain_Click(object sender, RoutedEventArgs e)
    {
      this.Model.Prompt = this.Model.RequestAgain();
    }

    public GuagleWindowModel Model { get; private set; }

    private void btnTest_Click(object sender, RoutedEventArgs e)
    {
      var md5Array = Helper.GetMD5HashFromFile(@"D:\丹霞山.jpg");
      var result = 0;
      for (int i = 0; i < md5Array.Length; i++)
      {
        result += (Int32)md5Array[i];
      }
      MessageBox.Show(result.ToString());
    }
  }

  public class GuagleWindowModel : NotifyPropertyChangedImp
  {
    public GuagleWindowModel()
    {
      this.RowCount = Rows[Helper.RandomObj.Next(2)];
      this.ColumnCount = Columns[Helper.RandomObj.Next(3)];
      this.DB = new GuagleDataContext(this.RowCount, this.ColumnCount, 4);

      _randomObj1 = new Random(2);
      System.Threading.Thread.Sleep(1000);
      _randomObj2 = new Random(2);
    }

    public String RandomRequest(out IVerification verifier)
    {
      verifier = null;
      try
      {
        int nRandom = 0;

        nRandom = Helper.RandomObj.Next(DB.GroupList.Count);
        var objGroup = DB.GroupList[nRandom];
        if (objGroup == null)
          return null;

        nRandom = Helper.RandomObj.Next(objGroup.CompanyList.Count);
        var objCompany = objGroup.CompanyList[nRandom];
        if (objCompany == null)
          return null;

        nRandom = Helper.RandomObj.Next(objCompany.ProductList.Count);
        var objProduct = objCompany.ProductList[nRandom];
        if (objProduct == null)
          return null;

        nRandom = Helper.RandomObj.Next(objProduct.LotList.Count);
        var objLot = objProduct.LotList[nRandom];
        if (objLot == null)
          return null;

        nRandom = Helper.RandomObj.Next(objLot.TransactionList.Count);
        var objTransaction = objLot.TransactionList[nRandom];
        if (objTransaction == null)
          return null;

        verifier = objTransaction.Verfication;
        _strRequestBackup = _strRequest = String.Format("{0};{1};{2};{3};{4}",
          objGroup.SID,
          objCompany.SID,
          objProduct.SID,
          objLot.SID,
          objTransaction.SID);
        return DB.Request(_strRequest);
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
        return null;
      }
      finally
      {
        this.SendPropertyChanged(() => CanRequestAgain);
      }
    }
    public String RequestAgain()
    {
      if (!String.IsNullOrWhiteSpace(_strRequestBackup))
      {
        _strRequest = _strRequestBackup;
        return DB.Request(_strRequest);
      }
      return null;
    }
    public VerificationResult Verify(String strVerification)
    {
      try
      {
        if (_strRequest == null)
          return null;
        return DB.Verify(_strRequest, strVerification);
      }
      finally
      {
        _strRequest = null;
      }
    }

    public Int16 RowCount { get; private set; }
    public Int16 ColumnCount { get; private set; }
    public Boolean HasVerified { get { return _strRequest == null; } }
    public Boolean CanRequestAgain { get { return _strRequestBackup != null; } }
    public String Prompt
    {
      get { return _prompt; }
      set
      {
        if (!String.Equals(_prompt, value))
        {
          _prompt = value;
          this.SendPropertyChanged(() => Prompt);
        }
      }
    }
    public String RandomString1
    {
      get
      {
        if (_randomString1 == null)
        {
          var valueList = new List<Int32>();
          for (int i = 0; i < 10; i++)
          {
            valueList.Add(_randomObj1.Next(10));
          }
          _randomString1 = Helper.ListToString(valueList);
        }
        return _randomString1;
      }
      set
      {
        if (!String.Equals(_randomString1, value))
        {
          _randomString1 = value;
          this.SendPropertyChanged(() => RandomString1);
        }
      }
    }
    public String RandomString2
    {
      get
      {
        if (_randomString2 == null)
        {
          var valueList = new List<Int32>();
          for (int i = 0; i < 10; i++)
          {
            valueList.Add(_randomObj2.Next(10));
          }
          _randomString2 = Helper.ListToString(valueList);
        }
        return _randomString2;
      }
      set
      {
        if (!String.Equals(_randomString2, value))
        {
          _randomString2 = value;
          this.SendPropertyChanged(() => RandomString2);
        }
      }
    }
    
    private GuagleDataContext DB = null;
    private String _strRequest = null;
    private String _strRequestBackup = null;
    private String _prompt = null;
    
    private Random _randomObj1 = null;
    private Random _randomObj2 = null;
    private String _randomString1 = null;
    private String _randomString2 = null;

    private static Int16[] Rows = new Int16[] { 3, 4 };
    private static Int16[] Columns = new Int16[] { 3, 4, 5 };
  }
}
