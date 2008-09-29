/*
QuantProject - Quantitative Finance Library

MessageManager.cs
Copyright (C) 2007 
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
using System.IO;

using QuantProject.ADT.Messaging;

namespace QuantProject.Presentation
{
	/// <summary>
	/// Manages text messages
	/// </summary>
	[Serializable]
	public class MessageManager
	{
		private string textFileName;

		/// <summary>
		/// Manages text messages
		/// </summary>
		/// <param name="textFileName">messages will be written to this file
		/// also</param>
		public MessageManager( string textFileName )
		{
			this.textFileName = textFileName;
		}

		/// <summary>
		/// Displays a text message to the console and logs the message
		/// in the specified text file
		/// </summary>
		/// <param name="message"></param>
		/// <param name="textFileName"></param>
		public static void DisplayMessage( string message , string textFileName )
		{
			//			Console.WriteLine( message );
			//			Console.WriteLine( "" );
			System.Diagnostics.Debug.Listeners[0].WriteLine( message );

			//			FileStream fileStream = new FileStream( "WFLagLog.Txt" ,
			//				FileMode.OpenOrCreate );
			StreamWriter streamWriter = new StreamWriter( textFileName ,
				true );
			streamWriter.WriteLine( message );
			streamWriter.Close();
			//			fileStream.Close();
		}

		#region Monitor

		private void newMessageHandler(
			Object sender , NewMessageEventArgs eventArgs )
		{
			MessageManager.DisplayMessage(
				eventArgs.Message + "\n" , this.textFileName );
		}
		/// <summary>
		/// Adds the messageSender to the list of senders that this MessageManager
		/// is listening to. When the messageSender will rise a new message,
		/// this MessageManager will display it
		/// </summary>
		/// <param name="messageSender"></param>
		public void Monitor( IMessageSender messageSender )
		{
			messageSender.NewMessage +=
				new NewMessageEventHandler( this.newMessageHandler );
		}
		#endregion Monitor
	}
}
