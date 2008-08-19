/*
QuantProject - Quantitative Finance Library

GenomeRepresentation.cs
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
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections;

namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Data to be saved/load to/from disk, resulting from the
	/// optimization process.
	/// </summary>
	[Serializable]
	public class OptimizationOutput : ArrayList, ISerializable
	{
//		public ArrayList BestGenomes
//		{
//			get { return this.bestGenomes; }
//		}
		
		public OptimizationOutput()
		{
			
		}
		/// <summary>
		/// Adds a genome representation
		/// </summary>
		/// <param name="genomeRepresentation"></param>
		public void Add( GenomeRepresentation genomeRepresentation )
		{
			base.Add( genomeRepresentation );
		}
		
		/// <summary>
		/// This constructor allows custom deserialization (see the ISerializable
		/// interface documentation)
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected OptimizationOutput( SerializationInfo info , StreamingContext context )
		{
			// get the set of serializable members for this class and its base classes
			Type thisType = this.GetType();
			MemberInfo[] mi = FormatterServices.GetSerializableMembers(
				thisType , context);

			// deserialize the fields from the info object
			for (Int32 i = 0 ; i < mi.Length; i++)
			{
				FieldInfo fieldInfo = (FieldInfo) mi[i];

				// set the field to the deserialized value
				try{
					fieldInfo.SetValue( this ,
					                   info.GetValue( fieldInfo.Name, fieldInfo.FieldType ) );
				}
				catch(Exception ex)
				{
					string forBreakpoint = ex.Message; forBreakpoint = forBreakpoint + "";
				}
			}

		}
		
		#region GetObjectData
		/// <summary>
		/// serialize the set of serializable members for this class and base classes
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(
			SerializationInfo info, StreamingContext context)
		{
			// get the set of serializable members for this class and base classes
			Type thisType = this.GetType();
			MemberInfo[] mi =
				FormatterServices.GetSerializableMembers( thisType , context);

			// serialize the fields to the info object
			for (Int32 i = 0 ; i < mi.Length; i++)
			{
				info.AddValue(mi[i].Name, ((FieldInfo) mi[i]).GetValue(this));
			}
		}
		#endregion
		
	}
}
