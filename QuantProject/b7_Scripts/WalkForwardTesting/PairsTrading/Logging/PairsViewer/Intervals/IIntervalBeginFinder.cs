/*
QuantProject - Quantitative Finance Library

IIntervalBeginFinder.cs
Copyright (C) 2009
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

namespace QuantProject.Scripts.WalkForwardTesting.PairsTrading
{
	/// <summary>
	/// Returns the interval begin to be used by the ReturnsComputer
	/// </summary>
	public interface IIntervalBeginFinder
	{
		/// <summary>
		/// returns the first date time for the interval that ends on dateTime: the
		/// interval that comes up will be used to calculate a return
		/// </summary>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		DateTime GetIntervalBeginDateTime( DateTime dateTime );
	}
}
