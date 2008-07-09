/*
QuantProject - Quantitative Finance Library

IBarsSelector.cs
Copyright (C) 2008 
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

namespace QuantProject.Applications.Downloader.OpenTickDownloader
{
	/// <summary>
	/// Interface to be implemented by bars selectors. A bar
	/// selector selects some OpenHighLowClose bars (that then
	/// will be downloaded)
	/// </summary>
	public interface IBarsSelector
	{
		/// <summary>
		/// returns the next BarIdentifier (for the historical OHLC bar
		/// to be requested)
		/// </summary>
		/// <returns></returns>
		BarIdentifier GetNextBarIdentifier();
		
		/// <summary>
		/// true iif all bars have already been selected and
		/// signaled outside
		/// </summary>
		bool AreAllBarsAlredyGiven { get; }
		
	}
}
