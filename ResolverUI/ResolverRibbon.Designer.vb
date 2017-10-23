Partial Class ResolverRibbon
    Inherits Microsoft.Office.Tools.Ribbon.RibbonBase

    <System.Diagnostics.DebuggerNonUserCode()> _
    Public Sub New(ByVal container As System.ComponentModel.IContainer)
        MyClass.New()

        'Required for Windows.Forms Class Composition Designer support
        If (container IsNot Nothing) Then
            container.Add(Me)
        End If

    End Sub

    <System.Diagnostics.DebuggerNonUserCode()> _
    Public Sub New()
        MyBase.New(Globals.Factory.GetRibbonFactory())

        'This call is required by the Component Designer.
        InitializeComponent()

    End Sub

    'Component overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Tab1 = Me.Factory.CreateRibbonTab
        Me.ResolverGroup = Me.Factory.CreateRibbonGroup
        Me.btnResolve = Me.Factory.CreateRibbonButton
        Me.Tab1.SuspendLayout()
        Me.ResolverGroup.SuspendLayout()
        '
        'Tab1
        '
        Me.Tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office
        Me.Tab1.ControlId.OfficeId = "TabData"
        Me.Tab1.Groups.Add(Me.ResolverGroup)
        Me.Tab1.Label = "TabData"
        Me.Tab1.Name = "Tab1"
        '
        'ResolverGroup
        '
        Me.ResolverGroup.Items.Add(Me.btnResolve)
        Me.ResolverGroup.Label = "Resolver"
        Me.ResolverGroup.Name = "ResolverGroup"
        '
        'btnResolve
        '
        Me.btnResolve.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge
        Me.btnResolve.Image = Global.ResolverUI.My.Resources.Resources.inova8
        Me.btnResolve.ImageName = "inova8"
        Me.btnResolve.Label = "Resolve"
        Me.btnResolve.Name = "btnResolve"
        Me.btnResolve.ShowImage = True
        '
        'ResolverRibbon
        '
        Me.Name = "ResolverRibbon"
        Me.RibbonType = "Microsoft.Excel.Workbook"
        Me.Tabs.Add(Me.Tab1)
        Me.Tab1.ResumeLayout(False)
        Me.Tab1.PerformLayout()
        Me.ResolverGroup.ResumeLayout(False)
        Me.ResolverGroup.PerformLayout()

    End Sub

    Friend WithEvents Tab1 As Microsoft.Office.Tools.Ribbon.RibbonTab
    Friend WithEvents ResolverGroup As Microsoft.Office.Tools.Ribbon.RibbonGroup
    Friend WithEvents btnResolve As Microsoft.Office.Tools.Ribbon.RibbonButton
End Class

Partial Class ThisRibbonCollection

    <System.Diagnostics.DebuggerNonUserCode()> _
    Friend ReadOnly Property Ribbon1() As ResolverRibbon
        Get
            Return Me.GetRibbon(Of ResolverRibbon)()
        End Get
    End Property
End Class
