// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.FSharp.Core.CompilerServices;

namespace TypeProviderInCSharp
{
    public class ArtificialParamInfo : ParameterInfo
    {
        Type _type;
        bool _isRetVal;

        public ArtificialParamInfo(Type type, bool isRetVal)
        {
            _type = type;
            _isRetVal = IsRetval;
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

        public override object DefaultValue
        {
            get
            {
                
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override bool Equals(object obj)
        {
            
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            Debug.Assert(false, "Why are we calling into GetCustomAttributes()?");
            return null;
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            Debug.Assert(false, "Why are we calling into GetCustomAttributes()?");
            return null;
        }

        public override int GetHashCode()
        {
            
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override Type[] GetOptionalCustomModifiers()
        {
            
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override Type[] GetRequiredCustomModifiers()
        {
            
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }

        public override MemberInfo Member
        {
            get
            {
                
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override int MetadataToken
        {
            get
            {
                
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override string Name
        {
            get
            {
                
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override int Position
        {
            get
            {
                
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override object RawDefaultValue
        {
            get
            {
                
                Debug.Assert(false, "NYI");
                throw new NotImplementedException();
            }
        }

        public override string ToString()
        {
            
            Debug.Assert(false, "NYI");
            throw new NotImplementedException();
        }
    }

}
