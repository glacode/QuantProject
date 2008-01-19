/*
QuantProject - Quantitative Finance Library

FixedLengthTwoPhasesLogItem.cs
Copyright (C) 2007
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

using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Timing;

namespace QuantProject.Scripts.WalkForwardTesting.FixedLengthTwoPhases
{
	/// <summary>
	/// Log item for the FixedLengthTwoPhases strategy
	/// </summary>
	public class FixedLengthTwoPhasesLogItem : LogItem
	{
		private WeightedPositions bestWeightedPositionsInSample;
		private int numberOfEligibleTickers;
		
		public WeightedPositions BestWeightedPositionsInSample
		{
			get
			{
				if ( this.bestWeightedPositionsInSample == null )
					throw new Exception( "This property has not " +
						"been assigned yet! If you are loading the LogItem from " +
						"a log, this property was not set before logging the LogItem." );
				return this.bestWeightedPositionsInSample;
			}
			set { this.bestWeightedPositionsInSample = value; }
		}

		public int NumberOfEligibleTickers
		{
			get
			{
				if ( this.numberOfEligibleTickers == int.MinValue )
					throw new Exception( "This property has not " +
						"been assigned yet! If you are loading the LogItem from " +
						"a log, this property was not set before logging the LogItem." );
				return this.numberOfEligibleTickers;
			}
			set { this.numberOfEligibleTickers = value; }
		}

		public FixedLengthTwoPhasesLogItem( EndOfDayDateTime endOfDayDateTime )
			: base( endOfDayDateTime )
		{
			this.numberOfEligibleTickers = int.MinValue;
		}
		public override void Run()
		{
			// TO DO write a script to debug the in sample
			// optimization result
		}
	}
}
