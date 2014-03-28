// A dummy type provider that does not do much other that
// declaring itself as "contributing to types in the global namespace"

using System;
using Microsoft.FSharp.Core.CompilerServices;
using System.Reflection;

[assembly: TypeProviderAssembly()]

namespace TypeProviderInCSharp
{
    class Namespace1 : IProvidedNamespace
    {
        public IProvidedNamespace[] GetNestedNamespaces()
        {
            return new IProvidedNamespace[] { };
        }

        public Type[] GetTypes()
        {
            return new Type[] { };
        }

        public string NamespaceName
        {
            // Returning 'null' means that our types live in the global namespace
            get { System.Console.WriteLine("From TP: returning null to mean global NS..."); return null; }
        }

        public Type ResolveTypeName(string typeName)
        {
            System.Console.WriteLine("From TP: typeName={0}", typeName);
            return null;  // we don't know any type
        }
    }


    [TypeProvider()]
    public class TypeProvider : ITypeProvider {
    
        public Type ApplyStaticArguments(Type typeWithoutArguments, string[] typeNameWithArguments, object[] staticArguments)
        {
            throw new NotImplementedException();
        }

        Microsoft.FSharp.Quotations.FSharpExpr ITypeProvider.GetInvokerExpression(MethodBase syntheticMethodBase, Microsoft.FSharp.Quotations.FSharpExpr[] parameterExpressions)
        {
            return null;
        }

        public IProvidedNamespace[] GetNamespaces()
        {
            return new IProvidedNamespace[] { new Namespace1() };
        }

        public System.Reflection.ParameterInfo[] GetStaticParameters(Type typeWithoutArguments)
        {
            return new ParameterInfo[] { };
        }

        public event EventHandler Invalidate;

        void System.IDisposable.Dispose() { }

        public byte[] GetGeneratedAssemblyContents(Assembly assembly) 
        {
            throw new Exception("GetGeneratedAssemblyContents - only erased types were provided!!");
        }
    }
}
