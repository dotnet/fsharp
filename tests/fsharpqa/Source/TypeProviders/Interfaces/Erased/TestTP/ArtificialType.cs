using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.FSharp.Core.CompilerServices;

namespace TypeProviderInCSharp
{
    public class ArtificialType : Type
    {
        string _Namespace;
        string _Name;
        bool _IsGenericType;
        bool _IsValueType;  // value type = not class / not interface
        bool _IsByRef;      // is the value passed by reference?
        bool _IsEnum;
        bool _IsPointer;

        bool _IsInterface;  // true if this type is an interface
        bool _IsAbstract;   // true if this type is abstract

        Type _BaseType;
        Type _ElementType;
        MethodInfo _Method1;
        MethodInfo _Method2;
        MethodInfo _Method3;
        MethodInfo _Method4;

        PropertyInfo _Property1;
        PropertyInfo _Property2;
        PropertyInfo _Property3;

        EventInfo _Event1;
        FieldInfo _Field1;
        ConstructorInfo _Ctor1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="name"></param>
        /// <param name="isGenericType"></param>
        /// <param name="basetype"></param>
        /// <param name="isValueType"></param>
        /// <param name="isByRef"></param>
        /// <param name="isEnum"></param>
        /// <param name="isPointer"></param>
        /// <param name="ElementType">
        /// Type of the object encompassed or referred to by the current array, pointer or reference type. It's what GetElementType() will return.
        /// Specify null when it is not needed
        /// </param>
        public ArtificialType(string @namespace, string name, bool isGenericType, Type basetype, 
                              bool isValueType, bool isByRef, bool isEnum, bool isPointer, Type ElementType,
                              bool isInterface,
                              bool isAbstract,
                              Type MReturnSynType
                            )
        {
            _Name = name;
            _Namespace = @namespace;
            _IsGenericType = isGenericType;
            _BaseType = basetype;
            _IsValueType = isValueType;
            _IsByRef = isByRef;
            _IsEnum = isEnum;
            _IsPointer = isPointer;
            _IsInterface = isInterface;
            _IsAbstract = isAbstract;


            _Method1 = new ArtificialMethodInfo("M", this, typeof(int[]), MethodAttributes.Public | (_IsAbstract ? MethodAttributes.PrivateScope | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.Abstract : MethodAttributes.Static), null, false);
            _Method2 = new ArtificialMethodInfo("RaiseEvent1", this, typeof(void), MethodAttributes.Public | (_IsAbstract ? MethodAttributes.Abstract : 0), null, false);
            _Method3 = new ArtificialMethodInfo("M1", this, this, MethodAttributes.Public | MethodAttributes.Static | (_IsAbstract ? MethodAttributes.Abstract : 0), new[] { new ArtificialParamInfo("x1", basetype, false) }, false);
            _Method4 = new ArtificialMethodInfo("M2", this, this, MethodAttributes.Public | MethodAttributes.Static | (_IsAbstract ? MethodAttributes.Abstract : 0), new[] { new ArtificialParamInfo("y1", this, false) }, false);

            _Property1 = new ArtificialPropertyInfo("StaticProp1", this, typeof(decimal), true, false);
            _Property2 = new ArtificialPropertyInfo("StaticProp2", this, typeof(System.Tuple<int, int, int, int, int, int, int, System.Tuple<int, int, int>>), true, false);
            _Property3 = new ArtificialPropertyInfo("StaticProp3", this, typeof(System.Tuple<int, int, int>), true, false);
            _Event1 = new ArtificalEventInfo("Event1", this, typeof(EventHandler));
            _Ctor1 = new ArtificialConstructorInfo(this, new ParameterInfo[] {} );  // parameter-less ctor
            _ElementType = ElementType;
        }

        public override System.Reflection.Assembly Assembly
        {
            get 
            { 
                Helpers.TraceCall();
                return Assembly.GetExecutingAssembly();
            }
        }

        public override string Name
        {
            get 
            { 
                Helpers.TraceCall();
                return _Name;
            }
        }

        public override Type BaseType
        {
            get
            {
                Helpers.TraceCall();
                return _BaseType;
            }
        }

        public override string Namespace
        {
            get
            {
                Helpers.TraceCall();
                return _Namespace;
            }
        }

        public override string FullName
        {
            get
            { 
                Helpers.TraceCall();
                return string.Format("{0}.{1}", _Namespace, _Name);
            }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "Why are we calling into GetCustomAttributes()?");
            return null;
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "Why are we calling into GetCustomAttributes()?");
            return null;
        }

        // TODO: what is this?
        protected override System.Reflection.TypeAttributes GetAttributeFlagsImpl()
        {
            Helpers.TraceCall();

            //return TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Class | TypeAttributes.NestedPublic | TypeAttributes.ClassSemanticsMask | TypeAttributes.Abstract | TypeAttributes.Serializable;
            return (_IsInterface ? TypeAttributes.Interface : TypeAttributes.Class) | TypeAttributes.Public | (_IsAbstract ? TypeAttributes.Abstract : 0) | (TypeAttributes)TypeProviderTypeAttributes.IsErased;  
        }

        // This one seems to be invoked when in IDE, I type something like:
        // let _ = typeof<N.
        // In this case => no constructors
        public override System.Reflection.ConstructorInfo[] GetConstructors(System.Reflection.BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            // According to the spec, we should only expect these 4, so we guard against that here!
            Debug.Assert(bindingAttr == (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly), "bindingAttr has value not according to the spec!");
            if (!_IsAbstract && _Ctor1 != null) 
                return new System.Reflection.ConstructorInfo[] { _Ctor1 };
            else
                return new System.Reflection.ConstructorInfo[] { };
        }

        // When you start typing more interesting things like...
        // let a = N.T.M()
        // this one gets invoked...
        public override System.Reflection.MethodInfo[] GetMethods(System.Reflection.BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            // According to the spec, we should only expect these 4, so we guard against that here!
            Debug.Assert(bindingAttr == (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly), "bindingAttr has value not according to the spec!");

            if (_IsInterface)
                return new System.Reflection.MethodInfo[] { _Method1 };
            else
                return new[] { _Method1, _Method2, _Method3, _Method4 };
        }

        // This method is called when in the source file we have something like:
        // - N.T.StaticProp 
        // (most likely also when we have an instance prop...)
        // name -> "StaticProp"
        protected override System.Reflection.PropertyInfo GetPropertyImpl(string name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, Type returnType, Type[] types, System.Reflection.ParameterModifier[] modifiers)
        {
            Helpers.TraceCall();
            // According to the spec, we should only expect these 4, so we guard against that here!
            Debug.Assert(bindingAttr == (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly), "bindingAttr has value not according to the spec!");
            Debug.Assert(binder == null && returnType == null && types == null && modifiers == null, "One of binder, returnType, types, or modifiers was not null");
            if (name == _Property1.Name)
                return _Property1;
            else if (name == _Property2.Name)
                return _Property2;
            else if (name == _Property3.Name)
                return _Property3;
            else
                return null;
        }

        // Advertise our property...
        // I think that is this one returns an empty array => you don't get intellisense/autocomplete in IDE/FSI
        public override System.Reflection.PropertyInfo[] GetProperties(System.Reflection.BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            // According to the spec, we should only expect these 4, so we guard against that here!
            Debug.Assert(bindingAttr == (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly), "bindingAttr has value not according to the spec!");

            if (_IsInterface)
                return new PropertyInfo[] { };
            else
                return new PropertyInfo[] { _Property1, _Property2, _Property3 };
        }

        // No fields...
        public override System.Reflection.FieldInfo GetField(string name, System.Reflection.BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            // According to the spec, we should only expect these 4, so we guard against that here!
            Debug.Assert(bindingAttr == (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly), "bindingAttr has value not according to the spec!");
            return null;
        }

        public override System.Reflection.FieldInfo[] GetFields(System.Reflection.BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            // According to the spec, we should only expect these 4, so we guard against that here!
            Debug.Assert(bindingAttr == (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly), "bindingAttr has value not according to the spec!");
            return new System.Reflection.FieldInfo[] { };
        }

        // Events
        public override System.Reflection.EventInfo GetEvent(string name, System.Reflection.BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            // According to the spec, we should only expect these 4, so we guard against that here!
            Debug.Assert(bindingAttr == (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly), "bindingAttr has value not according to the spec!");
            if (_Event1 != null && _Event1.Name == name && !_IsInterface)
                return _Event1;
            else
                return null;
        }

        // Events...
        public override System.Reflection.EventInfo[] GetEvents(System.Reflection.BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            if(_IsInterface)
                return new System.Reflection.EventInfo[] { };
            else
                return _Event1 != null ? new [] { _Event1 } : new System.Reflection.EventInfo[] { };
        }

        // TODO: according to the spec, this should not be invoked... instead it seems like it may be invoked...
        //       ?? I have no idea what this is used for... ??
        public override Type UnderlyingSystemType
        {
            get
            {
                Helpers.TraceCall();
                return null;
            }
        }

        // According to the spec, this should always be 'false'
        protected override bool IsArrayImpl()
        {
            Helpers.TraceCall();
            return false;    
        }

        // No interfaces...
        public override Type[] GetInterfaces()
        {
            Helpers.TraceCall();
            return new Type[] { };
        }

        // No nested type
        // This method is invoked on the type 'T', e.g.:
        //    let _ = N.T.M
        // to figure out if M is a nested type.
        public override Type GetNestedType(string name, System.Reflection.BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            return null;
        }
        public override Type[] GetNestedTypes(System.Reflection.BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            // According to the spec, we should only expect these 4, so we guard against that here!
            Debug.Assert(bindingAttr == (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly), "bindingAttr has value not according to the spec!");
            return new Type[] { };
        }

        // This one is invoked when the type has a .ctor 
        // and the code looks like
        // let _ = new N.T()
        // for example.
        // It was observed that the
        // TODO: cover both cases!
        public override bool IsGenericType
        {
            get
            {
                Helpers.TraceCall();
                return _IsGenericType;
            }
        }

        // This is invoked if the IsGenericType is true
        public override Type[] GetGenericArguments()
        {
            Helpers.TraceCall();
            if (_IsGenericType)
                return new Type[] { typeof(int), typeof(decimal), typeof(System.Guid) };        // This is currently triggering an ICE...
            else
            {
                Debug.Assert(false, "Why are we here?");
                throw new NotImplementedException();
            }

        }

        // This one seems to be invoked when compiling something like
        // let a = new N.T()
        // Let's just stay away from generics...
        public override bool IsGenericTypeDefinition
        {
            get
            {
                Helpers.TraceCall();
                return _IsGenericType;
            }
        }

        // This one seems to be invoked when compiling something like
        // let a = new N.T()
        // Let's just stay away from generics...
        public override bool ContainsGenericParameters
        {
            get
            {
                Helpers.TraceCall();
                return _IsGenericType;
            }
        }

        // This one seems to be checked when in IDE.
        // let b = N.T(
        protected override bool IsValueTypeImpl()
        {
            Helpers.TraceCall();
            return _IsValueType;
        }

        // This one seems to be checked when in IDE.
        // let b = N.T(
        protected override bool IsByRefImpl()
        {
            Helpers.TraceCall();
            return _IsByRef;
        }

        // This one seems to be checked when in IDE.
        // let b = N.T(
        public override bool IsEnum
        {
            get
            {
                Helpers.TraceCall();
                return _IsEnum;
            }
        }

        // This one seems to be checked when in IDE.
        // let b = N.T(
        protected override bool IsPointerImpl()
        {
            Helpers.TraceCall();
            return _IsPointer;
        }

        // I've seen this one being invoked when I had something like this:
        //  let t = new N.T()
        //  t.Event1.AddHandler( System.EventHandler( fun a b -> printfn "%A" System.DateTime.Now ) )
        //  // Raise the event:
        //  t.RaiseEvent1()              <-- 
        protected override TypeCode GetTypeCodeImpl()
        {
            Helpers.TraceCall();
            return TypeCode.Object;
        }

        public override string AssemblyQualifiedName
        {
            get 
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override Guid GUID
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        protected override System.Reflection.ConstructorInfo GetConstructorImpl(System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Reflection.CallingConventions callConvention, Type[] types, System.Reflection.ParameterModifier[] modifiers)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override Type GetElementType()
        {
            Helpers.TraceCall();
            Debug.Assert(this.IsArray || this.IsPointer || this.IsByRef, "Why are we calling GetElementType()?");
            return _ElementType;
        }

        public override Type GetInterface(string name, bool ignoreCase)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override System.Reflection.MemberInfo[] GetMembers(System.Reflection.BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        protected override System.Reflection.MethodInfo GetMethodImpl(string name, System.Reflection.BindingFlags bindingAttr, System.Reflection.Binder binder, System.Reflection.CallingConventions callConvention, Type[] types, System.Reflection.ParameterModifier[] modifiers)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        protected override bool HasElementTypeImpl()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override object InvokeMember(string name, System.Reflection.BindingFlags invokeAttr, System.Reflection.Binder binder, object target, object[] args, System.Reflection.ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        protected override bool IsCOMObjectImpl()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        protected override bool IsPrimitiveImpl()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override System.Reflection.Module Module
        {
            get {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override MethodBase DeclaringMethod
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                return base.DeclaringMethod;
            }
        }

        // This one is invoked by the F# compiler!
        public override Type DeclaringType
        {
            get
            {
                Helpers.TraceCall();
                return null; // base.DeclaringType;
            }
        }

        public override Type[] FindInterfaces(TypeFilter filter, object filterCriteria)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.FindInterfaces(filter, filterCriteria);
        }

        public override MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter, object filterCriteria)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.FindMembers(memberType, bindingAttr, filter, filterCriteria);
        }

        public override GenericParameterAttributes GenericParameterAttributes
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                return base.GenericParameterAttributes;
            }
        }

        public override int GenericParameterPosition
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                return base.GenericParameterPosition;
            }
        }

        public override int GetArrayRank()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.GetArrayRank();
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            Helpers.TraceCall();
            var attrs = new List<CustomAttributeData>();

            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderXmlDocAttribute("This is a synthetic type created by me!")));

            var f = System.IO.Path.GetTempFileName() + ".fs";
            System.IO.File.WriteAllText(f, string.Format("// This is a fake definition file to test TypeProviderDefinitionLocationAttribute for type {0}.{1}\nnamespace {0}\ntype {1} = // blah", _Namespace, _Name));
            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderDefinitionLocationAttribute() { Column = 5, FilePath = f, Line = 3 }));

            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderEditorHideMethodsAttribute()));

            return attrs;
        }

        public override MemberInfo[] GetDefaultMembers()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.GetDefaultMembers();
        }

        public override string GetEnumName(object value)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.GetEnumName(value);
        }

        public override string[] GetEnumNames()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.GetEnumNames();
        }

        public override Type GetEnumUnderlyingType()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.GetEnumUnderlyingType();
        }

        public override Array GetEnumValues()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.GetEnumValues();
        }

        public override EventInfo[] GetEvents()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.GetEvents();
        }

        public override Type[] GetGenericParameterConstraints()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.GetGenericParameterConstraints();
        }

        public override Type GetGenericTypeDefinition()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.GetGenericTypeDefinition();
        }

        public override InterfaceMapping GetInterfaceMap(Type interfaceType)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.GetInterfaceMap(interfaceType);
        }

        public override MemberInfo[] GetMember(string name, BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.GetMember(name, bindingAttr);
        }

        public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.GetMember(name, type, bindingAttr);
        }

        public override bool IsAssignableFrom(Type c)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.IsAssignableFrom(c);
        }

        protected override bool IsContextfulImpl()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.IsContextfulImpl();
        }

        public override bool IsEnumDefined(object value)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.IsEnumDefined(value);
        }

        public override bool IsEquivalentTo(Type other)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.IsEquivalentTo(other);
        }

        // This one seems to be invoked when I do something like:
        // type I = interface
        //           inherit N.I1
        //          end        
        //
        // (I :> N.I1).M
        // As per spec, this is supposed to return 'false'
        public override bool IsGenericParameter
        {
            get
            {
                Helpers.TraceCall();
                return _IsGenericType;
            }
        }

        public override bool IsInstanceOfType(object o)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.IsInstanceOfType(o);
        }

        protected override bool IsMarshalByRefImpl()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.IsMarshalByRefImpl();
        }

        public override bool IsSecurityCritical
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                return base.IsSecurityCritical;
            }
        }

        public override bool IsSecuritySafeCritical
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                return base.IsSecuritySafeCritical;
            }
        }

        public override bool IsSecurityTransparent
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                return base.IsSecurityTransparent;
            }
        }

        public override bool IsSerializable
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                return base.IsSerializable;
            }
        }

        public override bool IsSubclassOf(Type c)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.IsSubclassOf(c);
        }

        public override Type MakeArrayType()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.MakeArrayType();
        }

        public override Type MakeArrayType(int rank)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.MakeArrayType(rank);
        }

        public override Type MakeByRefType()
        {
            return base.MakeByRefType();
        }

        public override Type MakeGenericType(params Type[] typeArguments)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.MakeGenericType(typeArguments);
        }

        public override Type MakePointerType()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.MakePointerType();
        }

        public override MemberTypes MemberType
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                return base.MemberType;
            }
        }

        public override int MetadataToken
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                return base.MetadataToken;
            }
        }

        public override Type ReflectedType
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                return base.ReflectedType;
            }
        }

        public override System.Runtime.InteropServices.StructLayoutAttribute StructLayoutAttribute
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                return base.StructLayoutAttribute;
            }
        }

        public override RuntimeTypeHandle TypeHandle
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                return base.TypeHandle;
            }
        }

        public override string ToString()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            return base.ToString();
        }

        // By spec this NOT supposed to be called!
        // TODO: re-enable assert when bug is fixed
        public override bool Equals(object o)
        {
            Helpers.TraceCall();
            //Debug.Assert(false, "You should not call Equals");
            return base.Equals(o);
            // throw new NotImplementedException();
        }

        // By spec this NOT supposed to be called!
        // TODO: re-enable assert when bug is fixed
        public override bool Equals(Type o)
        {
            Helpers.TraceCall();
            //Debug.Assert(false, "You should not call Equals");
            //throw new NotImplementedException();
            return base.Equals(o);
        }

        // By spec this NOT supposed to be called!
        public override int GetHashCode()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "GetHashCode");
            throw new NotImplementedException();
        }
    }
}
