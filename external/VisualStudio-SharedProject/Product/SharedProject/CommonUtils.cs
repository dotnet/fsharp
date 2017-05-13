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

namespace Microsoft.VisualStudioTools {
    internal static class CommonUtils {
        private static readonly char[] InvalidPathChars = GetInvalidPathChars();

        private static readonly char[] DirectorySeparators = new[] {
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar
        };

        private static char[] GetInvalidPathChars() {
            return Path.GetInvalidPathChars().Concat(new[] { '*', '?' }).ToArray();
        }

        internal static bool TryMakeUri(string path, bool isDirectory, UriKind kind, out Uri uri) {
            if (isDirectory && !string.IsNullOrEmpty(path) && !HasEndSeparator(path)) {
                path += Path.DirectorySeparatorChar;
            }

            return Uri.TryCreate(path, kind, out uri);
        }

        internal static Uri MakeUri(string path, bool isDirectory, UriKind kind, string throwParameterName = "path") {
            try {
                if (isDirectory && !string.IsNullOrEmpty(path) && !HasEndSeparator(path)) {
                    path += Path.DirectorySeparatorChar;
                }

                return new Uri(path, kind);

            } catch (UriFormatException ex) {
                throw new ArgumentException("Path was invalid", throwParameterName, ex);
            } catch (ArgumentException ex) {
                throw new ArgumentException("Path was invalid", throwParameterName, ex);
            }
        }

        /// <summary>
        /// Normalizes and returns the provided path.
        /// </summary>
        public static string NormalizePath(string path) {
            if (string.IsNullOrEmpty(path)) {
                return null;
            }

            var uri = MakeUri(path, false, UriKind.RelativeOrAbsolute);
            if (uri.IsAbsoluteUri) {
                if (uri.IsFile) {
                    return uri.LocalPath;
                } else {
                    return uri.AbsoluteUri.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                }
            } else {
                return Uri.UnescapeDataString(uri.ToString()).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }
        }

        /// <summary>
        /// Normalizes and returns the provided directory path, always
        /// ending with '/'.
        /// </summary>
        public static string NormalizeDirectoryPath(string path) {
            if (string.IsNullOrEmpty(path)) {
                return null;
            }

            var uri = MakeUri(path, true, UriKind.RelativeOrAbsolute);
            if (uri.IsAbsoluteUri) {
                if (uri.IsFile) {
                    return uri.LocalPath;
                } else {
                    return uri.AbsoluteUri.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                }
            } else {
                return Uri.UnescapeDataString(uri.ToString()).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }
        }

        /// <summary>
        /// Return true if both paths represent the same directory.
        /// </summary>
        public static bool IsSameDirectory(string path1, string path2) {
            if (string.IsNullOrEmpty(path1)) {
                return string.IsNullOrEmpty(path2);
            } else if (string.IsNullOrEmpty(path2)) {
                return false;
            }

            if (String.Equals(path1, path2, StringComparison.Ordinal)) {
                // Quick return, but will only work where the paths are already normalized and
                // have matching case.
                return true;
            }

            Uri uri1, uri2;
            return
                TryMakeUri(path1, true, UriKind.Absolute, out uri1) &&
                TryMakeUri(path2, true, UriKind.Absolute, out uri2) &&
                uri1 == uri2;
        }

        /// <summary>
        /// Return true if both paths represent the same location.
        /// </summary>
        public static bool IsSamePath(string file1, string file2) {
            if (string.IsNullOrEmpty(file1)) {
                return string.IsNullOrEmpty(file2);
            } else if (string.IsNullOrEmpty(file2)) {
                return false;
            }

            if (String.Equals(file1, file2, StringComparison.Ordinal)) {
                // Quick return, but will only work where the paths are already normalized and
                // have matching case.
                return true;
            }

            Uri uri1, uri2;
            return
                TryMakeUri(file1, false, UriKind.Absolute, out uri1) &&
                TryMakeUri(file2, false, UriKind.Absolute, out uri2) &&
                uri1 == uri2;
        }

        /// <summary>
        /// Return true if the path represents a file or directory contained in
        /// root or a subdirectory of root.
        /// </summary>
        public static bool IsSubpathOf(string root, string path) {
            if (HasEndSeparator(root) && !path.Contains("..") && path.StartsWith(root, StringComparison.Ordinal)) {
                // Quick return, but only where the paths are already normalized and
                // have matching case.
                return true;
            }

            var uriRoot = MakeUri(root, true, UriKind.Absolute, "root");
            var uriPath = MakeUri(path, false, UriKind.Absolute, "path");

            if (uriRoot.Equals(uriPath) || uriRoot.IsBaseOf(uriPath)) {
                return true;
            }

            // Special case where root and path are the same, but path was provided
            // without a terminating separator.
            var uriDirectoryPath = MakeUri(path, true, UriKind.Absolute, "path");
            if (uriRoot.Equals(uriDirectoryPath)) {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a normalized directory path created by joining relativePath to root.
        /// The result is guaranteed to end with a backslash.
        /// </summary>
        /// <exception cref="ArgumentException">root is not an absolute path, or
        /// either path is invalid.</exception>
        /// <exception cref="InvalidOperationException">An absolute path cannot be
        /// created.</exception>
        public static string GetAbsoluteDirectoryPath(string root, string relativePath) {
            string absPath;

            if (string.IsNullOrEmpty(relativePath)) {
                return NormalizeDirectoryPath(root);
            }

            var relUri = MakeUri(relativePath, true, UriKind.RelativeOrAbsolute, "relativePath");
            Uri absUri;

            if (relUri.IsAbsoluteUri) {
                absUri = relUri;
            } else {
                var rootUri = MakeUri(root, true, UriKind.Absolute, "root");
                try {
                    absUri = new Uri(rootUri, relUri);
                } catch (UriFormatException ex) {
                    throw new InvalidOperationException("Cannot create absolute path", ex);
                }
            }

            absPath = absUri.IsFile ? absUri.LocalPath : absUri.AbsoluteUri;

            if (!string.IsNullOrEmpty(absPath) && !HasEndSeparator(absPath)) {
                absPath += absUri.IsFile ? Path.DirectorySeparatorChar : Path.AltDirectorySeparatorChar;
            }

            return absPath;
        }

        /// <summary>
        /// Returns a normalized file path created by joining relativePath to root.
        /// The result is not guaranteed to end with a backslash.
        /// </summary>
        /// <exception cref="ArgumentException">root is not an absolute path, or
        /// either path is invalid.</exception>
        public static string GetAbsoluteFilePath(string root, string relativePath) {
            var rootUri = MakeUri(root, true, UriKind.Absolute, "root");
            var relUri = MakeUri(relativePath, false, UriKind.RelativeOrAbsolute, "relativePath");

            Uri absUri;

            if (relUri.IsAbsoluteUri) {
                absUri = relUri;
            } else {
                try {
                    absUri = new Uri(rootUri, relUri);
                } catch (UriFormatException ex) {
                    throw new InvalidOperationException("Cannot create absolute path", ex);
                }
            }

            return absUri.IsFile ? absUri.LocalPath : absUri.AbsoluteUri;
        }

        /// <summary>
        /// Returns a relative path from the base path to the other path. This is
        /// intended for serialization rather than UI. See CreateFriendlyDirectoryPath
        /// for UI strings.
        /// </summary>
        /// <exception cref="ArgumentException">Either parameter was an invalid or a
        /// relative path.</exception>
        public static string GetRelativeDirectoryPath(string fromDirectory, string toDirectory) {
            var fromUri = MakeUri(fromDirectory, true, UriKind.Absolute, "fromDirectory");
            var toUri = MakeUri(toDirectory, true, UriKind.Absolute, "toDirectory");

            string relPath;
            var sep = toUri.IsFile ? Path.DirectorySeparatorChar : Path.AltDirectorySeparatorChar;

            try {
                var relUri = fromUri.MakeRelativeUri(toUri);
                if (relUri.IsAbsoluteUri) {
                    relPath = relUri.IsFile ? relUri.LocalPath : relUri.AbsoluteUri;
                } else {
                    relPath = Uri.UnescapeDataString(relUri.ToString());
                }
            } catch (InvalidOperationException ex) {
                Trace.WriteLine(string.Format("Error finding path from {0} to {1}", fromUri, toUri));
                Trace.WriteLine(ex);
                relPath = toUri.IsFile ? toUri.LocalPath : toUri.AbsoluteUri;
            }

            if (!string.IsNullOrEmpty(relPath) && !HasEndSeparator(relPath)) {
                relPath += Path.DirectorySeparatorChar;
            }

            if (toUri.IsFile) {
                return relPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            } else {
                return relPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }
        }

        /// <summary>
        /// Returns a relative path from the base path to the file. This is
        /// intended for serialization rather than UI. See CreateFriendlyFilePath
        /// for UI strings.
        /// </summary>
        public static string GetRelativeFilePath(string fromDirectory, string toFile) {
            var fromUri = MakeUri(fromDirectory, true, UriKind.Absolute, "fromDirectory");
            var toUri = MakeUri(toFile, false, UriKind.Absolute, "toFile");

            string relPath;
            var sep = toUri.IsFile ? Path.DirectorySeparatorChar : Path.AltDirectorySeparatorChar;

            try {
                var relUri = fromUri.MakeRelativeUri(toUri);
                if (relUri.IsAbsoluteUri) {
                    relPath = relUri.IsFile ? relUri.LocalPath : relUri.AbsoluteUri;
                } else {
                    relPath = Uri.UnescapeDataString(relUri.ToString());
                }
            } catch (InvalidOperationException ex) {
                Trace.WriteLine(string.Format("Error finding path from {0} to {1}", fromUri, toUri));
                Trace.WriteLine(ex);
                relPath = toUri.IsFile ? toUri.LocalPath : toUri.AbsoluteUri;
            }

            if (toUri.IsFile) {
                return relPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            } else {
                return relPath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }
        }

        /// <summary>
        /// Tries to create a friendly directory path: '.' if the same as base path,
        /// relative path if short, absolute path otherwise.
        /// </summary>
        public static string CreateFriendlyDirectoryPath(string basePath, string path) {
            var relativePath = GetRelativeDirectoryPath(basePath, path);

            if (relativePath.Length > 1) {
                relativePath = TrimEndSeparator(relativePath);
            }

            if (string.IsNullOrEmpty(relativePath)) {
                relativePath = ".";
            }

            return relativePath;
        }

        /// <summary>
        /// Tries to create a friendly file path.
        /// </summary>
        public static string CreateFriendlyFilePath(string basePath, string path) {
            return GetRelativeFilePath(basePath, path);
        }

        /// <summary>
        /// Returns the last directory segment of a path. The last segment is
        /// assumed to be the string between the second-last and last directory
        /// separator characters in the path. If there is no suitable substring,
        /// the empty string is returned.
        /// 
        /// The first segment of the path is only returned if it does not
        /// contain a colon. Segments equal to "." are ignored and the preceding
        /// segment is used.
        /// </summary>
        /// <remarks>
        /// This should be used in place of:
        /// <c>Path.GetFileName(CommonUtils.TrimEndSeparator(Path.GetDirectoryName(path)))</c>
        /// </remarks>
        public static string GetLastDirectoryName(string path) {
            if (string.IsNullOrEmpty(path)) {
                return string.Empty;
            }

            int last = path.LastIndexOfAny(DirectorySeparators);

            string result = string.Empty;
            while (last > 1) {
                int first = path.LastIndexOfAny(DirectorySeparators, last - 1);
                if (first < 0) {
                    if (path.IndexOf(':') < last) {
                        // Don't want to return scheme/drive as a directory
                        return string.Empty;
                    }
                    first = -1;
                }
                if (first == 1 && path[0] == path[1]) {
                    // Don't return computer name in UNC path
                    return string.Empty;
                }

                result = path.Substring(first + 1, last - (first + 1));
                if (!string.IsNullOrEmpty(result) && result != ".") {
                    // Result is valid
                    break;
                }

                last = first;
            }

            return result;
        }

        /// <summary>
        /// Returns the path to the parent directory segment of a path. If the
        /// last character of the path is a directory separator, the segment
        /// prior to that character is removed. Otherwise, the segment following
        /// the last directory separator is removed.
        /// </summary>
        /// <remarks>
        /// This should be used in place of:
        /// <c>Path.GetDirectoryName(CommonUtils.TrimEndSeparator(path)) + Path.DirectorySeparatorChar</c>
        /// </remarks>
        public static string GetParent(string path) {
            if (string.IsNullOrEmpty(path)) {
                return string.Empty;
            }

            int last = path.Length - 1;
            if (DirectorySeparators.Contains(path[last])) {
                last -= 1;
            }

            if (last <= 0) {
                return string.Empty;
            }

            last = path.LastIndexOfAny(DirectorySeparators, last);

            if (last < 0) {
                return string.Empty;
            }

            return path.Remove(last + 1);
        }

        /// <summary>
        /// Returns the last segment of the path. If the last character is a
        /// directory separator, this will be the segment preceding the
        /// separator. Otherwise, it will be the segment following the last
        /// separator.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileOrDirectoryName(string path) {
            if (string.IsNullOrEmpty(path)) {
                return string.Empty;
            }

            int last = path.Length - 1;
            if (DirectorySeparators.Contains(path[last])) {
                last -= 1;
            }

            if (last < 0) {
                return string.Empty;
            }

            int start = path.LastIndexOfAny(DirectorySeparators, last);

            return path.Substring(start + 1, last - start);
        }

        /// <summary>
        /// Returns true if the path has a directory separator character at the end.
        /// </summary>
        public static bool HasEndSeparator(string path) {
            return !string.IsNullOrEmpty(path) && DirectorySeparators.Contains(path[path.Length - 1]);
        }

        /// <summary>
        /// Removes up to one directory separator character from the end of path.
        /// </summary>
        public static string TrimEndSeparator(string path) {
            if (HasEndSeparator(path)) {
                if (path.Length > 2 && path[path.Length - 2] == ':') {
                    // The slash at the end of a drive specifier is not actually
                    // a separator.
                    return path;
                } else if (path.Length > 3 && path[path.Length - 2] == path[path.Length - 1] && path[path.Length - 3] == ':') {
                    // The double slash at the end of a schema is not actually a
                    // separator.
                    return path;
                }
                return path.Remove(path.Length - 1);
            } else {
                return path;
            }
        }

        /// <summary>
        /// Adds a directory separator character to the end of path if required.
        /// </summary>
        public static string EnsureEndSeparator(string path) {
            if (string.IsNullOrEmpty(path)) {
                return string.Empty;
            } else if (!HasEndSeparator(path)) {
                return path + Path.DirectorySeparatorChar;
            } else {
                return path;
            }
        }

        /// <summary>
        /// Removes leading @"..\" segments from a path.
        /// </summary>
        private static string TrimUpPaths(string path) {
            int actualStart = 0;
            while (actualStart + 2 < path.Length) {
                if (path[actualStart] == '.' && path[actualStart + 1] == '.' &&
                    (path[actualStart + 2] == Path.DirectorySeparatorChar || path[actualStart + 2] == Path.AltDirectorySeparatorChar)) {
                    actualStart += 3;
                } else {
                    break;
                }
            }

            return (actualStart > 0) ? path.Substring(actualStart) : path;
        }

        /// <summary>
        /// Returns true if the path is a valid path, regardless of whether the
        /// file exists or not.
        /// </summary>
        public static bool IsValidPath(string path) {
            return !string.IsNullOrEmpty(path) &&
                path.IndexOfAny(InvalidPathChars) < 0;
        }

        /// <summary>
        /// Recursively searches for a file using breadth-first-search. This
        /// ensures that the result closest to <paramref name="root"/> is
        /// returned first.
        /// </summary>
        /// <param name="root">
        /// Directory to start searching.
        /// </param>
        /// <param name="file">
        /// Filename to find. Wildcards are not supported.
        /// </param>
        /// <param name="depthLimit">
        /// The number of subdirectories to search in.
        /// </param>
        /// <param name="firstCheck">
        /// A sequence of subdirectories to prioritize.
        /// </param>
        /// <returns>
        /// The path to the file if found, including <paramref name="root"/>;
        /// otherwise, null.
        /// </returns>
        public static string FindFile(
            string root,
            string file,
            int depthLimit = 2,
            IEnumerable<string> firstCheck = null
        ) {
            var candidate = Path.Combine(root, file);
            if (File.Exists(candidate)) {
                return candidate;
            }
            if (firstCheck != null) {
                foreach (var subPath in firstCheck) {
                    candidate = Path.Combine(root, subPath, file);
                    if (File.Exists(candidate)) {
                        return candidate;
                    }
                }
            }

            // Do a BFS of the filesystem to ensure we find the match closest to
            // the root directory.
            var dirQueue = new Queue<string>();
            dirQueue.Enqueue(root);
            dirQueue.Enqueue("<EOD>");
            while (dirQueue.Any()) {
                var dir = dirQueue.Dequeue();
                if (dir == "<EOD>") {
                    depthLimit -= 1;
                    if (depthLimit <= 0) {
                        return null;
                    }
                    continue;
                }
                var result = Directory.EnumerateFiles(dir, file, SearchOption.TopDirectoryOnly).FirstOrDefault();
                if (result != null) {
                    return result;
                }
                foreach (var subDir in Directory.EnumerateDirectories(dir)) {
                    dirQueue.Enqueue(subDir);
                }
                dirQueue.Enqueue("<EOD>");
            }
            return null;
        }

        /// <summary>
        /// Gets a filename in the specified location with the specified name and extension.
        /// If the file already exist it will calculate a name with a number in it.
        /// </summary>
        public static string GetAvailableFilename(string location, string basename, string extension) {
            var newPath = Path.Combine(location, basename);
            int index = 0;
            if (File.Exists(newPath + extension)) {
                string candidateNewPath;
                do {
                    candidateNewPath = string.Format("{0}{1}", newPath, ++index);
                } while (File.Exists(candidateNewPath + extension));
                newPath = candidateNewPath;
            }
            string final = newPath + extension;
            return final;
        }
    }
}
