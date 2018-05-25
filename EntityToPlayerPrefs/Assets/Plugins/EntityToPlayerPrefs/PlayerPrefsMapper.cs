using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Assets.Plugins.EntityToPlayerPrefs.FieldHandlers;
using UnityEngine;

namespace Assets.Plugins.EntityToPlayerPrefs
{
	public class PlayerPrefsMapper
	{
		public const string EntityKeyPrefix = "__entity";
		public const string SingleEntityId = "__single";

		private static string GetEntityId(object entity)
		{
			Type entityType = entity.GetType();
			List<DataMemberInfo> dataMemberInfos = PlayerPrefsCache.GetDataMemberInfoWithEntityIdAttribute(entityType);

			if (dataMemberInfos.Count == 0)
				return SingleEntityId;

			if (dataMemberInfos.Count > 1)
				throw new Exception(string.Format("Entity {0} contains more than one EntityId attribute.", entityType));

			return dataMemberInfos[0].GetValue<string>(entity);
		}

		private static void SetEntityId(object entity, string entityId)
		{
			Type entityType = entity.GetType();
			List<DataMemberInfo> dataMemberInfos = PlayerPrefsCache.GetDataMemberInfoWithEntityIdAttribute(entityType);

			if (dataMemberInfos.Count == 0)
				throw new Exception(string.Format("Entity {0} doesn't contain EntityId attribute.", entityType));

			if (dataMemberInfos.Count > 1)
				throw new Exception(string.Format("Entity {0} contains more than one EntityId attribute.", entityType));

			dataMemberInfos[0].SetValue(entity, entityId);
		}

		private static string GetFieldKey(string entityId, Type entityType, DataMemberInfo dataMemberInfo)
		{
			return GetFieldKey(entityId, entityType, dataMemberInfo.GetName());
		}

		private static string GetFieldKey(string entityId, Type entityType, string dataMemberName)
		{
			return string.Format("{0}.{1}.{2}.{3}", EntityKeyPrefix, entityType.Name, entityId, dataMemberName);
		}

		private static List<string> GetEntityKeys(object entity)
		{
			string entityId = GetEntityId(entity);
			Type entityType = entity.GetType();
			return GetEntityKeys(entityType, entityId);
		}

		private static List<string> GetEntityKeys(Type entityType, string entityId)
		{
			List<string> entityKeys = new List<string>();
			foreach (DataMemberInfo dataMemberInfo in PlayerPrefsCache.GetDataMemberInfoWithFieldAttribute(entityType))
				entityKeys.Add(GetFieldKey(entityId, entityType, dataMemberInfo));
			return entityKeys;
		}

		private static string GetFieldKey<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> expr)
		{
			string entityId = GetEntityId(entity);
			MemberExpression memberExpression = expr.Body as MemberExpression;
			return GetFieldKey(entityId, typeof(TEntity), memberExpression.Member.Name);
		}

		#region Public API

		public static void Save(object entity)
		{
			string entityId = GetEntityId(entity);
			Type entityType = entity.GetType();
			List<DataMemberInfo> dataMemberInfos = PlayerPrefsCache.GetDataMemberInfoWithFieldAttribute(entityType);
			foreach (DataMemberInfo dataMemberInfo in dataMemberInfos)
			{
				string fieldKey = GetFieldKey(entityId, entityType, dataMemberInfo);
				Type fieldType = dataMemberInfo.GetMemberInfoType();
				PlayerPrefsFieldHandler fieldHandler = PlayerPrefsFieldFactory.Get(fieldType);
				fieldHandler.SetValue(fieldKey, dataMemberInfo, entity);
			}
			PlayerPrefs.Save();
		}

		#region Loading

		public static void Load(object entity)
		{
			string entityId = GetEntityId(entity);
			Load(entity, entityId);
		}

		public static void Load(object entity, string entityId)
		{
			Type entityType = entity.GetType();
			List<DataMemberInfo> dataMemberInfos = PlayerPrefsCache.GetDataMemberInfoWithFieldAttribute(entityType);
			foreach (DataMemberInfo dataMemberInfo in dataMemberInfos)
			{
				string fieldKey = GetFieldKey(entityId, entityType, dataMemberInfo);
				Type fieldType = dataMemberInfo.GetMemberInfoType();
				if (PlayerPrefs.HasKey(fieldKey))
				{
					PlayerPrefsFieldHandler fieldHandler = PlayerPrefsFieldFactory.Get(fieldType);
					dataMemberInfo.SetValue(entity, fieldHandler.GetValue(fieldKey));
				}
			}
		}

		public static void Load(object[] entities)
		{
			foreach (object entity in entities)
				Load(entity);
		}

		public static TEntity Load<TEntity>()
			where TEntity : new()
		{
			TEntity entity = new TEntity();
			Load(entity);
			return entity;
		}

		public static TEntity Load<TEntity>(string entityId)
			where TEntity : new()
		{
			TEntity entity = new TEntity();
			Load(entity, entityId);
			SetEntityId(entity, entityId);
			return entity;
		}

		#endregion

		#region Deleting

		public static void Delete(object entity)
		{
			foreach (string entityKey in GetEntityKeys(entity))
				PlayerPrefs.DeleteKey(entityKey);
			PlayerPrefs.Save();
		}

		public static void Delete<TEntity>(string entityId)
		{
			foreach (string entityKey in GetEntityKeys(typeof(TEntity), entityId))
				PlayerPrefs.DeleteKey(entityKey);
			PlayerPrefs.Save();
		}

		public static void DeleteSingle<TEntity>()
		{
			Delete<TEntity>(SingleEntityId);
		}

		public static void Delete<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> expr)
		{
			string fieldKey = GetFieldKey(entity, expr);
			PlayerPrefs.DeleteKey(fieldKey);
			PlayerPrefs.Save();
		}

		#endregion

		#region Checking

		public static bool HasKey<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> expr)
		{
			string fieldKey = GetFieldKey(entity, expr);
			return PlayerPrefs.HasKey(fieldKey);
		}

		public static bool Exists<TEntity>(string entityId)
		{
			foreach (string entityKey in GetEntityKeys(typeof(TEntity), entityId))
				if (PlayerPrefs.HasKey(entityKey))
					return true;
			return false;
		}

		public static bool ExistsSingle<TEntity>()
		{
			return Exists<TEntity>(SingleEntityId);
		}

		#endregion

		#endregion

		private static class PlayerPrefsCache
		{
			private static Dictionary<Type, List<DataMemberInfo>> _dataMemberInfosFieldCache = new Dictionary<Type, List<DataMemberInfo>>();
			private static Dictionary<Type, List<DataMemberInfo>> _dataMemberInfosEntityIdCache = new Dictionary<Type, List<DataMemberInfo>>();

			public static List<DataMemberInfo> GetDataMemberInfoWithFieldAttribute(Type type)
			{
				if (!_dataMemberInfosFieldCache.ContainsKey(type))
					_dataMemberInfosFieldCache.Add(type, GetDataMemberInfoWithAttribute<PlayerPrefsFieldAttribute>(type));
				return _dataMemberInfosFieldCache[type];
			}

			public static List<DataMemberInfo> GetDataMemberInfoWithEntityIdAttribute(Type type)
			{
				if (!_dataMemberInfosEntityIdCache.ContainsKey(type))
					_dataMemberInfosEntityIdCache.Add(type, GetDataMemberInfoWithAttribute<PlayerPrefsEntityIdAttribute>(type));
				return _dataMemberInfosEntityIdCache[type];
			}

			private static List<DataMemberInfo> GetDataMemberInfoWithAttribute<TAttribute>(Type type)
				where TAttribute : Attribute
			{
				List<DataMemberInfo> dataMemberInfos = new List<DataMemberInfo>();

				dataMemberInfos.AddRange(type.GetFields()
					.Where(fi => fi.GetCustomAttributes(typeof(TAttribute), true).Any())
					.Select(fieldInfo => new DataMemberInfo(fieldInfo)));

				dataMemberInfos.AddRange(type.GetProperties()
					.Where(pi => pi.GetCustomAttributes(typeof(TAttribute), true).Any())
					.Select(propertyInfo => new DataMemberInfo(propertyInfo)));

				return dataMemberInfos;
			}
		}
	}
}
