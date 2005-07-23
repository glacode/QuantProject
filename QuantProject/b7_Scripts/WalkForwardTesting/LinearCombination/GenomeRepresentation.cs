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

using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.Business.Financial.Ordering;

namespace QuantProject.Scripts.WalkForwardTesting.LinearCombination
{
	/// <summary>
	/// Provides the genome relevant informations
	/// </summary>
	[Serializable]
	public class GenomeRepresentation
	{
		private Genome genome;

		public string SignedTickers
		{
			get
			{
				string signedTickers = "";
				foreach ( string geneValue in (string[])genome.Meaning )
					signedTickers += geneValue + ";";
				signedTickers = signedTickers.Substring( 0 ,
					signedTickers.Length - 1 );
				return signedTickers;
			}
		}
		public double Fitness
		{
			get { return this.genome.Fitness; }
		}

		public static string[] GetSignedTickers( string signedTickers )
		{
			string[] returnValue = signedTickers.Split( ";".ToCharArray() );
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
		public GenomeRepresentation( Genome genome )
		{
			this.genome = genome;
		}
	}
}
