/*
QuantProject - Quantitative Finance Library

PriceRatio.cs
Copyright (C) 2009 
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

namespace QuantProject.Scripts.TechnicalAnalysisTesting.Oscillators.FixedLevelOscillators.PortfolioValueOscillator.InSampleChoosers.PriceRatioChooser
{
  /// <summary>
  /// Struct used for the storage of price ratio info 
  /// </summary>
  [Serializable]
  public struct PriceRatio : IComparable
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

    private double average;
		public double Average
		{
			get
			{
				return this.average;
			}
			set
			{
				this.average = value;
			}
		}
    
		private double absoluteStandardDeviation;
		public double AbsoluteStandardDeviation
		{
			get
			{
				return this.absoluteStandardDeviation;
			}
			set
			{
				this.absoluteStandardDeviation = value;
			}
		}
		
		public double RelativeStandardDeviation
		{
			get
			{
				return this.AbsoluteStandardDeviation / this.Average;
			}
			
		}
		
		public PriceRatio( string firstTicker,
					 string secondTicker, double average, double absoluteStandardDeviation)
    {
    	this.firstTicker = firstTicker;
			this.secondTicker = secondTicker;
			this.average = average;
			this.absoluteStandardDeviation = absoluteStandardDeviation;
    }

		public int CompareTo(object priceRatioToBeCompared)
		{
			int returnValue = 1;
			PriceRatio toBeCompared = (PriceRatio)priceRatioToBeCompared;
			if( this.RelativeStandardDeviation < toBeCompared.RelativeStandardDeviation)
				returnValue = - 1;
			else if (this.RelativeStandardDeviation == toBeCompared.RelativeStandardDeviation)
				returnValue = 0;
//			else if (this.RelativeStandardDeviation > toBeCompared.RelativeStandardDeviation)
//				returnValue = 1;

			return returnValue;
		}
  }
}
