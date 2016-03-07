using System;
using System.IO;

namespace SearchInLog_InterviewQuestions_Q5
{
	public class BinarySearchTextFile
	{
		#region FIELDS
		private string textFilePath;
		private long textFileSize;
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
        public long Search(string searchedTemplate, char textFileDelimiter, int posInCsvValues, bool equalToTemplate, long[] searchRange)
        {
            // initialized to negative to indicate no real index was found
            long        returnedPosition    = -1;
            //bool        continueSearch      = true;
            long        currPosition        = (long)(textFileSize / 2);
            DateTime    searchedTime        = DateTime.Parse(searchedTemplate);

            using (var fileStream = new FileStream(textFilePath, FileMode.Open))
            {
                long firstValueInstance = InitialSearch(fileStream, searchedTime, textFileDelimiter, posInCsvValues, equalToTemplate, searchRange);
                
                // TODO:    ADD ADDITIONAL BINARY SEARCHES FOR THE LAST AND FIRST
                //          INSTANCES OF THE DESIRED DATE 
                currPosition = (long)Math.Floor((double)currPosition / 2);
            }

            return returnedPosition;
        }
		#endregion


		#region PRIVATE_METHODS
		private long[] FindNewlinePositions(FileStream fs, long currPosition, long fileSizeBytes)
		{
			long[]      newlineIndices      = {-1,-1};
			int         byteContents;
			char        byteCharContents;


			// Looping to find previous newline char
			for (long i = currPosition; i > 0; i--)
			{

				fs.Position = i;
				byteContents = fs.ReadByte();
				byteCharContents = (char)byteContents;

				if (byteCharContents.Equals('\n'))
				{
					newlineIndices[0] = i;
					break;
				}
			}


			// Looping to find next newline char
			for (long i = currPosition; i < fileSizeBytes; i++)
			{

				fs.Position = i;
				byteContents = fs.ReadByte();
				byteCharContents = (char)byteContents;

				if (byteCharContents.Equals('\n'))
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

        private long InitialSearch(FileStream fs, DateTime searchedTime, char textFileDelimiter, int posInCsvValues, bool isEqualToTemplate, long[] rangeToSearch)
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

                if (searchedTime == currLineTime)
                {
                    returnedPosition = newlinePositions[0] + 1;
                    continueSearch = false;
                    break;
                }

                if (searchedTime > currLineTime)
                {
                    currPosition += (long)(currPosition / 2);
                }

                if (searchedTime < currLineTime)
                {
                    currPosition = (long)(currPosition / 2);
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
