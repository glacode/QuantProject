/*
QuantProject - Quantitative Finance Library

EmptyQueryException.cs
Copyright (C) 2009 
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
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/
using System;

namespace QuantProject.DataAccess
{
	/// <summary>
	/// It can be thrown when a query is launched, but no row is returned
	/// </summary>
	public class EmptyQueryException : Exception
	{
		private string sqlQuery;
		public override string Message
		{
			get
			{
				return "No row for this query:  " +	this.sqlQuery;
			}
		}
		public EmptyQueryException( string sqlQuery )
		{
			this.sqlQuery = sqlQuery;
		}
	}
}
