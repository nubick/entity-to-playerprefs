using System;
using System.Collections.Generic;
using System.IO;
using Assets.Plugins.EntityToPlayerPrefs;
using UnityEngine;

namespace Assets.Tests.Scripts
{
    public class FilePlayerPrefsProvider : IPlayerPrefsProvider
    {
        public static string GetDefaultSaveFilePath()
        {
            return Path.Combine(Application.persistentDataPath, "Save_Data.txt");
        }
        private readonly string _path;
        private readonly PlayerPrefsData _data;
        public FilePlayerPrefsProvider() : this(GetDefaultSaveFilePath()) { }
        public FilePlayerPrefsProvider(string path)
        {
            _path = path;
            if (File.Exists(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                _data = PlayerPrefsData.FromBinary(bytes);
            }
            else
            {
                _data = new PlayerPrefsData();
            }
        }

        public void Save()
        {
            byte[] bytes = _data.ToBinary();
            File.WriteAllBytes(_path, bytes);
        }

        public string GetString(string key, string defaultValue = "")
        {
            return _data.StringsMap.ContainsKey(key) ? _data.StringsMap[key] : defaultValue;
        }

        public void SetString(string key, string value)
        {
            _data.StringsMap[key] = value;
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            return _data.IntsMap.ContainsKey(key) ? _data.IntsMap[key] : defaultValue;
        }

        public void SetInt(string key, int value)
        {
            _data.IntsMap[key] = value;
        }

        public float GetFloat(string key, float defaultValue = 0f)
        {
            return _data.FloatsMap.ContainsKey(key) ? _data.FloatsMap[key] : defaultValue;
        }

        public void SetFloat(string key, float value)
        {
            _data.FloatsMap[key] = value;
        }

        public bool HasKey(string key)
        {
            return _data.StringsMap.ContainsKey(key) || _data.IntsMap.ContainsKey(key) || _data.FloatsMap.ContainsKey(key);
        }

        public void DeleteKey(string key)
        {
            _data.StringsMap.Remove(key);
            _data.IntsMap.Remove(key);
            _data.FloatsMap.Remove(key);
        }

        public void DeleteAll()
        {
            _data.StringsMap.Clear();
            _data.IntsMap.Clear();
            _data.FloatsMap.Clear();
        }

        public Dictionary<string, object> GetAllKeysAndValues()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            foreach (var item in _data.StringsMap)
            {
                result.Add(item.Key, item.Value);
            }
            foreach (var item in _data.IntsMap)
            {
                result.Add(item.Key, item.Value);
            }
            foreach (var item in _data.FloatsMap)
            {
                result.Add(item.Key, item.Value);
            }
            return result;
        }
    }

    public class PlayerPrefsData
    {
        public Dictionary<string, string> StringsMap = new Dictionary<string, string>();
        public Dictionary<string, int> IntsMap = new Dictionary<string, int>();
        public Dictionary<string, float> FloatsMap = new Dictionary<string, float>();

        public static PlayerPrefsData FromBinary(byte[] bytes)
        {
            PlayerPrefsData data = new PlayerPrefsData();
            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                {
                    data.StringsMap = ReadMap(binaryReader, br => br.ReadString());
                    data.IntsMap = ReadMap(binaryReader, br => br.ReadInt32());
                    data.FloatsMap = ReadMap(binaryReader, br => br.ReadSingle());
                }
            }
            return data;
        }

        public byte[] ToBinary()
        {
            byte[] bytes;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(stream))
                {
                    WriteMap(binaryWriter, StringsMap, (bw, v) => bw.Write(v));
                    WriteMap(binaryWriter, IntsMap, (bw, v) => bw.Write(v));
                    WriteMap(binaryWriter, FloatsMap, (bw, v) => bw.Write(v));
                }
                bytes = stream.GetBuffer();
            }
            return bytes;
        }

        private void WriteMap<T>(BinaryWriter binaryWriter, Dictionary<string, T> map, Action<BinaryWriter, T> writeAction)
        {
            binaryWriter.Write(map.Count);
            foreach (var pair in map)
            {
                binaryWriter.Write(pair.Key);
                writeAction(binaryWriter, pair.Value);
            }
        }

        private static Dictionary<string, T> ReadMap<T>(BinaryReader binaryReader, Func<BinaryReader, T> readFunc)
        {
            Dictionary<string, T> map = new Dictionary<string, T>();
            int count = binaryReader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string key = binaryReader.ReadString();
                T value = readFunc(binaryReader);
                map.Add(key, value);
            }
            return map;
        }
    }
}