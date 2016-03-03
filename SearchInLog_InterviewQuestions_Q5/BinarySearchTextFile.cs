using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			long returnedPosition = -1;

			bool continueSearch = true;
			long currPosition = (textFileSize / 2);

			while (continueSearch)
			{

				char byteCharContents = (char)0;
				int byteContents = 0;

				long lowNewlinePosition = -1;
				long highNewlinePosition = -1;

				// Looking for the next and previous '\n'
				// and parsing the data inbetween
				using (var fileStream = new FileStream(textFilePath, FileMode.Open))
				{
					

					for (long i = currPosition; i < textFileSize; i++)
					{
						fileStream.Position = i;
						by
					}
				}

				currPosition = (long)Math.Floor((double)currPosition / 2);

			}

			return returnedPosition;
		}
		#endregion


		#region STATIC_METHODS
		private static long FindNewline(FileStream fs, long startPosition, long endPosition, )
		{
			for (long i = currPosition; i > 0; i--)
			{
				fileStream.Position = i;
				byteContents = fileStream.ReadByte();
				byteCharContents = (char)byteContents;
				if (byteCharContents.Equals('\n'))
				{
					lowNewlinePosition = i;
					break;
				}
			}
		}
		#endregion
	}
}
