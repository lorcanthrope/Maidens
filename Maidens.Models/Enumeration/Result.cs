using System;
namespace Maidens.Models
{
    public enum Result
    {
        Unspecified = -1,
        First = 1,
        Second = 2,
        Third = 3,
        Fourth = 4,
        Fifth = 5,
        Sixth = 6
    }

    public static class ResultExtension
    {
        public static int Points(this Result source)
        {
            switch (source)
            {
                case Result.First:
                    return 3;
                case Result.Second:
                case Result.Third:
                    return 2;
                case Result.Fourth:
                case Result.Fifth:
                    return 1;
                case Result.Sixth:
                    return 0;
            }
            return 0;
        }

        public static Result[] ToArray(this Result source)
        {            
            Array a = Enum.GetValues(source.GetType());
            Result[] output = new Result[a.Length];
            for(int i=0;i<a.Length;i++)
            {
                output[i] = (Result)a.GetValue(i);
            }
            return output;
        }
    }
}
