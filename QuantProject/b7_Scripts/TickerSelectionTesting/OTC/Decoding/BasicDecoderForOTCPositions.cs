/*
QuantProject - Quantitative Finance Library

BasicDecoderForOTCPositions.cs
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

using QuantProject.ADT.Collections;
using QuantProject.ADT.Optimizing.Decoding;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.Optimizing.Decoding;

namespace QuantProject.Scripts.TickerSelectionTesting.OTC 
{
	/// <summary>
	/// Decodes optimization candidates to a 
	/// OTCPositions
	/// </summary>
	[Serializable]
	public class BasicDecoderForOTCPositions : BasicDecoderForTestingPositions
	{
		
		public BasicDecoderForOTCPositions() : base()
		{
		}

		protected override TestingPositions getMeaningForUndecodable()
		{
			return new OTCPositions();
		}
		
		protected override TestingPositions decodeDecodable()
		{
			SignedTickers signedTickers =	this.decodeSignedTickers();
			OTCPositions otcPositions =	new OTCPositions(
				new WeightedPositions( this.getUnsignedWeights(signedTickers), signedTickers) );
			
			return otcPositions;
		}
		
		protected override string getDescription()
		{
			return "OTC_Dcdr_NoWghts";
		}
	}
}

