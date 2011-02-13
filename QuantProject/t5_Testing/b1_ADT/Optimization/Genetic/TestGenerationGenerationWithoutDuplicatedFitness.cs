/*
QuantProject - Quantitative Finance Library

TestGenerationWithDuplicatedGenomes.cs
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
	/// Test for the class GenerationWithDuplicatedGenomes
	/// </summary>
	public class TestGenerationWithDuplicatedGenomes
	{
		[Test]
		public void TestMethod()
		{
			DynamicMock dynamicFitnessDiscretizer = new DynamicMock(
				typeof( IFitnessDiscretizer ) );

			dynamicFitnessDiscretizer.SetReturnValue( "Discretize" , 0.345 );
			
			GenerationWithoutDuplicatedFitness generationWithoutDuplicatedFitness =
				new GenerationWithoutDuplicatedFitness(
					10 , (IFitnessDiscretizer)dynamicFitnessDiscretizer.MockInstance );

			DynamicMock dynamicGenomeManager = new DynamicMock(
				typeof( IGenomeManager ) );
			GeneticOptimizer geneticOptimizer = new GeneticOptimizer(
				(IGenomeManager)dynamicGenomeManager.MockInstance , 20 , 15 );
			Genome genome = new Genome(
				(IGenomeManager)dynamicGenomeManager.MockInstance , geneticOptimizer );
//			bool canBeAdded = generationWithoutDuplicatedFitness.Add( genome );
			generationWithoutDuplicatedFitness.Add( genome );
			Assert.AreEqual( 1 , generationWithoutDuplicatedFitness.Count );
			
//			generationWithoutDuplicatedFitness.Add( genome );
			
//			canBeAdded = generationWithoutDuplicatedFitness.Add( genome );
			generationWithoutDuplicatedFitness.Add( genome );
			Assert.AreEqual( 1 , generationWithoutDuplicatedFitness.Count );
		}
	}
}
