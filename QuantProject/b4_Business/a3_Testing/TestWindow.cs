/*
QuantProject - Quantitative Finance Library

TestWindow.cs
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
  /// Summary description for TestWindow.
  /// </summary>
  public class TestWindow
  {
    private DateTime startDateTime;
    private DateTime endDateTime;
    
    public DateTime StartDateTime
    {
      get
      {
        return startDateTime;
      }
      set
      {
        startDateTime = value;
      }
    }

    public DateTime EndDateTime
    {
      get
      {
        return endDateTime;
      }
      set
      {
        endDateTime = value;
      }
    }

    public TestWindow( DateTime startDateTime , DateTime endDateTime )
    {
      StartDateTime = startDateTime;
      EndDateTime = endDateTime;
    }
  }
}
