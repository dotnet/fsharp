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
    class ArtificalEventInfo : EventInfo
    {
        string _Name;
        Type _DeclaringType;
        Type _EventHandleType;
        MethodInfo _AddMethod;
        MethodInfo _RemoveMethod;

        public ArtificalEventInfo(string Name, Type DeclaringType, Type EventHandleType)
        {
            _Name = Name;
            _DeclaringType = DeclaringType;
            _EventHandleType = EventHandleType;

            _AddMethod = new ArtificialMethodInfo("add_" + _Name, _DeclaringType, typeof(void), MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Public, null, false);
            _RemoveMethod = new ArtificialMethodInfo("remove_" + _Name, _DeclaringType, typeof(void), MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Public, null, false);
        }

        // This one is invoked
        public override Type EventHandlerType
        {
            get
            {
                Helpers.TraceCall();
                return _EventHandleType;
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

        public override Type DeclaringType
        {
            get
            {
                Helpers.TraceCall();
                return _DeclaringType;
            }
        }

        // This one is needed
        public override MethodInfo GetAddMethod(bool nonPublic)
        {
            Helpers.TraceCall();
            Debug.Assert(!nonPublic, "GetAddMethod() was called with nonPublic=true");
            return _AddMethod;
        }

        // This one is needed
        public override MethodInfo GetRemoveMethod(bool nonPublic)
        {
            Helpers.TraceCall();
            Debug.Assert(!nonPublic, "GetRemoveMethod() was called with nonPublic=true");
            return _RemoveMethod;
        }


        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            Helpers.TraceCall();
            Debug.Assert(false, "Why are we calling into GetCustomAttributes()?");
            return null;
        }

        public override EventAttributes Attributes
        {
            get
            {
                Helpers.TraceCall();
                return EventAttributes.None;
            }
        }

        public override MethodInfo GetRaiseMethod(bool nonPublic)
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

            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderXmlDocAttribute(string.Format("This is a synthetic *event* created by me for {0}.{1}", this._DeclaringType.Namespace, this._DeclaringType.Name))));

            var f = System.IO.Path.GetTempFileName() + ".fs";
            System.IO.File.WriteAllText(f, string.Format("// This is a fake definition file to test TypeProviderDefinitionLocationAttribute for type {0}.{1}\nnamespace {0}\ntype {1} = // blah", this._DeclaringType.Namespace, this._DeclaringType.Name));
            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderDefinitionLocationAttribute() { Column = 5 + this._DeclaringType.Name.Length, FilePath = f, Line = 3 }));

            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderEditorHideMethodsAttribute()));

            return attrs;
        }
    }
}
