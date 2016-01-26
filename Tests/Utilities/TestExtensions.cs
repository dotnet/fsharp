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

using EnvDTE;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestUtilities {
    public static class TestExtensions {
        public static void SetStartupFile(this Project project, string name) {
            Assert.IsNotNull(project, "null project");
            Assert.IsNotNull(project.Properties, "null project properties " + project.Name + " " + project.GetType().FullName + " " + project.Kind);
            Assert.IsNotNull(project.Properties.Item("StartupFile"), "null startup file property" + project.Name + " " + project.GetType().FullName);
            project.Properties.Item("StartupFile").Value = name;
        }

        public static IEnumerable<int> FindIndexesOf(this string s, string substring) {
            int pos = 0;
            while (true) {
                pos = s.IndexOf(substring, pos);
                if (pos < 0) {
                    break;
                }
                yield return pos;
                pos++;
            }
        }

        public static int IndexOfEnd(this string s, string substring) {
            int i = s.IndexOf(substring);
            if (i < 0) {
                return i;
            }
            return i + substring.Length;
        }

        public static HashSet<T> ToSet<T>(this IEnumerable<T> enumeration) {
            return new HashSet<T>(enumeration);
        }

        public static HashSet<T> ToSet<T>(this IEnumerable<T> enumeration, IEqualityComparer<T> comparer) {
            return new HashSet<T>(enumeration, comparer);
        }

        public static bool ContainsExactly<T>(this HashSet<T> set, IEnumerable<T> values) {
            if (set.Count != values.Count()) {
                return false;
            }
            foreach (var value in values) {
                if (!set.Contains(value)) {
                    return false;
                }
            }
            foreach (var value in set) {
                if (!values.Contains(value, set.Comparer)) {
                    return false;
                }
            }
            return true;
        }

        public static XName GetName(this XDocument doc, string localName) {
            return doc.Root.Name.Namespace.GetName(localName);
        }

        public static XElement Descendant(this XDocument doc, string localName) {
            return doc.Descendants(doc.Root.Name.Namespace.GetName(localName)).Single();
        }

        public static XElement Descendant(this XElement node, string localName) {
            return node.Descendants(node.Document.Root.Name.Namespace.GetName(localName)).Single();
        }
    }

}
