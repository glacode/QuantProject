/*
QuantProject - Quantitative Finance Library

GenerationWithoutDuplicatedFitness.cs
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
using System.Collections;

namespace QuantProject.ADT.Optimizing.Genetic
{
	/// <summary>
	/// This generation doesn't allow to add a genome, if a genome with the same
	/// fitnessApproximation is already in the generation
	/// </summary>
	[Serializable]
	public class GenerationWithoutDuplicatedFitness : ArrayList
	{
		private IFitnessDiscretizer fitnessDiscretizer;
		
		private Hashtable discretizedFitnessesAlreadyAdded;
		
		public GenerationWithoutDuplicatedFitness(
			int populationSize , IFitnessDiscretizer fitnessDiscretizer ) :
			base( populationSize )
		{
			this.fitnessDiscretizer = fitnessDiscretizer;
			this.discretizedFitnessesAlreadyAdded = new Hashtable();
		}
		
		private bool canBeAdded( Genome genome )
		{
			double discreteFitness = this.fitnessDiscretizer.Discretize( genome.Fitness );
			bool canBeAdded = !this.discretizedFitnessesAlreadyAdded.ContainsKey( discreteFitness );
			return canBeAdded;
		}
		
		public override int Add( object value )
		{
			int returnValue = int.MinValue;			
			
			if ( this.canBeAdded( (Genome)value ) )
			{
				returnValue =  base.Add(value);
				double discreteFitness = this.fitnessDiscretizer.Discretize(
					((Genome)value).Fitness );
				this.discretizedFitnessesAlreadyAdded.Add( discreteFitness , discreteFitness );
			}
//			if ( !this.CanBeAdded( (Genome)value ) )
//				throw new Exception(
//					"The genome has already been added! " +
//					"Please, Use the method CanBeAdded() before using Add()" );			
			return returnValue;
		}
		
		public override void Clear()
		{
			base.Clear();
			this.discretizedFitnessesAlreadyAdded.Clear();
		}
	}
}
