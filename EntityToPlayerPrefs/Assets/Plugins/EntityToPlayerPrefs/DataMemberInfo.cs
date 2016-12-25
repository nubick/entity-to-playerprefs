using System;
using System.Reflection;

namespace Assets.Plugins.EntityToPlayerPrefs
{
    public class DataMemberInfo
    {
        private readonly FieldInfo _fieldInfo;
        private readonly PropertyInfo _propertyInfo;

        public DataMemberInfo(FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
        }

        public DataMemberInfo(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public T GetValue<T>(object entity)
        {
            object value = _fieldInfo != null ? _fieldInfo.GetValue(entity) : _propertyInfo.GetValue(entity, null);
            return (T) value;
        }

        public void SetValue(object entity, object value)
        {
            if (_fieldInfo != null)
                _fieldInfo.SetValue(entity, value);
            else
                _propertyInfo.SetValue(entity, value, null);
        }

        public string GetName()
        {
            return _fieldInfo != null ? _fieldInfo.Name : _propertyInfo.Name;
        }

        public Type GetMemberInfoType()
        {
            return _fieldInfo != null ? _fieldInfo.FieldType : _propertyInfo.PropertyType;
        }
    }
}
