// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#if SINGLE_FILE_GENERATOR

namespace Microsoft.VisualStudio.FSharp.ProjectSystem
{
    using System;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    using System.IO;
    using System.Diagnostics;
    using System.Globalization;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.OLE.Interop;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Microsoft.Win32;

    using VSRegistry = Microsoft.VisualStudio.Shell.VSRegistry;

    /// <summary>
    /// Provides implementation IVsSingleFileGeneratorFactory for
    /// </summary>
    internal class SingleFileGeneratorFactory : IVsSingleFileGeneratorFactory
    {
        private class GeneratorMetaData
        {
            private Guid generatorClsid = Guid.Empty;
            private int generatesDesignTimeSource = -1;
            private int generatesSharedDesignTimeSource = -1;
            private int useDesignTimeCompilationFlag = -1;
            object generator = null;

            public GeneratorMetaData()
            {
            }

            /// <summary>
            /// Generator instance
            /// </summary>
            public Object Generator
            {
                get
                {
                    return generator;
                }
                set
                {
                    generator = value;
                }
            }

            /// <summary>
            /// GeneratesDesignTimeSource reg value name under HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\[VsVer]\Generators\[ProjFacGuid]\[GeneratorProgId]
            /// </summary>
            public int GeneratesDesignTimeSource
            {
                get
                {
                    return generatesDesignTimeSource;
                }
                set
                {
                    generatesDesignTimeSource = value;
                }
            }

            /// <summary>
            /// GeneratesSharedDesignTimeSource reg value name under HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\[VsVer]\Generators\[ProjFacGuid]\[GeneratorProgId]
            /// </summary>
            public int GeneratesSharedDesignTimeSource
            {
                get
                {
                    return generatesSharedDesignTimeSource;
                }
                set
                {
                    generatesSharedDesignTimeSource = value;
                }
            }

            /// <summary>
            /// UseDesignTimeCompilationFlag reg value name under HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\[VsVer]\Generators\[ProjFacGuid]\[GeneratorProgId]
            /// </summary>
            public int UseDesignTimeCompilationFlag
            {
                get
                {
                    return useDesignTimeCompilationFlag;
                }
                set
                {
                    useDesignTimeCompilationFlag = value;
                }
            }

            /// <summary>
            /// Generator Class ID.
            /// </summary>
            public Guid GeneratorClsid
            {
                get
                {
                    return generatorClsid;
                }
                set
                {
                    generatorClsid = value;
                }
            }
        }

        /// <summary>
        /// Base generator registry key for MPF based project
        /// </summary>
        private RegistryKey baseGeneratorRegistryKey = null;

        /// <summary>
        /// CLSID reg value name under HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\[VsVer]\Generators\[ProjFacGuid]\[GeneratorProgId]
        /// </summary>
        private string GeneratorClsid = "CLSID";

        /// <summary>
        /// GeneratesDesignTimeSource reg value name under HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\[VsVer]\Generators\[ProjFacGuid]\[GeneratorProgId]
        /// </summary>
        private string GeneratesDesignTimeSource = "GeneratesDesignTimeSource";

        /// <summary>
        /// GeneratesSharedDesignTimeSource reg value name under HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\[VsVer]\Generators\[ProjFacGuid]\[GeneratorProgId]
        /// </summary>
        private string GeneratesSharedDesignTimeSource = "GeneratesSharedDesignTimeSource";

        /// <summary>
        /// UseDesignTimeCompilationFlag reg value name under HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\[VsVer]\Generators\[ProjFacGuid]\[GeneratorProgId]
        /// </summary>
        private string UseDesignTimeCompilationFlag = "UseDesignTimeCompilationFlag";

        /// <summary>
        /// Caches all the generators registered for the project type.
        /// </summary>
        private Dictionary<string, GeneratorMetaData> generatorsMap = new Dictionary<string, GeneratorMetaData>();

        /// <summary>
        /// The project type guid of the associated project.
        /// </summary>
        private Guid projectType;

        /// <summary>
        /// A service provider
        /// </summary>
        private System.IServiceProvider serviceProvider;

        /// <summary>
        /// Constructor for SingleFileGeneratorFactory
        /// </summary>
        /// <param name="projectGuid">The project type guid of the associated project.</param>
        /// <param name="serviceProvider">A service provider.</param>
        public SingleFileGeneratorFactory(Guid projectType, System.IServiceProvider serviceProvider)
        {
            this.projectType = projectType;
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Defines the project type guid of the associated project.
        /// </summary>
        public Guid ProjectGuid
        {
            get { return this.projectType; }
            set { this.projectType = value; }
        }

        /// <summary>
        /// Defines an associated service provider.
        /// </summary>
        public System.IServiceProvider ServiceProvider
        {
            get { return this.serviceProvider; }
            set { this.serviceProvider = value; }
        }

        /// <summary>
        /// Returns the project generator key under [VS-ConfigurationRoot]]\Generators
        /// </summary>
        private RegistryKey BaseGeneratorsKey
        {
            get
            {
                if (this.baseGeneratorRegistryKey == null)
                {
                    using (RegistryKey root = VSRegistry.RegistryRoot(__VsLocalRegistryType.RegType_Configuration))
                    {
                        if (null != root)
                        {
                            string regPath = "Generators\\" + this.ProjectGuid.ToString("B");
                            baseGeneratorRegistryKey = root.OpenSubKey(regPath);
                        }
                    }
                }

                return this.baseGeneratorRegistryKey;
            }
        }

        /// <summary>
        /// Returns the local registry instance
        /// </summary>
        private ILocalRegistry LocalRegistry
        {
            get
            {
                return this.serviceProvider.GetService(typeof(SLocalRegistry)) as ILocalRegistry;
            }
        }

        /// <summary>
        /// Creates an instance of the single file generator requested
        /// </summary>
        /// <param name="progId">prog id of the generator to be created. For e.g HKLM\SOFTWARE\Microsoft\VisualStudio\9.0Exp\Generators\[prjfacguid]\[wszProgId]</param>
        /// <param name="generatesDesignTimeSource">GeneratesDesignTimeSource key value</param>
        /// <param name="generatesSharedDesignTimeSource">GeneratesSharedDesignTimeSource key value</param>
        /// <param name="useTempPEFlag">UseDesignTimeCompilationFlag key value</param>
        /// <param name="generate">IVsSingleFileGenerator interface</param>
        /// <returns>S_OK if succesful</returns>
        public virtual int CreateGeneratorInstance(string progId, out int generatesDesignTimeSource, out int generatesSharedDesignTimeSource, out int useTempPEFlag, out IVsSingleFileGenerator generate)
        {
            Guid genGuid;
            ErrorHandler.ThrowOnFailure(this.GetGeneratorInformation(progId, out generatesDesignTimeSource, out generatesSharedDesignTimeSource, out useTempPEFlag, out genGuid));

            //Create the single file generator and pass it out. Check to see if it is in the cache
            if (!this.generatorsMap.ContainsKey(progId) || ((this.generatorsMap[progId]).Generator == null))
            {
                Guid riid = VSConstants.IID_IUnknown;
                uint dwClsCtx = (uint)CLSCTX.CLSCTX_INPROC_SERVER;
                IntPtr genIUnknown = IntPtr.Zero;
                //create a new one.
                ErrorHandler.ThrowOnFailure(this.LocalRegistry.CreateInstance(genGuid, null, ref riid, dwClsCtx, out genIUnknown));
                if (genIUnknown != IntPtr.Zero)
                {
                    try
                    {
                        object generator = Marshal.GetObjectForIUnknown(genIUnknown);
                        //Build the generator meta data object and cache it.
                        GeneratorMetaData genData = new GeneratorMetaData();
                        genData.GeneratesDesignTimeSource = generatesDesignTimeSource;
                        genData.GeneratesSharedDesignTimeSource = generatesSharedDesignTimeSource;
                        genData.UseDesignTimeCompilationFlag = useTempPEFlag;
                        genData.GeneratorClsid = genGuid;
                        genData.Generator = generator;
                        this.generatorsMap[progId] = genData;
                    }
                    finally
                    {
                        Marshal.Release(genIUnknown);
                    }
                }
            }

            generate = (this.generatorsMap[progId]).Generator as IVsSingleFileGenerator;

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Gets the default generator based on the file extension. HKLM\Software\Microsoft\VS\9.0\Generators\[prjfacguid]\.extension
        /// </summary>
        /// <param name="filename">File name with extension</param>
        /// <param name="progID">The generator prog ID</param>
        /// <returns>S_OK if successful</returns>
        public virtual int GetDefaultGenerator(string filename, out string progID)
        {
            progID = "";
            return VSConstants.E_NOTIMPL;
        }

        /// <summary>
        /// Gets the generator information.
        /// </summary>
        /// <param name="progId">prog id of the generator to be created. For e.g HKLM\SOFTWARE\Microsoft\VisualStudio\9.0Exp\Generators\[prjfacguid]\[wszProgId]</param>
        /// <param name="generatesDesignTimeSource">GeneratesDesignTimeSource key value</param>
        /// <param name="generatesSharedDesignTimeSource">GeneratesSharedDesignTimeSource key value</param>
        /// <param name="useTempPEFlag">UseDesignTimeCompilationFlag key value</param>
        /// <param name="guiddGenerator">CLSID key value</param>
        /// <returns>S_OK if succesful</returns>
        public virtual int GetGeneratorInformation(string progId, out int generatesDesignTimeSource, out int generatesSharedDesignTimeSource, out int useTempPEFlag, out Guid guidGenerator)
        {
            RegistryKey genKey;
            generatesDesignTimeSource = -1;
            generatesSharedDesignTimeSource = -1;
            useTempPEFlag = -1;
            guidGenerator = Guid.Empty;
            if (string.IsNullOrEmpty(progId))
                return VSConstants.S_FALSE;

            //Create the single file generator and pass it out.
            if (!this.generatorsMap.ContainsKey(progId))
            {
                // We have to check whether the BaseGeneratorkey returns null.
                RegistryKey tempBaseGeneratorKey = this.BaseGeneratorsKey;
                if (tempBaseGeneratorKey == null || (genKey = tempBaseGeneratorKey.OpenSubKey(progId)) == null)
                {
                    return VSConstants.S_FALSE;
                }

                //Get the CLSID
                string guid = (string)genKey.GetValue(GeneratorClsid, "");
                if (string.IsNullOrEmpty(guid))
                    return VSConstants.S_FALSE;

                GeneratorMetaData genData = new GeneratorMetaData();

                genData.GeneratorClsid = guidGenerator = new Guid(guid);
                //Get the GeneratesDesignTimeSource flag. Assume 0 if not present.
                genData.GeneratesDesignTimeSource = generatesDesignTimeSource = (int)genKey.GetValue(this.GeneratesDesignTimeSource, 0);
                //Get the GeneratesSharedDesignTimeSource flag. Assume 0 if not present.
                genData.GeneratesSharedDesignTimeSource = generatesSharedDesignTimeSource = (int)genKey.GetValue(GeneratesSharedDesignTimeSource, 0);
                //Get the UseDesignTimeCompilationFlag flag. Assume 0 if not present.
                genData.UseDesignTimeCompilationFlag = useTempPEFlag = (int)genKey.GetValue(UseDesignTimeCompilationFlag, 0);
                this.generatorsMap.Add(progId, genData);
            }
            else
            {
                GeneratorMetaData genData = this.generatorsMap[progId];
                generatesDesignTimeSource = genData.GeneratesDesignTimeSource;
                //Get the GeneratesSharedDesignTimeSource flag. Assume 0 if not present.
                generatesSharedDesignTimeSource = genData.GeneratesSharedDesignTimeSource;
                //Get the UseDesignTimeCompilationFlag flag. Assume 0 if not present.
                useTempPEFlag = genData.UseDesignTimeCompilationFlag;
                //Get the CLSID
                guidGenerator = genData.GeneratorClsid;
            }

            return VSConstants.S_OK;
        }
    }
}
#endif
