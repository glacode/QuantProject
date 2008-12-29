/*
QuantProject - Quantitative Finance Library

ConfigManager.cs
Copyright (C) 2008
Glauco Siliprandi

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 */

using System;
using System.Configuration;
using System.Windows.Forms;

namespace QuantProject.DataAccess
{
	/// <summary>
	/// Use this class to access (read/write) the config file
	/// </summary>
	public class ConfigManager
	{
		private static string keyFor_mdbPath = "mdbPath";
		private static string keyFor_mySqlConnectionString = "mySqlConnectionString";
		
		private static string keyFor_chosenDbType = "dbType";
		private static string configValueForAccessDbType = "Access";
		private static string configValueForMySqlDbType = "MySQL";
		
//		/// <summary>
//		/// true iif the path for the Access file is configured
//		/// </summary>
//		public static bool IsMdbPathConfigured
//		{
//			get
//			{
//				bool isMdbPathConfigured =
//					ConfigManager.containsKey( ConfigManager.keyFor_mdbPath );
//				return isMdbPathConfigured;
//			}
//		}
		
		#region MdbPath
		/// <summary>
		/// Full path to the Access database
		/// </summary>
		public static string MdbPath
		{
			get { return ConfigManager.getMdbPath(); }
		}
		
		#region getMdbPath
		private static void checkIfDbTypeIsAccess()
		{
			if ( !ConfigManager.IsChosenDbTypeAccess )
				throw new Exception(
					"MdbPath can be read only if the chosen database type is Access" );
		}
		#region getMdbPathActually
		
		#region setMdbPathReadingItFromUser
		private static string readMdbPathFromUser()
		{
			AccessDataBaseLocator dataBaseLocator = new AccessDataBaseLocator();
			string mdbPath = dataBaseLocator.Path;
			return mdbPath;
		}
		private static void setMdbPathReadingItFromUser()
		{
			string mdbPath = ConfigManager.readMdbPathFromUser();
			ConfigManager.setAppConfig( ConfigManager.keyFor_mdbPath , mdbPath );
		}
		#endregion setMdbPathReadingItFromUser
		
		private static string getMdbPathActually()
		{
			if ( !ConfigManager.containsKey( ConfigManager.keyFor_mdbPath ) )
				// the config value for the mdb path has not been configured yet
				ConfigManager.setMdbPathReadingItFromUser();
			string mdbPath =
				ConfigManager.getAppConfig( ConfigManager.keyFor_mdbPath );
			return mdbPath;
		}
		#endregion getMdbPathActually
		private static string getMdbPath()
		{
			ConfigManager.checkIfDbTypeIsAccess();
			string mdbPath = ConfigManager.getMdbPathActually();
			return mdbPath;
		}
		#endregion getMdbPath
		
		#endregion MdbPath
		
		#region MySqlConnectionString
		/// <summary>
		/// Connection string to the MySQL database
		/// </summary>
		public static string MySqlConnectionString
		{
			get { return ConfigManager.getMySqlConnectionString(); }
		}
		
		#region getMySqlConnectionString
		private static void checkIfDbTypeIsMySQL()
		{
			if ( !ConfigManager.IsChosenDbTypeMySql )
				throw new Exception(
					"MySqlConnectionString can be read " +
					"only if the chosen database type is MySQL" );
		}
		
		#region setConnectionStringReadingDataFromUser
		private static string getMySqlConnectionStringReadingDataFromUser()
		{
			MySqlConnectionForm mySqlConnectionForm = new MySqlConnectionForm();
			mySqlConnectionForm.ShowDialog();
			string connectionString = mySqlConnectionForm.MySqlConnectionString;
			return connectionString;
		}
		private static void setMySqlConnectionStringReadingDataFromUser()
		{
			string connectionString =
				ConfigManager.getMySqlConnectionStringReadingDataFromUser();
			ConfigManager.setAppConfig(
				ConfigManager.keyFor_mySqlConnectionString , connectionString );
		}
		#endregion setConnectionStringReadingDataFromUser
		
		private static string getMySqlConnectionStringActually()
		{
			if ( !ConfigManager.containsKey( ConfigManager.keyFor_mySqlConnectionString ) )
				// the config value for the MySQL connection string has not been configured yet
				ConfigManager.setMySqlConnectionStringReadingDataFromUser();
			string mySqlConnectionString =
				ConfigManager.getAppConfig( ConfigManager.keyFor_mySqlConnectionString );
			return mySqlConnectionString;
		}
		private static string getMySqlConnectionString()
		{
			ConfigManager.checkIfDbTypeIsMySQL();
			string mySqlConnectionString = ConfigManager.getMySqlConnectionStringActually();
			return mySqlConnectionString;
		}
		#endregion getMySqlConnectionString
		
		#endregion MySqlConnectionString
		
		#region IsChosenDbTypeAccess
		
		#region getChosenDbType
		
		#region setChosenDbTypeIfTheCase
		
		private static bool isChosenDbTypeValid()
		{
			string configValueForChosenDbType =
				ConfigManager.getAppConfig( ConfigManager.keyFor_chosenDbType );
			bool isValid = (
				( configValueForChosenDbType == ConfigManager.configValueForAccessDbType ) ||
				( configValueForChosenDbType == ConfigManager.configValueForMySqlDbType ) );
			return isValid;			
		}
		
		#region setDbType
		private static string getConfigValue( DbType dbType )
		{
			string configValue = null;
			switch ( dbType )
			{
				case DbType.Access:
					configValue = ConfigManager.configValueForAccessDbType;
					break;
				case DbType.MySql:
					configValue = ConfigManager.configValueForMySqlDbType;
					break;
				default:
					throw new Exception(
						"An unmanaged dbType has been given: complete the switch " +
						"statement to manage it" );
			}
			return configValue;
		}
		private static void setDbType()
		{
			DbTypeChooser dbTypeChooser = new DbTypeChooser();
			dbTypeChooser.ShowDialog();
			string configValue = ConfigManager.getConfigValue( dbTypeChooser.DbType );
			ConfigManager.setAppConfig(
				ConfigManager.keyFor_chosenDbType ,	configValue );
		}
		#endregion setDbType
		
		private static void setChosenDbTypeIfTheCase()
		{
			if ( ( !ConfigManager.containsKey( ConfigManager.keyFor_chosenDbType ) ) ||
			    ( !ConfigManager.isChosenDbTypeValid() ) )
				// either the database typer has not been defined yet, or
				// it has been defined, but the config value is invalid
				ConfigManager.setDbType();
		}
		#endregion setChosenDbTypeIfTheCase
//		private static void checkIfKeyForChosenDbTypeIsDefined()
//		{
//			if ( !ConfigManager.containsKey( ConfigManager.keyFor_chosenDbType ) )
//				throw new Exception(
//					"The config file doesn't contain the key '" +
//					ConfigManager.keyFor_chosenDbType + "'" );
//		}
//		private static void checkIfConfigValueForChosenDbTypeIsAValidValue()
//		{
//			string chosenDbType = ConfigManager.getAppConfig(
//				ConfigManager.keyFor_chosenDbType );
//			if (
//				( chosenDbType != ConfigManager.configValueForAccessDbType ) &&
//				( chosenDbType != ConfigManager.configValueForMySqlDbType ) )
//				throw new Exception(
//					"An invalid database type value has been found in the config file. " +
//					"The key to be configured is '" + ConfigManager.keyFor_chosenDbType + "'. " +
//					"Allowed values are either '" +
//					ConfigManager.configValueForAccessDbType + "' or '" +
//					ConfigManager.configValueForMySqlDbType +"'" );
//		}
		private static string getChosenDbType()
		{
			ConfigManager.setChosenDbTypeIfTheCase();
//			ConfigManager.checkIfKeyForChosenDbTypeIsDefined();
//			ConfigManager.checkIfConfigValueForChosenDbTypeIsAValidValue();
			string chosenDbType = ConfigManager.getAppConfig(
				ConfigManager.keyFor_chosenDbType );
			return chosenDbType;
		}
		#endregion getChosenDbType
		
		/// <summary>
		/// true iif the current chosen database type is set to Access
		/// </summary>
		/// <param name="mdbPath"></param>
		public static bool IsChosenDbTypeAccess
		{
			get
			{
				string chosenDbType =
					ConfigManager.getChosenDbType();
				bool isChosenDbTypeAccess =
					( chosenDbType == ConfigManager.configValueForAccessDbType );
				return isChosenDbTypeAccess;
			}
		}
		#endregion IsChosenDbTypeAccess
		
		/// <summary>
		/// true iif the current chosen database type is set to MySQL
		/// </summary>
		public static bool IsChosenDbTypeMySql
		{
			get
			{
				string chosenDbType =
					ConfigManager.getChosenDbType();
				bool isChosenDbTypeMySQL =
					( chosenDbType == ConfigManager.configValueForMySqlDbType );
				return isChosenDbTypeMySQL;
			}
		}
		
		public ConfigManager()
		{
		}

		public static void SetMdbPath( string mdbPath )
		{
			ConfigManager.getAppConfig( ConfigManager.keyFor_mdbPath );
		}

		private static void setAppConfig(
			string key , string configurationValue )
		{
			Configuration configuration =
				ConfigurationManager.OpenExeConfiguration( Application.ExecutablePath );
			configuration.AppSettings.Settings.Remove( key );
			configuration.AppSettings.Settings.Add( key , configurationValue );
			configuration.Save();
		}
		
		private static string getAppConfig( string key )
		{
			Configuration configuration =
				ConfigurationManager.OpenExeConfiguration( Application.ExecutablePath );
			string configurationValue =
				configuration.AppSettings.Settings[ key ].Value;
			return configurationValue;
		}
		private static bool containsKey( string key )
		{
			Configuration configuration =
				ConfigurationManager.OpenExeConfiguration( Application.ExecutablePath );
			bool configurationContains =
				( configuration.AppSettings.Settings[ key ] != null );
			return configurationContains;
		}
	}
}
