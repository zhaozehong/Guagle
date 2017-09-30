using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zehong.CShart.Solution.HelperLib;

namespace Zehong.CSharp.Solution.DBHandler
{
  public interface ICompany : IDatabaseTable
  {
    ICompanyGroup Owner { get; }
    String CompanyName { get; }
    String Location { get; }
    String Description { get; set; }
    List<IProduct> ProductList { get; }

    void AddProduct(IProduct product);
  }
  public class Company : DatabaseTable, ICompany
  {
    public Company(ICompanyGroup owner, String companyName, String location)
    {
      this._owner = owner;
      this._companyName = companyName;
      this._location = location;
    }
    public void AddProduct(IProduct product)
    {
      lock (ProductList)
      {
        if (product != null)
          this.ProductList.Add(product);
      }
    }

    #region Properties
    public ICompanyGroup Owner { get { return _owner; } }
    public String CompanyName { get { return _companyName; } }
    public String Location { get { return _location; } }
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
    public List<IProduct> ProductList
    {
      get
      {
        if (_productList == null)
          _productList = new List<IProduct>();
        return _productList;
      }
    }

    #endregion

    #region Variables
    private ICompanyGroup _owner;
    private String _companyName;
    private String _location;
    private String _description;
    private List<IProduct> _productList;

    #endregion
  }
}
