/*
QuantProject - Quantitative Finance Library

WinningPeriods.cs
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

namespace QuantProject.Business.Strategies.EquityEvaluation
{
	/// <summary>
	/// Equity line evaluator, measuring the percentage of winning
	/// periods on the total number of non flat periods
	/// </summary>
	[Serializable]
	public class WinningPeriods : IEquityEvaluator
	{
		public string Description
		{
			get
			{
				return "EqEvltr_wnngPrds";
			}
		}

		public WinningPeriods()
		{
		}
		public float GetReturnsEvaluation( float[] returns )
		{
			int winningPeriods = 0;
			int losingPeriods = 0;
			for ( int i = 0 ; i < returns.Length ; i++ )
			{
				if ( returns[ i ] > 0 )
					winningPeriods++;
				if ( returns[ i ] < 0 )
					losingPeriods++;
			}
			return ( Convert.ToSingle( winningPeriods ) /
				Convert.ToSingle( winningPeriods + losingPeriods ) );
		}
	}
}
