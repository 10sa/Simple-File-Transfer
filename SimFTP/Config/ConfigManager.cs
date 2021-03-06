﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;


namespace SimFTP.Config
{
	public abstract class ConfigManager : IDisposable
	{
		private BinaryFormatter binaryFormatter = new BinaryFormatter();
		private Stream fileStream;

		public Dictionary<string, string> ConfigTable { get; protected set; } = new Dictionary<string, string>();

		protected abstract void InitializeConfig();

		// Not Using This Constructor. //
		private ConfigManager() { }

		public ConfigManager(string path)
		{
			if (File.Exists(path))
			{
				fileStream = File.Open(path, FileMode.Open);
				LoadData();
			}
			else
			{
				Console.WriteLine("Config File Not Found, Initialize... [" + path + "]");
				fileStream = File.Open(path, FileMode.CreateNew);
				InitializeConfig();
				SaveData();
			}
		}

		public virtual void AddConfigTable(string key, string value)
		{
			ConfigTable.Add(key, value);
			SaveData();
		}

		public virtual void SetConfigTable(string key, string value)
		{
			ConfigTable[key] = value;
			SaveData();
		}

		public virtual string GetConfigTable(string key)
		{
			return ConfigTable[key];
		}

		#region IO Part
		public void SaveData()
		{
			fileStream.Position = 0;
			try
			{
				binaryFormatter.Serialize(fileStream, ConfigTable);
				fileStream.Flush();
			}
			catch
			{
				Util.ErrorHandling("Data Serialize Failure.");
				throw;
			}
		}

		public void LoadData()
		{
			fileStream.Position = 0;
			try
			{
				ConfigTable = (Dictionary<string, string>)binaryFormatter.Deserialize(fileStream);
			}
			catch
			{
				Util.ErrorHandling("Data Deserialize Failure, Initialize Config...");
				InitializeConfig();
				SaveData();
			}
		}
		#endregion

		#region IDisposable Support
		private bool disposedValue = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					fileStream.Dispose();
				}

				binaryFormatter = null;
				ConfigTable = null;

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}
		#endregion
	}
}
