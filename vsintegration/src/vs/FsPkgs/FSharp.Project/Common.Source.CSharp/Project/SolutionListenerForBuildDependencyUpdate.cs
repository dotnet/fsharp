// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.


#if UNUSED_NESTED_PROJECTS
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Text;
using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using IServiceProvider = System.IServiceProvider;
using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;
using OleConstants = Microsoft.VisualStudio.OLE.Interop.Constants;

namespace Microsoft.VisualStudio.FSharp.ProjectSystem
{
    /// <summary>
    /// The purpose of this class is to set a build dependency from a modeling project to all its sub projects
    /// </summary>
    internal class SolutionListenerForBuildDependencyUpdate : SolutionListener
    {
        public SolutionListenerForBuildDependencyUpdate(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// Update build dependency list if solution is fully loaded
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="added"></param>
        /// <returns></returns>
        public override int OnAfterOpenProject(IVsHierarchy hierarchy, int added)
        {
            // Return from here if we are at load time
            if (added == 0)
            {
                return VSConstants.S_OK;
            }

            IBuildDependencyOnProjectContainer projectNode = hierarchy as IBuildDependencyOnProjectContainer;

            // We will update only nested project types and the BuildNestedProjectsOnBuild flag is set to true
            if (projectNode != null)
            {
                if (projectNode.BuildNestedProjectsOnBuild)
                {
                    // Enum all sub projects and add to dependency list
                    UpdateDependencyListWithSubProjects(projectNode);
                }
            }
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Called at load time when solution has finished opening.
        /// </summary>
        /// <param name="pUnkReserved">reserved</param>
        /// <param name="fNewSolution">true if this is a new solution</param>
        /// <returns></returns>
        public override int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
        {
            // Enum all sub project and add to dependeny list
            UpdateDependencyListWithSubProjects(null);

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Update dependency list
        /// </summary>
        /// <param name="projectNode">Project node to be updated. If null then all ProjectContainer nodes are updated</param>
        private void UpdateDependencyListWithSubProjects(IBuildDependencyOnProjectContainer projectNode)
        {
            if (projectNode != null)
            {
                // Get list of sub projects
                IList<IVsHierarchy> nestedProjectList = projectNode.EnumNestedHierachiesForBuildDependency();
                if (nestedProjectList != null && nestedProjectList.Count > 0)
                {
                    // Loop nested projects and add project dependency (if supported)
                    foreach (IVsHierarchy nestedProject in nestedProjectList)
                    {
                        AddBuildDependenyToNestedProject(projectNode as IBuildDependencyUpdate, nestedProject);
                    }
                }
            }
            else
            {
                // Update all ProjectContainerNode nodes
                List<IBuildDependencyOnProjectContainer> projectList = this.GetListOfProjectContainerNodes();
                if (projectList != null && projectList.Count > 0)
                {
                    foreach (IBuildDependencyOnProjectContainer project in projectList)
                    {
                        UpdateDependencyListWithSubProjects(project);
                    }
                }
            }
        }

        /// <summary>
        /// Enum all projects in the solution and collect all that derives from ProjectContainerNode
        /// </summary>
        /// <returns>List of ProjectContainerNode nodes</returns>
        private List<IBuildDependencyOnProjectContainer> GetListOfProjectContainerNodes()
        {
            List<IBuildDependencyOnProjectContainer> projectList = new List<IBuildDependencyOnProjectContainer>();

            Debug.Assert(this.Solution != null, "IVsSolution object not set on this object");
            if (this.Solution == null)
            {
                // Bad state, so we quit
                return projectList;
            }

            // Enum projects loaded in the solution (normal projects only)
            IEnumHierarchies enumHierarchies = null;
            Guid guid = Guid.Empty;
            __VSENUMPROJFLAGS flags = __VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION;
            ErrorHandler.ThrowOnFailure(this.Solution.GetProjectEnum((uint)flags, ref guid, out enumHierarchies));

            if (enumHierarchies != null)
            {
                // Loop projects found
                IVsHierarchy[] hierarchy = new IVsHierarchy[1];
                uint fetched = 0;
                while (enumHierarchies.Next(1, hierarchy, out fetched) == VSConstants.S_OK && fetched == 1)
                {
                    // If this is a ProjectContainerNode then add to list
                    IBuildDependencyOnProjectContainer projectNode = hierarchy[0] as IBuildDependencyOnProjectContainer;
                    if (projectNode != null)
                    {
                        projectList.Add(projectNode);
                    }
                }
            }

            return projectList;
        }

        /// <summary>
        /// Add build dependency to ProjectContainerNode if IVsBuildDependency is supported by the nested project
        /// </summary>
        /// <param name="projectContainer">Project Container where we should add the build dependency</param>
        /// <param name="nestedProject">Nested project to set a build dependency against</param>
        private void AddBuildDependenyToNestedProject(IBuildDependencyUpdate projectContainer, IVsHierarchy nestedProject)
        {
            // Validate input
            Debug.Assert(projectContainer != null, "Project Container must not be null");
            Debug.Assert(nestedProject != null, "Nested Project must not be null");
            if (projectContainer == null || nestedProject == null)
            {
                // Invalid argument
                return;
            }

            // Create new NestedProjectBuildDependency
            NestedProjectBuildDependency dependency = new NestedProjectBuildDependency(nestedProject);
            projectContainer.AddBuildDependency(dependency);
        }
    }
}
#endif
