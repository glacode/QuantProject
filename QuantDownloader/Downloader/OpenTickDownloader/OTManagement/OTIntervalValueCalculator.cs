/*
QuantProject - Quantitative Finance Library

OTIntervalValueCalculator.cs
Copyright (C) 2009
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

using OTFeed_NET;

namespace QuantProject.Applications.Downloader.OpenTickDownloader
{
	/// <summary>
	/// Computes the interval values for the given OTHistoricalType
	/// </summary>
	public class OTIntervalValueCalculator
	{
		public OTIntervalValueCalculator()
		{
		}
		
		/// <summary>
		/// returns the number of seconds for the given oTHistoricalType
		/// </summary>
		/// <param name="oTHistoricalType"></param>
		/// <returns></returns>
		public static int GetIntervalValueInSeconds( OTHistoricalType oTHistoricalType )
		{
			int intervalValueInSeconds;
			switch ( oTHistoricalType )
			{
				case OTHistoricalType.OhlcMinutely:
					intervalValueInSeconds = 60;
					break;
				case OTHistoricalType.OhlcHourly:
					intervalValueInSeconds = 3600;
					break;
				case OTHistoricalType.OhlcDaily:
					intervalValueInSeconds = 3600*24;
					break;
				case OTHistoricalType.OhlcWeekly:
					intervalValueInSeconds = 3600*24*7;
					break;
//				case OTHistoricalType.OhlcMonthly:
//					intervalValueInSeconds = 3600*24*31;  // approximation: the most common case is chosen
//					break;
//				case OTHistoricalType.OhlcYearly:
//					intervalValueInSeconds = 3600*24*365;  // approximation: the most common case is chosen
//					break;
				default:
					throw new Exception( "The given oTHistoricalType is not expected" );
			}
			return intervalValueInSeconds;
		}
	}
}
