// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
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

            _AddMethod = new ArtificialMethodInfo("add_" + _Name, _DeclaringType, typeof(void), MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Public);
            _RemoveMethod = new ArtificialMethodInfo("remove_" + _Name, _DeclaringType, typeof(void), MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Public);
        }

        // This one is invoked
        public override Type EventHandlerType
        {
            get
            {
                
                return _EventHandleType;
            }
        }

        public override string Name
        {
            get 
            { 
                
                return _Name;
            }
        }

        public override Type DeclaringType
        {
            get
            {
                
                return _DeclaringType;
            }
        }

        // This one is needed
        public override MethodInfo GetAddMethod(bool nonPublic)
        {
            
            Debug.Assert(!nonPublic, "GetAddMethod() was called with nonPublic=true");
            return _AddMethod;
        }

        // This one is needed
        public override MethodInfo GetRemoveMethod(bool nonPublic)
        {
            
            Debug.Assert(!nonPublic, "GetRemoveMethod() was called with nonPublic=true");
            return _RemoveMethod;
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            Debug.Assert(false, "Why are we calling into GetCustomAttributes()?");
            return null;
        }

        public override EventAttributes Attributes
        {
            get
            {
                
                return EventAttributes.None;
            }
        }

        public override MethodInfo GetRaiseMethod(bool nonPublic)
        {
            Debug.Assert(false, "Why are we calling into GetCustomAttributes()?");
            return null;
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override Type ReflectedType
        {
            get
            {
                
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            var attrs = new List<CustomAttributeData>();
            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderXmlDocAttribute(string.Format(""))));
            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderDefinitionLocationAttribute() { Column = 5 + this._DeclaringType.Name.Length, FilePath = "", Line = 3 }));
            // attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderEditorHideMethodsAttribute()));
            return attrs;
        }

    }
}
