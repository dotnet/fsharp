using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FSharp.Compiler.Service.Tests
{
    /// <summary>
    /// Documentation
    /// </summary>
    public interface ICSharpInterface
    {
        int InterfaceMethod(string parameter);
        bool InterfaceProperty { get; }

        event EventHandler InterfaceEvent;
    }

    public interface ICSharpExplicitInterface
    {
        int ExplicitMethod(string parameter);
        bool ExplicitProperty { get; }

        event EventHandler ExplicitEvent;
    }

    public class CSharpClass : ICSharpInterface, ICSharpExplicitInterface
    {
        /// <summary>
        /// Documentaton
        /// </summary>
        /// <param name="param"></param>
        public CSharpClass(int param)
        {
            
        }

        /// <summary>
        /// Documentaton
        /// </summary>
        /// <param name="first"></param>
        /// <param name="param"></param>
        public CSharpClass(int first, string param)
        {

        }

        /// <summary>Some docs for Method</summary>
        public int Method(string parameter)
        {
            throw new NotImplementedException();
        }

        /// <summary>Some docs for Method2</summary>
        public int Method2(string optParameter = "empty")
        {
            throw new NotImplementedException();
        }

        /// <summary>Some docs for Method3</summary>
        public int Method3(params string[] variadicParameter)
        {
            throw new NotImplementedException();
        }

        /// <summary>Some docs for MethodWithOutref</summary>
        public int MethodWithOutref(out int outParameter)
        {
            throw new NotImplementedException();
        }

        /// <summary>Some docs for MethodWithInref</summary>
        public int MethodWithInref(in int outParameter)
        {
            throw new NotImplementedException();
        }

        public void GenericMethod<T>(T input)
        {
            throw new NotImplementedException();
        }

        public void GenericMethod2<T>(T input) where T : class
        {
            throw new NotImplementedException();
        }

        public void GenericMethod3<T>(T input) where T : IComparable<T>
        {
            throw new NotImplementedException();
        }

        public bool Property
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler Event;

        public int InterfaceMethod(string parameter)
        {
            throw new NotImplementedException();
        }

        public bool InterfaceProperty
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler InterfaceEvent;

        int ICSharpExplicitInterface.ExplicitMethod(string parameter)
        {
            throw new NotImplementedException();
        }

        bool ICSharpExplicitInterface.ExplicitProperty
        {
            get { throw new NotImplementedException(); }
        }

        event EventHandler ICSharpExplicitInterface.ExplicitEvent
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }
    }

    public class CSharpOuterClass
    {
        public enum InnerEnum { Case1 }

        public class InnerClass
        {
            public static int StaticMember()
            {
                return 0;
            }
        }
    }
    public class CSharpGenericOuterClass<T> : CSharpOuterClass
    {
        public T InnerProperty { get; }
    }

    public class String
    {
    }

    namespace Linq
    {
        public class DummyClass
        {
        }
    }
}
