using System;
using DSP4DotNET;
using System.IO;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace BinaryAudioDecoder
{
    class Program
    {
        // Change this to the directory housing your files
        // To use the executable path, change it to Application.ExecutablePath
        const string path = @"C:\Users\Mark\Desktop\DeModProj\";
        enum Status { falling = -1, zero = 0, rising = 1 }

        static void Main(string[] args)
        {
            Wave data = new Wave(path + @"FMEncodedFileNormalized.wav");
            StringBuilder sb = new StringBuilder();
            Status status;
            int pointCount = 1;
            Dictionary<int, int> periods = new Dictionary<int, int>();

            // Loop through the wave data, starting after any silence
            for (int i = FindStart(ref data); i < data.m_LeftPCMInt16Data.Length - 1; i++)
            {
                short previous, current;
                previous = data.m_LeftPCMInt16Data[i - 1];
                current = data.m_LeftPCMInt16Data[i];

                // Get wave delta
                if (previous < current) status = Status.rising;
                else if (previous > current) status = Status.falling;
                else status = Status.zero;

                // Zero-crossing event: count point occurrence, add symbol to bit string and reset point counter
                if (IsZeroCrossing(ref data, i) && (status == Status.rising))
                {
                    if (pointCount != 0)
                    {
                        if (!periods.ContainsKey(pointCount)) periods.Add(pointCount, 1);
                        else periods[pointCount]++;
                    }
                    //since spaces come in pairs, we will need to process the string further after this loop
                    if (pointCount > 16 && pointCount < 29) sb.Append('z');
                    else if (pointCount >= 29 && pointCount < 42) sb.Append('1');
                    pointCount = 1;
                }
                pointCount++;
            }

            var list = periods.Keys.ToList();
            list.Sort();

            foreach(var key in list)
            {
                Console.WriteLine(String.Format("[{0}]: {1}", key, periods[key]));
            }

            // Replace all valid space signals with zeros, remove any excess zeros
            sb = sb.Replace("zz", "0");

            if(sb.ToString().Count(c => c == 'z') > 0) Console.WriteLine(String.Format("Warning: I still have {0} unpaired space characters!", sb.ToString().Count(c => c == 'z')));
            sb = sb.Replace("z", "");
            string dataString = sb.ToString().TrimStart('0'); // Remove all Leading null data

            // Start markers derived from inspecting the file
            string pattern = @"(?<data>1101010110111010.+?00000000000000000000000000000000)";
            MatchCollection matches = Regex.Matches(dataString, pattern);

            int fileNum = 0;
            foreach (Match match in matches)
            {
                FileStream file = File.Create(path + @"FMDecodedFile" + fileNum + ".dat");
                //byte[] dataArray = Encoding.ASCII.GetBytes(sb.ToString()); // Write as ASCII
                byte[] dataArray = BinaryStringToByteArray(match.Value);
                file.Write(dataArray, 0, dataArray.Length);
                fileNum++;
            }
            Thread.Sleep(0); // For setting a breakpoint to keep console open when needed
        }

        /// <summary>
        /// Converts a string of bits to a byte array
        /// </summary>
        /// <remarks>http://stackoverflow.com/questions/3436398/convert-a-binary-string-representation-to-a-byte-array</remarks>
        /// <param name="dataString"></param>
        /// <returns>a ByteArray representation of the bit string</returns>
        private static byte[] BinaryStringToByteArray(string dataString)
        {
            //dataString = "0000000" + dataString; // Shift bits (to see what data we get from re-aligning our byte array)
            // Ensure String is a multiple of 8, if not, pad the end with zeros since there should be null padding at the end anyways
            if ((dataString.Length % 8) > 0) dataString.PadRight(dataString.Length + (dataString.Length % 8), '0');
            int size = dataString.Length / 8;
            byte[] data = new byte[size];
            for(int i = 0; i < size; i++)
            {
                data[i] = Convert.ToByte(dataString.Substring(8 * i, 8), 2);
            }

            return data;
        }

        /// <summary>
        /// Seek through wave data to skip silence
        /// </summary>
        /// <param name="data"></param>
        /// <returns>The index where threshold is exceeded</returns>
        private static int FindStart(ref Wave data)
        {
            int i = 0;
            int level;
            for(i = 0; i < data.m_LeftPCMInt16Data.Length - 5; i++)
            {
                level =
                    Math.Abs(data.m_LeftPCMInt16Data[i]) +
                    Math.Abs(data.m_LeftPCMInt16Data[i + 1]) +
                    Math.Abs(data.m_LeftPCMInt16Data[i + 2]) +
                    Math.Abs(data.m_LeftPCMInt16Data[i + 3]) +
                    Math.Abs(data.m_LeftPCMInt16Data[i + 4]) +
                    Math.Abs(data.m_LeftPCMInt16Data[i + 5]);
                if (level > 2000) break;
            }
            return i;
        }

        /// <summary>
        /// Detects once a zero-crossing has occurred
        /// </summary>
        /// <param name="data"></param>
        /// <param name="index"></param>
        /// <returns>True if current point is immediately after a zero-crossing occurence.</returns>
        private static bool IsZeroCrossing(ref Wave data, int index)
        {
            if ((data.m_LeftPCMInt16Data[index - 1] > 0 && data.m_LeftPCMInt16Data[index] < 0)
                || (data.m_LeftPCMInt16Data[index - 1] < 0 && data.m_LeftPCMInt16Data[index] > 0)) return true;
            return false;
        }


    }
}

// Old implementation 
// At first I was tracking peaks, from rise to fall Zc's; then revised to track periods (from rise to rise Zc's) as it is more accurate
/*if (IsZeroCrossing(ref data, i) && (status == Status.falling))
{

    //since spaces come in pairs, we will need to process the string further after this loop
    if (pointCount > 16 && pointCount < 29) sb.Append('z');
    else if(pointCount >= 29 && pointCount < 42) sb.Append('1');
}
else*/
