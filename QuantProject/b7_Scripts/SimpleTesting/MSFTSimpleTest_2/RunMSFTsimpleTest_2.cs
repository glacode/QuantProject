/*
QuantProject - Quantitative Finance Library

RunMSFTsimpleTest_2.cs
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
	/// Summary description for runMSFTsimpleTest_2.
	/// </summary>
	public class RunMSFTsimpleTest_2 : Script
	{
    public RunMSFTsimpleTest_2()
  	{
			//
			// TODO: Add constructor logic here
			//
		}

    public override void Run()
    {
      DateTime startDateTime = new DateTime( 2000 , 1 , 1 );
      DateTime endDateTime = new DateTime( 2000 , 12 , 31 );
      QuoteCache.Add( new Instrument( "MSFT" ) , BarComponent.Open );
      QuoteCache.Add( new Instrument( "MSFT" ) , BarComponent.Close );
      QuoteCache.SetCache( startDateTime , endDateTime );

      TradingSystems tradingSystems = new TradingSystems();
     
      tradingSystems.Add( new TsMSFTsimpleTest_2() );

      Tester tester = new Tester(
        new TestWindow( startDateTime , endDateTime ) ,
        tradingSystems ,
        10000 );
	  
	  tester.Account.AccountStrategy = new AsMSFTsimpleTest_2(tester.Account ); 


      //tester.Parameters.Add( new Parameter( "SMAdays" , 10 , 10 , 2 ) );

      //tester.Optimize();

      //tester.OptimalParameters.ReportToConsole();

      //tester.Parameters = tester.OptimalParameters;

      //tester.Objective();
	  
      //tester.Account.ReportToConsole( endDateTime );
		tester.Test();
      ((History)tester.Account.GetProfitNetLossHistory(
        new ExtendedDateTime( endDateTime , BarComponent.Close ) ) ).ReportToConsole();

//      tester.Account.AccountReport.ReportToExcel( "MSFT" ,
//        new ExtendedDateTime( endDateTime , BarComponent.Close ) );

      AccountReport accountReport = tester.Account.CreateReport( "MSFT" , 7 ,
        new ExtendedDateTime( endDateTime , BarComponent.Close ) , "MSFT" );
      ExcelManager.Add( accountReport );
      ExcelManager.ShowReport();
    }
	}
}
