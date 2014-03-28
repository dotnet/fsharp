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
        Type _PropertyType;

        MethodInfo _GetMethod;
        MethodInfo _SetMethod;

        public ArtificialPropertyInfo(string Name, Type DeclaringType, Type PropertyType, bool CanRead, bool CanWrite)
        {
            _Name = Name;
            _DeclaringType = DeclaringType;
            _PropertyType = PropertyType;
            _CanRead = CanRead;
            _CanWrite = CanWrite;

            if(CanRead)
                _GetMethod = new ArtificialMethodInfo("get_" + _Name, _DeclaringType, _PropertyType);
            if (CanWrite)
                _SetMethod = new ArtificialMethodInfo("set_" + _Name, _DeclaringType, null /* ?? */);

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
            return _GetMethod;
        }

        // Why is this invoked?
        public override ParameterInfo[] GetIndexParameters()
        {
            Helpers.TraceCall();
            return new ParameterInfo[] { /* new ArtificialParamInfo(typeof(int), isRetVal: false), new ArtificialParamInfo(typeof(decimal), isRetVal: false) */};
        }

        // If CanRead is false, this one gets invoked... without checking 'CanWrite' (?)
        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            Helpers.TraceCall();
            return _SetMethod;
        }

        public override bool CanWrite
        {
            get
            {
                Helpers.TraceCall();
                return _CanWrite;
            }
        }

        // Interestingly enough, this one seems to be invoked only when I hover over the property in the IDE...
        public override Type PropertyType
        {
            get
            {
                Helpers.TraceCall();
                return _PropertyType;
            }
        }

        // Interestingly enough, this one seems to be invoked only when I hover over the property in the IDE...
        public override Type DeclaringType
        {
            get
            {
                Helpers.TraceCall();
                return _DeclaringType;
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

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
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
