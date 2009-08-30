/*
QuantProject - Quantitative Finance Library

DummyRandomFitnessEvaluator.cs
Copyright (C) 2009
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

using QuantProject.Business.Strategies.Optimizing.FitnessEvaluation;
using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Scripts.General.Strategies.Optimizing.FitnessEvaluation
{
	/// <summary>
	/// Dummy fitness Evaluator for testing purposes.
	/// It computes a random fitness value (a number between 0 and 1) 
	/// </summary>
	[Serializable]
	public class DummyRandomFitnessEvaluator : IFitnessEvaluator
	{
		public string Description
		{
			get
			{
				string description = "DummyRandomFitness";
				return description;
			}
		}
		public DummyRandomFitnessEvaluator()
		{
		}
		public double GetFitnessValue(object meaning , ReturnsManager returnsManager )
		{
			Random rnd = new Random(59);
			return rnd.NextDouble();
		}
	}
}
