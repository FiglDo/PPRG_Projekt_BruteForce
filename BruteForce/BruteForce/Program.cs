using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BruteForce
{

    //https://msdn.microsoft.com/en-us/library/dd997306.aspx
    static class Program
    {
        static char[] attackVector = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r',
        's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R',
        'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ','!', '"','#','$','%','&','\'',')','(','*','+',
        '´','-','.','/',':',';','<','=','>','?','@','[','\\',']','^','_','`','{','|','}','~'};

        static Object lockObj = new object();
        static int attackVectorLengt = attackVector.Length;
        static bool showOutput = true;
        static int finishedProducers = 0;
        static int maxLength;
        static string password;

        static void Main()
        {
            Console.WriteLine("Welcome to C# - Brute Force!");
            Console.Write("Please enter a password: ");
            password = Console.ReadLine();
            Console.Write("Max. Length [5]: ");
            var maxLengthS = Console.ReadLine();
            maxLength = String.IsNullOrEmpty(maxLengthS)?5:Int32.Parse(maxLengthS);
            Console.Write("Show Steps (true/[false]: ");
            var showOutputS = Console.ReadLine();
            showOutput = String.IsNullOrEmpty(showOutputS) ? false : Boolean.Parse(showOutputS);


            // The token source for issuing the cancelation request.
            CancellationTokenSource cts = new CancellationTokenSource();

            // A blocking collection that can hold no more than 3000 items at a time.
            BlockingCollection<char[]> attackCollection = new BlockingCollection<char[]>();//100000);

            // Set console buffer to hold our prodigious output.
            Console.SetBufferSize(80, 4000);

            // The simplest UI thread ever invented.
            Task.Run(() =>
            {
                if (Console.ReadKey(true).KeyChar == 'c')
                    cts.Cancel();
            });

            List<Task> WaitingTasks = new List<Task>();
            // Start producers and one consumers.

            // Use ParallelOptions instance to store the CancellationToken
            ParallelOptions po = new ParallelOptions();
            po.CancellationToken = cts.Token;

            try
            {
                //Producer
                for (int i = 1; i < maxLength+1; i++)
                {
                    int start_length = i;
                    WaitingTasks.Add(
                            Task.Run(() => NonBlockingProducer(attackCollection, start_length, cts.Token))
                    );
                }


                //Consumer
                for (int i = 0; i < (maxLength*2); i++)
                {
                    WaitingTasks.Add(
                        Task.Run(() => NonBlockingConsumer(attackCollection, cts))
                    );
                }


            // Wait for the tasks to complete execution
            Task.WaitAll(WaitingTasks.ToArray(),cts.Token);

            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Threads Aborted");
            }


            cts.Dispose();
            Console.WriteLine("Press the Enter key to exit.");
            Console.ReadLine();
            Environment.Exit(0);
        }

        static void NonBlockingConsumer(BlockingCollection<char[]> bc, CancellationTokenSource cts)
        {
            // IsCompleted == (IsAddingCompleted && Count == 0)
            while (!bc.IsCompleted)
            {
                char[] nextItem;
                try
                {
                    if (!bc.TryTake(out nextItem, 0, cts.Token))
                    {
                        Console.WriteLine("Take Blocked");
                    }
                    else
                    {
                        String _try = new String(nextItem);

                        if (showOutput)
                        {
                            Console.WriteLine(" Take:{0}", _try);
                        }

                        if(_try.Equals(password))
                        {
                            cts.Cancel();
                            Console.WriteLine("PASSWORD FOUND! => '{0}'",_try);
                        }
                    }
                }

                catch (OperationCanceledException)
                {
                    Console.WriteLine("Taking canceled.");
                    break;
                }

                // Slow down consumer just a little to cause
                // collection to fill up faster, and lead to "AddBlocked"
                Thread.SpinWait(5000);
            }

            Console.WriteLine("\r\nNo more items to take.");
        }

        static void NonBlockingProducer(BlockingCollection<char[]> bc, int length, CancellationToken ct)
        {
            Console.WriteLine("Starting Producer for length {0}",length);
            List<char[]> attack = new List<char[]>(length);

            for (int i = 0; i < length; i++)
            {
                attack.Add(attackVector);
            }

            var resultQuery = CartesianProduct(attack);

            // Use ParallelOptions instance to store the CancellationToken
            ParallelOptions po = new ParallelOptions();
            po.CancellationToken = ct;

            try
            {
                Parallel.ForEach(resultQuery, po, item =>
                    {
                        bool success = false;
                        do
                        {
                            char[] itemToAdd = item.ToArray();
                            // Cancellation causes OCE. We know how to handle it.
                            try
                            {
                                // A shorter timeout causes more failures.
                                success = bc.TryAdd(itemToAdd, 2, ct);
                            }
                            catch (OperationCanceledException)
                            {
                                Console.WriteLine("Add loop canceled.");
                                // Let other threads know we're done in case
                                // they aren't monitoring the cancellation token.
                                lock (bc)
                                {
                                    bc.CompleteAdding();
                                    po.CancellationToken.ThrowIfCancellationRequested();
                                }
                            }

                            if (success)
                            {
                                if (showOutput)
                                {
                                    Console.WriteLine(" Add:{0}", new String(itemToAdd));
                                }
                            }
                            else
                            {
                                Console.WriteLine(" AddBlocked:{0} Count = {1}", new String(itemToAdd), bc.Count);
                                // Don't increment nextItem. Try again on next iteration.

                                //Do something else useful instead.
                                //UpdateProgress(itemToAdd);
                            }

                        } while (success == false);

                        
                    });

                lock (lockObj)
                {
                    finishedProducers += 1;
                    if(finishedProducers >= maxLength)
                    {
                        bc.CompleteAdding();
                    }
                }

                Console.WriteLine("Finished Producer for Length {0}",length);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Aborted Produces for Length {0}",length);
            }       
        }


        //https://blogs.msdn.microsoft.com/ericlippert/2010/06/28/computing-a-cartesian-product-with-linq/
        public static IEnumerable<IEnumerable<T>> CartesianProduct<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            IEnumerable<IEnumerable<T>> emptyProduct = new[] { Enumerable.Empty<T>() };
            return sequences.Aggregate(
              emptyProduct,
              (accumulator, sequence) =>
                from accseq in accumulator
                from item in sequence
                select accseq.Concat(new[] { item }));
        }
    }
}