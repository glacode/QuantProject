/*
QuantProject - Quantitative Finance Library

FitnessComparer.cs
Copyright (C) 2008 
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

namespace QuantProject.ADT.Optimizing.Fitness
{
	/// <summary>
	/// Compares BruteForceOptimizableParameters by fitness
	/// </summary>
	[Serializable]
	public sealed class FitnessComparer : IComparer
	{
		public FitnessComparer()
		{
		}
		public int Compare( object x, object y)
		{
			int returnValue = 0;
			
			if( ( x is IWithFitness ) && ( y is IWithFitness ) )	
			{
				if ( ((IWithFitness)x).Fitness > ((IWithFitness)y).Fitness )
					returnValue = 1;
				else if ( ((IWithFitness) x).Fitness < ((IWithFitness) y).Fitness )
					returnValue = -1;
			}
			else
        throw new ArgumentException( "Both objects to compare must be IWithFitness!" );
									
			return returnValue;
		}
	}
}
