using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zehong.CShart.Solution.HelperLib;

namespace Zehong.CSharp.Solution.DBHandler
{
  public interface ICompanyGroup : IDatabaseTable
  {
    String GroupName { get; }
    String Location { get; }
    CountryNames Country { get; }
    String Description { get; set; }
    List<ICompany> CompanyList { get; }

    void AddCompany(ICompany company);
  }
  public class CompanyGroup : DatabaseTable, ICompanyGroup
  {
    public CompanyGroup(String groupName, String location, CountryNames country)
    {
      _groupName = groupName;
      _location = location;
      _country = country;
    }
    public void AddCompany(ICompany company)
    {
      lock (CompanyList)
      {
        if (company != null)
          this.CompanyList.Add(company);
      }
    }

    #region Properties
    public String GroupName { get { return _groupName; } }
    public String Location { get { return _location; } }
    public CountryNames Country { get { return _country; } }
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
    public List<ICompany> CompanyList
    {
      get
      {
        if (_companyList == null)
          _companyList = new List<ICompany>();
        return _companyList;
      }
    }

    #endregion

    #region Variables
    private String _groupName;
    private String _location;
    private CountryNames _country;
    private String _description;
    private List<ICompany> _companyList;

    #endregion
  }
}
