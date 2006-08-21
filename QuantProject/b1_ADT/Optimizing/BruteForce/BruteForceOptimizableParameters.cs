/*
QuantProject - Quantitative Finance Library

BruteForceOptimizableParameters.cs
Copyright (C) 2006
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

namespace QuantProject.ADT.Optimizing.BruteForce
{
	/// <summary>
	/// Parameters to be optimized by a Brute Force optimizer
	/// </summary>
	public class BruteForceOptimizableParameters
	{
		IBruteForceOptimizableParametersManager bruteForceOptimizableParametersManager;

		private bool isFitnessSet;

		private double fitness;

		private int[] parameterValues;

		/// <summary>
		/// Fitness value for current parameters value
		/// </summary>
		public double Fitness
		{
			get
			{
				if ( !this.isFitnessSet )
				{
					this.fitness = this.bruteForceOptimizableParametersManager.GetFitnessValue(
						this );
					this.isFitnessSet = true;
				}
				return this.fitness;
			}
		}
		public int Length
		{
			get
			{
				return this.parameterValues.Length;
			}
		}
		public BruteForceOptimizableParameters( int[] parameterValues ,
			IBruteForceOptimizableParametersManager
			bruteForceOptimizableParametersManager )
		{
			this.parameterValues = parameterValues;
			this.bruteForceOptimizableParametersManager =
				bruteForceOptimizableParametersManager;
			this.isFitnessSet = false;
		}
		public int[] GetValues()
		{
			return this.parameterValues;
		}
	}
}
