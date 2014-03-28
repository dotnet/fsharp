using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.FSharp.Core.CompilerServices;

namespace TypeProviderInCSharp
{
    public class ArtificialType : Type
    {
        string _Namespace;
        string _Name;

        public ArtificialType(string @namespace, string name)
        {
            _Name = name;
            _Namespace = @namespace;
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
                return typeof(int[]);
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
            Debug.Assert(false, "Why are we calling into GetCustomAttributes()?");
            return null;
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            Debug.Assert(false, "Why are we calling into GetCustomAttributes()?");
            return null;
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

        // TODO: what is this?
        protected override System.Reflection.TypeAttributes GetAttributeFlagsImpl()
        {
            Helpers.TraceCall();
            return TypeAttributes.Class | TypeAttributes.Public | 
                   (TypeAttributes)TypeProviderTypeAttributes.IsErased; // add the special flag to indicate an erased type
        }

        // This one seems to be invoked when in IDE, I type something like:
        // let _ = typeof<N.
        // In this case => no constructors
        public override System.Reflection.ConstructorInfo[] GetConstructors(System.Reflection.BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            // According to the spec, we should only expect these 4, so we guard against that here!
            Debug.Assert(bindingAttr == (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly), "bindingAttr has value not according to the spec!");
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
            var m = new ArtificialMethodInfo("M", this);
            return new[] { m };
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
            if (name == "StaticProp")
                return new ArtificialPropertyInfo(name, this, !false, false);
            else if (name == "StaticProp2")
                return new ArtificialPropertyInfo(name, this, false, !false);
            else
                return null;
        }
        public override System.Reflection.PropertyInfo[] GetProperties(System.Reflection.BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            // According to the spec, we should only expect these 4, so we guard against that here!
            Debug.Assert(bindingAttr == (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly), "bindingAttr has value not according to the spec!");
            return new PropertyInfo[] { };
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

        // No events...
        public override System.Reflection.EventInfo GetEvent(string name, System.Reflection.BindingFlags bindingAttr)
        {
            Helpers.TraceCall();
            // According to the spec, we should only expect these 4, so we guard against that here!
            Debug.Assert(bindingAttr == (BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly), "bindingAttr has value not according to the spec!");
            return null;
        }

        // No events...
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


        protected override bool IsByRefImpl()
        {
            Helpers.TraceCall();
            return false;
        }

        protected override bool IsPointerImpl()
        {
            Helpers.TraceCall();
            return false;
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
            Debug.Assert(false, "NYI");
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
    }
}
