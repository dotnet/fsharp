using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TypeProviderInCSharp
{
    class ArtificialFieldInfo : FieldInfo
    {
        string _Name;
        Type _DeclaringType;
        Type _FieldType;
        bool _IsLiteral;
        bool _IsStatic;
        bool _IsInitOnly;
        object _RawConstantValue;

        public ArtificialFieldInfo(string Name, Type DeclaringType, Type FieldType, bool IsLiteral, bool IsStatic, bool IsInitOnly, object RawConstantValue)
        {
            _Name = Name;
            _DeclaringType = DeclaringType;
            _FieldType = FieldType;
            _IsLiteral = IsLiteral;
            _IsStatic = IsStatic;
            _IsInitOnly = IsInitOnly || IsLiteral;  // Literals are really consts, so I think it's ok to enforce this..
            _RawConstantValue = RawConstantValue;
        }

        public override string Name
        {
            get
            {
                Helpers.TraceCall(); 
                return _Name;
            }
        }

        // This is one is definitely invoked.
        public override FieldAttributes Attributes
        {
            get 
            {
                var a = System.Reflection.FieldAttributes.FamANDAssem | System.Reflection.FieldAttributes.Family;
                if(_IsStatic)
                    a |= System.Reflection.FieldAttributes.Static;
                if(_IsLiteral)
                    a |= System.Reflection.FieldAttributes.Literal | System.Reflection.FieldAttributes.InitOnly;
                if (_IsInitOnly)
                    a |= System.Reflection.FieldAttributes.InitOnly;
                return a;
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

        public override bool Equals(object obj)
        {
            Helpers.TraceCall();
            System.Diagnostics.Debug.Assert(false, "BUG: We should not be calling Equals()");
            throw new NotImplementedException();
        }


        public override RuntimeFieldHandle FieldHandle
        {
            get
            {
                Helpers.TraceCall();
                System.Diagnostics.Debug.Assert(false, "Why are we here?");
                throw new NotImplementedException();
            }
        }

        public override Type FieldType
        {
            get { 
                Helpers.TraceCall();
                return _FieldType; }
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            Helpers.TraceCall();
            System.Diagnostics.Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            Helpers.TraceCall();
            System.Diagnostics.Debug.Assert(false, "NYI"); 
            throw new NotImplementedException();
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            Helpers.TraceCall();
            return new List<CustomAttributeData>();
        }

        public override int GetHashCode()
        {
            Helpers.TraceCall();
            System.Diagnostics.Debug.Assert(false, "BUG: We should not be calling Equals()");
            throw new NotImplementedException();
        }

        public override Type[] GetOptionalCustomModifiers()
        {
            Helpers.TraceCall();
            System.Diagnostics.Debug.Assert(false, "NYI"); 
            throw new NotImplementedException();
        }

        public override object GetRawConstantValue()
        {
            Helpers.TraceCall();
            return _RawConstantValue;
        }

        public override Type[] GetRequiredCustomModifiers()
        {
            Helpers.TraceCall();
            System.Diagnostics.Debug.Assert(false, "NYI"); 
            throw new NotImplementedException();
        }

        public override object GetValue(object obj)
        {
            Helpers.TraceCall();
            System.Diagnostics.Debug.Assert(false, "NYI"); 
            throw new NotImplementedException();
        }

        public override object GetValueDirect(TypedReference obj)
        {
            Helpers.TraceCall();
            System.Diagnostics.Debug.Assert(false, "NYI"); 
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            Helpers.TraceCall();
            System.Diagnostics.Debug.Assert(false, "NYI"); 
            throw new NotImplementedException();
        }

        public override bool IsSecurityCritical
        {
            get
            {
                Helpers.TraceCall();
                System.Diagnostics.Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override bool IsSecuritySafeCritical
        {
            get
            {
                Helpers.TraceCall();
                System.Diagnostics.Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override bool IsSecurityTransparent
        {
            get
            {
                Helpers.TraceCall();
                System.Diagnostics.Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override MemberTypes MemberType
        {
            get
            {
                Helpers.TraceCall();
                System.Diagnostics.Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override int MetadataToken
        {
            get
            {
                Helpers.TraceCall();
                System.Diagnostics.Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override Module Module
        {
            get
            {
                Helpers.TraceCall();
                System.Diagnostics.Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override Type ReflectedType
        {
            get
            {
                Helpers.TraceCall();
                System.Diagnostics.Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, System.Globalization.CultureInfo culture)
        {
            Helpers.TraceCall();
            System.Diagnostics.Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override void SetValueDirect(TypedReference obj, object value)
        {
            Helpers.TraceCall();
            System.Diagnostics.Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            Helpers.TraceCall();
            System.Diagnostics.Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }
    }
}
