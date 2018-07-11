// -----------------------------------------------------------------------
// <copyright file="BitCounter.cs" company="Payvision">
//     Payvision Copyright © 2017
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.CodeChallenge.Algorithms.CountingBits
{
    using System;
    using System.Collections.Generic;

    public class PositiveBitCounter
    {
        /// <summary>
        /// Return an enumerable of integers where the first element is the total number of 1-bits 
        /// in n's binary representation and the subsequent elements are the respective one-indexed 
        /// locations of each 1-bit from most to least significant.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IEnumerable<int> Count(int input)
        {
            // Check if the argument is a positive integer, else it returns an argument exception
            if (input < 0)
            {
                throw new ArgumentException("Argument must be a positive integers");
            }
            else
            {
                // Prepare array for return
                List<int> result = new List<int>();     
                int Counter = 0, i = 0;                 // Initialize one's counter and position pointer

                // Shift the input in one bit while the input is greater than zero
                while (input > 0)
                {
                    if ((input & 1) == 1)   // Fast get if number is even or odd
                    {
                        result.Add(i);      // If input is odd adds a position to the array
                        Counter++;          // Increment one's counter
                    }
                    input = input >> 1;     // Shift the input in one bit to the right
                    i++;                    // Increment position pointer
                }

                // Insert oneCounter at first position
                result.Insert(0, Counter);

                // Return the array
                return result;              
            }
        }
    }
}