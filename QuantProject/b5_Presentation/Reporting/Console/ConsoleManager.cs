/*
QuantProject - Quantitative Finance Library

ConsoleManager.cs
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
using QuantProject.Business.Financial.Accounting;

namespace QuantProject.Presentation.Reporting.Console
{
	/// <summary>
	/// Contains the static methods to write an account
	/// report to the console
	/// </summary>
	public class ConsoleManager
	{
		public ConsoleManager()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public static void Report( Account account , DateTime dateTime )
		{
			System.Console.WriteLine( "\n\n\n***********\n" );
			System.Console.WriteLine( "Report for Account: " + account.Key );
			System.Console.WriteLine( account.ToString( dateTime ) );
			System.Console.WriteLine( account.Transactions.ToString() );
		}
	}
}
