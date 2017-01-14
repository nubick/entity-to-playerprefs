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

            if(fieldType.IsEnum)
                return GetEnumFieldHandler(fieldType);
                
            throw new Exception("Not supported field type: " + fieldType);
        }
    }
}
