Public Class Options
    Public maxTime As Long
    Public iterations As Long
    Public precision As Double
    Public tolerance As Double
    Public linearModel As Boolean
    Public showResults As Boolean
    Public automaticScaling As Boolean
    Public estimateMethod As Integer
    Public derivativeMethod As Integer
    Public searchMethod As Integer
    Public convergence As Double
    Public assumeNonNegative As Boolean
    Public initializeValues As Boolean
    Public Sub reset()
        maxTime = 1000
        iterations = 100
        precision = 0.000001
        tolerance = 5
        linearModel = True
        showResults = True
        automaticScaling = True
        estimateMethod = 1
        derivativeMethod = 1
        searchMethod = 1
        convergence = 0.00000001
        assumeNonNegative = True
        initializeValues = True
    End Sub
    Public Function value() As String
        value = Str(maxTime) + "," _
                + Str(iterations) + "," _
                + Str(precision) + "," _
                + Str(tolerance) + "," _
                + Str(linearModel) + "," _
                + Str(showResults) + "," _
                + Str(automaticScaling) + "," _
                + Str(estimateMethod) + "," _
                + Str(derivativeMethod) + "," _
                + Str(searchMethod) + "," _
                + Str(convergence) + "," _
                + Str(assumeNonNegative) + "," _
                + Str(initializeValues) 
    End Function
    Public Function load(ByVal value As Excel.Range) As Boolean
        Dim options
        Dim formulaarray
        formulaarray = value.FormulaArray
        On Error GoTo nooptions
        options = Split(Mid(formulaarray, 3, Len(formulaarray) - 3), ",")
        maxTime = options(0)
        iterations = options(1)
        precision = options(2)
        tolerance = options(3)
        linearModel = options(4)
        showResults = options(5)
        automaticScaling = options(6)
        estimateMethod = options(7)
        derivativeMethod = options(8)
        searchMethod = options(9)
        convergence = options(10)
        assumeNonNegative = options(11)
        initializeValues = options(12)
        load = True
        Exit Function
nooptions:
        Resume nooptionshandler
nooptionshandler: On Error GoTo 0
        load = False
    End Function
End Class
