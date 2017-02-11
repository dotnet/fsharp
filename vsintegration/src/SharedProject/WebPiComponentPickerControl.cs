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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.VisualStudioTools.Project {
    [Guid("B7773A32-2EE5-4844-9630-F14768A5D03C")]
    partial class WebPiComponentPickerControl : UserControl {
        private readonly List<PackageInfo> _packages = new List<PackageInfo>();
        private const string _defaultFeeds = "https://www.microsoft.com/web/webpi/5.0/webproductlist.xml";
        private ListViewSorter _sorter = new ListViewSorter();

        public WebPiComponentPickerControl() {
            InitializeComponent();

            AutoSize = true;
            AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            _productsList.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(ProductsListItemSelectionChanged);
            _productsList.ListViewItemSorter = _sorter;
            _productsList.DoubleClick += new EventHandler(ProductsListDoubleClick);
            _productsList.ColumnClick += new ColumnClickEventHandler(ProductsListColumnClick);
        }

        private void ProductsListColumnClick(object sender, ColumnClickEventArgs e) {
            if (e.Column == _sorter.Column) {
                if (_sorter.Order == SortOrder.Ascending) {
                    _sorter.Order = SortOrder.Descending;
                } else {
                    _sorter.Order = SortOrder.Ascending;
                }
            } else {
                _sorter.Column = e.Column;
                _sorter.Order = SortOrder.Ascending;
            }
            _productsList.Sort();
        }

        private void ProductsListDoubleClick(object sender, EventArgs e) {
            NativeMethods.SendMessage(
                NativeMethods.GetParent(NativeMethods.GetParent(NativeMethods.GetParent(Handle))),
                (uint)VSConstants.CPDN_SELDBLCLICK,
                IntPtr.Zero,
                Handle
            );
        }

        private void ProductsListItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
            NativeMethods.SendMessage(
                NativeMethods.GetParent(NativeMethods.GetParent(NativeMethods.GetParent(Handle))),
                (uint)VSConstants.CPDN_SELCHANGED,
                IntPtr.Zero,
                Handle
            );
        }

        private async Task RequestFeeds(string feedSource) {
            try {
                await Task.Run(() => GetFeeds(feedSource));
            } catch (Exception ex) {
                if (ex.IsCriticalException()) {
                    throw;
                }

                MessageBox.Show(SR.GetString(SR.WebPiFeedError, feedSource, ex.Message));

                var fullMessage = SR.GetString(SR.WebPiFeedError, feedSource, ex);
                Trace.WriteLine(fullMessage);
                try {
                    ActivityLog.LogError("WebPiComponentPickerControl", fullMessage);
                } catch (InvalidOperationException) {
                }
            }
        }

        private void GetFeeds(string feed) {
            var doc = new XPathDocument(feed);
            XmlNamespaceManager mngr = new XmlNamespaceManager(new NameTable());
            mngr.AddNamespace("x", "http://www.w3.org/2005/Atom");

            var nav = doc.CreateNavigator();

            var nodes = nav.Select("/x:feed/x:entry", mngr);
            foreach (XPathNavigator node in nodes) {
                var title = node.Select("x:title", mngr);
                var updated = node.Select("x:updated", mngr);
                var productId = node.Select("x:productId", mngr);

                string titleVal = null;
                foreach (XPathNavigator titleNode in title) {
                    titleVal = titleNode.Value;
                    break;
                }

                string updatedVal = null;
                foreach (XPathNavigator updatedNode in updated) {
                    updatedVal = updatedNode.Value;
                }


                string productIdVal = null;
                foreach (XPathNavigator productIdNode in productId) {
                    productIdVal = productIdNode.Value;
                }

                if (titleVal != null && updatedVal != null && productIdVal != null) {
                    var newPackage = new PackageInfo(
                        titleVal,
                        updatedVal,
                        productIdVal,
                        feed
                    );

                    _packages.Add(newPackage);

                    try {
                        BeginInvoke(new Action<object>(AddPackage), newPackage);
                    } catch (InvalidOperationException) {
                        break;
                    }
                }
            }
        }

        private void AddPackage(object package) {
            var pkgInfo = (PackageInfo)package;
            var item = new ListViewItem(
                new[] { 
                    pkgInfo.Title,
                    pkgInfo.Updated,
                    pkgInfo.ProductId,
                    pkgInfo.Feed
                }
            );

            item.Tag = pkgInfo;
            _productsList.Items.Add(item);
        }

        class PackageInfo {
            public readonly string Title;
            public readonly string Updated;
            public readonly string ProductId;
            public readonly string Feed;

            public PackageInfo(string title, string updated, string productId, string feed) {
                Title = title;
                Updated = updated;
                ProductId = productId;
                Feed = feed;
            }
        }

        class ListViewSorter : IComparer {
            public SortOrder Order;
            public int Column;

            #region IComparer Members

            public int Compare(object x, object y) {
                ListViewItem itemX = (ListViewItem)x;
                ListViewItem itemY = (ListViewItem)y;

                int? res = null;
                if (Column == 1) {
                    DateTime dtX, dtY;
                    if (DateTime.TryParse(itemX.SubItems[1].Text, out dtX) &&
                        DateTime.TryParse(itemY.SubItems[1].Text, out dtY)) {
                        res = dtX.CompareTo(dtY);
                    }
                }

                if (res == null) {
                    res = String.Compare(
                       itemX.SubItems[0].Text,
                       itemY.SubItems[0].Text,
                       true
                   );
                }

                if (Order == SortOrder.Descending) {
                    return -res.Value;
                }
                return res.Value;
            }

            #endregion
        }
        private void AddNewFeedClick(object sender, EventArgs e) {
            RequestFeeds(_newFeedUrl.Text).DoNotWait();
        }

        protected override void DefWndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_INITDIALOG:
                    SetWindowStyleOnStaticHostControl();
                    goto default;
                case VSConstants.CPPM_INITIALIZELIST:
                    RequestFeeds(_defaultFeeds).DoNotWait();
                    break;
                case VSConstants.CPPM_SETMULTISELECT:
                    _productsList.MultiSelect = (m.WParam != IntPtr.Zero);
                    break;
                case VSConstants.CPPM_CLEARSELECTION:
                    _productsList.SelectedItems.Clear();
                    break;
                case VSConstants.CPPM_QUERYCANSELECT:
                    Marshal.WriteInt32(
                        m.LParam,
                        (_productsList.SelectedItems.Count > 0) ? 1 : 0
                    );
                    break;
                case VSConstants.CPPM_GETSELECTION:
                    var items = new PackageInfo[this._productsList.SelectedItems.Count];
                    for (int i = 0; i < items.Length; i++) {
                        items[i] = (PackageInfo)_productsList.SelectedItems[0].Tag;
                    }
                    int count = items != null ? items.Length : 0;
                    Marshal.WriteByte(m.WParam, Convert.ToByte(count));
                    if (count > 0) {
                        IntPtr ppItems = Marshal.AllocCoTaskMem(
                          count * Marshal.SizeOf(typeof(IntPtr)));
                        for (int i = 0; i < count; i++) {
                            IntPtr pItem = Marshal.AllocCoTaskMem(
                                    Marshal.SizeOf(typeof(VSCOMPONENTSELECTORDATA)));
                            Marshal.WriteIntPtr(
                                ppItems + i * Marshal.SizeOf(typeof(IntPtr)),
                                pItem);
                            VSCOMPONENTSELECTORDATA data = new VSCOMPONENTSELECTORDATA() {
                                dwSize = (uint)Marshal.SizeOf(typeof(VSCOMPONENTSELECTORDATA)),
                                bstrFile = items[i].Feed,
                                bstrTitle = items[i].ProductId,
                                bstrProjRef = items[i].Title,
                                type = VSCOMPONENTTYPE.VSCOMPONENTTYPE_Custom
                            };
                            Marshal.StructureToPtr(data, pItem, false);
                        }
                        Marshal.WriteIntPtr(m.LParam, ppItems);
                    }
                    break;
                case NativeMethods.WM_SIZE:
                    IntPtr parentHwnd = NativeMethods.GetParent(Handle);

                    if (parentHwnd != IntPtr.Zero) {
                        IntPtr grandParentHwnd = NativeMethods.GetParent(parentHwnd);

                        User32RECT parentClientRect, grandParentClientRect;
                        if (grandParentHwnd != IntPtr.Zero &&
                            NativeMethods.GetClientRect(parentHwnd, out parentClientRect) &&
                                NativeMethods.GetClientRect(grandParentHwnd, out grandParentClientRect)) {

                            int width = grandParentClientRect.Width;
                            int height = grandParentClientRect.Height;

                            if ((parentClientRect.Width != width) || (parentClientRect.Height != height)) {
                                NativeMethods.MoveWindow(parentHwnd, 0, 0, width, height, true);
                                this.Width = width;
                                this.Height = height;
                            }
                            this.Refresh();
                        }
                    }
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void NewFeedUrlTextChanged(object sender, EventArgs e) {
            try {
                new Uri(_newFeedUrl.Text);
                _addNewFeed.Enabled = true;
            } catch (UriFormatException) {
                _addNewFeed.Enabled = false;
            }
        }

        /// <summary>
        /// VS hosts us in a static control ("This is a static!") but that control doesn't
        /// have the WS_EX_CONTROLPARENT style like it should (and like our UserControl properly
        /// gets because it's a ContainerControl).  This causes an infinite loop when we start 
        /// trying to loop through the controls if the user navigates away from the control.
        /// 
        /// http://support.microsoft.com/kb/149501
        /// 
        /// So we go in and muck about with the This is a static! control's window style so that
        /// we don't hang VS if the user alt-tabs away.
        /// </summary>
        private void SetWindowStyleOnStaticHostControl() {
            var target = (NativeMethods.GetParent(Handle));
            NativeMethods.SetWindowLong(
                target,
                NativeMethods.GWL_EXSTYLE,
                NativeMethods.WS_EX_CONTROLPARENT | NativeMethods.GetWindowLong(target, NativeMethods.GWL_EXSTYLE)
            );
        }
    }
}

