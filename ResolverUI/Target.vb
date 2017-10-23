Public Class Target
    Public op As String
    Public limit As Double
    Public range As String
    Public Function getValues(ByVal target As Excel.Range) As Boolean
        'parse formula like "=MIN($C$4)" or "=$C$4=9"
        'equality MIN, MAX, =
        'Limit nothing or numeric
        'Formula = formula or cell
        On Error GoTo notarget
        range = target.DirectPrecedents.Address
        op = Mid(target.Formula, 2, IIf(InStr(target.Formula, range) > 2, InStr(target.Formula, range) - 3, 0))
        If op = "" Then
            limit = Mid(target.Formula, InStr(2, target.Formula, "=") + 1)
        End If
        getValues = True
        Exit Function
notarget:
        Resume notargethandler
notargethandler: On Error GoTo 0
        getValues = False
    End Function
    Public Sub reset()
        op = "="
        limit = 0.0#
        range = ""
    End Sub
End Class
