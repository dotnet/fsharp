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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using IServiceProvider = System.IServiceProvider;
using MSBuild = Microsoft.Build.Evaluation;

namespace Microsoft.VisualStudioTools.Project {
    public static class Utilities {
        private const string defaultMSBuildVersion = "4.0";

        /// <summary>
        /// Look in the registry under the current hive for the path
        /// of MSBuild
        /// </summary>
        /// <returns></returns>

        /// <summary>
        /// Is Visual Studio in design mode.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>true if visual studio is in design mode</returns>
        public static bool IsVisualStudioInDesignMode(IServiceProvider site) {
            Utilities.ArgumentNotNull("site", site);

            IVsMonitorSelection selectionMonitor = site.GetService(typeof(IVsMonitorSelection)) as IVsMonitorSelection;
            uint cookie = 0;
            int active = 0;
            Guid designContext = VSConstants.UICONTEXT_DesignMode;
            ErrorHandler.ThrowOnFailure(selectionMonitor.GetCmdUIContextCookie(ref designContext, out cookie));
            ErrorHandler.ThrowOnFailure(selectionMonitor.IsCmdUIContextActive(cookie, out active));
            return active != 0;
        }

        /// <include file='doc\VsShellUtilities.uex' path='docs/doc[@for="Utilities.IsInAutomationFunction"]/*' />
        /// <devdoc>
        /// Is an extensibility object executing an automation function.
        /// </devdoc>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>true if the extensiblity object is executing an automation function.</returns>
        public static bool IsInAutomationFunction(IServiceProvider serviceProvider) {
            Utilities.ArgumentNotNull("serviceProvider", serviceProvider);

            IVsExtensibility3 extensibility = serviceProvider.GetService(typeof(EnvDTE.IVsExtensibility)) as IVsExtensibility3;

            if (extensibility == null) {
                throw new InvalidOperationException();
            }
            int inAutomation = 0;
            ErrorHandler.ThrowOnFailure(extensibility.IsInAutomationFunction(out inAutomation));
            return inAutomation != 0;
        }

        /// <summary>
        /// Use this instead of VsShellUtilities.ShowMessageBox because VSU uses ThreadHelper which
        /// uses a private interface that can't be mocked AND goes to the global service provider.
        /// </summary>
        public static int ShowMessageBox(IServiceProvider serviceProvider, string message, string title, OLEMSGICON icon, OLEMSGBUTTON msgButton, OLEMSGDEFBUTTON defaultButton) {
            IVsUIShell uiShell = serviceProvider.GetService(typeof(IVsUIShell)) as IVsUIShell;
            Debug.Assert(uiShell != null, "Could not get the IVsUIShell object from the services exposed by this serviceprovider");
            if (uiShell == null) {
                throw new InvalidOperationException();
            }

            Guid emptyGuid = Guid.Empty;
            int result = 0;

            serviceProvider.GetUIThread().Invoke(() => {
                ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                    0,
                    ref emptyGuid,
                    title,
                    message,
                    null,
                    0,
                    msgButton,
                    defaultButton,
                    icon,
                    0,
                    out result));
            });
            return result;
        }

        /// <summary>
        /// Creates a semicolon delinited list of strings. This can be used to provide the properties for VSHPROPID_CfgPropertyPagesCLSIDList, VSHPROPID_PropertyPagesCLSIDList, VSHPROPID_PriorityPropertyPagesCLSIDList
        /// </summary>
        /// <param name="guids">An array of Guids.</param>
        /// <returns>A semicolon delimited string, or null</returns>

        public static string CreateSemicolonDelimitedListOfStringFromGuids(Guid[] guids) {
            if (guids == null || guids.Length == 0) {
                return String.Empty;
            }

            // Create a StringBuilder with a pre-allocated buffer big enough for the
            // final string. 39 is the length of a GUID in the "B" form plus the final ';'
            StringBuilder stringList = new StringBuilder(39 * guids.Length);
            for (int i = 0; i < guids.Length; i++) {
                stringList.Append(guids[i].ToString("B"));
                stringList.Append(";");
            }

            return stringList.ToString().TrimEnd(';');
        }

        private static char[] curlyBraces = new char[] { '{', '}' };
        /// <summary>
        /// Take list of guids as a single string and generate an array of Guids from it
        /// </summary>
        /// <param name="guidList">Semi-colon separated list of Guids</param>
        /// <returns>Array of Guids</returns>

        public static Guid[] GuidsArrayFromSemicolonDelimitedStringOfGuids(string guidList) {
            if (guidList == null) {
                return null;
            }

            List<Guid> guids = new List<Guid>();
            string[] guidsStrings = guidList.Split(';');
            foreach (string guid in guidsStrings) {
                if (!String.IsNullOrEmpty(guid))
                    guids.Add(new Guid(guid.Trim(curlyBraces)));
            }

            return guids.ToArray();
        }

        internal static bool GuidEquals(string x, string y) {
            Guid gx, gy;
            return Guid.TryParse(x, out gx) && Guid.TryParse(y, out gy) && gx == gy;
        }

        internal static bool GuidEquals(Guid x, string y) {
            Guid gy;
            return Guid.TryParse(y, out gy) && x == gy;
        }

        internal static void CheckNotNull(object value, string message = null) {
            if (value == null) {
                throw new InvalidOperationException(message);
            }
        }

        internal static void ArgumentNotNull(string name, object value) {
            if (value == null) {
                throw new ArgumentNullException(name);
            }
        }
        internal static void ArgumentNotNullOrEmpty(string name, string value) {
            if (String.IsNullOrEmpty(value)) {
                throw new ArgumentNullException(name);
            }
        }
        /// <summary>
        /// Validates a file path by validating all file parts. If the 
        /// the file name is invalid it throws an exception if the project is in automation. Otherwise it shows a dialog box with the error message.
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="filePath">A full path to a file name</param>
        /// <exception cref="InvalidOperationException">In case of failure an InvalidOperationException is thrown.</exception>
        public static void ValidateFileName(IServiceProvider serviceProvider, string filePath) {
            string errorMessage = String.Empty;
            if (String.IsNullOrEmpty(filePath)) {
                errorMessage = SR.GetString(SR.ErrorInvalidFileName, filePath);
            } else if (filePath.Length > NativeMethods.MAX_PATH) {
                errorMessage = SR.GetString(SR.PathTooLong, filePath);
            } else if (ContainsInvalidFileNameChars(filePath)) {
                errorMessage = SR.GetString(SR.ErrorInvalidFileName, filePath);
            }

            if (errorMessage.Length == 0) {
                string fileName = Path.GetFileName(filePath);
                if (String.IsNullOrEmpty(fileName) || IsFileNameInvalid(fileName)) {
                    errorMessage = SR.GetString(SR.ErrorInvalidFileName, filePath);
                }
            }

            if (errorMessage.Length > 0) {
                // If it is not called from an automation method show a dialog box.
                if (!Utilities.IsInAutomationFunction(serviceProvider)) {
                    string title = null;
                    OLEMSGICON icon = OLEMSGICON.OLEMSGICON_CRITICAL;
                    OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
                    OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
                    VsShellUtilities.ShowMessageBox(serviceProvider, title, errorMessage, icon, buttons, defaultButton);
                } else {
                    throw new InvalidOperationException(errorMessage);
                }
            }

        }

        /// <summary>
        /// Creates a CALPOLESTR from a list of strings 
        /// It is the responsability of the caller to release this memory.
        /// </summary>
        /// <param name="guids"></param>
        /// <returns>A CALPOLESTR that was created from the the list of strings.</returns>
        public static CALPOLESTR CreateCALPOLESTR(IList<string> strings) {
            CALPOLESTR calpolStr = new CALPOLESTR();

            if (strings != null) {
                // Demand unmanaged permissions in order to access unmanaged memory.
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();

                calpolStr.cElems = (uint)strings.Count;

                int size = Marshal.SizeOf(typeof(IntPtr));

                calpolStr.pElems = Marshal.AllocCoTaskMem(strings.Count * size);

                IntPtr ptr = calpolStr.pElems;

                foreach (string aString in strings) {
                    IntPtr tempPtr = Marshal.StringToCoTaskMemUni(aString);
                    Marshal.WriteIntPtr(ptr, tempPtr);
                    ptr = new IntPtr(ptr.ToInt64() + size);
                }
            }

            return calpolStr;
        }

        /// <summary>
        /// Creates a CADWORD from a list of tagVsSccFilesFlags. Memory is allocated for the elems. 
        /// It is the responsability of the caller to release this memory.
        /// </summary>
        /// <param name="guids"></param>
        /// <returns>A CADWORD created from the list of tagVsSccFilesFlags.</returns>
        public static CADWORD CreateCADWORD(IList<tagVsSccFilesFlags> flags) {
            CADWORD cadWord = new CADWORD();

            if (flags != null) {
                // Demand unmanaged permissions in order to access unmanaged memory.
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();

                cadWord.cElems = (uint)flags.Count;

                int size = Marshal.SizeOf(typeof(UInt32));

                cadWord.pElems = Marshal.AllocCoTaskMem(flags.Count * size);

                IntPtr ptr = cadWord.pElems;

                foreach (tagVsSccFilesFlags flag in flags) {
                    Marshal.WriteInt32(ptr, (int)flag);
                    ptr = new IntPtr(ptr.ToInt64() + size);
                }
            }

            return cadWord;
        }

        /// <summary>
        /// Splits a bitmap from a Stream into an ImageList
        /// </summary>
        /// <param name="imageStream">A Stream representing a Bitmap</param>
        /// <returns>An ImageList object representing the images from the given stream</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static ImageList GetImageList(Stream imageStream) {
            ImageList ilist = new ImageList();

            if (imageStream == null) {
                Debug.Fail("ImageStream was null.");
                return ilist;
            }
            ilist.ColorDepth = ColorDepth.Depth24Bit;
            ilist.ImageSize = new Size(16, 16);
            Bitmap bitmap = new Bitmap(imageStream);
            ilist.Images.AddStrip(bitmap);
            ilist.TransparentColor = Color.Magenta;
            return ilist;
        }

        /// <summary>
        /// Gets the active configuration name.
        /// </summary>
        /// <param name="automationObject">The automation object.</param>
        /// <returns>The name of the active configuartion.</returns>
        internal static string GetActiveConfigurationName(EnvDTE.Project automationObject) {
            Utilities.ArgumentNotNull("automationObject", automationObject);

            string currentConfigName = string.Empty;
            if (automationObject.ConfigurationManager != null) {
                try {
                    EnvDTE.Configuration activeConfig = automationObject.ConfigurationManager.ActiveConfiguration;
                    if (activeConfig != null) {
                        currentConfigName = activeConfig.ConfigurationName;
                    }
                } catch (COMException ex) {
                    Debug.WriteLine("Failed to get active configuration because of {0}", ex);
                }
            }
            return currentConfigName;

        }


        /// <summary>
        /// Verifies that two objects represent the same instance of a COM object.
        /// This essentially compares the IUnkown pointers of the 2 objects.
        /// This is needed in scenario where aggregation is involved.
        /// </summary>
        /// <param name="obj1">Can be an object, interface or IntPtr</param>
        /// <param name="obj2">Can be an object, interface or IntPtr</param>
        /// <returns>True if the 2 items represent the same thing</returns>
        public static bool IsSameComObject(object obj1, object obj2) {
            bool isSame = false;
            IntPtr unknown1 = IntPtr.Zero;
            IntPtr unknown2 = IntPtr.Zero;
            try {
                // If we have 2 null, then they are not COM objects and as such "it's not the same COM object"
                if (obj1 != null && obj2 != null) {
                    unknown1 = QueryInterfaceIUnknown(obj1);
                    unknown2 = QueryInterfaceIUnknown(obj2);

                    isSame = IntPtr.Equals(unknown1, unknown2);
                }
            } finally {
                if (unknown1 != IntPtr.Zero) {
                    Marshal.Release(unknown1);
                }

                if (unknown2 != IntPtr.Zero) {
                    Marshal.Release(unknown2);
                }

            }

            return isSame;
        }

        /// <summary>
        /// Retrieve the IUnknown for the managed or COM object passed in.
        /// </summary>
        /// <param name="objToQuery">Managed or COM object.</param>
        /// <returns>Pointer to the IUnknown interface of the object.</returns>
        internal static IntPtr QueryInterfaceIUnknown(object objToQuery) {
            bool releaseIt = false;
            IntPtr unknown = IntPtr.Zero;
            IntPtr result;
            try {
                if (objToQuery is IntPtr) {
                    unknown = (IntPtr)objToQuery;
                } else {
                    // This is a managed object (or RCW)
                    unknown = Marshal.GetIUnknownForObject(objToQuery);
                    releaseIt = true;
                }

                // We might already have an IUnknown, but if this is an aggregated
                // object, it may not be THE IUnknown until we QI for it.
                Guid IID_IUnknown = VSConstants.IID_IUnknown;
                ErrorHandler.ThrowOnFailure(Marshal.QueryInterface(unknown, ref IID_IUnknown, out result));
            } finally {
                if (releaseIt && unknown != IntPtr.Zero) {
                    Marshal.Release(unknown);
                }

            }

            return result;
        }

        /// <summary>
        /// Returns true if thename that can represent a path, absolut or relative, or a file name contains invalid filename characters.
        /// </summary>
        /// <param name="name">File name</param>
        /// <returns>true if file name is invalid</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
            Justification = "The name is validated.")]
        public static bool ContainsInvalidFileNameChars(string name) {
            if (String.IsNullOrEmpty(name)) {
                return true;
            }

            try {
                if (Path.IsPathRooted(name) && !name.StartsWith(@"\\", StringComparison.Ordinal)) {
                    string root = Path.GetPathRoot(name);
                    name = name.Substring(root.Length);
                }
            }
                // The Path methods used by ContainsInvalidFileNameChars return argument exception if the filePath contains invalid characters.
            catch (ArgumentException) {
                return true;
            }

            Microsoft.VisualStudio.Shell.Url uri = new Microsoft.VisualStudio.Shell.Url(name);

            // This might be confusing bur Url.IsFile means that the uri represented by the name is either absolut or relative.
            if (uri.IsFile) {
                string[] segments = uri.Segments;
                if (segments != null && segments.Length > 0) {
                    foreach (string segment in segments) {
                        if (IsFilePartInValid(segment)) {
                            return true;
                        }
                    }

                    // Now the last segment should be specially taken care, since that cannot be all dots or spaces.
                    string lastSegment = segments[segments.Length - 1];
                    string filePart = Path.GetFileNameWithoutExtension(lastSegment);
                    // if the file is only an extension (.fob) then it's ok, otherwise we need to do the special checks.
                    if (filePart.Length != 0 && (IsFileNameAllGivenCharacter('.', filePart) || IsFileNameAllGivenCharacter(' ', filePart))) {
                        return true;
                    }
                }
            } else {
                // The assumption here is that we got a file name.
                string filePart = Path.GetFileNameWithoutExtension(name);
                if (IsFileNameAllGivenCharacter('.', filePart) || IsFileNameAllGivenCharacter(' ', filePart)) {
                    return true;
                }


                return IsFilePartInValid(name);
            }

            return false;
        }

        /// Cehcks if a file name is valid.
        /// </devdoc>
        /// <param name="fileName">The name of the file</param>
        /// <returns>True if the file is valid.</returns>
        public static bool IsFileNameInvalid(string fileName) {
            if (String.IsNullOrEmpty(fileName)) {
                return true;
            }

            if (IsFileNameAllGivenCharacter('.', fileName) || IsFileNameAllGivenCharacter(' ', fileName)) {
                return true;
            }


            return IsFilePartInValid(fileName);

        }

        /// <summary>
        /// Initializes the in memory project. Sets BuildEnabled on the project to true.
        /// </summary>
        /// <param name="engine">The build engine to use to create a build project.</param>
        /// <param name="fullProjectPath">The full path of the project.</param>
        /// <returns>A loaded msbuild project.</returns>
        internal static MSBuild.Project InitializeMsBuildProject(MSBuild.ProjectCollection buildEngine, string fullProjectPath) {
            Utilities.ArgumentNotNullOrEmpty("fullProjectPath", fullProjectPath);

            // Call GetFullPath to expand any relative path passed into this method.
            fullProjectPath = CommonUtils.NormalizePath(fullProjectPath);


            // Check if the project already has been loaded with the fullProjectPath. If yes return the build project associated to it.
            List<MSBuild.Project> loadedProject = new List<MSBuild.Project>(buildEngine.GetLoadedProjects(fullProjectPath));
            MSBuild.Project buildProject = loadedProject != null && loadedProject.Count > 0 && loadedProject[0] != null ? loadedProject[0] : null;

            if (buildProject == null) {
                buildProject = buildEngine.LoadProject(fullProjectPath);
            }

            return buildProject;
        }

        /// <summary>
        /// Loads a project file for the file. If the build project exists and it was loaded with a different file then it is unloaded first. 
        /// </summary>
        /// <param name="engine">The build engine to use to create a build project.</param>
        /// <param name="fullProjectPath">The full path of the project.</param>
        /// <param name="exitingBuildProject">An Existing build project that will be reloaded.</param>
        /// <returns>A loaded msbuild project.</returns>
        internal static MSBuild.Project ReinitializeMsBuildProject(MSBuild.ProjectCollection buildEngine, string fullProjectPath, MSBuild.Project exitingBuildProject) {
            // If we have a build project that has been loaded with another file unload it.
            try {
                if (exitingBuildProject != null && exitingBuildProject.ProjectCollection != null && !CommonUtils.IsSamePath(exitingBuildProject.FullPath, fullProjectPath)) {
                    buildEngine.UnloadProject(exitingBuildProject);
                }
            }
                // We  catch Invalid operation exception because if the project was unloaded while we touch the ParentEngine the msbuild API throws. 
                // Is there a way to figure out that a project was unloaded?
            catch (InvalidOperationException) {
            }

            return Utilities.InitializeMsBuildProject(buildEngine, fullProjectPath);
        }

        /// <summary>>
        /// Checks if the file name is all the given character.
        /// </summary>
        private static bool IsFileNameAllGivenCharacter(char c, string fileName) {
            // A valid file name cannot be all "c" .
            int charFound = 0;
            for (charFound = 0; charFound < fileName.Length && fileName[charFound] == c; ++charFound)
                ;
            if (charFound >= fileName.Length) {
                return true;
            }

            return false;
        }

        private const string _reservedName = "(\\b(nul|con|aux|prn)\\b)|(\\b((com|lpt)[0-9])\\b)";
        private const string _invalidChars = "([\\/:*?\"<>|#%])";
        private const string _regexToUseForFileName = _reservedName + "|" + _invalidChars;
        private static Regex _unsafeFileNameCharactersRegex = new Regex(_regexToUseForFileName, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private static Regex _unsafeCharactersRegex = new Regex(_invalidChars, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        /// <summary>
        /// Checks whether a file part contains valid characters. The file part can be any part of a non rooted path.
        /// </summary>
        /// <param name="filePart"></param>
        /// <returns></returns>
        private static bool IsFilePartInValid(string filePart) {
            if (String.IsNullOrEmpty(filePart)) {
                return true;
            }
            String fileNameToVerify = filePart;

            // Define a regular expression that covers all characters that are not in the safe character sets.
            // It is compiled for performance.

            // The filePart might still be a file and extension. If it is like that then we must check them separately, since different rules apply
            string extension = String.Empty;
            try {
                extension = Path.GetExtension(filePart);
            }
                // We catch the ArgumentException because we want this method to return true if the filename is not valid. FilePart could be for example #¤&%"¤&"% and that would throw ArgumentException on GetExtension
            catch (ArgumentException) {
                return true;
            }

            if (!String.IsNullOrEmpty(extension)) {
                // Check the extension first
                bool isMatch = _unsafeCharactersRegex.IsMatch(extension);
                if (isMatch) {
                    return isMatch;
                }

                // We want to verify here everything but the extension.
                // We cannot use GetFileNameWithoutExtension because it might be that for example (..\\filename.txt) is passed in and that should fail, since that is not a valid filename.
                fileNameToVerify = filePart.Substring(0, filePart.Length - extension.Length);

                if (String.IsNullOrEmpty(fileNameToVerify)) {
                    // http://pytools.codeplex.com/workitem/497
                    // .fob is ok
                    return false;
                }
            }

            // We verify CLOCK$ outside the regex since for some reason the regex is not matching the clock\\$ added.
            if (String.Equals(fileNameToVerify, "CLOCK$", StringComparison.OrdinalIgnoreCase)) {
                return true;
            }

            return _unsafeFileNameCharactersRegex.IsMatch(fileNameToVerify);
        }

        /// <summary>
        /// Copy a directory recursively to the specified non-existing directory
        /// </summary>
        /// <param name="source">Directory to copy from</param>
        /// <param name="target">Directory to copy to</param>
        public static void RecursivelyCopyDirectory(string source, string target) {
            // Make sure it doesn't already exist
            if (Directory.Exists(target))
                throw new ArgumentException(SR.GetString(SR.FileOrFolderAlreadyExists, target));

            Directory.CreateDirectory(target);
            DirectoryInfo directory = new DirectoryInfo(source);

            // Copy files
            foreach (FileInfo file in directory.GetFiles()) {
                file.CopyTo(Path.Combine(target, file.Name));
            }

            // Now recurse to child directories
            foreach (DirectoryInfo child in directory.GetDirectories()) {
                RecursivelyCopyDirectory(child.FullName, Path.Combine(target, child.Name));
            }
        }

        /// <summary>
        /// Canonicalizes a file name, including:
        ///  - determines the full path to the file
        ///  - casts to upper case
        /// Canonicalizing a file name makes it possible to compare file names using simple simple string comparison.
        /// 
        /// Note: this method does not handle shared drives and UNC drives.
        /// </summary>
        /// <param name="anyFileName">A file name, which can be relative/absolute and contain lower-case/upper-case characters.</param>
        /// <returns>Canonicalized file name.</returns>
        internal static string CanonicalizeFileName(string anyFileName) {
            // Get absolute path
            // Note: this will not handle UNC paths
            FileInfo fileInfo = new FileInfo(anyFileName);
            string fullPath = fileInfo.FullName;

            // Cast to upper-case
            fullPath = fullPath.ToUpperInvariant();

            return fullPath;
        }

        /// <summary>
        /// Determines if a file is a template.
        /// </summary>
        /// <param name="fileName">The file to check whether it is a template file</param>
        /// <returns>true if the file is a template file</returns>
        internal static bool IsTemplateFile(string fileName) {
            if (String.IsNullOrEmpty(fileName)) {
                return false;
            }

            string extension = Path.GetExtension(fileName);
            return (String.Equals(extension, ".vstemplate", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".vsz", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Save dirty files
        /// </summary>
        /// <returns>Whether succeeded</returns>
        public static bool SaveDirtyFiles() {
            var rdt = ServiceProvider.GlobalProvider.GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
            if (rdt != null) {
                // Consider using (uint)(__VSRDTSAVEOPTIONS.RDTSAVEOPT_SaveIfDirty | __VSRDTSAVEOPTIONS.RDTSAVEOPT_PromptSave)
                // when VS settings include prompt for save on build
                var saveOpt = (uint)__VSRDTSAVEOPTIONS.RDTSAVEOPT_SaveIfDirty;
                var hr = rdt.SaveDocuments(saveOpt, null, VSConstants.VSITEMID_NIL, VSConstants.VSCOOKIE_NIL);
                if (hr == VSConstants.E_ABORT) {
                    return false;
                }
            }

            return true;
        }
    }
}
