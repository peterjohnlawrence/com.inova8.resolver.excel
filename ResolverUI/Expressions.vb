Public Class Expressions
    Private col As Collection

    Public Sub New()
        col = New Collection
    End Sub

    Public Sub Add(ByVal Item As Excel.Range)
        'test that it does not already exist
        Dim found As Boolean
        Dim temp As Excel.Range
        found = False
        On Error Resume Next
        temp = col(Item.Address)
        found = True
        On Error GoTo 0
        If temp Is Nothing Then col.Add(Item, Item.Address)
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


End Class
