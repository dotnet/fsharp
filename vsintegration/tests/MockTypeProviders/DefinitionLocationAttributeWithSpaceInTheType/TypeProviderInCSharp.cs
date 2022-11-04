﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.FSharp.Core.CompilerServices;
using Microsoft.FSharp.Quotations;
using System.Reflection;
using System.Diagnostics;

[assembly: TypeProviderAssembly()]

namespace TypeProviderInCSharp
{
    //namespace N
    //{
    //    class S
    //    {
    //        public int instanceField;
    //        public S(int x)
    //        {
    //            instanceField = x;
    //        }
    //    }
    //}

    class Namespace1 : IProvidedNamespace
    {
        const string _Namespace = "N";
        const string _Name = "T T";

        // Type myType = new myType(typeof(N.S), "Bad.Name", typeof(Action), true);
        Type myType = new ArtificialType(_Namespace, _Name, false, basetype: typeof(object), isValueType: false, isByRef: false, isEnum: false, IsPointer: false);

        public IProvidedNamespace[] GetNestedNamespaces()
        {
            
            return new IProvidedNamespace[] { };
        }

        public Type[] GetTypes()
        {
            
            return new Type[] { myType };
        }

        public Type ResolveTypeName(string typeName)
        {
            
            if (typeName == _Name)
            {
                return myType;
            }
            return null;
        }

        string IProvidedNamespace.get_NamespaceName()
        {
            throw new NotImplementedException();
        }
    }

    [TypeProvider()]
    public class TypeProvider : ITypeProvider
    {
        void IDisposable.Dispose()
        {
        }
        private void Param(int staticParam)
        {
        }

        public Type ApplyStaticArguments(Type typeWithoutArguments, string[] typeNameWithArguments, object[] staticArguments)
        {
            //Console.WriteLine("Hello from ApplyStaticArguments");
            //var n = new myType(typeof(N.S), "S,\"1\"", typeof(object), false);
            //return n;
            
            return null;
        }

        public FSharpExpr GetInvokerExpression(System.Reflection.MethodBase syntheticMethodBase, FSharpExpr[] parameters)
        {
            
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
                    return FSharpExpr.Lambda(new FSharpVar("", typeof(int[]), null), FSharpExpr.Value<int[]>(new[] { 1, 2, 3 }));
                }
                else if (am.DeclaringType.FullName == "N.T" && am.Name == "get_StaticProp")
                {
                    // Expression<Func<decimal[]>> e = () => K().Select(x=>(decimal)x).ToArray();
                    return FSharpExpr.Lambda(new FSharpVar("", typeof(decimal), null), FSharpExpr.Value<decimal>(4.2M));
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
            
            return new IProvidedNamespace[] { new Namespace1() };
        }

        public System.Reflection.ParameterInfo[] GetStaticParameters(Type typeWithoutArguments)
        {
            
            // No StaticParams
            return new ParameterInfo[] { /* new myParameterInfo() */ };
        }

        public event EventHandler Invalidate;
        public byte[] GetGeneratedAssemblyContents(Assembly assembly) { throw (new Exception("GetGeneratedAssemblyContents - only erased types were provided!!")); }
    }
}
