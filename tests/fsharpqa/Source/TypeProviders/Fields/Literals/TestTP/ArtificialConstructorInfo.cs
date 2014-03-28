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

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "Why are we calling into GetCustomAttributes()?");
            return null;
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
            Debug.Assert(false, "Why are we calling into GetCustomAttributes()?");
            return null;
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
            var attrs = new List<CustomAttributeData>();

            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderXmlDocAttribute(string.Format("This is a synthetic .ctor created by me for {0}.{1}", this._DeclaringType.Namespace, this._DeclaringType.Name))));

            var f = System.IO.Path.GetTempFileName() + ".fs";
            System.IO.File.WriteAllText(f, string.Format("// This is a fake definition file to test TypeProviderDefinitionLocationAttribute for type {0}.{1}\nnamespace {0}\ntype {1} = // blah", this._DeclaringType.Namespace, this._DeclaringType.Name));
            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderDefinitionLocationAttribute() { Column = 5 + this._DeclaringType.Name.Length, FilePath = f, Line = 3 }));

            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderEditorHideMethodsAttribute()));

            return attrs;
        }

        // By spec this NOT supposed to be called!
        public override bool Equals(object obj)
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

        public override string ToString()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "Why are we here?");
            throw new NotImplementedException();
        }
    }
}
