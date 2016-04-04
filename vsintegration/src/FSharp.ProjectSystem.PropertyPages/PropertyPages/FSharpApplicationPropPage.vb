' Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.ComponentModel.Design
Imports System.Windows.Forms
Imports System.Drawing
Imports Microsoft.VisualStudio.Editors
Imports Microsoft.VisualStudio.Editors.PropertyPages

Imports System.Runtime.InteropServices
Imports System.ComponentModel

Namespace Microsoft.VisualStudio.Editors.PropertyPages

    ''' <summary>
    ''' C#/J# application property page - see comments in proppage.vb: "Application property pages (VB, C#, J#)"
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
    Friend Class FSharpApplicationPropPage
        'Inherits System.Windows.Forms.UserControl
        ' If you want to be able to use the forms designer to edit this file,
        ' change the base class from PropPageUserControlBase to UserControl
        Inherits ApplicationPropPage


#Region " Windows Form Designer generated code "

        Public Sub New()
            MyBase.New()

            'This call is required by the Windows Form Designer.
            InitializeComponent()

            'Add any initialization after the InitializeComponent() call
            AddChangeHandlers()
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
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
            Me.SuspendLayout()
            '
            'FSharpApplicationPropPage
            '
            Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
            Me.Name = "FSharpApplicationPropPage"
            Me.Size = New System.Drawing.Size(518, 380)
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub
#End Region


        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub
    End Class

    Public MustInherit Class FSharpPropPageBase
        Inherits PropPageBase

        Protected Sub New()
            'The follow entry is a dummy value - without something stuffed in here the
            '  property page will NOT show the help button. The F1 keyword is the real 
            '  help context
            MyBase.New()
            Me.HelpContext = 1
            Me.HelpFile = "VBREF.CHM"
        End Sub

    End Class

    <System.Runtime.InteropServices.GuidAttribute("6D2D9B56-2691-4624-A1BF-D07A14594748")> _
    <ComVisible(True)> _
    <CLSCompliantAttribute(False)> _
    Public NotInheritable Class FSharpApplicationPropPageComClass 'See class hierarchy comments above
        Inherits FSharpPropPageBase

        Protected Overrides ReadOnly Property Title() As String
            Get
                Return SR.GetString(SR.PPG_ApplicationTitle)
            End Get
        End Property

        Protected Overrides ReadOnly Property ControlType() As System.Type
            Get
                Return GetType(FSharpApplicationPropPage)
            End Get
        End Property

        Protected Overrides Function CreateControl() As Control
            Return New FSharpApplicationPropPage
        End Function

    End Class


End Namespace
