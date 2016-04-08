using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

class Program
{
    #region Private variables

    // the secret password which we will try to find via brute force
    private static string password = "!eT@!e";
    private static string result;

    private static bool isMatched = false;

    /* The length of the charactersToTest Array is stored in a
     * additional variable to increase performance  */
    private static int charactersToTestLength = 0;
    private static long computedKeys = 0;

    /* An array containing the characters which will be used to create the brute force keys,
     * if less characters are used (e.g. only lower case chars) the faster the password is matched  */
    private static char[] charactersToTest =
    {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
        'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't',
        'u', 'v', 'w', 'x', 'y', 'z','A','B','C','D','E',
        'F','G','H','I','J','K','L','M','N','O','P','Q','R',
        'S','T','U','V','W','X','Y','Z','1','2','3','4','5',
        '6','7','8','9','0','!','$','#','@','-'
    };

    #endregion

    static void Main(string[] args)
    {
        var timeStarted = DateTime.Now;
        Console.WriteLine("Start BruteForce - {0}", timeStarted.ToString());

        // The length of the array is stored permanently during runtime
        charactersToTestLength = charactersToTest.Length;

        // The length of the password is unknown, so we have to run trough the full search space
        var estimatedPasswordLength = 0;

        while (!isMatched)
        {
            /* The estimated length of the password will be increased and every possible key for this
             * key length will be created and compared against the password */
            estimatedPasswordLength++;
            startBruteForce(estimatedPasswordLength);
        }

        Console.WriteLine("Password matched. - {0}", DateTime.Now.ToString());
        Console.WriteLine("Time passed: {0}s", DateTime.Now.Subtract(timeStarted).TotalSeconds);
        Console.WriteLine("Resolved password: {0}", result);
        Console.WriteLine("Computed keys: {0}", computedKeys);

        Console.ReadLine();
    }

    #region Private methods

    /// <summary>
    /// Starts the recursive method which will create the keys via brute force
    /// </summary>
    /// <param name="keyLength">The length of the key</param>
    private static void startBruteForce(int keyLength)
    {
        var keyChars = createCharArray(keyLength, charactersToTest[0]);
        // The index of the last character will be stored for slight perfomance improvement
        var indexOfLastChar = keyLength - 1;
        createNewKey(0, keyChars, keyLength, indexOfLastChar);
    }

    /// <summary>
    /// Creates a new char array of a specific length filled with the defaultChar
    /// </summary>
    /// <param name="length">The length of the array</param>
    /// <param name="defaultChar">The char with whom the array will be filled</param>
    /// <returns></returns>
    private static char[] createCharArray(int length, char defaultChar)
    {
        return (from c in new char[length] select defaultChar).ToArray();
    }

    /// <summary>
    /// This is the main workhorse, it creates new keys and compares them to the password until the password
    /// is matched or all keys of the current key length have been checked
    /// </summary>
    /// <param name="currentCharPosition">The position of the char which is replaced by new characters currently</param>
    /// <param name="keyChars">The current key represented as char array</param>
    /// <param name="keyLength">The length of the key</param>
    /// <param name="indexOfLastChar">The index of the last character of the key</param>
    private static void createNewKey(int currentCharPosition, char[] keyChars, int keyLength, int indexOfLastChar)
    {
        var nextCharPosition = currentCharPosition + 1;
        // We are looping trough the full length of our charactersToTest array
        for (int i = 0; i < charactersToTestLength; i++)
        {
            /* The character at the currentCharPosition will be replaced by a
             * new character from the charactersToTest array => a new key combination will be created */
            keyChars[currentCharPosition] = charactersToTest[i];

            // The method calls itself recursively until all positions of the key char array have been replaced
            if (currentCharPosition < indexOfLastChar)
            {
                createNewKey(nextCharPosition, keyChars, keyLength, indexOfLastChar);
            }
            else
            {
                // A new key has been created, remove this counter to improve performance
                computedKeys++;

                /* The char array will be converted to a string and compared to the password. If the password
                 * is matched the loop breaks and the password is stored as result. */
                if ((new String(keyChars)) == password)
                {
                    if (!isMatched)
                    {
                        isMatched = true;
                        result = new String(keyChars);
                    }
                    return;
                }
                else
                {
                    Console.WriteLine(new String(keyChars));
                }
            }
        }
    }

    #endregion
}