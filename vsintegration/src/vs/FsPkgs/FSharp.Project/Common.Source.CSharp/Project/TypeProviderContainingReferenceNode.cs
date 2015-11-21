// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.FSharp.ProjectSystem
{
    [CLSCompliant(false), ComVisible(true)]
    public abstract class TypeProviderContainingReferenceNode : ReferenceNode
    {
        private bool containsTypeProvider;
        
        internal TypeProviderContainingReferenceNode(ProjectNode root, ProjectElement element)
            : base(root, element)
        {
            this.containsTypeProvider = false;
        }

        internal TypeProviderContainingReferenceNode(ProjectNode root)
            : base(root)
        {
            this.containsTypeProvider = false;
        }
        
        public bool ContainsTypeProvider
        {
            get { return containsTypeProvider; }
            set
            {
                if (containsTypeProvider != value)
                {
                    containsTypeProvider = value;
                    this.ReDraw(UIHierarchyElement.Icon);
                }
            }
        }
        
        public abstract string AssemblyPath
        {
            get;
        }

        public override int ImageIndex
        {
            get
            {
                const int typeProviderAssemblyIcon = (int) ProjectNode.ImageName.TypeProviderAssembly;
                return ContainsTypeProvider ? typeProviderAssemblyIcon : base.ImageIndex;
            }
        }
    }
}
