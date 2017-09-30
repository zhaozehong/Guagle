using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zehong.CShart.Solution.HelperLib;

namespace Zehong.CSharp.Solution.DBHandler
{
  public interface ILot : IDatabaseTable
  {
    IProduct Owner { get; }
    String LotID { get; }
    String Description { get; set; }
    List<ITransaction> TransactionList { get; }

    void AddTransaction(ITransaction transaction);
  }
  public class Lot : DatabaseTable, ILot
  {
    public Lot(IProduct owner, String lotID)
    {
      this._owner = owner;
      this._lotID = lotID;
    }
    public void AddTransaction(ITransaction transaction)
    {
      lock (TransactionList)
      {
        if (transaction != null)
          this.TransactionList.Add(transaction);
      }
    }

    #region Properties
    public IProduct Owner { get { return _owner; } }
    public String LotID { get { return _lotID; } }
    public String Description
    {
      get { return _description; }
      set
      {
        if (!String.Equals(_description, value))
        {
          _description = value;
          this.SendPropertyChanged(() => Description);
        }
      }
    }
    public List<ITransaction> TransactionList
    {
      get
      {
        if (_transactionList == null)
          _transactionList = new List<ITransaction>();
        return _transactionList;
      }
    }

    #endregion

    #region Variables
    private IProduct _owner;
    private String _lotID;
    private String _description;
    private List<ITransaction> _transactionList;

    #endregion
  }
}
