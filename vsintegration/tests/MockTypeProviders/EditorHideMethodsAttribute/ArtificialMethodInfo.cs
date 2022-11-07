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
    class ArtificialMethodInfo : MethodInfo
    {
        string _Name;
        Type _DeclaringType;
        Type _ReturnType;
        MethodAttributes _MethodAttributes;

        public ArtificialMethodInfo(string Name, Type DeclaringType, Type ReturnType, MethodAttributes MethodAttributes)
        {
            _Name = Name;
            _DeclaringType = DeclaringType;
            _ReturnType = ReturnType;
            _MethodAttributes = MethodAttributes;
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

        // Make the method Public and Static - 
        // TODO: should be configurable in the ctor...
        public override MethodAttributes Attributes
        {
            get
            {
                
                return _MethodAttributes;
            }
        }

        // No params
        // TODO: should be configurable in the ctor...
        public override ParameterInfo[] GetParameters()
        {
            
            return new ParameterInfo[] {  };
        }

        public override ParameterInfo ReturnParameter
        {
            get
            {
                //
                //var retvalpi = new ArtificialParamInfo(typeof(List<>), true);
                //return retvalpi;
                
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();

            }
        }

        public override Type ReturnType
        {
            get
            {
                
                return _ReturnType;
            }
        }

        public override MethodInfo GetBaseDefinition()
        {
            
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override ICustomAttributeProvider ReturnTypeCustomAttributes
        {
            get 
            {
                
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
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
            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderXmlDocAttribute("This is a synthetic *method* created by me!!")));
            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderDefinitionLocationAttribute() { Column = 21, FilePath = "File.fs", Line = 3 }));
            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderEditorHideMethodsAttribute()));
            return attrs;
        }

    }
}
