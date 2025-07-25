
/*
QuantProject - Quantitative Finance Library

SharpeRatio.cs
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

using QuantProject.ADT.Statistics;

namespace QuantProject.Business.Strategies.EquityEvaluation
{
	/// <summary>
	/// Equity line evaluator, measuring the well known
	/// risk to reward Sharpe Ratio
	/// </summary>
	[Serializable]
	public class SharpeRatio : IEquityEvaluator
	{
		public string Description
		{
			get
			{
				return "EqEvltr_shrpRt";
			}
		}

		public SharpeRatio()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public float GetReturnsEvaluation( float[] returns )
		{
			return Convert.ToSingle( AdvancedFunctions.GetSharpeRatio( returns ) );
		}
	}
}
