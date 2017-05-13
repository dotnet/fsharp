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

using System.ComponentModel.Composition;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

#if NTVS_FEATURE_INTERACTIVEWINDOW
namespace Microsoft.NodejsTools.Repl {
#else
namespace Microsoft.VisualStudio.Repl {
#endif
    [Export(typeof(IReplCommand))]
    class ClearScreenReplCommand : IReplCommand {
        #region IReplCommand Members

        public Task<ExecutionResult> Execute(IReplWindow window, string arguments) {
            window.ClearScreen();
            return ExecutionResult.Succeeded;
        }

        public string Description {
            get { return "Clears the contents of the REPL editor window"; }
        }

        public string Command {
            get { return "cls"; }
        }

        public object ButtonContent {
            get {
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = Assembly.GetExecutingAssembly().GetManifestResourceStream("Microsoft.VisualStudio.Resources.clearscr.gif");
                image.EndInit();

                var res = new Image();
                res.Source = image;
                res.Opacity = 1.0;
                res.Width = res.Height = 16;
                return res;

            }
        }

        #endregion
    }
}
