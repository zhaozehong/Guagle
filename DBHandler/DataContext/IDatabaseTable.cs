using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zehong.CShart.Solution.HelperLib;

namespace Zehong.CSharp.Solution.DBHandler
{
  public interface IDatabaseTable
  {
    Int32 SID { get; }
  }
  public abstract class DatabaseTable : NotifyPropertyChangedImp, IDatabaseTable
  {
    public DatabaseTable()
    {
      this.SID = RefreshCurrentID();
    }
    private Int32 RefreshCurrentID()
    {
      var type = this.GetType();
      var currentID = RegHandler.GetRegValue("GaugleCurrentIDs", String.Format("CurrentID_{0}", type.Name), 0) + 1;
      RegHandler.SetRegValue("GaugleCurrentIDs", String.Format("CurrentID_{0}", type.Name), currentID);
      return currentID;
    }

    public Int32 SID { get; private set; }
  }
}
