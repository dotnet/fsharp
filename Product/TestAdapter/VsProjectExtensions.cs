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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Microsoft.VisualStudioTools.TestAdapter {
    internal static class VSProjectExtensions {
        internal static EnvDTE.Project GetProject(this IVsHierarchy hierarchy) {
            object project;

            ErrorHandler.ThrowOnFailure(
                hierarchy.GetProperty(
                    VSConstants.VSITEMID_ROOT,
                    (int)__VSHPROPID.VSHPROPID_ExtObject,
                    out project
                )
            );

            return (project as EnvDTE.Project);
        }

        /// <summary>
        /// Gets the name of the project.
        /// </summary>
        public static string GetProjectName(this IVsProject project) {
            ValidateArg.NotNull(project, "project");

            var projectHierarchy = (IVsHierarchy)project;
            object projectName;
            ErrorHandler.ThrowOnFailure(projectHierarchy.GetProperty((uint)VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_Name, out projectName));
            return (string)projectName;
        }

        public static bool TryGetProjectPath(this IVsProject project, out string path) {
            ValidateArg.NotNull(project, "project");

            return ErrorHandler.Succeeded(project.GetMkDocument(VSConstants.VSITEMID_ROOT, out path));
        }

        private static string GetAggregateProjectTypeGuids(this IVsProject project) {
            ValidateArg.NotNull(project, "project");

            var aggregatableProject = project as IVsAggregatableProject;
            var aggregatableProjectCorrected = project as IVsAggregatableProjectCorrected;

            // Then it is not an aggregated project
            if (aggregatableProject == null && aggregatableProjectCorrected == null) {
                return string.Empty;
            }

            var projectTypeGuids = string.Empty;

            if (aggregatableProject != null) {
                ErrorHandler.ThrowOnFailure(aggregatableProject.GetAggregateProjectTypeGuids(out projectTypeGuids));
            } else if (aggregatableProjectCorrected != null) {
                ErrorHandler.ThrowOnFailure(aggregatableProjectCorrected.GetAggregateProjectTypeGuids(out projectTypeGuids));
            }

            return projectTypeGuids;
        }

        /// <summary>
        /// Returns whether the parameter project is a test project or not. 
        /// </summary>
        public static bool IsTestProject(this IVsProject project, Guid projectGuid) {
            ValidateArg.NotNull(project, "project");

            string projectTypeGuids = project.GetAggregateProjectTypeGuids();

            // Currently we assume that all matching projects are test projects.
            return (projectTypeGuids.IndexOf(projectGuid.ToString(), StringComparison.OrdinalIgnoreCase) >= 0);
        }

        /// <summary>
        /// Gets the project home directory.
        /// </summary>
        public static string GetProjectHome(this IVsProject project) {
            Debug.Assert(project != null);
            var hier = (IVsHierarchy)project;
            object extObject;
            ErrorHandler.ThrowOnFailure(hier.GetProperty(
                (uint)VSConstants.VSITEMID.Root,
                (int)__VSHPROPID.VSHPROPID_ExtObject,
                out extObject
            ));
            var proj = extObject as EnvDTE.Project;
            if (proj == null) {
                return null;
            }
            var props = proj.Properties;
            if (props == null) {
                return null;
            }
            var projHome = props.Item("ProjectHome");
            if (projHome == null) {
                return null;
            }

            return projHome.Value as string;
        }

        /// <summary>
        /// Gets the file paths of items in the project.
        /// </summary>
        public static IEnumerable<string> GetProjectItemPaths(this IVsProject project) {
            string path;
            ErrorHandler.ThrowOnFailure(project.GetMkDocument(VSConstants.VSITEMID_ROOT, out path));
            if (string.IsNullOrEmpty(path)) {
                yield break;
            }

            yield return path;
            foreach (var filePath in project.GetProjectItems()) {
                if (!string.IsNullOrEmpty(filePath)) {
                    yield return filePath;
                }
            }
        }

        /// <summary>
        /// Get the items present in the project
        /// </summary>
        public static IEnumerable<string> GetProjectItems(this IVsProject project) {
            if (project == null) {
                throw new ArgumentNullException("project");
            }

            // Each item in VS OM is IVSHierarchy. 
            IVsHierarchy hierarchy = (IVsHierarchy)project;

            return GetProjectItems(hierarchy, VSConstants.VSITEMID_ROOT);
        }

        /// <summary>
        /// Get project items
        /// </summary>
        private static IEnumerable<string> GetProjectItems(IVsHierarchy project, uint itemId) {
            for (var childId = GetItemId(GetPropertyValue((int)__VSHPROPID.VSHPROPID_FirstChild, itemId, project));
                childId != VSConstants.VSITEMID_NIL;
                childId = GetItemId(GetPropertyValue((int)__VSHPROPID.VSHPROPID_NextSibling, childId, project))) {

                if ((GetPropertyValue((int)__VSHPROPID.VSHPROPID_IsNonMemberItem, childId, project) as bool?) ?? false) {
                    continue;
                }

                foreach (string item in GetProjectItems(project, childId)) {
                    yield return item;
                }

                string childPath = GetCanonicalName(childId, project);
                yield return childPath;
            }
        }

        /// <summary>
        /// Convert parameter object to ItemId
        /// </summary>
        private static uint GetItemId(object pvar) {
            if (pvar == null) return VSConstants.VSITEMID_NIL;
            if (pvar is int) return (uint)(int)pvar;
            if (pvar is uint) return (uint)pvar;
            if (pvar is short) return (uint)(short)pvar;
            if (pvar is ushort) return (uint)(ushort)pvar;
            if (pvar is long) return (uint)(long)pvar;
            return VSConstants.VSITEMID_NIL;
        }

        /// <summary>
        /// Get the parameter property value
        /// </summary>
        private static object GetPropertyValue(int propid, uint itemId, IVsHierarchy vsHierarchy) {
            if (itemId == VSConstants.VSITEMID_NIL) {
                return null;
            }

            object o;
            if (ErrorHandler.Failed(vsHierarchy.GetProperty(itemId, propid, out o))) {
                return null;
            }
            return o;
        }


        /// <summary>
        /// Get the canonical name
        /// </summary>
        private static string GetCanonicalName(uint itemId, IVsHierarchy hierarchy) {
            Debug.Assert(itemId != VSConstants.VSITEMID_NIL, "ItemId cannot be nill");

            string strRet = string.Empty;
            if (ErrorHandler.Failed(hierarchy.GetCanonicalName(itemId, out strRet))) {
                return string.Empty;
            }
            return strRet;
        }
    }
}
