using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighRankArrayTests
{
    public sealed class ClassWithArrayCtor<T>
    {
        private readonly Array m_array;

        public ClassWithArrayCtor(T[] array)
        {
            m_array = array;
        }

        public ClassWithArrayCtor(T[,] array)
        {
            m_array = array;
        }

        public ClassWithArrayCtor(T[, ,] array)
        {
            m_array = array;
        }

        public ClassWithArrayCtor(T[, , ,] array)
        {
            m_array = array;
        }

        public ClassWithArrayCtor(T[, , , ,] array)
        {
            m_array = array;
        }

        public ClassWithArrayCtor(T[, , , , ,] array)
        {
            m_array = array;
        }

        public ClassWithArrayCtor(T[, , , , , ,] array)
        {
            m_array = array;
        }

        public ClassWithArrayCtor(T[, , , , , , ,] array)
        {
            m_array = array;
        }

        public ClassWithArrayCtor(T[, , , , , , , ,] array)
        {
            m_array = array;
        }

        public ClassWithArrayCtor(T[, , , , , , , , ,] array)
        {
            m_array = array;
        }

        public ClassWithArrayCtor(T[, , , , , , , , , ,] array)
        {
            m_array = array;
        }
    }

    public static class StaticClassWithGenericMethod
    {
        public static void Example<T>(T[] array)
        {
            Console.WriteLine(array);
        }

        public static void Example<T>(T[,] array)
        {
            Console.WriteLine(array);
        }

        public static void Example<T>(T[, ,] array)
        {
            Console.WriteLine(array);
        }

        public static void Example<T>(T[, , ,] array)
        {
            Console.WriteLine(array);
        }

        public static void Example<T>(T[, , , ,] array)
        {
            Console.WriteLine(array);
        }

        public static void Example<T>(T[, , , , ,] array)
        {
            Console.WriteLine(array);
        }

        public static void Example<T>(T[, , , , , ,] array)
        {
            Console.WriteLine(array);
        }

        public static void Example<T>(T[, , , , , , ,] array)
        {
            Console.WriteLine(array);
        }

        public static void Example<T>(T[, , , , , , , ,] array)
        {
            Console.WriteLine(array);
        }

        public static void Example<T>(T[, , , , , , , , ,] array)
        {
            Console.WriteLine(array);
        }

        public static void Example<T>(T[, , , , , , , , , ,] array)
        {
            Console.WriteLine(array);
        }
    }

    public static class GenericStaticClassWithMethod<T>
    {
        public static void Example(T[] array)
        {
            Console.WriteLine(array);
        }

        public static void Example(T[,] array)
        {
            Console.WriteLine(array);
        }

        public static void Example(T[, ,] array)
        {
            Console.WriteLine(array);
        }

        public static void Example(T[, , ,] array)
        {
            Console.WriteLine(array);
        }

        public static void Example(T[, , , ,] array)
        {
            Console.WriteLine(array);
        }

        public static void Example(T[, , , , ,] array)
        {
            Console.WriteLine(array);
        }

        public static void Example(T[, , , , , ,] array)
        {
            Console.WriteLine(array);
        }

        public static void Example(T[, , , , , , ,] array)
        {
            Console.WriteLine(array);
        }

        public static void Example(T[, , , , , , , ,] array)
        {
            Console.WriteLine(array);
        }

        public static void Example(T[, , , , , , , , ,] array)
        {
            Console.WriteLine(array);
        }

        public static void Example(T[, , , , , , , , , ,] array)
        {
            Console.WriteLine(array);
        }
    }

    public static class MethodsReturningHighDArrays
    {
        public static T1[,] Example1<T1>()
        {
            return new T1[1, 1];
        }

        public static T1[, , , , ,] Example1<T1>(int x)
        {
            return new T1[1, 1, 1, 1, 1, 1];
        }

        public static T1[,] Example2<T1>()
        {
            return new T1[1, 1];
        }

        public static T1[, , , , , ,] Example2<T1, T2>()
        {
            return new T1[1, 1, 1, 1, 1, 1, 1];
        }
    }
}
