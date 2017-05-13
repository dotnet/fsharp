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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestUtilities
{
    public static class AssertUtil
    {
        public static void RequiresMta()
        {
            if (Thread.CurrentThread.GetApartmentState() != ApartmentState.MTA)
            {
                Assert.Inconclusive("Test requires MTA appartment to call COM reliably. Add solution item <root>\\Build\\Default.testsettings.");
            }

        }

        public static void Throws<TExpected>(Action throwingAction)
        {
            Throws<TExpected>(throwingAction, null);
        }

        public static void Throws<TExpected>(Action throwingAction, string description)
        {
            bool exceptionThrown = false;
            Type expectedType = typeof(TExpected);
            try
            {
                throwingAction();
            }
            catch (Exception ex)
            {
                exceptionThrown = true;
                Type thrownType = ex.GetType();
                if (!expectedType.IsAssignableFrom(thrownType))
                {
                    Assert.Fail("AssertUtil.Throws failure. Expected exception {0} not assignable from exception {1}, message: {2}", expectedType.FullName, thrownType.FullName, description);
                }
            }
            if (!exceptionThrown)
            {
                Assert.Fail("AssertUtil.Throws failure. Expected exception {0} but not exception thrown, message: {1}", expectedType.FullName, description);
            }
        }

        public static void MissingDependency(string dependency) {
            Assert.Inconclusive("Missing Dependency: {0}", dependency);
        }

        public static void ArrayEquals(IList expected, IList actual)
        {
            if (expected == null)
            {
                throw new ArgumentNullException("expected");
            }
            if (actual == null)
            {
                Assert.Fail("AssertUtils.ArrayEquals failure. Actual collection is null.");
            }

            if (expected.Count != actual.Count)
            {
                Assert.Fail("AssertUtils.ArrayEquals failure. Expected collection with length {0} but got collection with length {1}",
                    expected.Count, actual.Count);
            }
            for (int i = 0; i < expected.Count; i++)
            {
                if (!expected[i].Equals(actual[i]))
                {
                    Assert.Fail("AssertUtils.ArrayEquals failure. Expected value {0} at position {1} but got value {2}",
                        expected[i], i, actual[i]);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public static void ArrayEquals(IList expected, IList actual, Func<object, object, bool> comparison)
        {
            if (expected == null)
            {
                throw new ArgumentNullException("expected");
            }
            if (actual == null)
            {
                Assert.Fail("AssertUtils.ArrayEquals failure. Actual collection is null.");
            }
            if (comparison == null)
            {
                throw new ArgumentNullException("comparison");
            }

            if (expected.Count != actual.Count)
            {
                Assert.Fail("AssertUtils.ArrayEquals failure. Expected collection with length {0} but got collection with length {1}",
                    expected.Count, actual.Count);
            }
            for (int i = 0; i < expected.Count; i++)
            {
                if (!comparison(expected[i], actual[i]))
                {
                    Assert.Fail("AssertUtils.ArrayEquals failure. Expected value {0} at position {1} but got value {2}",
                        expected[i], i, actual[i]);
                }
            }
        }

        /// <summary>
        /// Asserts that two doubles are equal with regard to floating point error.
        /// Uses a default error message
        /// </summary>
        /// <param name="expected">Expected double value</param>
        /// <param name="actual">Actual double value</param>
        public static void DoublesEqual(double expected, double actual)
        {
            DoublesEqual(expected, actual, String.Format("AssertUtils.DoublesEqual failure. Expected value {0} but got value {1}", expected, actual));
        }

        /// <summary>
        /// Asserts that two doubles are equal with regard to floating point error
        /// </summary>
        /// <param name="expected">Expected double value</param>
        /// <param name="actual">Actual double value</param>
        /// <param name="error">Error message to display</param>
        public static void DoublesEqual(double expected, double actual, string error)
        {
            if (!(expected - actual < double.Epsilon && expected - actual > -double.Epsilon))
            {
                Assert.Fail(error);
            }
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static void Contains(string source, params string[] values) {
            foreach (var v in values) {
                if (!source.Contains(v)) {
                    Assert.Fail(String.Format("{0} does not contain {1}", source, v));
                }
            }
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static void Contains<T>(IEnumerable<T> source, T value) {
            foreach (var v in source) {
                if (v.Equals(value)) {
                    return;
                }
            }

            Assert.Fail(String.Format("{0} does not contain {1}", MakeText(source), value));
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static void AreEqual<T>(IEnumerable<T> source, params T[] value) {
            var items = source.ToArray();
            var message = string.Format(
                "Expected: <\n{0}\n>.\nActual: <\n{1}\n>.",
                string.Join("\n", value),
                string.Join("\n", items)
            );
            Console.WriteLine(message);

            Assert.AreEqual(value.Length, items.Length, message);
            for (int i = 0; i < value.Length; i++) {
                Assert.AreEqual(value[i], items[i]);
            }
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static void DoesntContain<T>(IEnumerable<T> source, T value) {
            foreach (var v in source) {
                if (v.Equals(value)) {
                    Assert.Fail(String.Format("{0} contains {1}", MakeText(source), value));
                }
            }

        }

        [System.Diagnostics.DebuggerStepThrough]
        public static void ContainsExactly<T>(IEnumerable<T> source, IEnumerable<T> expected) {
            ContainsExactly(new HashSet<T>(source), expected.ToArray());
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static void ContainsExactly<T>(IEnumerable<T> source, params T[] expected) {
            ContainsExactly(new HashSet<T>(source), expected);
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static void ContainsExactly<T>(HashSet<T> set, params T[] expected) {
            if (set.ContainsExactly(expected)) {
                return;
            }

            Assert.Fail(String.Format("ContainsExactly failed.\n\nExpected:\n{0}\n\nActual:\n{1}\n\nExpected not in actual:\n{2}\n\nActual not in expected:\n{3}",
                MakeText(expected),
                MakeText(set),
                MakeText(expected.Except(set)),
                MakeText(set.Except(expected))));
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static void ContainsAtLeast<T>(IEnumerable<T> source, IEnumerable<T> values) {
            ContainsAtLeast(new HashSet<T>(source), values.ToArray());
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static void ContainsAtLeast<T>(IEnumerable<T> source, params T[] values) {
            ContainsAtLeast(new HashSet<T>(source), values);
        }

        [System.Diagnostics.DebuggerStepThrough]
        public static void ContainsAtLeast<T>(HashSet<T> set, params T[] values) {
            if (set.IsSupersetOf(values)) {
                return;
            }
            var missing = new HashSet<T>(values);
            missing.ExceptWith(set);
            Assert.Fail(String.Format("Expected at least {0}, didn't find {1}. All: {2}",
                MakeText(values),
                MakeText(missing),
                MakeText(set)
            ));
        }

        public static string MakeText<T>(IEnumerable<T> values) {
            var ss = values.Select(x => x == null ? "(null)" : x.ToString()).ToArray();
            bool multiline = ss.Sum(s => s.Length) > 60;

            var sb = new StringBuilder("{");
            if (multiline) {
                sb.Append("\n");
            }

            bool first = true;
            foreach (var s in ss) {
                if (first) {
                    first = false;
                } else {
                    sb.Append(multiline ? ",\n" : ", ");
                }
                sb.Append(s);
            }

            if (multiline) {
                sb.Append("\n");
            }
            sb.Append("}");

            return sb.ToString();
        }


        public static void AreEqual(Regex expected, string actual, string message = null) {
            if (!expected.IsMatch(actual)) {
                Assert.Fail(string.Format("Expected <{0}>. Actual <{1}>. {2}", expected, actual, message ?? ""));
            }
        }

        public static void AreEqual(string expected, XmlDocument actual, string message = null) {
            var expectedDoc = new XmlDocument();
            expectedDoc.LoadXml(expected);
            AreEqual(expectedDoc, actual, message);
        }

        public static void AreEqual(XmlDocument expected, XmlDocument actual, string message = null) {
            var nav1 = expected.CreateNavigator();
            var nav2 = actual.CreateNavigator();

            if (string.IsNullOrEmpty(message)) {
                message = string.Empty;
            } else {
                message = " " + message;
            }

            AreXPathNavigatorsEqual(nav1, nav2, message);
        }

        private static string GetFullPath(XPathNavigator nav) {
            nav = nav.CreateNavigator();
            var names = new Stack<string>();

            names.Push(nav.Name);
            while (nav.MoveToParent()) {
                names.Push(nav.Name);
            }

            return "/" + string.Join("/", names);
        }

        private static void AreXPathNavigatorsEqual(XPathNavigator nav1, XPathNavigator nav2, string message) {
            while (true) {
                if (nav1.Name != nav2.Name) {
                    Assert.Fail("Expected element <{0}>. Actual element <{1}>.{2}", nav1.Name, nav2.Name, message);
                }
                var anav1 = nav1.CreateNavigator();
                var anav2 = nav2.CreateNavigator();
                var attr1 = new List<string>();
                var attr2 = new List<string>();

                if (anav1.MoveToFirstAttribute()) {
                    do {
                        attr1.Add(string.Format("{0}=\"{1}\"", anav1.Name, anav1.Value));
                    } while (anav1.MoveToNextAttribute());
                }
                if (anav2.MoveToFirstAttribute()) {
                    do {
                        attr2.Add(string.Format("{0}=\"{1}\"", anav2.Name, anav2.Value));
                    } while (anav2.MoveToNextAttribute());
                }

                AssertUtil.ContainsExactly(attr2, attr1);

                var cnav1 = nav1.CreateNavigator();
                var cnav2 = nav2.CreateNavigator();
                if (cnav1.MoveToFirstChild()) {
                    if (cnav2.MoveToFirstChild()) {
                        AreXPathNavigatorsEqual(cnav1, cnav2, message);
                    } else {
                        Assert.Fail("Expected element {0}.{1}", GetFullPath(cnav1), message);
                    }
                } else if (cnav2.MoveToFirstChild()) {
                    Assert.Fail("Unexpected element {0}.{1}", GetFullPath(cnav2), message);
                }

                if (nav1.MoveToNext()) {
                    if (nav2.MoveToNext()) {
                        continue;
                    } else {
                        Assert.Fail("Expected element {0}.{1}", GetFullPath(nav1), message);
                    }
                } else if (nav2.MoveToNext()) {
                    Assert.Fail("Unexpected element {0}.{1}", GetFullPath(nav2), message);
                }
                break;
            }
        }
    }
}
