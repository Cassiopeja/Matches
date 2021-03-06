using System;
using System.Collections.Generic;

namespace Matches.Core
{
    public static class ShuffleExtensions
    {
        private static readonly Random Rng = new Random();  

        public static void Shuffle<T>(this IList<T> list)  
        {  
            var n = list.Count;  
            while (n > 1) {  
                n--;  
                var k = Rng.Next(n + 1);  
                var value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }  
        } 
    }
}