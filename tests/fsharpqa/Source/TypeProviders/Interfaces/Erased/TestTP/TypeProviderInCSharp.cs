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
using Microsoft.FSharp.Collections;

[assembly: TypeProviderAssembly()]

namespace TypeProviderInCSharp
{
    class Namespace1 : IProvidedNamespace
    {
        const string _Namespace = "N";
        const string _Name = "T";
        const string _Name2 = "T2";
        const string _Name3 = "I1";
        const string _Name4 = "T4Tuple";

        Type myType;
        Type myType2;
        Type myType3_Interface;
        Type myTyp4_BigTuple;

        public Namespace1()
        {
            // Type myType = new myType(typeof(N.S), "Bad.Name", typeof(Action), true);
            myType = new ArtificialType(_Namespace, _Name, false, basetype: typeof(object), isValueType: false, isByRef: false, isEnum: false, isPointer: false, ElementType: null, isInterface: false, isAbstract: false, MReturnSynType: typeof(int));

            myType2 = new ArtificialType(_Namespace, _Name2, false, basetype: typeof(object), isValueType: false, isByRef: false, isEnum: false, isPointer: false, ElementType: typeof(int), isInterface: false, isAbstract: false, MReturnSynType: typeof(int));

            myType3_Interface = new ArtificialType(_Namespace, _Name3, false, basetype: null, isValueType: false, isByRef: false, isEnum: false, isPointer: false, ElementType: null, isInterface: true, isAbstract: true, MReturnSynType: typeof(int));

            myTyp4_BigTuple = new ArtificialType(_Namespace, _Name4, false, basetype: typeof(System.Tuple<int, int, int, int, int, int, int, System.Tuple<int, int, int>>), isValueType: false, isByRef: false, isEnum: false, isPointer: false, ElementType: null, isInterface: false, isAbstract: false, MReturnSynType: myType3_Interface );

        }

        public IProvidedNamespace[] GetNestedNamespaces()
        {
            Helpers.TraceCall();
            return new IProvidedNamespace[] { };
        }

        public Type[] GetTypes()
        {
            Helpers.TraceCall();
            return new Type[] { myType, myType2, myType3_Interface, myTyp4_BigTuple };
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
            if (typeName == _Name2)
            {
                return myType2;
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
                var am = syntheticMethodBase as ArtificialMethodInfo;
                if (am.DeclaringType.FullName == "N.T" && am.Name == "M")
                {
                    var plist = ListModule.OfArray(new[] { FSharpExpr.Value(1), FSharpExpr.Value(2), FSharpExpr.Value(3) });
                    return FSharpExpr.NewArray(typeof(int), plist);
                }
                else if (am.DeclaringType.FullName == "N.T" && am.Name == "get_StaticProp1")
                {
                    return FSharpExpr.Lambda(FSharpVar.Global("", typeof(decimal)), FSharpExpr.Value<decimal>(4.2M));
                }
                else if (am.DeclaringType.FullName == "N.T" && am.Name == "get_StaticProp2")
                {
                    return FSharpExpr.Lambda(FSharpVar.Global("", typeof(System.Tuple<int, int, int, int, int, int, int, System.Tuple<int, int, int>>)), FSharpExpr.Value<System.Tuple<int, int, int, int, int, int, int, System.Tuple<int, int, int>>>(new System.Tuple<int, int, int, int, int, int, int, System.Tuple<int, int, int>>(1, 2, 3, 4, 5, 6, 7, new Tuple<int, int, int>(8, 9, 10))));
                }
                else if (am.DeclaringType.FullName == "N.T" && am.Name == "get_StaticProp3")
                {
                    return FSharpExpr.Lambda(FSharpVar.Global("", typeof(System.Tuple<int, int, int>)), FSharpExpr.Value<System.Tuple<int, int, int>>(new Tuple<int, int, int>(1, 2, 3)));
                }
                else if (am.DeclaringType.FullName == "N.T" && am.Name == "RaiseEvent1")
                {
                    Debug.Assert(false, "NYI");
                    throw new NotImplementedException();
                }
                else if (am.DeclaringType.FullName == "N.T" && am.Name == "M1")
                {
                    var plist = ListModule.OfArray(new[] { FSharpExpr.Value(1), FSharpExpr.Value(2), FSharpExpr.Value(3) });
                    return FSharpExpr.NewArray(typeof(int), plist);
                }
                else if (am.DeclaringType.FullName == "N.I1" && am.Name == "M")
                {
                    var plist = ListModule.OfArray(new[] { FSharpExpr.Value(1), FSharpExpr.Value(2), FSharpExpr.Value(3) });
                    return FSharpExpr.NewArray(typeof(int), plist);
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
