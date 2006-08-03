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

using QuantProject.ADT;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Provides the genome relevant informations
	/// </summary>
	[Serializable]
	public class GenomeRepresentation : ISerializable
	{
		private double fitness;
		private string signedTickers;
    private string weights;
		private int generationCounter;
		private DateTime firstOptimizationDate;
		private DateTime lastOptimizationDate;
		private int eligibleTickers = -1;
		
		public string SignedTickers
		{
			get
			{
				return this.signedTickers;
			}
		}
    
		public int EligibleTickers
		{
			get
			{
				return this.eligibleTickers;
			}
		}
		
    public string WeightsForSignedTickers
    {
      get
      {
        return this.weights;
      }
    }
		public double Fitness
		{
			get { return this.fitness; }
		}
		public DateTime FirstOptimizationDate
		{
			get { return this.firstOptimizationDate; }
		}
		public DateTime LastOptimizationDate
		{
			get { return this.lastOptimizationDate; }
		}
		/// <summary>
		/// Number of the first generation containing the genome
		/// </summary>
		public int GenerationCounter
		{
			get { return this.generationCounter; }
		}

//		public static string[] GetSignedTickers( string signedTickers )
//		{
//			string[] returnValue = signedTickers.Split( ";".ToCharArray());
//			return returnValue;
//		}
		
    	public static double[] GetWeightsArray( string weightsWithSeparator )
    	{
    		double[] returnValue;
        string[] returnValueString = weightsWithSeparator.Split( ";".ToCharArray());
    		returnValue = new double[returnValueString.Length];
        for(int i = 0; i<returnValue.Length; i++)
        {
          returnValue[i] = Convert.ToDouble(returnValueString[i]);
        }
        return returnValue;
    	}

		public static string[] GetSignedTickers( string signedTickersWithWeights )
		{
			string[] returnValue = 
        signedTickersWithWeights.Split( ConstantsProvider.SeparatorForTickers.ToCharArray());
			if( signedTickersWithWeights.Split( ConstantsProvider.SeparatorForWeights.ToCharArray() ).Length > 1 )
			//the separator char for tickers is contained in signedTickersWithWeights: 
			//so weights have been saved within tickers, separated by this special char
			{
				for(int i = 0; i<returnValue.Length; i++)
				{
					returnValue[i] = 
						 returnValue[i].Split( ConstantsProvider.SeparatorForWeights.ToCharArray() )[0];
				}
			}	
			return returnValue;
		}
	
		public static double[] GetWeightsForSignedTickers( string signedTickersWithWeights )
		{
			string[] signedTickersWithWeightsArray = 
								signedTickersWithWeights.Split( ConstantsProvider.SeparatorForTickers.ToCharArray() );
			double[] returnValue =
				new double[signedTickersWithWeightsArray.Length];
			for(int i = 0; i<returnValue.Length; i++)
				returnValue[i] = 1.0/returnValue.Length;
			
			if((signedTickersWithWeights.Split(ConstantsProvider.SeparatorForWeights.ToCharArray())).Length > 1)
			//the separator for weights is contained in signedTickersWithWeights: 
			//so weights have been saved within tickers, separated by this char
			{
				for(int i = 0; i<returnValue.Length; i++)
				{
					returnValue[i] = 
						Convert.ToDouble( signedTickersWithWeightsArray[i].Split( ConstantsProvider.SeparatorForWeights.ToCharArray() )[1] );
				}
			}	
			return returnValue;
		}
		
		public static OrderType GetOrderType( string signedTicker )
		{
			OrderType returnValue;
			if ( signedTicker.IndexOf( "-" ) == 0 )
				returnValue = OrderType.MarketSellShort;
			else
				returnValue = OrderType.MarketBuy;
			return returnValue;
		}
		public static string GetTicker( string signedTicker )
		{
			string returnValue;
			if ( signedTicker.IndexOf( "-" ) == 0 )
				returnValue = signedTicker.Substring( 1 , signedTicker.Length - 1 );
			else
				returnValue = signedTicker;
			return returnValue;
		}

		private string getSignedTickers( Genome genome )
		{
			string signedTickers = "";
			foreach ( string geneValue in ((GenomeMeaning)genome.Meaning).Tickers )
				signedTickers += geneValue + ";";
			signedTickers = signedTickers.Substring( 0 ,
				signedTickers.Length - 1 );
			return signedTickers;
		}
    
    private string getWeights( Genome genome )
    {
      string weights = "";
      foreach ( double weight in ((GenomeMeaning)genome.Meaning).TickersPortfolioWeights )
        weights += weight.ToString() + ";";
      weights = weights.Substring( 0, weights.Length - 1 );
      return weights;
    }

//		private string getSignedTickersWithWeights( Genome genome )
//		{
//			string signedTickersWithWeights = "";
//			for ( int i = 0; i<((GenomeMeaning)genome.Meaning).Tickers.Length; i++ )
//			{
//				signedTickersWithWeights +=
//					((GenomeMeaning)genome.Meaning).Tickers[i] + 
//					ConstantsProvider.SeparatorForWeights +
//					((GenomeMeaning)genome.Meaning).TickersPortfolioWeights[i] +
//					ConstantsProvider.SeparatorForTickers;
//			}
//			signedTickersWithWeights = signedTickersWithWeights.Substring( 0 ,
//					signedTickersWithWeights.Length - 1 );
//			
//			return signedTickersWithWeights;
//		}
		
    private void genomeRepresentation_synchronizeOldWithNew()
    {
      if(this.weights == null)
      //for old genomes saved to disk not having "weights" field
      {
        foreach(double weight in GenomeRepresentation.GetWeightsForSignedTickers(this.signedTickers))
          this.weights += weight.ToString() + ";";
        this.weights = this.weights.Substring( 0, this.weights.Length - 1 );
      //
        string newRepresentationForSignedTickers = null;
        foreach(string ticker in GenomeRepresentation.GetSignedTickers(this.signedTickers))
          newRepresentationForSignedTickers += ticker + ";";
        this.signedTickers = newRepresentationForSignedTickers.Substring(0, newRepresentationForSignedTickers.Length -1);
      }

    }

		/// <summary>
		/// This constructor allows custom deserialization (see the ISerializable
		/// interface documentation)
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected GenomeRepresentation( SerializationInfo info , StreamingContext context )
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
				{ex = ex;}
			}
      
      this.genomeRepresentation_synchronizeOldWithNew();

		}
		
		private void genomeRepresentation( Genome genome ,
			DateTime firstOptimizationDate , DateTime lastOptimizationDate ,
			int generationCounter, int eligibleTickers )
		{
			this.fitness = genome.Fitness;
			//this.signedTickers = this.getSignedTickersWithWeights( genome );
			this.signedTickers = this.getSignedTickers( genome );
			this.weights = this.getWeights( genome );
      this.firstOptimizationDate = firstOptimizationDate;
			this.lastOptimizationDate = lastOptimizationDate;
			this.generationCounter = generationCounter;
			this.eligibleTickers = eligibleTickers;
		}
		public GenomeRepresentation( Genome genome ,
			DateTime firstOptimizationDate , DateTime lastOptimizationDate )
		{
			this.genomeRepresentation( genome ,
				firstOptimizationDate , lastOptimizationDate , -1, -1 );
		}
		public GenomeRepresentation( Genome genome ,
			DateTime firstOptimizationDate , DateTime lastOptimizationDate ,
			int generationCounter )
		{
			this.genomeRepresentation( genome , firstOptimizationDate ,
				lastOptimizationDate , generationCounter, -1 );
		}
		
		public GenomeRepresentation( Genome genome ,
			DateTime firstOptimizationDate , DateTime lastOptimizationDate ,
			int generationCounter, int eligibleTickers )
		{
			this.genomeRepresentation( genome , firstOptimizationDate ,
				lastOptimizationDate , generationCounter, eligibleTickers );
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
