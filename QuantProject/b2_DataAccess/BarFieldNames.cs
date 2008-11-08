/*
QuantProject - Quantitative Finance Library

BarFieldNames.cs
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

namespace QuantProject.DataAccess
{
	/// <summary>
	/// Provides field names for the bars table
	/// </summary>
	public class BarFieldNames
	{
		public static string Open
		{
			get { return "baOpen"; }
		}
		public static string DateTimeForOpen
		{
			get { return "baDateTimeForOpen"; }
		}
		public BarFieldNames()
		{
		}
	}
}
