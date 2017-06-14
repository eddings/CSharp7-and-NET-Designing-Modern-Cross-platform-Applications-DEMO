﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace C9
{
    class Program
    {
        
        static void Main(string[] args)
        {
            int startEven = -2;
            int startOdd = -1;
            int increment = 2;
            //To Generate first 1000 even numbers
            IEnumerable<int> evenSequences = Program4.GenerateSequences<int>(1000,
            () => startEven += increment);
            //To Generate first 1000 odd numbers
            IEnumerable<int> oddSequences = Program4.GenerateSequences<int>(1000,
            () => startOdd += increment);
            //Regular Recursion
            Func<int, int> factorial = (n) =>
                {
                    Func<int, int> fIterator = null;//Work-around for "use of unassigned variable" error!
                    fIterator = (m) => (m < 2) ? 1 : m * fIterator(m - 1);
                    return fIterator(n);
                };
            //Tail Recursion
            Func<int, int> factorial1 = (n) =>
                {
                    Func<int, int, int> fIterator = null;
                    fIterator = (product, i) => (i < 2) ? product : fIterator(product * i, i - 1);
                    return fIterator(1, n);
                };
            //Tail Recursion with Trampolining
            Func<int, int> factorial3 = (n) =>
                {
                    Func<int, int, int> trampoline = null;
                    Func<int, int, int> fIterator = (product, i) => (i < 2) ? product : trampoline(product * i, i - 1);
                    trampoline = (product, i) => fIterator(product * i, i - 1);
                    return trampoline(1, n);
                };


            var add10 = Program4.sum(10);//Returns a function that has 10 captured as a closure
            Console.WriteLine(add10(90));

            //var curriedSum = Program4.Curry<int, int, int>(Program4.AddOperation);
            Func<int, int, int> op = Program4.AddOperation;
            var curriedSum = op.Curry<int, int, int>();

            Console.WriteLine("Fact: {0}", factorial(6));//Prints Sum: 100
            Console.WriteLine("Sum: {0}", curriedSum(100)(90));//Prints Sum: 100
            Func<string, string, string> op2 = Program4.ConcatOperation;
            var curriedConcat = op2.Curry<string, string, string>();
            Func<string, string, string, string, string> op3 = Program4.ConcatOperation;
            var partiallyAppliedConcat = op3.PartialApply<string, string, string, string, string>("Partial ");
            Console.WriteLine("Concatenation: {0}", partiallyAppliedConcat("Function ", "Application ", "Rocks!!!"));//Prints Concatenation: Partial Function Application Rocks!!!
            ContinuationClient();
            SpellCheckerClient();
            GenerateSubsetClient();
            Console.WriteLine("PythagoreanTriples...");
            var watch = Stopwatch.StartNew();
            var count = (from triplets in PythagoreanTriples(50)
                         select triplets).Count();
            Console.WriteLine("No of PythagoreanTriples Within 50: {0} determined in {1} seconds", count, watch.ElapsedMilliseconds / 1000D);
            watch = Stopwatch.StartNew();
            count = (from triplets in PythagoreanTriplesCurried(50)
                         select triplets).Count();
            Console.WriteLine("PythagoreanTriples Within 50 Using Curried Functions: {0} determined in {1} seconds", count, watch.ElapsedMilliseconds / 1000D);
            watch = Stopwatch.StartNew();
            count = (from triplets in PythagoreanTriplesPartiallyApplied(50)
                     select triplets).Count();
            Console.WriteLine("PythagoreanTriples Within 50 Using Partially Applied Functions: {0} determined in {1} seconds", count, watch.ElapsedMilliseconds / 1000D);
            
Console.WriteLine("PythagoreanTriples within 50....");
foreach (var triplets in PythagoreanTriplesPartiallyApplied(50))
{
    Console.WriteLine(string.Join(",", triplets.ToArray()));
}
Console.ReadLine();            
            //Console.WriteLine("PythagoreanTriples Within 50 Using Partially Applied Functions: determined in {0} seconds",watch.ElapsedMilliseconds / 1000D);

        }
        static void Foo(int n)
        {
            n++;
            var c = CallCC<int>(a => a++, n);
            Console.WriteLine("Continuation Result: {0}", c.Result);
        }
        static void ContinuationClient()
        {
            Foo(4);
        }
        static async Task<T> CallCC<T>(Func<T, T> Return, T n)
        {
            // The following line simulates a task-returning asynchronous process.
            await Task.Delay(1000);
            return Return(n);
        }
static void SpellCheckerClient()
{
    var corrections = NorvigSpellChecker("somthing", 10);
    foreach (var correction in corrections)
    {
        Console.WriteLine(correction);
    }
    Console.WriteLine("Training Model Creation Complete! {0}", corrections.Count());
}

static void GenerateSubsetClient()
{
    Console.WriteLine("All possible subsets:");
    var watch = Stopwatch.StartNew();
    //decimal checkVal = new decimal(0.5);
    //var i = 0;
    //new List<int> 3,9,8,4,5,7,10 { 2, 3, 4, 5, 6, 7, 9, 10, 12, 15, 20, 28, 30, 35, 36, 45 } Enumerable.Range(2, 35)
    var resultSet = from subset in Program4.BigSubsetsP<int>(new List<int>() { 3, 1, 1, 2, 2, 1 }).AsParallel()
                    //let setSum = subset.Sum()
                    //where decimal.Equals(setSum, checkVal)
                    where subset.Sum() == 5
                    //where subset.Sum(no => (double)1 / (no * no)).ToString("G", CultureInfo.InvariantCulture) == "0.5"
                    select subset;

    foreach (var set in resultSet)
    {
        Console.WriteLine("SET >> {0} :: SUM >> {1}", string.Join(",", set), set.Sum());

    }
    /*foreach (char[] subset in Program4.Subsets<char>("abcdefghijklmnopqrstuvwxyz"))
    {
        Console.WriteLine(new String(subset));
        //i += 1;
    }*/
    Console.WriteLine("Elapsed Time for Subset Generation : {0} seconds", watch.ElapsedMilliseconds / 1000D);
}

        public static void NorvigSpellCheckerParallel(string word)
        {
            var alphabets = @"abcdefghijklmnopqrstuvwxyz";
            var WORDS = new ConcurrentDictionary<string, int>();
            var trainingFile = @"D:\Packt\Code\NSC_Training_Model.txt";
            var P = 0;
            //Stage #1
            var Train = Task.Factory.StartNew(() =>
            {
                foreach (var line in File.ReadLines(trainingFile)
                    .AsParallel())
                {
                    foreach (Match match in Regex.Matches(line, @"([a-z]+)", RegexOptions.IgnoreCase)
                        .AsParallel())
                    {
                        WORDS.AddOrUpdate(match.Value, 0, (k, v) => v + 1);
                    }
                }
            });
            var Probability = Task.Factory.StartNew(() =>
            {
                var N = (from x in WORDS.AsParallel()
                         select x.Value).Sum();
                int f = 0;
                if (WORDS.TryGetValue(word, out f))
                {
                    P = f / N;//Probability of Word
                }
            });
            var edits0 = Task.Factory.StartNew(() =>
            {
                return from i in Enumerable.Range(0, word.Length).AsParallel()
                       select new { a = word.Substring(0, i), b = word.Substring(i) };
            });
            Func<string,Task<System.Linq.ParallelQuery<string>>> edits1 = (tWord) => Task.Factory.StartNew(() =>
            {
                return from i in Enumerable.Range(0, tWord.Length).AsParallel()
                       select new { a = tWord.Substring(0, i), b = tWord.Substring(i) };
            })
            .ContinueWith(ant =>
            {
                return (from s in ant.Result
                        where s.b != ""
                        select s.a + s.b.Substring(1))//deletes
                        .Union(from s in ant.Result
                        where s.b.Length > 1
                        select s.a + s.b[1] + s.b[0] + s.b.Substring(2))//transposes
                        .Union(from s in ant.Result
                        from c in alphabets
                        where s.b != ""
                        select s.a + c + s.b.Substring(1))//replaces
                        .Union(from s in ant.Result
                        from c in alphabets
                        select s.a + c + s.b);//inserts
            });
            Func<string, Task<System.Linq.ParallelQuery<string>>> edits2 = (tWord) => Task.Factory.StartNew(() =>
            {
                return (from e1 in edits1(tWord).Result
                        from e2 in edits1(e1).Result
                        where WORDS.ContainsKey(e2)
                        select e2);
            });
            Func<IEnumerable<string>, Task<System.Linq.ParallelQuery<string>>> known = (tWords) => Task.Factory.StartNew(() =>
            {
                return (from e1 in tWords.AsParallel()
                        where WORDS.ContainsKey(e1)
                        select e1);
            });
            Func<string, Task<System.Linq.ParallelQuery<string>>> candidates = (tWord) => Task.Factory.StartNew(() =>
            {
                List<string> tWords = new List<string>();
                tWords.Add(word);
                return ((from e1 in known(tWords).Result
                            select e1)
                        .Union(from e2 in known(edits1(tWord).Result).Result
                                   select e2)
                        .Union(from e3 in known(edits2(tWord).Result).Result
                                   select e3)
                        .Union(from e4 in tWords.AsParallel()
                                   select e4))
                                   .Distinct();
            });
            Func<string, Task<System.Linq.ParallelQuery<string>>> corrections = (tWord) => Task.Factory.StartNew(() =>
            {
                var N = (from x in WORDS.AsParallel()
                         select x.Value).Sum();
                List<string> tWords = new List<string>();
                return (from e1 in candidates(tWord).Result
                        .OrderByDescending(e1 => WORDS.ContainsKey(e1) ? (float)WORDS[e1]/(float)N : 0)
                        select e1).Take(3);
            });
            Task.WaitAll(Train);
            Task.WaitAll(Probability);
            var Tedits1 = corrections(word);
            foreach (var edit1 in Tedits1.Result)
            {
                Console.WriteLine(edit1);
            }
            Console.WriteLine("Training Model Creation Complete! {0}", Tedits1.Result.Count());
        }


/// Implementation of a Spell Checker based on Peter Norvig's Post
/// http://norvig.com/spell-correct.html
/// </summary>
/// <param name="word">Word whose spelling is checked</param>
/// <param name="count">No. of Corrections Needed</param>
/// <returns>List of requested corrections, based on count</returns>
public static IEnumerable<string> NorvigSpellChecker(string word, int count)
{
    var alphabets = @"abcdefghijklmnopqrstuvwxyz";
    var WORDS = new ConcurrentDictionary<string, int>();
    var trainingFile = @"D:\Packt\Code\NSC_Training_Model.txt";
    //Training Model Creation
    var Train = Task.Factory.StartNew(() =>
    {
        foreach (var line in File.ReadLines(trainingFile)
            .AsParallel())
        {
            foreach (Match match in Regex.Matches(line, @"([a-z]+)", RegexOptions.IgnoreCase)
                .AsParallel())
            {
                WORDS.AddOrUpdate(match.Value, 0, (k, v) => v + 1);
            }
        }
    });
    //All edits that are 1 edit away from word
    Func<string, Task<IEnumerable<string>>> edits1 = (tWord) => Task.Factory.StartNew(() =>
    {
        return from i in Enumerable.Range(0, tWord.Length)
                select new { part1 = tWord.Substring(0, i), part2 = tWord.Substring(i) };//splits
    })
    .ContinueWith(ant =>
    {
        return (from splits in ant.Result
                where splits.part2 != ""
                select splits.part1 + splits.part2.Substring(1))//deletes
                .Union(from splits in ant.Result
                        where splits.part2.Length > 1
                        select splits.part1 + splits.part2[1] + splits.part2[0] + splits.part2.Substring(2))//transposes
                .Union(from splits in ant.Result
                        from c in alphabets
                        where splits.part2 != ""
                        select splits.part1 + c + splits.part2.Substring(1))//replaces
                .Union(from splits in ant.Result
                        from c in alphabets
                        select splits.part1 + c + splits.part2);//inserts
    });
    //All edits that are 2 edits away from word
    Func<string, Task<IEnumerable<string>>> edits2 = (tWord) => Task.Factory.StartNew(() =>
    {
        return (from e1 in edits1(tWord).Result
                from e2 in edits1(e1).Result
                where WORDS.ContainsKey(e2)
                select e2);
    });
    //Returns the subset of words that appear in the dictionary of WORDS
    Func<IEnumerable<string>, Task<IEnumerable<string>>> known = (tWords) => Task.Factory.StartNew(() =>
    {
        return (from e1 in tWords
                where WORDS.ContainsKey(e1)
                select e1);
    });
    //Generate all possible spelling corrections for word
    Func<string, Task<IEnumerable<string>>> candidates = (tWord) => Task.Factory.StartNew(() =>
    {
        List<string> tWords = new List<string>();
        tWords.Add(word);
        return ((from e1 in known(tWords).Result
                    select e1)
                .Union(from e2 in known(edits1(tWord).Result).Result
                        select e2)
                .Union(from e3 in known(edits2(tWord).Result).Result
                        select e3)
                .Union(from e4 in tWords
                        select e4))
                            .Distinct();
    });
    //Returns most probable spelling correction for word in the order of
    //their probability of occurrance in the corpus
    Func<string, Task<IEnumerable<string>>> corrections = (tWord) => Task.Factory.StartNew(() =>
    {
        var N = (from x in WORDS
                    select x.Value).Sum();
        List<string> tWords = new List<string>();
        return (from e1 in candidates(tWord).Result
                .OrderByDescending(e1 => WORDS.ContainsKey(e1) ? (float)WORDS[e1] / (float)N : 0)
                select e1).Take(count);
    });
    Task.WaitAll(Train);//Ensures Training Model is Created!
    return corrections(word).Result;
}
/// <summary>
/// Finds the Pythagorean Triples within a given range conventionally
/// </summary>
/// <param name="range">Input Range</param>
/// <returns>Triplet</returns>
public static IEnumerable<IEnumerable<int>> PythagoreanTriples(int range)
{
    HashSet<string> capturedTriplets = new HashSet<string>();
    for (int a = 1; a< range; a++)
    {
        for (int b = 1; b< range; b++)
        {
            for (int c = 1; c< range; c++)
            {
                if ((a * a + b * b) == c * c)
                {
                    string keyPart1 = a.ToString();
                    string keyPart2 = b.ToString();
                    //This check filters the duplicate triplets
                    if (!capturedTriplets.Contains(keyPart1 + ":" + keyPart2) && !capturedTriplets.Contains(keyPart2 + ":" + keyPart1))
                    {
                        capturedTriplets.Add(keyPart1 + ":" + keyPart2);
                        yield return new List<int>() { a, b, c };
                    }
                }
            }
        }
    }
}
/// <summary>
/// Finds the Pythagorean Triples within a given range employing Currying
/// </summary>
/// <param name="range">Input Range</param>
/// <returns>Triplet</returns>
public static IEnumerable<IEnumerable<int>> PythagoreanTriplesCurried(int range)
{
    Func<int, Func<int, Func<int, bool>>> formula = x => y => z => (x * x + y * y) == (z * z);//Curried
    HashSet<string> capturedTriplets = new HashSet<string>();
    for (int a = 1; a < range; a++)
    {
        var fx = formula(a);//First Curry
        for (int b = 1; b < range; b++)
        {
            var fy = fx(b);//Second Curry
            for (int c = 1; c < range; c++)
            {
                if (fy(c))//Final Evaluation
                {
                    string keyPart1 = a.ToString();
                    string keyPart2 = b.ToString();
                    //This check filters the duplicate triplets
                    if (!capturedTriplets.Contains(keyPart1 + ":" + keyPart2) && !capturedTriplets.Contains(keyPart2 + ":" + keyPart1))
                    {
                        capturedTriplets.Add(keyPart1 + ":" + keyPart2);
                        yield return new List<int>() { a, b, c };
                    }
                }
            }
        }
    }
}
/// <summary>
/// Finds the Pythagorean Triples within a given range employing Partial Application
/// </summary>
/// <param name="range">Input Range</param>
/// <returns>Triplet</returns>
public static IEnumerable<IEnumerable<int>> PythagoreanTriplesPartiallyApplied(int range)
{
    Func<int, Func<int, Func<int, bool>>> formula = x =>
        {
            var fxVal = x * x;
            return y =>
                {
                    var fyVal = fxVal + y * y;//fxVal: when x is partially applied is a closure here
                    return z => fyVal == (z * z);//fyVal: when y is partially applied is a closure here
                };
        };
    HashSet<string> capturedTriplets = new HashSet<string>();
    for (int a = 1; a < range; a++)
    {
        var fx = formula(a);//Partial Application 1
        for (int b = 1; b < range; b++)
        {
            var fy = fx(b);//Partial Application 2
            for (int c = 1; c < range; c++)
            {
                if (fy(c))//Final Evaluation
                {
                    string keyPart1 = a.ToString();
                    string keyPart2 = b.ToString();
                    //This check filters the duplicate triplets
                    if (!capturedTriplets.Contains(keyPart1 + ":" + keyPart2) && !capturedTriplets.Contains(keyPart2 + ":" + keyPart1))
                    {
                        capturedTriplets.Add(keyPart1 + ":" + keyPart2);
                        yield return new List<int>() { a, b, c };
                    }
                }
            }
        }
    }
}
        public async Task GetBase()
        {
            await Task.Delay(1000);
        }
        /*public static IEnumerable<TSource> DistinctBy<TSource, TKey>
            (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }*/
    }
}
