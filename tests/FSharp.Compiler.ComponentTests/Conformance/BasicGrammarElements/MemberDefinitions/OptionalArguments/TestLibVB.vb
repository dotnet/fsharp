Imports System
Namespace Test
    Public Class VBTestClass
        Public Function OptionalRefParam(Optional ByRef param2 As String = "superduper") As String
            param2 = param2 + " edited"
            Return param2
        End Function

        Public Function OptionalNullableRefParam(Optional ByRef param2 As System.Nullable(Of Int32) = 30) As Nullable(Of Int32)
            If param2.HasValue Then
                param2 = param2 + 100
                Return param2
            Else
                param2 = 55
                Return param2
            End If
        End Function
    End Class
End Namespace
