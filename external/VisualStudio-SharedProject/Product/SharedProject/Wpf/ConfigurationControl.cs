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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.VisualStudioTools.Wpf {
    sealed class ConfigurationTextBoxWithHelp : Control {
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(string), typeof(ConfigurationTextBoxWithHelp), new PropertyMetadata());
        public static readonly DependencyProperty HelpTextProperty = DependencyProperty.Register("HelpText", typeof(string), typeof(ConfigurationTextBoxWithHelp), new PropertyMetadata());
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ConfigurationTextBoxWithHelp), new PropertyMetadata());
        public static readonly DependencyProperty BrowseButtonStyleProperty = DependencyProperty.Register("BrowseButtonStyle", typeof(Style), typeof(ConfigurationTextBoxWithHelp), new PropertyMetadata());
        public static readonly DependencyProperty BrowseCommandParameterProperty = DependencyProperty.Register("BrowseCommandParameter", typeof(object), typeof(ConfigurationTextBoxWithHelp), new PropertyMetadata());

        public string Watermark {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        public string HelpText {
            get { return (string)GetValue(HelpTextProperty); }
            set { SetValue(HelpTextProperty, value); }
        }

        public string Text {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Style BrowseButtonStyle {
            get { return (Style)GetValue(BrowseButtonStyleProperty); }
            set { SetValue(BrowseButtonStyleProperty, value); }
        }

        public object BrowseCommandParameter {
            get { return (object)GetValue(BrowseCommandParameterProperty); }
            set { SetValue(BrowseCommandParameterProperty, value); }
        }
    }

    sealed class ConfigurationComboBoxWithHelp : Control {
        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(string), typeof(ConfigurationComboBoxWithHelp), new PropertyMetadata());
        public static readonly DependencyProperty HelpTextProperty = DependencyProperty.Register("HelpText", typeof(string), typeof(ConfigurationComboBoxWithHelp), new PropertyMetadata());
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(ConfigurationComboBoxWithHelp), new PropertyMetadata());
        public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register("Values", typeof(IList<string>), typeof(ConfigurationComboBoxWithHelp), new PropertyMetadata());

        public string Watermark {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        public string HelpText {
            get { return (string)GetValue(HelpTextProperty); }
            set { SetValue(HelpTextProperty, value); }
        }

        public string Value {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        
        public IList<string> Values {
            get { return (IList<string>)GetValue(ValuesProperty); }
            set { SetValue(ValuesProperty, value); }
        }
    }
}