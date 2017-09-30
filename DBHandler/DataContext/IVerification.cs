using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Zehong.CShart.Solution.HelperLib;
using StringResources = Zehong.CSharp.Solution.AppStrings.Properties.Resources;

namespace Zehong.CSharp.Solution.DBHandler
{
  // 扫二维码就跟这个接口打交道
  public interface IVerification : IDatabaseTable
  {
    ITransaction Owner { get; }
    DateTime? LastAccessTime { get; }
    List<ICode> CodeList { get; }

    Image GetQRCode(CodeSizes size);
    Image GetBarCode(CodeSizes size);
    String Request();
    Boolean Verify(String strValue);
  }
  public class CodeCard : DatabaseTable, IVerification
  {
    public CodeCard(ITransaction owner, Int16 rowCount, Int16 columnCount, Int16 codeLength)
    {
      this._owner = owner;
      this._rowCount = rowCount;
      this._columnCount = columnCount;
      var totalCount = _rowCount * _columnCount;
      for (int i = 0; i < totalCount; i++)
      {
        this.CodeList.Add(new GuagleCode(codeLength));
      }
    }

    public Image GetQRCode(CodeSizes size)
    {
      var uniqueString = String.Format("{0}\\{1}\\{2}\\{3}\\{4}", "GroupName", "CompanyName", "ProductName", "LotName", " TransactionIndex");
      return null;
    }
    public Image GetBarCode(CodeSizes size)
    {
      var uniqueString = String.Format("{0}\\{1}\\{2}\\{3}\\{4}", "GroupName", "CompanyName", "ProductName", "LotName", " TransactionIndex");
      return null;
    }
    public String Request()
    {
      for (int i = 0; i < this.CodeList.Count; i++)
      {
        var result = this.CodeList[i].Request();
        if (result == CodeRequestResults.HasVerified)
          continue;

        if (result == CodeRequestResults.HasLocked)
          return StringResources.StringCode_Warning_ProductLocked;

        this._lastAccessTime = DateTime.Now;
        this.StartTimer();

        return String.Format(StringResources.StringCode_Prompt, (i / _columnCount) + 1, (i % _columnCount) + 1);
      }
      return StringResources.StringCode_Warning_AllCodesOnCardVerified;
    }
    public Boolean Verify(String strValue)
    {
      try
      {
        var currentCode = this.CodeList.FirstOrDefault(p => p.Status.HasFlag(CodeStatus.Locked));
        if (currentCode == null)
          return false;
        return currentCode.Verify(strValue);
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
        return false;
      }
      finally
      {
        _timer.Stop();
      }
    }

    private void StartTimer()
    {
      if (_timer == null)
      {
        _timer = new System.Timers.Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
        _timer.Elapsed += delegate(object sender, System.Timers.ElapsedEventArgs e)
        {
          var thread = new Thread(() => this.Verify(null));
          thread.Start();
        };
      }
      _timer.Start();
    }

    public ITransaction Owner { get { return _owner; } }
    public DateTime? LastAccessTime { get { return _lastAccessTime; } }
    public List<ICode> CodeList
    {
      get
      {
        if (_codeList == null)
          _codeList = new List<ICode>();
        return _codeList;
      }
    }

    public ITransaction _owner;
    public DateTime? _lastAccessTime = null;
    public List<ICode> _codeList;
    private System.Timers.Timer _timer = null;
    private Int32 _rowCount, _columnCount;
  }
}
