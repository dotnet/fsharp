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
using System.Linq;
using System.Windows.Automation;

namespace TestUtilities.UI {
    public class TreeView : AutomationWrapper {

        public TreeView(AutomationElement element)
            : base(element) {
        }

        protected AutomationElement WaitForItemHelper(Func<string[], AutomationElement> getItem, string[] path) {
            AutomationElement item = null;
            for (int i = 0; i < 40; i++) {
                item = getItem(path);
                if (item != null) {
                    break;
                }
                System.Threading.Thread.Sleep(250);
            }
            if (item == null) {
                Console.WriteLine("Failed to find {0}", String.Join("\\", path));
                DumpElement(Element);
            }
            return item;
        }

        /// <summary>
        /// Waits for the item in the solution tree to be available for up to 10 seconds.
        /// </summary>
        public AutomationElement WaitForItem(params string[] path) {
            return WaitForItemHelper(FindItem, path);
        }

        protected AutomationElement WaitForItemRemovedHelper(Func<string[], AutomationElement> getItem, string[] path) {
            AutomationElement item = null;
            for (int i = 0; i < 40; i++) {
                item = getItem(path);
                if (item == null) {
                    break;
                }
                System.Threading.Thread.Sleep(250);
            }
            return item;
        }

        /// <summary>
        /// Waits for the item in the solution tree to be removed within 10 seconds.
        /// </summary>
        public AutomationElement WaitForItemRemoved(params string[] path) {
            return WaitForItemRemovedHelper(FindItem, path);
        }


        /// <summary>
        /// Finds the specified item in the solution tree and returns it.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public AutomationElement FindItem(params string[] path) {
            return FindNode(Element.FindAll(TreeScope.Children, Condition.TrueCondition), path, 0);
        }

        protected static AutomationElement FindNode(AutomationElementCollection nodes, string[] splitPath, int depth) {
            for (int i = 0; i < nodes.Count; i++) {
                var node = nodes[i];
                var name = node.GetCurrentPropertyValue(AutomationElement.NameProperty) as string;

                if (name.Equals(splitPath[depth], StringComparison.CurrentCulture)) {
                    if (depth == splitPath.Length - 1) {
                        return node;
                    }

                    // ensure the node is expanded...
                    try {
                        EnsureExpanded(node);
                    } catch (InvalidOperationException) {
                        // handle race w/ items being removed...
                        Console.WriteLine("Failed to expand {0}", splitPath[depth]);
                    }
                    return FindNode(node.FindAll(TreeScope.Children, Condition.TrueCondition), splitPath, depth + 1);
                }
            }
            return null;
        }

        /// <summary>
        /// return all visible nodes
        /// </summary>
        public List<TreeNode> Nodes {
            get {
                return Element.FindAll(
                    TreeScope.Children,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TreeItem)
                )
                    .OfType<AutomationElement>()
                    .Select(e => new TreeNode(e))
                    .ToList();
            }
        }

        /// <summary>
        /// Gets or sets a single selected item or null if no item is selected.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Multiple items are selected.
        /// </exception>
        public TreeNode SelectedItem {
            get {
                var selected = Element.GetSelectionPattern().Current.GetSelection().SingleOrDefault();
                return selected == null ? null : new TreeNode(selected);
            }
            set {
                foreach (var selected in Element.GetSelectionPattern().Current.GetSelection()) {
                    selected.GetSelectionItemPattern().RemoveFromSelection();
                }
                if (value != null) {
                    value.Select();
                }
            }
        }

        public IList<TreeNode> SelectedItems {
            get {
                return Element.GetSelectionPattern().Current.GetSelection().Select(e => new TreeNode(e)).ToArray();
            }
            set {
                foreach (var selected in Element.GetSelectionPattern().Current.GetSelection()) {
                    selected.GetSelectionItemPattern().RemoveFromSelection();
                }
                if (value != null) {
                    foreach (var item in value) {
                        item.Select();
                    }
                }
            }
        }

        /// <summary>
        /// Expands all nodes in the treeview.
        /// </summary>
        /// <returns>The total number of nodes in the treeview.</returns>
        public int ExpandAll() {
            var count = 0;
            var nodes = new Queue<TreeNode>(Nodes);
            while (nodes.Any()) {
                count += 1;
                var node = nodes.Dequeue();
                node.IsExpanded = true;
                foreach (var n in node.Nodes) {
                    nodes.Enqueue(n);
                }
            }
            return count;
        }

        public void CenterInView(AutomationElement node) {
            var treeBounds = (System.Windows.Rect)Element.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
            var lowHeight = treeBounds.Height / 2 - 10;
            var highHeight = treeBounds.Height / 2 + 10;

            var scroll = Element.GetScrollPattern();
            if (!scroll.Current.VerticallyScrollable) {
                return;
            }
            scroll.SetScrollPercent(ScrollPattern.NoScroll, 0);

            while (true) {
                var nodeBounds = (System.Windows.Rect)node.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
                var heightFromTop = nodeBounds.Top - treeBounds.Top;
                if (lowHeight < heightFromTop && heightFromTop < highHeight) {
                    break;
                } else if (heightFromTop >= 0 && heightFromTop < lowHeight) {
                    break;
                } else if (scroll.Current.VerticalScrollPercent == 100.0) {
                    break;
                }

                scroll.ScrollVertical(ScrollAmount.SmallIncrement);
            }
        }
    }
}
