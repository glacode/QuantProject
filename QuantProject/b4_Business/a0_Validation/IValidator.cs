using System;
using System.Data;

namespace QuantProject.Business.Validation
{
  public delegate void SuspiciousDataRowEventHandler(
  Object sender , SuspiciousDataRowEventArgs eventArgs );

	/// <summary>
	/// Interface to be implemented by quotes validators
	/// </summary>
	public interface IValidator
	{
    event SuspiciousDataRowEventHandler SuspiciousDataRow;
    void Validate( DataTable dataTable );
  }
}
