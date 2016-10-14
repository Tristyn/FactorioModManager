using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;

namespace FactorioModManager.UI.Framework
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        public void Notify([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Update<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, value))
            {
                field = value;
                // ReSharper disable once ExplicitCallerInfoArgument
                Notify(propertyName);
                return true;
            }
            return false;
        }
    }
}
