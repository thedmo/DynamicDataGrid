using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace DataGridDynamicTest
{
    public static class DataGridBehaviors
    {
        public static readonly DependencyProperty EnableAutoColumnsProperty =
            DependencyProperty.RegisterAttached(
                "EnableAutoColumns",
                typeof(bool),
                typeof(DataGridBehaviors),
                new PropertyMetadata(false, OnEnableAutoColumnsChanged));

        public static bool GetEnableAutoColumns(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableAutoColumnsProperty);
        }

        public static void SetEnableAutoColumns(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableAutoColumnsProperty, value);
        }

        private static void OnEnableAutoColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid)
            {
                if ((bool)e.NewValue)
                {
                    dataGrid.AutoGenerateColumns = false;
                    dataGrid.Loaded += DataGrid_Loaded;
                    dataGrid.SourceUpdated += DataGrid_ItemsSourceChanged;
                }
                else
                {
                    dataGrid.Loaded -= DataGrid_Loaded;
                    dataGrid.SourceUpdated -= DataGrid_ItemsSourceChanged;
                }
            }
        }

        private static void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                GenerateColumns(dataGrid);
            }
        }

        private static void DataGrid_ItemsSourceChanged(object sender, DataTransferEventArgs e)
        {
            if (sender is DataGrid dataGrid)
            {
                GenerateColumns(dataGrid);
            }
        }

        private static void GenerateColumns(DataGrid dataGrid)
        {
            if (dataGrid.ItemsSource == null) return;

            dataGrid.Columns.Clear();

            var items = (dataGrid.ItemsSource as IEnumerable)?.Cast<object>().ToList();
            if (items == null || !items.Any()) return;

            var firstItem = items.First();


            var properties = GetProperties(firstItem);

            foreach (var prop in properties)
            {
                var column = CreateColumn(prop.Key, prop.Value);
                if (column != null)
                {
                    dataGrid.Columns.Add(column);
                }
            }
        }

        private static Dictionary<string, Type> GetProperties(object item)
        {
            if (item is DynamicModelWrapper wrapper)
            {

                var properties = new Dictionary<string, Type>();

                // Get the properties of the wrapped model
                var modelField = wrapper.GetType().GetField("_model", BindingFlags.NonPublic | BindingFlags.Instance);
                var modelProperties = modelField?.GetValue(wrapper)?.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                if (modelProperties != null)
                {
                    foreach (var prop in modelProperties)
                    {
                        properties[prop.Name] = prop.PropertyType;
                    }
                }

                // Get the additional properties
                var additionalPropertiesField = wrapper.GetType().GetField("_additionalProperties", BindingFlags.NonPublic | BindingFlags.Instance);
                var additionalProperties = additionalPropertiesField?.GetValue(wrapper) as Dictionary<string, (object Source, PropertyInfo Property)>;
                if (additionalProperties != null)
                {
                    foreach (var kvp in additionalProperties)
                    {
                        properties[kvp.Key] = kvp.Value.Property.PropertyType;
                    }
                }

                return properties;
            }

            // Fallback to getting properties directly if it's not a DynamicModelWrapper
            return item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(p => p.Name, p => p.PropertyType);
        }

        private static DataGridColumn CreateColumn(string propertyName, Type propertyType)
        {
            if (propertyType == typeof(bool))
            {
                return new DataGridCheckBoxColumn
                {
                    Header = propertyName,
                    Binding = new Binding(propertyName) { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged }
                };
            }
            else if (propertyType.IsEnum)
            {
                return new DataGridComboBoxColumn
                {
                    Header = propertyName,
                    ItemsSource = Enum.GetValues(propertyType),
                    IsReadOnly = true,
                    SelectedItemBinding = new Binding(propertyName) { Mode = BindingMode.OneWay }
                };
            }
            else if (propertyType == typeof(Color))
            {
                return new DataGridTemplateColumn
                {
                    Header = propertyName,
                    CellTemplate = CreateColorPickerTemplate(propertyName)
                };
            }
            else
            {
                return new DataGridTextColumn
                {
                    Header = propertyName,
                    Binding = new Binding(propertyName) { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.LostFocus }
                };
            }
        }
        private static DataTemplate CreateColorPickerTemplate(string propertyName)
        {
            var template = new DataTemplate();
            var factory = new FrameworkElementFactory(typeof(ColorPicker));
            factory.SetBinding(ColorPicker.SelectedColorProperty, new Binding(propertyName) { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            factory.SetValue(ColorPicker.ShowStandardColorsProperty, true);
            factory.SetValue(ColorPicker.ShowRecentColorsProperty, false);
            template.VisualTree = factory;
            return template;
        }

        private class DynamicGetMemberBinder : GetMemberBinder
        {
            public DynamicGetMemberBinder(string name) : base(name, false) { }

            public override DynamicMetaObject FallbackGetMember(DynamicMetaObject target, DynamicMetaObject errorSuggestion)
            {
                throw new NotImplementedException();
            }
        }
    }
}