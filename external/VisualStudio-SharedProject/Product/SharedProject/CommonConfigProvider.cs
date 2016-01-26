/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * vspython@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudioTools.Project {
    /// <summary>
    /// Enables the Any CPU, x86, x64, and ARM Platform form names for Dynamic Projects.
    /// Hooks language specific project config.
    /// Projects that are platform aware should have a PlatformAware and AvailablePlatforms
    /// property for configuration handling to function correctly.
    /// PlatformAware value can be true or false. AvailablePlatforms is a comma separated string of supported platforms (e.g. "x86, X64")
    /// If the PlatformAware property is ommited then this provider will only provide "Any CPU" platform.
    /// </summary>
    internal class CommonConfigProvider : ConfigProvider {
        private CommonProjectNode _project;
        private bool _isPlatformAware;

        public CommonConfigProvider(CommonProjectNode project)
            : base(project) {
            bool appxPackage, windowsAppContainer;
            _project = project;
            bool.TryParse(this.ProjectMgr.BuildProject.GetPropertyValue(ProjectFileConstants.PlatformAware), out _isPlatformAware);

            if (!_isPlatformAware) {
                bool.TryParse(this.ProjectMgr.BuildProject.GetPropertyValue(ProjectFileConstants.AppxPackage), out appxPackage);
                bool.TryParse(this.ProjectMgr.BuildProject.GetPropertyValue(ProjectFileConstants.WindowsAppContainer), out windowsAppContainer);
                _isPlatformAware = appxPackage && windowsAppContainer;
            }
        }

        #region overridden methods

        protected override ProjectConfig CreateProjectConfiguration(string configName) {     
            if (_isPlatformAware) {
                if (configName != null) {
                    var configParts = configName.Split('|');

                    if (configParts.Length == 2) {
                        var config = _project.MakeConfiguration(configName);
                        config.PlatformName = configParts[1];
                        return config;
                    }
                }
            }

            return _project.MakeConfiguration(configName);
        }

        public override int GetPlatformNames(uint celt, string[] names, uint[] actual) {
            if (_isPlatformAware) {
                var platforms = GetSupportedPlatformsFromProject();
                return GetPlatforms(celt, names, actual, platforms);
            }
            else {
                if (names != null) {
                    names[0] = AnyCPUPlatform;
                }

                if (actual != null) {
                    actual[0] = 1;
                }

                return VSConstants.S_OK;
            }
        }

        public override int GetSupportedPlatformNames(uint celt, string[] names, uint[] actual) {
            if (_isPlatformAware) {
                var platforms = GetSupportedPlatformsFromProject();
                return GetPlatforms(celt, names, actual, platforms);
            }
            else {
                if (names != null) {
                    names[0] = AnyCPUPlatform;
                }

                if (actual != null) {
                    actual[0] = 1;
                }

                return VSConstants.S_OK;
            }
        }

        public override int GetCfgs(uint celt, IVsCfg[] a, uint[] actual, uint[] flags) {
            if (_isPlatformAware) {
                if (flags != null) {
                    flags[0] = 0;
                }

                int i = 0;
                string[] configList = GetPropertiesConditionedOn(ProjectFileConstants.Configuration);
                string[] platformList = GetSupportedPlatformsFromProject();

                if (a != null) {
                    foreach (string platformName in platformList) {
                        foreach (string configName in configList) {
                            a[i] = this.GetProjectConfiguration(configName + "|" + platformName);
                            i++;
                            if (i == celt) {
                                break;
                            }
                        }
                        if(i == celt) {
                            break;
                        }
                    }
                }
                else {
                    i = configList.Length * platformList.Length;
                }

                if (actual != null) {
                    actual[0] = (uint)i;
        }

                return VSConstants.S_OK;
            }

            return base.GetCfgs(celt, a, actual, flags);
            }

        public override int GetCfgOfName(string name, string platName, out IVsCfg cfg) {
            if (!string.IsNullOrEmpty(platName)) {
                cfg = this.GetProjectConfiguration(name + "|" + platName);

                return VSConstants.S_OK;
            }
            cfg = this.GetProjectConfiguration(name);

            return VSConstants.S_OK;
        }
        #endregion
    }
}
