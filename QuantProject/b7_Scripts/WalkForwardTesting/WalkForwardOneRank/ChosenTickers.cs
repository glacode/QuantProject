/*
QuantProject - Quantitative Finance Library

ChosenTickers.cs
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

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardOneRank
{
	/// <summary>
	/// The collection of the chosen tickers, among the best performing ones
	/// </summary>
	public class ChosenTickers : Hashtable
	{
		private int numberTickersToBeChosen;

		public ChosenTickers( int numberTickersToBeChosen )
		{
			this.numberTickersToBeChosen = numberTickersToBeChosen;
		}
		#region SetTickers
		private void setTickers_build( BestPerformingTickers bestPerformingTickers )
		{
			SortedList sortedList = new SortedList();
			foreach ( string ticker in bestPerformingTickers.Keys )
				sortedList.Add( ( ( ComparableAccount )bestPerformingTickers[ ticker ]).Goodness ,
					ticker );
			for ( int n=0; n<=this.numberTickersToBeChosen ; n++ )
				this.Add( sortedList.GetByIndex( n ) , 1 );
		}
		/// <summary>
		/// Populates the collection of eligible tickers
		/// </summary>
		/// <param name="dateTime"></param>
		public void SetTickers( BestPerformingTickers bestPerformingTickers )
		{
			this.Clear();
			this.setTickers_build( bestPerformingTickers );
		}
		#endregion
	}
}
