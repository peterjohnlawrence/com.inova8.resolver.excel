Public Class ProblemClass
    Public target As Target
    Public variables As Variables
    Public constraints As Constraints
    Public expressions As Expressions
    Public options As Options
    Public UserForm As frmProblem
    Private Resolver As com.inova8.resolver.ResolverClass
    Private ResolverLicense As com.inova8.resolver.LicenserClass = New com.inova8.resolver.LicenserClass


    Public Sub New()
        target = New Target
        variables = New Variables
        expressions = New Expressions
        constraints = New Constraints
        options = New Options
        UserForm = New frmProblem
    End Sub

    Public Function load(ByVal problemRange As Excel.Range) As Boolean
        Dim selectionOffset As Long
        selectionOffset = 1
        Me.target.getValues(problemRange.Cells(selectionOffset))
        selectionOffset = selectionOffset + 1
        Me.variables.load(problemRange.Cells(selectionOffset))
        selectionOffset = selectionOffset + 1
        While problemRange.Cells(selectionOffset).hasArray
            Me.constraints.load(problemRange.Cells(selectionOffset))
            selectionOffset = selectionOffset + 1
        End While
        Me.options.load(problemRange.Cells(selectionOffset))
        load = True
      End Function
    Public Sub reset()
        target.reset()
        variables.reset()
        constraints.reset()
        options.reset()
    End Sub
    Public Function save(ByVal problemRange As Excel.Range) As Boolean
        Dim myConstraint As Constraint
        Dim selectionOffset As Long
        Dim globalAddress As Boolean
        selectionOffset = 1

        If problemRange.Worksheet Is xlsSheet Then
            globalAddress = False
        Else
            globalAddress = True
        End If
        problemRange(selectionOffset).formula = "=MIN(" + problemRange(selectionOffset + 3 + Me.constraints.count).address(, , , globalAddress) + ")"
        selectionOffset = selectionOffset + 1
        If variables.count = 0 Then
            problemRange(selectionOffset).formula = "=COUNT(0)"
        Else
            problemRange(selectionOffset).formula = "=COUNT(" + variables.value(globalAddress) + ")"
        End If
        selectionOffset = selectionOffset + 1

        For Each myConstraint In Me.constraints.Collection
            'If myConstraint.hasArray Then
            problemRange(selectionOffset).formulaarray = "=" + myConstraint.value(globalAddress)
            'Else
            'problemRange(selectionOffset).formula = "=" + myConstraint.value(globalAddress)
            'End If
            selectionOffset = selectionOffset + 1
        Next myConstraint

        problemRange(selectionOffset).formula = "={" + options.value + "}"
        selectionOffset = selectionOffset + 1

        problemRange(selectionOffset).formula = "=Gaussian(" + variables.value(globalAddress) + ")"
        selectionOffset = selectionOffset + 1
        save = True
    End Function
    Public Sub display(ByVal problemForm As Windows.Forms.Form)
        constraints.display(CType(problemForm.Controls.Find("lbConstraints", True).First, Windows.Forms.ListBox))
        variables.display(CType(problemForm.Controls.Find("lbVariables", True).First, Windows.Forms.ListBox))
    End Sub
    Public Sub displayReset(ByVal problemForm As Windows.Forms.Form)
        constraints.displayReset(CType(problemForm.Controls.Find("lbConstraints", True).First, Windows.Forms.ListBox))
        variables.displayReset(CType(problemForm.Controls.Find("lbVariables", True).First, Windows.Forms.ListBox))
    End Sub
    Public Sub Resolve()
        Dim constraint As Constraint
        Dim variable As Excel.Range
        Dim expression As Excel.Range
        Dim index As Long
        Dim names As String = ""
        Dim constraints As String = ""
        Dim dependents As String = ""
        Dim measurements As String = ""
        Dim cellname As String = ""
        Dim errors As String = ""
        Dim cell As Excel.Range
        Dim Sheet As Excel.Worksheet

        xlsApp.ScreenUpdating = False
        xlsApp.Calculation = Excel.XlCalculation.xlCalculationManual

        Resolver = New com.inova8.resolver.ResolverClass
        flatten()

        For Each name In xlsSheet.Names
            If (name.visible) Then
                Try
                    'On Error GoTo nowsname
                    Sheet = name.referstorange.worksheet
                    If (Not (Sheet Is Nothing) And Sheet Is xlsSheet) Then
                        cellname = Mid(name.name, InStr(1, name.name, "!") + 1)
                        Resolver.addName(cellname, name.referstorange.address)
                        names = names + cellname + ";" + name.referstorange.address + vbCrLf
                    End If
                    'nowsname:       Resume nowsnamehandler
                    'nowsnamehandler: On Error GoTo 0
                catch
                End Try
            End If
        Next name

        For Each name In xlsWB.Names
            If (name.visible) Then
                Try
                    'On Error GoTo nowbname
                    Sheet = name.referstorange.worksheet
                    If (Not (Sheet Is Nothing) And Sheet Is xlsSheet) Then
                        cellname = Mid(name.name, InStr(1, name.name, "!") + 1)
                        Resolver.addName(cellname, name.referstorange.address)
                        'names = names + cellname + ";" + name.referstorange.address + vbCrLf
                    End If
                    'nowbname:       Resume nowbnamehandler
                    'nowbnamehandler: On Error GoTo 0
                Catch
                End Try
            End If
        Next name

        For Each constraint In Me.constraints.Collection
            With constraint
                For index = 1 To .reference.Cells.Count
                    If (CheckForErrors(.reference.Cells(index))) Then
                        errors += vbTab & "Cell error at " & .reference.Cells(index).address & vbCrLf
                    ElseIf (CheckForErrors(.constraintreference.Cells(index))) Then
                        errors += vbTab & "Cell error at " & .constraintreference.Cells(index).address & vbCrLf
                    ElseIf Len(.reference.Cells(index).formula) > 255 Then
                        errors += vbTab & "Formula > 256 characters at " & .constraintreference.Cells(index).address & vbCrLf
                    ElseIf Len(.constraintreference.Cells(index).formula) > 255 Then
                        errors += vbTab & "Formula > 256 characters at " & .constraintreference.Cells(index).address & vbCrLf
                    Else
                        Resolver.addConstraint(CellAddress(.reference.Cells(index)), CellFormula(.reference.Cells(index)), .equality, CellAddress(.constraintreference.Cells(index)), CellFormula(.constraintreference.Cells(index)))
                        'constraints = constraints + CellAddress(.reference.Cells(index)) + ";" + CellFormula(.reference.Cells(index)) + ";" + .equality + ";" + CellAddress(.constraintreference.Cells(index)) + ";" + CellFormula(.constraintreference.Cells(index)) + vbCrLf
                    End If
                Next index
            End With
        Next constraint

        For Each expression In Me.expressions.Collection
            With expression
                For Each cell In expression.Cells
                    If CheckForErrors(cell) Then
                        errors += vbTab & "Cell error at " & cell.Address & vbCrLf
                    Else
                        Resolver.addDependent(CellAddress(cell), CellFormula(cell))
                        'dependents = dependents + CellAddress(cell) + ";" + CellFormula(cell) + vbCrLf
                    End If
                Next cell
            End With
        Next expression

        For Each variable In Me.variables.Collection
            For Each cell In variable.Cells
                If CheckForErrors(cell) Then
                    errors += vbTab & "Cell error at " & cell.Address & vbCrLf
                Else
                    Resolver.addVariable(CellAddress(cell), IIf(options.initializeValues, Format(cell.Offset(, 1).Value), "") + " ", Format(cell.Offset(, 3).Value) + " ", Format(cell.Offset(, 2).Value) + " ", options.assumeNonNegative)
                    'measurements = measurements + CellAddress(cell) + ";" + IIf(options.initializeValues, Format(cell.Offset(, 1).Value), "") + " " + ";" + Format(cell.Offset(, 3).Value) + " " + ";" + Format(cell.Offset(, 2).Value) + " ;" + Format(options.assumeNonNegative) + vbCrLf
                End If
                Next cell ' index
        Next variable


        Dim reportResults As Boolean
        If ((Resolver.NumberCells > com.inova8.resolver.LicenserClass.maxCells) And (ResolverLicense.licenseInformation.type = com.inova8.resolver.LicenserClass.LicenseType.Trial)) Then
            MsgBox("Problem size exceeds that of the trial license (" & _
                   com.inova8.resolver.LicenserClass.maxCells & ")" & vbCrLf _
                   & "Your problem is using " & Resolver.NumberCells & " Excel cells.")
            ' & "http://inova8.com/joomla/index.php/software/viewdownload/4-software-downloads/3-resolver-excel-addin-for-process-data-reconciliation.")

        ElseIf errors <> "" Then
            MsgBox("Cannot reconcile due to these errors:" + vbCrLf + errors)
        Else
            reportResults = Resolver.Resolve(options.maxTime, options.iterations, options.precision, options.convergence)
            Dim ResultsForm = New frmResults
            If reportResults Then
                ResultsForm.lblResults.Text = "Resolver found a solution. All constraints and optimality conditions are satisfied."
            Else
                ResultsForm.lblResults.Text = "Resolver failed to converge within time limit and/or number of iterations."
            End If
            problem.UserForm.TopMost = False
            ResultsForm.ShowDialog()
            problem.UserForm.TopMost = True
            If ResultsForm.ok Then
                If ResultsForm.btnRetain.Checked Then
                    writeresults(True)
                End If
                If ResultsForm.btnAnswerReport.Checked Then
                    Report()
                End If
                If ResultsForm.btnVariableSensitivityReport.Checked Then
                    VariableSensitivityReport()
                End If
            End If
            ResultsForm.Hide()
        End If
        xlsApp.ScreenUpdating = True
        xlsApp.Calculation = Excel.XlCalculation.xlCalculationAutomatic

    End Sub
    'Private Sub printmodel(ByVal category As String, ByVal contents As String)
    '    Dim myfile As String
    '    Dim f
    '    f = FreeFile()
    '    myfile = "C:\Users\PeterL\test" & category & ".txt"
    'Open myfile For Output As f
    'Print #f, contents
    'Close #f
    'End Sub
    Private Function CheckForErrors(ByVal cell As Excel.Range) As Boolean
        If cell.Errors.Item(1).Value Then
            CheckForErrors = True
        Else
            CheckForErrors = False
        End If
    End Function
    Private Function CellAddress(ByVal cell As Excel.Range) As String
        On Error GoTo norange

        CellAddress = cell.Name.name
        'CellAddress = cell.Address
        CellAddress = Mid(CellAddress, InStr(1, CellAddress, "!") + 1)
        On Error GoTo 0
        Exit Function

norange: Resume norangeselected
norangeselected: On Error GoTo 0
        CellAddress = cell.Address(True, True, Excel.XlReferenceStyle.xlA1, False)
    End Function
    Private Function CellFormula(ByVal cell As Excel.Range) As String
        If cell.HasFormula Then
            CellFormula = xlsApp.ConvertFormula(cell.Formula, Excel.XlReferenceStyle.xlA1, Excel.XlReferenceStyle.xlA1, Excel.XlReferenceType.xlAbsolute)
        Else
            CellFormula = "=" + cell.Formula
        End If
    End Function
    Private Function TranslateFormula(ByVal cell As Excel.Range, ByVal formula As String) As String
        Dim index As Long
        Dim precedent As Excel.Range
        Dim precedentName As Object
        TranslateFormula = formula

        On Error GoTo noprecedents
        For Each precedent In cell.DirectPrecedents
            TranslateFormula = Replace(TranslateFormula, precedent.Address(True, True, Excel.XlReferenceStyle.xlA1, False), precedent.Name.name)
            'precedentName = precedent.Name
            'TranslateFormula = Replace(TranslateFormula, precedentName.Name, precedentName.RefersTo) '(True, True, Excel.XlReferenceStyle.xlA1, False))
        Next precedent
noprecedents: Resume noprecedentscomplete
noprecedentscomplete: On Error GoTo 0
    End Function

    Private Function Replace(ByVal StringIn As String, ByVal TargetString As String, ByVal ReplaceString As String) As String
        Dim strReturn As String = ""
        Dim strLeftOver As String = ""

        strLeftOver = StringIn

        Do While InStr(strLeftOver, TargetString) > 0
            strReturn = strReturn & Mid$(strLeftOver, 1, InStr(strLeftOver, TargetString) - 1) & ReplaceString
            strLeftOver = Mid$(strLeftOver, InStr(strLeftOver, TargetString) + Len(TargetString))
        Loop

        strReturn = strReturn & strLeftOver

        Replace = strReturn
    End Function
    Public Sub flatten()
        Dim constraint As Constraint
        Dim index As Long
        Dim found As Boolean
        Dim directPrecedent As Excel.Range
        Dim variable As Excel.Range
        Dim expression As Excel.Range
        Dim addresses As Excel.Range = Nothing
        Dim referenceCell As Excel.Range
        Dim constraintreferenceCell As Excel.Range
        If Me.variables.count > 0 Then
            For Each variable In Me.variables.Collection
                If addresses Is Nothing Then
                    addresses = variable
                Else
                    addresses = xlsApp.Union(addresses, variable)
                End If
            Next variable
        End If

        expressions = New Expressions
        For Each constraint In Me.constraints.Collection
            'For index = 1 To constraint.reference.Cells.count
            For Each referenceCell In constraint.reference.Cells
                If referenceCell.HasFormula And Not referenceCell.hasArray Then
                    On Error GoTo noprecedents
                    For Each directPrecedent In referenceCell.DirectPrecedents
                        If Me.variables.count > 0 Then
                            If xlsApp.Intersect(directPrecedent, addresses) Is Nothing Then
                                Me.expressions.Add(directPrecedent)
                            End If
                        Else
                            Me.expressions.Add(directPrecedent)
                        End If
                    Next directPrecedent
noprecedents:       Resume endloop

endloop:        End If
            Next referenceCell 'index
            On Error GoTo 0
            'For index = 1 To constraint.constraintreference.Cells.count
            For Each constraintreferenceCell In constraint.constraintreference.Cells
                If constraintreferenceCell.HasFormula And Not constraintreferenceCell.hasArray Then
                    On Error GoTo noconstraintprecedents
                    For Each directPrecedent In constraintreferenceCell.DirectPrecedents
                        If Me.variables.count > 0 Then
                            If xlsApp.Intersect(directPrecedent, addresses) Is Nothing Then
                                Me.expressions.Add(directPrecedent)
                            End If
                        Else
                            Me.expressions.Add(directPrecedent)
                        End If
                    Next directPrecedent
noconstraintprecedents: Resume endconstraintloop
endconstraintloop: End If
            Next constraintreferenceCell 'index
        Next constraint
        On Error GoTo 0
        For Each expression In Me.expressions.Collection
            If expression.HasFormula And Not expression.hasArray Then
                For Each directPrecedent In expression.DirectPrecedents
                    If addresses Is Nothing Then
                        Me.expressions.Add(directPrecedent)
                    ElseIf xlsApp.Intersect(directPrecedent, addresses) Is Nothing Then
                        Me.expressions.Add(directPrecedent)
                    End If
                Next directPrecedent
            End If
        Next expression
    End Sub
    Private Sub Report()
        'Dim j As Long
        Dim RowOffset As Long
        Dim ColOffset As Long
        Dim Worksheet As Excel.Worksheet
        Dim currentWorksheet As Excel.Worksheet
        Dim MeasurementCriticalValue As Double
        Dim ConstraintCriticalValue As Double

        currentWorksheet = xlsSheet
        Worksheet = xlsWB.Worksheets.Add

        MeasurementCriticalValue = Resolver.MeasurementCriticalValue
        ConstraintCriticalValue = Resolver.ConstraintCriticalValue

        Worksheet.Name = nextreport("Reconciliation Report")
        Worksheet.Columns("A").ColumnWidth = 2.0#
        Worksheet.Columns("B").ColumnWidth = 12.5
        Worksheet.Columns("C").ColumnWidth = 14.5
        Worksheet.Columns("D").ColumnWidth = 14.5
        Worksheet.Columns("E").ColumnWidth = 14.5
        Worksheet.Columns("F").ColumnWidth = 14.5
        Worksheet.Columns("G").ColumnWidth = 14.5
        Worksheet.Columns("H").ColumnWidth = 14.5
        Worksheet.Columns("I").ColumnWidth = 14.5
        Worksheet.Columns("J").ColumnWidth = 14.5

        Worksheet.Cells(1, 1).value = "Resolver Answer Report"
        Worksheet.Cells(2, 1).value = "Worksheet:" + "[" + xlsWB.Name + "]" + currentWorksheet.Name
        Worksheet.Cells(3, 1).value = "Report Created:" + Format(Now)

        font(xlsApp.Range("A1:A3"))

        RowOffset = 5
        ColOffset = 2

        With Resolver

            Worksheet.Cells(RowOffset, 1).value = "Problem"
            font(Worksheet.Cells(RowOffset, 1))
            RowOffset = RowOffset + 1

            Worksheet.Cells(RowOffset, ColOffset + 0).value = "Total Constraints"
            Worksheet.Cells(RowOffset, ColOffset + 1).value = "Linear"
            Worksheet.Cells(RowOffset, ColOffset + 2).value = "Nonlinear"
            Worksheet.Cells(RowOffset, ColOffset + 3).value = "Active Linear"
            Worksheet.Cells(RowOffset, ColOffset + 4).value = "Active Nonlinear"
            Worksheet.Cells(RowOffset, ColOffset + 5).value = "Variables"
            Worksheet.Cells(RowOffset, ColOffset + 6).value = "Fixed Variables"
            font(xlsApp.Range("B6:H6"))
            borders(xlsApp.Range("B6:H6"))
            RowOffset = RowOffset + 1

            Worksheet.Cells(RowOffset, ColOffset + 0).value = .Constraints.Count
            Worksheet.Cells(RowOffset, ColOffset + 1).value = .Constraints.LinearConstraints
            Worksheet.Cells(RowOffset, ColOffset + 2).value = .Constraints.NonlinearConstraints
            Worksheet.Cells(RowOffset, ColOffset + 3).value = .Constraints.ActiveLinearInequalityConstraints
            Worksheet.Cells(RowOffset, ColOffset + 4).value = .Constraints.ActiveNonlinearInequalityConstraints
            Worksheet.Cells(RowOffset, ColOffset + 5).value = .Results.Count
            Worksheet.Cells(RowOffset, ColOffset + 6).value = .Results.FixedValues
            RowOffset = RowOffset + 1

            Worksheet.Cells(RowOffset, 1).value = "Convergence"
            font(Worksheet.Cells(RowOffset, 1))
            RowOffset = RowOffset + 1

            Worksheet.Cells(RowOffset, ColOffset + 0).value = "Converged"
            Worksheet.Cells(RowOffset, ColOffset + 1).value = "Termination"
            Worksheet.Cells(RowOffset, ColOffset + 2).value = "Time"
            Worksheet.Cells(RowOffset, ColOffset + 3).value = "Iterations"
            Worksheet.Cells(RowOffset, ColOffset + 4).value = "Precision"
            Worksheet.Cells(RowOffset, ColOffset + 5).value = "Convergence"
            font(xlsApp.Range("B9:G9"))
            borders(xlsApp.Range("B9:G9"))
            RowOffset = RowOffset + 1

            Worksheet.Cells(RowOffset, ColOffset + 0).value = .Convergence.Converged
            Worksheet.Cells(RowOffset, ColOffset + 1).value = .Convergence.Terminated
            If Not (.Convergence.Converged) Then highlight(Worksheet.Cells(RowOffset, ColOffset + 1))
            Worksheet.Cells(RowOffset, ColOffset + 2).value = .Convergence.ConvergenceTime / 1000
            If (.Constraints.NonlinearConstraints > 0) Then
                Worksheet.Cells(RowOffset, ColOffset + 3).value = .Convergence.Iterations
                Worksheet.Cells(RowOffset, ColOffset + 4).value = .Convergence.CalculatedPrecision
                Worksheet.Cells(RowOffset, ColOffset + 5).value = .Convergence.CostChange
            End If
            RowOffset = RowOffset + 1

            Worksheet.Cells(RowOffset, 1).value = "Target"
            font(Worksheet.Cells(RowOffset, 1))
            RowOffset = RowOffset + 1

            Worksheet.Cells(RowOffset, ColOffset + 0).value = "Cell"
            Worksheet.Cells(RowOffset, ColOffset + 1).value = "Reconciled Cost"
            Worksheet.Cells(RowOffset, ColOffset + 2).value = "Global Critical Value"
            Worksheet.Cells(RowOffset, ColOffset + 3).value = "Redundancy Degree"
            Worksheet.Cells(RowOffset, ColOffset + 4).value = "Measurement Critical Value"
            Worksheet.Cells(RowOffset, ColOffset + 5).value = "Constraint Critical Value"

            font(xlsApp.Range("B12:G12"))
            borders(xlsApp.Range("B12:G12"))

            RowOffset = RowOffset + 1

            Worksheet.Cells(RowOffset, ColOffset + 0).value = problemRange.Address
            Worksheet.Cells(RowOffset, ColOffset + 1).value = .ReconciledCost
            If (.ReconciledCost >= .GlobalCriticalValue) Then highlight(Worksheet.Cells(RowOffset, ColOffset + 1))
            Worksheet.Cells(RowOffset, ColOffset + 2).value = .GlobalCriticalValue
            Worksheet.Cells(RowOffset, ColOffset + 3).value = .RedundancyDegree
            Worksheet.Cells(RowOffset, ColOffset + 4).value = .MeasurementCriticalValue
            Worksheet.Cells(RowOffset, ColOffset + 5).value = .ConstraintCriticalValue

            RowOffset = RowOffset + 2

            Worksheet.Cells(RowOffset, 1).value = "Adjusted Variables"
            font(Worksheet.Cells(RowOffset, 1))

            RowOffset = RowOffset + 1

            Worksheet.Cells(RowOffset, ColOffset + 0).value = "Cell"
            Worksheet.Cells(RowOffset, ColOffset + 1).value = "Reconciled Value"
            Worksheet.Cells(RowOffset, ColOffset + 2).value = "Measured Value"
            Worksheet.Cells(RowOffset, ColOffset + 3).value = "Solvability"
            Worksheet.Cells(RowOffset, ColOffset + 4).value = "Reconciled Test"
            Worksheet.Cells(RowOffset, ColOffset + 5).value = "Measured Test"
            Worksheet.Cells(RowOffset, ColOffset + 6).value = "Measured Tolerance"
            Worksheet.Cells(RowOffset, ColOffset + 7).value = "Reconciled Tolerance"

            font(xlsApp.Range("B16:I16"))
            borders(xlsApp.Range("B16:I16"))

            Dim result As com.inova8.resolver.ResultClass
            For Each result In .Results
                With result
                    RowOffset = RowOffset + 1
                    Worksheet.Cells(RowOffset, ColOffset + 0).value = .CellName
                    If (.SolvabilityText <> "Unobservable") Then
                        Worksheet.Cells(RowOffset, ColOffset + 1).value = .ReconciledValue
                        Worksheet.Cells(RowOffset, ColOffset + 7).value = .ReconciledTolerance
                        If (.hasMeasurement) Then

                            Worksheet.Cells(RowOffset, ColOffset + 2).value = .MeasuredValue
                            Worksheet.Cells(RowOffset, ColOffset + 4).value = .ReconciledTest
                            If (Math.Abs(.ReconciledTest) >= MeasurementCriticalValue) Then
                                highlight(Worksheet.Cells(RowOffset, ColOffset + 4))
                            End If
                            Worksheet.Cells(RowOffset, ColOffset + 5).value = .MeasuredTest
                            Worksheet.Cells(RowOffset, ColOffset + 6).value = .MeasuredTolerance
                        End If
                    End If
                    Worksheet.Cells(RowOffset, ColOffset + 3).value = .SolvabilityText
                End With
            Next result

            RowOffset = RowOffset + 2
            Worksheet.Cells(RowOffset, 1).value = "Constraints"
            font(Worksheet.Cells(RowOffset, 1))

            RowOffset = RowOffset + 1

            Worksheet.Cells(RowOffset, ColOffset + 0).value = "Cells"
            Worksheet.Cells(RowOffset, ColOffset + 1).value = "Active"
            Worksheet.Cells(RowOffset, ColOffset + 2).value = "Measured Residual"
            Worksheet.Cells(RowOffset, ColOffset + 3).value = "Reconciled Residual"
            Worksheet.Cells(RowOffset, ColOffset + 4).value = "Measured Test"
            Worksheet.Cells(RowOffset, ColOffset + 5).value = "Reconciled Test"
            Worksheet.Cells(RowOffset, ColOffset + 6).value = "Reconciled Deviation"
            Worksheet.Cells(RowOffset, ColOffset + 7).value = "Fomula"

            font(xlsApp.Range("B" + Format(RowOffset) + ":" + "I" + Format(RowOffset)))
            borders(xlsApp.Range("B" + Format(RowOffset) + ":" + "I" + Format(RowOffset)))

            Dim constraint As com.inova8.resolver.ConstraintClass
            Dim startRow As Int32 = RowOffset + 1
            For Each constraint In .Constraints
                With constraint
                    RowOffset = RowOffset + 1
                    Worksheet.Cells(RowOffset, ColOffset + 0).value = .Address
                    Worksheet.Cells(RowOffset, ColOffset + 1).value = .Active
                    Worksheet.Cells(RowOffset, ColOffset + 2).value = .MeasuredResidual
                    Worksheet.Cells(RowOffset, ColOffset + 3).value = .ReconciledResidual
                    If (.Active = True) Then
                        Worksheet.Cells(RowOffset, ColOffset + 4).value = .MeasuredTest
                        Worksheet.Cells(RowOffset, ColOffset + 5).value = .ReconciledTest
                        If (Math.Abs(.ReconciledTest) >= ConstraintCriticalValue) Then highlight(Worksheet.Cells(RowOffset, ColOffset + 5))
                        Worksheet.Cells(RowOffset, ColOffset + 6).value = .ReconciledDeviation
                    End If
                    Worksheet.Cells(RowOffset, ColOffset + 7).value = .Serialize
                End With
            Next constraint

            Dim serializeRange As Excel.Range

            serializeRange = Worksheet.Range(Worksheet.Cells(startRow, ColOffset + 7), Worksheet.Cells(RowOffset, ColOffset + 7))
            For Each result In .Results
                With result
                    If Math.Abs(.ReconciledTest) >= MeasurementCriticalValue Then
                        xlRangeTextMgmt(serializeRange, .CellName, 3)
                    End If
                End With
            Next result
        End With
        currentWorksheet.Activate()
    End Sub
    Private Sub VariableSensitivityReport()
        Dim RowOffset As Long
        Dim ColOffset As Long
        Dim Worksheet As Excel.Worksheet
        Dim currentWorksheet As Excel.Worksheet

        currentWorksheet = xlsSheet
        Worksheet = xlsWB.Worksheets.Add

        Worksheet.Name = nextreport("Variable Sensitivity Report")

        Worksheet.Cells(1, 1).value = "Resolver Variable Sensitivity Report"
        Worksheet.Cells(2, 1).value = "Worksheet:" + "[" + xlsWB.Name + "]" + currentWorksheet.Name
        Worksheet.Cells(3, 1).value = "Report Created:" + Format(Now)

        font(xlsApp.Range("A1:A3"))

        RowOffset = 5
        ColOffset = 1

        With Resolver

            Dim result As com.inova8.resolver.ResultClass

            For Each result In .Results
                With result
                    ColOffset += 1
                    Worksheet.Cells(RowOffset, ColOffset).value = .CellName
                    font(Worksheet.Cells(RowOffset, ColOffset))
                    If result.hasMeasurement Then
                        If (Math.Abs(result.ReconciledTest) >= Resolver.MeasurementCriticalValue) Then
                            highlight(Worksheet.Cells(RowOffset, ColOffset))
                        End If
                    End If
                 End With
            Next

            For Each result In .Results
                If result.hasMeasurement Then
                    RowOffset += 1
                    ColOffset = 1
                    Worksheet.Cells(RowOffset, ColOffset).value = result.CellName
                    font(Worksheet.Cells(RowOffset, ColOffset))
                    If (Math.Abs(result.ReconciledTest) >= Resolver.MeasurementCriticalValue) Then
                        highlight(Worksheet.Cells(RowOffset, ColOffset))
                    End If
                    For Each variable In .Results
                        ColOffset += 1
                        Worksheet.Cells(RowOffset, ColOffset).value = String.Format("{0:0.0000;-0.0000;''}", Resolver.VariableSensitivity(variable, result))
                    Next variable
                End If
            Next result

        End With
        currentWorksheet.Activate()
    End Sub
    Private Sub ConstraintSensitivityReport()
        Dim RowOffset As Long
        Dim ColOffset As Long
        Dim Worksheet As Excel.Worksheet
        Dim currentWorksheet As Excel.Worksheet

        currentWorksheet = xlsSheet
        Worksheet = xlsWB.Worksheets.Add

        Worksheet.Name = nextreport("Constraint Sensitivity Report")

        Worksheet.Cells(1, 1).value = "Resolver Constraint Sensitivity Report"
        Worksheet.Cells(2, 1).value = "Worksheet:" + "[" + xlsWB.Name + "]" + currentWorksheet.Name
        Worksheet.Cells(3, 1).value = "Report Created:" + Format(Now)

        font(xlsApp.Range("A1:A3"))

        RowOffset = 5
        ColOffset = 1

        With Resolver

            Dim result As com.inova8.resolver.ResultClass

            For Each result In .Results
                With result
                    If .hasMeasurement Then
                        ColOffset += 1
                        Worksheet.Cells(RowOffset, ColOffset).value = .CellName
                        font(Worksheet.Cells(RowOffset, ColOffset))
                        If (Math.Abs(result.ReconciledTest) >= Resolver.MeasurementCriticalValue) Then
                            highlight(Worksheet.Cells(RowOffset, ColOffset))
                        End If
                    End If
                End With
            Next

            For Each constraint In .Constraints
                If constraint.active Then
                    RowOffset += 1
                    ColOffset = 1
                    Worksheet.Cells(RowOffset, ColOffset).value = constraint.Address
                    font(Worksheet.Cells(RowOffset, ColOffset))
                    For Each variable In .Results
                        If variable.hasMeasurement Then
                            ColOffset += 1
                            Worksheet.Cells(RowOffset, ColOffset).value = Resolver.ConstraintSensitivity(constraint, variable)
                        End If
                    Next variable
                End If
            Next constraint

        End With
        currentWorksheet.Activate()
    End Sub
    Private Sub font(ByVal range As Excel.Range)
        range.Font.Color = 8210719
        range.Font.Size = 11
        range.Font.Bold = True
    End Sub
    Private Sub borders(ByVal range As Excel.Range)
        topborders(range)
        btmborders(range)
        range.VerticalAlignment = Excel.Constants.xlTop
        range.HorizontalAlignment = Excel.XlVAlign.xlVAlignCenter
        range.WrapText = True
    End Sub
    Private Sub topborders(ByVal range As Excel.Range)
        range.Borders(Excel.XlBordersIndex.xlEdgeTop).LineStyle = 1
        range.Borders(Excel.XlBordersIndex.xlEdgeTop).Color = 8210719
        range.Borders(Excel.XlBordersIndex.xlEdgeTop).Weight = -4138
    End Sub
    Private Sub btmborders(ByVal range As Excel.Range)
        range.Borders(Excel.XlBordersIndex.xlEdgeBottom).LineStyle = 1
        range.Borders(Excel.XlBordersIndex.xlEdgeBottom).Color = 8210719
        range.Borders(Excel.XlBordersIndex.xlEdgeBottom).Weight = -4138
    End Sub
    Private Function nextreport(ByVal reportname As String) As String
        Dim i As Long
        Dim max As Long
        Dim number As String
        max = 0
        For i = 1 To xlsApp.Worksheets.Count
            If InStr(Mid(xlsApp.Worksheets(i).name, 1, Len(reportname)), reportname) > 0 Then
                number = Mid(xlsApp.Worksheets(i).name, Len(reportname) + 1)
                If IsNumeric(number) Then If number > max Then max = number
            End If
        Next i
        nextreport = reportname + " " + Format(max + 1)
    End Function
    Private Sub writeresults(ByVal comments As Boolean)
        Dim address As String
        Dim result As com.inova8.resolver.ResultClass
        With Resolver
            For Each result In .Results
                address = result.CellAddress
                xlsSheet.Range(address).Offset(, 1).Value = result.Value
                If comments Then
                    If Not xlsSheet.Range(address).Comment Is Nothing Then
                        xlsSheet.Range(address).Comment.Delete()
                    End If
                    xlsSheet.Range(address).AddComment(result.SolvabilityText & vbCrLf & Format(result.MeasuredTest, "0.00"))
                End If
            Next result
        End With
    End Sub
    Private Sub highlight(ByVal range As Excel.Range)
        range.Font.Color = 255
        range.Font.Bold = True
    End Sub
    Sub xlRangeTextMgmt(ByVal TargetRange As Excel.Range, ByVal TargetWord As String, Optional ByVal FontColor As Int32 = 3, Optional ByVal FontBold As Boolean = True)
        Dim cell As Excel.Range
        For Each cell In TargetRange
            xlCellTextMgmt(cell, TargetWord, FontColor, FontBold)
        Next
    End Sub
    Sub xlCellTextMgmt(ByVal TargetCell As Excel.Range, ByVal TargetWord As String, Optional ByVal FontColor As Int32 = 3, Optional ByVal FontBold As Boolean = True)
        Dim Start As Int32
        Dim postChar As Int32
        Dim priorChar As Int32
        Start = 0
        Do
            Start = InStr(Start + 1, TargetCell.Text, TargetWord, )
            If Start < 1 Then Exit Sub
            '
            '           test for each font arguement, if present, apply appropriately
            ' check for 48-57, 65-90, 97-122, 95, 46, 92 at either ends.
            '
            priorChar = Asc(TargetCell.Characters(Start - 1, 1).Text)
            postChar = Asc(TargetCell.Characters(Start + Len(TargetWord), 1).Text)

            If Not (((48 <= priorChar And priorChar <= 57) Or (65 <= priorChar And priorChar <= 90) Or (97 <= priorChar And priorChar <= 122) Or (95 = priorChar) Or (46 = priorChar) Or (92 = priorChar)) _
              Or ((48 <= postChar And postChar <= 57) Or (65 <= postChar And postChar <= 90) Or (97 <= postChar And postChar <= 122) Or (95 = postChar) Or (46 = postChar) Or (92 = postChar))) _
               Then
                With TargetCell.Characters(Start, Len(TargetWord)).Font
                    'If IsNull(FontName) = False Then .Name = FontName
                    'If IsNull(FontBold) = False Then .Bold = FontBold
                    'If IsNull(FontSize) = False Then .Size = FontSize
                    .Bold = FontBold
                    .ColorIndex = FontColor
                End With
                '
                '           if request was for ONLY the first instance of TargetWord, exit
                '               otherwise, loop back and see if there are more instances
                '
                'If FirstOnly = True Then Exit Sub

            End If
        Loop
    End Sub
End Class
