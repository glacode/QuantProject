/*
QuantProject - Quantitative Finance Library

BataBaseVersionManager.cs
Copyright (C) 2003 
Marco Milletti

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
using System.Windows.Forms;
using System.Data.OleDb;
using System.Data;
using System.Collections;


namespace QuantProject.DataAccess
{
	/// <summary>
	/// This object updates database's structure in order to
	/// assure complete compatibility in case the user still uses an 
	/// old database version
	/// </summary>
	/// <remarks>
	/// The changes to database's structure are controlled by
	/// the UpdateDataBaseStructure method which executes all the methods that
	/// modify the structure of the database. These methods must have the same signature
	/// of the delegate updatingMethodHandler.
	/// A new method needs to be added as a new item in the updatingMethods 
	/// through the delegate updatingMethodHandler
	/// </remarks>
	
	public class DataBaseVersionManager
	{
		private OleDbDataAdapter oleDbDataAdapter;
		private DataTable dataTable_version;
		private string selectString;
		private string updateString;
		private int databaseVersion;
		private OleDbConnection oleDbConnection;
		delegate void updatingMethodHandler();
		private SortedList updatingMethods;
		
		public DataBaseVersionManager(OleDbConnection oleDbConnection)
		{
			try
			{
				this.oleDbConnection = oleDbConnection;
				this.oleDbConnection.Open();
				this.selectString = "SELECT * FROM version";
				this.updateString = "UPDATE version SET veId = ?";
				this.oleDbDataAdapter = new OleDbDataAdapter(selectString, this.oleDbConnection);
				this.oleDbDataAdapter.UpdateCommand = new OleDbCommand(this.updateString,this.oleDbConnection);
				this.oleDbDataAdapter.UpdateCommand.Parameters.Add("@veId", OleDbType.Integer); 
				this.oleDbDataAdapter.UpdateCommand.Parameters["@veId"].SourceColumn = "veId";
				this.dataTable_version  = new DataTable();
				this.initialize_updatingMethods();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
				this.oleDbConnection.Close();
			}
		}
		
		public void UpdateDataBaseStructure()
		{
			try
			{
				foreach (DictionaryEntry method in this.updatingMethods)
				{
					this.executeMethod(method);
				}
				this.oleDbConnection.Close();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
				this.oleDbConnection.Close();
			}

		}
		
		private int getDataBaseVersionNumber()
		{
			this.dataTable_version.Clear();
			this.oleDbDataAdapter.Fill(this.dataTable_version);
			DataRow dataRow = this.dataTable_version.Rows[0];
			return (int)dataRow["veId"];
		}

		
		private void setNewDataBaseVersionNumber(int newVersionNumber)
		{
			this.dataTable_version.Rows[0]["veId"] = newVersionNumber;
			this.oleDbDataAdapter.Update(this.dataTable_version);
			this.dataTable_version.AcceptChanges();
		}
	  /// <summary>
	  /// The method initialize a populate updatingMethods, a SortedList
	  /// that contains all the delegates that encapsulate all methods
	  /// whose function is to modify the structure of the database.
	  /// The key in the sortedList is the new version number that signs 
	  /// the database after the execution of the encapsulated method 
	  /// </summary>
		private void initialize_updatingMethods()
		{
			this.updatingMethods  = new SortedList();
			//After adding a new private method to the class, write
			//the following code, paying attention to the version number:
			//this.updatingMethods.Add(yourVersionNumber,
			//						new updatingMethodHandler(this.yourMethodName))

		}
		
		private void executeMethod(DictionaryEntry itemContainingMethod)
		{
			this.databaseVersion = this.getDataBaseVersionNumber();
			int differenceBetweenNewVersionAndDataBaseVersion =
				(int)itemContainingMethod.Key  - this.databaseVersion;
			if(differenceBetweenNewVersionAndDataBaseVersion == 1)
			//db's structure can be updated by the method contained in the item
			{
				updatingMethodHandler handler = (updatingMethodHandler)itemContainingMethod.Value;
				handler();
				//it calls the method that modifies the db structure
				this.setNewDataBaseVersionNumber((int)itemContainingMethod.Key);
			}

		}

		
	}
}
