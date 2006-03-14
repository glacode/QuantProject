/*
QuantProject - Quantitative Finance Library

QPHashtable.cs
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
using System.Runtime.Serialization;

namespace QuantProject.ADT.Collections
{
	/// <summary>
	/// Extends standard Hashtable features
	/// </summary>
	[Serializable]
	public class QPHashtable : Hashtable
	{
		/// <summary>
		/// Keys are returned in a single (semicolon separated)
		/// string. Useful for debugging purposes
		/// </summary>
		public string KeyConcat
		{
			get
			{
				string keyConcat = "";
				foreach ( object key in this.Keys )
					keyConcat += ";" + key.ToString();
				return keyConcat;
			}
		}
		public QPHashtable()
		{
			//
			// TODO: Add constructor logic here
			//
		}
		public QPHashtable(SerializationInfo info, StreamingContext context)
			: base(info, context)
	{
	}

	}
}
