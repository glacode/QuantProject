/*
QuantProject - Quantitative Finance Library

BarIdentifier.cs
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

using QuantProject.ADT.Messaging;

namespace QuantProject.Applications.Downloader.OpenTickDownloader
{
	/// <summary>
	/// Selects an exchange for a given ticker
	/// </summary>
	public interface IExchangeSelector : IMessageSender
	{
		/// <summary>
		/// returns an identifier for an exchange where the ticker is traded 
		/// </summary>
		/// <param name="ticker"></param>
		/// <returns></returns>
		string SelectExchange( string ticker );
	}
}
