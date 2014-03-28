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
        Type myType = new ArtificialType(_Namespace, _Name);

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
            Debug.Assert(false, "NYI");
            return null;
        }
    }

    public class myType : Type
    {
        string _Name;
        Type _UnderlyingSystemType;
        bool _IsArrayImpl;
        Type _t;

        public myType(Type t)
        {
            _t = t;
        }

        public myType(Type t, string name, Type UnderlyingSystemType, bool IsArrayImpl)
        {
            _Name = name;
            _UnderlyingSystemType = UnderlyingSystemType;
            _IsArrayImpl = IsArrayImpl;
            _t = t;
        }

        public override Assembly Assembly
        {
            get { Console.WriteLine("Call to Assembly"); return _t.Assembly; }
        }

        public override string AssemblyQualifiedName
        {
            get { Console.WriteLine("Call to AssemblyQualifiedName -> {0}", _t.AssemblyQualifiedName); return _t.AssemblyQualifiedName; }
        }

        public override Type BaseType
        {
            get {
                Console.WriteLine("Call to BaseType");
                return _t.BaseType; 
                
            }
        }

        public override string FullName
        {
            get {
                Console.WriteLine("Call to FullName -> {0}", "S");
                return "S"; 
            }
        }

        public override Guid GUID
        {
            get { return _t.GUID; }
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            // TODO: Understand this...
            Console.WriteLine("Call to GetAttributeFlagsImpl - does it matter what we return here?");
            return _t.Attributes;
        }

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            Debug.Assert(false, "NYA"); throw new NotImplementedException();
        }

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            Console.WriteLine("Call to GetConstructors: {0}", bindingAttr.ToString());
            return new ConstructorInfo[] { };
        }

        public override Type GetElementType()
        {
            Debug.Assert(false, "NYA"); throw new NotImplementedException();
        }

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            Console.WriteLine("Call to GetEvent: {0}, {1}", name, bindingAttr.ToString());
            return null;
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            Console.WriteLine("Call to GetEvents: {0}", bindingAttr.ToString());
            return new EventInfo[] {};
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            Debug.Assert(false, "NYA"); throw new NotImplementedException();
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            Console.WriteLine("Call to GetFields: {0}", bindingAttr.ToString());
            return new FieldInfo[] {};
        }

        public override Type GetInterface(string name, bool ignoreCase)
        {
            Debug.Assert(false, "NYA"); throw new NotImplementedException();
        }

        public override Type[] GetInterfaces()
        {
            Console.WriteLine("Call to GetInterfaces");
            return new Type[] {};
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            Console.WriteLine("Call to GetMembers: {0}", bindingAttr.ToString());
            return new MemberInfo[]{};
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            Debug.Assert(false, "NYA"); throw new NotImplementedException();
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            Console.WriteLine("Call to GetMethods: {0}", bindingAttr.ToString());
            return new MethodInfo[] {};
        }

        public override Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            Debug.Assert(false, "NYA"); throw new NotImplementedException();
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            Debug.Assert(false, "NYA"); throw new NotImplementedException();
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            Console.WriteLine("Call to GetProperties: {0}", bindingAttr.ToString());
            return new PropertyInfo[] { };
        }

        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            Debug.Assert(false, "NYA"); throw new NotImplementedException();
        }

        protected override bool HasElementTypeImpl()
        {
            return _t.HasElementType;
        }

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters)
        {
            Debug.Assert(false, "NYA"); throw new NotImplementedException();
        }

        protected override bool IsArrayImpl()
        {
            return _IsArrayImpl;
        }

        protected override bool IsByRefImpl()
        {
            return _t.IsByRef;
        }

        protected override bool IsCOMObjectImpl()
        {
            return _t.IsCOMObject;
        }

        protected override bool IsPointerImpl()
        {
            return _t.IsPointer;
        }

        protected override bool IsPrimitiveImpl()
        {
            return _t.IsPrimitive;
        }

        public override Module Module
        {
            get { return _t.Module; ; }
        }

        public override string Namespace
        {
            get { return _t.Namespace; }
        }

        public override Type UnderlyingSystemType
        {
            get { return _UnderlyingSystemType; }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _t.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _t.GetCustomAttributes(inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _t.IsDefined(attributeType, inherit);
        }

        public override string Name
        {
            get { Console.WriteLine("Call to Name -> {0}", "S"); return "S"; }
        }
    }

    class myParameterInfo : ParameterInfo
    {
        public override ParameterAttributes Attributes
        {
            get
            {
                throw new Exception();
            }
        }

        public override object DefaultValue
        {
            get
            {
                throw new Exception();
            }
        }

        public override bool Equals(object obj)
        {
            throw new Exception();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new Exception();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new Exception();
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            throw new Exception();
        }

        public override int GetHashCode()
        {
            throw new Exception();
        }

        public override Type[] GetOptionalCustomModifiers()
        {
            throw new Exception();
        }

        public override Type[] GetRequiredCustomModifiers()
        {
            throw new Exception();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new Exception();
        }
        public override MemberInfo Member
        {
            get
            {
                throw new Exception();
            }
        }

        public override int MetadataToken
        {
            get
            {
                throw new Exception();
            }
        }

        public override string Name
        {
            get
            {
                throw new Exception();
            }
        }

        // This one is consumed -
        // - Exception -> caught and a nice error is emitted
        // - null -> error is emitted "... 'Object reference not set to an instance of an object'"
        public override Type ParameterType
        {
            get
            {
                return typeof(string);
            }
        }

        public override int Position
        {
            get
            {
                throw new Exception();
            }
        }

        public override object RawDefaultValue
        {
            get
            {
                throw new Exception();
            }
        }

        public override string ToString()
        {
            throw new Exception();
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

        public event EventHandler Invalidate
        {
            add
            {
                Helpers.TraceCall();
            }
            remove
            {
                Helpers.TraceCall();
            }
        }

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
