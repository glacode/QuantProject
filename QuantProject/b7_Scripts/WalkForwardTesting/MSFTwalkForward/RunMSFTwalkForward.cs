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
using QuantProject.ADT;
using QuantProject.ADT.Histories;
using QuantProject.ADT.Optimizing;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Scripting;
using QuantProject.Business.Strategies;
using QuantProject.Business.Testing;
using QuantProject.Presentation.MicrosoftExcel;

namespace QuantProject.Scripts
{
	/// <summary>
	/// Summary description for runMSFTsimpleTest.
	/// </summary>
	public class RunMSFTwalkForward : Script
	{
    public RunMSFTwalkForward()
  	{
			//
			// TODO: Add constructor logic here
			//
		}

    public override void Run()
    {
      DateTime startDateTime = new DateTime( 1995 , 1 , 1 );
      DateTime endDateTime = new DateTime( 2003 , 9 , 1 );
      QuoteCache.Add( new Instrument( "MSFT" ) , BarComponent.Open );
      QuoteCache.Add( new Instrument( "MSFT" ) , BarComponent.Close );
      QuoteCache.SetCache( startDateTime , endDateTime );

      WalkForwardTester walkForwardTester = new WalkForwardTester();
      walkForwardTester.StartDateTime = startDateTime;
      walkForwardTester.EndDateTime = endDateTime;
      walkForwardTester.InSampleWindowNumDays = 300;
      walkForwardTester.OutOfSampleWindowNumDays = 60;
      walkForwardTester.Parameters.Add( new Parameter( "SMAdays" , 3 , 50 , 2 ) );
      walkForwardTester.Add( new TsMSFTsimpleTest() );
      walkForwardTester.Account.AddCash(
        new ExtendedDateTime( startDateTime , BarComponent.Open ) , 10000 );
      walkForwardTester.Test();
      
      
      AccountReport accountReport = walkForwardTester.Account.CreateReport( "MSFT" , 7 ,
        new ExtendedDateTime( endDateTime , BarComponent.Close ) , "MSFT" );
      ExcelManager.Add( accountReport );
      ExcelManager.ShowReport();
    }
	}
}
