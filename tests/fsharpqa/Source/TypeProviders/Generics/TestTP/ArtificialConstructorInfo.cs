using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;

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

        public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override MethodAttributes Attributes
        {
            get
            {
                Helpers.TraceCall();
                return MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
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
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
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
