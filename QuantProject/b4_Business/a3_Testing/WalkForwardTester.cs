/*
QuantProject - Quantitative Finance Library

WalkForwardTester.cs
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
using System.Diagnostics;
using QuantProject.ADT;
using QuantProject.ADT.Optimizing;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Financial.Instruments;
using QuantProject.Business.Strategies;
using QuantProject.Data.DataProviders;

namespace QuantProject.Business.Testing
{
  /// <summary>
  /// Summary description for WalkForwardTester.
  /// </summary>
  public class WalkForwardTester : BackTester
  {
    private DateTime startDateTime;
    private DateTime endDateTime;
    private int inSampleWindowNumDays;
    private int outOfSampleWindowNumDays;
    private TestWindows testWindows;
		private IDataStreamer dataStreamer;

    public DateTime StartDateTime
    {
      get
      {
        return startDateTime;
      }
      set
      {
        startDateTime = value;
      }
    }
    public DateTime EndDateTime
    {
      get
      {
        return endDateTime;
      }
      set
      {
        endDateTime = value;
      }
    }
    public int InSampleWindowNumDays
    {
      get
      {
        return inSampleWindowNumDays;
      }
      set
      {
        inSampleWindowNumDays = value;
      }
    }
    public int OutOfSampleWindowNumDays
    {
      get
      {
        return outOfSampleWindowNumDays;
      }
      set
      {
        outOfSampleWindowNumDays = value;
      }
    }

    public WalkForwardTester( IDataStreamer dataStreamer )
    {
			this.dataStreamer = dataStreamer;
    }

    public void Add( TradingSystem tradingSystem )
    {
      TradingSystems.Add( tradingSystem );
    }

    #region "Test"

    private Parameters getOptimizedParameters( TestWindow testWindow )
    {
      Tester tester = new Tester( testWindow , this.TradingSystems ,
        this.Account.CashAmount , this.dataStreamer );
      tester.Parameters = this.Parameters.Copy();
      tester.Optimize();
      return tester.OptimalParameters;
    }

    private void testNextStepOutOfSample( Parameters parameters , TestWindow testWindow )
    {
      Tester tester = new Tester( testWindow , this.TradingSystems , this.Account.CashAmount ,
				this.dataStreamer );
      tester.Account = this.Account;
      tester.Parameters = parameters;
      tester.Test();
    }

    private void testNextStep()
    {
      TestWindow inSampleTestWindow = testWindows.GetNextInSampleWindow();
      TestWindow outSampleTestWindow = testWindows.GetNextOutOfSampleWindow();
      Parameters optimizedParameters = getOptimizedParameters( inSampleTestWindow );
      Debug.WriteLine( optimizedParameters.ToString() );
      testNextStepOutOfSample( optimizedParameters , outSampleTestWindow );
    }

    public override void Test()
    {
//      HistoricalDataProvider.SetCachedHistories( startDateTime , endDateTime );
      testWindows = new TestWindows( startDateTime , endDateTime , inSampleWindowNumDays , outOfSampleWindowNumDays );
      DateTime lastDateTime = new DateTime();
      while ( ! testWindows.IsComplete() )
      {
        testNextStep();
        Console.WriteLine( lastDateTime.ToString() );
        lastDateTime = testWindows.OutOfSampleWindow.EndDateTime;
      }
//      this.accounts.ReportToConsole( lastDateTime );
      //this.accounts.Serialize( "c:\\quantProject.xml" );
    }

    #endregion
  }
}
