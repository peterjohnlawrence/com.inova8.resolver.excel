Imports Microsoft.Office.Tools.Ribbon

Public Class ResolverRibbon

    Private Sub ResolverRibbon_Load(ByVal sender As System.Object, ByVal e As RibbonUIEventArgs) Handles MyBase.Load

    End Sub

    Private Sub btnResolve_Click(ByVal sender As System.Object, ByVal e As Microsoft.Office.Tools.Ribbon.RibbonControlEventArgs) Handles btnResolve.Click
        Reconciliation.Resolve()
    End Sub
End Class
