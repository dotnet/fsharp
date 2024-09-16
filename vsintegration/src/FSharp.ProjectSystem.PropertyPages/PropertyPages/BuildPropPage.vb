' Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.IO
Imports System.Windows.Forms
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualStudio.Editors.Common
Imports Microsoft.VisualStudio.Shell.Interop
Imports VSLangProj80
Imports VSLangProj110
Imports System.Runtime.Versioning

Namespace Microsoft.VisualStudio.Editors.PropertyPages

    ''' <summary>
    '''
    ''' </summary>
    ''' <remarks></remarks>
    Friend NotInheritable Class BuildPropPage
        'Inherits System.Windows.Forms.UserControl
        ' If you want to be able to use the forms designer to edit this file,
        ' change the base class from PropPageUserControlBase to UserControl
        Inherits PropPageUserControlBase

        Protected m_stDocumentationFile() As String

#Region " Windows Form Designer generated code "

        Public Sub New()
            MyBase.New()

            'This call is required by the Windows Form Designer.
            InitializeComponent()

            'Add any initialization after the InitializeComponent() call
            AddChangeHandlers()
        End Sub

        'UserControl overrides dispose to clean up the component list.
        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If Not (components Is Nothing) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

        Friend WithEvents lblConditionalCompilationSymbols As System.Windows.Forms.Label
        Friend WithEvents lblPlatformTarget As System.Windows.Forms.Label
        Friend WithEvents txtConditionalCompilationSymbols As System.Windows.Forms.TextBox
        Friend WithEvents chkDefineDebug As System.Windows.Forms.CheckBox
        Friend WithEvents chkDefineTrace As System.Windows.Forms.CheckBox
        Friend WithEvents cboPlatformTarget As System.Windows.Forms.ComboBox
        Friend WithEvents chkOptimizeCode As System.Windows.Forms.CheckBox
        Friend WithEvents chkTailcalls As System.Windows.Forms.CheckBox
        Friend WithEvents lblWarningLevel As System.Windows.Forms.Label
        Friend WithEvents lblSuppressWarnings As System.Windows.Forms.Label
        Friend WithEvents cboWarningLevel As System.Windows.Forms.ComboBox
        Friend WithEvents txtSuppressWarnings As System.Windows.Forms.TextBox
        Friend WithEvents rbWarningNone As System.Windows.Forms.RadioButton
        Friend WithEvents rbWarningSpecific As System.Windows.Forms.RadioButton
        Friend WithEvents rbWarningAll As System.Windows.Forms.RadioButton
        Friend WithEvents txtSpecificWarnings As System.Windows.Forms.TextBox
        Friend WithEvents lblOutputPath As System.Windows.Forms.Label
        Friend WithEvents txtOutputPath As System.Windows.Forms.TextBox
        Friend WithEvents btnOutputPathBrowse As System.Windows.Forms.Button
        Friend WithEvents chkXMLDocumentationFile As System.Windows.Forms.CheckBox
        Friend WithEvents txtXMLDocumentationFile As System.Windows.Forms.TextBox
        Friend WithEvents overarchingTableLayoutPanel As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents generalHeaderTableLayoutPanel As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents generalLabel As System.Windows.Forms.Label
        Friend WithEvents generalLineLabel As System.Windows.Forms.Label
        Friend WithEvents errorsAndWarningsTableLayoutPanel As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents errorsAndWarningsLineLabel As System.Windows.Forms.Label
        Friend WithEvents errorsAndWarningsLabel As System.Windows.Forms.Label
        Friend WithEvents treatWarningsAsErrorsTableLayoutPanel As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents treatWarningsAsErrorsLineLabel As System.Windows.Forms.Label
        Friend WithEvents treatWarningsAsErrorsLabel As System.Windows.Forms.Label
        Friend WithEvents outputTableLayoutPanel As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents outputLineLabel As System.Windows.Forms.Label
        Friend WithEvents outputLabel As System.Windows.Forms.Label
        Friend WithEvents lblOtherFlags As System.Windows.Forms.Label
        Friend WithEvents txtOtherFlags As System.Windows.Forms.TextBox
        Friend WithEvents chkPrefer32Bit As System.Windows.Forms.CheckBox

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'PERF: A note about the labels used as lines.  The 3D label is being set to 1 px high,
        '   so you�re really only using the grey part of it.  Using BorderStyle.Fixed3D seems
        '   to fire an extra resize OnHandleCreated.  The simple solution is to use BorderStyle.None
        '   and BackColor = SystemColors.ControlDark.

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BuildPropPage))
            Me.lblConditionalCompilationSymbols = New System.Windows.Forms.Label()
            Me.txtConditionalCompilationSymbols = New System.Windows.Forms.TextBox()
            Me.chkDefineDebug = New System.Windows.Forms.CheckBox()
            Me.chkDefineTrace = New System.Windows.Forms.CheckBox()
            Me.lblPlatformTarget = New System.Windows.Forms.Label()
            Me.cboPlatformTarget = New System.Windows.Forms.ComboBox()
            Me.chkOptimizeCode = New System.Windows.Forms.CheckBox()
            Me.chkTailcalls = New System.Windows.Forms.CheckBox()
            Me.lblWarningLevel = New System.Windows.Forms.Label()
            Me.cboWarningLevel = New System.Windows.Forms.ComboBox()
            Me.lblSuppressWarnings = New System.Windows.Forms.Label()
            Me.txtSuppressWarnings = New System.Windows.Forms.TextBox()
            Me.rbWarningNone = New System.Windows.Forms.RadioButton()
            Me.rbWarningSpecific = New System.Windows.Forms.RadioButton()
            Me.rbWarningAll = New System.Windows.Forms.RadioButton()
            Me.txtSpecificWarnings = New System.Windows.Forms.TextBox()
            Me.lblOutputPath = New System.Windows.Forms.Label()
            Me.txtOutputPath = New System.Windows.Forms.TextBox()
            Me.btnOutputPathBrowse = New System.Windows.Forms.Button()
            Me.chkXMLDocumentationFile = New System.Windows.Forms.CheckBox()
            Me.txtXMLDocumentationFile = New System.Windows.Forms.TextBox()
            Me.overarchingTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
            Me.chkPrefer32Bit = New System.Windows.Forms.CheckBox()
            Me.outputTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
            Me.outputLineLabel = New System.Windows.Forms.Label()
            Me.outputLabel = New System.Windows.Forms.Label()
            Me.generalHeaderTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
            Me.generalLineLabel = New System.Windows.Forms.Label()
            Me.generalLabel = New System.Windows.Forms.Label()
            Me.treatWarningsAsErrorsTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
            Me.treatWarningsAsErrorsLineLabel = New System.Windows.Forms.Label()
            Me.treatWarningsAsErrorsLabel = New System.Windows.Forms.Label()
            Me.errorsAndWarningsTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
            Me.errorsAndWarningsLineLabel = New System.Windows.Forms.Label()
            Me.errorsAndWarningsLabel = New System.Windows.Forms.Label()
            Me.txtOtherFlags = New System.Windows.Forms.TextBox()
            Me.lblOtherFlags = New System.Windows.Forms.Label()
            Me.overarchingTableLayoutPanel.SuspendLayout()
            Me.outputTableLayoutPanel.SuspendLayout()
            Me.generalHeaderTableLayoutPanel.SuspendLayout()
            Me.treatWarningsAsErrorsTableLayoutPanel.SuspendLayout()
            Me.errorsAndWarningsTableLayoutPanel.SuspendLayout()
            Me.SuspendLayout()
            '
            'lblConditionalCompilationSymbols
            '
            resources.ApplyResources(Me.lblConditionalCompilationSymbols, "lblConditionalCompilationSymbols")
            Me.lblConditionalCompilationSymbols.Name = "lblConditionalCompilationSymbols"
            '
            'txtConditionalCompilationSymbols
            '
            resources.ApplyResources(Me.txtConditionalCompilationSymbols, "txtConditionalCompilationSymbols")
            Me.txtConditionalCompilationSymbols.Name = "txtConditionalCompilationSymbols"
            '
            'chkDefineDebug
            '
            resources.ApplyResources(Me.chkDefineDebug, "chkDefineDebug")
            Me.overarchingTableLayoutPanel.SetColumnSpan(Me.chkDefineDebug, 3)
            Me.chkDefineDebug.Name = "chkDefineDebug"
            '
            'chkDefineTrace
            '
            resources.ApplyResources(Me.chkDefineTrace, "chkDefineTrace")
            Me.overarchingTableLayoutPanel.SetColumnSpan(Me.chkDefineTrace, 3)
            Me.chkDefineTrace.Name = "chkDefineTrace"
            '
            'lblPlatformTarget
            '
            resources.ApplyResources(Me.lblPlatformTarget, "lblPlatformTarget")
            Me.lblPlatformTarget.Name = "lblPlatformTarget"
            '
            'cboPlatformTarget
            '
            resources.ApplyResources(Me.cboPlatformTarget, "cboPlatformTarget")
            Me.cboPlatformTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboPlatformTarget.FormattingEnabled = True
            Me.cboPlatformTarget.Name = "cboPlatformTarget"
            '
            'chkOptimizeCode
            '
            resources.ApplyResources(Me.chkOptimizeCode, "chkOptimizeCode")
            Me.overarchingTableLayoutPanel.SetColumnSpan(Me.chkOptimizeCode, 3)
            Me.chkOptimizeCode.Name = "chkOptimizeCode"
            '
            'chkTailcalls
            '
            resources.ApplyResources(Me.chkTailcalls, "chkTailcalls")
            Me.overarchingTableLayoutPanel.SetColumnSpan(Me.chkTailcalls, 3)
            Me.chkTailcalls.Name = "chkTailcalls"
            '
            'lblWarningLevel
            '
            resources.ApplyResources(Me.lblWarningLevel, "lblWarningLevel")
            Me.lblWarningLevel.Name = "lblWarningLevel"
            '
            'cboWarningLevel
            '
            resources.ApplyResources(Me.cboWarningLevel, "cboWarningLevel")
            Me.cboWarningLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.cboWarningLevel.FormattingEnabled = True
            Me.cboWarningLevel.Items.AddRange(New Object() {resources.GetString("cboWarningLevel.Items"), resources.GetString("cboWarningLevel.Items1"), resources.GetString("cboWarningLevel.Items2"), resources.GetString("cboWarningLevel.Items3"), resources.GetString("cboWarningLevel.Items4"), resources.GetString("cboWarningLevel.Items5")})
            Me.cboWarningLevel.Name = "cboWarningLevel"
            '
            'lblSuppressWarnings
            '
            resources.ApplyResources(Me.lblSuppressWarnings, "lblSuppressWarnings")
            Me.lblSuppressWarnings.Name = "lblSuppressWarnings"
            '
            'txtSuppressWarnings
            '
            resources.ApplyResources(Me.txtSuppressWarnings, "txtSuppressWarnings")
            Me.txtSuppressWarnings.Name = "txtSuppressWarnings"
            '
            'rbWarningNone
            '
            resources.ApplyResources(Me.rbWarningNone, "rbWarningNone")
            Me.overarchingTableLayoutPanel.SetColumnSpan(Me.rbWarningNone, 3)
            Me.rbWarningNone.Name = "rbWarningNone"
            '
            'rbWarningSpecific
            '
            resources.ApplyResources(Me.rbWarningSpecific, "rbWarningSpecific")
            Me.rbWarningSpecific.Name = "rbWarningSpecific"
            '
            'rbWarningAll
            '
            resources.ApplyResources(Me.rbWarningAll, "rbWarningAll")
            Me.overarchingTableLayoutPanel.SetColumnSpan(Me.rbWarningAll, 3)
            Me.rbWarningAll.Name = "rbWarningAll"
            '
            'txtSpecificWarnings
            '
            resources.ApplyResources(Me.txtSpecificWarnings, "txtSpecificWarnings")
            Me.txtSpecificWarnings.Name = "txtSpecificWarnings"
            '
            'lblOutputPath
            '
            resources.ApplyResources(Me.lblOutputPath, "lblOutputPath")
            Me.lblOutputPath.Name = "lblOutputPath"
            '
            'txtOutputPath
            '
            resources.ApplyResources(Me.txtOutputPath, "txtOutputPath")
            Me.txtOutputPath.Name = "txtOutputPath"
            '
            'btnOutputPathBrowse
            '
            resources.ApplyResources(Me.btnOutputPathBrowse, "btnOutputPathBrowse")
            Me.btnOutputPathBrowse.Name = "btnOutputPathBrowse"
            '
            'chkXMLDocumentationFile
            '
            resources.ApplyResources(Me.chkXMLDocumentationFile, "chkXMLDocumentationFile")
            Me.chkXMLDocumentationFile.Name = "chkXMLDocumentationFile"
            '
            'txtXMLDocumentationFile
            '
            resources.ApplyResources(Me.txtXMLDocumentationFile, "txtXMLDocumentationFile")
            Me.txtXMLDocumentationFile.Name = "txtXMLDocumentationFile"
            '
            'overarchingTableLayoutPanel
            '
            resources.ApplyResources(Me.overarchingTableLayoutPanel, "overarchingTableLayoutPanel")
            Me.overarchingTableLayoutPanel.Controls.Add(Me.chkPrefer32Bit, 0, 7)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.outputTableLayoutPanel, 0, 17)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.generalHeaderTableLayoutPanel, 0, 0)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.treatWarningsAsErrorsTableLayoutPanel, 0, 13)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.errorsAndWarningsTableLayoutPanel, 0, 10)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.chkXMLDocumentationFile, 0, 19)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.txtXMLDocumentationFile, 1, 19)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.btnOutputPathBrowse, 2, 18)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.txtOutputPath, 1, 18)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.lblOutputPath, 0, 18)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.rbWarningAll, 0, 16)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.rbWarningSpecific, 0, 15)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.txtSpecificWarnings, 1, 15)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.rbWarningNone, 0, 14)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.txtSuppressWarnings, 1, 12)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.lblSuppressWarnings, 0, 12)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.cboWarningLevel, 1, 11)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.lblWarningLevel, 0, 11)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.cboPlatformTarget, 1, 4)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.chkOptimizeCode, 0, 8)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.chkTailcalls, 0, 9)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.chkDefineTrace, 0, 3)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.chkDefineDebug, 0, 2)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.txtConditionalCompilationSymbols, 1, 1)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.lblConditionalCompilationSymbols, 0, 1)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.lblPlatformTarget, 0, 4)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.txtOtherFlags, 1, 6)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.lblOtherFlags, 0, 6)
            Me.overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel"
            '
            'chkPrefer32Bit
            '
            resources.ApplyResources(Me.chkPrefer32Bit, "chkPrefer32Bit")
            Me.overarchingTableLayoutPanel.SetColumnSpan(Me.chkPrefer32Bit, 3)
            Me.chkPrefer32Bit.Name = "chkPrefer32Bit"
            '
            'outputTableLayoutPanel
            '
            resources.ApplyResources(Me.outputTableLayoutPanel, "outputTableLayoutPanel")
            Me.overarchingTableLayoutPanel.SetColumnSpan(Me.outputTableLayoutPanel, 3)
            Me.outputTableLayoutPanel.Controls.Add(Me.outputLineLabel, 1, 0)
            Me.outputTableLayoutPanel.Controls.Add(Me.outputLabel, 0, 0)
            Me.outputTableLayoutPanel.Name = "outputTableLayoutPanel"
            '
            'outputLineLabel
            '
            resources.ApplyResources(Me.outputLineLabel, "outputLineLabel")
            Me.outputLineLabel.BackColor = System.Drawing.SystemColors.ControlDark
            Me.outputLineLabel.Name = "outputLineLabel"
            '
            'outputLabel
            '
            resources.ApplyResources(Me.outputLabel, "outputLabel")
            Me.outputLabel.Name = "outputLabel"
            '
            'generalHeaderTableLayoutPanel
            '
            resources.ApplyResources(Me.generalHeaderTableLayoutPanel, "generalHeaderTableLayoutPanel")
            Me.overarchingTableLayoutPanel.SetColumnSpan(Me.generalHeaderTableLayoutPanel, 3)
            Me.generalHeaderTableLayoutPanel.Controls.Add(Me.generalLineLabel, 1, 0)
            Me.generalHeaderTableLayoutPanel.Controls.Add(Me.generalLabel, 0, 0)
            Me.generalHeaderTableLayoutPanel.Name = "generalHeaderTableLayoutPanel"
            '
            'generalLineLabel
            '
            resources.ApplyResources(Me.generalLineLabel, "generalLineLabel")
            Me.generalLineLabel.BackColor = System.Drawing.SystemColors.ControlDark
            Me.generalLineLabel.Name = "generalLineLabel"
            '
            'generalLabel
            '
            resources.ApplyResources(Me.generalLabel, "generalLabel")
            Me.generalLabel.Name = "generalLabel"
            '
            'treatWarningsAsErrorsTableLayoutPanel
            '
            resources.ApplyResources(Me.treatWarningsAsErrorsTableLayoutPanel, "treatWarningsAsErrorsTableLayoutPanel")
            Me.overarchingTableLayoutPanel.SetColumnSpan(Me.treatWarningsAsErrorsTableLayoutPanel, 3)
            Me.treatWarningsAsErrorsTableLayoutPanel.Controls.Add(Me.treatWarningsAsErrorsLineLabel, 1, 0)
            Me.treatWarningsAsErrorsTableLayoutPanel.Controls.Add(Me.treatWarningsAsErrorsLabel, 0, 0)
            Me.treatWarningsAsErrorsTableLayoutPanel.Name = "treatWarningsAsErrorsTableLayoutPanel"
            '
            'treatWarningsAsErrorsLineLabel
            '
            resources.ApplyResources(Me.treatWarningsAsErrorsLineLabel, "treatWarningsAsErrorsLineLabel")
            Me.treatWarningsAsErrorsLineLabel.BackColor = System.Drawing.SystemColors.ControlDark
            Me.treatWarningsAsErrorsLineLabel.Name = "treatWarningsAsErrorsLineLabel"
            '
            'treatWarningsAsErrorsLabel
            '
            resources.ApplyResources(Me.treatWarningsAsErrorsLabel, "treatWarningsAsErrorsLabel")
            Me.treatWarningsAsErrorsLabel.Name = "treatWarningsAsErrorsLabel"
            '
            'errorsAndWarningsTableLayoutPanel
            '
            resources.ApplyResources(Me.errorsAndWarningsTableLayoutPanel, "errorsAndWarningsTableLayoutPanel")
            Me.overarchingTableLayoutPanel.SetColumnSpan(Me.errorsAndWarningsTableLayoutPanel, 3)
            Me.errorsAndWarningsTableLayoutPanel.Controls.Add(Me.errorsAndWarningsLineLabel, 1, 0)
            Me.errorsAndWarningsTableLayoutPanel.Controls.Add(Me.errorsAndWarningsLabel, 0, 0)
            Me.errorsAndWarningsTableLayoutPanel.Name = "errorsAndWarningsTableLayoutPanel"
            '
            'errorsAndWarningsLineLabel
            '
            resources.ApplyResources(Me.errorsAndWarningsLineLabel, "errorsAndWarningsLineLabel")
            Me.errorsAndWarningsLineLabel.BackColor = System.Drawing.SystemColors.ControlDark
            Me.errorsAndWarningsLineLabel.Name = "errorsAndWarningsLineLabel"
            '
            'errorsAndWarningsLabel
            '
            resources.ApplyResources(Me.errorsAndWarningsLabel, "errorsAndWarningsLabel")
            Me.errorsAndWarningsLabel.Name = "errorsAndWarningsLabel"
            '
            'txtOtherFlags
            '
            resources.ApplyResources(Me.txtOtherFlags, "txtOtherFlags")
            Me.txtOtherFlags.Name = "txtOtherFlags"
            '
            'lblOtherFlags
            '
            resources.ApplyResources(Me.lblOtherFlags, "lblOtherFlags")
            Me.lblOtherFlags.Name = "lblOtherFlags"
            '
            'BuildPropPage
            '
            resources.ApplyResources(Me, "$this")
            Me.Controls.Add(Me.overarchingTableLayoutPanel)
            Me.Name = "BuildPropPage"
            Me.overarchingTableLayoutPanel.ResumeLayout(False)
            Me.overarchingTableLayoutPanel.PerformLayout()
            Me.outputTableLayoutPanel.ResumeLayout(False)
            Me.outputTableLayoutPanel.PerformLayout()
            Me.generalHeaderTableLayoutPanel.ResumeLayout(False)
            Me.generalHeaderTableLayoutPanel.PerformLayout()
            Me.treatWarningsAsErrorsTableLayoutPanel.ResumeLayout(False)
            Me.treatWarningsAsErrorsTableLayoutPanel.PerformLayout()
            Me.errorsAndWarningsTableLayoutPanel.ResumeLayout(False)
            Me.errorsAndWarningsTableLayoutPanel.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

#End Region

        Enum TreatWarningsSetting
            WARNINGS_ALL
            WARNINGS_SPECIFIC
            WARNINGS_NONE
        End Enum



        'True when we're changing control values ourselves
        Protected m_bInsideInternalUpdate As Boolean = False

        '// Stored conditional compilation symbols. We need these to calculate the new strings
        '//   to return for the conditional compilation constants when the user changes any
        '//   of the controls related to conditional compilation symbols (the data in the
        '//   controls is not sufficient because they could be indeterminate, and we are acting
        '//   as if we have three separate properties, so we need the original property values).
        '// Array same length and indexing as the objects passed into SetObjects.
        Protected m_stCondCompSymbols() As String

        Protected Const Const_DebugConfiguration As String = "Debug" 'Name of the debug configuration
        Protected Const Const_ReleaseConfiguration As String = "Release" 'Name of the release configuration
        Protected Const Const_CondConstantDEBUG As String = "DEBUG"
        Protected Const Const_CondConstantTRACE As String = "TRACE"

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="_enabled"></param>
        ''' <remarks></remarks>
        Protected Overrides Sub EnableAllControls(ByVal _enabled As Boolean)
            MyBase.EnableAllControls(_enabled)
        End Sub


        '' Using AllowUnsafeBlocks as a placeholder for tailcalls dispid
        ''' <summary>
        '''
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Protected Overrides ReadOnly Property ControlData() As PropertyControlData()
            Get
                If m_ControlData Is Nothing Then
                    m_ControlData = New PropertyControlData() {
                     New PropertyControlData(VsProjPropId.VBPROJPROPID_DefineConstants, "DefineConstants", Me.txtConditionalCompilationSymbols, AddressOf ConditionalCompilationSet, AddressOf ConditionalCompilationGet, ControlDataFlags.None, New Control() {Me.txtConditionalCompilationSymbols, Me.chkDefineDebug, Me.chkDefineTrace, Me.lblConditionalCompilationSymbols}),
                     New PropertyControlData(VsProjPropId80.VBPROJPROPID_PlatformTarget, "PlatformTarget", Me.cboPlatformTarget, AddressOf PlatformTargetSet, AddressOf PlatformTargetGet, ControlDataFlags.None, New Control() {Me.lblPlatformTarget}),
                     New SingleConfigPropertyControlData(SingleConfigPropertyControlData.Configs.Release,
                        VsProjPropId.VBPROJPROPID_Optimize, "Optimize", Me.chkOptimizeCode),
                     New SingleConfigPropertyControlData(SingleConfigPropertyControlData.Configs.Release,
                        VsProjPropId.VBPROJPROPID_AllowUnsafeBlocks, "Tailcalls", Me.chkTailcalls),
                     New PropertyControlData(VsProjPropId.VBPROJPROPID_WarningLevel, "WarningLevel", Me.cboWarningLevel, AddressOf WarningLevelSet, AddressOf WarningLevelGet, ControlDataFlags.None, New Control() {lblWarningLevel}),
                     New PropertyControlData(VsProjPropId2.VBPROJPROPID_NoWarn, "NoWarn", Me.txtSuppressWarnings, New Control() {Me.lblSuppressWarnings}),
                     New PropertyControlData(VsProjPropId.VBPROJPROPID_TreatWarningsAsErrors, "TreatWarningsAsErrors", Me.rbWarningAll, AddressOf TreatWarningsInit, AddressOf TreatWarningsGet),
                     New PropertyControlData(VsProjPropId80.VBPROJPROPID_TreatSpecificWarningsAsErrors, "TreatSpecificWarningsAsErrors", Me.txtSpecificWarnings, AddressOf TreatSpecificWarningsInit, AddressOf TreatSpecificWarningsGet),
                     New SingleConfigPropertyControlData(SingleConfigPropertyControlData.Configs.Release,
                        VsProjPropId.VBPROJPROPID_OutputPath, "OutputPath", Me.txtOutputPath, New Control() {Me.lblOutputPath}),
                     New PropertyControlData(VsProjPropId.VBPROJPROPID_DocumentationFile, "DocumentationFile", Me.txtXMLDocumentationFile, AddressOf Me.XMLDocumentationFileInit, AddressOf Me.XMLDocumentationFileGet, ControlDataFlags.None, New Control() {Me.txtXMLDocumentationFile, Me.chkXMLDocumentationFile}),
                     New PropertyControlData(VsProjPropId.VBPROJPROPID_OutputType, "OutputType", Nothing, AddressOf Me.OutputTypeSet, Nothing),
                     New PropertyControlData(VsProjPropId.VBPROJPROPID_Unused1, "OtherFlags", Me.txtOtherFlags, New Control() {Me.txtOtherFlags, Me.lblOtherFlags}),
                     New PropertyControlData(VsProjPropId110.VBPROJPROPID_Prefer32Bit, "Prefer32Bit", Me.chkPrefer32Bit, AddressOf Prefer32BitSet, AddressOf Prefer32BitGet)
                     }

                End If
                Return m_ControlData
            End Get
        End Property

        Private Function IsCurrentProjectDotNetPortable() As Boolean
            Return ApplicationPropPage.IsCurrentProjectDotNetPortable(DTEProject)
        End Function

        ''' <summary>
        ''' Customizable processing done before the class has populated controls in the ControlData array
        ''' </summary>
        ''' <remarks>
        ''' Override this to implement custom processing.
        ''' IMPORTANT NOTE: this method can be called multiple times on the same page.  In particular,
        '''   it is called on every SetObjects call, which means that when the user changes the
        '''   selected configuration, it is called again.
        ''' </remarks>
        Protected Overrides Sub PreInitPage()
            MyBase.PreInitPage()

            cboPlatformTarget.Items.Clear()


            Dim PlatformEntries As New List(Of String)

            ' Let's try to sniff the supported platforms from our hierarchy (if any)
            If Me.ProjectHierarchy IsNot Nothing Then
                Dim oCfgProv As Object = Nothing
                Dim hr As Integer
                hr = ProjectHierarchy.GetProperty(VSITEMID.ROOT, __VSHPROPID.VSHPROPID_ConfigurationProvider, oCfgProv)
                If VSErrorHandler.Succeeded(hr) Then
                    Dim cfgProv As IVsCfgProvider2 = TryCast(oCfgProv, IVsCfgProvider2)
                    If cfgProv IsNot Nothing Then
                        Dim actualPlatformCount(0) As UInteger
                        hr = cfgProv.GetSupportedPlatformNames(0, Nothing, actualPlatformCount)
                        If VSErrorHandler.Succeeded(hr) Then
                            Dim platformCount As UInteger = actualPlatformCount(0)
                            Dim platforms(CInt(platformCount)) As String
                            hr = cfgProv.GetSupportedPlatformNames(platformCount, platforms, actualPlatformCount)
                            If VSErrorHandler.Succeeded(hr) Then
                                For platformNo As Integer = 0 To CInt(platformCount - 1)
                                    PlatformEntries.Add(platforms(platformNo))
                                Next
                            End If
                        End If
                    End If
                End If
            End If

            ' ...and if we couldn't get 'em from the project system, let's add a hard-coded list of platforms...
            If PlatformEntries.Count = 0 Then
                Debug.Fail("Unable to get platform list from configuration manager")
                PlatformEntries.AddRange(New String() {"Any CPU", "x86", "x64", "Itanium"})
            End If

            ' ... Finally, add the entries to the combobox
            Me.cboPlatformTarget.Items.AddRange(PlatformEntries.ToArray())

            If IsCurrentProjectDotNetPortable() Then
                ' F# Portable projects do not support targets like x86, disable selection for this dropdown
                    Me.cboPlatformTarget.Enabled = False
            End If
        End Sub

        ''' <summary>
        ''' Customizable processing done after base class has populated controls in the ControlData array
        ''' </summary>
        ''' <remarks>
        ''' Override this to implement custom processing.
        ''' IMPORTANT NOTE: this method can be called multiple times on the same page.  In particular,
        '''   it is called on every SetObjects call, which means that when the user changes the
        '''   selected configuration, it is called again.
        ''' </remarks>
        Protected Overrides Sub PostInitPage()
            MyBase.PostInitPage()

            'OutputPath browse button should only be enabled when the text box is enabled and Not ReadOnly
            Me.btnOutputPathBrowse.Enabled = (Me.txtOutputPath.Enabled AndAlso Not Me.txtOutputPath.ReadOnly)

            Me.rbWarningNone.Enabled = Me.rbWarningAll.Enabled
            Me.rbWarningSpecific.Enabled = Me.rbWarningAll.Enabled

            RefreshEnabledStatusForPrefer32Bit(Me.chkPrefer32Bit)
        End Sub


        ''' <summary>
        '''
        ''' </summary>
        Private Function IsExeProject() As Boolean

            Dim obj As Object = Nothing
            Dim OutputType As VSLangProj.prjOutputType

            Try
                GetCurrentProperty(VsProjPropId.VBPROJPROPID_OutputType, "OutputType", obj)
                OutputType = CType(obj, VSLangProj.prjOutputType)
            Catch ex As InvalidCastException
                '// When all else fails assume dll (so they can edit it)
                OutputType = VSLangProj.prjOutputType.prjOutputTypeLibrary
            Catch ex As System.Reflection.TargetInvocationException
                ' Property must be missing for this project flavor
                OutputType = VSLangProj.prjOutputType.prjOutputTypeLibrary
            End Try

            If (OutputType = VSLangProj.prjOutputType.prjOutputTypeLibrary) Then
                Return False
            Else
                Return True
            End If
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="prop"></param>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Private Function OutputTypeSet(ByVal control As Control, ByVal prop As PropertyDescriptor, ByVal value As Object) As Boolean
            If Not m_fInsideInit AndAlso Not m_bInsideInternalUpdate Then
                'Changes to the OutputType may affect whether Prefer32Bit is enabled
                RefreshEnabledStatusForPrefer32Bit(Me.chkPrefer32Bit)
            End If

            Return True
        End Function

        ''' <summary>
        ''' Gets the absolute path to the path currently in the Folder textbox.  If the path is invalid (contains bad
        '''   characters, etc.), returns simply the current text.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetCurrentOutputPathAbsolute() As String
            Dim OutputText As String = Trim(txtOutputPath.Text)
            If OutputText.Length > 0 Then
                Try
                    'Interpret as relative to the project path, and make it absolute
                    OutputText = IO.Path.Combine(GetBuildPathProjectPath(), OutputText)
                    OutputText = Utils.AppendBackslash(OutputText)
                Catch ex As Exception
                    Common.RethrowIfUnrecoverable(ex)
                End Try
            End If

            Return OutputText
        End Function

        ''' <summary>
        ''' Given a base path and a full path to a directory, returns the path of the full path relative to the base path.  Note: does
        '''   *not* return relative paths that begin with "..\", instead in this case it returns the original full path.
        '''   This is what Everett did.
        ''' </summary>
        ''' <param name="BasePath">The base path (with or without backslash at the end)</param>
        ''' <param name="DirectoryPath">The full path to the directory (with or without backslash at the end)</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function GetRelativeBuildPathDirectoryPath(ByVal BasePath As String, ByVal DirectoryPath As String) As String
            Dim RelativePath As String = ""

            BasePath = Utils.AppendBackslash(BasePath)
            DirectoryPath = Utils.AppendBackslash(DirectoryPath)

            If DirectoryPath = "" Then
                DirectoryPath = ""
            End If

            If BasePath = "" Then
                Return DirectoryPath
            End If

            ' Remove the project directory path
            If String.Compare(BasePath, VisualBasic.Left(DirectoryPath, Len(BasePath)), StringComparison.OrdinalIgnoreCase) = 0 Then
                Dim ch As Char = CChar(Mid(DirectoryPath, Len(BasePath), 1))
                If ch = System.IO.Path.DirectorySeparatorChar OrElse ch = System.IO.Path.AltDirectorySeparatorChar Then
                    RelativePath = Mid(DirectoryPath, Len(BasePath) + 1)
                ElseIf ch = ChrW(0) Then
                    RelativePath = ""
                End If
            Else
                RelativePath = DirectoryPath
            End If

            Return RelativePath
        End Function

        Function GetBuildPathProjectPath() As String
            Dim fullPathProperty As EnvDTE.Property = DTEProject.Properties.Item("FullPath")
            If fullPathProperty IsNot Nothing AndAlso fullPathProperty.Value IsNot Nothing Then
                Return CType(fullPathProperty.Value, String)
            Else
                Return ""
            End If
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub OutputPathBrowse_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOutputPathBrowse.Click
            Dim value As String = Nothing
            If GetDirectoryViaBrowse(GetCurrentOutputPathAbsolute(), SR.GetString(SR.PPG_SelectOutputPathTitle), value) Then
                Dim projectFullPath As String = GetBuildPathProjectPath()
                If Not String.IsNullOrEmpty(projectFullPath) Then
                    txtOutputPath.Text = GetRelativeBuildPathDirectoryPath(projectFullPath, value)
                    SetDirty(True)
                End If
            End If
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="prop"></param>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Private Function TreatSpecificWarningsInit(ByVal control As Control, ByVal prop As PropertyDescriptor, ByVal value As Object) As Boolean

            m_bInsideInternalUpdate = True

            Try
                Dim bIndeterminateState As Boolean = False
                Dim warnings As TreatWarningsSetting

                If (Not (PropertyControlData.IsSpecialValue(value))) Then
                    Dim stSpecificWarnings As String

                    stSpecificWarnings = CType(value, String)
                    If (stSpecificWarnings <> "") Then
                        warnings = TreatWarningsSetting.WARNINGS_SPECIFIC
                        Me.txtSpecificWarnings.Text = stSpecificWarnings

                        bIndeterminateState = False
                    Else
                        Dim propTreatAllWarnings As PropertyDescriptor
                        Dim obj As Object
                        Dim bTreatAllWarningsAsErrors As Boolean = False

                        propTreatAllWarnings = GetPropertyDescriptor("TreatWarningsAsErrors")

                        obj = TryGetNonCommonPropertyValue(propTreatAllWarnings)

                        If Not (PropertyControlData.IsSpecialValue(obj)) Then
                            Me.txtSpecificWarnings.Text = ""
                            bTreatAllWarningsAsErrors = CType(obj, Boolean)
                            If (bTreatAllWarningsAsErrors) Then
                                warnings = TreatWarningsSetting.WARNINGS_ALL
                            Else
                                warnings = TreatWarningsSetting.WARNINGS_NONE
                            End If

                            bIndeterminateState = False
                        Else
                            '// Since TreadAllWarnings is indeterminate we should be too
                            bIndeterminateState = True
                        End If
                    End If
                Else
                    '// Indeterminate. Leave all the radio buttons unchecked
                    bIndeterminateState = True
                End If

                If (Not bIndeterminateState) Then
                    Me.rbWarningAll.Checked = (warnings = TreatWarningsSetting.WARNINGS_ALL)
                    Me.rbWarningSpecific.Checked = (warnings = TreatWarningsSetting.WARNINGS_SPECIFIC)
                    Me.txtSpecificWarnings.Enabled = (warnings = TreatWarningsSetting.WARNINGS_SPECIFIC)
                    Me.rbWarningNone.Checked = (warnings = TreatWarningsSetting.WARNINGS_NONE)
                Else
                    Me.rbWarningAll.Checked = False
                    Me.rbWarningSpecific.Checked = False
                    Me.txtSpecificWarnings.Enabled = False
                    Me.txtSpecificWarnings.Text = ""
                    Me.rbWarningNone.Checked = False
                End If
            Finally
                m_bInsideInternalUpdate = False
            End Try

            Return True
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <remarks></remarks>
        Private Function TreatSpecificWarningsGetValue() As TreatWarningsSetting
            Dim warnings As TreatWarningsSetting

            If Me.rbWarningAll.Checked Then
                warnings = TreatWarningsSetting.WARNINGS_ALL
            ElseIf Me.rbWarningSpecific.Checked Then
                warnings = TreatWarningsSetting.WARNINGS_SPECIFIC
            ElseIf Me.rbWarningNone.Checked Then
                warnings = TreatWarningsSetting.WARNINGS_NONE
            Else
                warnings = TreatWarningsSetting.WARNINGS_NONE
            End If

            Return warnings
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="prop"></param>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Private Function TreatSpecificWarningsGet(ByVal control As Control, ByVal prop As PropertyDescriptor, ByRef value As Object) As Boolean
            Dim bRetVal As Boolean = True

            If Me.rbWarningAll.Checked Then
                value = ""
                bRetVal = True
            ElseIf Me.rbWarningSpecific.Checked Then
                value = Me.txtSpecificWarnings.Text
                bRetVal = True
            ElseIf Me.rbWarningNone.Checked Then
                value = ""
                bRetVal = True
            Else
                '// We're in the indeterminate state. Let the architecture handle it
                bRetVal = False
            End If

            Return bRetVal
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="prop"></param>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Private Function TreatWarningsInit(ByVal control As Control, ByVal prop As PropertyDescriptor, ByVal value As Object) As Boolean
            '// Don't need to do anything here (it's done in TreatSpecificWarningsInit)
            Return True
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="prop"></param>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Private Function TreatWarningsGet(ByVal control As Control, ByVal prop As PropertyDescriptor, ByRef value As Object) As Boolean
            Dim bRetVal As Boolean = True

            If Me.rbWarningAll.Checked Then
                value = Me.rbWarningAll.Checked
                bRetVal = True
            ElseIf Me.rbWarningSpecific.Checked Then
                value = False
                bRetVal = True
            ElseIf Me.rbWarningNone.Checked Then
                value = Not (Me.rbWarningNone.Checked)    '// If none is checked we want value to be false
                bRetVal = True
            Else
                '// We're in the indeterminate state. Let the architecture handle it.
                bRetVal = False
            End If

            Return bRetVal
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub rbStartAction_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbWarningAll.CheckedChanged, rbWarningSpecific.CheckedChanged, rbWarningNone.CheckedChanged
            If (Not m_bInsideInternalUpdate) Then
                Dim warnings As TreatWarningsSetting = TreatSpecificWarningsGetValue()
                Me.rbWarningAll.Checked = (warnings = TreatWarningsSetting.WARNINGS_ALL)
                Me.rbWarningSpecific.Checked = (warnings = TreatWarningsSetting.WARNINGS_SPECIFIC)
                Me.txtSpecificWarnings.Enabled = (warnings = TreatWarningsSetting.WARNINGS_SPECIFIC)
                Me.rbWarningNone.Checked = (warnings = TreatWarningsSetting.WARNINGS_NONE)
                IsDirty = True

                '// Dirty both of the properties since either one could have changed
                SetDirty(Me.rbWarningAll)
                SetDirty(Me.txtSpecificWarnings)
            End If
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="prop"></param>
        ''' <param name="values"></param>
        ''' <remarks></remarks>
        Private Function XMLDocumentationFileInit(ByVal control As Control, ByVal prop As PropertyDescriptor, ByVal values() As Object) As Boolean
            Dim bOriginalState As Boolean = m_bInsideInternalUpdate

            m_bInsideInternalUpdate = True
            ReDim m_stDocumentationFile(values.Length - 1)
            values.CopyTo(m_stDocumentationFile, 0)

            Dim objDocumentationFile As Object
            objDocumentationFile = GetValueOrIndeterminateFromArray(m_stDocumentationFile)

            If (Not (PropertyControlData.IsSpecialValue(objDocumentationFile))) Then
                If (Trim(TryCast(objDocumentationFile, String)) <> "") Then
                    Me.txtXMLDocumentationFile.Text = Trim(TryCast(objDocumentationFile, String))
                    Me.chkXMLDocumentationFile.Checked = True
                    Me.txtXMLDocumentationFile.Enabled = True
                Else
                    Me.chkXMLDocumentationFile.Checked = False
                    Me.txtXMLDocumentationFile.Enabled = False
                    Me.txtXMLDocumentationFile.Text = ""
                End If
            Else
                Me.chkXMLDocumentationFile.CheckState = CheckState.Indeterminate
                Me.txtXMLDocumentationFile.Text = ""
                Me.txtXMLDocumentationFile.Enabled = False
            End If

            '// Reset value
            m_bInsideInternalUpdate = bOriginalState
            Return True
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="prop"></param>
        ''' <param name="values"></param>
        ''' <remarks></remarks>
        Private Function XMLDocumentationFileGet(ByVal control As Control, ByVal prop As PropertyDescriptor, ByRef values() As Object) As Boolean
            Debug.Assert(m_stDocumentationFile IsNot Nothing)
            ReDim values(m_stDocumentationFile.Length - 1)
            m_stDocumentationFile.CopyTo(values, 0)
            Return True
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <remarks></remarks>
        Protected Overrides Function GetF1HelpKeyword() As String
            Return HelpKeywords.FSProjPropBuild
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="prop"></param>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Private Function WarningLevelSet(ByVal control As Control, ByVal prop As PropertyDescriptor, ByVal value As Object) As Boolean
            If (Not (PropertyControlData.IsSpecialValue(value))) Then
                Me.cboWarningLevel.SelectedIndex = CType(value, Integer)
                Return True
            Else
                '// Indeterminate. Let the architecture handle
                Me.cboWarningLevel.SelectedIndex = -1
                Return True
            End If
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="prop"></param>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Private Function WarningLevelGet(ByVal control As Control, ByVal prop As PropertyDescriptor, ByRef value As Object) As Boolean
            value = CType(Me.cboWarningLevel.SelectedIndex, Integer)
            Return True
        End Function

        Protected Overrides Sub OnPageActivated(activated As Boolean)
            MyBase.OnPageActivated(activated)

            'Changes may affect whether Prefer32Bit is enabled
            RefreshEnabledStatusForPrefer32Bit(Me.chkPrefer32Bit)
        End Sub

        Private Sub cboPlatformTarget_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboPlatformTarget.SelectionChangeCommitted
            If m_fInsideInit OrElse m_bInsideInternalUpdate Then
                Return
            End If

            'Changes to PlatformTarget may affect whether Prefer32Bit is enabled
            RefreshEnabledStatusForPrefer32Bit(Me.chkPrefer32Bit)
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="prop"></param>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Private Function PlatformTargetSet(ByVal control As Control, ByVal prop As PropertyDescriptor, ByVal value As Object) As Boolean
            If (Not (PropertyControlData.IsSpecialValue(value))) Then
                If (IsNothing(TryCast(value, String)) OrElse TryCast(value, String) = "") Then
                    Me.cboPlatformTarget.SelectedIndex = 0     '// AnyCPU
                Else
                    Dim strPlatform As String = TryCast(value, String)

                    '// For Undo, we may get called to set the value
                    '// to AnyCpu (no space but the one we display in the combobox has a space so
                    '// convert to the one with the space for this specific case

                    '// Convert the no-space to one with a space
                    If (String.Compare(strPlatform, "AnyCPU", StringComparison.Ordinal) = 0) Then
                        strPlatform = "Any CPU"
                    End If

                    Me.cboPlatformTarget.SelectedItem = strPlatform

                    If (Me.cboPlatformTarget.SelectedIndex = -1) Then   '// If we can't find a match
                        '// For the standard SKU, we do not include Itanium in the list. However,
                        '// if the property is already set to Itanium (most likely from the project file set from
                        '// a non-Standard SKU then add it to the list so we do not report the wrong
                        '// platform target to the user.

                        Dim stValue As String = TryCast(value, String)
                        If (String.Compare(Trim(stValue), "Itanium", StringComparison.Ordinal) = 0) Then
                            Me.cboPlatformTarget.Items.Add("Itanium")
                            Me.cboPlatformTarget.SelectedItem = stValue
                        Else
                            '// Note that the project system will return "AnyCPU" (no space) but in the UI we want to show the one with a space
                            Me.cboPlatformTarget.SelectedItem = "Any CPU"
                        End If
                    End If
                End If
                Return True
            Else
                '// Indeterminate - allow the architecture to handle
                Return False
            End If
        End Function

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="prop"></param>
        ''' <param name="value"></param>
        ''' <remarks></remarks>
        Private Function PlatformTargetGet(ByVal control As Control, ByVal prop As PropertyDescriptor, ByRef value As Object) As Boolean
            If (Me.cboPlatformTarget.SelectedItem.ToString() <> "AnyCPU") And (Me.cboPlatformTarget.SelectedItem.ToString() <> "Any CPU") Then
                value = Me.cboPlatformTarget.SelectedItem
            Else
                '// Return to the project system the one without a space
                value = "AnyCPU"
            End If

            Return True
        End Function


        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub XMLDocumentationEnable_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkXMLDocumentationFile.CheckStateChanged
            Const XML_FILE_EXTENSION As String = ".XML"

            If Me.chkXMLDocumentationFile.Checked Then

                '// Enable the textbox
                Me.txtXMLDocumentationFile.Enabled = True

                If Trim(Me.txtXMLDocumentationFile.Text) = "" Then
                    '// The textbox is empty so initialize it
                    Dim stOutputPath As String
                    Dim stAssemblyName As String
                    Dim obj As Object = Nothing

                    '// Get OutputPath for all configs. We're going to calculate the documentation file
                    '// for each config (and the value is dependent on the OutputPath

                    Dim RawDocFiles() As Object = RawPropertiesObjects(GetPropertyControlData(VsProjPropId.VBPROJPROPID_DocumentationFile))
                    Dim OutputPathData() As Object
                    Dim cLen As Integer = RawDocFiles.Length

                    ReDim OutputPathData(cLen)

                    Dim p As PropertyControlData = GetPropertyControlData(VsProjPropId.VBPROJPROPID_OutputPath)
                    For i As Integer = 0 To cLen - 1
                        OutputPathData(i) = p.GetPropertyValueNative(RawDocFiles(i))
                    Next i

                    GetCurrentProperty(VsProjPropId.VBPROJPROPID_AssemblyName, "AssemblyName", obj)
                    stAssemblyName = TryCast(obj, String)

                    GetCurrentProperty(VsProjPropId.VBPROJPROPID_AbsoluteProjectDirectory, "AbsoluteProjectDirectory", obj)
                    Dim stProjectDirectory As String = TryCast(obj, String)
                    If Microsoft.VisualBasic.Right(stProjectDirectory, 1) <> "\" Then
                        stProjectDirectory &= "\"
                    End If

                    If (Not IsNothing(m_stDocumentationFile)) Then
                        '// Loop through each config and calculate what we think the output path should be
                        Dim i As Integer

                        For i = 0 To m_stDocumentationFile.Length - 1

                            If (Not IsNothing(OutputPathData)) Then
                                stOutputPath = TryCast(OutputPathData(i), String)
                            Else
                                GetProperty(VsProjPropId.VBPROJPROPID_OutputPath, obj)
                                stOutputPath = CType(obj, String)
                            End If

                            If (Not IsNothing(stOutputPath)) Then
                                If Microsoft.VisualBasic.Right(stOutputPath, 1) <> "\" Then
                                    stOutputPath &= "\"
                                End If

                                If (Path.IsPathRooted(stOutputPath)) Then
                                    '// stOutputPath is an Absolute path so check to see if its within the project path

                                    If (String.Compare(Path.GetFullPath(stProjectDirectory), _
                                                       Microsoft.VisualBasic.Left(Path.GetFullPath(stOutputPath), Len(stProjectDirectory)), _
                                                       StringComparison.Ordinal) = 0) Then

                                        '// The output path is within the project so suggest the output directory (or suggest just the filename
                                        '// which will put it in the default location

                                        m_stDocumentationFile(i) = stOutputPath & stAssemblyName & XML_FILE_EXTENSION

                                    Else

                                        '// The output path is outside the project so just suggest the project directory.
                                        m_stDocumentationFile(i) = stProjectDirectory & stAssemblyName & XML_FILE_EXTENSION

                                    End If

                                Else
                                    '// OutputPath is a Relative path so it will be based on the project directory. use
                                    '// the OutputPath to suggest a location for the documentation file
                                    m_stDocumentationFile(i) = stOutputPath & stAssemblyName & XML_FILE_EXTENSION
                                End If

                            End If
                        Next

                        '// Now if all the values are the same then set the textbox text
                        Dim objDocumentationFile As Object
                        objDocumentationFile = GetValueOrIndeterminateFromArray(m_stDocumentationFile)

                        If (Not (PropertyControlData.IsSpecialValue(objDocumentationFile))) Then
                            Me.txtXMLDocumentationFile.Text = TryCast(objDocumentationFile, String)
                        End If
                    End If
                End If

                Me.txtXMLDocumentationFile.Focus()
            Else
                '// Disable the checkbox
                Me.txtXMLDocumentationFile.Enabled = False
                Me.txtXMLDocumentationFile.Text = ""

                '// Clear the values
                Dim i As Integer
                For i = 0 To m_stDocumentationFile.Length - 1
                    m_stDocumentationFile(i) = ""
                Next
            End If

            If Not m_bInsideInternalUpdate Then
                SetDirty(Me.txtXMLDocumentationFile)
            End If
        End Sub


        ''' <summary>
        ''' Fired when the conditional compilations contents textbox has changed.  We are manually handling
        '''   events associated with this control, so we need to recalculate related values
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub DocumentationFile_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtXMLDocumentationFile.TextChanged
            If Not m_bInsideInternalUpdate Then
                Debug.Assert(m_stDocumentationFile IsNot Nothing)
                For i As Integer = 0 To m_stDocumentationFile.Length - 1
                    'store it
                    m_stDocumentationFile(i) = txtXMLDocumentationFile.Text
                Next

                'No need to mark the property dirty - the property page architecture hooks up the FormControl automatically
                '  to TextChanged and will mark it dirty, and will make sure it's persisted on LostFocus.
            End If
        End Sub

        Private Function IsPrefer32BitSupportedForPlatformTarget() As Boolean

            ' Get the current value of PlatformTarget

            Dim controlValue As Object = GetControlValueNative("PlatformTarget")

            If PropertyControlData.IsSpecialValue(controlValue) Then
                ' Property is missing or indeterminate
                Return False
            End If

            If Not TypeOf controlValue Is String Then
                Return False
            End If

            ' Prefer32Bit is only allowed for AnyCPU

            Dim stringValue As String = CStr(controlValue)

            If String.IsNullOrEmpty(stringValue) Then
                ' Allow if the value is blank (means AnyCPU)
                Return True
            End If

            Return String.Compare(stringValue, "AnyCPU", StringComparison.Ordinal) = 0

        End Function

        ''' <summary>
        ''' Determines whether the project associated with the given hierarchy is targeting .NET 4.5 or above
        ''' </summary>
        ''' <param name="hierarchy"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Function IsTargetingDotNetFramework45OrAbove(hierarchy As IVsHierarchy) As Boolean

            Dim propertyValue As Object = Nothing

            If VSErrorHandler.Failed(hierarchy.GetProperty(VSITEMID.ROOT, __VSHPROPID4.VSHPROPID_TargetFrameworkMoniker, propertyValue)) Then
                Return False
            End If

            If propertyValue Is Nothing Then
                Return False
            End If

            If Not TypeOf propertyValue Is String Then
                Return False
            End If

            Dim frameworkName As New FrameworkName(CStr(propertyValue))

            ' Verify that we are targeting .NET
            If String.Compare(frameworkName.Identifier, ".NETFramework", StringComparison.OrdinalIgnoreCase) <> 0 Then
                Return False
            End If

            ' Verify that the version of the target framework is >= 4.5
            Return frameworkName.Version.Major > 4 OrElse
                   (frameworkName.Version.Major = 4 AndAlso frameworkName.Version.Minor >= 5)

        End Function

        ''' <summary>
        ''' Determines whether the given hierarchy is an appcontainer project
        ''' </summary>
        ''' <param name="hierarchy"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Function IsAppContainerProject(hierarchy As IVsHierarchy) As Boolean

            Dim propertyValue As Object = Nothing

            If VSErrorHandler.Failed(hierarchy.GetProperty(VSITEMID.ROOT, __VSHPROPID5.VSHPROPID_AppContainer, propertyValue)) Then
                Return False
            End If

            If propertyValue Is Nothing Then
                Return False
            End If

            If Not TypeOf propertyValue Is Boolean Then
                Return False
            End If

            Return CBool(propertyValue)

        End Function

        Private Function IsPrefer32BitSupportedForTargetFramework() As Boolean

            Return IsTargetingDotNetFramework45OrAbove(Me.ProjectHierarchy) OrElse
                   IsAppContainerProject(Me.ProjectHierarchy)

        End Function

        Private Function IsPrefer32BitSupported() As Boolean

            Return IsPrefer32BitSupportedForPlatformTarget() AndAlso
                   IsExeProject() AndAlso
                   IsPrefer32BitSupportedForTargetFramework()

        End Function

        ' Holds the last value the Prefer32Bit check box had when enabled, so that the
        ' proper state is restored if the control is disabled and then later enabled
        Private lastEnabledPrefer32BitValue As Boolean

        Private Sub RefreshEnabledStatusForPrefer32Bit(control As CheckBox)

            Dim enabledBefore As Boolean = control.Enabled

            If control.Enabled Then
                Me.lastEnabledPrefer32BitValue = control.Checked
            End If

            control.Enabled = IsPrefer32BitSupported()

            If Not control.Enabled Then
                ' When disabled, we want to show an unchecked checkbox 
                ' regardless of the underlying property value.
                control.Checked = False
            ElseIf Not enabledBefore AndAlso control.Enabled Then
                ' If transitioning from disabled to enabled, restore the value of the checkbox.
                control.Checked = Me.lastEnabledPrefer32BitValue
            End If

        End Sub

        Protected Function Prefer32BitSet(control As Control, prop As PropertyDescriptor, value As Object) As Boolean

            If PropertyControlData.IsSpecialValue(value) Then
                ' Don't do anything if the value is missing or indeterminate
                Return False
            End If

            If Not TypeOf value Is Boolean Then
                ' Don't do anything if the value isn't of the expected type
                Return False
            End If

            CType(control, CheckBox).Checked = CBool(value)
            Return True
        End Function

        Protected Function Prefer32BitGet(control As Control, prop As PropertyDescriptor, ByRef value As Object) As Boolean

            Dim checkBox As CheckBox = CType(control, CheckBox)
            value = checkBox.Checked

            Return True
        End Function


#Region "Special handling of the conditional compilation constants textbox and the Define DEBUG/TRACE checkboxes"

        'Intended behavior:
        '  For simplified configurations mode ("Tools.Options.Projects and Solutions.Show Advanced Configurations" is off),
        '    we want the display to show only the release value for the DEBUG constant, and keep DEBUG defined always for
        '    the Debug configuration.  If the user changes the DEBUG constant checkbox in simplified mode, then the change
        '    should only affect the Debug configuration.
        '    For the TRACE constant checkbox, we want the normal behavior (show indeterminate if they're different, but they
        '    won't be for the default templates in simplified configs mode).
        '    The conditional compilation textbox likewise should show indeterminate if the debug and release values differ, but
        '    for the default templates they won't.
        '    This behavior is not easy to get, because the DEBUG/TRACE checkboxes are not actual properties in C# like they
        '    are in VB, but are rather parsed from the conditional compilation value.  The conditional compilation textbox
        '    then shows any remaining constants that the user defines besides DEBUG and TRACE>
        '  For advanced configurations, we still parse the conditional compilation constants into DEBUG, TRACE, and everything
        '    else, but we should use normal indeterminate behavior for all of these controls if the values differ in any of the
        '    selected configurations.
        '
        'Note: a minor disadvantage with the current implementation is that the property page architecture doesn't know about
        '  the virtual "DEBUG" and "TRACE" properties that we've created, so the undo/redo descriptions for changes to these
        '  properties will always just say "DefineConstants"




        ''' <summary>
        ''' Fired when the conditional compilations contents textbox has changed.  We are manually handling
        '''   events associated with this control, so we need to recalculate related values
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub DefineConstants_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtConditionalCompilationSymbols.TextChanged
            If Not m_bInsideInternalUpdate Then
                Debug.Assert(m_stCondCompSymbols IsNot Nothing)
                For i As Integer = 0 To m_stCondCompSymbols.Length - 1
                    'Parse the original compilation constants value for this configuration (we need to do this
                    '  to get the original DEBUG/TRACE values for these configurations - we can't rely on the
                    '  current control values for these two because they might be indeterminate)
                    Dim OriginalOtherConstants As String = ""
                    Dim DebugDefined, TraceDefined As Boolean
                    ParseConditionalCompilationConstants(m_stCondCompSymbols(i), DebugDefined, TraceDefined, OriginalOtherConstants)

                    'Now build the new string based off of the old DEBUG/TRACE values and the new string the user entered for any
                    '  other constants
                    Dim NewOtherConstants As String = txtConditionalCompilationSymbols.Text
                    Dim NewCondCompSymbols As String = NewOtherConstants
                    If DebugDefined Then
                        NewCondCompSymbols = AddSymbol(NewCondCompSymbols, Const_CondConstantDEBUG)
                    End If
                    If TraceDefined Then
                        NewCondCompSymbols = AddSymbol(NewCondCompSymbols, Const_CondConstantTRACE)
                    End If

                    '... and store it
                    m_stCondCompSymbols(i) = NewCondCompSymbols
                Next

                'No need to mark the property dirty - the property page architecture hooks up the FormControl automatically
                '  to TextChanged and will mark it dirty, and will make sure it's persisted on LostFocus.
            End If
        End Sub


        ''' <summary>
        ''' Fired when the "Define DEBUG Constant" check has changed.  We are manually handling
        '''   events associated with this control, so we need to recalculate related values.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub chkDefineDebug_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkDefineDebug.CheckedChanged
            If Not m_bInsideInternalUpdate Then
                Dim DebugIndexDoNotChange As Integer 'Index to avoid changing, if in simplified configs mode
                If IsSimplifiedConfigs() Then
                    'In simplified configs mode, we do not want to change the value of the DEBUG constant
                    '  in the Debug configuration, but rather only in the Release configuration
                    Debug.Assert(m_stCondCompSymbols.Length = 2, "In simplified configs, we should only have two configurations")
                    DebugIndexDoNotChange = GetIndexOfConfiguration(Const_DebugConfiguration)
                Else
                    DebugIndexDoNotChange = -1 'Go ahead and make changes in all selected configurations
                End If

                For i As Integer = 0 To m_stCondCompSymbols.Length - 1
                    If i <> DebugIndexDoNotChange Then
                        Select Case chkDefineDebug.CheckState
                            Case CheckState.Checked
                                'Make sure DEBUG is present in the configuration
                                m_stCondCompSymbols(i) = AddSymbol(m_stCondCompSymbols(i), Const_CondConstantDEBUG)
                            Case CheckState.Unchecked
                                'Remove DEBUG from the configuration
                                m_stCondCompSymbols(i) = RemoveSymbol(m_stCondCompSymbols(i), Const_CondConstantDEBUG)
                            Case Else
                                Debug.Fail("If the user changed the checked state, it should be checked or unchecked")
                        End Select
                    End If
                Next

                SetDirty(VsProjPropId.VBPROJPROPID_DefineConstants, True)
            End If
        End Sub


        ''' <summary>
        ''' Fired when the "Define DEBUG Constant" check has changed.  We are manually handling
        '''   events associated with this control, so we need to recalculate related values.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub chkDefineTrace_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkDefineTrace.CheckedChanged
            If Not m_bInsideInternalUpdate Then
                For i As Integer = 0 To m_stCondCompSymbols.Length - 1
                    Select Case chkDefineTrace.CheckState
                        Case CheckState.Checked
                            'Make sure TRACE is present in the configuration
                            m_stCondCompSymbols(i) = AddSymbol(m_stCondCompSymbols(i), Const_CondConstantTRACE)
                        Case CheckState.Unchecked
                            'Remove TRACE from the configuration
                            m_stCondCompSymbols(i) = RemoveSymbol(m_stCondCompSymbols(i), Const_CondConstantTRACE)
                        Case Else
                            Debug.Fail("If the user changed the checked state, it should be checked or unchecked")
                    End Select
                Next

                SetDirty(VsProjPropId.VBPROJPROPID_DefineConstants, True)
            End If
        End Sub


        ''' <summary>
        ''' Given DefineConstants string, parse it into a DEBUG value, a TRACE value, and everything else
        ''' </summary>
        ''' <param name="DefineConstantsFullValue"></param>
        ''' <param name="DebugDefined"></param>
        ''' <param name="TraceDefined"></param>
        ''' <param name="OtherConstants"></param>
        ''' <remarks></remarks>
        Private Sub ParseConditionalCompilationConstants(ByVal DefineConstantsFullValue As String, ByRef DebugDefined As Boolean, ByRef TraceDefined As Boolean, ByRef OtherConstants As String)
            'Start out with the full set of defined constants
            OtherConstants = DefineConstantsFullValue

            'Check for DEBUG
            If (FindSymbol(OtherConstants, Const_CondConstantDEBUG)) Then
                DebugDefined = True

                'Strip it out
                OtherConstants = RemoveSymbol(OtherConstants, Const_CondConstantDEBUG)
            Else
                DebugDefined = False
            End If

            'Check for TRACE
            If (FindSymbol(OtherConstants, Const_CondConstantTRACE)) Then
                TraceDefined = True

                'Strip it out
                OtherConstants = RemoveSymbol(OtherConstants, Const_CondConstantTRACE)
            Else
                TraceDefined = False
            End If
        End Sub

        Function GetValueOrIndeterminateFromArray(ByVal Values() As Object) As Object
            'Determine if all the values are the same or not
            If Values Is Nothing OrElse Values.Length = 0 Then
                Debug.Fail("Bad Values array")
                Throw New ArgumentNullException("Values")
            End If

            Dim Value As Object = Values(0)
            For i As Integer = 0 + 1 To Values.Length - 1
                'Perform object comparison
                If (Value Is Nothing OrElse Values(i) Is Nothing) Then
                    If Value IsNot Values(i) Then
                        Return PropertyControlData.Indeterminate
                    End If
                ElseIf Not Value.Equals(Values(i)) Then
                    Return PropertyControlData.Indeterminate
                End If
            Next

            'They were all the same
            Return Value
        End Function

        ''' <summary>
        ''' Multi-value setter for the conditional compilation constants value.  We parse the values and determine
        '''   what to display in the textbox and checkboxes.
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="prop"></param>
        ''' <param name="values"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ConditionalCompilationSet(ByVal control As Control, ByVal prop As PropertyDescriptor, ByVal values() As Object) As Boolean
            Debug.Assert(values IsNot Nothing)

            'Store off the conditional full (unparsed) compilation strings, we'll need this in the getter (because the
            '  values displayed in the controls are lossy when there are indeterminate values).
            ReDim m_stCondCompSymbols(values.Length - 1)
            values.CopyTo(m_stCondCompSymbols, 0)

            m_bInsideInternalUpdate = True
            Try
                Dim DebugDefinedValues(values.Length - 1) As Object 'Defined as object so we can use GetValueOrIndeterminateFromArray
                Dim TraceDefinedValues(values.Length - 1) As Object
                Dim OtherConstantsValues(values.Length - 1) As String

                'Parse out each individual set of DefineConstants values from the project
                For i As Integer = 0 To values.Length - 1
                    Dim FullDefineConstantsValue As String = DirectCast(values(i), String)
                    Dim DebugDefinedValue, TraceDefinedValue As Boolean
                    Dim OtherConstantsValue As String = ""

                    ParseConditionalCompilationConstants(FullDefineConstantsValue, DebugDefinedValue, TraceDefinedValue, OtherConstantsValue)
                    DebugDefinedValues(i) = DebugDefinedValue
                    TraceDefinedValues(i) = TraceDefinedValue
                    OtherConstantsValues(i) = OtherConstantsValue
                Next

                'Figure out whether the values each configuration are the same or different.  For each
                '  of these properties, get either the value which is the same across all of the values,
                '  or get a value of Indeterminate.
                Dim DebugDefined As Object = GetValueOrIndeterminateFromArray(DebugDefinedValues)
                Dim TraceDefined As Object = GetValueOrIndeterminateFromArray(TraceDefinedValues)
                Dim OtherConstants As Object = GetValueOrIndeterminateFromArray(OtherConstantsValues)

                If IsSimplifiedConfigs() Then
                    'Special behavior for simplified configurations - we want to only display the
                    '  release value of the DEBUG checkbox.
                    Dim ReleaseIndex As Integer = GetIndexOfConfiguration(Const_ReleaseConfiguration)
                    If ReleaseIndex >= 0 Then
                        DebugDefined = DebugDefinedValues(ReleaseIndex) 'Get the release-config value for DEBUG constant
                    End If
                End If

                'Finally, set the control values to their calculated state
                If PropertyControlData.IsSpecialValue(DebugDefined) Then
                    chkDefineDebug.CheckState = CheckState.Indeterminate
                Else
                    SetCheckboxDeterminateState(chkDefineDebug, CBool(DebugDefined))
                End If
                If PropertyControlData.IsSpecialValue(TraceDefined) Then
                    chkDefineTrace.CheckState = CheckState.Indeterminate
                Else
                    SetCheckboxDeterminateState(chkDefineTrace, CBool(TraceDefined))
                End If
                If PropertyControlData.IsSpecialValue(OtherConstants) Then
                    txtConditionalCompilationSymbols.Text = ""
                Else
                    txtConditionalCompilationSymbols.Text = DirectCast(OtherConstants, String)
                End If

            Finally
                m_bInsideInternalUpdate = False
            End Try

            Return True
        End Function


        ''' <summary>
        ''' Multi-value getter for the conditional compilation constants values.
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="prop"></param>
        ''' <param name="values"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ConditionalCompilationGet(ByVal control As Control, ByVal prop As PropertyDescriptor, ByRef values() As Object) As Boolean
            'Fetch the original values we stored in the setter (the values stored in the controls are lossy when there are indeterminate values)
            Debug.Assert(m_stCondCompSymbols IsNot Nothing)
            ReDim values(m_stCondCompSymbols.Length - 1)
            m_stCondCompSymbols.CopyTo(values, 0)
            Return True
        End Function


        ''' <summary>
        ''' Searches in the RawPropertiesObjects for a configuration object whose name matches the name passed in,
        '''   and returns the index to it.
        ''' </summary>
        ''' <param name="ConfigurationName"></param>
        ''' <returns>The index of the found configuration, or -1 if it was not found.</returns>
        ''' <remarks>
        ''' We're only guaranteed to find the "Debug" or "Release" configurations when in
        '''   simplified configuration mode.
        ''' </remarks>
        Private Function GetIndexOfConfiguration(ByVal ConfigurationName As String) As Integer
            Debug.Assert(IsSimplifiedConfigs, "Shouldn't be calling this in advanced configs mode - not guaranteed to have Debug/Release configurations")

            Dim DefineConstantsData As PropertyControlData = GetPropertyControlData(VsProjPropId.VBPROJPROPID_DefineConstants)
            Debug.Assert(DefineConstantsData IsNot Nothing)

            Dim Objects() As Object = RawPropertiesObjects(DefineConstantsData)
            Dim Index As Integer = 0
            For Each Obj As Object In Objects
                Debug.Assert(Obj IsNot Nothing, "Why was Nothing passed in as a config object?")
                Dim Config As IVsCfg = TryCast(Obj, IVsCfg)
                Debug.Assert(Config IsNot Nothing, "Object was not IVsCfg")
                If Config IsNot Nothing Then
                    Dim ConfigName As String = Nothing
                    Dim PlatformName As String = Nothing
                    Common.ShellUtil.GetConfigAndPlatformFromIVsCfg(Config, ConfigName, PlatformName)
                    If ConfigurationName.Equals(ConfigName, StringComparison.CurrentCultureIgnoreCase) Then
                        'Found it - return the index to it
                        Return Index
                    End If
                End If
                Index += 1
            Next

            Debug.Fail("Unable to find the configuration '" & ConfigurationName & "'")
            Return -1
        End Function


        ''' <summary>
        ''' Returns whether or not we're in simplified config mode for this project, which means that
        '''   we hide the configuration/platform comboboxes.
        ''' </summary>
        ''' <remarks></remarks>
        Function IsSimplifiedConfigs() As Boolean
            Return Common.ShellUtil.GetIsSimplifiedConfigMode(ProjectHierarchy)
        End Function


        ''' <summary>
        ''' Given a string containing conditional compilation constants, adds the given constant to it, if it
        '''   doesn't already exist.
        ''' </summary>
        ''' <param name="stOldCondCompConstants"></param>
        ''' <param name="stSymbol"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function AddSymbol(ByVal stOldCondCompConstants As String, ByVal stSymbol As String) As String
            '// See if we find it
            Dim rgConstants() As String
            Dim bFound As Boolean = False
            Dim stNewConstants As String = ""

            If (Not (IsNothing(stOldCondCompConstants))) Then
                rgConstants = stOldCondCompConstants.Split(New [Char]() {";"c})

                Dim stTemp As String

                If (Not (IsNothing(rgConstants))) Then
                    For Each stTemp In rgConstants
                        If (String.Compare(Trim(stTemp), stSymbol, StringComparison.Ordinal) = 0) Then
                            bFound = True
                            Exit For
                        End If
                    Next
                End If
            End If

            If (Not bFound) Then
                '// Add it to the beginning
                stNewConstants = stSymbol

                If stOldCondCompConstants <> "" Then
                    stNewConstants += ";"
                End If
                stNewConstants += stOldCondCompConstants

                Return stNewConstants
            Else
                Return stOldCondCompConstants
            End If
        End Function

        ''' <summary>
        ''' Given a string containing conditional compilation constants, determines if the given constant is defined in it
        ''' </summary>
        ''' <param name="stOldCondCompConstants"></param>
        ''' <param name="stSymbol"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function FindSymbol(ByVal stOldCondCompConstants As String, ByVal stSymbol As String) As Boolean
            '// See if we find it
            Dim rgConstants() As String

            If (Not (IsNothing(stOldCondCompConstants))) Then
                rgConstants = stOldCondCompConstants.Split(New [Char]() {";"c})

                Dim stTemp As String

                If (Not (IsNothing(rgConstants))) Then
                    For Each stTemp In rgConstants
                        If (String.Compare(Trim(stTemp), stSymbol, StringComparison.Ordinal) = 0) Then
                            Return True
                        End If
                    Next
                End If
            End If
            Return False
        End Function


        ''' <summary>
        ''' Given a string containing conditional compilation constants, removes the given constant from it, if it
        '''   is in the list.
        ''' </summary>
        ''' <param name="stOldCondCompConstants"></param>
        ''' <param name="stSymbol"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Function RemoveSymbol(ByVal stOldCondCompConstants As String, ByVal stSymbol As String) As String
            '// Look for the DEBUG constant
            Dim rgConstants() As String
            Dim stNewConstants As String = ""

            If (Not (IsNothing(stOldCondCompConstants))) Then
                rgConstants = stOldCondCompConstants.Split(New [Char]() {";"c})

                Dim stTemp As String

                If (Not (IsNothing(rgConstants))) Then
                    For Each stTemp In rgConstants
                        If (String.Compare(Trim(stTemp), stSymbol, StringComparison.Ordinal) <> 0) Then
                            If (stNewConstants <> "") Then
                                stNewConstants += ";"
                            End If

                            stNewConstants += stTemp
                        End If
                    Next
                End If
            Else
                stNewConstants = ""
            End If

            Return stNewConstants
        End Function

#End Region

        Private Sub overarchingTableLayoutPanel_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles overarchingTableLayoutPanel.Paint

        End Sub


        Private Sub Label1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblOtherFlags.Click

        End Sub
    End Class

    <System.Runtime.InteropServices.GuidAttribute("FAC0A17E-2E70-4211-916A-0D34FB708BFF")> _
    <ComVisible(True)> _
    <CLSCompliantAttribute(False)> _
    Public NotInheritable Class FSharpBuildPropPageComClass
        Inherits FSharpPropPageBase

        Protected Overrides ReadOnly Property Title() As String
            Get
                Return SR.GetString(SR.PPG_BuildTitle)
            End Get
        End Property

        Protected Overrides ReadOnly Property ControlType() As System.Type
            Get
                Return GetType(BuildPropPage)
            End Get
        End Property

        Protected Overrides Function CreateControl() As Control
            Return New BuildPropPage
        End Function

    End Class

End Namespace
