/*
QuantProject - Quantitative Finance Library

BruteForceOptimizableParametersManagerWithoutEquivalentsAsTopBestParameters.cs
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

namespace QuantProject.ADT.Optimizing.BruteForce
{
	/// <summary>
	/// Inheriting this abstract class, all different meanings are considered
	/// to be not equivalent as top best parameters (thus, every new parameters
	/// with top fitness will be added to the TopBestParameters)
	/// </summary>
	public abstract class
		BruteForceOptimizableParametersManagerWithoutEquivalentsAsTopBestParameters :
		IBruteForceOptimizableParametersManager
	{
		public abstract int TotalIterations	{ get; }

		public abstract object Current { get; }

		public
			BruteForceOptimizableParametersManagerWithoutEquivalentsAsTopBestParameters()
		{
		}

		public abstract object Decode( BruteForceOptimizableParameters
			bruteForceOptimizableParameters );

		public abstract double GetFitnessValue(
			BruteForceOptimizableParameters bruteForceOptimizableParameters );

		public virtual bool AreEquivalentAsTopBestParameters(
			BruteForceOptimizableParameters bruteForceOptimizableParameters1 ,
			BruteForceOptimizableParameters bruteForceOptimizableParameters2 )
		{
			bool areEquivalentAsTopBestParameters =
				( bruteForceOptimizableParameters1.Meaning ==
				bruteForceOptimizableParameters2.Meaning );
			return areEquivalentAsTopBestParameters;
		}

		public abstract bool MoveNext();

		public abstract void Reset();
	}
}
