/*
QuantProject - Quantitative Finance Library

RunMSFTsimpleTest.cs
Copyright (C) 2003 
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
using System.Collections;
using System.Data;
using QuantProject.ADT;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Scripting;
using QuantProject.Business.Strategies;
using QuantProject.Business.Testing;
using QuantProject.Business.Timing;
using QuantProject.Presentation.Reporting.MicrosoftExcel;

namespace QuantProject.Scripts
{
	/// <summary>
	/// Summary description for MultiTestOneRank.
	/// </summary>
	public class RunMultiTestOneRank : Script
	{
    private ArrayList backTesters;
    private ReportTable reportTable;
    private DateTime startDateTime;
    private DateTime endDateTime;
    private int numIntervalDays;

		public RunMultiTestOneRank()
		{
			this.reportTable = new ReportTable( "Summary_Reports" );
      this.startDateTime = new DateTime( 2001 , 1 , 1 );
      this.endDateTime = new DateTime( 2001 , 12 , 31 );
      this.numIntervalDays = 7;
		}
    #region Run
    #region getBackTesters
    private ArrayList getTickers()
    {
      ArrayList tickers = new ArrayList();
      tickers.Add( "RYVYX" );
      tickers.Add( "RYVNX" );
      tickers.Add( "RYVIX" );
      return tickers;
    }
    private BackTester getBackTester( string ticker )
    {
      TradingSystems tradingSystems = new TradingSystems();
      tradingSystems.Add( new TsOneRank( ticker ) );
      Tester tester = new Tester(
        new TestWindow( this.startDateTime , this.endDateTime ) ,
        tradingSystems , 10000 );
      tester.Name = ticker;
      return tester;
    }
    private ArrayList getBackTestersWithTickers( ArrayList tickers )
    {
      ArrayList backTesters = new ArrayList();
      foreach ( string ticker in tickers )
      {
        QuoteCache.Add( new Instrument( ticker ) , BarComponent.Open );
        QuoteCache.Add( new Instrument( ticker ) , BarComponent.Close );
        backTesters.Add( getBackTester( ticker ) );
      }
      return backTesters;
    }
    private ArrayList getBackTesters()
    {
      ArrayList tickers = this.getTickers();
      return this.getBackTestersWithTickers( tickers );
    }
    #endregion
    private void runBackTesters()
    {
      foreach ( BackTester backTester in this.backTesters )
        backTester.Test();
    }    
    #region "report"
    private void report_setColumns()
    {
      this.reportTable.DataTable.Columns.Add( "Ticker"  , Type.GetType( "System.String" ) );
      this.reportTable.DataTable.Columns.Add( "Return on Act" , Type.GetType( "System.Double" ) );
      this.reportTable.DataTable.Columns.Add( "B&H % Return" , Type.GetType( "System.Double" ) );
      this.reportTable.DataTable.Columns.Add( "Ann % Return" , Type.GetType( "System.Double" ) );
    }
    private void report_addRow( BackTester backTester )
    {
      AccountReport accountReport = backTester.Account.CreateReport( "" , this.numIntervalDays ,
        new EndOfDayDateTime( this.endDateTime , EndOfDaySpecificTime.OneHourAfterMarketClose ) , backTester.Name );
      DataRow newRow = this.reportTable.DataTable.NewRow();
      newRow[ "Ticker" ] = backTester.Name;
      newRow[ "Return on Act" ] = accountReport.Summary.ReturnOnAccount;
      newRow[ "B&H % Return" ] = accountReport.Summary.BuyAndHoldPercentageReturn;
      newRow[ "Ann % Return" ] = accountReport.Summary.AnnualSystemPercentageReturn;
      this.reportTable.DataTable.Rows.Add( newRow );
    }
    private void report()
    {
      this.report_setColumns();
      foreach ( BackTester backTester in this.backTesters )
        report_addRow( backTester );
      ExcelManager.Add( this.reportTable );
      ExcelManager.ShowReport();
    }
    #endregion
    public override void Run()
    {
      this.backTesters = this.getBackTesters();
      QuoteCache.SetCache( this.startDateTime , this.endDateTime );
      this.runBackTesters();
      this.report();
    }
    #endregion
	}
}
