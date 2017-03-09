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
		private object threadLocker = new object();
		private Stream fileStream;

		protected Dictionary<string, string> configTable = new Dictionary<string, string>();
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
			lock(threadLocker)
			{
				configTable.Add(key, value);
			}
		}

		public virtual string GetConfigTable(string key)
		{
			return configTable[key];
		}

		#region IO Part
		public void SaveData()
		{
			try
			{
				binaryFormatter.Serialize(fileStream, configTable);
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
			try
			{
				configTable = (Dictionary<string, string>)binaryFormatter.Deserialize(fileStream);
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
				configTable = null;

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
