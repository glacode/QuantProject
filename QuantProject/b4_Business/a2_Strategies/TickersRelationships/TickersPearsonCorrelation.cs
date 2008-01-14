/*
QuantProject - Quantitative Finance Library

TickersPearsonCorrelation.cs
Copyright (C) 2007 
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

using QuantProject.Business.Timing;

namespace QuantProject.Business.Strategies.TickersRelationships
{
  /// <summary>
  /// Struct used for the storage of correlation info 
  /// </summary>
  [Serializable]
  public struct TickersPearsonCorrelation : IComparable
  {
    private string firstTicker;
		public string FirstTicker
		{
			get
			{
				return this.firstTicker;
			}
		}
    
		private string secondTicker;
		public string SecondTicker
		{
			get
			{
				return this.secondTicker;
			}
		}

    private double correlationValue;
		public double CorrelationValue
		{
			get
			{
				return this.correlationValue;
			}
		}
    
		public TickersPearsonCorrelation( string firstTicker,
					 string secondTicker, double pearsonCorrelationValue)
    {
    	this.firstTicker = firstTicker;
			this.secondTicker = secondTicker;
			this.correlationValue = pearsonCorrelationValue;
    }

		public int CompareTo(object correlationToBeCompared)
		{
			int returnValue = 1;
			TickersPearsonCorrelation toBeCompared = (TickersPearsonCorrelation)correlationToBeCompared;
			if( this.correlationValue < toBeCompared.correlationValue)
				returnValue = - 1;
			else if (this.correlationValue == toBeCompared.correlationValue)
				returnValue = 0;
//			else if (this.correlationValue > toBeCompared.correlationValue)
//				returnValue = 1;

			return returnValue;
		}
  }
}
