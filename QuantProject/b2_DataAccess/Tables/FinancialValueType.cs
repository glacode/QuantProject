/*
QuantProject - Quantitative Finance Library

FinancialValueType.cs
Copyright (C) 2011 
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

namespace QuantProject.DataAccess.Tables
{
  /// <summary>
  /// Enum providing the codes used in the DB
  /// as ID for financial values
  /// </summary>
  public enum FinancialValueType
  {
    AveragePriceEarnings = 59 ,
    BookValuePerShare = 63 , 
    DebtEquityRatio = 64 ,
    Depreciation = 54 , 
    EBIT = 55 , 
    EarningsPerShare = 56 , 
    InterestCoverage = 67 , 
    LongTermDebt = 31 ,
    NetIncome = 40 , 
    NetProfitMargin = 62 ,
    PriceToBook = 61 , 
    PriceToSales = 60 ,
    Revenues = 1 , 
    ROA = 66 , 
    ROE = 65 , 
    SharesOutstanding = 58 ,
    TaxRateInPercentage = 57 ,
    TotalCurrentAssets = 42 , 
    TotalCurrentLiabilities = 44
  }
}
