using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.FSharp.Core.CompilerServices;

namespace TypeProviderInCSharp
{
    class ArtificialMethodInfo : MethodInfo
    {
        string _Name;
        Type _DeclaringType;
        Type _ReturnType;

        public ArtificialMethodInfo(string Name, Type DeclaringType, Type ReturnType)
        {
            _Name = Name;
            _DeclaringType = DeclaringType;
            _ReturnType = ReturnType;
        }

        public override string Name
        {
            get 
            { 
                Helpers.TraceCall();
                return _Name;
            }
        }

        public override Type DeclaringType
        {
            get
            {
                Helpers.TraceCall();
                return _DeclaringType;
            }
        }

        // Make the method Public and Static - 
        // TODO: should be configurable in the ctor...
        public override MethodAttributes Attributes
        {
            get
            {
                Helpers.TraceCall();
                return MethodAttributes.Public | MethodAttributes.Static;
            }
        }

        // No params
        // TODO: should be configurable in the ctor...
        public override ParameterInfo[] GetParameters()
        {
            Helpers.TraceCall();
            return new ParameterInfo[] {  };
        }

        public override ParameterInfo ReturnParameter
        {
            get
            {
                //Helpers.TraceCall();
                //var retvalpi = new ArtificialParamInfo(typeof(List<>), true);
                //return retvalpi;
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();

            }
        }

        public override Type ReturnType
        {
            get
            {
                Helpers.TraceCall();
                return _ReturnType;
            }
        }

        public override MethodInfo GetBaseDefinition()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override ICustomAttributeProvider ReturnTypeCustomAttributes
        {
            get 
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override RuntimeMethodHandle MethodHandle
        {
            get 
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            Helpers.TraceCall();
            Debug.Assert(inherit == false, "Is it expected that 'inherit' is true?");
            var attrs = new List<Attribute>();

            if (attributeType == typeof(TypeProviderXmlDocAttribute))
                attrs.Add(new TypeProviderXmlDocAttribute("This is a synthetic *method* created by me!"));
            else if (attributeType == typeof(TypeProviderDefinitionLocationAttribute))
            {
                var f = System.IO.Path.GetTempFileName() + ".fs";
                System.IO.File.WriteAllText(f, string.Format("// This is a fake definition file to test TypeProviderDefinitionLocationAttribute for method {0}.{1}.{2}\nnamespace {0}\ntype {1} = static member {2}() = [|1,2,3|]", _DeclaringType.Namespace, _DeclaringType.Name, _Name));
                attrs.Add(new TypeProviderDefinitionLocationAttribute() { Column = 22 + _DeclaringType.Name.Length, FilePath = f, Line = 3 });
            }
            else
                throw new Exception(string.Format("Are we looking for an undocumented attribute '{0}'?", attributeType.Name));

            return attrs.ToArray();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI"); 
            throw new NotImplementedException();
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
    }
}
