/*
QuantProject - Quantitative Finance Library

DownloaderConstants.cs
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

namespace QuantProject.Applications.Downloader.OpenTickDownloader
{
	/// <summary>
	/// Constants to be used by the download process
	/// </summary>
	public class DownloaderConstants
	{
		/// <summary>Number of bars that will be written by the database writer,
		/// with a single Sql command</summary>
		public const
			int MAX_NUMBER_OF_BARS_TO_BE_WRITTEN_WITH_A_SINGLE_SQL_COMMAND = 1;
		
		/// <summary>
		/// Max number of bars that a single OTManager object will have
		/// pending (if new requests arrive, they will be delayed until
		/// the previous ones have been satisfied)
		/// </summary>
		public const
			int MAX_NUMBER_OF_PENDING_REQUESTS_FOR_A_SINGLE_OTMANAGER = 100;

//		public DownloaderConstants()
//		{
//		}
	}
}
