using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TypeProviderInCSharp
{
    public class ArtificialParamInfo : ParameterInfo
    {
        Type _type;
        bool _isRetVal;
        string _Name;

        public ArtificialParamInfo(string Name, Type type, bool isRetVal)
        {
            _type = type;
            _isRetVal = IsRetval;
            _Name = Name;
        }

        // TODO: allow more customizations...
        public override ParameterAttributes Attributes
        {
            get
            {
                return _isRetVal ? ParameterAttributes.Retval : ParameterAttributes.None;
            }
        }

        public override Type ParameterType
        {
            get
            {
                return _type;
            }
        }

        // THis one is invoked when you have methods that takes arguments.
        public override string Name
        {
            get
            {
                Helpers.TraceCall();
                return _Name;
            }
        }


        public override object DefaultValue
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override bool Equals(object obj)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "Why are we calling into GetCustomAttributes()?");
            return null;
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "Why are we calling into GetCustomAttributes()?");
            return null;
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            Helpers.TraceCall();
            return new List<CustomAttributeData>();
        }

        public override int GetHashCode()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override Type[] GetOptionalCustomModifiers()
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override Type[] GetRequiredCustomModifiers()
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

        public override MemberInfo Member
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

        public override int Position
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override object RawDefaultValue
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
    }

}
