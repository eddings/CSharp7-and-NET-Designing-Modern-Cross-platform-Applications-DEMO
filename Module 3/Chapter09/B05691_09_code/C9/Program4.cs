﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Linq.Expressions;

namespace C9
{
    public static class Program4
    {

        public static IEnumerable<T> GenerateSequences<T>(int noOfElements, Func<T> generator)
        {
            for (int i = 0; i < noOfElements; i++)
            {
                yield return generator();
            }
        }

        public static IEnumerable<TSource> Where<TSource> (IEnumerable<TSource> source, Predicate<TSource> predicate)
        {
            foreach (TSource element in source)
            {
                if (predicate(element))
                    yield return element;
            }
        }

        public static Func<int, int> sum2(int x)
        {
            Func<int, int> partialSum = y => y + x;//Partial Application
            return partialSum;
            //Func<int, Func<int, int>> sum2 = z => y => z + y;
        }

public static Func<int, Func<int, int>> sum = x => y => x + y;

        public delegate int AnyFunction(int x, int y);
public static int AddOperation(int x, int y)
{
    return x + y;
}

public static string ConcatOperation(string x, string y)
{
    return x + y;
}

public static string ConcatOperation(string a, string b, string c, string d)
{
    return a + b + c + d;
}

public static void DemoLambda()
{
    Func<int, int, int> AddOperation1 = delegate(int x, int y) { return x + y; };
    Func<int, int, int> AddOperation2 = (x, y) => x + y;//Lambda Expression - 1
    Func<string, string, string> ConcatOperation = (x, y) => x + y;//Lambda Expression - 2
}

    

public static Func<T1, Func<T2, TResult>> Curry2<T1, T2, TResult>
    (Func<T1, T2, TResult> function)
{
    return a => b => function(a, b);
}

public static Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>
(this Func<T1, T2, TResult> function)
{
    return a => b => function(a, b);
}
//Supports 3 Input Parameters
public static Func<T1, Func<T2, Func<T3, TResult>>> Curry<T1, T2, T3, TResult>
(this Func<T1, T2, T3, TResult> function)
{
    return a => b => c => function(a, b, c);
}
//Supports 4 Input Parameters
public static Func<T1, Func<T2, Func<T3, Func<T4, TResult>>>> Curry<T1, T2, T3, T4, TResult>
(this Func<T1, T2, T3, T4, TResult> function)
{
    return a => b => c => d => function(a, b, c, d);
}
//Supports 3 Input Parameters
public static Func<T2, T3, TResult> PartialApply<T1, T2, T3, TResult>
(this Func<T1, T2, T3, TResult> function, T1 a)
{
    return (b , c) => function(a, b, c);
}
//Supports 4 Input Parameters
public static Func<T2, T3, T4, TResult> PartialApply<T1, T2, T3, T4, TResult>
(this Func<T1, T2, T3, T4, TResult> function, T1 a)
{
    return (b, c, d) => function(a, b, c, d);
}

public static T CallCC<T>(Func<T,T> f, T a)
{
    return f(a);
}

        public static T power<T>(T number, int exponent, Func<T, T, T> function)
        {
            int remainingEvenExp = exponent/2;
            int remainingOddExp = exponent%2;
            T square = function.Invoke(number, number);
            T result = square;
            if (remainingEvenExp != 1) {
                result = power<T>(square, remainingEvenExp, function);
            } else if (remainingOddExp != 0) {
                result = function.Invoke(result, number); ;
            }
            return result;
        }

        public static IEnumerable<IEnumerable<T>> Subsets<T>(IEnumerable<T> inputSet)
        {
            T[] _input = inputSet.ToArray<T>();
            int _bitcount = _input.Length;
            int _mask = Convert.ToInt32(Math.Pow(2.0d, _bitcount));
            int i = 0;
            while (i < _mask)
            {
                List<T> _output = new List<T>();
                int j = 0;
                while (j < _bitcount)
                {
                    if ((i & (1 << j)) > 0) { _output.Add(_input[j]); }
                    j++;
                }
                yield return _output.ToArray<T>();
                i++;
            }
        }
        public static IEnumerable<IEnumerable<T>> BigSubsets<T>(IEnumerable<T> inputSet)
        {
            T[] _input = inputSet.ToArray<T>();
            int _bitcount = _input.Length;
            BitArray _bits = new BitArray(_bitcount);
            BitArray _bitsIncrement = new BitArray(_bitcount);
            _bitsIncrement.Set(0, true);
            int _mask = Convert.ToInt32(Math.Pow(2.0d, _bitcount));
            int i = 0;
            Console.WriteLine("All possible subsets:");
            while (i < _mask)
            {
                List<T> _output = new List<T>();
                int j = 0;
                bool a;
                bool b;
                bool cIN = false;
                bool cOUT = false;
                bool bSUM = false;
                while (j < _bitcount)
                {
                    a = _bits[j];
                    b = _bitsIncrement[j];
                    bSUM = a ^ b ^ cOUT;
                    cOUT = (a & b) | (cIN & (a ^ b));
                    cIN = cOUT;
                    _bits.Set(j, bSUM);
                    if (bSUM) { _output.Add(_input[j]); }
                    j++;
                }
                yield return _output.ToArray<T>();
                i++;
            }
        }
public static IEnumerable<IEnumerable<T>> BigSubsetsP<T>(IEnumerable<T> inputSet)
{
    T[] _input = inputSet.ToArray<T>();
    BlockingCollection<IEnumerable<T>> output = new BlockingCollection<IEnumerable<T>>(boundedCapacity: 20);
    int _bitcount = _input.Length;
    BitArray bits = new BitArray(_bitcount);
    BitArray _bitsIncrement = new BitArray(_bitcount);
    _bitsIncrement.Set(0, true);
    //Stage#1 [GENERATE]
    var generate = Task.Factory.StartNew(() =>
    {
        try
        {
            Parallel.For(0, _bitcount,
                (chunkIndex) =>
                {
                    BitArray _bits = new BitArray(_bitcount);
                    bool overFlow = false;
                    _bits.Set(chunkIndex, true);
                    output.Add(new [] {_input[chunkIndex]});
                    while (!overFlow)
                    {
                        List<T> _output = new List<T>();
                        int j = 0;
                        bool a;
                        bool b;
                        bool cIN = false;
                        bool cOUT = false;
                        bool bSUM = false;
                        while (j <= chunkIndex)
                        {
                            a = _bits[j];
                            b = _bitsIncrement[j];
                            bSUM = a ^ b ^ cIN;
                            cOUT = (a & b) | (cIN & (a ^ b));
                            _bits.Set(j, bSUM);
                            if (bSUM) 
                            {
                                _output.Add(_input[j]);
                            }
                            cIN = cOUT;
                            j++;
                        }
                        overFlow = cIN;
                        if (_output.Count > 0)
                        {
                            output.Add(_output.ToArray<T>());
                        }
                        _output.Clear();
                        _output = null;
                    }
                    _bits = null;
                });
        }
        finally
        {
            output.CompleteAdding();
        }
    });
    //Stage#2 [CONCURRENT READ]
    foreach (var subset in output.GetConsumingEnumerable().AsParallel())
    {
        yield return subset;
    }
    generate.Wait();
}
    }

public class Counter
{
    private int i = 0;
    public int AddOneRO(int x) //Referentially Opaque
    {
        i+=1;
        return x + i;
    }
    public int AddOneRT(int x) //Referentially Transparent
    {
        return x + 1;
    }
}
}
