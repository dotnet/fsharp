// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.ProjectSystem
{
    partial class SecurityWarningDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SecurityWarningDialog));
            this.warningText = new System.Windows.Forms.Label();
            this.detailsButton = new System.Windows.Forms.Button();
            this.optionBox = new System.Windows.Forms.GroupBox();
            this.loadButton = new System.Windows.Forms.RadioButton();
            this.browseButton = new System.Windows.Forms.RadioButton();
            this.loadText = new System.Windows.Forms.Label();
            this.browseText = new System.Windows.Forms.Label();
            this.askAgainCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.optionBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // warningText
            // 
            resources.ApplyResources(this.warningText, "warningText");
            this.warningText.Name = "warningText";
            // 
            // detailsButton
            // 
            resources.ApplyResources(this.detailsButton, "detailsButton");
            this.detailsButton.Name = "detailsButton";
            this.detailsButton.UseVisualStyleBackColor = true;
            this.detailsButton.Click += new System.EventHandler(this.detailsButton_Click);
            // 
            // optionBox
            // 
            this.optionBox.Controls.Add(this.loadButton);
            this.optionBox.Controls.Add(this.browseButton);
            this.optionBox.Controls.Add(this.loadText);
            this.optionBox.Controls.Add(this.browseText);
            resources.ApplyResources(this.optionBox, "optionBox");
            this.optionBox.Name = "optionBox";
            this.optionBox.TabStop = false;
            // 
            // loadButton
            // 
            resources.ApplyResources(this.loadButton, "loadButton");
            this.loadButton.Name = "loadButton";
            this.loadButton.TabStop = true;
            this.loadButton.UseVisualStyleBackColor = true;
            // 
            // browseButton
            // 
            resources.ApplyResources(this.browseButton, "browseButton");
            this.browseButton.Name = "browseButton";
            this.browseButton.TabStop = true;
            this.browseButton.UseVisualStyleBackColor = true;
            // 
            // loadText
            // 
            resources.ApplyResources(this.loadText, "loadText");
            this.loadText.Name = "loadText";
            // 
            // browseText
            // 
            resources.ApplyResources(this.browseText, "browseText");
            this.browseText.Name = "browseText";
            // 
            // askAgainCheckBox
            // 
            resources.ApplyResources(this.askAgainCheckBox, "askAgainCheckBox");
            this.askAgainCheckBox.Name = "askAgainCheckBox";
            this.askAgainCheckBox.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // SecurityWarningDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.askAgainCheckBox);
            this.Controls.Add(this.optionBox);
            this.Controls.Add(this.detailsButton);
            this.Controls.Add(this.warningText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SecurityWarningDialog";
            this.ShowInTaskbar = false;
            this.optionBox.ResumeLayout(false);
            this.optionBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label warningText;
        private System.Windows.Forms.Button detailsButton;
        private System.Windows.Forms.GroupBox optionBox;
        private System.Windows.Forms.Label browseText;
        private System.Windows.Forms.Label loadText;
        private System.Windows.Forms.RadioButton browseButton;
        private System.Windows.Forms.RadioButton loadButton;
        private System.Windows.Forms.CheckBox askAgainCheckBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}
