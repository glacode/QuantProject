/*
QuantProject - Quantitative Finance Library

LogArchiver.cs
Copyright (C) 2008
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

using QuantProject.Business.Strategies.Logging;
using QuantProject.Presentation;

namespace QuantProject.Scripts.General.Logging
{
	/// <summary>
	/// Saves/Loads a backtest log to/from disk invoking a dialog form to ask for
	/// the full file path to write/read to/from.
	/// </summary>
	public class LogArchiver
	{
		public LogArchiver()
		{
		}
		public static void Save(
			BackTestLog backTestLog ,
			string suggestedLogFileName ,
			string initialFolderPath )
		{
			VisualObjectArchiver visualObjectArchiver =
				new VisualObjectArchiver();
			visualObjectArchiver.Save(
				backTestLog , "qPl" , "Save backtest Log" ,
				suggestedLogFileName , initialFolderPath );
		}
		public static BackTestLog Load( string initialFolderPath )
		{
			VisualObjectArchiver visualObjectArchiver =
				new VisualObjectArchiver();
			object backTestLog =
				visualObjectArchiver.Load(
				"qPl" , "Load backtest Log" , initialFolderPath );
			if ( !( backTestLog is BackTestLog ) )
				throw new Exception( "The loaded file is not a valid serialization for " +
					"a BackTestLog Object!" );
			return (BackTestLog)backTestLog;
		}
	}
}
