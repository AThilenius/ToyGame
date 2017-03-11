using System.ComponentModel;
using PropertyChanged;

namespace ToyGame.Editor
{
  [ImplementPropertyChanged]
  internal class BaseViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
  }
}