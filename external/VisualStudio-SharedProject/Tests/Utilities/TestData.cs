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
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudioTools;

namespace TestUtilities {
    public static class TestData {
        const string BinariesAltSourcePath = @"Tests";
        const string BinariesSourcePath = @"BuildOutput\" +
#if DEBUG
            @"Debug" + 
#else
            @"Release" + 
#endif
            AssemblyVersionInfo.VSVersion + @"\Tests";
        const string BinariesOutPath = "";

        const string DataAltSourcePath = @"Tests\TestData";
        const string DataOutPath = @"TestData";

        private static string GetSolutionDir() {
            var dir = Path.GetDirectoryName((typeof(TestData)).Assembly.Location);
            while (!string.IsNullOrEmpty(dir) && 
                Directory.Exists(dir) && 
                !File.Exists(Path.Combine(dir, "build.root"))) {
                dir = Path.GetDirectoryName(dir);
            }
            return dir ?? "";
        }

        public static void CopyFiles(string sourceDir, string destDir) {
            FileUtils.CopyDirectory(sourceDir, destDir);
        }

        public static string BinarySourceLocation {
            get {
                var sourceRoot = GetSolutionDir();
                var binSource = Path.Combine(sourceRoot, BinariesSourcePath);
                if (!Directory.Exists(binSource)) {
                    binSource = Path.Combine(sourceRoot, BinariesAltSourcePath);
                    if (!Directory.Exists(binSource)) {
                        Debug.Fail("Could not find location of test binaries." + Environment.NewLine + "    " + binSource);
                    }
                }
                Console.WriteLine("Binary source location: {0}", binSource);
                return binSource;
            }
        }

        public static void Deploy(string dataSourcePath, bool includeTestData = true) {
            var sourceRoot = GetSolutionDir();
            var deployRoot = Path.GetDirectoryName((typeof(TestData)).Assembly.Location);

            if (deployRoot.Length < 5) {
                Debug.Fail("Invalid deploy root", string.Format("sourceRoot={0}\ndeployRoot={1}", sourceRoot, deployRoot));
            }

            var binSource = BinarySourceLocation;

            var binDest = Path.Combine(deployRoot, BinariesOutPath);
            if (binSource == binDest) {
                if (includeTestData) {
                    Debug.Fail("Running tests inside build directory", "Select the default.testsettings file before running tests.");
                } else {
                    return;
                }
            }

            CopyFiles(binSource, binDest);

            if (includeTestData) {
                var dataSource = Path.Combine(sourceRoot, dataSourcePath);
                if (!Directory.Exists(dataSource)) {
                    dataSource = Path.Combine(sourceRoot, DataAltSourcePath);
                    if (!Directory.Exists(dataSource)) {
                        Debug.Fail("Could not find location of test data." + Environment.NewLine + "    " + dataSource);
                    }
                }

                CopyFiles(dataSource, Path.Combine(deployRoot, DataOutPath));
            }
        }

        /// <summary>
        /// Returns the full path to the test data root.
        /// </summary>
        public static string GetPath() {
            return Path.GetDirectoryName((typeof(TestData)).Assembly.Location);
        }

        /// <summary>
        /// Returns the full path to the deployed file.
        /// </summary>
        public static string GetPath(string relativePath) {
            return CommonUtils.GetAbsoluteFilePath(GetPath(), relativePath);
        }

        /// <summary>
        /// Returns the full path to a temporary directory. This is within the
        /// deployment to ensure that test files are easily cleaned up.
        /// </summary>
        public static string GetTempPath(string subPath = null, bool randomSubPath = false) {
            var path = TestData.GetPath("Temp");
            if (randomSubPath) {
                subPath = Path.GetRandomFileName();
                while (Directory.Exists(Path.Combine(path, subPath))) {
                    subPath = Path.GetRandomFileName();
                }
            }
            if (!string.IsNullOrEmpty(subPath)) {
                path = Path.Combine(path, subPath);
            }
            Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Opens a FileStream for a file from the current deployment.
        /// </summary>
        public static FileStream Open(string relativePath, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, FileShare share = FileShare.Read) {
            return new FileStream(GetPath(relativePath), mode, access, share);
        }

        /// <summary>
        /// Opens a StreamReader for a file from the current deployment.
        /// </summary>
        public static StreamReader Read(string relativePath, Encoding encoding = null) {
            return new StreamReader(GetPath(relativePath), encoding ?? Encoding.Default);
        }
    }
}
