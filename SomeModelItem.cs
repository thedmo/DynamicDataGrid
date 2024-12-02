using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;

namespace DataGridDynamicTest
{
    public class SomeModelItem : INotifyPropertyChanged, IPropChange
    {
        private double parameter;

        public string Name
        {
            get => name; set
            {
                name = value;
                someValueChanged(value);
            }
        }
        public Color Color { get; set; }


        private bool isChecked;
        private string name = "";
		private SomeEnum someEnum;

		public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                someValueChanged(value);
            }
        }



        public double Parameter
        {
            get => parameter;
            set
            {
                parameter = value;
                someValueChanged(value);
            }
        }

		public SomeEnum SomeEnum { 
            get => someEnum;
            set { 
                someEnum = value;
                someValueChanged(value);
            }
        }

		public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        private void someValueChanged<T>(T newValue)
        {
            if (newValue != null)
            {
                Debug.WriteLine(newValue.ToString());
            }
        }
    }
}
