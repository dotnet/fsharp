' Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System
Imports System.ComponentModel.Design
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.Editors.Interop
Imports System.Diagnostics


Namespace Microsoft.VisualStudio.Editors

    Public Interface IVBPackage


        Function GetLastShownApplicationDesignerTab(ByVal projectHierarchy As IVsHierarchy) As Integer

        Sub SetLastShownApplicationDesignerTab(ByVal projectHierarchy As IVsHierarchy, ByVal tab As Integer)

        ReadOnly Property GetService(ByVal serviceType As Type) As Object

        ReadOnly Property MenuCommandService() As IMenuCommandService

    End Interface


    Public Class VBPackageUtils

        Private Shared m_editorsPackage As IVBPackage
        Public Delegate Function getServiceDelegate(ByVal ServiceType As Type) As Object
        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="GetService"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property PackageInstance(ByVal GetService As getServiceDelegate) As IVBPackage
            Get
                If m_editorsPackage Is Nothing Then
                    Dim shell As IVsShell = DirectCast(GetService(GetType(IVsShell)), IVsShell)
                    Dim pPackage As IVsPackage = Nothing
                    If shell IsNot Nothing Then
                        Dim hr As Integer = shell.IsPackageLoaded(New Guid("67909B06-91E9-4F3E-AB50-495046BE9A9A"), pPackage)
                        Debug.Assert(NativeMethods.Succeeded(hr) AndAlso pPackage IsNot Nothing, "VB editors package not loaded?!?")
                    End If

                    m_editorsPackage = TryCast(pPackage, IVBPackage)
                End If
                Return m_editorsPackage
            End Get
        End Property
    End Class
End Namespace
