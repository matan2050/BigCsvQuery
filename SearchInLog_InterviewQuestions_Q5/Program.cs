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
			string searchedText = "2013-11-13 11:45:14";

			BinarySearchTextFile bsText			= new BinarySearchTextFile(filePath);
			long[]               rangeToSearch  = { 0, bsText.TextFileSize };

			long[] positionInFile = bsText.Search(searchedText, delim, 0, rangeToSearch);

		}
	}
}
