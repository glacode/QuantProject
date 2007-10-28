/*
QuantProject - Quantitative Finance Library

WFLagNewChosenPositionsEventArgs.cs
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

using QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag
{
	/// <summary>
	/// EventArgs for new chosen positions for the walk forward
	/// Lag Strategy
	/// </summary>
	public class WFLagNewChosenPositionsEventArgs : EventArgs
	{
		private WFLagLogItem wFLagLogItem;

		public WFLagLogItem WFLagLogItem
		{
			get
			{
				return this.wFLagLogItem;
			}
		}

		public WFLagNewChosenPositionsEventArgs(
			WFLagLogItem wFLagLogItem )
		{
			this.wFLagLogItem = wFLagLogItem;
		}
	}
}
