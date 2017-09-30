using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zehong.CShart.Solution.HelperLib;

namespace Zehong.CSharp.Solution.DBHandler
{
  public interface IProduct : IDatabaseTable
  {
    ICompany Owner { get; }
    String ProductName { get; }
    String ProductID { get; set; }
    String Description { get; set; }
    object ProductImage { get; set; }
    List<String> GridiantList { get; set; }
    List<ILot> LotList { get; }

    void AddGridant(String strGridient);
    void RefreshGridants(List<String> gridients);
    void AddLot(ILot lot);
  }
  public class Product : DatabaseTable, IProduct
  {
    public Product(ICompany owner, String productName)
    {
      this._owner = owner;
      this._productName = productName;
    }
    public void AddGridant(String strGridient)
    {
      lock (GridiantList)
      {
        if (!String.IsNullOrWhiteSpace(strGridient))
          this.GridiantList.Add(strGridient);
      }
    }
    public void RefreshGridants(List<String> gridients)
    {
      lock (GridiantList)
      {
        this.GridiantList.Clear();
        if (gridients != null)
          this.GridiantList.AddRange(gridients.Where(p => !String.IsNullOrWhiteSpace(p)));
      }
    }
    public void AddLot(ILot lot)
    {
      lock (LotList)
      {
        if (lot != null)
          this.LotList.Add(lot);
      }
    }

    #region Properties
    public ICompany Owner { get { return _owner; } }
    public String ProductName { get { return _productName; } }
    public String ProductID
    {
      get { return _productID; }
      set
      {
        if (!String.Equals(_productID, value))
        {
          _productID = value;
          this.SendPropertyChanged(() => ProductID);
        }
      }
    }
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
    public object ProductImage
    {
      get { return _productImage; }
      set
      {
        if (!object.Equals(_productImage, value))
        {
          _productImage = value;
          this.SendPropertyChanged(() => ProductImage);
        }
      }
    }
    public List<String> GridiantList
    {
      get
      {
        if (_gridiantList == null)
          _gridiantList = new List<String>();
        return _gridiantList;
      }
      set
      {
        if (!object.Equals(_gridiantList, value))
        {
          this._gridiantList = value;
          this.SendPropertyChanged(() => GridiantList);
        }
      }
    }
    public List<ILot> LotList
    {
      get
      {
        if (_lotList == null)
          _lotList = new List<ILot>();
        return _lotList;
      }
    }

    #endregion

    #region Variables
    private ICompany _owner;
    private String _productName;
    private String _productID;
    private object _productImage;
    private String _description;
    private List<String> _gridiantList;
    private List<ILot> _lotList;

    #endregion
  }
}
