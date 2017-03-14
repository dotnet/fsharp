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

using System.Windows;
using System.Windows.Controls;

namespace Microsoft.VisualStudioTools.Wpf {
    sealed class LabelledControl : ContentControl {
        public string Title {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(LabelledControl), new PropertyMetadata());

        public string HelpText {
            get { return (string)GetValue(HelpTextProperty); }
            set { SetValue(HelpTextProperty, value); }
        }

        public static readonly DependencyProperty HelpTextProperty = DependencyProperty.Register("HelpText", typeof(string), typeof(LabelledControl), new PropertyMetadata(HelpText_PropertyChanged));

        private static void HelpText_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            d.SetValue(HasHelpTextPropertyKey, !string.IsNullOrWhiteSpace(e.NewValue as string));
        }


        public bool HasHelpText {
            get { return (bool)GetValue(HasHelpTextProperty); }
            private set { SetValue(HasHelpTextPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey HasHelpTextPropertyKey = DependencyProperty.RegisterReadOnly("HasHelpText", typeof(bool), typeof(LabelledControl), new PropertyMetadata(false));
        public static readonly DependencyProperty HasHelpTextProperty = HasHelpTextPropertyKey.DependencyProperty;

    }
}
