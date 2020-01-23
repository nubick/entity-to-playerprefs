using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Assets.Plugins.EntityToPlayerPrefs.FieldHandlers;

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
				throw new Exception($"Entity {entityType} contains more than one EntityId attribute.");

			return dataMemberInfos[0].GetValue<string>(entity);
		}

		private static void SetEntityId(object entity, string entityId)
		{
			Type entityType = entity.GetType();
			List<DataMemberInfo> dataMemberInfos = PlayerPrefsCache.GetDataMemberInfoWithEntityIdAttribute(entityType);

			if (dataMemberInfos.Count == 0)
				throw new Exception($"Entity {entityType} doesn't contain EntityId attribute.");

			if (dataMemberInfos.Count > 1)
				throw new Exception($"Entity {entityType} contains more than one EntityId attribute.");

			dataMemberInfos[0].SetValue(entity, entityId);
		}

		private static string GetFieldKey(string entityId, Type entityType, DataMemberInfo dataMemberInfo)
		{
			return GetFieldKey(entityId, entityType, dataMemberInfo.GetName());
		}

		private static string GetFieldKey(string entityId, Type entityType, string dataMemberName)
		{
			string entityName = PlayerPrefsCache.GetEntityName(entityType);
			return $"{EntityKeyPrefix}.{entityName}.{entityId}.{dataMemberName}";
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
			PlayerPrefsProvider.Save();
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
				if (PlayerPrefsProvider.HasKey(fieldKey))
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
				PlayerPrefsProvider.DeleteKey(entityKey);
			PlayerPrefsProvider.Save();
		}

		public static void Delete<TEntity>(string entityId)
		{
			foreach (string entityKey in GetEntityKeys(typeof(TEntity), entityId))
				PlayerPrefsProvider.DeleteKey(entityKey);
			PlayerPrefsProvider.Save();
		}

		public static void DeleteSingle<TEntity>()
		{
			Delete<TEntity>(SingleEntityId);
		}

		public static void Delete<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> expr)
		{
			string fieldKey = GetFieldKey(entity, expr);
			PlayerPrefsProvider.DeleteKey(fieldKey);
			PlayerPrefsProvider.Save();
		}

		#endregion

		#region Checking

		public static bool HasKey<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> expr)
		{
			string fieldKey = GetFieldKey(entity, expr);
			return PlayerPrefsProvider.HasKey(fieldKey);
		}

		public static bool Exists<TEntity>(string entityId)
		{
			foreach (string entityKey in GetEntityKeys(typeof(TEntity), entityId))
				if (PlayerPrefsProvider.HasKey(entityKey))
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
			private static readonly Dictionary<Type, List<DataMemberInfo>> DataMemberInfosFieldCache = new Dictionary<Type, List<DataMemberInfo>>();
			private static readonly Dictionary<Type, List<DataMemberInfo>> DataMemberInfosEntityIdCache = new Dictionary<Type, List<DataMemberInfo>>();
			private static readonly Dictionary<Type, string> EntityNameCache = new Dictionary<Type, string>();

			public static List<DataMemberInfo> GetDataMemberInfoWithFieldAttribute(Type type)
			{
				if (!DataMemberInfosFieldCache.ContainsKey(type))
					DataMemberInfosFieldCache.Add(type, GetDataMemberInfoWithAttribute<PlayerPrefsFieldAttribute>(type));
				return DataMemberInfosFieldCache[type];
			}

			public static List<DataMemberInfo> GetDataMemberInfoWithEntityIdAttribute(Type type)
			{
				if (!DataMemberInfosEntityIdCache.ContainsKey(type))
					DataMemberInfosEntityIdCache.Add(type, GetDataMemberInfoWithAttribute<PlayerPrefsEntityIdAttribute>(type));
				return DataMemberInfosEntityIdCache[type];
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

			public static string GetEntityName(Type type)
			{
				if (!EntityNameCache.ContainsKey(type))
				{
					PlayerPrefsEntityNameAttribute attribute = Attribute.GetCustomAttribute(type, typeof(PlayerPrefsEntityNameAttribute)) as PlayerPrefsEntityNameAttribute;
					string name = attribute == null ? type.Name : attribute.Name;
					EntityNameCache.Add(type, name);
				}
				return EntityNameCache[type];
			}
		}
	}
}
