/*
QuantProject - Quantitative Finance Library

TestFitnessApproximator.cs
Copyright (C) 2011
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
using NUnit.Framework;
using NUnit.Mocks;

using QuantProject.ADT.Optimizing.Genetic;

namespace QuantTesting.ADT.Optimizing.Genetic
{
	[TestFixture]
	/// <summary>
	/// Test for the class FitnessApproximator
	/// </summary>
	public class TestFitnessApproximator
	{
		[Test]
		public void TestMethod()
		{
			FitnessApproximator fitnessApproximator = new FitnessApproximator( 3 );
			
			double approximation = fitnessApproximator.Discretize( 1.67895 );
			Assert.AreEqual( 1.679 , approximation  );
			
			approximation = fitnessApproximator.Discretize( 1.67849 );
			Assert.AreEqual( 1.678 , approximation  );
			
			approximation = fitnessApproximator.Discretize( 0.3456 );
			Assert.AreEqual( 0.346 , approximation  );
		}
	}
}
