using System;
using System.IO;
using System.Collections.Generic;

namespace SearchInLog_InterviewQuestions_Q5
{
	public class BinarySearchTextFile
	{
		#region FIELDS
		private string textFilePath;
		private long textFileSize;

        enum SearchDirection
        {
            Forward,
            Backward
        };
		#endregion


		#region CONSTRUCTORS
		public BinarySearchTextFile(string _textFilePath)
		{
			textFilePath = _textFilePath;

			// Get file size in bytes
			var textFileInfo = new FileInfo(textFilePath);
			textFileSize = textFileInfo.Length;
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
        public long[] Search(string searchedTemplate, char delim, int posInCsvValues, long[] searchRange)
        {
            // initialized to negative to indicate no real index was found
            long[]      returnedRange       = new long[2];
            long        currPosition        = (long)(textFileSize / 2);
            DateTime    searchedTime        = DateTime.Parse(searchedTemplate);

            using (var fileStream = new FileStream(textFilePath, FileMode.Open))
            {
                // getting first found line containing the searched value
                long    firstObserved = BinarySearch(fileStream, searchedTime, delim, posInCsvValues, searchRange);

                // starting binary search in the latter part of the file
                long[]  rangeToSearch   = {firstObserved, textFileSize};
                long    lastValueIndex  = BinarySearchDirected(fileStream, searchedTime, delim, posInCsvValues, rangeToSearch, SearchDirection.Forward);

                rangeToSearch[0] = 0;
                rangeToSearch[1] = firstObserved;
                long    firstValueIndex = BinarySearchDirected(fileStream, searchedTime, delim, posInCsvValues, rangeToSearch, SearchDirection.Backward);

                returnedRange[0] = firstValueIndex;
                returnedRange[1] = lastValueIndex;
            }

            return returnedRange;
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
			long[]      newlineIndices      = {-1,-1};
			char        byteCharContents;


            // Check if we didn't land exactly on a newline character
            byteCharContents = ByteInFileToChar(fs, currPosition);
            if (byteCharContents == '\n')
            {
                currPosition += 1;
            }

            // Looping to find previous newline char
            for (long i = currPosition; i >= 0; i--)
			{

                byteCharContents = ByteInFileToChar(fs, i);

                if ((byteCharContents.Equals('\n')) || (i == 0))
				{
					newlineIndices[0] = i;
					break;
				}
			}

			// Looping to find next newline char
			for (long i = currPosition; i <= fileSizeBytes; i++)
			{

                byteCharContents = ByteInFileToChar(fs, i);

                if ((byteCharContents.Equals('\n')) || (i == fileSizeBytes))
				{
					newlineIndices[1] = i;
					break;
				}
			}

			return newlineIndices; 
		}

        /// <summary>
        /// Parse csv line to array of string values
        /// </summary>
        /// <param name="fs">Filestream object streaming from desired file</param>
        /// <param name="indexLow">Position index in file for the start of the csv line</param>
        /// <param name="indexHigh">Position index in file for the end of the csv line</param>
        /// <param name="delim">Delimiter character</param>
        /// <returns></returns>
		private string[] ParseCsvLine(FileStream fs, long indexLow, long indexHigh, char delim)
		{
			string[]    csvValues;
			int         currByte;
			char        currCharByte;
			char[]      currLineChars		= new char[indexHigh - indexLow];
			string      currLineString;
									
			for (long i = indexLow; i < indexHigh; i++)
			{
				fs.Position = i;
				currByte = fs.ReadByte();
				currCharByte = (char)currByte;
				currLineChars[i - indexLow] = currCharByte;
			}

			currLineString = new String(currLineChars);
			csvValues = currLineString.Split(delim);

			return csvValues;
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

                string[]    values          = ParseCsvLine(fs, newlinePositions[0] + 1, newlinePositions[1] - 1, delim);
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

        private long BinarySearchDirected(FileStream fs, DateTime searchedTime, char delim, int posInCsvValues, long[] rangeToSearch, SearchDirection searchDir)
        {
            long            returnedPosition        = -1;
            List<long>      prevPositionsArchive    = new List<long>();
            long[]          rangeToSearchTemp       = rangeToSearch;
            long            searchedTimeTicks       = searchedTime.Date.Ticks;
            bool            continueSearch          = true;

            long[]          newlinePositions;
            long            currPosition;
            
            while (continueSearch)
            {
                currPosition = rangeToSearchTemp[1] - (rangeToSearchTemp[1] - rangeToSearchTemp[0]) / 2;
                newlinePositions = FindNewlinePositions(fs, currPosition, textFileSize);
                
                string[]    values;
                values = ParseCsvLine(fs, newlinePositions[0], newlinePositions[1], delim);

                DateTime    currLineTime        = DateTime.Parse(values[posInCsvValues]);
                long        currLineTicks       = currLineTime.Date.Ticks;

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
                    //if (prevPositionsArchive.Count >= 1)
                    //{
                    //    if (prevPositionsArchive[prevPositionsArchive.Count - 1] == rangeToSearchTemp[0])
                    //    {
                    //        returnedPosition = rangeToSearchTemp[0];
                    //        continueSearch = false;
                    //        break;
                    //    }
                    //}
                }

                if (prevPositionsArchive.Count > 1)
                {
                    if (prevPositionsArchive[prevPositionsArchive.Count - 1] == newlinePositions[0])
                    {
                        returnedPosition = prevPositionsArchive[prevPositionsArchive.Count - 1];

                        if (currLineTicks != searchedTimeTicks)
                        {
                            
                        }
                        break;
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
