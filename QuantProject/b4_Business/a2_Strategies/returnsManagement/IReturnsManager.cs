/*
QuantProject - Quantitative Finance Library

IReturnsManager.cs
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

using QuantProject.Business.DataProviders;
using QuantProject.Business.Strategies.ReturnsManagement.Time;

namespace QuantProject.Business.Strategies.ReturnsManagement
{
	/// <summary>
	/// Interface to be implemented by those classes that compute returns
	/// on intervals
	/// </summary>
	public interface IReturnsManager
	{
		IHistoricalMarketValueProvider HistoricalMarketValueProvider{ get; }
		int NumberOfReturns{ get; }
		ReturnIntervals ReturnIntervals{ get; }
		float GetReturn( string ticker , int returnIndex );
		float[] GetReturns( string ticker );
		float GetReturnsStandardDeviation( string ticker );
	}
}
