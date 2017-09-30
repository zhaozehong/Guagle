using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Zehong.CShart.Solution.HelperLib;

namespace CSharp.Solution.Guagle
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public App()
    {
      Helper.ApplicationType = ApplicationTypes.Gaugle;
    }
  }
}
