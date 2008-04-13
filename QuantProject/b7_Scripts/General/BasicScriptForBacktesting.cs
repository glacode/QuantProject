/*
QuantProject - Quantitative Finance Library

BasicScript.cs
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
using System.IO;

using QuantProject.ADT;
using QuantProject.ADT.FileManaging;
using QuantProject.Business.Strategies;
using QuantProject.Business.Strategies.Eligibles;
using QuantProject.Business.Strategies.InSample;
using QuantProject.Presentation;

namespace QuantProject.Scripts.General
{
	/// <summary>
	/// Provides methods to save script's results: to be inherited by all
	/// those scripts that want their results to be permanently saved
	/// </summary>
	public abstract class BasicScriptForBacktesting
	{
		private string pathForTheMainFolderWhereScriptsResultsAreToBeSaved;
		private string customSmallTextForFolderName;
		private string fullPathFileNameForMain;

		private string nameForTheSubfolderForScriptResults;
		private MessageManager messageManager;

		protected IEligiblesSelector eligiblesSelector;
		protected IInSampleChooser inSampleChooser;
		protected IEndOfDayStrategyForBacktester endOfDayStrategy;
		protected EndOfDayStrategyBackTester endOfDayStrategyBackTester;

		/// <summary>
		/// provides methods to save scripts' results: to be inherited by all
		/// those scripts that launch an EndOfDayBacktester and that want
		/// their results to be permanently saved
		/// </summary>
		public BasicScriptForBacktesting()
		{
			string pathForTheMainFolderWhereScriptsResultsAreToBeSaved =
				this.getPathForTheMainFolderWhereScriptsResultsAreToBeSaved();
			string customSmallTextForFolderName =
				this.getCustomSmallTextForFolderName();
			string fullPathFileNameForMain =
				this.getFullPathFileNameForMain();
			this.checkIfPathsAreFine(
				pathForTheMainFolderWhereScriptsResultsAreToBeSaved ,
				fullPathFileNameForMain );
			this.initializePrivateMembers(
				pathForTheMainFolderWhereScriptsResultsAreToBeSaved ,
				customSmallTextForFolderName ,
				fullPathFileNameForMain );
		}

		/// <summary>
		/// it has to return the path to the main folder within wich a new subfolder
		/// will be created: this subfolder will contain all the files with the script's results;
		/// the name for the subfolder will be automatically created by this basic class
		/// (some automatic information about the script will be included in the subfolder name)
		/// </summary>
		/// <returns></returns>
		protected abstract string getPathForTheMainFolderWhereScriptsResultsAreToBeSaved();

		/// <summary>
		/// it has to return a short string to be used to build the name for the subfolder
		/// that will will contain all the files with the script's results;
		/// this short string can be used to disambiguate among different
		/// scripts that might be run one next to each the other
		/// </summary>
		/// <returns></returns>
		protected abstract string getCustomSmallTextForFolderName();

		/// <summary>
		/// complete path for the file containing the entry point for the script: it should
		/// be the file that inherits this class
		/// </summary>
		/// <returns></returns>
		protected abstract string getFullPathFileNameForMain();

		protected abstract IEligiblesSelector getEligiblesSelector();

		protected abstract IInSampleChooser getInSampleChooser();

		protected abstract IEndOfDayStrategyForBacktester getEndOfDayStrategy();

		protected abstract EndOfDayStrategyBackTester getEndOfDayStrategyBackTester();

		private void checkIfPathsAreFine(
			string pathForTheMainFolderWhereScriptsResultsAreToBeSaved ,
			string fullPathFileNameForMain )
		{
			if ( !Directory.Exists( pathForTheMainFolderWhereScriptsResultsAreToBeSaved ) )
				throw new Exception(
					"The given path for the main folder where script results are " +
					"to be saved, does not exist!" );
			if ( !File.Exists( fullPathFileNameForMain ) )
				throw new Exception( "The main file does not exist at the given path!" );
		}

		#region initializePrivateMembers
		private void initializePrivateMember_addAFinalSlashIfNeeded()
		{
			string lastCharacter =
				this.pathForTheMainFolderWhereScriptsResultsAreToBeSaved.Substring(
				this.pathForTheMainFolderWhereScriptsResultsAreToBeSaved.Length - 1 );
			if ( lastCharacter != "\\" )
				// the given path does not end with a slash
				this.pathForTheMainFolderWhereScriptsResultsAreToBeSaved +=	"\\";
		}
		private void
			initializePrivateMember_pathForTheMainFolderWhereScriptsResultsAreToBeSaved(
			string pathForTheMainFolderWhereScriptsResultsAreToBeSaved )
		{
			this.pathForTheMainFolderWhereScriptsResultsAreToBeSaved =
				pathForTheMainFolderWhereScriptsResultsAreToBeSaved;
			this.initializePrivateMember_addAFinalSlashIfNeeded();
		}

		private void initializePrivateMembers(
			string pathForTheMainFolderWhereScriptsResultsAreToBeSaved ,
			string customSmallTextForFolderName ,
			string fullPathFileNameForMain )
		{
			this.initializePrivateMember_pathForTheMainFolderWhereScriptsResultsAreToBeSaved(
				pathForTheMainFolderWhereScriptsResultsAreToBeSaved );

			this.customSmallTextForFolderName = customSmallTextForFolderName;
			this.fullPathFileNameForMain = fullPathFileNameForMain;
		}
		#endregion initializePrivateMembers
		
		private void setNameForTheSubfolderForScriptResults()
		{
			this.nameForTheSubfolderForScriptResults =
				ExtendedDateTime.GetCompleteShortDescriptionForFileName( DateTime.Now ) +
				"_" + this.customSmallTextForFolderName;
		}
		private string getInitialPathForTheSubfolderForScriptResults()
		{
			string pathForTheSubfolderForScriptResults =
				this.pathForTheMainFolderWhereScriptsResultsAreToBeSaved +
				this.nameForTheSubfolderForScriptResults + "\\";
			return pathForTheSubfolderForScriptResults;
		}
		
		private void createTheSubfolderForScriptResults()
		{
			string pathForTheFolderForScriptResults=
				this.getInitialPathForTheSubfolderForScriptResults();
      
			if( !Directory.Exists( pathForTheFolderForScriptResults ) )
				Directory.CreateDirectory( pathForTheFolderForScriptResults );
		}
		
		private void copyMainToTheSubfolderForScriptResults()
		{
			string initialPathForTheSubfolderForScriptResults =
				this.getInitialPathForTheSubfolderForScriptResults();
			string fileNameForMain =
				Path.GetFileName( this.fullPathFileNameForMain );
			string initialFullPathFileNameForMainCopy =
				initialPathForTheSubfolderForScriptResults + fileNameForMain;
			File.Copy( this.fullPathFileNameForMain ,
			          initialFullPathFileNameForMainCopy , false );
		}

		private void initializeObjectsForTheBacktest()
		{
			this.eligiblesSelector = this.getEligiblesSelector();
			this.inSampleChooser = this.getInSampleChooser();
			this.endOfDayStrategy = this.getEndOfDayStrategy();
			this.endOfDayStrategyBackTester =	this.getEndOfDayStrategyBackTester();
		}
		
		#region initializeMessageSendersManagement
		private string
			initializeMessageSendersManagement_getInitialNameForOutputFile()
		{
			string initialNameForOutputFile =
				this.nameForTheSubfolderForScriptResults + ".txt";
			return initialNameForOutputFile;
		}
		
		private string getInitialFullPathForOutputTxtFile()
		{
			string fullPathForTheOutputFile =
				this.getInitialPathForTheSubfolderForScriptResults() +
				this.initializeMessageSendersManagement_getInitialNameForOutputFile();
			return fullPathForTheOutputFile;
		}
			
		private void initializeMessageSendersManagement()
		{
			this.messageManager = new MessageManager(
				this.getInitialFullPathForOutputTxtFile() );
			this.messageManager.Monitor( this.eligiblesSelector );
			this.messageManager.Monitor( this.inSampleChooser );
			this.messageManager.Monitor( this.endOfDayStrategy );
			this.messageManager.Monitor( this.endOfDayStrategyBackTester );
		}
		#endregion initializeMessageSendersManagement

		private string getAdditionalInfoForFileName()
		{
			string additionalInfoForFileName =
				"from_" +
				ExtendedDateTime.GetShortDescriptionForFileName(
				this.endOfDayStrategyBackTester.FirstDateTime ) +
				"_to_" +
				ExtendedDateTime.GetShortDescriptionForFileName(
				this.endOfDayStrategyBackTester.ActualLastDateTime ) +
				"_annlRtrn_" +
				this.endOfDayStrategyBackTester.AccountReport.Summary.AnnualSystemPercentageReturn.FormattedValue +
				"_maxDD_" +
				this.endOfDayStrategyBackTester.AccountReport.Summary.MaxEquityDrawDown.FormattedValue;
			return additionalInfoForFileName;
		}

		//Saves (in silent mode):
		//- a log file where the In Sample Analysis are
		//  stored;
		//- a report;
		//- the main file for this script
		private void saveScriptResults()
		{
			string fullPathFileNameForLog =
				this.getInitialPathForTheSubfolderForScriptResults() +
				this.nameForTheSubfolderForScriptResults + "_" +
				this.getAdditionalInfoForFileName() + ".qpL";
			string fullPathFileNameForReport =
				this.getInitialPathForTheSubfolderForScriptResults() +
				this.nameForTheSubfolderForScriptResults + "_" +
				this.getAdditionalInfoForFileName() + ".qpR";
			ObjectArchiver.Archive( endOfDayStrategyBackTester.Log,
				fullPathFileNameForLog );
			ObjectArchiver.Archive( endOfDayStrategyBackTester.AccountReport,
				fullPathFileNameForReport );
		}


		#region renameOutputTxtFile_addingFurtherInformation
		private string getFinalFullPathForOutputTxtFile()
		{
			string initialFullPathForOutputTxtFile =
				this.getInitialFullPathForOutputTxtFile();
			string initialFullPathForOutputTxtFile_withoutSuffix =
				initialFullPathForOutputTxtFile.Substring(
					0 , initialFullPathForOutputTxtFile.Length - 4 );
			string finalFullPathForOutputTxtFile =
				initialFullPathForOutputTxtFile_withoutSuffix + "_" +
				this.getAdditionalInfoForFileName() + ".txt";
			return finalFullPathForOutputTxtFile;
		}
		private void renameOutputTxtFile_addingFurtherInformation()
		{
			string initialFullPathForOutputTxtFile =
				this.getInitialFullPathForOutputTxtFile();
			string finalFullPathForOutputTxtFile =
				this.getFinalFullPathForOutputTxtFile();
			File.Move( initialFullPathForOutputTxtFile ,
			          finalFullPathForOutputTxtFile );
		}
		#endregion renameOutputTxtFile_addingFurtherInformation
		
		#region renameSubfolderForScriptsResults_addingFurtherInformation
		private string getFinalPathForTheSubfolderForScriptResults()
		{
			string initialPathForTheSubfolderForScriptResults =
				this.getInitialPathForTheSubfolderForScriptResults();
			string initialPathForTheSubolderForScriptResults_withoutTheFinalSlash =
				initialPathForTheSubfolderForScriptResults.Substring(
					0 , initialPathForTheSubfolderForScriptResults.Length - 1 );
			string finalPathForTheFolderForScriptResults =
				initialPathForTheSubolderForScriptResults_withoutTheFinalSlash + "_" +
				this.getAdditionalInfoForFileName() + "\\";
			return finalPathForTheFolderForScriptResults;
		}
		private void renameSubfolderForScriptsResults_addingFurtherInformation()
		{
			string initialPathForTheFolderForScriptResults =
				this.getInitialPathForTheSubfolderForScriptResults();
			string finalPathForTheFolderForScriptResults =
				this.getFinalPathForTheSubfolderForScriptResults();
			Directory.Move( initialPathForTheFolderForScriptResults ,
				finalPathForTheFolderForScriptResults );
		}
		#endregion renameSubfolderForScriptsResults_addingFurtherInformation

		

		public void Run()
		{
			this.setNameForTheSubfolderForScriptResults();
			this.createTheSubfolderForScriptResults();
			this.copyMainToTheSubfolderForScriptResults();
			this.initializeObjectsForTheBacktest();
			this.initializeMessageSendersManagement();
			this.endOfDayStrategyBackTester.Run();
			this.saveScriptResults();
			this.renameOutputTxtFile_addingFurtherInformation();
			this.renameSubfolderForScriptsResults_addingFurtherInformation();
		}
	}
}
