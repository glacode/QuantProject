using System;
using QuantProject.ADT.Optimizing;
using QuantProject.ADT;
using QuantProject.Business.Financial.Accounting;

namespace QuantProject.Business.Testing
{
	/// <summary>
	/// Summary description for BackTester.
	/// </summary>
	public class BackTester : Optimizable
	{
    private Account account = new Account();

    public Account Account
    {
      get { return account; }
      set { account = value; }
    }
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
	}
}
