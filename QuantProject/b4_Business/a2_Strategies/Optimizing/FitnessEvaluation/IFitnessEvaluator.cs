/*
QuantProject - Quantitative Finance Library

DecoderForBalancedWeightedPositions.cs
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

using QuantProject.Business.Strategies.Logging;

using QuantProject.Business.Strategies.ReturnsManagement;

namespace QuantProject.Business.Strategies.Optimizing.FitnessEvaluation
{
	/// <summary>
	/// Interface to be implemented by fitness evaluators
	/// </summary>
	public interface IFitnessEvaluator : ILogDescriptor
	{
		/// <summary>
		/// Computes the fitness value for the given meaning,
		/// measured on the given returns
		/// </summary>
		/// <param name="meaning"></param>
		/// <param name="returnsManager"></param>
		/// <returns></returns>
		double GetFitnessValue( object meaning , IReturnsManager returnsManager );
	}
}
