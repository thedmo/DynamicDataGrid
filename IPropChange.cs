using System.ComponentModel;

namespace DataGridDynamicTest
{
    public interface IPropChange
    {
        void OnPropertyChanged(object? sender, PropertyChangedEventArgs e);
    }
}