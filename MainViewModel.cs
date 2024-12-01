using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace DataGridDynamicTest
{
    public class MainViewModel : INotifyPropertyChanged
    {

        public ObservableCollection<dynamic> Dynamos { get; set; }

        public List<SomeModelItem> Items { get; set; }

        private List<SomeOtherModel> OtherItems;

        public ICommand resetCommand { get; }

        public MainViewModel()
        {
            Dynamos = new ObservableCollection<dynamic>();
            Items = new List<SomeModelItem>();

            ResetTheList();

            resetCommand = new Command(ResetTheList);
        }



        public event PropertyChangedEventHandler? PropertyChanged;


        public void ResetTheList()
        {
            Items = new List<SomeModelItem>() {
                new SomeModelItem { IsChecked = true, Name = "Item 1", Color = Color.FromRgb(119, 14, 253), Parameter = 30.39, SomeEnum = SomeEnum.optionOne},
                new SomeModelItem { IsChecked = false,Name = "Item 2", Color = Color.FromRgb(137, 87, 95), Parameter = 41.32, SomeEnum = SomeEnum.optionThree },
                new SomeModelItem { IsChecked = true, Name = "Item 3", Color = Color.FromRgb(244, 6, 155), Parameter = 94.69, SomeEnum = SomeEnum.optionTwo },
                new SomeModelItem { IsChecked = false,Name = "Item 4", Color = Color.FromRgb(118, 106, 30), Parameter = 50.52, SomeEnum = SomeEnum.optionTwo },
                new SomeModelItem { IsChecked = true, Name = "Item 5", Color = Color.FromRgb(105, 209, 228), Parameter = 56.87, SomeEnum = SomeEnum.optionTwo },
                new SomeModelItem { IsChecked = false,Name = "Item 6", Color = Color.FromRgb(74, 199, 54), Parameter = 80.68, SomeEnum = SomeEnum.optionOne },
                new SomeModelItem {IsChecked = true, Name = "Item 7", Color = Color.FromRgb(219, 26, 236), Parameter = 95.91, SomeEnum = SomeEnum.optionThree },
                new SomeModelItem {IsChecked = false,Name = "Item 8", Color = Color.FromRgb(72, 194, 227), Parameter = 13.77, SomeEnum = SomeEnum.optionThree },
                new SomeModelItem {IsChecked = true, Name = "Item 9", Color = Color.FromRgb(121, 154, 213), Parameter = 12.98, SomeEnum = SomeEnum.optionTwo },
                new SomeModelItem {IsChecked = false,Name = "Item 10", Color = Color.FromRgb(36, 109, 123), Parameter = 8.44, SomeEnum = SomeEnum.optionOne }
            };

            OtherItems = new List<SomeOtherModel>()
            {
                new SomeOtherModel {SomeInt1 = 1},
                new SomeOtherModel {SomeInt1 = 132},
                new SomeOtherModel {SomeInt1 = 23},
                new SomeOtherModel {SomeInt1 = 532},
                new SomeOtherModel {SomeInt1 = 12},
                new SomeOtherModel {SomeInt1 = 23},
                new SomeOtherModel {SomeInt1 = 634},
                new SomeOtherModel {SomeInt1 = 56 },
                new SomeOtherModel {SomeInt1 = 23},
                new SomeOtherModel {SomeInt1 = 4},
            };

            var dinamics = DynamicFactory.CreateDynamicObjects(Items);

            for (int i = 0; i < dinamics.Count; i++)
            {
                var dyn = dinamics[i] as DynamicModelWrapper;

                var item = OtherItems[i];

                var propName = nameof(item.SomeInt1);


                dyn.AddProperty(OtherItems[i], propName);
            }



            Dynamos = new(dinamics);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
