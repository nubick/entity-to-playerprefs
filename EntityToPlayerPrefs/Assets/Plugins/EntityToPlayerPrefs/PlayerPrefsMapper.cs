using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Plugins.EntityToPlayerPrefs
{
    public class PlayerPrefsMapper
    {
        public const string EntityKeyPrefix = "__entity";
        private const string SingleEntityId = "__single";

        private static string GetEntityId(object entity)
        {
            Type entityType = entity.GetType();
            List<DataMemberInfo> dataMemberInfos = GetDataMemberInfoWithAttribute<PlayerPrefsEntityIdAttribute>(entityType);

            if (dataMemberInfos.Count == 0)
                return SingleEntityId;

            if (dataMemberInfos.Count > 1)
                throw new Exception(string.Format("Entity {0} contains more than one EntityId attribute.", entityType));

            return dataMemberInfos[0].GetValue<string>(entity);
        }

        private static void SetEntityId(object entity, string entityId)
        {
            Type entityType = entity.GetType();
            List<DataMemberInfo> dataMemberInfos = GetDataMemberInfoWithAttribute<PlayerPrefsEntityIdAttribute>(entityType);

            if (dataMemberInfos.Count == 0)
                throw new Exception(string.Format("Entity {0} doesn't contain EntityId attribute.", entityType));

            if (dataMemberInfos.Count > 1)
                throw new Exception(string.Format("Entity {0} contains more than one EntityId attribute.", entityType));

            dataMemberInfos[0].SetValue(entity, entityId);
        }

        private static List<DataMemberInfo> GetDataMemberInfoWithAttribute<TAttribute>(Type type)
            where TAttribute : Attribute
        {
            List<DataMemberInfo> dataMemberInfos = new List<DataMemberInfo>();

            dataMemberInfos.AddRange(type.GetFields()
                .Where(fi => fi.GetCustomAttributes(typeof (TAttribute), true).Any())
                .Select(fieldInfo => new DataMemberInfo(fieldInfo)));

            dataMemberInfos.AddRange(type.GetProperties()
                .Where(pi => pi.GetCustomAttributes(typeof (TAttribute), true).Any())
                .Select(propertyInfo => new DataMemberInfo(propertyInfo)));

            return dataMemberInfos;
        }

        private static string GetFieldKey(string entityId, Type entityType, DataMemberInfo dataMemberInfo)
        {
            return string.Format("{0}.{1}.{2}.{3}", EntityKeyPrefix, entityType.Name, entityId, dataMemberInfo.GetName());
        }

        public static void Save(object entity)
        {
            string entityId = GetEntityId(entity);
            Type entityType = entity.GetType();
            List<DataMemberInfo> dataMemberInfos = GetDataMemberInfoWithAttribute<PlayerPrefsFieldAttribute>(entityType);
            foreach (DataMemberInfo dataMemberInfo in dataMemberInfos)
            {
                string fieldKey = GetFieldKey(entityId, entityType, dataMemberInfo);

                if (dataMemberInfo.GetMemberInfoType() == typeof (string))
                {
                    PlayerPrefs.SetString(fieldKey, dataMemberInfo.GetValue<string>(entity));
                }
                else if (dataMemberInfo.GetMemberInfoType() == typeof(bool))
                {
                    PlayerPrefs.SetString(fieldKey, dataMemberInfo.GetValue<bool>(entity).ToString());
                }
                else if (dataMemberInfo.GetMemberInfoType() == typeof (int))
                {
                    PlayerPrefs.SetInt(fieldKey, dataMemberInfo.GetValue<int>(entity));
                }
                else if (dataMemberInfo.GetMemberInfoType() == typeof (float))
                {
                    PlayerPrefs.SetFloat(fieldKey, dataMemberInfo.GetValue<float>(entity));
                }
                else
                {
                    throw new Exception("Not supported filed type: " + dataMemberInfo.GetMemberInfoType());
                }
            }
            PlayerPrefs.Save();
        }

        public static void Load(object entity)
        {
            string entityId = GetEntityId(entity);
            Load(entity, entityId);
        }

        public static void Load(object entity, string entityId)
        {
            Type entityType = entity.GetType();
            List<DataMemberInfo> dataMemberInfos = GetDataMemberInfoWithAttribute<PlayerPrefsFieldAttribute>(entityType);
            foreach (DataMemberInfo dataMemberInfo in dataMemberInfos)
            {
                string fieldKey = GetFieldKey(entityId, entityType, dataMemberInfo);
                if (PlayerPrefs.HasKey(fieldKey))
                {
                    if (dataMemberInfo.GetMemberInfoType() == typeof(string))
                    {
                        string strValue = PlayerPrefs.GetString(fieldKey);
                        dataMemberInfo.SetValue(entity, strValue);
                    }
                    else if (dataMemberInfo.GetMemberInfoType() == typeof(bool))
                    {
                        bool boolValue = bool.Parse(PlayerPrefs.GetString(fieldKey));
                        dataMemberInfo.SetValue(entity, boolValue);
                    }
                    else if (dataMemberInfo.GetMemberInfoType() == typeof(int))
                    {
                        int intValue = PlayerPrefs.GetInt(fieldKey);
                        dataMemberInfo.SetValue(entity, intValue);
                    }
                    else if (dataMemberInfo.GetMemberInfoType() == typeof (float))
                    {
                        float floatValue = PlayerPrefs.GetFloat(fieldKey);
                        dataMemberInfo.SetValue(entity, floatValue);
                    }
                    else
                    {
                        throw new Exception("Not supported filed type: " + dataMemberInfo.GetMemberInfoType());
                    }
                }
            }
        }

        public static void Load(object[] entities)
        {
            foreach(object entity in entities)
                Load(entity);
        }

        public static T Load<T>()
            where T : new()
        {
            T entity = new T();
            Load(entity);
            return entity;
        }

        public static T Load<T>(string entityId)
            where T : new()
        {
            T entity = new T();
            Load(entity, entityId);
            SetEntityId(entity, entityId);
            return entity;
        }

        public static void Delete(object entity)
        {
            List<string> entityKeys = GetEntityKeys(entity);
            foreach (string key in entityKeys)
                PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }

        private static List<string> GetEntityKeys(object entity)
        {
            string entityId = GetEntityId(entity);
            Type entityType = entity.GetType();
            List<string> entityKeys = new List<string>();
            foreach (DataMemberInfo dataMemberInfo in GetDataMemberInfoWithAttribute<PlayerPrefsFieldAttribute>(entityType))
                entityKeys.Add(GetFieldKey(entityId, entityType, dataMemberInfo));
            return entityKeys;
        }
    }
}
