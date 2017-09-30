using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Zehong.CShart.Solution.HelperLib
{
  public abstract class NotifyPropertyChangedImp : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    virtual public void SendPropertyChanged(String propertyName)
    {
      PropertyChangedEventHandler propertychanged = null;
      lock (this)
      {
        propertychanged = PropertyChanged;
      }
      if (propertychanged != null)
        propertychanged(this, new PropertyChangedEventArgs(propertyName));
    }
    public void SendPropertyChanged<TProperty>(Expression<Func<TProperty>> projection)
    {
      var memberExpression = (MemberExpression)projection.Body;
      this.SendPropertyChanged(memberExpression.Member.Name);
    }
    public string GetPropertyName<TProperty>(Expression<Func<TProperty>> projection)
    {
      var memberExpression = (MemberExpression)projection.Body;
      return (memberExpression.Member.Name);
    }
  }
}
