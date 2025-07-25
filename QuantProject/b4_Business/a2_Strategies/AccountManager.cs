/*
QuantProject - Quantitative Finance Library

AccountManager.cs
Copyright (C) 2007 
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
using System.Data;
using System.Collections;

using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Financial.Ordering;
using QuantProject.Business.Timing;
using QuantProject.Business.Strategies;
using QuantProject.Data.DataProviders;
using QuantProject.Data.Selectors;
using QuantProject.Data.DataTables;


namespace QuantProject.Business.Strategies
{
	
  /// <summary>
  /// Class providing static methods for common operations on
  /// a given account
  /// </summary>
  [Serializable]
  public class AccountManager
  {
    private static ArrayList orders;
        
    static AccountManager()
    {
    	orders = new ArrayList();
    }
    
		static public string[] GetTickersInOpenedPositions(Account account)
		{
			Position[] positions = new Position[account.Portfolio.Count];
			account.Portfolio.Positions.CopyTo( positions, 0);
			string[] returnValue = new string[positions.Length];
			for( int i = 0; i < positions.Length; i++ )
				returnValue[i] = positions[i].Instrument.Key;
			return returnValue;
		}

    static public void ClosePositions(Account account)
    {
      string[] tickers = GetTickersInOpenedPositions( account );
			foreach( string ticker in tickers)
				account.ClosePosition( ticker );
    }

		#region OpenPositions
    static private  void addWeightedPositionToOrderList(WeightedPosition weightedPosition, 
		                                                    Account account, double valueToInvestInPositions)
    {
    	string ticker = weightedPosition.Ticker;
      double cashForSinglePosition = 
      	valueToInvestInPositions * Math.Abs( weightedPosition.Weight );
      long quantity =
        Convert.ToInt64( Math.Floor( cashForSinglePosition / account.DataStreamer.GetCurrentBid( ticker ) ) );
      Order order = 
      	new Order( weightedPosition.GetOrderType(),
      	          new Instrument( ticker ) , quantity );
      orders.Add(order);
    }
    /// <summary>
		/// Modifies the state for the given account,
		/// opening positions provided by the given weightedPositions
		/// </summary>
		/// <param name="weightedPositions"></param>
		/// <param name="account"></param>
		/// <param name="initialCashForTheAccount">The initial cash for the account
		/// with no transactions at all (totally empty account)</param>
		static public void OpenPositions(WeightedPositions weightedPositions,
																		 Account account, double initialCashIfTheAccountIsEmpty)
		{
			OpenPositions(weightedPositions, account, initialCashIfTheAccountIsEmpty, 1);
		}
		
		/// <summary>
		/// Modifies the state for the given account,
		/// opening positions provided by the given weightedPositions
		/// </summary>
		/// <param name="weightedPositions"></param>
		/// <param name="account"></param>
		/// <param name="initialCashForTheAccount">The initial cash for the account
		/// <param name="leverage">It multiplies initial cash in order to compute
		/// 											 value to invest in the given weighted positions</param>
		static public void OpenPositions(WeightedPositions weightedPositions,
																		 Account account, double initialCashIfTheAccountIsEmpty,
																		 double leverage)
		{
			if(account.Transactions.Count == 0)
				account.AddCash(initialCashIfTheAccountIsEmpty);
			if(weightedPositions == null || account == null)
				throw new Exception("Both parameters have to be set to valid objects!");
			orders.Clear();
			double valueToInvestInPositions = account.CashAmount * leverage;
			foreach(WeightedPosition weightedPosition in weightedPositions)
				addWeightedPositionToOrderList( weightedPosition, account, valueToInvestInPositions );
			foreach(object item in orders)
				account.AddOrder( (Order)item );
		}

		/// <summary>
		/// Modifies the state for the given account,
		/// opening positions provided by the given weightedPositions
		/// </summary>
		static public void OpenPositions(WeightedPositions weightedPositions,
																		 Account account)
		{
			OpenPositions(weightedPositions, account, 15000);
		}
    #endregion

		static private double getWeightedPositions_getPositionsAbsoluteValue(Account account)
		{
			double totalValue = 0;
			foreach (Position position in account.Portfolio.Values)
				totalValue += Math.Abs( account.DataStreamer.GetCurrentBid(
					position.Instrument.Key ) * position.Quantity );
			return totalValue;
		} 

		/// <summary>
		/// Gets the current open WeightedPositions for the given account
		/// </summary>
		static public WeightedPositions GetWeightedPositions(Account account)
		{
			double positionsAbsoluteValue = getWeightedPositions_getPositionsAbsoluteValue(account); 
			string[] tickers = GetTickersInOpenedPositions( account );
			double[] weights = new double[tickers.Length];
			for(int i = 0; i < tickers.Length; i++)
				weights[i] = 
					( account.GetMarketValue( tickers[i] )*
					account.Portfolio.GetPosition( tickers[i] ).Quantity ) /
					positionsAbsoluteValue;
			
			return new WeightedPositions( weights, tickers );
		}

  	static public void ReversePositions(Account account)
    {
      orders.Clear();
			WeightedPositions currentWeightedPositions = GetWeightedPositions( account );
			currentWeightedPositions.ReverseSigns();
			ClosePositions(account);
      OpenPositions( currentWeightedPositions, account );
    }   
  } // end of class
}
