/*
QuantProject - Quantitative Finance Library

RunProfunds.cs
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
using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.ADT.Optimizing;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Testing;
using QuantProject.Business.Strategies;
using QuantProject.Business.Scripting;
using QuantProject.Presentation.MicrosoftExcel;

namespace QuantProject.Scripts
{
	/// <summary>
	/// Summary description for RunProfunds.
	/// </summary>
	public class RunOneRank : Script
	{
    public RunOneRank()
    {
    }
    public override void Run()
    {
      //this.testTicker( "FSELX" , new DateTime( 2003 , 5 , 20 ) , new DateTime( 2003 , 9 , 26 ) );
      this.testTicker( "REPIX" , new DateTime( 2003 , 7 , 1 ) , new DateTime( 2003 , 11 , 20 ) );
    }
    private void testTicker( string ticker , DateTime startDateTime , DateTime endDateTime )
    {
      QuoteCache.Add( new Instrument( ticker ) , BarComponent.Open );
      QuoteCache.Add( new Instrument( ticker ) , BarComponent.Close );
      //QuoteCache.Add( new Instrument( "MSFT" ) , BarComponent.Close );
      QuoteCache.SetCache( startDateTime , endDateTime );

      TradingSystems tradingSystems = new TradingSystems();
     
      tradingSystems.Add( new TsOneRank( ticker  ) );

      Tester tester = new Tester(
        new TestWindow( startDateTime , endDateTime ) ,
        tradingSystems ,
        10000 );

      tester.Account.AccountStrategy = new AsOneRank( tester.Account );

      //tester.Parameters.Add( new Parameter( "SMAdays" , 10 , 10 , 2 ) );

      //tester.Optimize();

      tester.Test();

        //tester.Account.ReportToConsole( endDateTime );

      ((History)tester.Account.GetProfitNetLossHistory(
        new ExtendedDateTime( endDateTime , BarComponent.Close ) ) ).ReportToConsole();

      //      tester.Account.AccountReport.ReportToExcel( "MSFT" ,
      //        new ExtendedDateTime( endDateTime , BarComponent.Close ) );

      AccountReport accountReport = tester.Account.CreateReport( ticker , 7 ,
        new ExtendedDateTime( endDateTime , BarComponent.Close ) , ticker );
      ExcelManager.Add( accountReport );
      ExcelManager.ShowReport();
    }
	}
}
