using System;
using System.IO;
using Newtonsoft.Json;

namespace EveOPreview.Configuration.Implementation
{
	class ConfigurationStorage : IConfigurationStorage
	{
		private const string CONFIGURATION_FILE_NAME = "EVE-O-Preview.json";

		private readonly IAppConfig _appConfig;
		private readonly IThumbnailConfiguration _thumbnailConfiguration;

		public ConfigurationStorage(IAppConfig appConfig, IThumbnailConfiguration thumbnailConfiguration)
		{
			this._appConfig = appConfig;
			this._thumbnailConfiguration = thumbnailConfiguration;
		}

		public void Load()
		{
			string filename = this.GetConfigFileName();
			string backupFilename = this.GetBackupConfigFileName(filename);

			if (!File.Exists(filename) && !File.Exists(backupFilename))
			{
				return;
			}

			JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
			{
				ObjectCreationHandling = ObjectCreationHandling.Replace
			};

			if (!this.TryLoad(filename, jsonSerializerSettings))
			{
				this.TryLoad(backupFilename, jsonSerializerSettings);
			}

			// Validate data after loading it
			this._thumbnailConfiguration.ApplyRestrictions();
		}

		public void Save()
		{
			string rawData = JsonConvert.SerializeObject(this._thumbnailConfiguration, Formatting.Indented);
			string filename = this.GetConfigFileName();
			string backupFilename = this.GetBackupConfigFileName(filename);
			string temporaryFilename = filename + ".tmp";

			try
			{
				File.WriteAllText(temporaryFilename, rawData);
				if (File.Exists(filename))
				{
					File.Copy(filename, backupFilename, true);
				}

				File.Move(temporaryFilename, filename, true);
			}
			catch (Exception)
			{
				// Ignore error if for some reason the updated config cannot be written down.
				// A locked (antivirus, cloud sync, another instance) config file
				// should never bring the whole application down
			}
			finally
			{
				try
				{
					if (File.Exists(temporaryFilename))
					{
						File.Delete(temporaryFilename);
					}
				}
				catch (Exception)
				{
					// Nothing can be done here - the temporary file will be overwritten on the next save
				}
			}
		}

		private bool TryLoad(string filename, JsonSerializerSettings settings)
		{
			if (!File.Exists(filename))
			{
				return false;
			}

			try
			{
				string rawData = File.ReadAllText(filename);
				JsonConvert.PopulateObject(rawData, this._thumbnailConfiguration, settings);
				return true;
			}
			catch (IOException)
			{
				return false;
			}
			catch (JsonException)
			{
				return false;
			}
		}

		private string GetBackupConfigFileName(string filename)
		{
			return filename + ".backup";
		}

		private string GetConfigFileName()
		{
			return string.IsNullOrEmpty(this._appConfig.ConfigFileName) ? ConfigurationStorage.CONFIGURATION_FILE_NAME : this._appConfig.ConfigFileName;
		}
	}
}
