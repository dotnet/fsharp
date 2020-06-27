' Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

Imports EnvDTE
Imports Microsoft.VisualStudio.Shell.Interop
Imports System
Imports System.Collections.Generic
Imports System.Runtime.Versioning
Imports System.ComponentModel

Namespace Microsoft.VisualStudio.Editors.PropertyPages

    ''' <summary>
    ''' Represents a target framework moniker and can be placed into a control
    ''' </summary>
    Class TargetFrameworkMoniker

        ''' <summary>
        ''' Stores the target framework moniker
        ''' </summary>
        Private m_Moniker As String

        ''' <summary>
        ''' Stores the display name of the target framework moniker
        ''' </summary>
        Private m_DisplayName As String

        ''' <summary>
        ''' Constructor that uses the target framework moniker and display name provided by DTAR
        ''' </summary>
        Public Sub New(ByVal moniker As String, ByVal displayName As String)

            m_Moniker = moniker
            m_DisplayName = displayName

        End Sub

        ''' <summary>
        ''' Gets the target framework moniker
        ''' </summary>
        Public ReadOnly Property Moniker() As String
            Get
                Return m_Moniker
            End Get
        End Property

        ''' <summary>
        ''' Use the display name provided by DTAR for the string display
        ''' </summary>
        Public Overrides Function ToString() As String
            Return m_DisplayName
        End Function

        Private Shared Function AddDotNetCoreFramework(prgSupportedFrameworks As Array, supportedTargetFrameworksDescriptor As PropertyDescriptor) As Array
            Dim _TypeConverter As TypeConverter = supportedTargetFrameworksDescriptor.Converter
            If _TypeConverter IsNot Nothing Then
                Dim supportedFrameworksList As List(Of String) = New List(Of String)
                For Each moniker As String In prgSupportedFrameworks
                    supportedFrameworksList.Add(moniker)
                Next

                For Each frameworkValue As Object In _TypeConverter.GetStandardValues()
                    Dim framework As String = CStr(frameworkValue)
                    If framework IsNot Nothing Then
                        supportedFrameworksList.Add(framework)
                    End If
                Next

                Return supportedFrameworksList.ToArray
            End If

            Return prgSupportedFrameworks
        End Function


        ''' <summary>
        ''' Gets the supported target framework monikers from DTAR
        ''' </summary>
        ''' <param name="vsFrameworkMultiTargeting"></param>
        Public Shared Function GetSupportedTargetFrameworkMonikers(ByVal vsFrameworkMultiTargeting As IVsFrameworkMultiTargeting,
                                                                   ByVal currentProject As Project,
                                                                   isSdkProject As Boolean,
                                                                   ByVal supportedTargetFrameworksDescriptor As PropertyDescriptor) As IEnumerable(Of TargetFrameworkMoniker)

            Dim supportedFrameworksArray As Array = Nothing
            VSErrorHandler.ThrowOnFailure(vsFrameworkMultiTargeting.GetSupportedFrameworks(supportedFrameworksArray))
            If supportedTargetFrameworksDescriptor IsNot Nothing Then
                supportedFrameworksArray = AddDotNetCoreFramework(supportedFrameworksArray, supportedTargetFrameworksDescriptor)
            End If

            Dim targetFrameworkMonikerProperty As [Property] = currentProject.Properties.Item(ApplicationPropPage.Const_TargetFrameworkMoniker)
            Dim currentTargetFrameworkMoniker As String = CStr(targetFrameworkMonikerProperty.Value)
            Dim currentFrameworkName As New FrameworkName(currentTargetFrameworkMoniker)

            Dim supportedTargetFrameworkMonikers As New List(Of TargetFrameworkMoniker)
            Dim hashSupportedTargetFrameworkMonikers As New HashSet(Of String)

            ' UNDONE: DTAR may currently send back duplicate monikers, so explicitly filter them out for now
            For Each moniker As String In supportedFrameworksArray
                If hashSupportedTargetFrameworkMonikers.Add(moniker) Then

                    ' Filter out frameworks with a different identifier since they are not applicable to the current project type
                    Dim newFrameworkName As FrameworkName = New FrameworkName(moniker)
                    Dim displayName As String = ""

                    If isSdkProject Then
                        If String.Compare(newFrameworkName.Identifier, ".NETStandard", StringComparison.Ordinal) = 0 OrElse
                           String.Compare(newFrameworkName.Identifier, ".NETCoreApp", StringComparison.Ordinal) = 0 Then
                            displayName = CStr(supportedTargetFrameworksDescriptor.Converter?.ConvertTo(moniker, GetType(String)))
                            supportedTargetFrameworkMonikers.Add(New TargetFrameworkMoniker(moniker, displayName))
                        Else
                            If String.Compare(newFrameworkName.Identifier, ".NETFramework", StringComparison.OrdinalIgnoreCase) = 0 And newFrameworkName.Version >= New Version(4, 5, 0, 0) Then
                                ' Use DTAR to get the display name corresponding to the moniker
                                VSErrorHandler.ThrowOnFailure(vsFrameworkMultiTargeting.GetDisplayNameForTargetFx(moniker, displayName))
                                supportedTargetFrameworkMonikers.Add(New TargetFrameworkMoniker(moniker, displayName))
                            End If
                        End If
                    Else
                        If String.Compare(newFrameworkName.Identifier, currentFrameworkName.Identifier, StringComparison.OrdinalIgnoreCase) = 0 Then
                            ' Use DTAR to get the display name corresponding to the moniker
                            VSErrorHandler.ThrowOnFailure(vsFrameworkMultiTargeting.GetDisplayNameForTargetFx(moniker, displayName))
                            supportedTargetFrameworkMonikers.Add(New TargetFrameworkMoniker(moniker, displayName))
                        End If
                    End If
                End If
            Next
            Return supportedTargetFrameworkMonikers

        End Function
    End Class

End Namespace
