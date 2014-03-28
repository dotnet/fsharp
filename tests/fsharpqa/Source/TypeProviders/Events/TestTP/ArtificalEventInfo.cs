using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TypeProviderInCSharp
{
    class ArtificalEventInfo : EventInfo
    {
        string _Name;
        Type _DeclaringType;

        public ArtificalEventInfo(string Name, Type DeclaringType)
        {
            _Name = Name;
            _DeclaringType = DeclaringType;
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

        public override EventAttributes Attributes
        {
            get
            {
                Helpers.TraceCall();
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override MethodInfo GetAddMethod(bool nonPublic)
        {
            Helpers.TraceCall();
#if GetAddMethodReturnsNull
            return null;
#else
            return new ArtificialMethodInfo("foo_add", this.DeclaringType, typeof(void));
#endif
        }

        public override MethodInfo GetRemoveMethod(bool nonPublic)
        {
            Helpers.TraceCall();
#if GetRemoveMethodReturnsNull
            return null;
#else
            return new ArtificialMethodInfo("foo_remove", this.DeclaringType, typeof(void));
#endif
        }

        public override MethodInfo GetRaiseMethod(bool nonPublic)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "NYI");
            //throw new NotImplementedException();
            return null;
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

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            Helpers.TraceCall();
            return new List<CustomAttributeData>();
        }
    }
}
