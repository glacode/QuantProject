/*
QuantProject - Quantitative Finance Library

TestWindows.cs
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

namespace QuantProject.Business.Testing
{
	/// <summary>
	/// Summary description for TestWindows.
	/// </summary>
	public class TestWindows
	{
    private DateTime privateStartDateTime;
    private DateTime privateEndDateTime;
    private int privateInSampleWindowNumDays;
    private int privateOutOfSampleWindowNumDays;

    private TestWindow inSampleWindow;
    private TestWindow outOfSampleWindow;

    public TestWindow InSampleWindow
    {
      get { return inSampleWindow; }
    }

    public TestWindow OutOfSampleWindow
    {
      get { return outOfSampleWindow; }
    }

    public TestWindows(
      DateTime startDateTime ,
      DateTime endDateTime ,
      int inSampleWindowNumDays ,
      int outOfSampleWindowNumDays
      )
    {
      privateStartDateTime = startDateTime;
      privateEndDateTime = endDateTime;
      privateInSampleWindowNumDays = inSampleWindowNumDays;
      privateOutOfSampleWindowNumDays = outOfSampleWindowNumDays;
    }

    public bool IsComplete()
    {
      return ( ( inSampleWindow != null ) &&
        ( outOfSampleWindow.EndDateTime == this.privateEndDateTime ) );
    }

    public TestWindow GetFirstInSampleWindow()
    {
      // probably this method is not needed and can be deleted
      DateTime startDateTime = privateStartDateTime;
      DateTime endDateTime = privateStartDateTime.AddDays( this.privateInSampleWindowNumDays - 1 );
      TestWindow testWindow = new TestWindow( startDateTime , endDateTime );
      inSampleWindow = testWindow;
      return testWindow;
    }
  
    public TestWindow GetNextInSampleWindow()
    {
      if ( inSampleWindow == null )
      {
        // this is the first call to the TestWindows object, to return an in sample window
        inSampleWindow = this.GetFirstInSampleWindow();
        return inSampleWindow;
      }
      else
      {
        // the TestWindows object has already been asked for an in sample window
        DateTime startDateTime = inSampleWindow.StartDateTime.AddDays( 
          this.privateOutOfSampleWindowNumDays ); 
        DateTime endDateTime = startDateTime.AddDays( this.privateInSampleWindowNumDays - 1 );
        TestWindow testWindow = new TestWindow( startDateTime , endDateTime );
        inSampleWindow = testWindow;
        return testWindow;
      }
    }

    public TestWindow GetNextOutOfSampleWindow()
    {
      DateTime startDateTime = inSampleWindow.EndDateTime.AddDays( 1 );
      DateTime endDateTime;
      if ( this.privateEndDateTime < startDateTime.AddDays( this.privateOutOfSampleWindowNumDays - 1 ) )
        endDateTime = this.privateEndDateTime;
      else
        endDateTime = startDateTime.AddDays( this.privateOutOfSampleWindowNumDays - 1 );
      outOfSampleWindow = new TestWindow( startDateTime , endDateTime );
      return outOfSampleWindow;
    }
  }
}
