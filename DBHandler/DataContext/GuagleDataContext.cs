using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Zehong.CShart.Solution.HelperLib;
namespace Zehong.CSharp.Solution.DBHandler
{
  public class GuagleDataContext
  {
    public GuagleDataContext(Int16 rowCount, Int16 columnCount, Int16 codeLength)
    {
      this.GroupList.Add(this.CreateMaotaiGroup(rowCount, columnCount, codeLength));
      this.GroupList.Add(this.CreateWuliangyeGroup(rowCount, columnCount, codeLength));
      this.GroupList.Add(this.CreateHexagonGroup(rowCount, columnCount, codeLength));
    }
    private ICompanyGroup CreateMaotaiGroup(Int16 rowCount, Int16 columnCount, Int16 codeLength)
    {
      var group = new CompanyGroup("茅台集团", "贵州省", CountryNames.China);
      var company = new Company(group, "茅台镇酒厂", "贵州省茅台镇");
      var product = new Product(company, "茅台国酒");
      for (int i = 1; i <= 12; i++)
      {
        var lot = new Lot(product, String.Format("第{0}批", i));
        var nCount = Helper.RandomObj.Next(100);
        while (nCount == 0)
        {
          nCount = Helper.RandomObj.Next(100);
        }
        var startDate = new DateTime(2016, i, 1);
        for (int n = 1; n < nCount; n++)
        {
          var nAddDays = Helper.RandomObj.Next(30);
          var transaction = new Transaction(lot, startDate.AddDays(nAddDays), rowCount, columnCount, codeLength);
          lot.AddTransaction(transaction);
        }
        product.AddLot(lot);
      }
      company.AddProduct(product);
      group.AddCompany(company);

      return group;
    }
    private ICompanyGroup CreateWuliangyeGroup(Int16 rowCount, Int16 columnCount, Int16 codeLength)
    {
      var group = new CompanyGroup("五粮液集团", "四川省", CountryNames.China);
      var company = new Company(group, "五粮液酿酒基地", "四川省宜宾市");
      var product = new Product(company, "五粮液国宾酒");
      for (int i = 1; i <= 12; i++)
      {
        var lot = new Lot(product, String.Format("第{0}批", i));
        var nCount = Helper.RandomObj.Next(100);
        while (nCount == 0)
        {
          nCount = Helper.RandomObj.Next(100);
        }
        var startDate = new DateTime(2001, i, 1);
        for (int n = 1; n < nCount; n++)
        {
          var nAddDays = Helper.RandomObj.Next(30);
          var transaction = new Transaction(lot, startDate.AddDays(nAddDays), rowCount, columnCount, codeLength);
          lot.AddTransaction(transaction);
        }
        product.AddLot(lot);
      }
      company.AddProduct(product);
      group.AddCompany(company);

      return group;
    }
    private ICompanyGroup CreateHexagonGroup(Int16 rowCount, Int16 columnCount, Int16 codeLength)
    {
      var group = new CompanyGroup("海克斯康集团", "斯德哥尔摩市", CountryNames.Sweden);
      var companyList = new List<Company>();
      companyList.Add(new Company(group, "海克斯康测量技术（青岛）有限公司", "山东省青岛市"));
      companyList.Add(new Company(group, "海克斯康测量技术（巴西）有限公司", "布宜诺斯艾利斯"));
      foreach (var company in companyList)
      {
        var product = new Product(company, "Portable");
        for (int i = 1; i <= 9; i++)
        {
          var lot = new Lot(product, String.Format("第{0}批", i));
          var nCount = Helper.RandomObj.Next(100);
          while (nCount == 0)
          {
            nCount = Helper.RandomObj.Next(100);
          }
          var startDate = new DateTime(2017, i, 1);
          for (int n = 1; n < nCount; n++)
          {
            var nAddDays = Helper.RandomObj.Next(30);
            var transaction = new Transaction(lot, startDate.AddDays(nAddDays), rowCount, columnCount, codeLength);
            lot.AddTransaction(transaction);
          }
          product.AddLot(lot);
        }
        company.AddProduct(product);
        group.AddCompany(company);
      }

      return group;
    }



    public String Request(String strRequest)
    {
      var verifier = this.ParseVerfication(strRequest);
      if (verifier == null)
        return null;
      return verifier.Request();
    }
    public VerificationResult Verify(String strRequest, String strVerification)
    {
      var verifier = this.ParseVerfication(strRequest);
      if (verifier == null)
        return null;
      if (!verifier.Verify(strVerification))
        return new VerificationResult();
      return new VerificationResult(verifier.Owner);
    }

    private IVerification ParseVerfication(String strRequest)
    {
      try
      {
        var intList = Helper.StringToIntList(strRequest, ";");
        if (intList.Count != 5)
          return null;

        var groupId = intList[0];
        var companyId = intList[1];
        var productId = intList[2];
        var lotId = intList[3];
        var transactionId = intList[4];

        var group = this.GroupList.FirstOrDefault(p => p.SID == groupId);
        if (group == null)
          return null;

        var company = group.CompanyList.FirstOrDefault(p => p.SID == companyId);
        if (company == null)
          return null;

        var product = company.ProductList.FirstOrDefault(p => p.SID == productId);
        if (product == null)
          return null;

        var lot = product.LotList.FirstOrDefault(p => p.SID == lotId);
        if (lot == null)
          return null;

        var transaction = lot.TransactionList.FirstOrDefault(p => p.SID == transactionId);
        if (transaction == null)
          return null;

        return transaction.Verfication;
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
        return null;
      }
    }

    public List<ICompanyGroup> GroupList = new List<ICompanyGroup>();
  }
}
