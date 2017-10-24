<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmResults
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmResults))
        Me.lblResults = New System.Windows.Forms.Label()
        Me.btnRetain = New System.Windows.Forms.RadioButton()
        Me.btnRevert = New System.Windows.Forms.RadioButton()
        Me.btnAnswerReport = New System.Windows.Forms.CheckBox()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnSaveScenario = New System.Windows.Forms.Button()
        Me.btnHelp = New System.Windows.Forms.Button()
        Me.btnVariableSensitivityReport = New System.Windows.Forms.CheckBox()
        Me.btnConstraintSensitivityReport = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'lblResults
        '
        Me.lblResults.AutoSize = True
        Me.lblResults.Location = New System.Drawing.Point(8, 9)
        Me.lblResults.Name = "lblResults"
        Me.lblResults.Size = New System.Drawing.Size(378, 13)
        Me.lblResults.TabIndex = 0
        Me.lblResults.Text = "Resolver found a solution. All constraints and optimality conditions are satisfie" &
    "d."
        '
        'btnRetain
        '
        Me.btnRetain.AutoSize = True
        Me.btnRetain.Location = New System.Drawing.Point(37, 35)
        Me.btnRetain.Name = "btnRetain"
        Me.btnRetain.Size = New System.Drawing.Size(134, 17)
        Me.btnRetain.TabIndex = 1
        Me.btnRetain.TabStop = True
        Me.btnRetain.Text = "Keep Resolver solution"
        Me.btnRetain.UseVisualStyleBackColor = True
        '
        'btnRevert
        '
        Me.btnRevert.AutoSize = True
        Me.btnRevert.Location = New System.Drawing.Point(37, 72)
        Me.btnRevert.Name = "btnRevert"
        Me.btnRevert.Size = New System.Drawing.Size(139, 17)
        Me.btnRevert.TabIndex = 2
        Me.btnRevert.TabStop = True
        Me.btnRevert.Text = "Revert to original values"
        Me.btnRevert.UseVisualStyleBackColor = True
        '
        'btnAnswerReport
        '
        Me.btnAnswerReport.AutoSize = True
        Me.btnAnswerReport.Checked = True
        Me.btnAnswerReport.CheckState = System.Windows.Forms.CheckState.Checked
        Me.btnAnswerReport.Location = New System.Drawing.Point(228, 36)
        Me.btnAnswerReport.Name = "btnAnswerReport"
        Me.btnAnswerReport.Size = New System.Drawing.Size(96, 17)
        Me.btnAnswerReport.TabIndex = 3
        Me.btnAnswerReport.Text = "Answer Report"
        Me.btnAnswerReport.UseVisualStyleBackColor = True
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(11, 108)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(88, 30)
        Me.btnOK.TabIndex = 4
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(114, 108)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(88, 30)
        Me.btnCancel.TabIndex = 5
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnSaveScenario
        '
        Me.btnSaveScenario.Location = New System.Drawing.Point(217, 108)
        Me.btnSaveScenario.Name = "btnSaveScenario"
        Me.btnSaveScenario.Size = New System.Drawing.Size(88, 30)
        Me.btnSaveScenario.TabIndex = 6
        Me.btnSaveScenario.Text = "Save Scenario"
        Me.btnSaveScenario.UseVisualStyleBackColor = True
        '
        'btnHelp
        '
        Me.btnHelp.Location = New System.Drawing.Point(320, 108)
        Me.btnHelp.Name = "btnHelp"
        Me.btnHelp.Size = New System.Drawing.Size(88, 30)
        Me.btnHelp.TabIndex = 7
        Me.btnHelp.Text = "Help"
        Me.btnHelp.UseVisualStyleBackColor = True
        '
        'btnVariableSensitivityReport
        '
        Me.btnVariableSensitivityReport.AutoSize = True
        Me.btnVariableSensitivityReport.Checked = True
        Me.btnVariableSensitivityReport.CheckState = System.Windows.Forms.CheckState.Checked
        Me.btnVariableSensitivityReport.Location = New System.Drawing.Point(228, 59)
        Me.btnVariableSensitivityReport.Name = "btnVariableSensitivityReport"
        Me.btnVariableSensitivityReport.Size = New System.Drawing.Size(146, 17)
        Me.btnVariableSensitivityReport.TabIndex = 25
        Me.btnVariableSensitivityReport.Text = "VariableSensitivity Report"
        Me.btnVariableSensitivityReport.UseVisualStyleBackColor = True
        '
        'btnConstraintSensitivityReport
        '
        Me.btnConstraintSensitivityReport.AutoSize = True
        Me.btnConstraintSensitivityReport.Location = New System.Drawing.Point(228, 83)
        Me.btnConstraintSensitivityReport.Name = "btnConstraintSensitivityReport"
        Me.btnConstraintSensitivityReport.Size = New System.Drawing.Size(158, 17)
        Me.btnConstraintSensitivityReport.TabIndex = 26
        Me.btnConstraintSensitivityReport.Text = "Constraint Sensitivity Report"
        Me.btnConstraintSensitivityReport.UseVisualStyleBackColor = True
        '
        'frmResults
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(421, 144)
        Me.Controls.Add(Me.btnConstraintSensitivityReport)
        Me.Controls.Add(Me.btnVariableSensitivityReport)
        Me.Controls.Add(Me.btnHelp)
        Me.Controls.Add(Me.btnSaveScenario)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.btnAnswerReport)
        Me.Controls.Add(Me.btnRevert)
        Me.Controls.Add(Me.btnRetain)
        Me.Controls.Add(Me.lblResults)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmResults"
        Me.Text = "Resolver Results"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblResults As System.Windows.Forms.Label
    Friend WithEvents btnRetain As System.Windows.Forms.RadioButton
    Friend WithEvents btnRevert As System.Windows.Forms.RadioButton
    Friend WithEvents btnAnswerReport As System.Windows.Forms.CheckBox
    Friend WithEvents btnOK As System.Windows.Forms.Button
    Friend WithEvents btnCancel As System.Windows.Forms.Button
    Friend WithEvents btnSaveScenario As System.Windows.Forms.Button
    Friend WithEvents btnHelp As System.Windows.Forms.Button
    Friend WithEvents btnVariableSensitivityReport As System.Windows.Forms.CheckBox
    Friend WithEvents btnConstraintSensitivityReport As Windows.Forms.CheckBox
End Class
