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
using System.Windows;
using System.Windows.Automation;
using System.Windows.Input;

namespace TestUtilities.UI
{
    public class TreeNode : AutomationWrapper, ITreeNode
    {
        public TreeNode(AutomationElement element)
            : base(element) {
        }

        public new void Select()
        {
            try
            {
                var parent = Element.GetSelectionItemPattern().Current.SelectionContainer;
                foreach (var item in parent.GetSelectionPattern().Current.GetSelection())
                {
                    item.GetSelectionItemPattern().RemoveFromSelection();
                }
                Element.GetSelectionItemPattern().AddToSelection();
            }
            catch (InvalidOperationException)
            {
                // Control does not support this pattern, so let's just click
                // on it.
                var point = Element.GetClickablePoint();
                point.Offset(0.0, 50.0);
                Mouse.MoveTo(point);
                System.Threading.Thread.Sleep(100);
                point.Offset(0.0, -50.0);
                Mouse.MoveTo(point);
                System.Threading.Thread.Sleep(100);
                Mouse.Click(System.Windows.Input.MouseButton.Left);
                System.Threading.Thread.Sleep(100);
            }
        }

        void ITreeNode.Select() {
            base.Select();
        }

        void ITreeNode.AddToSelection() {
            AutomationWrapper.AddToSelection(Element);
        }

        public void Deselect()
        {
            Element.GetSelectionItemPattern().RemoveFromSelection();
        }

        public string Value
        {
            get
            {
                return this.Element.Current.Name.ToString();
            }
        }

        public bool IsExpanded
        {
            get
            {
                switch (Element.GetExpandCollapsePattern().Current.ExpandCollapseState)
                {
                    case ExpandCollapseState.Collapsed:
                        return false;
                    case ExpandCollapseState.Expanded:
                        return true;
                    case ExpandCollapseState.LeafNode:
                        return true;
                    case ExpandCollapseState.PartiallyExpanded:
                        return false;
                    default:
                        return false;
                }
            }
            set
            {
                if (value)
                {
                    Element.GetExpandCollapsePattern().Expand();
                }
                else
                {
                    Element.GetExpandCollapsePattern().Collapse();
                }
            }
        }

        public List<TreeNode> Nodes
        {
            get
            {
                return Element.FindAll(
                    TreeScope.Children,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TreeItem)
                )
                    .OfType<AutomationElement>()
                    .Select(e => new TreeNode(e))
                    .ToList();
            }
        }

        public void ExpandCollapse()
        {
            try {
                var pattern = Element.GetExpandCollapsePattern();
                switch (pattern.Current.ExpandCollapseState)
                {
                    case ExpandCollapseState.Collapsed:
                        pattern.Expand();
                        break;
                    case ExpandCollapseState.Expanded:
                        pattern.Collapse();
                        break;
                    case ExpandCollapseState.LeafNode:
                        break;
                    case ExpandCollapseState.PartiallyExpanded:
                        pattern.Expand();
                        break;
                    default:
                        break;
                }
            } catch (InvalidOperationException) {
                Element.GetInvokePattern().Invoke();
            }
        }

        public void DoubleClick()
        {
            Element.GetInvokePattern().Invoke();
        }

        public void ShowContextMenu()
        {
            Select();
            System.Threading.Thread.Sleep(100);
            Mouse.Click(System.Windows.Input.MouseButton.Right);
            System.Threading.Thread.Sleep(100);
        }

        /// <summary>
        /// Selects the provided items with the mouse preparing for a drag and drop
        /// </summary>
        /// <param name="source"></param>
        private static void SelectItemsForDragAndDrop(ITreeNode[] source) {
            AutomationWrapper.Select(((TreeNode)source.First()).Element);
            for (int i = 1; i < source.Length; i++) {
                AutomationWrapper.AddToSelection(((TreeNode)source[i]).Element);
            }

            Mouse.MoveTo(((TreeNode)source.Last()).Element.GetClickablePoint());
            Mouse.Down(MouseButton.Left);
        }


        public void DragOntoThis(params ITreeNode[] source) {
            DragOntoThis(Key.None, source);
        }

        public void DragOntoThis(Key modifier, params ITreeNode[] source) {
            SelectItemsForDragAndDrop(source);

            try {
                try {
                    if (modifier != Key.None) {
                        Keyboard.Press(modifier);
                    }
                    var dest = Element;
                    if (source.Length == 1 && source[0] == this) {
                        // dragging onto ourself, the mouse needs to move
                        var point = dest.GetClickablePoint();
                        Mouse.MoveTo(new Point(point.X + 1, point.Y + 1));
                    } else {
                        Mouse.MoveTo(dest.GetClickablePoint());
                    }
                } finally {
                    Mouse.Up(MouseButton.Left);
                }
            } finally {
                if (modifier != Key.None) {
                    Keyboard.Release(modifier);
                }
            }
        }

    }
}
