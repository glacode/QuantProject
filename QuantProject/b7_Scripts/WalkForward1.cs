/*
QuantProject - Quantitative Finance Library

WalkForward1.cs
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
using QuantProject.Business.Financial.Instruments;


namespace QuantProject.Scripts
{
	/// <summary>
	/// Summary description for WalkForward1.
	/// </summary>
	public class WalkForward1
	{
		public WalkForward1()
		{
			//
			// TODO: Add constructor logic here
			//
		}

    public static void Run()
    {
      DataProvider.Add( new Instrument( "MSFT" ) );

      MyTradingSystem tradingSystem = new MyTradingSystem();

      Account account = new Account( "account" );
      account.AddCash( new DateTime( 2000 , 1 , 1 ) , 10000 );

      WalkForwardTester walkForwardTester = new WalkForwardTester();
      //walkForwardTester.StartDateTime = new DateTime( 1995 , 1 , 1 );
      walkForwardTester.StartDateTime = new DateTime( 2000 , 1 , 1 );
      walkForwardTester.EndDateTime = new DateTime( 2002 , 7 , 31 );
      //walkForwardTester.InSampleWindowNumDays = 600;
      //walkForwardTester.OutOfSampleWindowNumDays = 30;
      walkForwardTester.InSampleWindowNumDays = 200;
      walkForwardTester.OutOfSampleWindowNumDays = 30;
      walkForwardTester.Add( tradingSystem );
      //walkForwardTester.Parameters.Add( new Parameter( "SMAdays" , 2 , 4 , 0.5 ) );
      //walkForwardTester.Parameters.Add( new Parameter( "CrossPercentage" , 0 , 5 , 1 ) );
      walkForwardTester.Parameters.Add( new Parameter( "SMAdays" , 3 , 9 , 2) );
      walkForwardTester.Parameters.Add( new Parameter( "CrossPercentage" , 0 , 1 , 1 ) );
      walkForwardTester.Add( account );
      walkForwardTester.Test();
      walkForwardTester.GetAccount( "account" ).DrawReport();
    }
	}
}
