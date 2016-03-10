using System;
using System.IO;

namespace SearchInLog_InterviewQuestions_Q5
{
	class Program
	{
		static void Main(string[] args)
		{

            if (args.Length < 2)
            { 
                Console.WriteLine("Missing parameters");
                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("Specified file does not exist");
            }

            string      filePath    = @args[0];
            string      startDate   = args[1];
            string      endDate     = (args.Length == 3) ? args[2] : args[1];

            BinarySearchTextFile    bsText              = new BinarySearchTextFile(filePath);
            long[]                  lowerRange          = bsText.Search(startDate, ',', 0);
            long[]                  upperRange          = bsText.Search(endDate, ',', 0);
            long[]                  outputRange         = {lowerRange[0], upperRange[1]};

            //long                    currCursor          = outputRange[0];

            using (var fs = new FileStream(filePath, FileMode.Open))
            {
                byte[]  matchingBytes = new byte[outputRange[1] - outputRange[0]];
                fs.Read(matchingBytes, (int)outputRange[0], (int)(outputRange[1] - outputRange[0]));

                string result = System.Text.Encoding.UTF8.GetString(matchingBytes);

                Console.Write(result);
            }




            //string filePath = @"C:\Users\User\Desktop\InterviewQuestions\weblog_sim.log";
            //char delim = ',';
            //string searchedTextLow = "2016-3-19";
            //string searchedTextHigh = "2016-3-28";

            //BinarySearchTextFile bsText			= new BinarySearchTextFile(filePath);

            //long[] lowPositionInFile = bsText.Search(searchedTextLow, delim, 0);

            //         long[] highPositionInFile = bsText.Search(searchedTextHigh, delim, 0);

        }
	}
}
