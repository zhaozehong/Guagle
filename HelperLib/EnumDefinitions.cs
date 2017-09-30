using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zehong.CShart.Solution.HelperLib
{
  public enum ApplicationTypes { Gaugle }
  public enum CountryNames { China, UnitedStates, Japan, Germany, Frence, England, Swiss, Sweden }
  public enum CodeSizes { Small, Normal, Large }

  [Flags]
  public enum CodeStatus { Normal = 0x00, Verified = 0x01, Locked = 0x02 }
  public enum CodeRequestResults { Valid, HasLocked, HasVerified }
}
