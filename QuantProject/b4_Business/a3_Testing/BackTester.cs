using System;
using QuantProject.ADT.Optimizing;
using QuantProject.ADT;
using QuantProject.Business.Financial.Accounting;
using QuantProject.Business.Strategies;

namespace QuantProject.Business.Testing
{
	/// <summary>
	/// Summary description for BackTester.
	/// </summary>
	public abstract class BackTester : Optimizable
	{
    private Account account = new Account();
    public string Name;

    public Account Account
    {
      get { return account; }
      set { account = value; }
    }
    public TradingSystems TradingSystems = new TradingSystems();

    public BackTester()
		{
			//
			// TODO: Add constructor logic here
			//
		}
    public override double Objective()
    {
      return 0;
    }
    public abstract void Test();
	}
}
