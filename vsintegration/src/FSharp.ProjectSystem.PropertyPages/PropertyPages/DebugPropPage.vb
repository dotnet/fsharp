' Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

Imports System
Imports Microsoft.VisualBasic
Imports Microsoft.VisualStudio.Editors.Common
Imports VB = Microsoft.VisualBasic
Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Diagnostics
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.Editors.Interop
Imports System.IO
Imports VSLangProj80

Namespace Microsoft.VisualStudio.Editors.PropertyPages

    Friend Class DebugPropPage
        'Inherits UserControl
        Inherits PropPageUserControlBase

        Private m_controlGroup As Control()()

        'PERF: A note about the labels used as lines.  The 3D label is being set to 1 px high,
        '   so you’re really only using the grey part of it.  Using BorderStyle.Fixed3D seems
        '   to fire an extra resize OnHandleCreated.  The simple solution is to use BorderStyle.None 
        '   and BackColor = SystemColors.ControlDark.

#Region " Windows Form Designer generated code "

        Public Sub New()
            MyBase.New()

            'This call is required by the Windows Form Designer.
            InitializeComponent()

            'Add any initialization after the InitializeComponent() call
            AddChangeHandlers()

            Me.MinimumSize = Me.Size
        End Sub

        'Form overrides dispose to clean up the component list.
        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If Not (components Is Nothing) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        Friend WithEvents rbStartProject As System.Windows.Forms.RadioButton
        Friend WithEvents rbStartProgram As System.Windows.Forms.RadioButton
        Friend WithEvents StartProgram As System.Windows.Forms.TextBox
        Friend WithEvents RemoteDebugEnabled As System.Windows.Forms.CheckBox
        Friend WithEvents StartArguments As MultilineTextBoxRejectsEnter
        Friend WithEvents StartWorkingDirectory As System.Windows.Forms.TextBox
        Friend WithEvents RemoteDebugMachine As System.Windows.Forms.TextBox
        Friend WithEvents EnableUnmanagedDebugging As System.Windows.Forms.CheckBox
        Friend WithEvents EnableSQLServerDebugging As System.Windows.Forms.CheckBox
        Friend WithEvents UseVSHostingProcess As System.Windows.Forms.CheckBox
        Friend WithEvents StartProgramBrowse As System.Windows.Forms.Button
        Friend WithEvents StartWorkingDirectoryBrowse As System.Windows.Forms.Button
        Friend WithEvents StartOptionsLabel As System.Windows.Forms.Label
        Friend WithEvents CommandLineArgsLabel As System.Windows.Forms.Label
        Friend WithEvents WorkingDirLabel As System.Windows.Forms.Label
        Friend WithEvents EnableDebuggerLabelLine As System.Windows.Forms.Label
        Friend WithEvents EnableDebuggerLabel As System.Windows.Forms.Label
        Friend WithEvents StartActionLabel As System.Windows.Forms.Label
        Friend WithEvents StartActionLabelLine As System.Windows.Forms.Label
        Friend WithEvents overarchingTableLayoutPanel As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents startActionTableLayoutPanel As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents startOptionsTableLayoutPanel As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents enableDebuggersTableLayoutPanel As System.Windows.Forms.TableLayoutPanel
        Friend WithEvents StartOptionsLabelLine As System.Windows.Forms.Label

        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DebugPropPage))
            Me.StartActionLabel = New System.Windows.Forms.Label()
            Me.StartActionLabelLine = New System.Windows.Forms.Label()
            Me.rbStartProject = New System.Windows.Forms.RadioButton()
            Me.rbStartProgram = New System.Windows.Forms.RadioButton()
            Me.StartProgram = New System.Windows.Forms.TextBox()
            Me.StartProgramBrowse = New System.Windows.Forms.Button()
            Me.StartOptionsLabelLine = New System.Windows.Forms.Label()
            Me.StartOptionsLabel = New System.Windows.Forms.Label()
            Me.CommandLineArgsLabel = New System.Windows.Forms.Label()
            Me.WorkingDirLabel = New System.Windows.Forms.Label()
            Me.RemoteDebugEnabled = New System.Windows.Forms.CheckBox()
            Me.StartArguments = New Microsoft.VisualStudio.Editors.PropertyPages.DebugPropPage.MultilineTextBoxRejectsEnter()
            Me.StartWorkingDirectory = New System.Windows.Forms.TextBox()
            Me.RemoteDebugMachine = New System.Windows.Forms.TextBox()
            Me.StartWorkingDirectoryBrowse = New System.Windows.Forms.Button()
            Me.EnableDebuggerLabelLine = New System.Windows.Forms.Label()
            Me.EnableDebuggerLabel = New System.Windows.Forms.Label()
            Me.EnableUnmanagedDebugging = New System.Windows.Forms.CheckBox()
            Me.UseVSHostingProcess = New System.Windows.Forms.CheckBox()
            Me.EnableSQLServerDebugging = New System.Windows.Forms.CheckBox()
            Me.overarchingTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
            Me.startActionTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
            Me.startOptionsTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
            Me.enableDebuggersTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
            Me.overarchingTableLayoutPanel.SuspendLayout()
            Me.startActionTableLayoutPanel.SuspendLayout()
            Me.startOptionsTableLayoutPanel.SuspendLayout()
            Me.enableDebuggersTableLayoutPanel.SuspendLayout()
            Me.SuspendLayout()
            '
            'StartActionLabel
            '
            resources.ApplyResources(Me.StartActionLabel, "StartActionLabel")
            Me.StartActionLabel.Name = "StartActionLabel"
            '
            'StartActionLabelLine
            '
            Me.StartActionLabelLine.AccessibleRole = System.Windows.Forms.AccessibleRole.Graphic
            resources.ApplyResources(Me.StartActionLabelLine, "StartActionLabelLine")
            Me.StartActionLabelLine.BackColor = System.Drawing.SystemColors.ControlDark
            Me.StartActionLabelLine.Name = "StartActionLabelLine"
            '
            'rbStartProject
            '
            resources.ApplyResources(Me.rbStartProject, "rbStartProject")
            Me.rbStartProject.Name = "rbStartProject"
            '
            'rbStartProgram
            '
            resources.ApplyResources(Me.rbStartProgram, "rbStartProgram")
            Me.rbStartProgram.Name = "rbStartProgram"
            '
            'StartProgram
            '
            resources.ApplyResources(Me.StartProgram, "StartProgram")
            Me.StartProgram.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
            Me.StartProgram.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem
            Me.StartProgram.Name = "StartProgram"
            '
            'StartProgramBrowse
            '
            resources.ApplyResources(Me.StartProgramBrowse, "StartProgramBrowse")
            Me.StartProgramBrowse.Name = "StartProgramBrowse"
            '
            'StartOptionsLabelLine
            '
            Me.StartOptionsLabelLine.AccessibleRole = System.Windows.Forms.AccessibleRole.Graphic
            resources.ApplyResources(Me.StartOptionsLabelLine, "StartOptionsLabelLine")
            Me.StartOptionsLabelLine.BackColor = System.Drawing.SystemColors.ControlDark
            Me.StartOptionsLabelLine.Name = "StartOptionsLabelLine"
            '
            'StartOptionsLabel
            '
            resources.ApplyResources(Me.StartOptionsLabel, "StartOptionsLabel")
            Me.StartOptionsLabel.Name = "StartOptionsLabel"
            '
            'CommandLineArgsLabel
            '
            resources.ApplyResources(Me.CommandLineArgsLabel, "CommandLineArgsLabel")
            Me.CommandLineArgsLabel.Name = "CommandLineArgsLabel"
            '
            'WorkingDirLabel
            '
            resources.ApplyResources(Me.WorkingDirLabel, "WorkingDirLabel")
            Me.WorkingDirLabel.Name = "WorkingDirLabel"
            '
            'RemoteDebugEnabled
            '
            resources.ApplyResources(Me.RemoteDebugEnabled, "RemoteDebugEnabled")
            Me.RemoteDebugEnabled.Name = "RemoteDebugEnabled"
            '
            'StartArguments
            '
            resources.ApplyResources(Me.StartArguments, "StartArguments")
            Me.StartArguments.Name = "StartArguments"
            '
            'StartWorkingDirectory
            '
            resources.ApplyResources(Me.StartWorkingDirectory, "StartWorkingDirectory")
            Me.StartWorkingDirectory.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
            Me.StartWorkingDirectory.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem
            Me.StartWorkingDirectory.Name = "StartWorkingDirectory"
            '
            'RemoteDebugMachine
            '
            resources.ApplyResources(Me.RemoteDebugMachine, "RemoteDebugMachine")
            Me.RemoteDebugMachine.Name = "RemoteDebugMachine"
            '
            'StartWorkingDirectoryBrowse
            '
            resources.ApplyResources(Me.StartWorkingDirectoryBrowse, "StartWorkingDirectoryBrowse")
            Me.StartWorkingDirectoryBrowse.Name = "StartWorkingDirectoryBrowse"
            '
            'EnableDebuggerLabelLine
            '
            Me.EnableDebuggerLabelLine.AccessibleRole = System.Windows.Forms.AccessibleRole.Graphic
            resources.ApplyResources(Me.EnableDebuggerLabelLine, "EnableDebuggerLabelLine")
            Me.EnableDebuggerLabelLine.BackColor = System.Drawing.SystemColors.ControlDark
            Me.EnableDebuggerLabelLine.Name = "EnableDebuggerLabelLine"
            '
            'EnableDebuggerLabel
            '
            resources.ApplyResources(Me.EnableDebuggerLabel, "EnableDebuggerLabel")
            Me.EnableDebuggerLabel.Name = "EnableDebuggerLabel"
            '
            'EnableUnmanagedDebugging
            '
            resources.ApplyResources(Me.EnableUnmanagedDebugging, "EnableUnmanagedDebugging")
            Me.overarchingTableLayoutPanel.SetColumnSpan(Me.EnableUnmanagedDebugging, 2)
            Me.EnableUnmanagedDebugging.Name = "EnableUnmanagedDebugging"
            '
            'UseVSHostingProcess
            '
            resources.ApplyResources(Me.UseVSHostingProcess, "UseVSHostingProcess")
            Me.overarchingTableLayoutPanel.SetColumnSpan(Me.UseVSHostingProcess, 2)
            Me.UseVSHostingProcess.Name = "UseVSHostingProcess"
            '
            'EnableSQLServerDebugging
            '
            resources.ApplyResources(Me.EnableSQLServerDebugging, "EnableSQLServerDebugging")
            Me.overarchingTableLayoutPanel.SetColumnSpan(Me.EnableSQLServerDebugging, 2)
            Me.EnableSQLServerDebugging.Name = "EnableSQLServerDebugging"
            '
            'overarchingTableLayoutPanel
            '
            resources.ApplyResources(Me.overarchingTableLayoutPanel, "overarchingTableLayoutPanel")
            Me.overarchingTableLayoutPanel.Controls.Add(Me.startActionTableLayoutPanel, 0, 0)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.rbStartProject, 0, 1)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.rbStartProgram, 0, 2)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.StartProgram, 1, 2)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.StartProgramBrowse, 2, 2)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.startOptionsTableLayoutPanel, 0, 4)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.CommandLineArgsLabel, 0, 5)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.StartArguments, 1, 5)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.WorkingDirLabel, 0, 6)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.StartWorkingDirectory, 1, 6)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.StartWorkingDirectoryBrowse, 2, 6)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.RemoteDebugEnabled, 0, 7)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.RemoteDebugMachine, 1, 7)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.enableDebuggersTableLayoutPanel, 0, 8)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.EnableUnmanagedDebugging, 0, 9)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.EnableSQLServerDebugging, 0, 10)
            Me.overarchingTableLayoutPanel.Controls.Add(Me.UseVSHostingProcess, 0, 11)
            Me.overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel"
            '
            'startActionTableLayoutPanel
            '
            resources.ApplyResources(Me.startActionTableLayoutPanel, "startActionTableLayoutPanel")
            Me.overarchingTableLayoutPanel.SetColumnSpan(Me.startActionTableLayoutPanel, 3)
            Me.startActionTableLayoutPanel.Controls.Add(Me.StartActionLabel, 0, 0)
            Me.startActionTableLayoutPanel.Controls.Add(Me.StartActionLabelLine, 1, 0)
            Me.startActionTableLayoutPanel.Name = "startActionTableLayoutPanel"
            '
            'startOptionsTableLayoutPanel
            '
            resources.ApplyResources(Me.startOptionsTableLayoutPanel, "startOptionsTableLayoutPanel")
            Me.overarchingTableLayoutPanel.SetColumnSpan(Me.startOptionsTableLayoutPanel, 3)
            Me.startOptionsTableLayoutPanel.Controls.Add(Me.StartOptionsLabel, 0, 0)
            Me.startOptionsTableLayoutPanel.Controls.Add(Me.StartOptionsLabelLine, 1, 0)
            Me.startOptionsTableLayoutPanel.Name = "startOptionsTableLayoutPanel"
            '
            'enableDebuggersTableLayoutPanel
            '
            resources.ApplyResources(Me.enableDebuggersTableLayoutPanel, "enableDebuggersTableLayoutPanel")
            Me.overarchingTableLayoutPanel.SetColumnSpan(Me.enableDebuggersTableLayoutPanel, 3)
            Me.enableDebuggersTableLayoutPanel.Controls.Add(Me.EnableDebuggerLabel, 0, 0)
            Me.enableDebuggersTableLayoutPanel.Controls.Add(Me.EnableDebuggerLabelLine, 1, 0)
            Me.enableDebuggersTableLayoutPanel.Name = "enableDebuggersTableLayoutPanel"
            '
            'DebugPropPage
            '
            resources.ApplyResources(Me, "$this")
            Me.Controls.Add(Me.overarchingTableLayoutPanel)
            Me.Name = "DebugPropPage"
            Me.overarchingTableLayoutPanel.ResumeLayout(False)
            Me.overarchingTableLayoutPanel.PerformLayout()
            Me.startActionTableLayoutPanel.ResumeLayout(False)
            Me.startActionTableLayoutPanel.PerformLayout()
            Me.startOptionsTableLayoutPanel.ResumeLayout(False)
            Me.startOptionsTableLayoutPanel.PerformLayout()
            Me.enableDebuggersTableLayoutPanel.ResumeLayout(False)
            Me.enableDebuggersTableLayoutPanel.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

#End Region

#Region "Class MultilineTextBoxRejectsEnter"

        ''' <summary>
        ''' A multi-line textbox control which does not accept ENTER as valid input.
        ''' </summary>
        ''' <remarks></remarks>
        Friend Class MultilineTextBoxRejectsEnter
            Inherits TextBox

            Public Sub New()
                MyBase.Multiline = True
            End Sub

            Protected Overrides Function IsInputChar(ByVal charCode As Char) As Boolean
                If charCode = vbLf OrElse charCode = vbCr Then
                    Return False
                End If

                Return MyBase.IsInputChar(charCode)
            End Function

            Protected Overrides Function ProcessDialogChar(ByVal charCode As Char) As Boolean
                If charCode = vbLf OrElse charCode = vbCr Then
                    Return True
                End If

                Return MyBase.ProcessDialogChar(charCode)
            End Function
        End Class

#End Region

        Protected Overrides ReadOnly Property ControlData() As PropertyControlData()
            Get
                If m_ControlData Is Nothing Then
                    m_ControlData = New PropertyControlData() { _
                        New PropertyControlData(VsProjPropId.VBPROJPROPID_StartArguments, "StartArguments", Me.StartArguments, ControlDataFlags.PersistedInProjectUserFile, New Control() {Me.CommandLineArgsLabel}), _
                        New PropertyControlData(VsProjPropId.VBPROJPROPID_StartWorkingDirectory, "StartWorkingDirectory", Me.StartWorkingDirectory, ControlDataFlags.PersistedInProjectUserFile, New Control() {Me.StartWorkingDirectoryBrowse, Me.WorkingDirLabel}), _
                        New PropertyControlData(VsProjPropId.VBPROJPROPID_StartProgram, "StartProgram", Me.StartProgram, ControlDataFlags.PersistedInProjectUserFile), _
                        New PropertyControlData(VsProjPropId.VBPROJPROPID_StartAction, "StartAction", Nothing, _
                                AddressOf Me.StartActionSet, AddressOf Me.StartActionGet, ControlDataFlags.PersistedInProjectUserFile, _
                                New Control() {startActionTableLayoutPanel, rbStartProject, rbStartProgram, StartProgram, StartProgramBrowse}), _
                        New PropertyControlData(VsProjPropId.VBPROJPROPID_EnableSQLServerDebugging, "EnableSQLServerDebugging", Me.EnableSQLServerDebugging, ControlDataFlags.PersistedInProjectUserFile), _
                        New PropertyControlData(VsProjPropId.VBPROJPROPID_EnableUnmanagedDebugging, "EnableUnmanagedDebugging", Me.EnableUnmanagedDebugging, ControlDataFlags.PersistedInProjectUserFile), _
                        New PropertyControlData(VsProjPropId.VBPROJPROPID_RemoteDebugMachine, "RemoteDebugMachine", Me.RemoteDebugMachine, ControlDataFlags.PersistedInProjectUserFile), _
                        New PropertyControlData(VsProjPropId.VBPROJPROPID_RemoteDebugEnabled, "RemoteDebugEnabled", Me.RemoteDebugEnabled, ControlDataFlags.PersistedInProjectUserFile), _
                        New PropertyControlData(VsProjPropId80.VBPROJPROPID_UseVSHostingProcess, "UseVSHostingProcess", Me.UseVSHostingProcess) _
                        }
                End If
                Return m_ControlData
            End Get
        End Property


        Protected Overrides ReadOnly Property ValidationControlGroups() As Control()()
            Get
                If m_controlGroup Is Nothing Then
                    m_controlGroup = New Control()() { _
                        New Control() {rbStartProject, rbStartProgram, StartProgram, StartProgramBrowse}, _
                        New Control() {RemoteDebugEnabled, StartWorkingDirectory, RemoteDebugMachine, StartWorkingDirectoryBrowse} _
                        }
                End If
                Return m_controlGroup
            End Get
        End Property

        Private Function StartActionSet(ByVal control As Control, ByVal prop As PropertyDescriptor, ByVal value As Object) As Boolean
            Dim originalInsideInit As Boolean = MyBase.m_fInsideInit
            MyBase.m_fInsideInit = True
            Try
                Dim action As VSLangProj.prjStartAction
                If PropertyControlData.IsSpecialValue(value) Then 'Indeterminate or IsMissing
                    action = VSLangProj.prjStartAction.prjStartActionNone
                Else
                    action = CType(value, VSLangProj.prjStartAction)
                End If

                Me.rbStartProject.Checked = (action = VSLangProj.prjStartAction.prjStartActionProject)
                Me.rbStartProgram.Checked = (action = VSLangProj.prjStartAction.prjStartActionProgram)

                If action = VSLangProj.prjStartAction.prjStartActionProject Then
                    EnableControl(UseVSHostingProcess, True)
                Else
                    ' We don't want to enable this unless the the startup action is to start the project
                    UseVSHostingProcess.Enabled = False
                End If
            Finally
                MyBase.m_fInsideInit = originalInsideInit
            End Try
            Return True
        End Function

        Private Function StartActionGetValue() As VSLangProj.prjStartAction
            Dim action As VSLangProj.prjStartAction

            If Me.rbStartProject.Checked Then
                action = VSLangProj.prjStartAction.prjStartActionProject
            ElseIf Me.rbStartProgram.Checked Then
                action = VSLangProj.prjStartAction.prjStartActionProgram
            Else
                action = VSLangProj.prjStartAction.prjStartActionNone
            End If

            Return action
        End Function

        Private Function StartActionGet(ByVal control As Control, ByVal prop As PropertyDescriptor, ByRef value As Object) As Boolean
            value = StartActionGetValue()
            Return True
        End Function

        Protected Overrides Sub EnableAllControls(ByVal _enabled As Boolean)
            MyBase.EnableAllControls(_enabled)

            GetPropertyControlData(VsProjPropId.VBPROJPROPID_StartAction).EnableControls(_enabled)
            GetPropertyControlData(VsProjPropId.VBPROJPROPID_StartProgram).EnableControls(_enabled)

            If _enabled AndAlso StartActionGetValue() = VSLangProj.prjStartAction.prjStartActionProject Then
                EnableControl(Me.UseVSHostingProcess, True)
            Else
                ' We don't want to enable this unless the the startup action is to start the project
                Me.UseVSHostingProcess.Enabled = False
            End If
        End Sub

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

            startActionTableLayoutPanel.Visible = True
            rbStartProject.Visible = True
            rbStartProgram.Visible = True
            StartProgram.Visible = True
            StartProgramBrowse.Visible = True
            RemoteDebugMachine.Enabled = RemoteDebugEnabled.Checked

            'We want the page to grow as needed.  However, we can't use AutoSize, because
            '  if the container window is made too small to show all the controls, we need
            '  the property page to *not* shrink past that point, otherwise we won't get
            '  a horizontal scrollbar.
            'So we fix the width at the width that the page naturally wants to be.
            Me.Size = Me.GetPreferredSize(System.Drawing.Size.Empty)

        End Sub

        Private Sub rbStartAction_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rbStartProgram.CheckedChanged, rbStartProject.CheckedChanged
            Dim action As VSLangProj.prjStartAction = StartActionGetValue()
            Me.StartProgram.Enabled = (action = VSLangProj.prjStartAction.prjStartActionProgram)
            Me.StartProgramBrowse.Enabled = Me.StartProgram.Enabled

            If Not m_fInsideInit Then
                Dim button As RadioButton = CType(sender, RadioButton)
                If button.Checked Then
                    SetDirty(VsProjPropId.VBPROJPROPID_StartAction, True)
                Else
                    'IsDirty = True
                    SetDirty(VsProjPropId.VBPROJPROPID_StartAction, False)
                End If

                If StartActionGetValue() = VSLangProj.prjStartAction.prjStartActionProject Then
                    EnableControl(Me.UseVSHostingProcess, True)
                Else
                    ' We don't want to enable this unless the the startup action is to start the project
                    Me.UseVSHostingProcess.Enabled = False
                End If

                If Me.StartProgram.Enabled Then
                    Me.StartProgram.Focus()
                    DelayValidate(StartProgram)     ' we need validate StartProgram to make sure it is not empty
                End If
            End If
        End Sub

        Function GetDebugPathProjectPath() As String
            Dim fullPathProperty As EnvDTE.Property = DTEProject.Properties.Item("FullPath")
            If fullPathProperty IsNot Nothing AndAlso fullPathProperty.Value IsNot Nothing Then
                Return CType(fullPathProperty.Value, String)
            Else
                Return ""
            End If
        End Function

        Private Sub StartWorkingDirectoryBrowse_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles StartWorkingDirectoryBrowse.Click
            Dim sInitialDirectory As String
            Dim DirName As String = ""

            SkipValidating(StartWorkingDirectory)   ' skip this because we will pop up dialog to edit it...
            ProcessDelayValidationQueue(False)

            sInitialDirectory = Trim(Me.StartWorkingDirectory.Text)
            If sInitialDirectory = "" Then
                Try
                    sInitialDirectory = Path.Combine(GetDebugPathProjectPath(), GetSelectedConfigOutputPath())
                Catch ex As IO.IOException
                    'Ignore
                Catch ex As Exception
                    Common.RethrowIfUnrecoverable(ex)
                    Debug.Fail("Exception getting project output path for selected config: " & ex.Message)
                End Try
            End If

            If GetDirectoryViaBrowse(sInitialDirectory, SR.GetString(SR.PPG_SelectWorkingDirectoryTitle), DirName) Then
                StartProgramBrowse.Focus()
                StartWorkingDirectory.Text = DirName
                SetDirty(StartWorkingDirectory, True)
            Else
                StartWorkingDirectoryBrowse.Focus()
                DelayValidate(StartWorkingDirectory)
            End If

        End Sub

        Private Sub RemoteDebugEnabled_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles RemoteDebugEnabled.CheckedChanged
            RemoteDebugMachine.Enabled = RemoteDebugEnabled.Checked

            If Not m_fInsideInit Then
                If RemoteDebugEnabled.Checked Then
                    RemoteDebugMachine.Focus()
                    DelayValidate(RemoteDebugMachine)
                Else
                    DelayValidate(StartWorkingDirectory)
                End If
            End If
        End Sub

        Private Sub StartProgramBrowse_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles StartProgramBrowse.Click
            Dim FileName As String = Nothing

            SkipValidating(StartProgram)
            ProcessDelayValidationQueue(False)

            If GetFileViaBrowse("", FileName, Common.CreateDialogFilter(SR.GetString(SR.PPG_ExeFilesFilter), ".exe")) Then
                StartProgramBrowse.Focus()
                StartProgram.Text = FileName
                SetDirty(StartProgram, True)
            Else
                StartProgramBrowse.Focus()
                DelayValidate(StartProgram)
            End If

        End Sub

        Protected Overrides Function GetF1HelpKeyword() As String
            Return HelpKeywords.FBProjPropDebug
        End Function

        ''' <summary>
        ''' validate a property
        ''' </summary>
        ''' <param name="controlData"></param>
        ''' <param name="message"></param>
        ''' <param name="returnControl"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overrides Function ValidateProperty(ByVal controlData As PropertyControlData, ByRef message As String, ByRef returnControl As Control) As ValidationResult
            Select Case controlData.DispId
                Case VsProjPropId.VBPROJPROPID_StartProgram
                    If rbStartProgram.Checked Then
                        'DO NOT Validate file existance if we are remote debugging, as they are local to the remote machine
                        If Not RemoteDebugEnabled.Checked Then
                            If Not File.Exists(StartProgram.Text) Then
                                message = SR.GetString(SR.PropPage_ProgramNotExist)
                                Return ValidationResult.Warning
                            End If
                        End If
                        If Trim(StartProgram.Text).Length = 0 Then
                            message = SR.GetString(SR.PropPage_NeedExternalProgram)
                            Return ValidationResult.Warning
                        ElseIf Not Path.GetExtension(StartProgram.Text).Equals(".exe", StringComparison.OrdinalIgnoreCase) Then
                            message = SR.GetString(SR.PropPage_NotAnExeError)
                            Return ValidationResult.Warning
                        End If
                    End If
                Case VsProjPropId.VBPROJPROPID_RemoteDebugMachine
                    If RemoteDebugEnabled.Checked AndAlso Len(Trim(RemoteDebugMachine.Text)) = 0 Then
                        message = SR.GetString(SR.PropPage_RemoteMachineBlankError)
                        Return ValidationResult.Warning
                    End If
                Case VsProjPropId.VBPROJPROPID_StartWorkingDirectory
                    'DO NOT Validate working directory if we are remote debugging, as they are local to the remote machine
                    If Not RemoteDebugEnabled.Checked AndAlso Trim(StartWorkingDirectory.Text).Length <> 0 AndAlso Not Directory.Exists(StartWorkingDirectory.Text) Then
                        ' Warn the user when working dir is invalid
                        message = SR.GetString(SR.PropPage_WorkingDirError)
                        Return ValidationResult.Warning
                    End If
            End Select
            Return ValidationResult.Succeeded
        End Function

        ''' <summary>
        ''' Attempts to get the output path for the currently selected configuration.  If there
        '''   are multiple configurations selected, gets the output path for the first one.
        ''' </summary>
        ''' <returns>The output path, relative to the project's folder.</returns>
        ''' <remarks></remarks>
        Private Function GetSelectedConfigOutputPath() As String
            'If there are multiple selected configs, we'll just use the first one
            Dim Properties As PropertyDescriptorCollection = System.ComponentModel.TypeDescriptor.GetProperties(m_Objects(0))
            Dim OutputPathDescriptor As PropertyDescriptor = Properties("OutputPath")
            Return CStr(TryGetNonCommonPropertyValue(OutputPathDescriptor))
        End Function

        ''' <summary>
        ''' Sets the Startup Arguments textbox's height
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub SetStartArgumentsHeight()
            'Set StartArguments text to be approximately four lines high
            '  (it won't necessarily be exact due to GDI/GDI+ differences)
            Const ApproximateDesiredHeightInLines As Integer = 4
            Using g As Drawing.Graphics = Me.CreateGraphics()
                Const ApproximateBorderHeight As Integer = 2 + 1 '+1 for a little extra buffer
                StartArguments.Height = 2 * ApproximateBorderHeight _
                    + CInt(Math.Ceiling(g.MeasureString(" " & New String(CChar(vbLf), ApproximateDesiredHeightInLines - 1) & " ", StartArguments.Font, Integer.MaxValue).Height))
            End Using
        End Sub

        Protected Overrides Sub OnLayout(ByVal levent As System.Windows.Forms.LayoutEventArgs)
            SetStartArgumentsHeight()
            MyBase.OnLayout(levent)
        End Sub

        Private Sub overarchingTableLayoutPanel_Paint(ByVal sender As System.Object, ByVal e As System.Windows.Forms.PaintEventArgs) Handles overarchingTableLayoutPanel.Paint

        End Sub

        Private Sub EnableUnmanagedDebugging_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EnableUnmanagedDebugging.CheckedChanged

        End Sub

        Private Sub StartArguments_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StartArguments.TextChanged

        End Sub
    End Class

    <System.Runtime.InteropServices.GuidAttribute("9CFBEB2A-6824-43e2-BD3B-B112FEBC3772")> _
    <ComVisible(True)> _
    <CLSCompliantAttribute(False)> _
    Public NotInheritable Class FSharpDebugPropPageComClass
        Inherits FSharpPropPageBase

        Protected Overrides ReadOnly Property Title() As String
            Get
                Return SR.GetString(SR.PPG_DebugTitle)
            End Get
        End Property

        Protected Overrides ReadOnly Property ControlType() As System.Type
            Get
                Return GetType(DebugPropPage)
            End Get
        End Property

        Protected Overrides Function CreateControl() As Control
            Return New DebugPropPage
        End Function

    End Class

End Namespace
