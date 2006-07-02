/*
QuantProject - Quantitative Finance Library

SelectorByMaxLinearIndipendence.cs
Copyright (C) 2003 
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
using System.Collections;
using System.Data;
using System.Windows.Forms;
using QuantProject.ADT.Optimizing.Genetic;
using QuantProject.DataAccess.Tables;
using QuantProject.Data.DataTables;

namespace QuantProject.Data.Selectors.ByLinearIndipendence
{
  /// <summary>
  /// Class for selection of a given number of tickers 
  /// that are mostly linear indipendent (two by two)
  /// </summary>
   public class SelectorByMaxLinearIndipendence : TickerSelector , ITickerSelector
  {
    private int numOfGenerationForGeneticOptimizer;
    private int populationSizeForGeneticOptimizer;
    private string marketIndex;
    
    public SelectorByMaxLinearIndipendence(DataTable setOfTickersToBeSelected, 
                               DateTime firstQuoteDate,
                               DateTime lastQuoteDate,
                               long indipendentTickersToBeReturned,
                               int numOfGenerationForGeneticOptimizer,
                               int populationSizeForGeneticOptimizer,
                               string marketIndex):
                                    base(setOfTickersToBeSelected, 
                                         false,
                                         firstQuoteDate,
                                         lastQuoteDate,
                                         indipendentTickersToBeReturned)
    {
      this.numOfGenerationForGeneticOptimizer = numOfGenerationForGeneticOptimizer;
      this.populationSizeForGeneticOptimizer = populationSizeForGeneticOptimizer;
      this.marketIndex = marketIndex;
    }
     public SelectorByMaxLinearIndipendence(string groupID, 
                                DateTime firstQuoteDate,
                                DateTime lastQuoteDate,
                                long indipendentTickersToBeReturned,
                                int numOfGenerationForGeneticOptimizer,
                                int populationSizeForGeneticOptimizer,
                                string marketIndex):
                                  base(QuantProject.DataAccess.Tables.Tickers_tickerGroups.GetTickers(groupID), 
                                  false,
                                  firstQuoteDate,
                                  lastQuoteDate,
                                  indipendentTickersToBeReturned)
     {
       this.numOfGenerationForGeneticOptimizer = numOfGenerationForGeneticOptimizer;
       this.populationSizeForGeneticOptimizer = populationSizeForGeneticOptimizer;
       this.marketIndex = marketIndex;
     }

    public DataTable GetTableOfSelectedTickers()
    {
      DataTable returnValue = new DataTable();
      returnValue.Columns.Add("ticker", Type.GetType("System.String"));
      SelectorByQuotationAtEachMarketDay selectorByQuotation = 
      	new SelectorByQuotationAtEachMarketDay(this.setOfTickersToBeSelected,false,this.firstQuoteDate,
      	                                       this.lastQuoteDate,this.setOfTickersToBeSelected.Rows.Count,
      	                                       this.marketIndex);
      	                                       
      GeneticOptimizer GO =
        new GeneticOptimizer(
            new GenomeManagerForMaxLinearIndipendenceSelector(
      	            selectorByQuotation.GetTableOfSelectedTickers(),this.firstQuoteDate,
                    this.lastQuoteDate,(int)this.MaxNumOfReturnedTickers),
        						this.populationSizeForGeneticOptimizer,this.numOfGenerationForGeneticOptimizer);
      GO.Run(false);
      string[] bestTickers = (string[])GO.BestGenome.Meaning;
      for(int i = 0; i< (int)this.maxNumOfReturnedTickers; i++)
      {
      	DataRow newRow = returnValue.NewRow();
      	newRow[0] = bestTickers[i];
      	returnValue.Rows.Add(newRow);
      }
      return returnValue;
    }
    
    public void SelectAllTickers()
    {
      ;
    }	
   }
}
