using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchInLog_InterviewQuestions_Q5
{
	class Program
	{
		static void Main(string[] args)
		{

			string filePath = @"C:\Users\User\Desktop\InterviewQuestions\web.log";
			char delim = ',';
			string searchedTextLow = "2013-11-13";
            string searchedTextHigh = "2013-11-14";

			BinarySearchTextFile bsText			= new BinarySearchTextFile(filePath);
			long[]               rangeToSearch  = { 0, bsText.TextFileSize };

			long[] lowPositionInFile = bsText.Search(searchedTextLow, delim, 0, rangeToSearch);

            long[] highPositionInFile = bsText.Search(searchedTextHigh, delim, 0, rangeToSearch);

		}
	}
}
