/*
 * Created by SharpDevelop.
 * User: Glauco
 * Date: 22/03/2010
 * Time: 16.13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace QuantProject.ADT.Econometrics
{
	/// <summary>
	/// Interface to be implemented by those classes that run a
	/// linear regression
	/// </summary>
	public interface ILinearRegression
	{
		double[] EstimatedCoefficients{ get; }
		double CenteredRSquare{ get; }
		void RunRegression( double[] regressand , double[,] regressors );
	}
}
