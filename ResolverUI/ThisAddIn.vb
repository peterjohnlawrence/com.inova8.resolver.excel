Public Class ThisAddIn
    Private MainMenuBar As Office.CommandBar
    Private MenuBarItem As Office.CommandBarControl
    Private WithEvents MenuItem As Office.CommandBarButton
    Private Worksheet As Worksheet
    Private Const strAddInName As String = "Resolver"
    Private Const strMenuName As String = "&Resolver"
    Private Sub ThisAddIn_Startup() Handles Me.Startup
        If Application.Version = "11.0" Then AddMenuItems()

        'AddUDFToCustomCategory()

        On Error GoTo 0
    End Sub

    Private Sub ThisAddIn_Shutdown() Handles Me.Shutdown
        If Application.Version = "11.0" Then Call DelMenuItems()

        On Error Resume Next 'In case it has already gone.
        Application.CommandBars("Worksheet Menu Bar").Controls("Resolver").Delete()
        xlsApp.ActiveWorkbook.Worksheets("ResolverReservedData").Delete()
        On Error GoTo 0
    End Sub
    Private Sub AddMenuItems()

        Dim objMainMenuBar As Microsoft.Office.Core.CommandBar
        Dim objCustomMenu As Microsoft.Office.Core.CommandBarControl

        Call DelMenuItems()

        objMainMenuBar = Application.CommandBars("Worksheet Menu Bar")

        objCustomMenu = objMainMenuBar.Controls.Add(Type:=Office.MsoControlType.msoControlPopup)

        objCustomMenu.Caption = strMenuName

        With objCustomMenu.Controls.Add(Type:=Office.MsoControlType.msoControlButton)
            .Caption = "&Resolver"
            .OnAction = "Resolve"
            .TooltipText = "Open Resolver configuration"
            .FaceId = 37
        End With

    End Sub
    Private Sub DelMenuItems()
        On Error Resume Next
        Application.CommandBars("Worksheet Menu Bar").Controls(strMenuName).Delete()
        On Error GoTo 0
    End Sub
    Private Sub AddUDFToCustomCategory()
        Const rangedesc = "The range(s) contain the estimate (result), tolerance and the measurement respectively. The range can contain arrays otherwise values are assumed in adjacent cells"
        Application.MacroOptions(Macro:="Reconciliation.Gaussian", _
            Description:="Calculates the sum of (weighted error squared) objective function of the supplied range(s)." + rangedesc, _
            Category:="Reconciliation")
        Application.MacroOptions(Macro:="Reconciliation.Lorentzian", _
            Description:="Calculates the Lorentzian objective function of the supplied range(s)." + rangedesc, _
            Category:="Reconciliation")
        Application.MacroOptions(Macro:="Reconciliation.Fair", _
            Description:="Calculates the Fair objective function of the supplied range(s). The first parameter is the 'tuning' parameter, suggested to be ~10.", _
            Category:="Reconciliation")
        Application.MacroOptions(Macro:="Reconciliation.ContaminatedGaussian", _
            Description:="Calculates the Tjoa-Biegler contaminated gaussian objective function of the supplied range(s). Param b shapes the distribution, usually 10~20 reducing the effect of gross errors 100~400 fold. Param nu is the prior probablity that a gross error occurs.", _
            Category:="Reconciliation")
        Application.MacroOptions(Macro:="Reconciliation.ErrorCriticalLevel", _
            Description:="Calculates error (global, constraint, or measurement) given the degrees of freedom (number of constraints or measurements, and confidence level required ~ 0.05, or 5%.", _
            Category:="Reconciliation")
    End Sub
End Class
