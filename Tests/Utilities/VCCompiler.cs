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
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;

namespace TestUtilities {
    public class VCCompiler {
        public readonly string BinPath;

        public readonly string BinPaths;
        public readonly string LibPaths;
        public readonly string IncludePaths;

        public static VCCompiler VC9_X86 { get { return FindVC("9.0", ProcessorArchitecture.X86); } }
        public static VCCompiler VC10_X86 { get { return FindVC("10.0", ProcessorArchitecture.X86); } }
        public static VCCompiler VC11_X86 { get { return FindVC("11.0", ProcessorArchitecture.X86); } }
        public static VCCompiler VC12_X86 { get { return FindVC("12.0", ProcessorArchitecture.X86); } }
        public static VCCompiler VC14_X86 { get { return FindVC("14.0", ProcessorArchitecture.X86); } }
        public static VCCompiler VC9_X64 { get { return FindVC("9.0", ProcessorArchitecture.Amd64); } }
        public static VCCompiler VC10_X64 { get { return FindVC("10.0", ProcessorArchitecture.Amd64); } }
        public static VCCompiler VC11_X64 { get { return FindVC("11.0", ProcessorArchitecture.Amd64); } }
        public static VCCompiler VC12_X64 { get { return FindVC("12.0", ProcessorArchitecture.Amd64); } }
        public static VCCompiler VC14_X64 { get { return FindVC("14.0", ProcessorArchitecture.Amd64); } }

        private VCCompiler(string bin, string bins, string include, string lib) {
            BinPath = bin ?? string.Empty;
            BinPaths = bins ?? BinPath;
            LibPaths = lib ?? string.Empty;
            IncludePaths = include ?? string.Empty;
        }

        private static VCCompiler FindVC(string version, ProcessorArchitecture arch) {
            string vcDir = null, vsDir = null;

            using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
            using (var key1 = baseKey.OpenSubKey("SOFTWARE\\Microsoft\\VisualStudio\\SxS\\VC7"))
            using (var key2 = baseKey.OpenSubKey("SOFTWARE\\Microsoft\\VisualStudio\\SxS\\VS7")) {
                if (key1 != null) {
                    vcDir = key1.GetValue(version) as string;
                }
                if (key2 != null) {
                    vsDir = key2.GetValue(version) as string;
                }
            }

            if (string.IsNullOrEmpty(vcDir)) {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32))
                using (var key = baseKey.OpenSubKey("SOFTWARE\\Microsoft\\DevDiv\\VCForPython\\" + version)) {
                    if (key != null) {
                        vcDir = key.GetValue("InstallDir") as string;
                    }
                }
            }
            if (string.IsNullOrEmpty(vcDir)) {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                using (var key = baseKey.OpenSubKey("SOFTWARE\\Microsoft\\DevDiv\\VCForPython\\" + version)) {
                    if (key != null) {
                        vcDir = key.GetValue("InstallDir") as string;
                    }
                }
            }

            if (string.IsNullOrEmpty(vcDir)) {
                return null;
            }

            string bin = Path.Combine(vcDir, "bin"), bins = bin;
            if (arch == ProcessorArchitecture.Amd64) {
                bin = Path.Combine(bin, "x86_amd64");
                bins = bin + ";" + bins;
            }
            if (!string.IsNullOrEmpty(vsDir)) {
                bins += ";" + vsDir;
            }

            string include = Path.Combine(vcDir, "include");

            string lib = Path.Combine(vcDir, "lib");
            if (arch == ProcessorArchitecture.Amd64) {
                lib = Path.Combine(lib, "amd64");
            }

            AddWindowsSdk(version, arch, ref include, ref lib);

            return new VCCompiler(bin, bins, include, lib);
        }

        private static void AddWindowsSdk(
            string vcVersion,
            ProcessorArchitecture arch,
            ref string includePaths,
            ref string libPaths
        ) {
            var isX64 = (arch == ProcessorArchitecture.Amd64);

            if (vcVersion == "14.0") {
                // Windows 10 kit is required for this version
                AddWindows10KitPaths(isX64, ref includePaths, ref libPaths);
                return;
            }

            if (vcVersion == "10.0" || vcVersion == "11.0" || vcVersion == "12.0") {
                // If we find a Windows 8 kit, then return
                if (AddWindows8KitPaths(isX64, ref includePaths, ref libPaths)) {
                    return;
                }
            }

            AddWindowsSdkPaths(isX64, ref includePaths, ref libPaths);
        }

        private static void AppendPath(ref string paths, string path) {
            if (string.IsNullOrEmpty(paths)) {
                paths = path;
            } else {
                paths += ";" + path;
            }
        }

        private static bool AddWindowsSdkPaths(
            bool isX64,
            ref string includePaths,
            ref string libPaths
        ) {
            foreach(var version in new[] { "v8.0A", "v7.0A", "v7.0" }) {
            var regValue = Registry.GetValue(
                "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Microsoft SDKs\\Windows\\" + version,
                "InstallationFolder",
                null
            );

            string[] locations = new[] {
                regValue != null ? regValue.ToString() : null,
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft SDKs", "Windows", version),
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft SDKs", "Windows", version)
                };

            foreach(var rootPath in locations) {
                if (!Directory.Exists(rootPath)) {
                    continue;
                }

                var include = Path.Combine(rootPath, "Include");
                var lib = Path.Combine(rootPath, "Lib");
                if (isX64) {
                    lib = Path.Combine(lib, "x64");
                }
                if (Directory.Exists(include) && Directory.Exists(lib)) {
                    AppendPath(ref includePaths, include);
                    AppendPath(ref libPaths, lib);
                    return true;
                }
            }}
            return false;
        }

        private static bool AddWindows8KitPaths(
            bool isX64,
            ref string includePaths,
            ref string libPaths
        ) {
            foreach (var version in new[] { "KitsRoot81", "KitsRoot" }) {
                var regValue = Registry.GetValue(
                    "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Kits\\Installed Roots",
                    version,
                    null
                );

                if (regValue == null) {
                    continue;
                }

                var rootPath = regValue.ToString();
                var includeShared = Path.Combine(rootPath, "Include", "shared");
                var includeum = Path.Combine(rootPath, "Include", "um");
                var lib8 = Path.Combine(rootPath, "Lib", "win8", "um", isX64 ? "x64" : "x86");
                var libv63 = Path.Combine(rootPath, "Lib", "winv6.3", "um", isX64 ? "x64" : "x86");

                if (!Directory.Exists(includeShared) || !Directory.Exists(includeum)) {
                    continue;
                }

                if (Directory.Exists(lib8)) {
                    AppendPath(ref libPaths, lib8);
                } else if (Directory.Exists(libv63)) {
                    AppendPath(ref libPaths, libv63);
                } else {
                    continue;
                }

                AppendPath(ref includePaths, includeShared);
                AppendPath(ref includePaths, includeum);

                return true;
            }

            return false;
        }

        private static bool AddWindows10KitPaths(
            bool isX64,
            ref string includePaths,
            ref string libPaths
        ) {
            string include8 = null;
            string lib8 = null;

            if (!AddWindows8KitPaths(isX64, ref include8, ref lib8)) {
                return false;
            }

            foreach (var version in new[] { "KitsRoot10" }) {
                var regValue = Registry.GetValue(
                    "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows Kits\\Installed Roots",
                    version,
                    null
                );

                if (regValue == null) {
                    continue;
                }

                var rootPath = regValue.ToString();
                var include = Path.Combine(rootPath, "Include");
                if (!Directory.Exists(include)) {
                    continue;
                }
                // We want a subfolder that is a version number - get the latest
                var crtVersion = new DirectoryInfo(include).EnumerateDirectories()
                    .Select(d => d.Name)
                    .OrderByDescending(s => s)
                    .FirstOrDefault();
                if (string.IsNullOrEmpty(crtVersion)) {
                    continue;
                }

                include = Path.Combine(include, crtVersion, "ucrt");
                var lib = Path.Combine(rootPath, "Lib", crtVersion, "ucrt", isX64 ? "x64" : "x86");

                if (!Directory.Exists(include) || !Directory.Exists(lib)) {
                    continue;
                }

                AppendPath(ref includePaths, include);
                AppendPath(ref includePaths, include8);

                AppendPath(ref libPaths, lib);
                AppendPath(ref libPaths, lib8);

                return true;
            }

            return false;

        }
    }

}
