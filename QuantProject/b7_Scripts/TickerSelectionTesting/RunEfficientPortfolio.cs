/*
QuantProject - Quantitative Finance Library

RunEfficientPorfolio.cs
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
using System.Data;
using QuantProject.ADT.Optimizing.Genetic;

/*
using System.Collections;
using QuantProject.ADT;
using QuantProject.Business.Financial.Accounting.Reporting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Strategies;
using QuantProject.Business.Testing;
using QuantProject.Data.DataTables;
using QuantProject.Data.DataProviders;
using QuantProject.Presentation.Reporting.WindowsForm;
*/
using QuantProject.Business.Scripting;
using QuantProject.Data.Selectors; 
using QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios;

namespace QuantProject.Scripts.TickerSelectionTesting.EfficientPortfolios
{
	/// <summary>
	/// Script to find the efficient portfolio for the given interval of days
	/// (the value of portfolio is computed through adjusted close values)
	/// </summary>
	public class RunEfficientPorfolio : Script
	{
		DateTime firstDate = new DateTime(2001,1,1);
    DateTime lastDate = new DateTime(2001,3,1);
		
    public RunEfficientPorfolio()
		{
			
		}
    #region Run
    
      
    public override void Run()
    {
      //"STOCKMI"  "MIB_F"

      TickerSelector mostLiquid = new TickerSelector(SelectionType.Liquidity,
                                                    false, "STOCKMI", firstDate, lastDate, 60);
      DataTable tickers = mostLiquid.GetTableOfSelectedTickers();
 	    IGenomeManager genManEfficientPortfolio = 
                   new GenomeManagerForEfficientPortfolio(4, tickers,firstDate,
                                                          lastDate, 5, 0.01, 0.1);
      GeneticOptimizer GO = new GeneticOptimizer(genManEfficientPortfolio);
      //GO.KeepOnRunningUntilConvergenceIsReached = true;
      GO.GenerationNumber = 4;
      GO.MutationRate = 0.05;
      GO.Run(true);
      System.Console.WriteLine("\n\nThe best solution found is: " + (string)GO.BestGenome.Meaning +
        " with {0} generations", GO.GenerationCounter);
		}
    #endregion 
	}
}
