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

			string filePath = @"C:\Users\matan\Desktop\int\web.log";
			char delim = ',';
			string searchedText = "2013-11-14 11:45:14";

			BinarySearchTextFile bsText = new BinarySearchTextFile(filePath);

			bsText.Search(searchedText, delim, 1);

		}
	}
}
