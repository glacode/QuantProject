/*
QuantProject - Quantitative Finance Library

IEquityEvaluator.cs
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

using QuantProject.Business.Strategies.Logging;

namespace QuantProject.Business.Strategies.EquityEvaluation
{
	/// <summary>
	/// Interface to be implemented by equity line evaluators
	/// </summary>
	public interface IEquityEvaluator : ILogDescriptor
	{
//		/// <summary>
//		/// Short description (it might be used in file names describing the strategy)
//		/// </summary>
//		string Description { get; }

		float GetReturnsEvaluation( float[] returns );
	}
}
