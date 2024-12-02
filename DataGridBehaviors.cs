using System.Dynamic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace DataGridDynamicTest {
	public static class DataGridBehaviors {
		public static readonly DependencyProperty EnableAutoColumnsProperty =
			DependencyProperty.RegisterAttached(
				"EnableAutoColumns",
				typeof(bool),
				typeof(DataGridBehaviors),
				new PropertyMetadata(false, OnEnableAutoColumnsChanged));

		public static bool GetEnableAutoColumns(DependencyObject obj) {
			return (bool)obj.GetValue(EnableAutoColumnsProperty);
		}

		public static void SetEnableAutoColumns(DependencyObject obj, bool value) {
			obj.SetValue(EnableAutoColumnsProperty, value);
		}

		private static void OnEnableAutoColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (d is DataGrid dataGrid) {
				if ((bool)e.NewValue) {
					dataGrid.AutoGenerateColumns = false;
					dataGrid.Loaded += DataGrid_Loaded;
					dataGrid.SourceUpdated += DataGrid_ItemsSourceChanged;
				}
				else {
					dataGrid.Loaded -= DataGrid_Loaded;
					dataGrid.SourceUpdated -= DataGrid_ItemsSourceChanged;
				}
			}
		}

		private static void DataGrid_Loaded(object sender, RoutedEventArgs e) {
			if (sender is DataGrid dataGrid) {
				GenerateColumns(dataGrid);
			}
		}

		private static void DataGrid_ItemsSourceChanged(object sender, DataTransferEventArgs e) {
			if (sender is DataGrid dataGrid) {
				GenerateColumns(dataGrid);
			}
		}

		private static void GenerateColumns(DataGrid dataGrid) {
			if (dataGrid.ItemsSource == null) return;

			dataGrid.Columns.Clear();

			if (dataGrid.ItemsSource is not ICollection<DynamicModelWrapper> items || items == null || items.Count == 0)
				return;

			var properties = items.First().GetProperties();

			foreach (var prop in properties) {
				var column = CreateColumn(prop.Key, prop.Value);
				if (column != null) {
					dataGrid.Columns.Add(column);
				}
			}
		}


		private static DataGridColumn CreateColumn(string propertyName, Type propertyType) {
			if (propertyType == typeof(bool)) {
				return new DataGridCheckBoxColumn {
					Header = propertyName,
					Binding = new Binding(propertyName) { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged }
				};
			}
			else if (propertyType.IsEnum) {
				return new DataGridComboBoxColumn {
					Header = propertyName,
					ItemsSource = Enum.GetValues(propertyType),
					SelectedItemBinding = new Binding(propertyName) { Mode = BindingMode.TwoWay }
				};
			}
			else if (propertyType == typeof(Color)) {
				return new DataGridTemplateColumn {
					Header = propertyName,
					CellTemplate = CreateColorPickerTemplate(propertyName)
				};
			}
			else {
				return new DataGridTextColumn {
					Header = propertyName,
					Binding = new Binding(propertyName) { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.LostFocus }
				};
			}
		}
		private static DataTemplate CreateColorPickerTemplate(string propertyName) {
			var template = new DataTemplate();
			var factory = new FrameworkElementFactory(typeof(ColorPicker));
			factory.SetBinding(ColorPicker.SelectedColorProperty, new Binding(propertyName) { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
			factory.SetValue(ColorPicker.ShowStandardColorsProperty, true);
			factory.SetValue(ColorPicker.ShowRecentColorsProperty, false);
			template.VisualTree = factory;
			return template;
		}

		private class DynamicGetMemberBinder : GetMemberBinder {
			public DynamicGetMemberBinder(string name) : base(name, false) { }

			public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion) {
				throw new NotImplementedException();
			}
		}
	}
}