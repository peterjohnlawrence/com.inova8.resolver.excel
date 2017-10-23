Public Class frmResults
    Public ok As Boolean
    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCancel.Click
        ok = False
        Me.Close()
    End Sub

    Private Sub btnHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnHelp.Click
        Dim p As Diagnostics.ProcessStartInfo = New Diagnostics.ProcessStartInfo()
        p.FileName = getDefaultBrowser()
        p.Arguments = "http://www.inova8.com/joomla/index.php/products/resolver-production-data-reconciliation/reconciling-with-resolver#Results"
        Diagnostics.Process.Start(p)
    End Sub
    Private Sub frmResults_HelpButtonClicked(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.HelpButtonClicked
        MsgBox("Help not yet implemented ... yet!")
    End Sub
    Private Sub btnOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnOK.Click
        ok = True
        Me.Close()
    End Sub
    Private Sub btnRetain_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRetain.Click
        btnRevert.Checked = False
    End Sub

    Private Sub btnRevert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRevert.Click
        btnRetain.Checked = False
    End Sub

    Private Sub btnSaveScenario_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveScenario.Click
        Dim Scenario = New frmScenario
        Scenario.ShowDialog()
    End Sub


    Private Sub UserForm_QueryClose(ByVal Cancel As Integer, ByVal CloseMode As Integer)
        ok = False
    End Sub

 
End Class