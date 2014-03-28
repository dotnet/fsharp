using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TypeProviderInCSharp
{
    class ArtificialPropertyInfo : PropertyInfo
    {
        string _Name;
        bool _CanRead;
        bool _CanWrite;
        Type _DeclaringType;

        public ArtificialPropertyInfo(string Name, Type DeclaringType, bool CanRead, bool CanWrite)
        {
            _Name = Name;
            _DeclaringType = DeclaringType;
            _CanRead = CanRead;
            _CanWrite = CanWrite;
        }

        // The name of this property...
        public override string Name
        {
            get
            {
                Helpers.TraceCall();
                return _Name;
            }
        }

        // Needed
        public override bool CanRead
        {
            get
            {
                Helpers.TraceCall();
                return _CanRead;
            }
        }

        // If CanRead is true, this one gets invoked.
        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            Helpers.TraceCall();
            Debug.Assert(!nonPublic, "GetGetMethod was invoked with nonPublic=true");
            return null;
        }

        // If CanRead is false, this one gets invoked... without checking 'CanWrite' (?)
        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            Helpers.TraceCall();
            Debug.Assert(!nonPublic, "GetSetMethod was invoked with nonPublic=true");
            return null;
        }

        public override bool CanWrite
        {
            get
            {
                Helpers.TraceCall();
                return _CanWrite;
            }
        }

        public override PropertyAttributes Attributes
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override Type PropertyType
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override Type DeclaringType
        {
            get
            {
                Helpers.TraceCall();
                return _DeclaringType;
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
