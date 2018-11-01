// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.FSharp.ProjectSystem
{
    [CLSCompliant(false), ComVisible(true)]
    public class PackageReferenceNode : ReferenceNode
    {
        private string referencedPackageName = String.Empty;
        private string referencedPackageVersion = String.Empty;

        internal PackageReferenceNode(ProjectNode root, ProjectElement element)
            : base(root, element)
        {
            this.referencedPackageName = this.ItemNode.GetEvaluatedMetadata(ProjectFileConstants.Include);
            this.referencedPackageVersion = this.ItemNode.GetEvaluatedMetadata(ProjectFileConstants.Version);
        }

        public override void BindReferenceData()
        {
            Debug.Assert(!String.IsNullOrEmpty(this.referencedPackageName), "The referencedPackageName field has not been initialized");

            this.ItemNode = new ProjectElement(this.ProjectMgr, "", ProjectFileConstants.PackageReference);

            this.ItemNode.SetMetadata(ProjectFileConstants.Name, this.referencedPackageName);
            this.ItemNode.SetMetadata(ProjectFileConstants.Version, this.referencedPackageVersion);
        }

        public override object GetIconHandle(bool open)
        {
            return this.ProjectMgr.ImageHandler.GetIconHandle((int)ProjectNode.ImageName.NuGetPackage);
        }

        public string ReferencedPackageName
        {
            get
            {
                return this.referencedPackageName;
            }
        }

        public override string SimpleName
        {
            get
            {
                return this.referencedPackageName;
            }
        }

        public override string Caption
        {
            get
            {
                return this.referencedPackageName;
            }
        }

        public string Version
        {
            get
            {
                return this.referencedPackageVersion;
            }
        }

        public override NodeProperties CreatePropertiesObject()
        {
            return new PackageReferencesProperties(this);
        }

        private Automation.OAPackageReference packageReference;
        public override object Object
        {
            get
            {
                if (null == packageReference)
                {
                    packageReference = new Automation.OAPackageReference(this);
                }
                return packageReference;
            }
        }
    }
}
