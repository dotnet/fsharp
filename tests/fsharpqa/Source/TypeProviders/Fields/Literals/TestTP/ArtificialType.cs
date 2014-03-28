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

    public class TypeThatHasToStringThatThrows
    {
        public override string ToString()
        {
            throw new Exception("Intentional exception to cover an obscure code path");
        }
    }

    public class ArtificialType : Type
    {
        string _Namespace;
        string _Name;
        Type _BaseType;
        
        ArtificialConstructorInfo _Ctor1;
        FieldInfo _Field_null;
        FieldInfo _Field_single; 
        FieldInfo _Field_double;
        FieldInfo _Field_bool;
        FieldInfo _Field_char;
        FieldInfo _Field_string;
        FieldInfo _Field_sbyte; 
        FieldInfo _Field_byte;
        FieldInfo _Field_int16; 
        FieldInfo _Field_uint16;
        FieldInfo _Field_int; 
        FieldInfo _Field_uint32;
        FieldInfo _Field_int64; 
        FieldInfo _Field_uint64;
        FieldInfo _Field_decimal;
        FieldInfo _Field_TypeThatHasToStringThatThrows;
        FieldInfo _Field_enum;
        FieldInfo _InstField_null;
        FieldInfo _InstField_single; 
        FieldInfo _InstField_double; 
        FieldInfo _InstField_bool; 
        FieldInfo _InstField_char; 
        FieldInfo _InstField_string;
        FieldInfo _InstField_sbyte;
        FieldInfo _InstField_byte; 
        FieldInfo _InstField_int16;
        FieldInfo _InstField_uint16; 
        FieldInfo _InstField_int;
        FieldInfo _InstField_uint32; 
        FieldInfo _InstField_int64;
        FieldInfo _InstField_uint64; 
        FieldInfo _InstField_decimal;
        FieldInfo _InstField_enum;
        FieldInfo _InstField_TypeThatHasToStringThatThrows;

        public ArtificialType(string @namespace, string name, Type basetype)
        {
            _Name = name;
            _Namespace = @namespace;
            _BaseType = basetype;

            _Field_null =   new ArtificialFieldInfo("Field_null", this, typeof(object), IsLiteral: true, IsStatic: true, IsInitOnly: true, RawConstantValue: null);
            _Field_single = new ArtificialFieldInfo("Field_single", this, typeof(float), IsLiteral: true, IsStatic: true, IsInitOnly: true, RawConstantValue: 2.1f);
            _Field_double = new ArtificialFieldInfo("Field_double", this, typeof(double), IsLiteral: true, IsStatic: true, IsInitOnly: true, RawConstantValue: 1.2d);
            _Field_bool =   new ArtificialFieldInfo("Field_bool", this, typeof(bool), IsLiteral: true, IsStatic: true, IsInitOnly: true, RawConstantValue: true);
            _Field_char =   new ArtificialFieldInfo("Field_char", this, typeof(char), IsLiteral: true, IsStatic: true, IsInitOnly: true, RawConstantValue: 'A');
            _Field_string = new ArtificialFieldInfo("Field_string", this, typeof(string), IsLiteral: true, IsStatic: true, IsInitOnly: true, RawConstantValue: "Hello");
            _Field_sbyte =  new ArtificialFieldInfo("Field_sbyte", this, typeof(sbyte), IsLiteral: true, IsStatic: true, IsInitOnly: true, RawConstantValue: SByte.MaxValue);
            _Field_byte =   new ArtificialFieldInfo("Field_byte", this, typeof(byte), IsLiteral: true, IsStatic: true, IsInitOnly: true, RawConstantValue: Byte.MaxValue);
            _Field_int16 =  new ArtificialFieldInfo("Field_int16", this, typeof(short), IsLiteral: true, IsStatic: true, IsInitOnly: true, RawConstantValue: Int16.MaxValue);
            _Field_uint16 = new ArtificialFieldInfo("Field_uint16", this, typeof(ushort), IsLiteral: true, IsStatic: true, IsInitOnly: true, RawConstantValue: UInt16.MaxValue);
            _Field_int =    new ArtificialFieldInfo("Field_int", this, typeof(int), IsLiteral: true, IsStatic: true, IsInitOnly: true, RawConstantValue: Int32.MaxValue);
            _Field_uint32 = new ArtificialFieldInfo("Field_uint32", this, typeof(uint), IsLiteral: true, IsStatic: true, IsInitOnly: true, RawConstantValue: UInt32.MaxValue);
            _Field_int64 =  new ArtificialFieldInfo("Field_int64", this, typeof(Int64), IsLiteral: true, IsStatic: true, IsInitOnly: true, RawConstantValue: Int64.MaxValue);
            _Field_uint64 = new ArtificialFieldInfo("Field_uint64", this, typeof(UInt64), IsLiteral: true, IsStatic: true, IsInitOnly: true, RawConstantValue: UInt64.MaxValue );
            _Field_decimal =new ArtificialFieldInfo("Field_decimal", this, typeof(decimal), IsLiteral: true, IsStatic: true, IsInitOnly: true, RawConstantValue: 10M);
            _Field_enum =   new ArtificialFieldInfo("Field_enum", this, typeof(System.DayOfWeek), IsLiteral: true, IsStatic: true, IsInitOnly: true, RawConstantValue: System.DayOfWeek.Friday);
            _Field_TypeThatHasToStringThatThrows = new ArtificialFieldInfo("Field_TypeThatHasToStringThatThrows", this, typeof(TypeThatHasToStringThatThrows), IsLiteral: true, IsStatic: true, IsInitOnly: true, RawConstantValue: new TypeThatHasToStringThatThrows());

            _InstField_null =   new ArtificialFieldInfo("Instance_Field_null", this, typeof(object), IsLiteral: true, IsStatic: false, IsInitOnly: true, RawConstantValue: null);
            _InstField_single = new ArtificialFieldInfo("Instance_Field_single", this, typeof(float), IsLiteral: true, IsStatic: false, IsInitOnly: true, RawConstantValue: 2.1f);
            _InstField_double = new ArtificialFieldInfo("Instance_Field_double", this, typeof(double), IsLiteral: true, IsStatic: false, IsInitOnly: true, RawConstantValue: 1.2d);
            _InstField_bool = new ArtificialFieldInfo("Instance_Field_bool", this, typeof(bool), IsLiteral: true, IsStatic: false, IsInitOnly: true, RawConstantValue: true);
            _InstField_char = new ArtificialFieldInfo("Instance_Field_char", this, typeof(char), IsLiteral: true, IsStatic: false, IsInitOnly: true, RawConstantValue: 'A');
            _InstField_string = new ArtificialFieldInfo("Instance_Field_string", this, typeof(string), IsLiteral: true, IsStatic: false, IsInitOnly: true, RawConstantValue: "Hello");
            _InstField_sbyte = new ArtificialFieldInfo("Instance_Field_sbyte", this, typeof(sbyte), IsLiteral: true, IsStatic: false, IsInitOnly: true, RawConstantValue: SByte.MaxValue);
            _InstField_byte = new ArtificialFieldInfo("Instance_Field_byte", this, typeof(byte), IsLiteral: true, IsStatic: false, IsInitOnly: true, RawConstantValue: Byte.MaxValue);
            _InstField_int16 = new ArtificialFieldInfo("Instance_Field_int16", this, typeof(short), IsLiteral: true, IsStatic: false, IsInitOnly: true, RawConstantValue: Int16.MaxValue);
            _InstField_uint16 = new ArtificialFieldInfo("Instance_Field_uint16", this, typeof(ushort), IsLiteral: true, IsStatic: false, IsInitOnly: true, RawConstantValue: UInt16.MaxValue);
            _InstField_int = new ArtificialFieldInfo("Instance_Field_int", this, typeof(int), IsLiteral: true, IsStatic: false, IsInitOnly: true, RawConstantValue: Int32.MaxValue);
            _InstField_uint32 = new ArtificialFieldInfo("Instance_Field_uint32", this, typeof(uint), IsLiteral: true, IsStatic: false, IsInitOnly: true, RawConstantValue: UInt32.MaxValue);
            _InstField_int64 = new ArtificialFieldInfo("Instance_Field_int64", this, typeof(Int64), IsLiteral: true, IsStatic: false, IsInitOnly: true, RawConstantValue: Int64.MaxValue);
            _InstField_uint64 = new ArtificialFieldInfo("Instance_Field_uint64", this, typeof(UInt64), IsLiteral: true, IsStatic: false, IsInitOnly: true, RawConstantValue: UInt64.MaxValue);
            _InstField_decimal = new ArtificialFieldInfo("Instance_Field_decimal", this, typeof(decimal), IsLiteral: true, IsStatic: false, IsInitOnly: true, RawConstantValue: 10M);
            _InstField_enum = new ArtificialFieldInfo("Instance_Field_enum", this, typeof(System.DayOfWeek), IsLiteral: true, IsStatic: false, IsInitOnly: true, RawConstantValue: System.DayOfWeek.Friday);
            _InstField_TypeThatHasToStringThatThrows = new ArtificialFieldInfo("Instance_Field_TypeThatHasToStringThatThrows", this, typeof(TypeThatHasToStringThatThrows), IsLiteral: true, IsStatic: false, IsInitOnly: true, RawConstantValue: new TypeThatHasToStringThatThrows());


            _Ctor1 = new ArtificialConstructorInfo(this, new ParameterInfo[] { });
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
            return TypeAttributes.Class | TypeAttributes.Public | (TypeAttributes)TypeProviderTypeAttributes.IsErased;  
        }

        // This one seems to be invoked when in IDE, I type something like:
        // let _ = typeof<N.
        // In this case => no constructors
        public override System.Reflection.ConstructorInfo[] GetConstructors(System.Reflection.BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            // According to the spec, we should only expect these 4, so we guard against that here!
            Debug.Assert(bindingAttr == (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly), "bindingAttr has value not according to the spec!");
            return new System.Reflection.ConstructorInfo[] { _Ctor1 };
        }

        // When you start typing more interesting things like...
        // let a = N.T.M()
        // this one gets invoked...
        public override System.Reflection.MethodInfo[] GetMethods(System.Reflection.BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            // According to the spec, we should only expect these 4, so we guard against that here!
            Debug.Assert(bindingAttr == (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly), "bindingAttr has value not according to the spec!");
            return new System.Reflection.MethodInfo[] { };
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
            return null;
        }

        // Advertise our property...
        // I think that is this one returns an empty array => you don't get intellisense/autocomplete in IDE/FSI
        public override System.Reflection.PropertyInfo[] GetProperties(System.Reflection.BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            // According to the spec, we should only expect these 4, so we guard against that here!
            Debug.Assert(bindingAttr == (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly), "bindingAttr has value not according to the spec!");
            return new PropertyInfo[] { };
        }

        // This on is invoked by the compiler by passing a (possible) field name
        // (i.e. the one it is looking for)
        public override System.Reflection.FieldInfo GetField(string name, System.Reflection.BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            // According to the spec, we should only expect these 4, so we guard against that here!
            Debug.Assert(bindingAttr == (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly), "bindingAttr has value not according to the spec!");

            switch (name)
            {
                case "Field_null":                            return _Field_null;
                case "Field_single":                          return _Field_single;
                case "Field_double":                          return _Field_double;
                case "Field_bool":                            return _Field_bool; 
                case "Field_char":                            return _Field_char;  
                case "Field_string":                          return _Field_string;
                case "Field_sbyte":                           return _Field_sbyte; 
                case "Field_byte":                            return _Field_byte;
                case "Field_int16":                           return _Field_int16; 
                case "Field_uint16":                          return _Field_uint16;
                case "Field_int":                             return _Field_int; 
                case "Field_uint32":                          return _Field_uint32;
                case "Field_int64":                           return _Field_int64;
                case "Field_uint64":                          return _Field_uint64;
                case "Field_decimal":                         return _Field_decimal; 
                case "Field_enum":                            return _Field_enum;
                case "Field_TypeThatHasToStringThatThrows":   return _Field_TypeThatHasToStringThatThrows;
                case "Instance_Field_null":                   return _InstField_null;
                case "Instance_Field_single":                 return _InstField_single;
                case "Instance_Field_double":                 return _InstField_double;
                case "Instance_Field_bool":                   return _InstField_bool;
                case "Instance_Field_char":                   return _InstField_char;
                case "Instance_Field_string":                 return _InstField_string;
                case "Instance_Field_sbyte":                  return _InstField_sbyte;
                case "Instance_Field_byte":                   return _InstField_byte;
                case "Instance_Field_int16":                  return _InstField_int16;
                case "Instance_Field_uint16":                 return _InstField_uint16;
                case "Instance_Field_int":                    return _InstField_int;
                case "Instance_Field_uint32":                 return _InstField_uint32;
                case "Instance_Field_int64":                  return _InstField_int64;
                case "Instance_Field_uint64":                 return _InstField_uint64;
                case "Instance_Field_decimal":                return _InstField_decimal;
                case "Instance_Field_enum":                   return _InstField_enum;
                case "Instance_Field_TypeThatHasToStringThatThrows": return _InstField_TypeThatHasToStringThatThrows;
                default:
                    return null;
            }
        }

        public override System.Reflection.FieldInfo[] GetFields(System.Reflection.BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            // According to the spec, we should only expect these 4, so we guard against that here!
            Debug.Assert(bindingAttr == (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly), "bindingAttr has value not according to the spec!");
            return new System.Reflection.FieldInfo[] 
            {
                _Field_null,
                _Field_single,
                _Field_double,
                _Field_bool, 
                _Field_char,  
                _Field_string,
                _Field_sbyte, 
                _Field_byte,
                _Field_int16, 
                _Field_uint16,
                _Field_int, 
                _Field_uint32,
                _Field_int64,
                _Field_uint64,
                _Field_decimal, 
                _Field_enum,
                _Field_TypeThatHasToStringThatThrows,

                _InstField_null,
                _InstField_single,
                _InstField_double,
                _InstField_bool,
                _InstField_char,
                _InstField_string,
                _InstField_sbyte,
                _InstField_byte,
                _InstField_int16,
                _InstField_uint16,
                _InstField_int,
                _InstField_uint32,
                _InstField_int64,
                _InstField_uint64,
                _InstField_decimal,
                _InstField_enum,
                _InstField_TypeThatHasToStringThatThrows
            };
        }

        // Events
        public override System.Reflection.EventInfo GetEvent(string name, System.Reflection.BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            // According to the spec, we should only expect these 4, so we guard against that here!
            Debug.Assert(bindingAttr == (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly), "bindingAttr has value not according to the spec!");
            return null;
        }

        // Events...
        public override System.Reflection.EventInfo[] GetEvents(System.Reflection.BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            return new System.Reflection.EventInfo[] { };
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
                return false;
            }
        }

        // This is invoked if the IsGenericType is true
        public override Type[] GetGenericArguments()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "Why are we here?");
            throw new NotImplementedException();
        }

        // This one seems to be invoked when compiling something like
        // let a = new N.T()
        // Let's just stay away from generics...
        public override bool IsGenericTypeDefinition
        {
            get
            {
                Helpers.TraceCall();
                return false;
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
                return false;
            }
        }

        // This one seems to be checked when in IDE.
        // let b = N.T(
        protected override bool IsValueTypeImpl()
        {
            Helpers.TraceCall();
            return false;
        }

        // This one seems to be checked when in IDE.
        // let b = N.T(
        protected override bool IsByRefImpl()
        {
            Helpers.TraceCall();
            return false;
        }

        // This one seems to be checked when in IDE.
        // let b = N.T(
        public override bool IsEnum
        {
            get
            {
                Helpers.TraceCall();
                return false;
            }
        }

        // This one seems to be checked when in IDE.
        // let b = N.T(
        protected override bool IsPointerImpl()
        {
            Helpers.TraceCall();
            return false;
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
            Debug.Assert(false, "Why are we calling GetElementType()?");
            throw new NotImplementedException();
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
                return false;
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

        // This method is invoked when I try to create a method that
        // returns an array of synthetic types... 
        // N.T
        // N.T.M() -> N.T[]
        public override Type MakeArrayType()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();

        }

        public override Type MakeArrayType(int rank)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override Type MakeByRefType()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override Type MakeGenericType(params Type[] typeArguments)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override Type MakePointerType()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override MemberTypes MemberType
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override int MetadataToken
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override Type ReflectedType
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override System.Runtime.InteropServices.StructLayoutAttribute StructLayoutAttribute
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override RuntimeTypeHandle TypeHandle
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override string ToString()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        // By spec this NOT supposed to be called!
        public override bool Equals(object o)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "BUG: You should not call Equals");
            throw new NotImplementedException();
        }

        // By spec this NOT supposed to be called!
        public override bool Equals(Type o)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "BUG: You should not call Equals");
            throw new NotImplementedException();
        }

        // By spec this NOT supposed to be called!
        public override int GetHashCode()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "BUG: You should not call GetHashCode");
            throw new NotImplementedException();
        }
    }
}
