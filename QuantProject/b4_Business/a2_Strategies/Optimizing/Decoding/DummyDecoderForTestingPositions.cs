/*
QuantProject - Quantitative Finance Library

IDecoderForTestingPositions.cs
Copyright (C) 2008
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

using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Business.Strategies.Optimizing.Decoding
{
	/// <summary>
	/// A dummy decoder for testing positions (to be used when, for instance,
	/// the GenomeManager has its own decoder)
	/// </summary>
	[Serializable]
	public class DummyDecoderForTestingPositions : IDecoderForTestingPositions
	{
		public DummyDecoderForTestingPositions()
		{
		}
		/// <summary>
		/// this is a dummy method that we add just to implement the IDecoderForTestingPositions
		/// interface, but this decoder is ment to be used by a GenomeManagerForLinearRegression
		/// and it will use the Decode with four parameters (see above)
		/// </summary>
		/// <param name="encoded"></param>
		/// <param name="eligibleTickers"></param>
		/// <param name="returnsManager"></param>
		/// <returns></returns>
		public TestingPositions Decode( int[] encoded , EligibleTickers eligibleTickers ,
		                               IReturnsManager returnsManager )
		{
			throw new Exception( "This method should never be invoked, this is a dummy " +
			                    "method!" );
//			return null;
		}
		protected virtual string getDescription()
		{
			return "BscFrTstngPstns";
		}
		public string Description
		{
			get
			{
				string description = "Dcdr_" + this.getDescription();
				return description;
			}
		}
	}
}
