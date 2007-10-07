/*
QuantProject - Quantitative Finance Library

WFLagChosenPositions.cs
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
using System.Reflection;
using System.Runtime.Serialization;

using QuantProject.ADT.Collections;
using QuantProject.Business.Strategies;
using QuantProject.Scripts.WalkForwardTesting.WalkForwardLag;

namespace QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WFLagDebugger
{
	/// <summary>
	/// Data to be logged (out of sample) for each new optimization
	/// </summary>
	[Serializable]
	public class WFLagChosenPositions : ISerializable
	{
		// these private members are used for old versions' deserialization, only
		private QPHashtable drivingPositions;
		private QPHashtable portfolioPositions;

		private WeightedPositions drivingWeightedPositions;
		private WeightedPositions portfolioWeightedPositions;
		private DateTime lastOptimizationDate;
		private int generation;

		public WeightedPositions DrivingWeightedPositions
		{
			get
			{
				if ( this.drivingWeightedPositions == null )
					// an old version has been deserialized
					this.setWeightedPositionsFromQPHashtables();
				return this.drivingWeightedPositions;
			}
		}
		public WeightedPositions PortfolioWeightedPositions
		{
			get
			{
				if ( this.portfolioWeightedPositions == null )
					// an old version has been deserialized
					this.setWeightedPositionsFromQPHashtables();
				return this.portfolioWeightedPositions;
			}
		}
		public DateTime LastOptimizationDate
		{
			get
			{
				return this.lastOptimizationDate;
			}
		}

		/// <summary>
		/// First generation of the genetic optimizer, when the best genome was created
		/// </summary>
		public int Generation
		{
			get { return this.generation; }
		}
		public WFLagChosenPositions( WFLagChosenTickers wFLagChosenTickers ,
			DateTime lastOptimizationDate )
		{
			//			this.drivingPositions =
			//				this.copy( wFLagChosenTickers.DrivingWeightedPositions );
			//			this.portfolioPositions =
			//				this.copy( wFLagChosenTickers.PortfolioWeightedPositions );
			this.drivingWeightedPositions = wFLagChosenTickers.DrivingWeightedPositions;
			this.portfolioWeightedPositions = wFLagChosenTickers.PortfolioWeightedPositions;
			this.lastOptimizationDate = lastOptimizationDate;
		}
		/// <summary>
		/// Data to be logged (out of sample) for each new optimization
		/// </summary>
		/// <param name="wFLagWeightedPositions">driving and portfolio
		/// positions chosen</param>
		/// <param name="generationWhenTheBestGenomeWasFound">generation when the
		/// genetic optimizer found the best genome. This parameter is meaningless
		/// if the optimizer does not use generations</param>
		/// <param name="lastOptimizationDate"></param>
		public WFLagChosenPositions( WFLagWeightedPositions wFLagWeightedPositions ,
			int generationWhenTheBestGenomeWasFound ,
			DateTime lastOptimizationDate )
		{
			//			this.drivingPositions =
			//				this.copy( wFLagChosenTickers.DrivingWeightedPositions );
			//			this.portfolioPositions =
			//				this.copy( wFLagChosenTickers.PortfolioWeightedPositions );
			this.initialize( wFLagWeightedPositions.DrivingWeightedPositions ,
				wFLagWeightedPositions.PortfolioWeightedPositions ,
				generationWhenTheBestGenomeWasFound , lastOptimizationDate );
		}

		public WFLagChosenPositions( WeightedPositions drivingWeightedPositions ,
			WeightedPositions portfolioWeightedPositions , DateTime lastOptimizationDate )
		{
			// -1 is used because the optimizer was not genetic, so there is
			// no generation number to log
			this.initialize( drivingWeightedPositions ,
				portfolioWeightedPositions ,
				-1 , lastOptimizationDate );
		}
		private void initialize( WeightedPositions drivingWeightedPositions ,
			WeightedPositions portfolioWeightedPositions ,
			int generationWhenTheBestGenomeWasFound ,
			DateTime lastOptimizationDate )
		{
			this.drivingWeightedPositions =
				drivingWeightedPositions;
			this.portfolioWeightedPositions =
				portfolioWeightedPositions;
			this.generation = generationWhenTheBestGenomeWasFound;
			this.lastOptimizationDate = lastOptimizationDate;
		}

		#region deserialization related constructor and methods
		protected WFLagChosenPositions( SerializationInfo info , StreamingContext context )
		{
			this.deserializeBaseClassMembers( info , context );
			this.deserializeThisClassMembers( info , context );
		}
		private void deserializeBaseClassMembers( SerializationInfo info , StreamingContext context )
		{
			// get the set of serializable members for this class and its base classes
			Type thisType = this.GetType();
			MemberInfo[] memberInfos = FormatterServices.GetSerializableMembers(
				thisType , context);

			// deserialize the fields from the info object, only if of the base clas
			for (Int32 i = 0 ; i < memberInfos.Length; i++) 
			{
				// Don't deserialize fields for this class
				if (memberInfos[i].DeclaringType != thisType)
				{
					FieldInfo fieldInfo = (FieldInfo) memberInfos[i];

					// set the field to the deserialized value
					fieldInfo.SetValue( this ,
						info.GetValue( fieldInfo.Name, fieldInfo.FieldType ) );
				}
			}
		}
		private void deserializeDrivingWeightedPositions(
			SerializationInfo info , StreamingContext context )
		{
			try
			{
//				this.drivingWeightedPositions = new WeightedPositions()
//				System.Type type = System.Type.GetType(
//					"QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WeightedPositions" );
				this.drivingWeightedPositions = (WeightedPositions)info.GetValue(
					"drivingWeightedPositions" , WeightedPositions.Type );
			}
			catch( Exception ex1 )
			{
				// the serialized WFLagChosenPositions is of old type
				// drivingPositions and portfolioPositions are QPHashtable
				try
				{
					string errorMessage1 = ex1.Message;
					this.drivingPositions = new QPHashtable();
					this.drivingPositions = (QPHashtable)info.GetValue(
						"drivingPositions" , this.drivingPositions.GetType() );
					//				drivingWeightedPositions = this.getWeightedPositions( drivingPositions );
				}
				catch( Exception ex2 )
				{
					string errorMessage = ex2.Message;
					errorMessage = errorMessage;
				}
			}
		}
		private void deserializePortfolioWeightedPositions(
			SerializationInfo info , StreamingContext context )
		{
			try
			{
				//				this.portfolioWeightedPositions = new WeightedPositions();
				//				System.Type type = System.Type.GetType(
				//					"QuantProject.Scripts.WalkForwardTesting.WalkForwardLag.WeightedPositions" );
				this.portfolioWeightedPositions = (WeightedPositions)info.GetValue(
					"portfolioWeightedPositions" , WeightedPositions.Type );
			}
			catch
			{
				// the serialized WFLagChosenPositions is of old type
				// drivingPositions and portfolioPositions are QPHashtable
				this.portfolioPositions = new QPHashtable();
				this.portfolioPositions = (QPHashtable)info.GetValue(
					"portfolioPositions" , this.portfolioPositions.GetType() );
				//				portfolioWeightedPositions = this.getWeightedPositions( portfolioPositions );
			}
		}
		private void deserializeGeneration(
			SerializationInfo info , StreamingContext context )
		{
			try
			{
				this.generation = (int)info.GetValue(
					"generation" , int.MaxValue.GetType() );
			}
			catch
			{
				this.generation = -9999;
			}
		}
		private void deserializeThisClassMembers( SerializationInfo info , StreamingContext context )
		{
			this.lastOptimizationDate = (DateTime)info.GetValue( "lastOptimizationDate" ,
				this.lastOptimizationDate.GetType() );
			this.deserializeDrivingWeightedPositions( info , context );
			this.deserializePortfolioWeightedPositions( info , context );
			this.deserializeGeneration( info , context );
		}
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
				string memberName = mi[i].Name;
				if ( ( memberName != "drivingPositions" ) && ( memberName != "portfolioPositions" ) )
					// current member is not used for old versions' deserialization, only
					info.AddValue(mi[i].Name, ((FieldInfo) mi[i]).GetValue(this));
			}
		}
//		private Hashtable copy( Hashtable hashTable )
//		{
//			Hashtable newCopy = new Hashtable();
//			foreach ( string key in hashTable.Keys )
//				newCopy.Add( key , null );
//			return newCopy;
//		}
		#region setWeightedPositionsFromQPHashtables
		private double getWeightedPositions_getWeight(
			double absoluteWeightForEachPosition , SignedTicker signedTicker )
		{
			double weight = absoluteWeightForEachPosition;
			if ( signedTicker.IsShort )
				weight = - absoluteWeightForEachPosition;
			return weight;
		}
		private WeightedPositions getWeightedPositions( QPHashtable signedTickers )
		{
			double absoluteWeightForEachPosition =
				1 / Convert.ToDouble( signedTickers.Count );
			double[] weights = new double[ signedTickers.Count ];
			string[] tickers = new string[ signedTickers.Count ];
			int i = 0;
			foreach ( string signedTicker in signedTickers.Keys )
			{
				weights[ i ] = this.getWeightedPositions_getWeight(
					absoluteWeightForEachPosition , new SignedTicker( signedTicker ) );
				tickers[ i ] = SignedTicker.GetTicker( signedTicker );
				i++;
			}
			WeightedPositions weightedPositions =
				new WeightedPositions( weights , tickers );
			return weightedPositions;
		}
		private void setWeightedPositionsFromQPHashtables()
		{
			this.drivingWeightedPositions = this.getWeightedPositions( this.drivingPositions );
			this.portfolioWeightedPositions = this.getWeightedPositions( this.portfolioPositions );
		}
		#endregion
		#endregion
	}
}
