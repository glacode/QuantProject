/*
QuantProject - Quantitative Finance Library

LogItem.cs
Copyright (C) 2007
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

using QuantProject.Business.Timing;

namespace QuantProject.Business.Strategies.Logging
{
	/// <summary>
	/// Single information (about a backtest) to be logged
	/// </summary>
	public abstract class LogItem
	{
		protected DateTime creationTime;
		protected EndOfDayDateTime endOfDayDateTime;

		public LogItem( EndOfDayDateTime endOfDayDateTime )
		{
			this.creationTime = DateTime.Now;
			this.endOfDayDateTime = endOfDayDateTime;
		}
		/// <summary>
		/// Since LogItem(s) are usually used to store in sample
		/// optimization results, this method is used to run a script
		/// to show the optimization result. More generally, this
		/// method is used to show some information about the LogItem
		/// </summary>
		public abstract void Run();
	}
}
