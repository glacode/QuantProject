/*
QuantProject - Quantitative Finance Library

BasicDecoderForTestingPositionsWithWeights.cs
Copyright (C) 2011
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

namespace QuantProject.Business.Strategies.Optimizing.Decoding 
{
	/// <summary>
	/// Decodes optimization candidates to a 
	/// TestingPositions with weights
	/// </summary>
	[Serializable]
	public class BasicDecoderForTestingPositionsWithWeights : DecoderForTestingPositionsWithWeights
	{
		
		public BasicDecoderForTestingPositionsWithWeights() : base()
		{
		}

		protected override TestingPositions getMeaningForUndecodable()
		{
			return new TestingPositionsForUndecodableEncoded();
		}
		
		protected override string getDescription()
		{
			return "Basic_Dcdr_WithWghts";
		}
	}
}
