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
        Implements IComparable(Of TargetFrameworkMoniker)

        Public Function CompareTo(other As TargetFrameworkMoniker) As Integer Implements IComparable(Of TargetFrameworkMoniker).CompareTo
            Return String.Compare(other.DisplayOrder, Me.DisplayOrder)
        End Function

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

        Public ReadOnly Property NormalizedTFM() As String
            Get
                Dim p = GetFrameworkProfileFromMoniker(Me.Moniker)
                If String.IsNullOrWhiteSpace(p) Then
                    Return String.Format("{0},Version=v{1}", GetFrameworkIdFromMoniker(Me.Moniker), GetFrameworkVersionFromMoniker(Me.Moniker))
                Else
                    Return String.Format("{0},Version=v{1}-{2}", GetFrameworkIdFromMoniker(Me.Moniker), GetFrameworkVersionFromMoniker(Me.Moniker), p)
                End If
            End Get
        End Property

        Public ReadOnly Property DisplayName() As String
            Get
                Return m_DisplayName
            End Get
        End Property

        Public ReadOnly Property DisplayOrder() As String
            Get
                Dim tfm As String = DisplayName
                If tfm.StartsWith(".NET Core", StringComparison.OrdinalIgnoreCase) Then
                    Return "80" + m_DisplayName
                ElseIf tfm.StartsWith(".NET Standard", StringComparison.OrdinalIgnoreCase) Then
                    Return "70" + m_DisplayName
                ElseIf tfm.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase) Then
                    Return "60" + m_DisplayName
                ElseIf tfm.StartsWith(".NET ", StringComparison.OrdinalIgnoreCase) Then
                    Return "90" + m_DisplayName
                Else
                    Return "00" + m_DisplayName
                End If
            End Get
        End Property

        ''' <summary>
        ''' Use the display name provided by DTAR for the string display
        ''' </summary>
        Public Overrides Function ToString() As String
            Return DisplayName
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

        Public Shared Function GetDisplayIdFromMoniker(moniker As String) As String
            If moniker.StartsWith(".NETStandard", StringComparison.OrdinalIgnoreCase) Or
               moniker.StartsWith("netstandard", StringComparison.OrdinalIgnoreCase) Then
                Return ".NET Standard"
            ElseIf moniker.StartsWith(".NETCoreApp", StringComparison.OrdinalIgnoreCase) Or
                   moniker.StartsWith(".NETCore", StringComparison.OrdinalIgnoreCase) Or
                   moniker.StartsWith("netcoreapp", StringComparison.OrdinalIgnoreCase) Then
                Return ".NET Core"
            ElseIf moniker.StartsWith(".NETFramework", StringComparison.OrdinalIgnoreCase) Then
                Return ".NET Framework"
            ElseIf moniker.StartsWith(".NET ", StringComparison.OrdinalIgnoreCase) Or
                   moniker.StartsWith("net", StringComparison.OrdinalIgnoreCase) Then
                Return ".NET"
            End If
        End Function

        Public Shared Function GetFrameworkIdFromMoniker(moniker As String) As String
            If moniker.StartsWith(".NETStandard", StringComparison.OrdinalIgnoreCase) Or
               moniker.StartsWith("netstandard", StringComparison.OrdinalIgnoreCase) Then
                Return ".NETStandard"
            ElseIf moniker.StartsWith(".NETCore", StringComparison.OrdinalIgnoreCase) Then
                Return ".NETCore"
            ElseIf moniker.StartsWith(".NETCoreApp", StringComparison.OrdinalIgnoreCase) Or
                   moniker.StartsWith("netcoreapp", StringComparison.OrdinalIgnoreCase) Or
                   (moniker.StartsWith("net", StringComparison.OrdinalIgnoreCase) And moniker.Contains(".")) Then
                Return ".NETCoreApp"
            ElseIf moniker.StartsWith(".NETFramework", StringComparison.OrdinalIgnoreCase) Then
                Return ".NETFramework"
            ElseIf moniker.StartsWith(".NET ", StringComparison.OrdinalIgnoreCase) Or
                   (moniker.StartsWith("net", StringComparison.OrdinalIgnoreCase) And Not moniker.Contains(".")) Then
                Return ".NET"
            End If
        End Function

        Public Shared Function GetFrameworkVersionFromMoniker(moniker As String) As String
            If moniker.StartsWith(".NETStandard", StringComparison.OrdinalIgnoreCase) Or
               moniker.StartsWith(".NETCoreApp", StringComparison.OrdinalIgnoreCase) Or
               moniker.StartsWith(".NETCore", StringComparison.OrdinalIgnoreCase) Or
               moniker.StartsWith(".NETFramework", StringComparison.OrdinalIgnoreCase) Or
               moniker.StartsWith(".NET ", StringComparison.OrdinalIgnoreCase) Then
                Dim fw As FrameworkName = New FrameworkName(moniker)
                Return fw.Version.ToString()
            ElseIf moniker.StartsWith("netcoreapp", StringComparison.OrdinalIgnoreCase) Then
                Dim v As String = moniker.Substring(10)
                Return New Version(v).ToString()
            ElseIf moniker.StartsWith("netstandard", StringComparison.OrdinalIgnoreCase) Then
                Dim v As String = moniker.Substring(11)
                Return New Version(v).ToString()
            ElseIf moniker.StartsWith("net", StringComparison.OrdinalIgnoreCase) Then
                Dim v As String = moniker.Substring(3)
                Return New Version(v).ToString()
            End If
        End Function

        Public Shared Function GetFrameworkProfileFromMoniker(moniker As String) As String
            If moniker.StartsWith(".NETStandard", StringComparison.OrdinalIgnoreCase) Or
               moniker.StartsWith(".NETCoreApp", StringComparison.OrdinalIgnoreCase) Or
               moniker.StartsWith(".NETCore", StringComparison.OrdinalIgnoreCase) Or
               moniker.StartsWith(".NETFramework", StringComparison.OrdinalIgnoreCase) Or
               moniker.StartsWith(".NET ", StringComparison.OrdinalIgnoreCase) Then
                Dim fw As FrameworkName = New FrameworkName(moniker)
                Return fw.Profile.ToString()
            Else
                Return ""
            End If
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
            Dim currentFrameworkId = GetDisplayIdFromMoniker(currentTargetFrameworkMoniker)

            Dim supportedTargetFrameworkMonikers As New List(Of TargetFrameworkMoniker)
            Dim hashSupportedTargetFrameworkMonikers As New HashSet(Of String)

            ' UNDONE: DTAR may currently send back duplicate monikers, so explicitly filter them out for now
            For Each moniker As String In supportedFrameworksArray
                If hashSupportedTargetFrameworkMonikers.Add(moniker) Then
                    ' Filter out frameworks with a different identifier since they are not applicable to the current project type
                    If isSdkProject Then
                        Dim displayName As String = CStr(supportedTargetFrameworksDescriptor.Converter?.ConvertTo(moniker, GetType(String)))
                        If moniker.StartsWith(".NETStandard", StringComparison.OrdinalIgnoreCase) OrElse
                           moniker.StartsWith(".NETCoreApp", StringComparison.OrdinalIgnoreCase) OrElse
                           moniker.StartsWith(".NETCore", StringComparison.OrdinalIgnoreCase) OrElse
                           moniker.StartsWith(".NETFramework", StringComparison.OrdinalIgnoreCase) OrElse
                           moniker.StartsWith(".NET ", StringComparison.OrdinalIgnoreCase) Then
                            Dim fw As FrameworkName = New FrameworkName(moniker)
                            If Not (moniker.StartsWith(".NETCore", StringComparison.OrdinalIgnoreCase) And fw.Version <= New Version("4.5")) Then
                                displayName = String.Format("{0} {1} {2}", GetDisplayIdFromMoniker(moniker), GetFrameworkVersionFromMoniker(moniker), GetFrameworkProfileFromMoniker(moniker))
                                supportedTargetFrameworkMonikers.Add(New TargetFrameworkMoniker(moniker, displayName))
                            End If
                        ElseIf moniker.StartsWith("netcoreapp", StringComparison.OrdinalIgnoreCase) Then
                            displayName = String.Format("{0} {1} {2}", GetDisplayIdFromMoniker(moniker), GetFrameworkVersionFromMoniker(moniker), GetFrameworkProfileFromMoniker(moniker))
                            supportedTargetFrameworkMonikers.Add(New TargetFrameworkMoniker(moniker, displayName))
                        ElseIf moniker.StartsWith("netstandard", StringComparison.OrdinalIgnoreCase) Then
                            displayName = String.Format("{0} {1} {2}", GetDisplayIdFromMoniker(moniker), GetFrameworkVersionFromMoniker(moniker), GetFrameworkProfileFromMoniker(moniker))
                            supportedTargetFrameworkMonikers.Add(New TargetFrameworkMoniker(moniker, displayName))
                        ElseIf moniker.StartsWith("net", StringComparison.OrdinalIgnoreCase) Then
                            displayName = String.Format("{0} {1} {2}", GetDisplayIdFromMoniker(moniker), GetFrameworkVersionFromMoniker(moniker), GetFrameworkProfileFromMoniker(moniker))
                            supportedTargetFrameworkMonikers.Add(New TargetFrameworkMoniker(moniker, displayName))
                        Else
                            If currentFrameworkId = GetDisplayIdFromMoniker(moniker) Then
                                ' Use DTAR to get the display name corresponding to the moniker
                                displayName = String.Format("{0} {1} {2}", GetDisplayIdFromMoniker(moniker), GetFrameworkVersionFromMoniker(moniker), GetFrameworkProfileFromMoniker(moniker))
                                supportedTargetFrameworkMonikers.Add(New TargetFrameworkMoniker(moniker, displayName))
                            End If
                        End If
                    End If
                End If
            Next
            Return supportedTargetFrameworkMonikers

        End Function
    End Class

End Namespace
