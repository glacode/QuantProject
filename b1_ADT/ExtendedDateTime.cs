using System;

namespace QuantProject.ADT
{
	/// <summary>
	/// Summary description for DateTime.
	/// </summary>
  public class ExtendedDateTime
  {
    private DateTime dateTime;
    private BarComponent barComponent;
    private bool isSimpleDateTime;

    public DateTime DateTime
    {
      get { return dateTime; }
      set { dateTime = value; }
    }

    public BarComponent BarComponent
    {
      get { return barComponent; }
      set { barComponent = value; }
    }

    public bool IsSimpleDateTime
    {
      get { return isSimpleDateTime; }
    }

    public ExtendedDateTime( DateTime dateTime )
    {
      this.dateTime = dateTime;
      this.isSimpleDateTime = true;
    }

    public ExtendedDateTime( DateTime dateTime , BarComponent barComponent )
    {
      this.dateTime = dateTime;
      this.barComponent = barComponent;
      this.isSimpleDateTime = false;
    }

    public int CompareTo( Object barDateTimeToCast )
    {
      ExtendedDateTime extendedDateTime = (ExtendedDateTime) barDateTimeToCast;
      int compareTo = 0;
      if (  ( this.DateTime < extendedDateTime.DateTime ) ||
        ( ( this.DateTime == extendedDateTime.DateTime ) &&
        ( this.barComponent == BarComponent.Open ) &&
        ( extendedDateTime.barComponent == BarComponent.Close ) ) )
        compareTo = -1;
      else
      {
        if ( ( this.DateTime == extendedDateTime.DateTime ) &&
          ( this.barComponent == extendedDateTime.barComponent ) )
          compareTo = 0;
        else
          compareTo = 1;
      }
      return compareTo;
    }

    public override string ToString()
    {
      return this.DateTime.ToString() + " - " + this.BarComponent.ToString();
    }
	}
}
