using System;
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

        Type myType;

        public Namespace1()
        {
            myType = new ArtificialType(_Namespace, _Name, basetype: typeof(object));
        }

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
                Debug.Assert(false, "Why are we here?");
                throw new NotImplementedException();
            }
            else
            {
                Debug.Assert(false, "GetInvokerExpression() invoked with neither ConstructorInfo nor MethodInfo!");
                return null;
            }
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

        public void Dispose()
        {
            Helpers.TraceCall();
        }

        public byte[] GetGeneratedAssemblyContents(Assembly assembly) 
        {
            Helpers.TraceCall();
            Debug.Assert(false, "GetGeneratedAssemblyContents - only erased types were provided!!");
            throw new Exception("GetGeneratedAssemblyContents - only erased types were provided!!");
        }
    }
}
