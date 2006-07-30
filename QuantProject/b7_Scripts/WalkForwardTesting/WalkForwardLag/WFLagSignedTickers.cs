/*
QuantProject - Quantitative Finance Library

WFLagSignedTickers.cs
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

using QuantProject.ADT.Collections;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
	/// <summary>
	/// This class identifies all the signed tickers to apply
	/// the lag strategy out of sample: it contains both the
	/// driving positions and the portfolio positions. Each genome
	/// can be decoded to an instance of this class
	/// </summary>
	public class WFLagSignedTickers
	{

		private QPHashtable drivingPositions;
		private QPHashtable portfolioPositions;

		public QPHashtable DrivingPositions
		{
			get { return this.drivingPositions; }
		}
		public QPHashtable PortfolioPositions
		{
			get { return this.portfolioPositions; }
		}

		public WFLagSignedTickers()
		{
			this.drivingPositions = new QPHashtable();
			this.portfolioPositions = new QPHashtable();
		}
	}
}
