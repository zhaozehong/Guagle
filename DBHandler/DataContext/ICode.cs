using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zehong.CShart.Solution.HelperLib;

namespace Zehong.CSharp.Solution.DBHandler
{
  public interface ICode : IDatabaseTable
  {
    CodeStatus Status { get; }

    CodeRequestResults Request();
    Boolean Verify(String strValue);
  }
  public class GuagleCode : DatabaseTable, ICode
  {
    public GuagleCode(Int16 length)
    {
      _codeText = this.CreateRandomString(length);
    }
    public CodeRequestResults Request()
    {
      if (this.Status.HasFlag(CodeStatus.Verified))
        return CodeRequestResults.HasVerified;

      if (this.Status.HasFlag(CodeStatus.Locked))
        return CodeRequestResults.HasLocked;

      this.Status |= CodeStatus.Locked;
      return CodeRequestResults.Valid;
    }
    public Boolean Verify(String strValue)
    {
      this.Status = CodeStatus.Verified;
      return String.Equals(strValue, _codeText, StringComparison.InvariantCultureIgnoreCase);
    }

    private String CreateRandomString(Int16 length)
    {
      try
      {
        var retValue = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
          var iRandom = Helper.RandomObj.Next(AvailableChars.Length);
          retValue.Append(AvailableChars[iRandom]);
        }
        return retValue.ToString();
      }
      catch (Exception ex)
      {
        ExceptionHandler.ThrowException(ex);
        return null;
      }
    }

    public String CodeText { get { return _codeText; } }
    public CodeStatus Status { get; private set; }

    private String _codeText = null;

    private static String[] AvailableChars = new String[]{"0","1","2","3","4","5","6","7","8","9",
      "a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z"};
  }
}
