/*
QuantProject - Quantitative Finance Library

TickerSelectorByGroup.cs
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
using System.Data;

namespace QuantProject.Data.Selectors
{
	/// <summary>
	/// Selects the list of tickers that belonged to a given group, at a given time
	/// </summary>
	[Serializable]
	public class TickerSelectorByGroup : ITickerSelectorByGroup
	{
		public TickerSelectorByGroup()
		{
		}
		
		public List<string> GetSelectedTickers( string groupId , DateTime date )
		{
			SelectorByGroup selectorByGroup = new SelectorByGroup( groupId , date );
			DataTable dataTableOfSelectedTickers = selectorByGroup.GetTableOfSelectedTickers();
			List<string> selectedTickers = TickerSelector.GetList( dataTableOfSelectedTickers );
			return selectedTickers;
		}
	}
}
