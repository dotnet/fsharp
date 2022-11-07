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
                return _DeclaringType;
            }
        }

        // This one is invoked
        public override ParameterInfo[] GetParameters()
        {
            return _ParameterInfo;
        }

        // This one is indeed invoked
        // I believe we should always return ".ctor"
        public override string Name
        {
            get
            {
                return ".ctor";
            }
        }

        // Does it matter what we return here?
        // This property is definitely checked by the compiler in code like this:
        // let _ = new N.T()
        // I copied the attribute set from the .ctor of System.DateTime - the documentation on MSDN assumes that one is already familiar with
        // what they mean (=totally useless, as often happens)
        public override MethodAttributes Attributes
        {
            get
            {
                return MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
            }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            Debug.Assert(false, "Why are we calling into GetCustomAttributes()?");
            return null;
        }

        public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
        {
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, System.Globalization.CultureInfo culture)
        {
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override RuntimeMethodHandle MethodHandle
        {
            get
            {
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            Debug.Assert(false, "Why are we calling into GetCustomAttributes()?");
            return null;
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
            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderXmlDocAttribute(string.Format("This is a synthetic .ctor created by me for {0}.{1}", this._DeclaringType.Namespace, this._DeclaringType.Name))));
            // attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderEditorHideMethodsAttribute()));
            return attrs;
        }
    }
}
