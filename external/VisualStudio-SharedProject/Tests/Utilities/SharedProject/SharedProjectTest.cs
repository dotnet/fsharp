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
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Build.Construction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudioTools;

namespace TestUtilities.SharedProject {
    /// <summary>
    /// Base class for all test cases which generate projects at runtime in a language
    /// agnostic way.  This class will initialize the MEF catalog and get the various
    /// project kind(s) to be tested.  The kinds wil be available via the ProjectKinds
    /// property.
    /// 
    /// It also provides a number of convenience methods for creating project definitions.
    /// This helps to make the project definition more readable and similar to typical
    /// MSBuild structure.
    /// </summary>
    [TestClass]
    public class SharedProjectTest {
        public static CompositionContainer Container;
        public static IEnumerable<ProjectType> ProjectTypes { get; set; }

        static SharedProjectTest() {
            var runningLoc = Path.GetDirectoryName(typeof(SharedProjectTest).Assembly.Location);
            // we want to pick up all of the MEF exports which are available, but they don't
            // depend upon us.  So if we're just running some tests in the IDE when the deployment
            // happens it won't have the DLLS with the MEF exports.  So we copy them here.
            TestData.Deploy(null, includeTestData: false);

            // load all of the available DLLs that depend upon TestUtilities into our catalog
            List<AssemblyCatalog> catalogs = new List<AssemblyCatalog>();
            foreach (var file in Directory.GetFiles(runningLoc, "*.dll")) {
                TryAddAssembly(catalogs, file);
            }

            // Compose everything
            var catalog = new AggregateCatalog(catalogs.ToArray());

            var container = Container = new CompositionContainer(catalog);
            var compBatch = new CompositionBatch();
            container.Compose(compBatch);

            // Initialize our ProjectTypes information from the catalog.            

            // First, get a mapping from extension type to all available IProjectProcessor's for
            // that extension
            var processorsMap = container
                .GetExports<IProjectProcessor, IProjectProcessorMetadata>()
                .GroupBy(x => x.Metadata.ProjectExtension)
                .ToDictionary(
                    x => x.Key, 
                    x => x.Select(lazy => lazy.Value).ToArray(), 
                    StringComparer.OrdinalIgnoreCase
                );

            // Then create the ProjectTypes
            ProjectTypes = container
                .GetExports<ProjectTypeDefinition, IProjectTypeDefinitionMetadata>()
                .Select(lazyVal => {
                    var md = lazyVal.Metadata;
                    IProjectProcessor[] processors;
                    processorsMap.TryGetValue(md.ProjectExtension, out processors);

                    return new ProjectType(
                        md.CodeExtension,
                        md.ProjectExtension,
                        Guid.Parse(md.ProjectTypeGuid),
                        md.SampleCode,
                        processors
                    );
                });

            // something's broken if we don't have any languages to test against, so fail the test.
            Assert.IsTrue(ProjectTypes.Count() > 0, "no project types were registered and no tests will run");
        }

        private static void TryAddAssembly(List<AssemblyCatalog> catalogs, string file) {
            Assembly asm;
            try {
                asm = Assembly.Load(Path.GetFileNameWithoutExtension(file));
            } catch {
                return;
            }

            // Include any test assemblies which reference this assembly, they might
            // have defined a project kind.
            foreach (var reference in asm.GetReferencedAssemblies()) {
                if (reference.FullName == typeof(SharedProjectTest).Assembly.GetName().FullName) {
                    Console.WriteLine("Including {0}", file);
                    catalogs.Add(new AssemblyCatalog(asm));
                    break;
                }
            }
        }

        /// <summary>
        /// Helper function to create a ProjectProperty object to simply syntax in 
        /// project definitions.
        /// </summary>
        public static ProjectProperty Property(string name, string value) {
            return new ProjectProperty(name, value);
        }

        /// <summary>
        /// Helper function to create a StartupFileProjectProperty object to simply syntax in 
        /// project definitions.
        /// </summary>
        public static StartupFileProjectProperty StartupFile(string filename) {
            return new StartupFileProjectProperty(filename);
        }
        
        /// <summary>
        /// Helper function to create a group of properties when creating project definitions.
        /// These aren't strictly necessary and just serve to add structure to the code
        /// and make it similar to an MSBuild project file.
        /// </summary>
        public static ProjectContentGroup PropertyGroup(params ProjectProperty[] properties) {
            return new ProjectContentGroup(properties);
        }

        /// <summary>
        /// Helper function to create a CompileItem object to simply syntax in 
        /// defining project definitions.
        /// </summary>
        public static CompileItem Compile(string name, string content = null, bool isExcluded = false, bool isMissing = false) {
            return new CompileItem(name, content, isExcluded, isMissing);
        }

        /// <summary>
        /// Helper function to create a CompileItem object to simply syntax in 
        /// defining project definitions.
        /// </summary>
        public static ContentItem Content(string name, string content, bool isExcluded = false) {
            return new ContentItem(name, content, isExcluded);
        }

        /// <summary>
        /// Helper function to create a SymbolicLinkItem object to simply syntax in 
        /// defining project definitions.
        /// </summary>
        public static SymbolicLinkItem SymbolicLink(string name, string referencePath, bool isExcluded = false, bool isMissing = false) {
            return new SymbolicLinkItem(name, referencePath, isExcluded, isMissing);
        }

        /// <summary>
        /// Helper function to create a FolderItem object to simply syntax in 
        /// defining project definitions.
        /// </summary>
        public static FolderItem Folder(string name, bool isExcluded = false, bool isMissing = false) {
            return new FolderItem(name, isExcluded, isMissing);
        }

        /// <summary>
        /// Helper function to create a CustomItem object which is an MSBuild item with
        /// the specified item type.
        /// </summary>
        public static CustomItem CustomItem(string itemType, string name, string content = null, bool isExcluded = false, bool isMissing = false, IEnumerable<KeyValuePair<string, string>> metadata = null) {
            return new CustomItem(itemType, name, content, isExcluded, isMissing, metadata);
        }

        /// <summary>
        /// Helper function to create a group of items when creating project definitions.
        /// These aren't strictly necessary and just serve to add structure to the code
        /// and make it similar to an MSBuild project file.
        /// </summary>
        public static ProjectContentGroup ItemGroup(params ProjectContentGenerator[] properties) {
            return new ProjectContentGroup(properties);
        }

        /// <summary>
        /// Returns a new SolutionFolder object which can be used to create
        /// a solution folder in the generated project.
        /// </summary>
        public static SolutionFolder SolutionFolder(string name) {
            return new SolutionFolder(name);
        }

        /// <summary>
        /// Returns a new TargetDefinition which represents a specified Target
        /// inside of the project file.  The various stages of the target can be
        /// created using the members of the static Tasks class.
        /// </summary>
        public static TargetDefinition Target(string name, params Action<ProjectTargetElement>[] creators) {
            return new TargetDefinition(name, creators);
        }

        /// <summary>
        /// Returns a new TargetDefinition which represents a specified Target
        /// inside of the project file.  The various stages of the target can be
        /// created using the members of the static Tasks class.
        /// </summary>
        public static TargetDefinition Target(string name, string dependsOnTargets, params Action<ProjectTargetElement>[] creators) {
            return new TargetDefinition(name, creators) { DependsOnTargets = dependsOnTargets };
        }

        /// <summary>
        /// Returns a new ImportDefinition for the specified project.
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static ImportDefinition Import(string project) {
            return new ImportDefinition(project);
        }

        /// <summary>
        /// Provides tasks for creating target definitions in generated projects.
        /// </summary>
        public static class Tasks {
            /// <summary>
            /// Creates a task which outputs a message during the build.
            /// </summary>
            public static Action<ProjectTargetElement> Message(string message, string importance = null) {
                return target => {
                    var messageTask = target.AddTask("Message");
                    messageTask.SetParameter("Text", message);
                    if (importance != null) {
                        messageTask.SetParameter("Importance", importance);
                    }
                };
            }
        }
    }
}
