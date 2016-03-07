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
            Up,
            Down
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
        public long[] Search(string searchedTemplate, char textFileDelimiter, int posInCsvValues, long[] searchRange)
        {
            // initialized to negative to indicate no real index was found
            long[]      returnedRange       = new long[2];
            long        currPosition        = (long)(textFileSize / 2);
            DateTime    searchedTime        = DateTime.Parse(searchedTemplate);

            using (var fileStream = new FileStream(textFilePath, FileMode.Open))
            {
                long firstValueInstance = BinarySearchEquality(fileStream, searchedTime, textFileDelimiter, posInCsvValues, searchRange);

                // TODO:    ADD ADDITIONAL BINARY SEARCHES FOR THE LAST AND FIRST
                //          INSTANCES OF THE DESIRED DATE 
                //currPosition = (long)Math.Floor((double)currPosition / 2);
                long[] rangeToSearch = new long[2];
                rangeToSearch[0] = firstValueInstance;
                rangeToSearch[1] = textFileSize;
                long lastValueIndex = BinarySearchInequality(fileStream, searchedTime, textFileDelimiter, posInCsvValues, rangeToSearch, SearchDirection.Up);

                rangeToSearch[0] = 0;
                rangeToSearch[1] = firstValueInstance;
                long firstValueIndex = BinarySearchInequality(fileStream, searchedTime, textFileDelimiter, posInCsvValues, rangeToSearch, SearchDirection.Down);

                returnedRange[0] = firstValueIndex;
                returnedRange[1] = lastValueIndex;
            }

            return returnedRange;
        }
		#endregion


		#region PRIVATE_METHODS
		private long[] FindNewlinePositions(FileStream fs, long currPosition, long fileSizeBytes)
		{
			long[]      newlineIndices      = {-1,-1};
			int         byteContents;
			char        byteCharContents;


			// Looping to find previous newline char
			for (long i = currPosition; i >= 0; i--)
			{

				fs.Position = i;
				byteContents = fs.ReadByte();
				byteCharContents = (char)byteContents;

				if ((byteCharContents.Equals('\n')) || (i == 0))
				{
					newlineIndices[0] = i;
					break;
				}
			}


			// Looping to find next newline char
			for (long i = currPosition; i <= fileSizeBytes; i++)
			{

				fs.Position = i;
				byteContents = fs.ReadByte();
				byteCharContents = (char)byteContents;

				if ((byteCharContents.Equals('\n')) || (i == fileSizeBytes))
				{
					newlineIndices[1] = i;
					break;
				}
			}

			return newlineIndices; 
		}

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

        private long BinarySearchEquality(FileStream fs, DateTime searchedTime, char textFileDelimiter, int posInCsvValues, long[] rangeToSearch)
        {
            long[]      newlinePositions;

            long        returnedPosition    = -1;
            bool        continueSearch      = true;
            long        currPosition        = (long)(textFileSize / 2);

            while (continueSearch)
            {
                // Finding neighbouring newlines
                newlinePositions = FindNewlinePositions(fs, currPosition, textFileSize);

                // Parsing the line between found indices
                string[]    values;
                values = ParseCsvLine(fs, newlinePositions[0] + 1, newlinePositions[1] - 1, ',');

                DateTime currLineTime = DateTime.Parse(values[posInCsvValues]);

                if (searchedTime.Date == currLineTime.Date)
                {
                    returnedPosition = newlinePositions[0] + 1;
                    continueSearch = false;
                    break;
                }

                if (searchedTime.Date > currLineTime.Date)
                {
                    currPosition += (long)(currPosition / 2);
                }

                if (searchedTime.Date < currLineTime.Date)
                {
                    currPosition = (long)(currPosition / 2);
                }
            }

            return returnedPosition;
        }

        private long BinarySearchInequality(FileStream fs, DateTime unequalToTime, char textFileDelimiter, int posInCsvValues, long[] rangeToSearch, SearchDirection searchDir)
        {
            long            returnedPosition        = -1;

            List<long>      prevPositionsArchive    = new List<long>();
            List<DateTime>  prevValuesArchive       = new List<DateTime>();

            long[]          rangeToSearchTemp       = rangeToSearch;
            long[]          newlinePositions;

            long            currPosition            = rangeToSearch[1] - (rangeToSearch[1] - rangeToSearch[0])/2;      // Default is SearchDriection.Down - CHECK IF NECESSARY
            bool            continueSearch          = true;

            while (continueSearch)
            {
                if (searchDir == SearchDirection.Up)
                {
                    currPosition = rangeToSearchTemp[0] + (rangeToSearchTemp[1] - rangeToSearchTemp[0]) / 2;
                }
                else
                {
                    currPosition = rangeToSearchTemp[1] - (rangeToSearchTemp[1] - rangeToSearchTemp[0]) / 2;
                }

                newlinePositions = FindNewlinePositions(fs, currPosition, textFileSize);
                
                string[]    values;
                //values = ParseCsvLine(fs, newlinePositions[0] + 1, newlinePositions[1] - 1, ',');
                values = ParseCsvLine(fs, newlinePositions[0], newlinePositions[1], textFileDelimiter);
                DateTime currLineTime = DateTime.Parse(values[posInCsvValues]);

                prevPositionsArchive.Add(newlinePositions[0]);
                prevValuesArchive.Add(currLineTime);

                if (currLineTime.Date < unequalToTime.Date)
                {
                    rangeToSearchTemp[0] = newlinePositions[0];
                }

                if (currLineTime.Date > unequalToTime.Date)
                {
                    rangeToSearchTemp[1] = newlinePositions[0];
                }

                if (currLineTime.Date == unequalToTime.Date)
                {
                    if (searchDir == SearchDirection.Up)
                    {
                        rangeToSearchTemp[0] = newlinePositions[0];
                    }
                    else
                    {
                        rangeToSearchTemp[1] = newlinePositions[0];
                    }
                    if (prevPositionsArchive[prevPositionsArchive.Count - 1] == rangeToSearchTemp[0])
                    {
                        returnedPosition = rangeToSearchTemp[0];
                        continueSearch = false;
                        break;
                    }
                }
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
