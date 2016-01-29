// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.ProjectSystem
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using System.IO;
    using Microsoft.VisualStudio.Shell.Interop;
    using System.Globalization;
    using Microsoft.VisualStudio.Shell;
    using System.Windows.Forms.Design;

    internal partial class SecurityWarningDialog: Form
    {
        #region fields
        /// <summary>
        /// The associated service provider
        /// </summary>
        private IServiceProvider serviceProvider;

        /// <summary>
        /// The dialog message to be presented when the 'More' button is pressed.
        /// </summary>
        private string dialogMessage;

        /// <summary>
        /// Teh full path to teh project.
        /// </summary>
        private string projectFullPath;

        /// <summary>
        /// The value of the ask again check box.
        /// </summary>
        private bool askAgainCheckBoxValue;

        /// <summary>
        /// The project load option the userwill choose on this form.
        /// </summary>
        private ProjectLoadOption projectLoadOption = ProjectLoadOption.DonNotLoad;
        #endregion

        #region properties
        /// <summary>
        /// The value of the ask again check box.
        /// </summary>
        /*internal, but public for FSharp.Project.dll*/ public bool AskAgainCheckBoxValue
        {
            get
            {
                return this.askAgainCheckBoxValue;
            }
        }

        /// <summary>
        /// The project load option the user has chosen to perform.
        /// </summary>
        internal ProjectLoadOption ProjectLoadOption
        {
            get
            {
                return this.projectLoadOption;
            }
        }
        #endregion

        #region ctors
        /// <summary>
        /// Overloaded ctor.
        /// </summary>
        /// <param name="serviceProvider">The associated service provider.</param>
        /// <param name="dialogMessage">The message that will be shown when the 'More' button is pressed.</param>
        /// <param name="projectFullpath">The full path of the project.</param>
        public SecurityWarningDialog(IServiceProvider serviceProvider, string dialogMessage, string projectFullpath)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }

            if (String.IsNullOrEmpty(projectFullpath))
            {
                throw new ArgumentException(SR.GetString(SR.ParameterCannotBeNullOrEmpty, CultureInfo.CurrentUICulture), "projectFullpath");
            }

            this.serviceProvider = serviceProvider;

            this.projectFullPath = projectFullpath;

            this.dialogMessage = dialogMessage;

            this.InitializeComponent();

            this.SetupComponents();
        }
        #endregion

        #region helpers
        /// <summary>
        /// Shows the dialog if possible hosted by the IUIService.
        /// </summary>
        /// <returns>A DialogResult</returns>
        /*internal, but public for FSharp.Project.dll*/ public new DialogResult ShowDialog()
        { 
            IUIService uiService = this.serviceProvider.GetService(typeof(IUIService)) as IUIService;
            if (uiService == null)
            {
                return this.ShowDialog();
            }

            return uiService.ShowDialog(this);
        }
        
        /// <summary>
        /// Sets up the different UI elements.
        /// </summary>
        private void SetupComponents()
        { 
            // Get the project name.
            string projectFileName = Path.GetFileName(this.projectFullPath);
            string projectName = Path.GetFileNameWithoutExtension(this.projectFullPath);
            
            IVsUIShell shell = this.serviceProvider.GetService(typeof(SVsUIShell)) as IVsUIShell;

            if (shell == null)
            {
                throw new InvalidOperationException();
            }
             
            String applicationName;
            
            // Get the name of the SKU.
            shell.GetAppName(out applicationName);
            
            // Set the dialog box caption (title).
            this.Text = String.Format(CultureInfo.CurrentCulture, this.Text, projectName);

            // Set the text at the top of the dialog that gives a brief description of the security
            // implications of loading this project.
            this.warningText.Text = String.Format(CultureInfo.CurrentCulture, this.warningText.Text, projectName, applicationName);

            // Set the text that describes the "Browse" option.
            this.browseText.Text = String.Format(CultureInfo.CurrentCulture, this.browseText.Text, applicationName);

            // Set the text that describes the "Load" option.
            this.loadText.Text = String.Format(CultureInfo.CurrentCulture, this.loadText.Text, applicationName);

             // The default selection is "Browse" so select that radio button.
            this.browseButton.Checked = true;
    
             // Turn on the "Ask me always" checkbox by default.
            this.askAgainCheckBox.Checked = true;
        
            // Set the focus to the Browse button, so hitting Enter will press the OK button
            this.browseButton.Focus();

            this.CenterToScreen();
        }

        /// <summary>
        /// The Cancel button was clicked.
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">An event arg Associated to the event.</param>
        private void cancelButton_Click(object sender, EventArgs e)
        {
            // In case the user presses the Cancel button, we assume he never wants to see this dialog again
            // and pretend the "Ask me every time" checkbox is unchecked even if it is really checked.
            this.askAgainCheckBoxValue = false;
            this.projectLoadOption = ProjectLoadOption.DonNotLoad;           
        }

        /// <summary>
        /// The OK button was clicked.
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">An event arg Associated to the event.</param>
        private void okButton_Click(object sender, EventArgs e)
        {
            if (this.browseButton.Checked && !this.loadButton.Checked)
            {
                this.projectLoadOption = ProjectLoadOption.LoadOnlyForBrowsing;
            }
            else
            {
                this.projectLoadOption = ProjectLoadOption.LoadNormally;
            }
            
            this.askAgainCheckBoxValue = this.askAgainCheckBox.Checked;

            this.Close();
        }

        /// <summary>
        /// Loads a messagebox explaining in detail the security problem
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">An event arg Associated to the event.</param>
        private void detailsButton_Click(object sender, EventArgs e)
        {
            string title = null;
            OLEMSGICON icon = OLEMSGICON.OLEMSGICON_INFO;
            OLEMSGBUTTON buttons = OLEMSGBUTTON.OLEMSGBUTTON_OK;
            OLEMSGDEFBUTTON defaultButton = OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST;
            VsShellUtilities.ShowMessageBox(this.serviceProvider, title, this.dialogMessage, icon, buttons, defaultButton);
        }
        
        #endregion
    }
}
