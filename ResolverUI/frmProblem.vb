Public Class frmProblem
    Private currentConstraint As Constraint
    Private currentVariable As Excel.Range
    Private editConstraintIndex As Long
    Private editConstraintKey As String
    Private editVariableIndex As Long
    Private editVariableKey As String
    Public Sub validateButtons()
        If lbVariables.Items.Count > 0 Then
            If lbConstraints.Items.Count > 0 Then
                btnResolve.Enabled = True
                btnSaveModel.Enabled = True
            Else
                btnResolve.Enabled = False
                btnSaveModel.Enabled = False
            End If
        Else
            btnResolve.Enabled = False
            btnSaveModel.Enabled = False
        End If
        If lbConstraints.Items.Count > 0 Then
            btnGuess.Enabled = True
        Else
            btnGuess.Enabled = False
        End If
        If lbVariables.Items.Count > 1 Then
            btnConsolidate.Enabled = True
        Else
            btnConsolidate.Enabled = False
        End If
    End Sub
    Private Sub btnAddConstraint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddConstraint.Click
        Dim newConstraint As New Constraint
        newConstraint.reference = currentConstraint.reference
        newConstraint.equality = currentConstraint.equality
        newConstraint.constraintreference = currentConstraint.constraintreference
        On Error GoTo duplicate
        If Not newConstraint.validate Then
            MsgBox("Constraint invalid: need same number of expressions on both sides")
            Exit Sub
        End If
        problem.constraints.Add(newConstraint)
        On Error GoTo 0
        lbConstraints.Items.Add(newConstraint.value(False))
        currentConstraint_reset()
        btnAddConstraint.Enabled = False
        validateButtons()
        Exit Sub
duplicate: Resume duplicatehandler
duplicatehandler: On Error GoTo 0
        MsgBox("Duplicate constraint")
        btnAddConstraint.Enabled = False
        Exit Sub
    End Sub

    Private Sub btnAddVariable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddVariable.Click
        Dim newVariable As Excel.Range
        newVariable = currentVariable
        On Error GoTo duplicate
        problem.variables.Add(newVariable)
        On Error GoTo 0
        lbVariables.Items.Add(newVariable.Address)
        currentVariable_reset()
        btnAddVariable.Enabled = False
        validateButtons()
        Exit Sub
duplicate: Resume duplicatehandler
duplicatehandler: On Error GoTo 0
        MsgBox("Duplicate variable")
        btnAddVariable.Enabled = False
    End Sub

    Private Sub btnClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClose.Click
        Dim saveRange As Excel.Range
        If (btnResolve.Enabled = True) Then
            'Does name exist on this sheet?
            On Error Resume Next
            saveRange = xlsSheet.Names.Item("ResolverData").RefersToRange
            On Error GoTo 0

            If (saveRange Is Nothing) Then
                'name does not exist on this sheet
                'does sheet already exist in saved data
                On Error Resume Next
                saveRange = xlsApp.Range("'ResolverReservedData'!A1").Rows(1).Find(what:=xlsSheet.Name, lookat:=Excel.XlLookAt.xlWhole, MatchCase:=True)
                On Error GoTo 0

                If (saveRange Is Nothing) Then
                    ' sheet does not already exist, create it
                    saveRange = nextFreeColumn()
                Else
                    ' sheet already exists so use it for the saved data
                End If
                xlsSheet.Names.Add(Name:="ResolverData", RefersTo:=saveRange)
                saveRange.Formula = "='" & xlsSheet.Name & "'!A1"
            Else
                'name exists on this sheet
                'does sheet match the range
                If (formulasheet(saveRange) <> xlsApp.ActiveSheet.name) Then
                    'does not match

                    'does sheet already exist in range ?????????????????
                    On Error Resume Next
                    saveRange = xlsApp.Range("'ResolverReservedData'!A1").Rows(1).Find(what:=xlsApp.ActiveSheet.name, lookat:=Excel.XlLookAt.xlWhole, MatchCase:=True)
                    On Error GoTo 0
                    If (saveRange Is Nothing) Then
                        ' sheet does not already exist, create it
                        saveRange = nextFreeColumn()

                    Else
                        'sheet exists so reset name
                    End If
                    saveRange.Formula = "='" & xlsSheet.Name & "'!A1"
                Else
                    'yes does match so ok
                End If
                'reset na,e
                xlsSheet.Names.Add(Name:="ResolverData", RefersTo:=saveRange)
            End If
Savedata:
            If Not (problemRange Is Nothing) Then
                saveRange.Offset(1).Formula = "=" & problemRange.Address(, , , True)
            End If
            problem.save(saveRange.Offset(2))
            xlsSheet.Names.Item("ResolverData").Visible = False
        End If
        On Error GoTo 0
        xlsSheet.Activate()
        Me.Close()
    End Sub
    Public Function formulasheet(ByVal formula As Excel.Range) As String
        If formula.HasFormula Then
            Return xlsApp.Range(Mid(formula.Formula, 2)).Worksheet.Name
        Else
            Return ""
        End If
    End Function
    Public Function nextFreeColumn() As Excel.Range
        Dim col As Long
        Dim datarange As Excel.Range
        Dim worksheet As Excel.Worksheet
        On Error Resume Next
        Dim ws = xlsApp.Worksheets("ResolverReservedData")
        On Error GoTo 0
        If ws Is Nothing Then
            ws = xlsWB.Worksheets.Add
            ws.Name = "ResolverReservedData"
            ws.Visible = Excel.XlSheetVisibility.xlSheetVisible '.xlSheetHidden
        End If
        datarange = xlsApp.Range("'ResolverReservedData'!A1")
        'first clear any errors since they can be overwritten
        On Error Resume Next
        xlsApp.Worksheets("ResolverReservedData").Rows(1).SpecialCells(Excel.XlCellType.xlCellTypeFormulas, Excel.XlSpecialCellsValue.xlErrors).Clear()
        col = -1
        Do
            col = col + 1
        Loop Until datarange.Offset(, col).Formula = ""
        Return datarange.Offset(, col)
    End Function
    Private Sub btnConsolidate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConsolidate.Click
        Dim guesses As Excel.Range = Nothing
        Dim guess As Excel.Range = Nothing
        Dim variable As Excel.Range
        Dim index As Long
        Dim firstVariable As Boolean
        firstVariable = True
        For Each variable In problem.variables.Collection
            If firstVariable Then
                guesses = variable
                firstVariable = False
            Else
                guesses = xlsApp.Union(guesses, variable)
            End If
        Next variable
        problem.variables.displayReset(lbVariables)
        problem.variables.reset()
        For Each guess In guesses.Areas
            On Error GoTo duplicate
            problem.variables.Add(guess)
duplicate:  Resume duplicatehandler
duplicatehandler:
        Next guess
        On Error GoTo 0
        problem.variables.display(lbVariables)
        validateButtons()
    End Sub

    Private Sub btnDeleteConstraint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDeleteConstraint.Click
        Dim intCount As Long
        For intCount = lbConstraints.Items.Count - 1 To 0 Step -1
            If lbConstraints.GetSelected(intCount) Then
                problem.constraints.remove(lbConstraints.Items(intCount))
                lbConstraints.Items.RemoveAt(intCount)
            End If
        Next intCount
        validateButtons()
    End Sub

    Private Sub btnDeleteVariable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDeleteVariable.Click
        Dim intCount As Long
        For intCount = lbVariables.Items.Count - 1 To 0 Step -1
            If lbVariables.GetSelected(intCount) Then
                problem.variables.remove(lbVariables.Items(intCount))
                lbVariables.Items.RemoveAt(intCount)
            End If
        Next intCount
        validateButtons()
    End Sub

    Private Sub btnEditConstraint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEditConstraint.Click
        Dim intCount As Long
        For intCount = lbConstraints.Items.Count - 1 To 0 Step -1
            If lbConstraints.GetSelected(intCount) Then
                currentConstraint = problem.constraints.Collection(lbConstraints.Items(intCount))
                currentConstraint_set()
                editConstraintIndex = intCount
                editConstraintKey = currentConstraint.value(False)
                btnUpdateConstraint.Enabled = True
                btnUpdateConstraint.Visible = True
                btnAddConstraint.Enabled = False
                btnAddConstraint.Visible = False
            End If
        Next intCount
    End Sub
    Private Sub btnEditVariable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEditVariable.Click
        Dim intCount As Long
        For intCount = lbVariables.Items.Count - 1 To 0 Step -1
            If lbVariables.GetSelected(intCount) Then
                currentVariable = problem.variables.Collection(lbVariables.Items(intCount))
                currentVariable_set()
                editVariableIndex = intCount
                editVariableKey = currentVariable.Address
                btnUpdateVariable.Enabled = True
                btnUpdateVariable.Visible = True
                btnAddVariable.Enabled = True
                btnAddVariable.Visible = True
            End If
        Next intCount
    End Sub

    Private Sub btnGuess_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGuess.Click
        Dim guesses As Excel.Range = Nothing
        Dim guess As Excel.Range = Nothing
        Dim constraint As Constraint
        Dim expression As Excel.Range
        Dim precedent As Excel.Range
        Dim index As Long
        Dim firstRange As Boolean
        firstRange = True
        problem.flatten()

        For Each expression In problem.expressions.Collection
            If (expression.HasFormula And expression.HasArray) Or Not (expression.HasFormula) Then
                If firstRange Then
                    guesses = expression
                    firstRange = False
                Else
                    guesses = xlsApp.Union(guesses, expression)
                End If
            End If
        Next expression
        If Not (guesses Is Nothing) Then
            For Each guess In guesses.Areas
                On Error GoTo duplicate
                problem.variables.Add(guess)
                lbVariables.Items.Add(guess.Address)
duplicate:      Resume duplicatehandler
duplicatehandler:
            Next guess
        End If
        On Error GoTo 0
        validateButtons()
    End Sub

    Private Sub btnHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHelp.Click
        Dim p As Diagnostics.ProcessStartInfo = New Diagnostics.ProcessStartInfo()
        p.FileName = getDefaultBrowser()
        p.Arguments = "http://www.inova8.com/joomla/index.php/products/resolver-production-data-reconciliation/reconciling-with-resolver#DefineandSolve"
        Diagnostics.Process.Start(p)
    End Sub
    Private Sub frmProblem_HelpButtonClicked(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.HelpButtonClicked
        MsgBox("Help not yet implemented")
    End Sub
    Private Sub btnOptions_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOptions.Click
        Dim OptionForm = New frmOptions
        OptionForm.ShowDialog()
    End Sub
    Private Sub btnResetAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnResetAll.Click
        If MsgBox("Reset all Resolver options and cell selections?", vbOKCancel) Then
            currentVariable_reset()
            currentConstraint_reset()
            problem.reset()
            problem.displayReset(Me)
        End If
        validateButtons()
    End Sub

    Private Sub btnResolve_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnResolve.Click
        Dim validationMsg As String
        validationMsg = problem.constraints.validate()
        If validationMsg = "" Then
            problem.Resolve()
        Else
            MsgBox("Overlapping constraints error: " & validationMsg)
        End If
    End Sub

    Private Sub btnSaveModel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveModel.Click
        Dim mySelection As Excel.Range
        Me.Hide()
        On Error GoTo NoneSelected
        If Not (problemRange Is Nothing) Then
            problemRange.Select()
            mySelection = xlsApp.InputBox(Prompt:="Select a range of cells", Type:=8, Default:=problemRange.Address)
        Else
            mySelection = xlsApp.InputBox(Prompt:="Select a range of cells", Type:=8)
        End If
        On Error GoTo 0
        problem.save(mySelection)
        problemRange = mySelection
        Me.Show()
        Exit Sub
NoneSelected: Resume nonselectedhandler
nonselectedhandler: On Error GoTo 0
        Me.Show()
    End Sub

    Private Sub btnUpdateConstraint_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdateConstraint.Click
        lbConstraints.Items(editConstraintIndex) = currentConstraint.value(False)
        problem.constraints.remove(editConstraintKey)
        problem.constraints.Add(currentConstraint)
        btnUpdateConstraint.Enabled = False
        btnUpdateConstraint.Visible = False
        btnAddConstraint.Enabled = True
        btnAddConstraint.Visible = True
        currentConstraint_reset()
    End Sub

    Private Sub btnUpdateVariable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdateVariable.Click
        lbVariables.Items(editVariableIndex) = currentVariable.Address
        problem.variables.remove(editVariableKey)
        problem.variables.Add(currentVariable)
        btnUpdateVariable.Enabled = False
        btnUpdateVariable.Visible = False
        btnAddVariable.Enabled = True
        btnAddVariable.Visible = True
        currentVariable_reset()
    End Sub

    Private Sub btnVariable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnVariable.Click
        Dim mySelection As Excel.Range
        Me.Hide()
        If tbVariable.Text <> "" Then xlsApp.Range(tbVariable.Text).Select()
        On Error GoTo NoneSelected
        mySelection = xlsApp.InputBox(Prompt:="Select a range of cells", Type:=8)
        On Error GoTo 0
        Me.Show()
        Me.TopMost = True
        If Not (mySelection Is Nothing) Then
            If tbVariable.Text <> "" Then
                If validateVariable(mySelection) Then
                    currentVariable = mySelection
                    tbVariable.Text = currentVariable.Address
                    btnAddVariable.Enabled = True
                End If
            Else
                currentVariable = mySelection
                tbVariable.Text = currentVariable.Address
                btnAddVariable.Enabled = True
            End If
        Else
            currentVariable = Nothing
            tbVariable.Text = 0
        End If
        Me.Show()
        Me.TopMost = True
        currentVariable_set()
        Exit Sub
NoneSelected: Resume nonselectedhandler
nonselectedhandler: On Error GoTo 0
        Me.Show()
        Me.TopMost = True
        currentVariable_set()
    End Sub

    'Private Sub cbEquality_Change(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbEquality.Click
    '    currentConstraint.equality = cbEquality.Text
    'End Sub

    Private Sub cbEquality_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbEquality.SelectedIndexChanged
        currentConstraint.equality = cbEquality.Text
    End Sub

    Private Sub lbConstraints_Change(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbConstraints.ChangeUICues
        lbConstraints_Click(sender, e)
    End Sub
    Private Sub lbConstraints_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbConstraints.Click
        Dim intCount As Long
        Dim selectedCount As Long
        selectedCount = 0
        For intCount = lbConstraints.Items.Count - 1 To 0 Step -1
            If lbConstraints.GetSelected(intCount) Then selectedCount = selectedCount + 1
        Next intCount
        If selectedCount = 1 Then
            btnEditConstraint.Enabled = True
        Else
            btnEditConstraint.Enabled = False
        End If
        If selectedCount > 0 Then
            btnDeleteConstraint.Enabled = True
        Else
            btnDeleteConstraint.Enabled = False
            btnEditConstraint.Enabled = False
            btnUpdateConstraint.Visible = False
            btnUpdateConstraint.Enabled = False
        End If
    End Sub

    Private Sub btnConstraintReference_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConstraintReference.Click
        Dim mySelection As Excel.Range
        Me.Hide()
        If tbConstraintReference.Text <> "" Then xlsApp.Range(tbConstraintReference.Text).Select()
        On Error GoTo NoneSelected
        mySelection = xlsApp.InputBox(Prompt:="Select a range of cells", Type:=8)
        On Error GoTo 0

        Me.Show()
        Me.TopMost = True
        If Not (mySelection Is Nothing) Then
            If tbReference.Text <> "" Then
                If validateConstraint(currentConstraint.reference, mySelection) Then
                    currentConstraint.constraintreference = mySelection
                    tbConstraintReference.Text = currentConstraint.constraintreference.Address
                    btnAddConstraint.Enabled = True
                End If
            Else
                currentConstraint.constraintreference = mySelection
                tbConstraintReference.Text = currentConstraint.constraintreference.Address
                btnAddConstraint.Enabled = False
            End If
        Else
            tbConstraintReference.Text = ""
            btnAddConstraint.Enabled = False
        End If
        Exit Sub
        Me.Show()
        Me.TopMost = True
        currentConstraint_set()
NoneSelected: Resume nonselectedhandler
nonselectedhandler: On Error GoTo 0
        Me.Show()
        Me.TopMost = True
        currentConstraint_set()
    End Sub

    Private Sub btnReference_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReference.Click
        Dim mySelection As Excel.Range
        Me.Show()
        If tbReference.Text <> "" Then xlsApp.Range(tbReference.Text).Select()
        On Error GoTo NoneSelected
        mySelection = xlsApp.InputBox(Prompt:="Select a range of cells", Type:=8)
        On Error GoTo 0
        Me.Show()
        Me.TopMost = True
        If Not (mySelection Is Nothing) Then
            If tbConstraintReference.Text <> "" Then
                If validateConstraint(mySelection, currentConstraint.constraintreference) Then
                    currentConstraint.reference = mySelection
                    tbReference.Text = currentConstraint.reference.Address
                    btnAddConstraint.Enabled = True
                End If
            Else
                currentConstraint.reference = mySelection
                tbReference.Text = currentConstraint.reference.Address
                btnAddConstraint.Enabled = False
            End If
        Else
            tbReference.Text = ""
            btnAddConstraint.Enabled = False
        End If
        Me.Show()
        Me.TopMost = True
        Exit Sub
NoneSelected: Resume nonselectedhandler
nonselectedhandler: On Error GoTo 0
        Me.Show()
        Me.TopMost = True
        tbReference.Text = currentConstraint.reference.Address
        btnAddConstraint.Enabled = False
    End Sub
    Private Function validateConstraint(ByVal reference As Excel.Range, ByVal constraintreference As Excel.Range) As Boolean
        If Not (constraintreference Is Nothing) Then
            If (reference.Columns.Count = constraintreference.Columns.Count) And (reference.Rows.Count = constraintreference.Rows.Count) Then
                validateConstraint = True
            Else
                MsgBox("The cell reference and constraint ranges must be the same size and shape")
                validateConstraint = False
            End If
        Else
            validateConstraint = True
        End If
    End Function
    Private Function validateVariable(ByVal variable As Excel.Range) As Boolean
        validateVariable = True
    End Function
    Private Sub currentConstraint_set()
        If currentConstraint.reference Is Nothing Then
            tbReference.Text = ""
        Else
            tbReference.Text = currentConstraint.reference.Address
        End If
        cbEquality.Text = currentConstraint.equality
        tbConstraintReference.Text = currentConstraint.constraintreference.Address
    End Sub
    Private Sub currentConstraint_get()
        currentConstraint.reference = xlsApp.Range(tbReference.Text)
        currentConstraint.equality = cbEquality.Text
        currentConstraint.constraintreference = xlsApp.Range(tbConstraintReference.Text)
    End Sub
    Private Sub currentConstraint_reset()
        'currentConstraint.reference = Nothing
        'currentConstraint.equality = "="
        'currentConstraint.constraintreference = Nothing
        tbReference.Text = ""
        'cbEquality.Text = "="
        tbConstraintReference.Text = ""
    End Sub
    Private Sub currentVariable_set()
        If currentVariable Is Nothing Then
            tbVariable.Text = ""
        Else
            tbVariable.Text = currentVariable.Address
        End If
    End Sub
    Private Function currentVariable_get()
        Return xlsApp.Range(tbReference.Text)
    End Function
    Private Sub currentVariable_reset()
        currentVariable = Nothing
        tbVariable.Text = ""
    End Sub
    Private Sub lbVariables_Change(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbVariables.ChangeUICues

        lbVariables_Click(sender, e)
    End Sub
    Private Sub lbVariables_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lbVariables.Click
        Dim intCount As Long
        Dim selectedCount As Long
        selectedCount = 0
        For intCount = lbVariables.Items.Count - 1 To 0 Step -1
            If lbVariables.GetSelected(intCount) Then selectedCount = selectedCount + 1
        Next intCount
        If selectedCount = 0 Then
            btnUpdateVariable.Enabled = False
            btnUpdateVariable.Visible = False
        End If
        If selectedCount = 1 Then
            btnEditVariable.Enabled = True
        Else
            btnEditVariable.Enabled = False
        End If
        If selectedCount > 0 Then
            btnDeleteVariable.Enabled = True
        Else
            btnDeleteVariable.Enabled = False
            btnEditVariable.Enabled = False
            btnUpdateVariable.Visible = False
            btnUpdateVariable.Enabled = False
        End If
    End Sub


    'Private Sub UserForm_QueryClose(ByVal Cancel As Integer, ByVal CloseMode As Integer)
    '    btnClose_Click(sender, e)
    'End Sub


    Private Sub btnLoadModel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLoadModel.Click
        Dim mySelection As Excel.Range
        Dim newProblem As ProblemClass
        Me.Hide()
        'Me.Left = 5000
        On Error GoTo NoneSelected
        If Not (problemRange Is Nothing) Then
            problemRange.Select()
            mySelection = xlsApp.InputBox(Prompt:="Select a range of cells", Type:=8, Default:=problemRange.Address)
        Else
            mySelection = xlsApp.InputBox(Prompt:="Select a range of cells", Type:=8)
        End If
        On Error GoTo 0
        newProblem = New ProblemClass
        If newProblem.load(mySelection.Cells) Then
            problem = newProblem
            problem.displayReset(Me)
            problem.display(Me)
            problemRange = mySelection
        Else
            'Loading problem
        End If
        Me.Show()
        validateButtons()
        Exit Sub
NoneSelected: Resume nonselectedhandler
nonselectedhandler: On Error GoTo 0
        Me.Show()
        validateButtons()

    End Sub
    Private Sub UserForm_Activate()
        Me.validateButtons()
    End Sub
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        currentConstraint = New Constraint
        currentConstraint_reset()
        currentVariable_reset()
        Me.validateButtons()

    End Sub

    Private Sub btnRegister_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRegister.Click
        Dim register = New com.inova8.resolver.frmRegister(ResolverLicense)
        register.ShowDialog()
        If ResolverLicense.licenseInformation.type = com.inova8.resolver.LicenserClass.LicenseType.Trial Then
            CType(problem.UserForm.Controls.Find("btnRegister", True).First, Windows.Forms.Button).Enabled = True
        Else
            CType(problem.UserForm.Controls.Find("btnRegister", True).First, Windows.Forms.Button).Enabled = False
        End If
    End Sub
End Class