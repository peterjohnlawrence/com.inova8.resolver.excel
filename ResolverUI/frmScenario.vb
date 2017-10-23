Public Class frmScenario
    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub
    Private Sub btnHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHelp.Click
        MsgBox("Scenario help not implemented ... yet!")
    End Sub

    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click

        On Error GoTo NoScenario

        If xlsApp.ActiveSheet.Scenarios(tbScenario.Text).name <> "" Then
            If MsgBox("Scenario already exists: do you wish to overwrite?", vbYesNo) = vbNo Then
                btnOK.Enabled = False
                Exit Sub
            Else
                xlsApp.ActiveSheet.Scenarios(tbScenario.Text).Delete()
            End If
        End If
NoScenario:
        Resume complete
complete: On Error GoTo 0
        xlsApp.ActiveSheet.Scenarios.Add(Name:=tbScenario.Text, _
            ChangingCells:=xlsApp.Range(problemRange.Cells(1, 1), problemRange.Cells(1, 1).Offset(3 + problem.constraints.count, 0)))

        'ChangingCells:=Application.Union( _
        '    Application.range(problemRange.Cells(1, 1), problemRange.Cells(1, 1).Offset(3 + problem.constraints.count, 0)), _
        '    Application.range(problem.variables.value).Precedents)
        Me.Close()
    End Sub
    Private Sub tbScenario_Change(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbScenario.ChangeUICues
        If tbScenario.Text <> "" Then
            btnOK.Enabled = True
        End If
    End Sub

End Class