Public Class frmOptions
    Private Sub cbCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbCancel.Click
        'Unload(Me)
        Me.Close()
    End Sub
    Private Sub cbHelp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbHelp.Click
        Dim p As Diagnostics.ProcessStartInfo = New Diagnostics.ProcessStartInfo()
        p.FileName = getDefaultBrowser()
        p.Arguments = "http://www.inova8.com/joomla/index.php/products/resolver-production-data-reconciliation/reconciling-with-resolver#SolverOptions"
        Diagnostics.Process.Start(p)
    End Sub
    Private Sub frmOptions_HelpButtonClicked(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.HelpButtonClicked
        MsgBox("Help not yet implemented")
    End Sub
    Private Sub cbOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbOK.Click
        problem.options.maxTime = tbMaxTime.Text '.value
        problem.options.iterations = tbIterations.Text '.value
        problem.options.precision = tbPrecision.Text '.value
        problem.options.convergence = tbConvergence.Text '.value
        problem.options.assumeNonNegative = cbAssumeNonNegative.CheckState
        problem.options.initializeValues = cbInitializevalues.CheckState '.value
        'Unload(Me)
        Me.Close()
    End Sub
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        tbMaxTime.Text = problem.options.maxTime
        tbIterations.Text = problem.options.iterations
        tbPrecision.Text = problem.options.precision
        'tblinearModel = options.linearModel
        'tbshowResults = options.showResults
        'tbautomaticScaling = options.automaticScaling
        'tbestimateMethod = options.estimateMethod
        'tbderivativeMethod = options.derivativeMethod
        'tbsearchMethod = options.searchMethod
        tbConvergence.Text = problem.options.convergence
        cbAssumeNonNegative.Checked = problem.options.assumeNonNegative
        cbInitializevalues.Checked = problem.options.initializeValues

    End Sub

End Class