/*
QuantProject - Quantitative Finance Library

TradingSystem.cs
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
using QuantProject.ADT;
using QuantProject.ADT.Optimizing;

namespace QuantProject.Business.Strategies
{
	/// <summary>
	/// Summary description for TradingSystem.
	/// </summary>
	/// 


	public abstract class TradingSystem
  {
    private Parameters parameters = new Parameters();
    private DateTime testStartDateTime;
    private DateTime testEndDateTime;

    public Parameters Parameters
    {
      get { return parameters; }
      set { parameters = value; }
    }

    public DateTime TestStartDateTime
    {
      get { return testStartDateTime; }
      set { testStartDateTime = value; }
    }

    public DateTime TestEndDateTime
    {
      get { return testEndDateTime; }
      set { testEndDateTime = value; }
    }

    public TradingSystem()
    {
      //
      // TODO: Add constructor logic here
      //
    }

    public abstract Signals GetSignals( ExtendedDateTime extendedDateTime );

    public virtual void InitializeData()
    {
    }
  }
}
