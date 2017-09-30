using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zehong.CShart.Solution.HelperLib;

namespace Zehong.CSharp.Solution.DBHandler
{
  public interface ITransaction : IDatabaseTable
  {
    ILot Owner { get; }
    DateTime TransactionDate { get; }
    IVerification Verfication { get; }

    String GetDetailedInformation();
  }
  public class Transaction : DatabaseTable, ITransaction
  {
    public Transaction(ILot owner, DateTime transactionDate, Int16 rowCount, Int16 columnCount, Int16 codeLength)
    {
      this._owner = owner;
      this._transactionDate = transactionDate;
      this._verfication = new CodeCard(this, rowCount, columnCount, codeLength);
    }
    public String GetDetailedInformation()
    {
      var info = String.Format("This is a [{0}].\r\nIt's manufactured by [{1}] of [{2}] on {3}.\r\nOrigin:{4}",
        this.Product.ProductName,
        this.Company.CompanyName,
        this.CompanyGroup.GroupName,
        this.TransactionDate.ToString("yyyy-MM-dd"),
        this.Company.Location);
      return info;
    }

    #region Properties
    public ILot Owner { get { return _owner; } }
    public DateTime TransactionDate { get { return _transactionDate; } }
    public IVerification Verfication
    {
      get { return _verfication; }
      set
      {
        if (!String.Equals(_verfication, value))
        {
          _verfication = value;
          this.SendPropertyChanged(() => Verfication);
        }
      }
    }

    private ICompanyGroup CompanyGroup { get { return this.Owner.Owner.Owner.Owner; } }
    private ICompany Company { get { return this.Owner.Owner.Owner; } }
    private IProduct Product { get { return this.Owner.Owner; } }
    private String LotID { get { return this.Owner.LotID; } }
    #endregion

    #region Variables
    private ILot _owner;
    private DateTime _transactionDate;
    private IVerification _verfication;

    #endregion
  }
}
