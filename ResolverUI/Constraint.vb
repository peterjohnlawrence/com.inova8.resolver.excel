Public Class Constraint

    Public reference As Excel.Range
    Public equality As String
    Public constraintreference As Excel.Range

    Public Property value(ByVal globalAddress As Boolean) As String
        Get
            Return reference.AddressLocal(, , , globalAddress) + equality + constraintreference.AddressLocal(, , , globalAddress)
        End Get
        Set(ByVal value As String)

        End Set
    End Property
    Public Property hasArray() As Boolean
        Get
            Return (reference.Cells.Count > 1) Or (constraintreference.Cells.Count > 1)
        End Get
        Set(ByVal value As Boolean)

        End Set
    End Property
    Public Property validate() As Boolean
        Get
            Dim index As Long
            validate = (reference.Cells.Count = constraintreference.Cells.Count)
            For index = 1 To reference.Cells.Count
                If (TypeName(reference.Cells(index).value) = "Double" _
                Or TypeName(reference.Cells(index).value) = "Single" _
                Or TypeName(reference.Cells(index).value) = "Integer" _
                Or TypeName(reference.Cells(index).value) = "Long") _
                Then
                    validate = validate And True
                Else
                    validate = False
                End If
            Next index
            For index = 1 To reference.Cells.Count
                If (TypeName(constraintreference.Cells(index).value) = "Double" _
                Or TypeName(constraintreference.Cells(index).value) = "Single" _
                Or TypeName(constraintreference.Cells(index).value) = "Integer" _
                Or TypeName(constraintreference.Cells(index).value) = "Long") Then
                    validate = validate And True
                Else
                    validate = False
                End If
            Next index
            Return validate
        End Get
        Set(ByVal value As Boolean)

        End Set
    End Property



End Class
