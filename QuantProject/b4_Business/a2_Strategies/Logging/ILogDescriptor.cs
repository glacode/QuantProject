using System;

namespace QuantProject.Business.Strategies.Logging
{
	/// <summary>
	/// Summary description for ILogDescriptor.
	/// </summary>
	public interface ILogDescriptor
	{
		/// <summary>
		/// Short description to be added to some log (it might be used in file names, too)
		/// </summary>
		string Description{ get; }
	}
}
