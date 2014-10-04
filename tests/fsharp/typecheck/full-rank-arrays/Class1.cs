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

        public ClassWithArrayCtor(T[,,] array)
        {
            m_array = array;
        }

        public ClassWithArrayCtor(T[,,,] array)
        {
            m_array = array;
        }

        public ClassWithArrayCtor(T[,,,,] array)
        {
            m_array = array;
        }

        public ClassWithArrayCtor(T[,,,,,] array)
        {
            m_array = array;
        }

        public ClassWithArrayCtor(T[,,,,,,] array)
        {
            m_array = array;
        }

        public ClassWithArrayCtor(T[,,,,,,,] array)
        {
            m_array = array;
        }

        public ClassWithArrayCtor(T[,,,,,,,,] array)
        {
            m_array = array;
        }

        public ClassWithArrayCtor(T[,,,,,,,,,] array)
        {
            m_array = array;
        }

        public ClassWithArrayCtor(T[,,,,,,,,,,] array)
        {
            m_array = array;
        }
    }

    public static class StaticClassWithGenericMethod
    {
        public static void Example<T>(T[] array)
        {
            throw new NotImplementedException();
        }

        public static void Example<T>(T[,] array)
        {
            throw new NotImplementedException();
        }

        public static void Example<T>(T[,,] array)
        {
            throw new NotImplementedException();
        }

        public static void Example<T>(T[,,,] array)
        {
            throw new NotImplementedException();
        }

        public static void Example<T>(T[,,,,] array)
        {
            throw new NotImplementedException();
        }

        public static void Example<T>(T[,,,,,] array)
        {
            throw new NotImplementedException();
        }

        public static void Example<T>(T[,,,,,,] array)
        {
            throw new NotImplementedException();
        }

        public static void Example<T>(T[,,,,,,,] array)
        {
            throw new NotImplementedException();
        }

        public static void Example<T>(T[,,,,,,,,] array)
        {
            throw new NotImplementedException();
        }

        public static void Example<T>(T[,,,,,,,,,] array)
        {
            throw new NotImplementedException();
        }

        public static void Example<T>(T[,,,,,,,,,,] array)
        {
            throw new NotImplementedException();
        }
    }

    public static class GenericStaticClassWithMethod<T>
    {
        public static void Example(T[] array)
        {
            throw new NotImplementedException();
        }

        public static void Example(T[,] array)
        {
            throw new NotImplementedException();
        }

        public static void Example(T[, ,] array)
        {
            throw new NotImplementedException();
        }

        public static void Example(T[, , ,] array)
        {
            throw new NotImplementedException();
        }

        public static void Example(T[, , , ,] array)
        {
            throw new NotImplementedException();
        }

        public static void Example(T[, , , , ,] array)
        {
            throw new NotImplementedException();
        }

        public static void Example(T[, , , , , ,] array)
        {
            throw new NotImplementedException();
        }

        public static void Example(T[, , , , , , ,] array)
        {
            throw new NotImplementedException();
        }

        public static void Example(T[, , , , , , , ,] array)
        {
            throw new NotImplementedException();
        }

        public static void Example(T[, , , , , , , , ,] array)
        {
            throw new NotImplementedException();
        }

        public static void Example(T[, , , , , , , , , ,] array)
        {
            throw new NotImplementedException();
        }
    }
    
   public static class MethodsReturningHighDArrays
   {
        public static T1[,] Example1<T1>()
        {
            throw new NotImplementedException();
        }
        
        public static T1[,,,,,] Example1<T1>(int x)
        {
            throw new NotImplementedException();
        }

        public static T1[,] Example2<T1>()
        {
            throw new NotImplementedException();
        }

        public static T1[,,,,,,] Example2<T1, T2>()
        {
            throw new NotImplementedException();
        }
    }
}
