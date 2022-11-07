// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.FSharp.Core.CompilerServices;

namespace TypeProviderInCSharp
{
    static class Helpers
    {
        internal class TypeProviderCustomAttributeData : CustomAttributeData
        {
            protected readonly Attribute _a;
            public TypeProviderCustomAttributeData(Attribute a)
            {
                _a = a;
            }

            public override ConstructorInfo Constructor
            {
                get
                {
                    return _a.GetType().GetConstructors()[0];
                }
            }

            public override IList<CustomAttributeTypedArgument> ConstructorArguments
            {
                get
                {
                    return new List<CustomAttributeTypedArgument>();
                }
            }

            public override IList<CustomAttributeNamedArgument> NamedArguments
            {
                get
                {
                    return new List<CustomAttributeNamedArgument>();
                }
            }
        }

    }
}
