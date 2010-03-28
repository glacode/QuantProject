/*
QuantProject - Quantitative Finance Library

LinearRegressionTestingPositions.cs
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
using System.Collections.Generic;

using QuantProject.ADT.Econometrics;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.OutOfSample;

namespace QuantProject.Scripts.WalkForwardTesting.LinearRegression
{
	/// <summary>
	/// TestingPositions for the linear regressin strategy
	/// </summary>
	[Serializable]
	public class LinearRegressionTestingPositions : TestingPositions , IGeneticallyOptimizable
	{
		private ILinearRegression linearRegression;
		private WeightedPositions[] signalingPortfolios;

		
		public virtual ILinearRegression LinearRegression {
			get { return this.linearRegression; }
			set { this.linearRegression = value; }
		}
		
		public WeightedPositions[] SignalingPortfolios {
			get { return this.signalingPortfolios; }
			set { signalingPortfolios = value; }
		}
		
		public WeightedPositions TradingPortfolio
		{
			get { return this.WeightedPositions; }
		}
		
		private int generation;

		public int Generation
		{
			get { return this.generation; }
			set { this.generation = value; }
		}
		
		public List<string> SignalingTickers
		{
			get
			{
				List<string> signalingTickers = new List<string>();
				foreach( WeightedPositions signalingPortfolio in this.SignalingPortfolios )
					foreach( string ticker in signalingPortfolio.SignedTickers.Tickers )
						if ( !signalingTickers.Contains( ticker ) )
							signalingTickers.Add( ticker );
				return signalingTickers;
			}
		}
		
		/// <summary>
		/// testing positions to be used by the linear regression strategy
		/// </summary>
		/// <param name="signalingPortfolios">the return
		/// of each portfolio is a regressor</param>
		/// <param name="tradingPortfolio">the regressand is the return of this
		/// portfolio</param>
		public LinearRegressionTestingPositions(
			WeightedPositions[] signalingPortfolios ,
			WeightedPositions tradingPortfolio ) : base( tradingPortfolio )
		{
			this.signalingPortfolios = signalingPortfolios;
		}
		
	}
}
