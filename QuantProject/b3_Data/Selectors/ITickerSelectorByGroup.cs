/*
QuantProject - Quantitative Finance Library

ITickerSelectorByGroup.cs
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
using System.Collections.Generic;

namespace QuantProject.Data.Selectors
{
	/// <summary>
	/// Interface to be implemented by a ticker selector that
	/// selects the tickers in a group, at a given DateTime
	/// </summary>
	public interface ITickerSelectorByGroup
	{
		/// <summary>
		/// selects the tickers in a group, at a given DateTime
		/// </summary>
		/// <param name="groupId"></param>
		/// <param name="dateTime"></param>
		/// <returns></returns>
		List<string> GetSelectedTickers( string groupId , DateTime dateTime );
	}
}
