using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Zehong.CShart.Solution.UserControls
{
  public delegate void CodeChangedEvent(String code);
  public partial class CoveredItem : UserControl
  {
    public CoveredItem(String code)
    {
      this.Code = code;
      InitializeComponent();
    }

    private void tbCover_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      var tb = sender as TextBlock;
      if (tb != null && e.ClickCount > 1)
      {
        tb.Background = Brushes.Transparent;

        if (CodeChanged != null)
          CodeChanged(this.Code);
      }
    }

    public event CodeChangedEvent CodeChanged;

    public String Code { get; private set; }
  }
}
