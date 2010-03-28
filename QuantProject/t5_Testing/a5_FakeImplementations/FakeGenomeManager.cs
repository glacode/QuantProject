/*
QuantProject - Quantitative Finance Library

FakeGenomeManager.cs
Copyright (C) 2010
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

using QuantProject.ADT.Optimizing.Genetic;

namespace QuantTesting.ADT.Optimizing.Genetic
{
	/// <summary>
	/// Fake genome manager
	/// </summary>
	public class FakeGenomeManager : IGenomeManager
	{
		private int genomeSize;
		public int GenomeSize{ get { return this.genomeSize; }}
		
		public FakeGenomeManager( int genomeSize )
		{
			this.genomeSize = genomeSize;
		}
		
		public int GetMaxValueForGenes(int genePosition)
		{
			return 10000;
		}
		
		public int GetMinValueForGenes(int genePosition)
		{
			return -10000;
		}
		public int GetNewGeneValue(Genome genome, int genePosition)
		{
			return 3;
		}
		
		public object Decode(Genome genome)
		{
			return null;
		}
		public double GetFitnessValue(Genome genome)
		{
			return 0;
		}
		public Genome[] GetChilds(Genome parent1, Genome parent2)
		{
			return null;
		}
		public void Mutate(Genome genome)
		{
			;
		}
	}
}
