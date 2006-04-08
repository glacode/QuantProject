/*
QuantProject - Quantitative Finance Library

WFLagChosenPositions.cs
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

using QuantProject.ADT.Collections;
using QuantProject.Scripts.WalkForwardTesting.WalkForwardLag;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger
{
	/// <summary>
	/// Creates a copy of the relevant data for the WFLagChosenTickers object
	/// </summary>
	[Serializable]
	public class WFLagChosenPositions
	{
		private QPHashtable drivingPositions;
		private QPHashtable portfolioPositions;
		private DateTime lastOptimizationDate;

		public QPHashtable DrivingPositions
		{
			get
			{
				return this.drivingPositions;
			}
		}
		public QPHashtable PortfolioPositions
		{
			get
			{
				return this.portfolioPositions;
			}
		}
		public DateTime LastOptimizationDate
		{
			get
			{
				return this.lastOptimizationDate;
			}
		}

		public WFLagChosenPositions( WFLagChosenTickers wFLagChosenTickers ,
			DateTime lastOptimizationDate )
		{
			this.drivingPositions =
				this.copy( wFLagChosenTickers.DrivingPositions );
			this.portfolioPositions =
				this.copy( wFLagChosenTickers.PortfolioPositions );
			this.lastOptimizationDate = lastOptimizationDate;
		}

		private QPHashtable copy( QPHashtable qPHashTable )
		{
			QPHashtable newCopy = new QPHashtable();
			foreach ( string key in qPHashTable.Keys )
				newCopy.Add( key , null );
			return newCopy;
		}
	}
}
