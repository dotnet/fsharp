using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.FSharp.Core.CompilerServices;

namespace TypeProviderInCSharp
{
    class ArtificialConstructorInfo : ConstructorInfo
    {
        Type _DeclaringType;
        ParameterInfo[] _ParameterInfo;

        public ArtificialConstructorInfo(Type DeclaringType, ParameterInfo[] ParamInfo)
        {
            _DeclaringType = DeclaringType;
            _ParameterInfo = ParamInfo;
        }

        public override Type DeclaringType
        {
            get
            {
                Helpers.TraceCall();
                return _DeclaringType;
            }
        }

        // This one is invoked
        public override ParameterInfo[] GetParameters()
        {
            Helpers.TraceCall();
            return _ParameterInfo;
        }

        // This one is indeed invoked
        // I believe we should always return ".ctor"
        public override string Name
        {
            get
            {
                Helpers.TraceCall();
                return ".ctor";
            }
        }

        // Does it matter what we return here?
        // This property is definitely checked by the compiler in code like this:
        // let _ = new N.T()
        // I copied the attribute set from the .ctor of System.DateTime - the documentation on MSDN assumes that one is already familiar with
        // what they mean
        public override MethodAttributes Attributes
        {
            get
            {
                Helpers.TraceCall();
                return MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
            }
        }

        // TODO: this one is not listed in the spec.
        // This method is invoked to display the tooltip when hovering over the constructor
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            Helpers.TraceCall();
            Debug.Assert(inherit == false, "Is it expected that 'inherit' is true?");
            var attrs = new List<Attribute>();

            if (attributeType == typeof(TypeProviderXmlDocAttribute))
                attrs.Add(new TypeProviderXmlDocAttribute( string.Format("This is a synthetic .ctor created by me for {0}.{1}", this._DeclaringType.Namespace, this._DeclaringType.Name)));
            else if (attributeType == typeof(TypeProviderDefinitionLocationAttribute))
            {
                var f = System.IO.Path.GetTempFileName() + ".fs";
                System.IO.File.WriteAllText(f, string.Format("// This is a fake definition file to test TypeProviderDefinitionLocationAttribute for type {0}.{1}\nnamespace {0}\ntype {1}() = // blah", this._DeclaringType.Namespace, this._DeclaringType.Name));
                attrs.Add(new TypeProviderDefinitionLocationAttribute() { Column = 5 + this._DeclaringType.Name.Length, FilePath = f, Line = 3 });
            }
            else
            {
                var ex = string.Format("Are we looking for an undocumented attribute '{0}'?", attributeType.Name);
                Debug.Assert(false, ex);
                throw new Exception(ex);
            }

            return attrs.ToArray();
        }

        public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
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

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            Helpers.TraceCall();
            return new List<CustomAttributeData>();
        }

    }
}
