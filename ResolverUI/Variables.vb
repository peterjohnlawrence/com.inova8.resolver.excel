Public Class Variables
    Private col As Collection
    Public Sub New()
        col = New Collection
    End Sub

    Public Sub Add(ByVal Item As Excel.Range)
        col.Add(Item, Item.Address)
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
    Public Function load(ByVal variableRange As Excel.Range) As Boolean
        Dim variables
        Dim index As Long
        On Error GoTo novariables
        variables = Split(Mid(variableRange.Formula, 8, Len(variableRange.Formula) - 8), ",")
        For index = LBound(variables) To UBound(variables)
            col.Add(xlsApp.Range(variables(index)), xlsApp.Range(variables(index)).Address)
        Next index
        load = True
        Exit Function
novariables:
        Resume novariableshandler
novariableshandler: On Error GoTo 0
        load = False
    End Function
    Public Function value(ByVal globalAddress As Boolean) As String
        Dim myVariable As Excel.Range
        Dim firstVariable As Boolean
        Dim val As String = ""
        firstVariable = True
        For Each myVariable In problem.variables.Collection
            If Not firstVariable Then
                val = val + ","
            Else
                firstVariable = False
            End If
            val = val + myVariable.Address(, , , globalAddress)
        Next myVariable
        Return val
    End Function
    Public Sub reset()
        col = New Collection
    End Sub
    Public Sub display(ByVal lbVariables As Windows.Forms.ListBox)
        Dim variable As Excel.Range
        For Each variable In col
            lbVariables.Items.Add(variable.Address)
        Next
    End Sub
    Public Sub displayReset(ByVal lbVariables As Windows.Forms.ListBox)
        lbVariables.Items.Clear()
    End Sub
End Class
