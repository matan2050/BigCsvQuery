using System;
using System.IO;
using System.Collections.Generic;

namespace SearchInLog_InterviewQuestions_Q5
{

    public enum SearchDirection
    {
        Forward,
        Backward
    };

    /// <summary>
    /// BinarySearchTextFile represents a binary search for finding first and last line occurance
    /// of DateTime string index in a text file
    /// </summary>
	public class BinarySearchTextFile
	{
		#region FIELDS
		private     string      textFilePath;
		private     long        textFileSize;
        private     long[]      initialSearchRange;    
		#endregion


		#region CONSTRUCTORS
		public BinarySearchTextFile(string _textFilePath)
		{
			textFilePath = _textFilePath;

			// Get file size in bytes
			var textFileInfo = new FileInfo(textFilePath);
			textFileSize = textFileInfo.Length;

            initialSearchRange = new long[2];
            initialSearchRange[0] = 0;
            initialSearchRange[1] = textFileSize;
		}
        #endregion


        #region PUBLIC_METHODS
        /// <summary>
        /// Search returns an array containing the byte position in the file,
        /// in which the searched value appears first and last
        /// </summary>
        /// <param name="searchedTemplate">DateTime representing single date</param>
        /// <param name="textFileDelimiter">Csv delimiter character</param>
        /// <param name="posInCsvValues">Location of the relevant value for searching in the csv line</param>
        /// <param name="searchRange">Upper and lower bound searching</param>
        /// <returns></returns>
        public long[] Search(string searchedTemplate, char delim, int posInCsvValues)
        {
            // initialized to negative to indicate no real index was found
            long[]      returnedRange       = new long[2];
            long        currPosition        = (long)(textFileSize / 2);
            DateTime    searchedTime        = DateTime.Parse(searchedTemplate);

            using (var fileStream = new FileStream(textFilePath, FileMode.Open))
            {
                // getting first found line containing the searched value
                long    firstObserved = BinarySearch(fileStream, searchedTime, delim, posInCsvValues, initialSearchRange);

                // two separate binary searches in both ranges divided by 'firstObserved' 
                long[]  rangeToSearch   = new long[2];

                rangeToSearch[0] = 0;
                rangeToSearch[1] = firstObserved;

                long    firstValueIndex = DirectedBinarySearch(fileStream, searchedTime, delim, posInCsvValues, rangeToSearch, SearchDirection.Backward);

                rangeToSearch[0] = firstObserved;
                rangeToSearch[1] = textFileSize;

                long    lastValueIndex  = DirectedBinarySearch(fileStream, searchedTime, delim, posInCsvValues, rangeToSearch, SearchDirection.Forward);

                returnedRange[0] = firstValueIndex;
                returnedRange[1] = lastValueIndex;
            }

            return returnedRange;
        }

        public long FindNextNewline(FileStream fs, long currPosition, SearchDirection searchDir)
        {
            long    nextNewlinePos      = -1;
            bool    continueScan        = true;
            char    charInCurrentPos    = ByteInFileToChar(fs, currPosition);

            if (charInCurrentPos == '\n')
            {
                currPosition = (searchDir == SearchDirection.Forward) ? currPosition += 1 : currPosition -= 1;
            }

            long i = currPosition;

            charInCurrentPos = ByteInFileToChar(fs, i);

            while (continueScan)
            {
                charInCurrentPos = ByteInFileToChar(fs, i);

                if (charInCurrentPos == '\n')
                {
                    // check if we are not starting in a newline (@currPosition)
                    nextNewlinePos = i;
                    continueScan = false;
                    break;
                }

                if (searchDir == SearchDirection.Forward)
                {
                    i++;
                    if (i == textFileSize)
                    {
                        nextNewlinePos = i;
                        continueScan = false;
                    }
                }
                else
                {
                    i--;
                    if (i == 0)
                    {
                        nextNewlinePos = i;
                        continueScan = false;
                    }
                }

            }
            return nextNewlinePos;
        }

        /// <summary>
        /// Parse csv line to array of string values
        /// </summary>
        /// <param name="fs">Filestream object streaming from desired file</param>
        /// <param name="indexLow">Position index in file for the start of the csv line</param>
        /// <param name="indexHigh">Position index in file for the end of the csv line</param>
        /// <param name="delim">Delimiter character</param>
        /// <returns></returns>
		public string[] ParseCsvLine(FileStream fs, long indexLow, long indexHigh, char delim)
        {
            string[]    csvValues;
            char        currCharByte;
            char[]      currLineChars       = new char[indexHigh - indexLow];
            string      currLineString;

            for (long i = indexLow; i < indexHigh; i++)
            {
                currCharByte = ByteInFileToChar(fs, i);
                currLineChars[i - indexLow] = currCharByte;
            }

            currLineString = new String(currLineChars);
			currLineString = currLineString.Trim('\n');
		csvValues = currLineString.Split(delim);

            return csvValues;
        }

		public string FileContentsRange(FileStream fs, long indexLow, long indexHigh)
		{
			string returnedString = "";

			long currPosition = indexLow;

			while (currPosition < indexHigh)
			{
				long[]		currNewlineIndex		= FindNewlinePositions(fs, currPosition, TextFileSize);
				string[]	lineValues				= ParseCsvLine(fs, currNewlineIndex[0], currNewlineIndex[1], ',');

				for (int i = 0; i < lineValues.Length; i++)
				{
					returnedString += lineValues[i];
					returnedString = (i == (lineValues.Length - 1)) ?
						returnedString + '\n' : returnedString + ',';

					currPosition = currNewlineIndex[1] + 1;
				}					
			}

			return returnedString;
		}
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Returns newline character position neighbouring the given position
        /// </summary>
        /// <param name="fs">Filestream object</param>
        /// <param name="currPosition">Position index in bytes, from which we want to extract neighbouring newlines</param>
        /// <param name="fileSizeBytes">Size of the file in bytes</param>
        /// <returns></returns>
        private long[] FindNewlinePositions(FileStream fs, long currPosition, long fileSizeBytes)
		{
			long[]      newlineIndices      = new long[2];

			if (ByteInFileToChar(fs, currPosition) == '\n')
			{
				currPosition++;
			}


			newlineIndices[0] = FindNextNewline(fs, currPosition, SearchDirection.Backward);
			newlineIndices[1] = FindNextNewline(fs, currPosition, SearchDirection.Forward);

			return newlineIndices; 
		}

        

        /// <summary>
        /// Returns the char in the position given as parameter
        /// </summary>
        /// <param name="fs">Filestream object initialized to the given file path</param>
        /// <param name="position">Position index in bytes</param>
        /// <returns></returns>
        private char ByteInFileToChar(FileStream fs, long position)
        {
            fs.Position = position;
            int byteContents = fs.ReadByte();
            return (char)byteContents;
        }

        private long BinarySearch(FileStream fs, DateTime searchedTime, char delim, int posInCsvValues, long[] rangeToSearch)
        {
            long[]      newlinePositions;

            long        returnedPosition    = -1;
            bool        continueSearch      = true;
            long        currPosition        = (long)(textFileSize / 2);

            long        searchedTimeTicks   = searchedTime.Date.Ticks;

            while (continueSearch)
            {
                newlinePositions = FindNewlinePositions(fs, currPosition, textFileSize);

                string[]    values          = ParseCsvLine(fs, newlinePositions[0] + 1, newlinePositions[1], delim);
                DateTime    currLineTime    = DateTime.Parse(values[posInCsvValues]);
                long        currLineTicks   = currLineTime.Date.Ticks;

                if (searchedTimeTicks == currLineTicks)
                {
                    returnedPosition = newlinePositions[0] + 1;
                    continueSearch = false;
                    break;
                }

                if (searchedTimeTicks > currLineTicks)
                {
                    currPosition += (long)(currPosition / 2);
                }

                if (searchedTimeTicks < currLineTicks)
                {
                    currPosition = (long)(currPosition / 2);
                }
            }

            return returnedPosition;
        }

        private long DirectedBinarySearch(FileStream fs, DateTime searchedTime, char delim, int posInCsvValues, long[] rangeToSearch, SearchDirection searchDir)
        {
            long            returnedPosition        = -1;
            List<long>      prevPositionsArchive    = new List<long>();
            long[]          rangeToSearchTemp       = rangeToSearch;
            long            searchedTimeTicks       = searchedTime.Date.Ticks;
            bool            continueSearch          = true;

            long[]          newlinePositions;
            long            currPosition;
            long            currLineTicks;

            while (continueSearch)
            {
                currPosition = rangeToSearchTemp[1] - (rangeToSearchTemp[1] - rangeToSearchTemp[0]) / 2;
                newlinePositions = FindNewlinePositions(fs, currPosition, textFileSize);
                
                string[]    values;
                values = ParseCsvLine(fs, newlinePositions[0], newlinePositions[1], delim);

                DateTime    currLineTime        = DateTime.Parse(values[posInCsvValues]);
                currLineTicks = currLineTime.Date.Ticks;

                if (currLineTicks < searchedTimeTicks)
                {
                    rangeToSearchTemp[0] = currPosition;
                }

                if (currLineTicks > searchedTimeTicks)
                {
                    rangeToSearchTemp[1] = currPosition;
                }

                if (currLineTicks == searchedTimeTicks)
                {
                    if (searchDir == SearchDirection.Forward)
                    {
                        rangeToSearchTemp[0] = currPosition;
                    }
                    else
                    {
                        rangeToSearchTemp[1] = currPosition;
                    }
                }

                if (prevPositionsArchive.Count > 1)
                {
                    if (prevPositionsArchive[prevPositionsArchive.Count - 1] == newlinePositions[0])
                    {
                        returnedPosition = prevPositionsArchive[prevPositionsArchive.Count - 1];
						continueSearch = false;

						if (currLineTicks != searchedTimeTicks)
                        {
                            long nextLineInDirection;

							// Propogating index to next newline if we are in a wrong value-line
                            SearchDirection finalSearchDir = (searchDir == SearchDirection.Forward) ? SearchDirection.Backward : SearchDirection.Forward;
                            nextLineInDirection = FindNextNewline(fs, returnedPosition, finalSearchDir);
                            returnedPosition = nextLineInDirection;
						}

						if (searchDir == SearchDirection.Forward)
						{
							returnedPosition = (returnedPosition == -1) ?
								textFileSize : FindNextNewline(fs, returnedPosition, searchDir);
						}
                    }
                }

				prevPositionsArchive.Add(newlinePositions[0]);
            }

            return returnedPosition;
        }
        #endregion


        #region PROPERTIES
        public long TextFileSize
		{
			get
			{
				return textFileSize;
			}			
			set
			{
				textFileSize = value;
			}
		}


		public string TextFilePath
		{
			get
			{
				return textFilePath;
			}
			set
			{
				textFilePath = value;
			}
		}
		#endregion
	}
}
