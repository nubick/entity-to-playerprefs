using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Plugins.EntityToPlayerPrefs.FieldHandlers
{
    public class PlayerPrefsFieldFactory
    {
        private static StringFieldHandler _stringFieldHandler;
        private static StringFieldHandler StringFieldHandler
        {
            get
            {
                if(_stringFieldHandler == null)
                    _stringFieldHandler = new StringFieldHandler();
                return _stringFieldHandler;
            }
        }

        private static BoolFieldHandler _boolFieldHandler;
        private static BoolFieldHandler BoolFieldHandler
        {
            get
            {
                if(_boolFieldHandler == null)
                    _boolFieldHandler = new BoolFieldHandler();
                return _boolFieldHandler;
            }
        }

        private static IntFieldHandler _intFieldHandler;
        private static IntFieldHandler IntFieldHandler
        {
            get
            {
                if(_intFieldHandler == null)
                    _intFieldHandler = new IntFieldHandler();
                return _intFieldHandler;
            }
        }

        private static FloatFieldHandler _floatFieldHandler;
        private static FloatFieldHandler FloatFieldHandler
        {
            get
            {
                if(_floatFieldHandler == null)
                    _floatFieldHandler = new FloatFieldHandler();
                return _floatFieldHandler;
            }
        }

        private static DateTimeFieldHandler _dateTimeFieldHandler;
        private static DateTimeFieldHandler DateTimeFieldHandler
        {
            get
            {
                if(_dateTimeFieldHandler == null)
                    _dateTimeFieldHandler = new DateTimeFieldHandler();
                return _dateTimeFieldHandler;
            }
        }
        
        private static Vector2FieldHandler _vector2FieldHandler;
        private static Vector2FieldHandler Vector2FieldHandler
        {
            get
            {
                if (_vector2FieldHandler == null)
                    _vector2FieldHandler = new Vector2FieldHandler();
                return _vector2FieldHandler;
            }
        }

        private static Vector3FieldHandler _vector3FieldHandler;
        private static Vector3FieldHandler Vector3FieldHandler
        {
            get
            {
                if (_vector3FieldHandler == null)
                    _vector3FieldHandler = new Vector3FieldHandler();
                return _vector3FieldHandler;
            }
        }

        private static ListOfIntsHandler _listOfIntsHandler;
        private static ListOfIntsHandler ListOfIntsHandler
        {
            get
            {
                if (_listOfIntsHandler == null)
                    _listOfIntsHandler = new ListOfIntsHandler();
                return _listOfIntsHandler;
            }
        }

        private static ListOfStringsHandler _listOfStringsHandler;
        private static ListOfStringsHandler ListOfStringsHandler
        {
            get
            {
                if (_listOfStringsHandler == null)
                    _listOfStringsHandler = new ListOfStringsHandler();
                return _listOfStringsHandler;
            }
        }
        
        private static Dictionary<Type, EnumFieldHandler> _enumFieldHandlers = new Dictionary<Type, EnumFieldHandler>();
        private static EnumFieldHandler GetEnumFieldHandler(Type enumType)
        {
            if(!_enumFieldHandlers.ContainsKey(enumType))
                _enumFieldHandlers.Add(enumType, new EnumFieldHandler(enumType));
            return _enumFieldHandlers[enumType];
        }
        
        public static PlayerPrefsFieldHandler Get(Type fieldType)
        {
            if (fieldType == typeof (string))
                return StringFieldHandler;

            if (fieldType == typeof (bool))
                return BoolFieldHandler;

            if (fieldType == typeof (int))
                return IntFieldHandler;

            if (fieldType == typeof (float))
                return FloatFieldHandler;

            if (fieldType == typeof (DateTime))
                return DateTimeFieldHandler;

            if (fieldType == typeof(Vector2))
                return Vector2FieldHandler;

            if (fieldType == typeof(Vector3))
                return Vector3FieldHandler;

            if (fieldType == typeof(List<int>))
                return ListOfIntsHandler;

            if (fieldType == typeof(List<string>))
                return ListOfStringsHandler;

            if(fieldType.IsEnum)
                return GetEnumFieldHandler(fieldType);
                
            throw new Exception("Not supported field type: " + fieldType);
        }
    }
}
