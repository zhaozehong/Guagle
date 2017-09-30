using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zehong.CSharp.Solution.DBHandler
{
  public class VerificationResult
  {
    public VerificationResult(ITransaction transaction = null)
    {
      this.Result = transaction != null;
      this.Transaction = transaction;
    }

    public Boolean Result { get; private set; }
    public ITransaction Transaction { get; private set; }
  }
}
