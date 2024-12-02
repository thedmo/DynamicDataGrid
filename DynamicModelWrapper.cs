using System.ComponentModel;
using System.Dynamic;
using System.Reflection;

namespace DataGridDynamicTest {
	public class DynamicModelWrapper : DynamicObject, INotifyPropertyChanged {
		private readonly object _model;
		private readonly Dictionary<string, PropertyInfo> _properties;
		private readonly Dictionary<string, (object Source, PropertyInfo Property)> _additionalProperties;
		public DynamicModelWrapper(object model) {
			_model = model;
			_properties = model.GetType().GetProperties().ToDictionary(p => p.Name);
			_additionalProperties = new Dictionary<string, (object, PropertyInfo)>();
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		protected virtual void OnPropertyChanged(string propertyName) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public void AddProperty(object source, string customName, string propertyName) {
			if (_properties.ContainsKey(propertyName) || _additionalProperties.ContainsKey(propertyName)) {
				throw new ArgumentException($"Property '{customName}' already exists.");
			}

			var property = source.GetType().GetProperty(propertyName);
			if (property == null) {
				throw new ArgumentException($"Property '{propertyName}' not found on the provided object.");
			}

			_additionalProperties[customName] = (source, property);
			OnPropertyChanged(customName);
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result) {
			if (_properties.TryGetValue(binder.Name, out var modelProp)) {
				result = modelProp.GetValue(_model);
				return true;
			}
			if (_additionalProperties.TryGetValue(binder.Name, out var additionalProp)) {
				result = additionalProp.Property.GetValue(additionalProp.Source);
				return true;
			}
			result = null;
			return false;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value) {
			(object Source, PropertyInfo Property) addedProperty = new();

			if (!_properties.TryGetValue(binder.Name, out var property) && !_additionalProperties.TryGetValue(binder.Name, out addedProperty))
				return false;

			if (property != null)

				return SetValue(binder, _model, value, property);
			else if (addedProperty.Source is not null && addedProperty.Property is not null)
				return SetValue(binder, addedProperty.Source, value, addedProperty.Property);
			else
				return false;
		}

		private bool SetValue(SetMemberBinder binder, object model, object value, PropertyInfo? property) {
			object convertedValue = value;

			// int
			if (property.PropertyType == typeof(int) && value is string s) {
				if (int.TryParse(s, out int intVal)) {
					convertedValue = intVal;
				}
				else {
					return false; // Parsing failed
				}
			}

			// decimal
			if (property.PropertyType == typeof(decimal) && value is string ds) {
				if (decimal.TryParse(ds, out decimal decVal)) {
					convertedValue = decVal;
				}
				else {
					return false; // Parsing failed
				}
			}

			// double
			if (property.PropertyType == typeof(double) && value is string stringValue) {
				if (double.TryParse(stringValue, out double doubleValue)) {
					convertedValue = doubleValue;
				}
				else {
					return false; // Parsing failed
				}
			}

			// enum
			if (property.PropertyType.IsEnum && value != null) {
				Type pType = property.PropertyType;
				

				convertedValue = Enum.ToObject(property.PropertyType, value);
			}

			property.SetValue(model, convertedValue);
			if (model is IPropChange notifyPropertyChanged) {
				notifyPropertyChanged.OnPropertyChanged(_model, new PropertyChangedEventArgs(binder.Name));
			}
			return true;
		}

		public Dictionary<string, Type> GetProperties() {
			var properties = new Dictionary<string, Type>();

			// Get the properties of the wrapped model
			if (_properties != null) {
				foreach (var prop in _properties) {
					properties[prop.Key] = prop.Value.PropertyType;
				}
			}

			// Get the additional properties
			if (_additionalProperties != null) {
				foreach (var kvp in _additionalProperties) {
					properties[kvp.Key] = kvp.Value.Property.PropertyType;
				}
			}

			return properties;

		}
	}
}
