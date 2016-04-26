using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuzzleApp2
{
    //Fisher–Yates shuffle
    //https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle

    //-- To shuffle an array a of n elements (indices 0..n-1):
    // for i from 0 to n−2 do
    // j ← random integer such that 0 ≤ j < n-i
    // exchange a[i] and a[i+j]

    public static class MatrixGenerator
    {
        private static Random rand;
        private static List<int> result;

        private static void GenerateSequence(int count)
        {
            result = new List<int>();

            for (int i = 0; i < count; i++)
                result.Add(i + 1);
        }

        private static void ShuffleSequence()
        {
            rand = new Random();
            int j, temp;
            for(int i = 0; i < result.Count - 1; i++)
            {
                j = rand.Next(0, result.Count - i);
                temp = result[i];
                result[i] = result[i + j];
                result[i + j] = temp;
            }
        }

        public static List<int> GetSequence(int count)
        {
            GenerateSequence(count);
            ShuffleSequence();
            return result;
        }

        public static List<List<int>> GetMatrix(int count)
        {
            int size = (int)Math.Sqrt((double)count);
            List<List<int>> matrixResult = new List<List<int>>();
            matrixResult.Add(new List<int>());

            GenerateSequence(count);
            ShuffleSequence();

            int j, k;
            j = k = 0;
            for(int i = 0; i < count; i++)
            {
                if( i != 0 && i % size == 0)
                {
                    j++;
                    matrixResult.Add(new List<int>());
                    k = 0;
                    Console.WriteLine();
                }
                matrixResult[j].Add(result[i]);
                Console.Write(matrixResult[j][k] + " |");
                k++;
            }
            Console.WriteLine();
            return matrixResult;
        }
    }
}
