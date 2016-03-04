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


		#region METHODS
		public long Search(string SearchedTemplate, char textFileDelimiter, int positionInDelimitedValues)
		{
			// initialized to negative to indicate no real index was found
			long    returnedPosition    = -1;
			bool    continueSearch      = true;
			long    currPosition        = (textFileSize / 2);

			using (var fileStream = new FileStream(textFilePath, FileMode.Open))
			{

				char    byteCharContents;
				int     byteContents            = 0;

				long[]  newlinePositions;


				// Looking for adjacent newline chars, and parsing line
				while (continueSearch)
				{
					
					// Finding neighbouring newlines
					newlinePositions = FindNewlinePositions(fileStream, currPosition, textFileSize);

					// Parsing the line between found indices
					string[]    values;
					values = ParseCsvLine(fileStream, newlinePositions[0] + 1, newlinePositions[1] - 1, ',');

					if (values[positionInDelimitedValues].Equals(SearchedTemplate))
					{
						returnedPosition = newlinePositions[0] + 1;
						continueSearch = false;
						break;
					}


					currPosition = (long)Math.Floor((double)currPosition / 2);
				}
			}

			return returnedPosition;
		}
		#endregion


		#region STATIC_METHODS
		private static long[] FindNewlinePositions(FileStream fs, long currPosition, long fileSizeBytes)
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

		private static string[] ParseCsvLine(FileStream fs, long indexLow, long indexHigh, char delim)
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
		#endregion
	}
}
