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
using System.Data.Common;
using System.Data.OleDb;
using System.Data;
using System.Collections;

using MySql.Data.MySqlClient;

using QuantProject.ADT;


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
		private DbConnection dbConnection;
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
		this.updatingMethods.Add(new updatingMethodHandler(this.alterTablesRemovePrimaryKeys));
    this.updatingMethods.Add(new updatingMethodHandler(this.alterTablesAddForeignKeys));
    this.updatingMethods.Add(new updatingMethodHandler(this.alterTablesAddIndexes));
		this.updatingMethods.Add(new updatingMethodHandler(this.alterTablesAddColumns));
		this.updatingMethods.Add(new updatingMethodHandler(this.dropTables));
		this.updatingMethods.Add(new updatingMethodHandler(this.updateTables));
		
    }
		public DataBaseVersionManager( DbConnection dbConnection)
		{
			try
			{
				this.dbConnection = dbConnection;
				this.initialize_updatingMethods();
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
				this.dbConnection.Close();
			}
		}
		
    #endregion


    #region "updating methods"
	private void createDatabase()
	{
		//TODO: code to create an empty DB if user doesn't select the QuantProject.mdb;
	}
	private void createTables()
    {
		this.executeCommand("CREATE TABLE tickers (tiTicker TEXT(10))");
		this.executeCommand("CREATE TABLE quotes (quTicker TEXT(10), quDate DATETIME, " +
							"quOpen REAL, quHigh REAL, quLow REAL, quClose REAL, " +
							"quVolume INTEGER, quAdjustedClose REAL, quAdjustedCloseToCloseRatio FLOAT)");
		// table of groups where you can collect tickers.
		// Groups are used to simplify operations like:
		// validating, updating data from the web, testing strategies
		this.executeCommand("CREATE TABLE tickerGroups " +
			"( tgId TEXT(8) , tgDescription TEXT(100), tgTgId TEXT(8))");
    // where to store the relation between a ticker and a group
    // NOTE that a group can be created inside another group and
    // a ticker can belong to one or more groups 
    this.executeCommand("CREATE TABLE tickers_tickerGroups " +
      "( ttTgId TEXT(8) , ttTiId TEXT(10))");
    // validatedTickers will contain a record for each ticker whose quotes have already
    // been validated. The quotes are meant to be ok from vtStartDate to vtEndDate.
    this.executeCommand( "CREATE TABLE validatedTickers " +
      "( vtTicker TEXT(10) , vtStartDate DATETIME , vtEndDate DATETIME , " +
			"vtHashValue TEXT(50) , vtEditDate DATETIME, " +
      "CONSTRAINT myKey PRIMARY KEY ( vtTicker ) )" );
    // visuallyValidatedQuotes will contain a record for each
    // quote with suspicious ratio that has been validated.
    // Field list:
		// vvTicker: validated ticker
		// vvDate: validated quote date
		// vvValidationType: 1 = close to close ratio ; 2 = range to range ratio
    // vvHashValue: hash value for the visually validated quotes
    // vvEditDate: Last date this record has been added/modified
    this.executeCommand( "CREATE TABLE visuallyValidatedQuotes " +
      "( vvTicker TEXT(10) , vvDate DATETIME , vvValidationType INT , " +
      "vvHashValue TEXT(50) , vvEditDate DATETIME , " +
      "CONSTRAINT myKey PRIMARY KEY ( vvTicker , vvDate , vvValidationType ) )" );
    // quotesFromSecondarySources will contain quotes coming from sources different
    // from the main one. It will be used for confirming and thus validation purposes.
    // Field descriptions:
    // qsSource: 1 = manually inserted; 2 = automatically downloaded/imported
    // qsEditDate: last date this record has been added/modified
    this.executeCommand( "create table quotesFromSecondarySources " +
      "(qsTicker TEXT(10) , " +
      "qsDate DATETIME , " +
      "qsSource SHORT , " +
      "qsOpen SINGLE , " +
      "qsHigh SINGLE , " +
      "qsLow SINGLE , " +
      "qsClose SINGLE , " +
      "qsVolume SINGLE , " +
      "qsAdjustedClose SINGLE , " +
      "qsAdjustedCloseToCloseRatio SINGLE , " +
      "qsEditDate DATETIME , " +
      "CONSTRAINT myKey PRIMARY KEY ( qsTicker , qsDate , qsSource ) )" );

	// faultyTickers will contain tickers not downloaded from the web
		this.executeCommand( "CREATE TABLE  faultyTickers " +
      "(ftTicker TEXT(10) , " +
      "ftDate DATETIME)");
    }

	private void alterTablesAddPrimaryKeys()
	{
			this.executeCommand("ALTER TABLE tickers ADD CONSTRAINT PKtiTicker PRIMARY KEY (tiTicker)");
			this.executeCommand("ALTER TABLE quotes ADD CONSTRAINT PKquTicker_quDate " +
								"PRIMARY KEY (quTicker, quDate)");
			this.executeCommand("ALTER TABLE tickerGroups ADD CONSTRAINT PKtgId PRIMARY KEY (tgId)");
			this.executeCommand("ALTER TABLE validatedTickers ADD CONSTRAINT myKey PRIMARY KEY ( vtTicker )");
			//this.executeCommand("ALTER TABLE tickers_tickerGroups " + 
			//					"ADD CONSTRAINT PKttTgId_ttTiId PRIMARY KEY ( ttTgId, ttTiId)");
	}

  private void alterTablesRemovePrimaryKeys()
  {
    this.executeCommand("DROP INDEX PKttTgId_ttTiId ON tickers_tickerGroups");
  }
	private void alterTablesAddForeignKeys()
	{
		// add code here for adding foreign keys to existing tables;
	}
	private void alterTablesAddColumns()
	{
		// add code here for adding new columns to existing tables;

    this.executeCommand("ALTER TABLE tickers " + 
			"ADD COLUMN tiCompanyName TEXT(100) NOT NULL");
    this.executeCommand("ALTER TABLE validatedTickers " +
      "ADD COLUMN vtHashValue TEXT(50)");
    this.executeCommand("ALTER TABLE validatedTickers " +
      "ADD COLUMN vtEditDate DATETIME");
    this.executeCommand("ALTER TABLE tickers_tickerGroups " + 
      "ADD COLUMN ttEventType TEXT(1) NOT NULL");
    this.executeCommand("ALTER TABLE tickers_tickerGroups " +
      "ADD COLUMN ttEventDate DATETIME NOT NULL");
	}
	private void alterTablesAddIndexes()
	{
		//add code here for adding indexes to existing tables;
    this.executeCommand("CREATE INDEX " + 
                        "PK_ttTgId_ttTiId_ttEventDate_ttEventDate " +
                        "ON tickers_tickerGroups " + 
                        "(ttTgId, ttTiId, ttEventType, ttEventDate) "+
                       	"WITH PRIMARY");
	}

	private void dropTables()
	{
		this.executeCommand("DROP TABLE version");
		this.executeCommand("DROP TABLE visuallyValidatedTickers");
	}
	
	//inserts new rows or updates 
	//existing rows in tables after structure's modifications
	private void updateTables()
	{
		this.executeCommand("UPDATE tickers_tickerGroups " +
		                    "SET tickers_tickerGroups.ttEventType ='I', " +
		                    "tickers_tickerGroups.ttEventDate =" + 
                        SQLBuilder.GetDateConstant(ConstantsProvider.DefaultDateForTickersAddedToGroups) + " " +
		                    "WHERE tickers_tickerGroups.ttEventType Is Null " +
		                    "AND tickers_tickerGroups.ttEventDate Is Null");
		
	}
	
	private void executeCommand(string commandToBeExecuted)
	{
		try
		{		
//			DbCommand dbCommand = new DbCommand( commandToBeExecuted , this.dbConnection );
//			DbCommand dbCommand = this.getDbCommand( commandToBeExecuted );
			DbCommand dbCommand = DbCommandProvider.GetDbCommand( commandToBeExecuted );
			int checkCommandExecution = dbCommand.ExecuteNonQuery();
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
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}
    #endregion	
	
	}
}
