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

            string filePath = @"C:\Users\User\Desktop\InterviewQuestions\weblog_sim.log";
            char delim = ',';
			string searchedTextLow = "2016-3-19";
            string searchedTextHigh = "2016-3-28";

			BinarySearchTextFile bsText			= new BinarySearchTextFile(filePath);
			long[]               rangeToSearch  = { 0, bsText.TextFileSize };

			long[] lowPositionInFile = bsText.Search(searchedTextLow, delim, 0, rangeToSearch);

            long[] highPositionInFile = bsText.Search(searchedTextHigh, delim, 0, rangeToSearch);

		}
	}
}
