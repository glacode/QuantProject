/*
QuantProject - Quantitative Finance Library

VisualObjectArchiver.cs
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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using QuantProject.ADT.FileManaging;

namespace QuantProject.Presentation
{
	/// <summary>
	/// Saves/Loads objects to/form disk, invoking a dialog form to ask for
	/// the full file path to write/read to/from.
	/// </summary>
	public class VisualObjectArchiver
	{
		private SaveFileDialog saveFileDialog;
		/// <summary>
		/// object to be saved
		/// </summary>
		private Object obj;

		public VisualObjectArchiver()
		{
		}
//		public VisualObjectArchiver( ArrayList bestGenomes , string suffix ,
//			string initialDirectory , string title )
//		{
//			this.visualObjectArchiver( bestGenomes , suffix , initialDirectory ,
//				title );
//		}
		private void fileOkEventHandler( object sender , CancelEventArgs e )
		{
			QuantProject.ADT.FileManaging.ObjectArchiver.Archive(
				this.obj , ((SaveFileDialog)this.saveFileDialog).FileName );
		}
		private void save( object obj , string suffix ,
			 string title , string initialDirectory )
		{
			this.obj = obj;
			this.saveFileDialog = new SaveFileDialog();
			this.saveFileDialog.DefaultExt = suffix;
			this.saveFileDialog.Filter = suffix + " files (*."+
				suffix + ")|*." + suffix;
			this.saveFileDialog.Title = title;
			this.saveFileDialog.FileName = title;
			this.saveFileDialog.InitialDirectory = initialDirectory;
			this.saveFileDialog.AddExtension = true;
			this.saveFileDialog.CreatePrompt = true;
			this.saveFileDialog.OverwritePrompt = true;
			//the saveFileDialog title is the same as the
			//menu item clicked by the user
			this.saveFileDialog.CheckPathExists = true;
			this.saveFileDialog.FileOk += new CancelEventHandler(
				this.fileOkEventHandler );
			this.saveFileDialog.ShowDialog();
		}
		public void Save( object obj , string suffix ,
			string title , string initialDirectory )
		{
			this.save( obj , suffix , title , initialDirectory );
		}
		public void Save( object obj , string suffix ,
			string title )
		{
			this.Save( obj , suffix , title ,
				System.Configuration.ConfigurationSettings.AppSettings["ReportsArchive"] );
		}
		private string getPath( string title , string suffix )
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = title;
			openFileDialog.Filter = suffix + " files (*."+
				suffix + ")|*." + suffix;
			openFileDialog.Multiselect = false;
			openFileDialog.CheckFileExists = true;
			openFileDialog.ShowDialog();
			return openFileDialog.FileName;
		}
		private object load( object obj , string suffix ,
			string title , string initialDirectory )
		{
			string chosenPath = this.getPath( title , suffix );
			return ObjectArchiver.Extract( chosenPath );
		}
		public object Load( object obj , string suffix , string title )
		{
			return this.load( obj , suffix , title ,
				System.Configuration.ConfigurationSettings.AppSettings["ReportsArchive"] );
		}
	}
}
