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
		private OleDbConnection oleDbConnection;
		delegate void updatingMethodHandler();
		private ArrayList updatingMethods;
		
    #region "DataBaseVersionManager"
    /// <summary>
    /// The method initialize a populate updatingMethods, an ArrayList
    /// that contains all the delegates that encapsulate all methods
    /// whose function is to modify the structure of the database.
    /// The key in the sortedList is the new version number that signs 
    /// the database after the execution of the encapsulated method 
    /// </summary>
    private void initialize_updatingMethods()
    {
		this.updatingMethods  = new ArrayList();
		// After adding a new private method to the class (e.g. dropConstraints o
		// or similar) write similar code to the following:
		this.updatingMethods.Add(new updatingMethodHandler(this.createDatabase));
		this.updatingMethods.Add(new updatingMethodHandler(this.createTables));
		this.updatingMethods.Add(new updatingMethodHandler(this.alterTablesAddPrimaryKeys));
		this.updatingMethods.Add(new updatingMethodHandler(this.alterTablesAddForeignKeys));
		this.updatingMethods.Add(new updatingMethodHandler(this.alterTablesAddIndexes));
		this.updatingMethods.Add(new updatingMethodHandler(this.alterTablesAddColumns));
		this.updatingMethods.Add(new updatingMethodHandler(this.dropTables));
		
    }
		public DataBaseVersionManager(OleDbConnection oleDbConnection)
		{
			try
			{
				this.oleDbConnection = oleDbConnection;
				this.oleDbConnection.Open();
				this.initialize_updatingMethods();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
				this.oleDbConnection.Close();
			}
		}
		
    #endregion


    #region "updating methods"
	private void createDatabase()
	{
		//TODO code to create an empty DB if user doesn't select the QuantProject.mdb;
	}
	private void createTables()
    {
		this.executeCommand("CREATE TABLE tickers (tiTicker TEXT(8))");
		this.executeCommand("CREATE TABLE quotes (quTicker TEXT(8), quDate DATETIME, " +
							"quOpen REAL, quHigh REAL, quLow REAL, quClose REAL, " +
							"quVolume INTEGER, quAdjustedClose REAL, quAdjustedCloseToCloseRatio FLOAT)");
		// table where to store the time period for which tickers' quotes have been validated
		this.executeCommand("CREATE TABLE validatedTickers " +
        "( vtTicker TEXT(8) , vtStartDate DATETIME , vtEndDate DATETIME , vtDate DATETIME)");
		// table of groups where you can collect tickers.
		// Groups are used to simplify operations like:
		// validating, updating data from the web, testing strategies
		this.executeCommand("CREATE TABLE tickerGroups " +
			"( tgId TEXT(8) , tgDescription TEXT(100), tgTgId TEXT(8))");
    // where to store the relation between a ticker and a group
    // NOTE that a group can be created inside another group and
    // a ticker can belong to one or more groups 
    this.executeCommand("CREATE TABLE tickers_tickerGroups " +
      "( ttTgId TEXT(8) , ttTiId TEXT(8))");
    // validatedTickers will contain a record for each ticker whose quotes have already
    // been validated. The quotes are meant to be ok from vtStartDate to vtEndDate.
    this.executeCommand( "CREATE TABLE validatedTickers " +
      "( vtTicker TEXT(8) , vtStartDate DATETIME , vtEndDate DATETIME , vtEditDate DATETIME, " +
      "CONSTRAINT myKey PRIMARY KEY ( vtTicker ) )" );
    // visuallyValidatedTickers will contain a record for each ticker whose
    // quotes with suspicious ratios have been validated.
    // Field list:
    // vvTicker: validated ticker
    // vvStartDate: starting date of the time span being visually validated
    // vvEndDate: ending date of the time span being visually validated
    // vvHashValue: hash value for the visually validated quotes
    // vvCloseToCloseRatio: the close to close ratio has been checked to be acceptable
    // vvRangeToRangeRatio: the High-Low range ratio has been checked to be acceptable
    // vvDate: Last date this record has been added/modified
    this.executeCommand( "CREATE TABLE visuallyValidatedTickers " +
      "( vvTicker TEXT(8) , vvStartDate DATETIME , vvEndDate DATETIME , " +
      "vvHashValue TEXT(50) , vvEditDate DATETIME, " +
      "vvCloseToCloseRatio BIT , vvRangeToRangeRatio BIT , " +
      "CONSTRAINT myKey PRIMARY KEY ( vvTicker ) )" );
    // quotesFromSecondarySources will contain quotes coming from sources different
    // from the main one. It will be used for confirming and thus validation purposes.
    // Field descriptions:
    // qsSource: 1 = manually inserted; 2 = automatically downloaded/imported
    // qsEditDate: last date this record has been added/modified
    this.executeCommand( "create table quotesFromSecondarySources " +
      "(qsTicker TEXT(8) , " +
      "qsDate DATETIME , " +
      "qsSource SHORT , " +
      "qsOpen SINGLE , " +
      "qsHigh SINGLE , " +
      "qsLow SINGLE , " +
      "qsClose SINGLE , " +
      "qsVolume SINGLE , " +
      "qsAdjustedClose SINGLE , " +
      "qsAdjustedCloseToCloseRatio DOUBLE , " +
      "qsEditDate DATETIME , " +
      "CONSTRAINT myKey PRIMARY KEY ( qsTicker , qsDate , qsSource ) )" );
    }

	private void alterTablesAddPrimaryKeys()
	{
			this.executeCommand("ALTER TABLE tickers ADD CONSTRAINT PKtiTicker PRIMARY KEY (tiTicker)");
			this.executeCommand("ALTER TABLE quotes ADD CONSTRAINT PKquTicker_quDate " +
								"PRIMARY KEY (quTicker, quDate)");
			this.executeCommand("ALTER TABLE tickerGroups ADD CONSTRAINT PKtgId PRIMARY KEY (tgId)");
			this.executeCommand("ALTER TABLE validatedTickers ADD CONSTRAINT myKey PRIMARY KEY ( vtTicker )");
			this.executeCommand("ALTER TABLE tickers_tickerGroups " + 
								"ADD CONSTRAINT PKttTgId_ttTiId PRIMARY KEY ( ttTgId, ttTiId)");
	}
	private void alterTablesAddForeignKeys()
	{
		// add code here for adding foreign keys to existing tables;
	}
	private void alterTablesAddColumns()
	{
		this.executeCommand("ALTER TABLE tickers " + 
			"ADD COLUMN tiCompanyName TEXT(100)");
	}
	private void alterTablesAddIndexes()
	{
		//add code here for adding indexes to existing tables;
	}

	private void dropTables()
	{
		this.executeCommand("DROP TABLE version");
	}	
	private void executeCommand(string commandToBeExecuted)
	{
		try
		{		
			OleDbCommand oleDbCommand = new OleDbCommand( commandToBeExecuted , this.oleDbConnection );
			int checkCommandExecution = oleDbCommand.ExecuteNonQuery();
		}
		catch(Exception ex)
		{
      string notUsed = ex.ToString();// to avoid warning after compilation
    }	
	}
	
    #endregion


    #region "UpdateDataBaseStructure"
   		public void UpdateDataBaseStructure()
		{
			try
			{
				foreach (Object method in this.updatingMethods)
				{
					updatingMethodHandler handler = (updatingMethodHandler)method;
					handler();
				}
				this.oleDbConnection.Close();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
				this.oleDbConnection.Close();
			}
		}
    #endregion	
	
	}
}
