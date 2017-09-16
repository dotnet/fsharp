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
                _GetMethod = new ArtificialMethodInfo("get_" + _Name, _DeclaringType, _PropertyType, MethodAttributes.Public | MethodAttributes.Static);
            if (CanWrite)
                _SetMethod = new ArtificialMethodInfo("set_" + _Name, _DeclaringType, null /* ?? */, MethodAttributes.Public | MethodAttributes.Static);

        }

        // The name of this property...
        public override string Name
        {
            get
            {
                
                return _Name;
            }
        }

        // Needed
        public override bool CanRead
        {
            get
            {
                
                return _CanRead;
            }
        }

        // If CanRead is true, this one gets invoked.
        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            
            Debug.Assert(!nonPublic, "GetGetMethod was invoked with nonPublic=true");
            return _GetMethod;
        }

        // Why is this invoked?
        public override ParameterInfo[] GetIndexParameters()
        {
            
            return new ParameterInfo[] { /* new ArtificialParamInfo(typeof(int), isRetVal: false), new ArtificialParamInfo(typeof(decimal), isRetVal: false) */};
        }

        // If CanRead is false, this one gets invoked... without checking 'CanWrite' (?)
        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            
            return _SetMethod;
        }

        public override bool CanWrite
        {
            get
            {
                
                return _CanWrite;
            }
        }

        // Interestingly enough, this one seems to be invoked only when I hover over the property in the IDE...
        public override Type PropertyType
        {
            get
            {
                
                return _PropertyType;
            }
        }

        // Interestingly enough, this one seems to be invoked only when I hover over the property in the IDE...
        public override Type DeclaringType
        {
            get
            {
                
                return _DeclaringType;
            }
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            Debug.Assert(false, "Why are we calling into GetCustomAttributes()?");
            return null;
        }


        public override PropertyAttributes Attributes
        {
            get
            {
                
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
        {
            
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, System.Globalization.CultureInfo culture)
        {
            
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
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
            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderXmlDocAttribute(string.Format("This is a synthetic *property* created by me for {0}.{1}. Which is used to test the tool tip of the typeprovider Property to check if it shows the right message or not.!", this._DeclaringType.Namespace, this._DeclaringType.Name))));
            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderDefinitionLocationAttribute() { Column = 21, FilePath = "File.fs", Line = 3 }));
            attrs.Add(new Helpers.TypeProviderCustomAttributeData(new TypeProviderEditorHideMethodsAttribute()));
            return attrs;
        }
    }
}
