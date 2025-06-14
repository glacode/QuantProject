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
	[Serializable]
	public abstract class LogItem
	{
		protected DateTime realCreationTime;
		protected DateTime simulatedCreationDateTime;

		/// <summary>
		/// Real time when this log item is created
		/// </summary>
		public DateTime RealCreationTime
		{
			get { return this.realCreationTime; }
		}

		/// <summary>
		/// DateTime in the backtest, when this object is created
		/// </summary>
		public DateTime SimulatedCreationDateTime
		{
			get { return this.simulatedCreationDateTime; }
		}
		public string Simu
		{
			get { return this.simulatedCreationDateTime.ToString(); }
		}

		public LogItem( DateTime simulatedCreationDateTime )
		{
			this.realCreationTime = DateTime.Now;
			this.simulatedCreationDateTime = simulatedCreationDateTime;
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
