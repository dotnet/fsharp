' Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

Namespace Microsoft.VisualStudio.Editors.MyApplication

    Public Class MyApplicationPropertiesBase
        ''' <summary>
        ''' Returns the set of files that need to be checked out to change the given property
        ''' Must be overridden in sub-class
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overridable Function FilesToCheckOut(ByVal CreateIfNonexistent As Boolean) As String()
            Return New String() {}
        End Function


    End Class ' Class MyApplicationPropertiesBase

End Namespace
