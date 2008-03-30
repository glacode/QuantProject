/*
QuantProject - Quantitative Finance Library

BruteForceOptimizer.cs
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

using QuantProject.ADT.Statistics.Combinatorial;

namespace QuantProject.ADT.Optimizing.BruteForce
{
	/// <summary>
	/// Abstract class to be inherited by those IBruteForceOptimizableParametersManager
	/// who are based on a single Combination to be scan through
	/// </summary>
	public abstract class CombinationBasedBruteForceOptimizableParametersManager :
		BruteForceOptimizableParametersManagerWithoutEquivalentsAsTopBestParameters
	{
		protected Combination combination;

		public override int TotalIterations
		{
			get
			{
				return Convert.ToInt32( this.combination.TotalNumberOfCombinations );
			}
		}

		public override object Current
		{
			get
			{
				int[] currentValues = new int[ this.combination.Length ];
				for ( int i = 0 ; i < this.combination.Length ; i ++ )
					currentValues[ i ] = this.combination.GetValue( i );
				BruteForceOptimizableParameters bruteForceOptimizableParameters =
					new BruteForceOptimizableParameters( currentValues ,
					this );
				return bruteForceOptimizableParameters;
			}
		}

		public CombinationBasedBruteForceOptimizableParametersManager(
			Combination combination )
		{
			this.combination = combination;
		}

		public override bool MoveNext()
		{
			return this.combination.MoveNext();
		}
		public override void Reset()
		{
			this.combination.Reset();
		}


	}
}
