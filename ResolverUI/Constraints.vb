Public Class Constraints

    Private col As Collection
    Public Sub New()
        col = New Collection
    End Sub

    Public Sub Add(ByVal Item As Constraint)
        col.Add(Item, Item.value(False))
    End Sub

    Public Sub remove(ByVal index As Object)
        col.Remove(index)
    End Sub
    Public Function count()
        count = col.Count
    End Function
    Public Property Collection() As Collection
        Get
            Return col
        End Get
        Set(ByVal value As Collection)

        End Set
    End Property
    Public Function load(ByVal constraintRange As Excel.Range) As Boolean
        Dim equalities() As String = {"<=", ">=", "="}
        'equalities = Array()
        Dim index As Long
        Dim pos As Long
        Dim rhs As String
        Dim newConstraint As Constraint
        If constraintRange.Errors(1).Value = False Then
            For index = LBound(equalities) To UBound(equalities)
                pos = InStr(2, constraintRange.Formula, equalities(index))
                If pos > 0 Then
                    newConstraint = New Constraint
                    newConstraint.equality = equalities(index)
                    newConstraint.reference = xlsApp.Range(Mid(constraintRange.Formula, 2, pos - 2))
                    rhs = Mid(constraintRange.Formula, pos + Len(equalities(index)))
                    If IsNumeric(rhs) Then
                        newConstraint.constraintreference = Nothing
                    Else
                        newConstraint.constraintreference = xlsApp.Range(rhs)
                    End If
                    Me.Add(newConstraint)
                    Exit For
                End If
            Next index
            load = True
        Else
            load = False
        End If
    End Function
    Public Sub reset()
        col = New Collection
    End Sub
    Public Sub display(ByVal lbConstraints As Windows.Forms.ListBox)
        Dim constraint As Constraint
        For Each constraint In col
            lbConstraints.Items.Add(constraint.value(False))
        Next
    End Sub
    Public Sub displayReset(ByVal lbConstraints As Windows.Forms.ListBox)
        lbConstraints.Items.Clear()
    End Sub
    Public Function validate() As String
        Dim rindex As Long
        Dim cindex As Long
        validate = False
        For rindex = 1 To col.Count
            If (xlsApp.Intersect(col(rindex).reference, col(rindex).constraintreference) Is Nothing) Then
                validate = ""
            Else
                Return False
                Exit Function
            End If
            For cindex = rindex + 1 To col.Count

                If (Not (xlsApp.Intersect(col(rindex).reference, col(cindex).reference) Is Nothing) _
                And Not (xlsApp.Intersect(col(rindex).constraintreference, col(cindex).constraintreference) Is Nothing)) _
                Or (Not (xlsApp.Intersect(col(rindex).reference, col(cindex).constraintreference) Is Nothing) _
                And Not (xlsApp.Intersect(col(rindex).constraintreference, col(cindex).reference) Is Nothing)) _
                Then
                    validate = col(rindex).value(False).ToString & " overlaps with " & col(cindex).value(False).ToString
                    Return validate
                    Exit Function

                Else
                    validate = ""
                End If


                '  If (xlsApp.Intersect(col(rindex).reference, col(cindex).reference) Is Nothing) Then _
                ' And (xlsApp.Intersect(col(rindex).reference, col(cindex).constraintreference) Is Nothing) Then _
                ' And (xlsApp.Intersect(col(rindex).constraintreference, col(cindex).reference) Is Nothing) Then _
                'And (xlsApp.Intersect(col(rindex).constraintreference, col(cindex).constraintreference) Is Nothing) Then _
                ' Then
                '     validate = True
                ' Else
                'Return False
                '     Exit Function
                ' End If

            Next cindex
        Next rindex
        Return validate
    End Function
End Class
