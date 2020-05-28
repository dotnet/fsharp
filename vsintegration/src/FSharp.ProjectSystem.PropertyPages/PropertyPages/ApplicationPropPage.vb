' Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

Imports Microsoft.VisualBasic
Imports Microsoft.VisualStudio.Shell.Interop
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.IO
Imports System.ComponentModel.Design
Imports System.Windows.Forms
Imports System.Drawing
Imports Microsoft.VisualStudio.Editors
Imports OLE = Microsoft.VisualStudio.OLE.Interop

Imports Shell = Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.Editors.PropertyPages
Imports System.Runtime.InteropServices
Imports System.ComponentModel
Imports VSLangProj80
Imports VslangProj90
Imports VslangProj100
Imports System.Runtime.Versioning
Imports Microsoft.VisualStudio.FSharp.ProjectSystem
Imports Microsoft.VisualStudio.Shell

Namespace Microsoft.VisualStudio.Editors.PropertyPages

    ''' <summary>
    ''' Not currently used directly (but it's inherited from)
    '''   - see comments in proppage.vb: "Application property pages (VB, C#, J#)"
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    Friend Class ApplicationPropPage
        Inherits ApplicationPropPageBase
        'Inherits UserControl

        Protected Const Const_SubMain As String = "Sub Main"
        Protected Const Const_OutputType As String = "OutputType"

        Friend WithEvents iconTableLayoutPanel As System.Windows.Forms.TableLayoutPanel

        Private m_OutputTypeStringKeys As String()

        Protected Const INDEX_INVALID As Integer = -1
        Protected Const INDEX_WINDOWSAPP As Integer = 0
        Protected Const INDEX_COMMANDLINEAPP As Integer = 1
        Protected Const INDEX_WINDOWSCLASSLIB As Integer = 2
        Protected Const INDEX_LAST As Integer = INDEX_WINDOWSCLASSLIB
        Public Const Const_TargetFrameworkMoniker As String = "TargetFrameworkMoniker"

        Friend WithEvents TargetFramework As System.Windows.Forms.ComboBox
        Friend WithEvents TargetFrameworkLabel As System.Windows.Forms.Label
        Friend WithEvents AssemblyNameLabel As System.Windows.Forms.Label
        Friend WithEvents ResourceLabel As System.Windows.Forms.Label
        Friend WithEvents UseStandardResourceNames As System.Windows.Forms.CheckBox
        Friend WithEvents TargetFSharpCoreVersion As System.Windows.Forms.ComboBox
        Friend WithEvents TargetFSharpCoreVersionLabel As System.Windows.Forms.Label

        Private m_controlGroup As Control()()

#Region " Windows Form Designer generated code "

        Public Sub New()
            MyBase.New()

            'This call is required by the Windows Form Designer.
            InitializeComponent()

            m_OutputTypeStringKeys = New String(INDEX_LAST) {}
            m_OutputTypeStringKeys(INDEX_WINDOWSAPP) = SR.GetString(SR.PPG_WindowsApp)
            m_OutputTypeStringKeys(INDEX_COMMANDLINEAPP) = SR.GetString(SR.PPG_CommandLineApp)
            m_OutputTypeStringKeys(INDEX_WINDOWSCLASSLIB) = SR.GetString(SR.PPG_WindowsClassLib)

            'Add any initialization after the InitializeComponent() call
            AddChangeHandlers()

            'Opt out of page scaling since we're using AutoScaleMode
            PageRequiresScaling = False
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
        Friend WithEvents AssemblyName As System.Windows.Forms.TextBox
        Friend WithEvents OutputType As System.Windows.Forms.ComboBox
        Friend WithEvents OutputTypeLabel As System.Windows.Forms.Label
        Friend WithEvents ResourcesLabel As System.Windows.Forms.Label
        Friend WithEvents ResourcesGroupBox As System.Windows.Forms.GroupBox
        Friend WithEvents Win32ResourceFileBrowse As System.Windows.Forms.Button
        Friend WithEvents Win32ResourceFile As System.Windows.Forms.TextBox
        Friend WithEvents TopHalfLayoutPanel As System.Windows.Forms.TableLayoutPanel

        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ApplicationPropPage))
            Me.TopHalfLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
            Me.AssemblyNameLabel = New System.Windows.Forms.Label()
            Me.ResourcesGroupBox = New System.Windows.Forms.GroupBox()
            Me.Win32ResourceFileBrowse = New System.Windows.Forms.Button()
            Me.ResourcesLabel = New System.Windows.Forms.Label()
            Me.ResourceLabel = New System.Windows.Forms.Label()
            Me.Win32ResourceFile = New System.Windows.Forms.TextBox()
            Me.UseStandardResourceNames = New System.Windows.Forms.CheckBox()
            Me.iconTableLayoutPanel = New System.Windows.Forms.TableLayoutPanel()
            Me.AssemblyName = New System.Windows.Forms.TextBox()
            Me.OutputTypeLabel = New System.Windows.Forms.Label()
            Me.OutputType = New System.Windows.Forms.ComboBox()
            Me.TargetFrameworkLabel = New System.Windows.Forms.Label()
            Me.TargetFramework = New System.Windows.Forms.ComboBox()
            Me.TargetFSharpCoreVersion = New System.Windows.Forms.ComboBox()
            Me.TargetFSharpCoreVersionLabel = New System.Windows.Forms.Label()
            Me.TopHalfLayoutPanel.SuspendLayout()
            Me.ResourcesGroupBox.SuspendLayout()
            Me.SuspendLayout()
            '
            'TopHalfLayoutPanel
            '
            resources.ApplyResources(Me.TopHalfLayoutPanel, "TopHalfLayoutPanel")
            Me.TopHalfLayoutPanel.Controls.Add(Me.AssemblyNameLabel, 0, 0)
            Me.TopHalfLayoutPanel.Controls.Add(Me.ResourcesGroupBox, 0, 8)
            Me.TopHalfLayoutPanel.Controls.Add(Me.AssemblyName, 0, 1)
            Me.TopHalfLayoutPanel.Controls.Add(Me.OutputTypeLabel, 1, 2)
            Me.TopHalfLayoutPanel.Controls.Add(Me.OutputType, 1, 3)
            Me.TopHalfLayoutPanel.Controls.Add(Me.TargetFrameworkLabel, 0, 2)
            Me.TopHalfLayoutPanel.Controls.Add(Me.TargetFramework, 0, 3)
            Me.TopHalfLayoutPanel.Controls.Add(Me.TargetFSharpCoreVersion, 1, 1)
            Me.TopHalfLayoutPanel.Controls.Add(Me.TargetFSharpCoreVersionLabel, 1, 0)
            Me.TopHalfLayoutPanel.Name = "TopHalfLayoutPanel"
            '
            'AssemblyNameLabel
            '
            resources.ApplyResources(Me.AssemblyNameLabel, "AssemblyNameLabel")
            Me.AssemblyNameLabel.Name = "AssemblyNameLabel"
            '
            'ResourcesGroupBox
            '
            resources.ApplyResources(Me.ResourcesGroupBox, "ResourcesGroupBox")
            Me.TopHalfLayoutPanel.SetColumnSpan(Me.ResourcesGroupBox, 2)
            Me.ResourcesGroupBox.Controls.Add(Me.Win32ResourceFileBrowse)
            Me.ResourcesGroupBox.Controls.Add(Me.ResourcesLabel)
            Me.ResourcesGroupBox.Controls.Add(Me.ResourceLabel)
            Me.ResourcesGroupBox.Controls.Add(Me.Win32ResourceFile)
            Me.ResourcesGroupBox.Controls.Add(Me.UseStandardResourceNames)
            Me.ResourcesGroupBox.Controls.Add(Me.iconTableLayoutPanel)
            Me.ResourcesGroupBox.Name = "ResourcesGroupBox"
            Me.ResourcesGroupBox.TabStop = False
            '
            'Win32ResourceFileBrowse
            '
            resources.ApplyResources(Me.Win32ResourceFileBrowse, "Win32ResourceFileBrowse")
            Me.Win32ResourceFileBrowse.Name = "Win32ResourceFileBrowse"
            '
            'ResourcesLabel
            '
            resources.ApplyResources(Me.ResourcesLabel, "ResourcesLabel")
            Me.ResourcesLabel.Name = "ResourcesLabel"
            '
            'ResourceLabel
            '
            resources.ApplyResources(Me.ResourceLabel, "ResourceLabel")
            Me.ResourceLabel.Name = "ResourceLabel"
            '
            'Win32ResourceFile
            '
            resources.ApplyResources(Me.Win32ResourceFile, "Win32ResourceFile")
            Me.Win32ResourceFile.Name = "Win32ResourceFile"
            '
            'UseStandardResourceNames
            '
            resources.ApplyResources(Me.UseStandardResourceNames, "UseStandardResourceNames")
            Me.UseStandardResourceNames.Name = "UseStandardResourceNames"
            '
            'iconTableLayoutPanel
            '
            resources.ApplyResources(Me.iconTableLayoutPanel, "iconTableLayoutPanel")
            Me.iconTableLayoutPanel.Name = "iconTableLayoutPanel"
            '
            'AssemblyName
            '
            resources.ApplyResources(Me.AssemblyName, "AssemblyName")
            Me.AssemblyName.Name = "AssemblyName"
            '
            'OutputTypeLabel
            '
            resources.ApplyResources(Me.OutputTypeLabel, "OutputTypeLabel")
            Me.OutputTypeLabel.Name = "OutputTypeLabel"
            '
            'OutputType
            '
            Me.OutputType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.OutputType.FormattingEnabled = True
            resources.ApplyResources(Me.OutputType, "OutputType")
            Me.OutputType.Name = "OutputType"
            '
            'TargetFrameworkLabel
            '
            resources.ApplyResources(Me.TargetFrameworkLabel, "TargetFrameworkLabel")
            Me.TargetFrameworkLabel.Name = "TargetFrameworkLabel"
            '
            'TargetFramework
            '
            Me.TargetFramework.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.TargetFramework.FormattingEnabled = True
            resources.ApplyResources(Me.TargetFramework, "TargetFramework")
            Me.TargetFramework.Name = "TargetFramework"
            Me.TargetFramework.Sorted = True
            '
            'TargetFSharpCoreVersion
            '
            Me.TargetFSharpCoreVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            Me.TargetFSharpCoreVersion.FormattingEnabled = True
            resources.ApplyResources(Me.TargetFSharpCoreVersion, "TargetFSharpCoreVersion")
            Me.TargetFSharpCoreVersion.Name = "TargetFSharpCoreVersion"
            Me.TargetFSharpCoreVersion.Sorted = True
            '
            'TargetFSharpCoreVersionLabel
            '
            resources.ApplyResources(Me.TargetFSharpCoreVersionLabel, "TargetFSharpCoreVersionLabel")
            Me.TargetFSharpCoreVersionLabel.Name = "TargetFSharpCoreVersionLabel"
            '
            'ApplicationPropPage
            '
            resources.ApplyResources(Me, "$this")
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
            Me.Controls.Add(Me.TopHalfLayoutPanel)
            Me.Name = "ApplicationPropPage"
            Me.TopHalfLayoutPanel.ResumeLayout(False)
            Me.TopHalfLayoutPanel.PerformLayout()
            Me.ResourcesGroupBox.ResumeLayout(False)
            Me.ResourcesGroupBox.PerformLayout()
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
#End Region


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <value></value>
        ''' <remarks></remarks>
        Protected Overrides ReadOnly Property ControlData() As PropertyControlData()
            Get
                If m_ControlData Is Nothing Then
                    m_ControlData = New PropertyControlData() {
                        New PropertyControlData(VsProjPropId.VBPROJPROPID_AssemblyName, "AssemblyName", Me.AssemblyName, New Control() {Me.AssemblyNameLabel}),
                        New PropertyControlData(VsProjPropId.VBPROJPROPID_OutputType, Const_OutputType, Me.OutputType, AddressOf Me.OutputTypeSet, AddressOf Me.OutputTypeGet, ControlDataFlags.UserHandledEvents, New Control() {Me.OutputTypeLabel}),
                        New PropertyControlData(VsProjPropId80.VBPROJPROPID_Win32ResourceFile, "Win32ResourceFile", Me.Win32ResourceFile, AddressOf Me.Win32ResourceSet, AddressOf Me.Win32ResourceGet, ControlDataFlags.None, New Control() {Me.Win32ResourceFileBrowse}),
                        New PropertyControlData(&H10000000, "UseStandardResourceNames", Me.UseStandardResourceNames, AddressOf CheckBoxSet, AddressOf CheckBoxGet),
                        New PropertyControlData(
                            VsProjPropId100.VBPROJPROPID_TargetFrameworkMoniker, Const_TargetFrameworkMoniker,
                            TargetFramework,
                            AddressOf SetTargetFramework, AddressOf GetTargetFramework,
                            ControlDataFlags.ProjectMayBeReloadedDuringPropertySet Or ControlDataFlags.NoOptimisticFileCheckout,
                            New Control() {Me.TargetFrameworkLabel}),
                        New PropertyControlData(
                            VsProjPropId100.VBPROJPROPID_TargetFrameworkMoniker + 100, ProjectFileConstants.TargetFSharpCoreVersion,
                            Me.TargetFSharpCoreVersion,
                            AddressOf SetTargetFSharpCore, AddressOf GetTargetFSharpCore,
                            ControlDataFlags.ProjectMayBeReloadedDuringPropertySet Or ControlDataFlags.NoOptimisticFileCheckout,
                            New Control() {Me.TargetFSharpCoreVersionLabel})
                        }
                End If
                Return m_ControlData
            End Get
        End Property

        Protected Function CheckBoxSet(control As Control, prop As PropertyDescriptor, value As Object) As Boolean
            'If PropertyControlData.IsSpecialValue(value) Then
            '    ' Don't do anything if the value is missing or indeterminate
            '    Return False
            'End If

            If Not TypeOf value Is Boolean Then
                ' Don't do anything if the value isn't of the expected type
                Return False
            End If

            CType(control, CheckBox).Checked = CBool(value)
            Return True
        End Function

        Protected Function CheckBoxGet(control As Control, prop As PropertyDescriptor, ByRef value As Object) As Boolean
            Dim checkBox As CheckBox = CType(control, CheckBox)
            value = checkBox.Checked.ToString()
            Return True
        End Function

        Protected Overrides ReadOnly Property ValidationControlGroups() As Control()()
            Get
                If m_controlGroup Is Nothing Then
                    m_controlGroup = New Control()() {
                        New Control() {Win32ResourceFile, Win32ResourceFileBrowse}
                        }
                End If
                Return m_controlGroup
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="OutputType"></param>
        ''' <remarks></remarks>
        Private Sub PopulateControlSet(ByVal OutputType As VSLangProj.prjOutputType)
            Debug.Assert(m_Objects.Length <= 1, "Multiple project updates not supported")
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="OutputType"></param>
        ''' <remarks></remarks>
        Private Sub EnableControlSet(ByVal OutputType As VSLangProj.prjOutputType)
        End Sub


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="prop"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overridable Function OutputTypeGet(ByVal control As Control, ByVal prop As PropertyDescriptor, ByRef value As Object) As Boolean
            If (Me.OutputType.SelectedIndex <> INDEX_INVALID) Then
                value = Me.OutputType.SelectedIndex
                Return True
            Else
                '// We're indeterminate. Just let the architecture handle it
                Return False
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="prop"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overridable Function OutputTypeSet(ByVal control As Control, ByVal prop As PropertyDescriptor, ByVal value As Object) As Boolean
            If Not (PropertyControlData.IsSpecialValue(value)) Then
                Dim OutputType As VSLangProj.prjOutputType

                OutputType = CType(value, VSLangProj.prjOutputType)
                Me.OutputType.SelectedIndex = OutputType
                PopulateControlSet(OutputType)

                'Populate the target framework combobox
                PopulateTargetFrameworkAssemblies()
            Else
                '// We're indeterminate 
                Me.OutputType.SelectedIndex = INDEX_INVALID

            End If
            Return True
        End Function

        Private Function Win32ResourceFileSupported() As Boolean
            Return Not GetPropertyControlData(VsProjPropId80.VBPROJPROPID_Win32ResourceFile).IsMissing
        End Function

        Function SetIconAndWin32ResourceFile() As Boolean
            If Not IsCurrentProjectDotNetPortable(DTEProject) Then  ' F# Portable projects don't do resources (same with C#)
                EnableControl(Me.Win32ResourceFile, True)
                EnableControl(Me.Win32ResourceFileBrowse, True)
                Return True
            Else
                Return False
            End If

        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="prop"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overridable Function Win32ResourceGet(ByVal control As Control, ByVal prop As PropertyDescriptor, ByRef value As Object) As Boolean
            value = Me.Win32ResourceFile.Text
            Return True
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="prop"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Overridable Function Win32ResourceSet(ByVal control As Control, ByVal prop As PropertyDescriptor, ByVal value As Object) As Boolean
            Me.Win32ResourceFile.Text = CStr(value)
            Return SetIconAndWin32ResourceFile()
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
                Case VsProjPropId80.VBPROJPROPID_Win32ResourceFile
                    'If Trim(Win32ResourceFile.Text).Length = 0 Then
                    '    message = SR.GetString(SR.PropPage_NeedResFile)
                    '    Return ValidationResult.Warning
                    'Else
                    If ((Not (IsNothing(Win32ResourceFile.Text)) AndAlso Win32ResourceFile.Text <> "") AndAlso Not File.Exists(Win32ResourceFile.Text)) Then
                        message = SR.GetString(SR.PropPage_ResourceFileNotExist)
                        Return ValidationResult.Warning
                    End If
            End Select
            Return ValidationResult.Succeeded
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

            Me.OutputType.Items.Clear()
            Me.OutputType.Items.AddRange(m_OutputTypeStringKeys)
            If IsCurrentProjectDotNetPortable(DTEProject) Then
                ' F# Portable projects can only be 'Library', disable selection for this dropdown
                Me.OutputType.Enabled = False
                ' F# Portable projects don't do resources (same with C#)
                Me.Win32ResourceFile.Enabled = False
                Me.Win32ResourceFileBrowse.Enabled = False
            End If

            ' Populate list of possible versions of FSharp.Core
            PopulateAvailableFSharpCoreVersions()
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

            EnableControlSet(CType(GetControlValueNative(Const_OutputType), VSLangProj.prjOutputType))
            Me.UseStandardResourceNames.Enabled = True
        End Sub


        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub OutputType_SelectionChangeCommitted(ByVal sender As Object, ByVal e As System.EventArgs) Handles OutputType.SelectionChangeCommitted
            If m_fInsideInit Then
                Return
            End If

            Dim OutputType As VSLangProj.prjOutputType
            OutputType = CType(GetControlValueNative(Const_OutputType), VSLangProj.prjOutputType)
            Me.EnableControlSet(OutputType)

            SetDirty(VsProjPropId.VBPROJPROPID_OutputType, False)
            SetDirty(True) 'True forces Apply
            If ProjectReloadedDuringCheckout Then
                Return
            End If

            Me.PopulateControlSet(OutputType)

            PopulateTargetFrameworkAssemblies()

            SetDirty(VsProjPropId.VBPROJPROPID_OutputType, False)
            SetDirty(True) 'True forces Apply
            SetIconAndWin32ResourceFile()
        End Sub

        Protected Overrides Function GetF1HelpKeyword() As String

            Return Common.HelpKeywords.FSProjPropApplication
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub Win32ResourceFileBrowse_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Win32ResourceFileBrowse.Click

            SkipValidating(Win32ResourceFile)   ' skip this because we will pop up dialog to edit it...
            ProcessDelayValidationQueue(False)

            Dim sInitialDirectory As String = Nothing
            Dim sFileName As String

            If sInitialDirectory = "" Then
                sFileName = ""
                sInitialDirectory = ""
            Else
                sFileName = System.IO.Path.GetFileName(sInitialDirectory)
                sInitialDirectory = System.IO.Path.GetDirectoryName(sInitialDirectory)
            End If

            Dim fileNames As ArrayList = Common.Utils.GetFilesViaBrowse(ServiceProvider, Me.Handle, sInitialDirectory, SR.GetString(SR.PPG_AddWin32ResourceTitle),
                    Common.CombineDialogFilters(
                        Common.CreateDialogFilter(SR.GetString(SR.PPG_AddWin32ResourceFilter), "res"),
                        Common.Utils.GetAllFilesDialogFilter()
                        ),
                        0, False, sFileName)
            If fileNames IsNot Nothing AndAlso fileNames.Count = 1 Then
                sFileName = CStr(fileNames(0))
                If System.IO.File.Exists(sFileName) Then
                    Me.Win32ResourceFile.Text = sFileName
                    SetDirty(Win32ResourceFile, True)
                Else
                    DelayValidate(Win32ResourceFile)
                End If
            Else
                DelayValidate(Win32ResourceFile)
            End If
        End Sub

        ''' <summary>
        ''' Set the drop-down width of comboboxes with user-handled events so they'll fit their contents
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Private Sub ComboBoxes_DropDown(ByVal sender As Object, ByVal e As EventArgs) Handles OutputType.DropDown
            Common.SetComboBoxDropdownWidth(DirectCast(sender, ComboBox))
        End Sub

#Region "Target framework assembly"

        ''' <summary>
        ''' Fill up the allowed values in the target framework listbox
        ''' </summary>
        ''' <remarks></remarks>
        Private Function ValidateTargetFrameworkMoniker(ByVal moniker As String) As Boolean
            If String.IsNullOrWhiteSpace(moniker) Then
                Return False
            ElseIf moniker.StartsWith(".NETCoreApp", StringComparison.OrdinalIgnoreCase) Then
                If Me.OutputType.SelectedIndex <> INDEX_INVALID Then    ' NetCore always include
                    ' .NET Core and .NETStandard don't need redists to be installed.
                    Return True
                End If
            ElseIf moniker.StartsWith(".NETStandard", StringComparison.OrdinalIgnoreCase) Then
                If Me.OutputType.SelectedIndex = 2 Then                 ' NetStandard ClassLibrary only
                    Return True
                End If
            ElseIf moniker.Contains("v2") OrElse moniker.Contains("v3.0") OrElse moniker.Contains("v3.5") OrElse moniker.Contains("v4") Then
                ' With the latest tooling, if we have editors the redist is installed by definition
                Return True
            End If
            Return False
        End Function

        Friend Shared Function IsCurrentProjectDotNetPortable(ByVal dteProject As EnvDTE.Project) As Boolean
            Dim currentFrameworkName As FrameworkName = GetCurrentFrameworkName(dteProject)
            Return IsDotNetPortable(currentFrameworkName)
        End Function

        Private Shared Function GetCurrentFrameworkName(ByVal dteProject As EnvDTE.Project) As FrameworkName
            Dim targetFrameworkMonikerProperty As EnvDTE.Property = dteProject.Properties.Item(ApplicationPropPage.Const_TargetFrameworkMoniker)
            Dim currentTargetFrameworkMoniker As String = CStr(targetFrameworkMonikerProperty.Value)
            Return New FrameworkName(currentTargetFrameworkMoniker)
        End Function

        Private Shared Function IsDotNetPortable(ByVal frameworName As FrameworkName) As Boolean
            Return frameworName.Identifier = ".NETPortable"
        End Function

        Private Shared Function IsDotNetCore(ByVal frameworName As FrameworkName) As Boolean
            Return frameworName.Identifier = ".NETCore"
        End Function

        Private Function GetServiceProvider() As IServiceProvider
            Dim siteServiceProvider As Microsoft.VisualStudio.OLE.Interop.IServiceProvider = Nothing
            VSErrorHandler.ThrowOnFailure(MyBase.ProjectHierarchy.GetSite(siteServiceProvider))

            Return New Microsoft.VisualStudio.Shell.ServiceProvider(siteServiceProvider)
        End Function


        Private Sub PopulateAvailableFSharpCoreVersions()
            TargetFSharpCoreVersion.Items.Clear()
            TargetFSharpCoreVersion.SelectedIndex = INDEX_INVALID

            Dim currentFrameworkName As FrameworkName = GetCurrentFrameworkName(DTEProject)
            Dim siteServiceProvider As Microsoft.VisualStudio.OLE.Interop.IServiceProvider = Nothing
            VSErrorHandler.ThrowOnFailure(MyBase.ProjectHierarchy.GetSite(siteServiceProvider))

            Dim sp As System.IServiceProvider = GetServiceProvider()

            Dim canUseTargetFSharpCoreVersionProperty As Boolean = CType(DTEProject.Properties.Item(ProjectSystemConstants.CanUseTargetFSharpCoreVersion).Value, Boolean)

            If canUseTargetFSharpCoreVersionProperty Then
                Try
                    Dim fsharpVersionLookup As IFSharpCoreVersionLookupService = CType(sp.GetService(GetType(IFSharpCoreVersionLookupService)), IFSharpCoreVersionLookupService)
                    For Each fsharpCoreVersion As FSharpCoreVersion In fsharpVersionLookup.ListAvailableFSharpCoreVersions(currentFrameworkName)
                        TargetFSharpCoreVersion.Items.Add(fsharpCoreVersion)
                    Next
                    TargetFSharpCoreVersion.Enabled = True
                Catch ex As Exception
                    TargetFSharpCoreVersion.Items.Clear()
                    TargetFSharpCoreVersion.Enabled = False
                End Try
            Else
                Dim targetVersionString As String = CType(DTEProject.Properties.Item(ProjectFileConstants.TargetFSharpCoreVersion).Value, String)
                Dim description As String = If(targetVersionString Is Nothing, My.Resources.Designer.PPG_NotApplicable, targetVersionString)
                Dim versionObj As New FSharpCoreVersion(targetVersionString, description)
                TargetFSharpCoreVersion.Items.Add(versionObj)
                TargetFSharpCoreVersion.SelectedIndex = 0
                TargetFSharpCoreVersion.Enabled = False
            End If


        End Sub

        Private Sub PopulateTargetFrameworkAssemblies()
            Dim targetFrameworkSupported As Boolean = False
            Dim selectedItem As String = Nothing
            If Me.TargetFramework.SelectedIndex <> INDEX_INVALID Then
                selectedItem = Me.TargetFramework.Text
            End If
            Me.TargetFramework.Items.Clear()
            Me.TargetFramework.SelectedIndex = INDEX_INVALID

            Try
                Dim sp As System.IServiceProvider = GetServiceProvider()

                Dim vsFrameworkMultiTargeting As IVsFrameworkMultiTargeting = TryCast(sp.GetService(GetType(SVsFrameworkMultiTargeting)), IVsFrameworkMultiTargeting)
                Dim slnSvc As IVsSolution = TryCast(sp.GetService(GetType(SVsSolution)), IVsSolution)

                Dim hier As IVsHierarchy = Nothing
                Dim isSdkProject = False
                If slnSvc.GetProjectOfUniqueName(DTEProject.UniqueName, hier) = 0 Then
                    isSdkProject = hier.IsCapabilityMatch("CPS")
                End If

                If vsFrameworkMultiTargeting IsNot Nothing Then
                    Dim currentFrameworkName As FrameworkName = GetCurrentFrameworkName(DTEProject)
                    Dim isPortable As Boolean = IsDotNetPortable(currentFrameworkName)
                    targetFrameworkSupported = True

                    Dim supportedTargetFrameworksDescriptor As PropertyDescriptor = GetPropertyDescriptor("SupportedTargetFrameworks")
                    Dim supportedFrameworks As IEnumerable(Of TargetFrameworkMoniker) = TargetFrameworkMoniker.GetSupportedTargetFrameworkMonikers(vsFrameworkMultiTargeting, DTEProject, isSdkProject, supportedTargetFrameworksDescriptor)

                    For Each supportedFramework As TargetFrameworkMoniker In supportedFrameworks
                        If Me.ValidateTargetFrameworkMoniker(supportedFramework.Moniker) Then
                            Dim candidate As FrameworkName = New FrameworkName(supportedFramework.Moniker)
                            ' For portables add profile of current project
                            If Not isPortable OrElse candidate.Profile = currentFrameworkName.Profile Then
                                Me.TargetFramework.Items.Add(supportedFramework)
                            End If
                        End If
                    Next

                End If

                ' Put back previous value
                If Not IsNothing(selectedItem) Then
                    Me.TargetFramework.Text = selectedItem
                End If

            Catch ex As Exception
                targetFrameworkSupported = False
                Me.TargetFramework.Items.Clear()
            End Try

            If Not targetFrameworkSupported Then
                Me.TargetFramework.Enabled = False
            End If
        End Sub

        Private Function SetTargetFSharpCore(ByVal control As Control, ByVal prop As PropertyDescriptor, ByVal value As Object) As Boolean
            Dim combobox As ComboBox = CType(control, ComboBox)
            combobox.SelectedIndex = INDEX_INVALID
            If PropertyControlData.IsSpecialValue(value) Then 'Indeterminate or IsMissing
                'Leave it unselected
            Else
                Dim stringValue As String = DirectCast(value, String)
                For Each entry As FSharpCoreVersion In combobox.Items
                    If entry.Version = stringValue Then
                        combobox.SelectedItem = entry
                        Exit For
                    End If
                Next
            End If

            Return True
        End Function

        Private Function GetTargetFSharpCore(ByVal control As Control, ByVal prop As PropertyDescriptor, ByRef value As Object) As Boolean
            Dim currentVersion As FSharpCoreVersion = CType(CType(control, ComboBox).SelectedItem, FSharpCoreVersion)
            If currentVersion IsNot Nothing Then
                value = currentVersion.Version
                Return True
            End If

            Debug.Fail("The combobox should not have still been unselected yet be dirty")
            Return False
        End Function

        ''' <summary>
        ''' Takes the current value of the TargetFramework property (in UInt32 format), and sets
        '''   the current dropdown list to that value.
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="prop"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Private Function SetTargetFramework(ByVal control As Control, ByVal prop As PropertyDescriptor, ByVal value As Object) As Boolean
            Dim combobox As ComboBox = CType(control, ComboBox)
            combobox.SelectedIndex = INDEX_INVALID
            If PropertyControlData.IsSpecialValue(value) Then 'Indeterminate or IsMissing
                'Leave it unselected
            Else
                Dim stringValue As String = DirectCast(value, String)
                For Each entry As TargetFrameworkMoniker In combobox.Items
                    If entry.Moniker = stringValue Then
                        combobox.SelectedItem = entry
                        Exit For
                    End If
                Next
            End If

            Return True
        End Function


        ''' <summary>
        ''' Retrieves the current value of the TargetFramework dropdown text and converts it into
        '''   the native property type of UInt32 so it can be stored into the project's property.
        '''   Called by the base class code.
        ''' </summary>
        ''' <param name="control"></param>
        ''' <param name="prop"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Private Function GetTargetFramework(ByVal control As Control, ByVal prop As PropertyDescriptor, ByRef value As Object) As Boolean
            Dim currentTarget As TargetFrameworkMoniker = CType(CType(control, ComboBox).SelectedItem, TargetFrameworkMoniker)
            If currentTarget IsNot Nothing Then
                value = currentTarget.Moniker
                Return True
            End If

            Debug.Fail("The combobox should not have still been unselected yet be dirty")
            Return False
        End Function

        Private Sub Win32ResourceFile_TextChanged(sender As Object, e As EventArgs) Handles Win32ResourceFile.TextChanged

        End Sub


#End Region

    End Class


End Namespace
