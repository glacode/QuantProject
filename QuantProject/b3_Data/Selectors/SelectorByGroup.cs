/*
QuantProject - Quantitative Finance Library

SelectorByGroup.cs
Copyright (C) 2003 
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
using System.Collections;
using System.Data;
using System.Windows.Forms;
using QuantProject.DataAccess.Tables;
using QuantProject.Data.DataTables;

namespace QuantProject.Data.Selectors
{
	/// <summary>
	/// Class for selection on tickers by groupId
	/// </summary>
	public class SelectorByGroup : ITickerSelector 
	{
    private string groupID;

		public SelectorByGroup( string groupID )
		{
			this.groupID = groupID;
		}

		public DataTable GetTableOfSelectedTickers()
		{
			return QuantProject.DataAccess.Tables.Tickers_tickerGroups.GetTickers( this.groupID );        
		}
		public void SelectAllTickers()
		{
			;
		}	

	}
}
