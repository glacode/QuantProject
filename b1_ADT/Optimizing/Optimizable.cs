using System;

namespace QuantProject.ADT.Optimizing
{
	/// <summary>
	/// Summary description for Optimizable.
	/// </summary>
	public abstract class Optimizable
	{
    public Parameters Parameters = new Parameters();
    public Parameters OptimalParameters;

		public Optimizable()
		{
			//
			// TODO: Add constructor logic here
			//
		}

    public abstract double Objective();

    private void updateOptimalParameters()
    {
      //OptimalParameters.Clear();
      //foreach (Parameter parameter in this.Parameters )
        //OptimalParameters.Add( parameter );
      OptimalParameters = (Parameters) this.Parameters.Copy();
    }

    public void Optimize()
    {
      double minUntilNow = double.MaxValue;
      //bool doLoop = Parameters.SetNextValues();
      while ( Parameters.SetNextValues() )
      {
        double objectiveValue = this.Objective();
        if (objectiveValue<minUntilNow)
        {
          updateOptimalParameters();
          minUntilNow = objectiveValue;
        }
      }
    }
	}
}
