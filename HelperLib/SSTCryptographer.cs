using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Zehong.CShart.Solution.HelperLib
{
  public class SSTCryptographer
  {
    public static String Boxing(String strToBoxing, String strKey)
    {
      try
      {
        var objDESCrypto = new TripleDESCryptoServiceProvider();
        var objHashMD5 = new MD5CryptoServiceProvider();

        var strTempKey = strKey;
        byte[] byteHash = objHashMD5.ComputeHash(Encoding.ASCII.GetBytes(strTempKey));
        objHashMD5 = null;
        objDESCrypto.Key = byteHash;
        objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB

        var byteBuff = Encoding.UTF8.GetBytes(strToBoxing);
        return Convert.ToBase64String(objDESCrypto.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
      }
      catch (Exception ex)
      {
        return "Wrong Input. " + ex.Message;
      }
    }
    public static String Unboxing(String strToBoxing, String strKey)
    {
      try
      {
        var objDESCrypto = new TripleDESCryptoServiceProvider();
        var objHashMD5 = new MD5CryptoServiceProvider();

        var strTempKey = strKey;

        byte[] byteHash = objHashMD5.ComputeHash(Encoding.ASCII.GetBytes(strTempKey));
        objHashMD5 = null;
        objDESCrypto.Key = byteHash;
        objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB

        var byteBuff = Convert.FromBase64String(strToBoxing);
        var strDecrypted = Encoding.UTF8.GetString(objDESCrypto.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
        objDESCrypto = null;

        return strDecrypted;
      }
      catch (Exception ex)
      {
        return "Wrong Input. " + ex.Message;
      }
    }

    private String _sstName = "Zehong.Solution";
    public String SSTName
    {
      get { return _sstName; }
      set { _sstName = value; }
    }
  }
}
