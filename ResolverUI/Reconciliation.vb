Module Reconciliation
    Public xlsApp As Excel.Application
    Public xlsWB As Excel.Workbook
    Public xlsSheet As Excel.Worksheet
    Public xlsCell As Excel.Range
    Public problem As ProblemClass
    Public problemRange As Excel.Range
    Public ResolverLicense As com.inova8.resolver.LicenserClass = New com.inova8.resolver.LicenserClass
    Private checked As Boolean = False
    Private maxVariables As Int16 = 20
    Public Sub Resolve()
        Dim saveRange As Excel.Range
        problem = New ProblemClass
        If Not checked Then
            If ResolverLicense.handleLicense() Then
                checked = True
            Else
                checked = False
                MsgBox("Resolver is unlicensed. You will be limited to problems defined in " & com.inova8.resolver.LicenserClass.maxCells & " spreadsheet cells")
                'Exit Sub
            End If
        End If
        If ResolverLicense.licenseInformation.type = com.inova8.resolver.LicenserClass.LicenseType.Trial Then
            CType(problem.UserForm.Controls.Find("btnRegister", True).First, Windows.Forms.Button).Enabled = True
        Else
            CType(problem.UserForm.Controls.Find("btnRegister", True).First, Windows.Forms.Button).Enabled = False
        End If

        xlsApp = Globals.ThisAddIn.Application 'GetObject(, "Excel.Application")
        xlsWB = Globals.ThisAddIn.Application.ActiveWorkbook
        xlsSheet = Globals.ThisAddIn.Application.ActiveSheet
        On Error Resume Next
        saveRange = xlsApp.Range(xlsApp.ActiveSheet.names("ResolverData").RefersTo)
        If Not (saveRange Is Nothing) Then
            If problem.UserForm.formulasheet(saveRange) = xlsApp.ActiveSheet.name Then
                If saveRange.Offset(1).Value <> "" Then
                    On Error Resume Next
                    problemRange = xlsApp.Range(Mid(saveRange.Offset(1).Formula, 2))
                    On Error GoTo 0
                End If
                problem.reset()
                If problem.load(saveRange.Offset(2)) Then
                    problem.displayReset(problem.UserForm)
                    problem.display(problem.UserForm)
                End If
                'saveRange.value = xlsApp.ActiveSheet.name
            Else
                problemRange = Nothing
                problem.reset()
                problem.displayReset(problem.UserForm)
            End If
        Else
            problemRange = Nothing
            problem.reset()
            problem.displayReset(problem.UserForm)
        End If
        problem.UserForm.validateButtons()
        problem.UserForm.Show()
        On Error GoTo 0
    End Sub
    Public Function getDefaultBrowser() As String
        Dim browser As String = String.Empty
        Dim key As Microsoft.Win32.RegistryKey = Nothing
        Try
            key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey("HTTP\shell\open\command", False)

            'trim off quotes
            browser = key.GetValue(Nothing).ToString().ToLower().Replace("""", "")
            If Not browser.EndsWith("exe") Then
                'get rid of everything after the ".exe"
                browser = browser.Substring(0, browser.LastIndexOf(".exe") + 4)
            End If
        Finally
            If key IsNot Nothing Then
                key.Close()
            End If
        End Try
        Return browser
    End Function
    'Public Function Gaussian(ByVal ParamArray valueArray() As Object) As Double
    '    Dim index As Long
    '    Dim estimaterange As Excel.Range
    '    Dim estimate As Double
    '    Dim measurement As Double
    '    Dim tolerance As Double
    '    Dim arg As Long
    '    Dim values As Excel.Range
    '    Dim gauss As Double = 0.0#
    '    For arg = LBound(valueArray) To UBound(valueArray)
    '        If IsObject(valueArray(arg)) Then
    '            values = valueArray(arg)
    '            For index = 1 To values.Count
    '                If IsNumeric(values(index).formula) Then
    '                    estimaterange = values(index)
    '                Else
    '                    estimaterange = xlsSheet.Range(values(index).formula)
    '                End If
    '                estimate = estimaterange.Columns(1)
    '                tolerance = estimaterange.Columns(2)
    '                measurement = estimaterange.Columns(3)
    '                If tolerance > 0 Then gauss = gauss + ((measurement - estimate) / tolerance) ^ 2
    '            Next index
    '        End If
    '    Next arg
    '    Return gauss
    'End Function
    'Public Function Lorentzian(ByVal ParamArray valueArray() As Object) As Double
    '    Dim index As Long
    '    Dim estimaterange As Excel.Range
    '    Dim estimate As Double
    '    Dim measurement As Double
    '    Dim tolerance As Double
    '    Dim arg As Long
    '    Dim values As Excel.Range
    '    Dim Loren As Double = 0.0#
    '    For arg = LBound(valueArray) To UBound(valueArray)
    '        If IsObject(valueArray(arg)) Then
    '            values = valueArray(arg)
    '            For index = 1 To values.Count
    '                If IsNumeric(values(index).formula) Then
    '                    estimaterange = values(index)
    '                Else
    '                    estimaterange = xlsSheet.Range(values(index).formula)
    '                End If
    '                estimate = estimaterange.Columns(1)
    '                tolerance = estimaterange.Columns(2)
    '                measurement = estimaterange.Columns(3)
    '                If tolerance > 0 Then Loren = Loren + 1 / ((((measurement - estimate) / tolerance) ^ 2) / 2 + 1)
    '            Next index
    '        End If
    '    Next arg
    '    Return Loren
    'End Function
    'Public Function Fair(ByVal c As Double, ByVal ParamArray valueArray() As Object) As Double
    '    Dim index As Long
    '    Dim estimaterange As Excel.Range
    '    Dim estimate As Double
    '    Dim measurement As Double
    '    Dim tolerance As Double
    '    Dim measurementerror As Double
    '    Dim arg As Long
    '    Dim values As Excel.Range
    '    Dim Fa As Double = 0.0#
    '    For arg = LBound(valueArray) To UBound(valueArray)
    '        If IsObject(valueArray(arg)) Then
    '            values = valueArray(arg)
    '            For index = 1 To values.Count
    '                If IsNumeric(values(index).formula) Then
    '                    estimaterange = values(index)
    '                Else
    '                    estimaterange = xlsSheet.Range(values(index).formula)
    '                End If
    '                estimate = estimaterange.Columns(1)
    '                tolerance = estimaterange.Columns(2)
    '                measurement = estimaterange.Columns(3)
    '                If tolerance > 0 Then
    '                    measurementerror = (measurement - estimate) / tolerance
    '                    Fa = Fa + c ^ 2 * (Math.Abs(measurementerror) / c - Math.Log(1 + Math.Abs(measurementerror) / c))
    '                End If
    '            Next index
    '        End If
    '    Next arg
    '    Return Fa
    'End Function
    'Public Function ContaminatedGaussian(ByVal b As Double, ByVal nu As Double, ByVal ParamArray valueArray() As Object) As Double
    '    Dim index As Long
    '    Dim estimaterange As Excel.Range
    '    Dim estimate As Double
    '    Dim measurement As Double
    '    Dim tolerance As Double
    '    Dim measurementerror As Double
    '    Dim arg As Long
    '    Dim values As Excel.Range
    '    Dim val As Double
    '    Const pi = 3.14159265358979
    '    Dim CG As Double = 0.0#
    '    For arg = LBound(valueArray) To UBound(valueArray)
    '        If IsObject(valueArray(arg)) Then
    '            values = valueArray(arg)
    '            For index = 1 To values.Count
    '                If IsNumeric(values(index).formula) Then
    '                    estimaterange = values(index)
    '                Else
    '                    estimaterange = xlsSheet.Range(values(index).formula)
    '                End If
    '                estimate = estimaterange.Columns(1)
    '                tolerance = estimaterange.Columns(2)
    '                measurement = estimaterange.Columns(3)
    '                If tolerance > 0 Then
    '                    measurementerror = (measurement - estimate) / tolerance
    '                    val = (1 - nu) * Math.Exp(-measurementerror ^ 2 / 2) + nu * Math.Exp(-(measurementerror / b) ^ 2 / 2) / b
    '                    CG = CG - Math.Log(val) + Math.Log(tolerance * Math.Sqrt(2 * pi))
    '                End If
    '            Next index
    '        End If
    '    Next arg
    '    Return CG
    'End Function
    'Public Function ErrorCriticalLevel(ByVal degreesFreedom As Long, ByVal confidence As Double) As Double
    '    Dim beta As Double
    '    beta = 1 - (1 - confidence) ^ (1 / degreesFreedom)
    '    Return NormSInv(1 - beta / 2)

    'End Function
    'Private Function IsObject(ByVal arg As Object) As Boolean
    '    If Not (arg Is Nothing) Then
    '        Return True
    '    Else
    '        Return False
    '    End If
    'End Function
    'Public Function NormSInv(ByVal p As Double) As Double
    '    Const a1 = -39.6968302866538, a2 = 220.946098424521, a3 = -275.928510446969
    '    Const a4 = 138.357751867269, a5 = -30.6647980661472, a6 = 2.50662827745924
    '    Const b1 = -54.4760987982241, b2 = 161.585836858041, b3 = -155.698979859887
    '    Const b4 = 66.8013118877197, b5 = -13.2806815528857, c1 = -0.00778489400243029
    '    Const c2 = -0.322396458041136, c3 = -2.40075827716184, c4 = -2.54973253934373
    '    Const c5 = 4.37466414146497, c6 = 2.93816398269878, d1 = 0.00778469570904146
    '    Const d2 = 0.32246712907004, d3 = 2.445134137143, d4 = 3.75440866190742
    '    Const p_low = 0.02425, p_high = 1 - p_low
    '    Dim q As Double, r As Double, pNormSInv As Double
    '    If p < 0 Or p > 1 Then
    '        Err.Raise(vbObjectError, , "NormSInv: Argument out of range.")
    '    ElseIf p < p_low Then
    '        q = Math.Sqrt(-2 * Math.Log(p))
    '        pNormSInv = (((((c1 * q + c2) * q + c3) * q + c4) * q + c5) * q + c6) / _
    '           ((((d1 * q + d2) * q + d3) * q + d4) * q + 1)
    '    ElseIf p <= p_high Then
    '        q = p - 0.5 : r = q * q
    '        pNormSInv = (((((a1 * r + a2) * r + a3) * r + a4) * r + a5) * r + a6) * q / _
    '           (((((b1 * r + b2) * r + b3) * r + b4) * r + b5) * r + 1)
    '    Else
    '        q = Math.Sqrt(-2 * Math.Log(1 - p))
    '        pNormSInv = -(((((c1 * q + c2) * q + c3) * q + c4) * q + c5) * q + c6) / _
    '           ((((d1 * q + d2) * q + d3) * q + d4) * q + 1)
    '    End If
    '    Return pNormSInv
    'End Function
End Module
