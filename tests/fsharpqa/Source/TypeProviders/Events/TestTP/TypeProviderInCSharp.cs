using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Linq.Expressions;
using System.Diagnostics;
using Microsoft.FSharp.Core.CompilerServices;
using Microsoft.FSharp.Quotations;

[assembly: TypeProviderAssembly()]

namespace TypeProviderInCSharp
{
    class Namespace1 : IProvidedNamespace
    {
        const string _Namespace = "N";
        const string _Name = "T";

        // Type myType = new myType(typeof(N.S), "Bad.Name", typeof(Action), true);
        Type myType = new ArtificialType(_Namespace, _Name, false, basetype: typeof(object), isValueType: false, isByRef: false, isEnum: false, IsPointer: false);

        public IProvidedNamespace[] GetNestedNamespaces()
        {
            Helpers.TraceCall();
            return new IProvidedNamespace[] { };
        }

        public Type[] GetTypes()
        {
            Helpers.TraceCall();
            return new Type[] { myType };
        }

        public string NamespaceName
        {
            get { return _Namespace; }
        }

        public Type ResolveTypeName(string typeName)
        {
            Helpers.TraceCall();
            if (typeName == _Name)
            {
                return myType;
            }
            return null;
        }
    }

    [TypeProvider()]
    public class TypeProvider : ITypeProvider
    {
        private void Param(int staticParam)
        {
        }

        public Type ApplyStaticArguments(Type typeWithoutArguments, string[] typePathWithArguments, object[] staticArguments)
        {
            //Console.WriteLine("Hello from ApplyStaticArguments");
            //var n = new myType(typeof(N.S), "S,\"1\"", typeof(object), false);
            //return n;
            Helpers.TraceCall();
            return null;
        }

        public Microsoft.FSharp.Quotations.FSharpExpr GetInvokerExpression(MethodBase syntheticMethodBase, Microsoft.FSharp.Quotations.FSharpExpr[] parameters)
        {
            Helpers.TraceCall();
            if (syntheticMethodBase is System.Reflection.ConstructorInfo)
            {
                var ac = syntheticMethodBase as ArtificialConstructorInfo;
                if (ac.DeclaringType.FullName == "N.T")
                {
                    return FSharpExpr.DefaultValue(ac.DeclaringType.BaseType);
                }
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
            else if (syntheticMethodBase is System.Reflection.MethodInfo)
            {
                var am = syntheticMethodBase as ArtificialMethodInfo;
                if (am.DeclaringType.FullName == "N.T" && am.Name == "M")
                {
                    return FSharpExpr.Lambda(FSharpVar.Global("", typeof(int[])), FSharpExpr.Value<int[]>(new[] { 1, 2, 3 }));
                }
                else if (am.DeclaringType.FullName == "N.T" && am.Name == "get_StaticProp")
                {
                    return FSharpExpr.Lambda(FSharpVar.Global("", typeof(decimal)), FSharpExpr.Value<decimal>(4.2M));
                }
                else
                {
                    Debug.Assert(false, "NYI");
                    throw new NotImplementedException();
                }
            }
            else
            {
                Debug.Assert(false, "GetInvokerExpression() invoked with neither ConstructorInfo nor MethodInfo!");
                return null;
            }
            //Expression<Func<S>> e = () => new S(9);
            //return e.Body;
 
            //throw new NotImplementedException();
        }

        public IProvidedNamespace[] GetNamespaces()
        {
            Helpers.TraceCall();
            return new IProvidedNamespace[] { new Namespace1() };
        }

        public System.Reflection.ParameterInfo[] GetStaticParameters(Type typeWithoutArguments)
        {
            Helpers.TraceCall();
            // No StaticParams
            return new ParameterInfo[] { /* new myParameterInfo() */ };
        }

        public event EventHandler Invalidate;

        // Not much to dispose, really...
        public void Dispose()
        {
        }

        public byte[] GetGeneratedAssemblyContents(Assembly assembly) 
        {
            Helpers.TraceCall();
            Debug.Assert(false, "GetGeneratedAssemblyContents - only erased types were provided!!");
            throw new Exception("GetGeneratedAssemblyContents - only erased types were provided!!");
        }

    }
}
