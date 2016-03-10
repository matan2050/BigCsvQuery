using System;
using System.IO;
using System.Collections.Generic;

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


            List<long>              lowerRange          = new List<long>(2);
            List<long>              upperRange          = new List<long>(2);
            List<long>              outputRange         = new List<long>(4);

            BinarySearchTextFile    bsText              = new BinarySearchTextFile(filePath);
            lowerRange = bsText.Search(startDate, ',', 0);
            upperRange = bsText.Search(endDate, ',', 0);

            outputRange.AddRange(lowerRange);
            outputRange.AddRange(upperRange);

            long[] range = FindListMinMax(outputRange);

            using (var fs = new FileStream(filePath, FileMode.Open))
            {
				string output = bsText.FileContentsRange(fs, range[0], range[1]);
				Console.Write(output);
			}
        }


        /// <summary>
        /// FindListMinMax finds the min and max values in the list, otherwise {0,0}
        /// </summary>
        /// <param name="inList">list of long</param>
        /// <returns>array of min max</returns>
        public static long[] FindListMinMax(List<long> inList)
        {
            long[]  minMaxRange = new long[2] {0,0};
            int     listSize = inList.Count;

            if (listSize > 0)
            {
                minMaxRange[0] = inList[0];
                minMaxRange[1] = inList[0];
            }
            else
            {
                minMaxRange[0] = 0;
                minMaxRange[1] = 0;
            }

            for (int i = 0; i < listSize; i++)
            {
                if (inList[i] < minMaxRange[0])
                {
                    minMaxRange[0] = inList[i];
                }

                if (inList[i] > minMaxRange[1])
                {
                    minMaxRange[1] = inList[i];
                }
            }
            return minMaxRange;
        }
	}
}
