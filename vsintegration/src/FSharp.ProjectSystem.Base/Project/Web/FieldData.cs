// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Globalization;
using System.Reflection;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Designer.Interfaces;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Design;
using Microsoft.VisualStudio.Shell.Design.Serialization;

using IServiceProvider = System.IServiceProvider;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

namespace Microsoft.VisualStudio.FSharp.ProjectSystem.Web
{
    internal class FieldData
    {
        private string _name;
        private string _class;
        private int    _depth;
        private bool   _isPrivate;
        private bool   _isProtected;
        private bool   _isPublic;

        public FieldData(CodeClass codeClass, CodeVariable codeVariable, int depth)
        {
             _name = codeVariable.Name;
             _class = codeClass.FullName;
             _depth = depth;
             vsCMAccess access = codeVariable.Access;
             _isPrivate = ((access & vsCMAccess.vsCMAccessPrivate) == vsCMAccess.vsCMAccessPrivate);
             _isProtected = ((access & vsCMAccess.vsCMAccessProtected) == vsCMAccess.vsCMAccessProtected);

             // Special casing vsCMAccessAssemblyOrFamily
             if ((access & vsCMAccess.vsCMAccessAssemblyOrFamily) == vsCMAccess.vsCMAccessAssemblyOrFamily)
                 _isProtected = true;

             _isPublic = ((access & vsCMAccess.vsCMAccessPublic) == vsCMAccess.vsCMAccessPublic);
        }

        public FieldData(string name)
        {
             _name = name;
             _class = null;
             _depth = 0;
             _isPrivate = false;
             _isProtected = false;
             _isPublic = true;
        }

        public bool IsPrivate
        {
            get
            {
                return _isPrivate;
            }
        }

        public bool IsProtected
        {
            get
            {
                return _isProtected;
            }
        }

        public bool IsPublic
        {
            get
            {
                return _isPublic;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public int Depth
        {
            get
            {
                return _depth;
            }
        }

        public string Class
        {
            get
            {
                return _class;
            }
        }
    }

    internal class FieldDataDictionary : Dictionary<string,FieldData>
    {
        public FieldDataDictionary(bool caseSensitive) : base(caseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}
