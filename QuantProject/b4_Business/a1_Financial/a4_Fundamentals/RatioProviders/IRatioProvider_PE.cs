/*
QuantProject - Quantitative Finance Library

IRatioProvider_PE.cs
Copyright (C) 2010
Marco Milletti

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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.*/

using System;

namespace QuantProject.Business.Financial.Fundamentals.RatioProviders
{
	/// <summary>
	/// Interface to be implemented by objects implementing a way of computing
	/// the popular PE Ratio
	/// </summary>
	public interface IRatioProvider_PE
	{
		double GetPERatio( string ticker , DateTime atDate );
	}
}
