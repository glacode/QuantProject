/*
QuantProject - Quantitative Finance Library

HistoricalBarProvider.cs
Copyright (C) 2008
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

using QuantProject.Data.DataProviders.Bars;
using QuantProject.Data.DataProviders.Bars.Caching;

namespace QuantProject.Business.DataProviders
{
	/// <summary>
	/// Returns historical bars
	/// </summary>
	public class HistoricalBarProvider : HistoricalMarketValueProvider
	{
		private QuantProject.Data.DataProviders.Bars.HistoricalBarProvider
			historicalBarProvider;
		
		public HistoricalBarProvider( IBarCache barCache )
		{
			this.historicalBarProvider =
				new QuantProject.Data.DataProviders.Bars.HistoricalBarProvider(
					barCache );
		}
		
		public override double GetMarketValue(
			string ticker ,
			DateTime dateTime )
		{
			double marketValue = double.MinValue;
			try
			{
				marketValue = this.historicalBarProvider.GetMarketValue(
					ticker , dateTime );
			}
			catch( MissingBarException missingBarException )
			{
				string forBreakPoint = missingBarException.Message; forBreakPoint += " ";
				throw new TickerNotExchangedException( ticker , dateTime );
			}
			return marketValue;
		}
		
		protected override string getDescription()
		{
			return "barProvider";
		}
	}
}
