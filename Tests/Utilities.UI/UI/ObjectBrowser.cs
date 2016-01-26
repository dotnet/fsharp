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

using System.Windows.Automation;

namespace TestUtilities.UI {
    public class ObjectBrowser : AutomationWrapper {
        private TypeBrowserPane _typeBrowserPane;
        private TypeNavigatorPane _typeNavigatorPane;
        private DetailPane _detailPane;
        private TextBox _searchText;
        private Button _searchButton;
        private Button _clearSearchButton;
        private Button _backButton;
        private Button _forwardButton;

        public ObjectBrowser(AutomationElement element)
            : base(element) {
        }

        /// <summary>
        /// Returns the Type Browser Pane (left pane)
        /// </summary>
        /// <history>
        /// [xiangyan] 1/26/2011 created
        /// </history>
        public TypeBrowserPane TypeBrowserPane {
            get {
                if (_typeBrowserPane == null) {
                    Condition con = new AndCondition(
                                new PropertyCondition(
                                    AutomationElement.ClassNameProperty,
                                    "LiteTreeView32"
                                ),
                                new PropertyCondition(
                                    AutomationElement.NameProperty,
                                    "VsObjectBrowserTypesPane"
                                )
                            );
                    AutomationElement el = this.Element.FindFirst(TreeScope.Descendants, con);
                    if (el != null)
                        _typeBrowserPane = new TypeBrowserPane(el);
                }
                return _typeBrowserPane;
        }
        }

        /// <summary>
        /// Returns the Type Navigation Pane (right pane - shows members)
        /// </summary>
        /// <history>
        /// [xiangyan] 1/26/2011 created
        /// </history>
        public TypeNavigatorPane TypeNavigatorPane {
            get {
                if (_typeNavigatorPane == null) {
                    Condition con = new AndCondition(
                                new PropertyCondition(
                                    AutomationElement.ClassNameProperty,
                                    "LiteTreeView32"
                                ),
                                new PropertyCondition(
                                    AutomationElement.NameProperty,
                                    "VsObjectBrowserMembersPane"
                                )
                            );
                    AutomationElement el = this.Element.FindFirst(TreeScope.Descendants, con);
                    if (el != null)
                        _typeNavigatorPane = new TypeNavigatorPane(el);
                }
                return _typeNavigatorPane;
        }
        }

        /// <summary>
        /// Returns Detail Pane - richedit textbox with member description
        /// </summary>
        /// <history>
        /// [xiangyan] 1/26/2011 created
        /// </history>
        public DetailPane DetailPane {
            get {
                if (_detailPane == null) {
                    Condition con = new AndCondition(
                                new PropertyCondition(
                                    AutomationElement.ClassNameProperty,
                                    "RICHEDIT50W"
                                ),
                                new PropertyCondition(
                                    AutomationElement.NameProperty,
                                    "VsObjectBrowserDescriptionPane"
                                )
                            );
                    AutomationElement el = this.Element.FindFirst(TreeScope.Descendants, con);
                    if (el != null)
                        _detailPane = new DetailPane(el);
                }
                return _detailPane;
        }
        }

        /// <summary>
        /// Returns Search textbox
        /// </summary>
        /// <history>
        /// [xiangyan] 1/28/2011 created
        /// </history>
        public TextBox SearchText {
            get {
                if (_searchText == null) {
                    Condition con = new AndCondition(
                                new PropertyCondition(
                                    AutomationElement.ClassNameProperty,
                                    "TextBox"
                                ),
                                new PropertyCondition(
                                    AutomationElement.AutomationIdProperty,
                                    "PART_EditableTextBox"
                                )
                            );
                    AutomationElement el = this.Element.FindFirst(TreeScope.Descendants, con);

                    if (el != null) {
                        _searchText = new TextBox(el);
                    }
                }
                return _searchText;
            }
        }

        /// <summary>
        /// Returns Search button
        /// </summary>
        /// <history>
        /// [xiangyan] 1/28/2011 created
        /// </history>
        public Button SearchButton {
            get {
                if (_searchButton == null) {
                    Condition con = new AndCondition(
                                new PropertyCondition(
                                    AutomationElement.ClassNameProperty,
                                    "Button"
                                ),
                                new PropertyCondition(
                                    AutomationElement.NameProperty,
                                    "Object Browser Search"
                                )
                            );
                    AutomationElementCollection ell = this.Element.FindAll(TreeScope.Descendants, con);

                    if (ell.Count == 2) {
                        _searchButton = new Button(ell[1]);
                    }
                }
                return _searchButton;
            }
        }

        /// <summary>
        /// Returns Clear Search button
        /// </summary>
        /// <history>
        /// [xiangyan] 1/28/2011 created
        /// </history>
        public Button ClearSearchButton {
            get {
                if (_clearSearchButton == null) {
                    Condition con = new AndCondition(
                                new PropertyCondition(
                                    AutomationElement.ClassNameProperty,
                                    "Button"
                                ),
                                new PropertyCondition(
                                    AutomationElement.NameProperty,
                                    "Object Browser Clear Search"
                                )
                            );
                    AutomationElement el = this.Element.FindFirst(TreeScope.Descendants, con);
                    if (el != null) {
                        _clearSearchButton = new Button(el);
                    }
                }
                return _clearSearchButton;
            }
        }

        /// <summary>
        /// Returns Back button
        /// </summary>
        /// <history>
        /// [xiangyan] 1/28/2011 created
        /// </history>
        public Button BackButton {
            get {
                if (_backButton == null) {
                    Condition con = new AndCondition(
                                new PropertyCondition(
                                    AutomationElement.ClassNameProperty,
                                    "Button"
                                ),
                                new PropertyCondition(
                                    AutomationElement.NameProperty,
                                    "Object Browser Back"
                                )
                            );
                    AutomationElement el = this.Element.FindFirst(TreeScope.Descendants, con);
                    if (el != null) {
                        _backButton = new Button(el);
                    }
                }
                return _backButton;
            }
        }

        /// <summary>
        /// Returns Forward button
        /// </summary>
        /// <history>
        /// [xiangyan] 1/28/2011 created
        /// </history>
        public Button ForwardButton {
            get {
                if (_forwardButton == null) {
                    Condition con = new AndCondition(
                                new PropertyCondition(
                                    AutomationElement.ClassNameProperty,
                                    "Button"
                                ),
                                new PropertyCondition(
                                    AutomationElement.NameProperty,
                                    "Object Browser Forward"
                                )
                            );
                    AutomationElement el = this.Element.FindFirst(TreeScope.Descendants, con);
                    if (el != null) {
                        _forwardButton = new Button(el);
                    }
                }
                return _forwardButton;
            }
        }

        public void EnsureLoaded() {
            for (int i = 0; i < 30; i++) {
                var node = TypeBrowserPane.Nodes[0];
                if (node.Value != "No information. Try browsing a different component set.") {
                    break;
                }
                System.Threading.Thread.Sleep(100);
            }

        }
    }
}
