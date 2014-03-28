using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.FSharp.Core.CompilerServices;
using System.Collections.Generic;
using FSharp.TypeMagic;

[assembly: TypeProviderAssembly]
namespace Provider
{

    [TypeProvider]
    public class Provider : ITypeProvider, IProvidedNamespace
    {
        void IDisposable.Dispose()
        {
        }
        Type helloWorldType;
        public Provider()
        {
            System.Console.WriteLine("Creating provider...");
            var thisAssembly = typeof(Provider).Assembly;
            var modul = thisAssembly.GetModules()[0];
            var rootNamespace = "FSharp.HelloWorld";
            var container = TypeContainer.NewNamespace(modul, rootNamespace);
            helloWorldType = TypeBuilder.CreateSimpleType(container, "HelloWorldType", null, null, null);

        }


        string IProvidedNamespace.NamespaceName { get { return "FSharp.HelloWorld"; } }

        Type[] IProvidedNamespace.GetTypes() { return new Type[] { helloWorldType }; }

        Type IProvidedNamespace.ResolveTypeName(string typeBaseName) { if (typeBaseName == "HelloWorldType") return helloWorldType; else return null; }

        IProvidedNamespace[] ITypeProvider.GetNamespaces() { return new IProvidedNamespace[] { this }; }

        IProvidedNamespace[] IProvidedNamespace.GetNestedNamespaces() { return new IProvidedNamespace[] { }; }

        ParameterInfo[] ITypeProvider.GetStaticParameters(Type typeWithoutArguments) { return new ParameterInfo[] { }; }

        Type ITypeProvider.ApplyStaticArguments(Type typeWithoutArguments, string[] typeNameWithArguments, object[] staticArguments) { return typeWithoutArguments; }

        Microsoft.FSharp.Quotations.FSharpExpr ITypeProvider.GetInvokerExpression(MethodBase syntheticMethodBase, Microsoft.FSharp.Quotations.FSharpExpr[] parameterExpressions)
        {
            if (!syntheticMethodBase.Name.StartsWith("get_"))
            {
                throw (new Exception("expected syntheticMethodBase to be a property getter, with name starting with \"get_\""));
            }
            // trim off the "get_"
            var propertyName = syntheticMethodBase.Name.Substring(4);
            var syntheticMethodBase2 = syntheticMethodBase as MethodInfo;
            var getClassInstancesByName = (typeof(Provider)).GetMethod("GetPropertyByName", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(new Type[] { syntheticMethodBase2.ReturnType });
            return Microsoft.FSharp.Quotations.FSharpExpr.Call(getClassInstancesByName, Microsoft.FSharp.Collections.ListModule.OfArray (new Microsoft.FSharp.Quotations.FSharpExpr[] { Microsoft.FSharp.Quotations.FSharpExpr.Value(propertyName) }));
        }

        public event System.EventHandler Invalidate;
        public byte[] GetGeneratedAssemblyContents(Assembly assembly) { throw (new Exception("GetGeneratedAssemblyContents - only erased types were provided!!")); }

    }
}
