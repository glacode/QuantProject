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
using QuantProject.Business.Strategies.Logging;
using QuantProject.Business.Strategies.OutOfSample;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Business.Strategies.Optimizing.Decoding
{
	/// <summary>
	/// Interface to be implemented by decoders for optimizers (both for
	/// genetic and for brute force optimizers) that will return a
	/// StrategyPositionsForTesting
	/// </summary>
	public interface IDecoderForTestingPositions : ILogDescriptor
	{
		/// <summary>
		/// Decodes to a TestingPositions, an
		/// object that is a candidate for an out of sample
		/// testing
		/// </summary>
				
		TestingPositions Decode( int[] encoded , EligibleTickers eligibleTickers ,
		             IReturnsManager returnsManager );
	}
}

