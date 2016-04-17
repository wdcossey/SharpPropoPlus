using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SharpPropoPlus.Annotations;

namespace SharpPropoPlus.ViewModels
{
  public class BaseViewModel: INotifyPropertyChanged, IDisposable
  {
    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public virtual void Dispose()
    {
      
    }
  }
}