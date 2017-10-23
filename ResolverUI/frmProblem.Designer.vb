<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmProblem
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmProblem))
        Me.lbConstraints = New System.Windows.Forms.ListBox()
        Me.gbConstraints = New System.Windows.Forms.GroupBox()
        Me.gbConstraint = New System.Windows.Forms.GroupBox()
        Me.btnAddConstraint = New System.Windows.Forms.Button()
        Me.btnUpdateConstraint = New System.Windows.Forms.Button()
        Me.cbEquality = New System.Windows.Forms.ComboBox()
        Me.btnConstraintReference = New System.Windows.Forms.Button()
        Me.btnReference = New System.Windows.Forms.Button()
        Me.tbConstraintReference = New System.Windows.Forms.TextBox()
        Me.tbReference = New System.Windows.Forms.TextBox()
        Me.btnDeleteConstraint = New System.Windows.Forms.Button()
        Me.btnEditConstraint = New System.Windows.Forms.Button()
        Me.gbVariables = New System.Windows.Forms.GroupBox()
        Me.btnConsolidate = New System.Windows.Forms.Button()
        Me.btnGuess = New System.Windows.Forms.Button()
        Me.btnDeleteVariable = New System.Windows.Forms.Button()
        Me.btnEditVariable = New System.Windows.Forms.Button()
        Me.gbVariable = New System.Windows.Forms.GroupBox()
        Me.btnAddVariable = New System.Windows.Forms.Button()
        Me.btnUpdateVariable = New System.Windows.Forms.Button()
        Me.btnVariable = New System.Windows.Forms.Button()
        Me.tbVariable = New System.Windows.Forms.TextBox()
        Me.lbVariables = New System.Windows.Forms.ListBox()
        Me.btnResolve = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnOptions = New System.Windows.Forms.Button()
        Me.btnLoadModel = New System.Windows.Forms.Button()
        Me.btnSaveModel = New System.Windows.Forms.Button()
        Me.btnResetAll = New System.Windows.Forms.Button()
        Me.btnHelp = New System.Windows.Forms.Button()
        Me.btnRegister = New System.Windows.Forms.Button()
        Me.gbConstraints.SuspendLayout()
        Me.gbConstraint.SuspendLayout()
        Me.gbVariables.SuspendLayout()
        Me.gbVariable.SuspendLayout()
        Me.SuspendLayout()
        '
        'lbConstraints
        '
        Me.lbConstraints.FormattingEnabled = True
        Me.lbConstraints.Location = New System.Drawing.Point(13, 24)
        Me.lbConstraints.Name = "lbConstraints"
        Me.lbConstraints.Size = New System.Drawing.Size(223, 238)
        Me.lbConstraints.TabIndex = 0
        '
        'gbConstraints
        '
        Me.gbConstraints.Controls.Add(Me.gbConstraint)
        Me.gbConstraints.Controls.Add(Me.btnDeleteConstraint)
        Me.gbConstraints.Controls.Add(Me.btnEditConstraint)
        Me.gbConstraints.Controls.Add(Me.lbConstraints)
        Me.gbConstraints.Location = New System.Drawing.Point(11, 6)
        Me.gbConstraints.Name = "gbConstraints"
        Me.gbConstraints.Size = New System.Drawing.Size(331, 328)
        Me.gbConstraints.TabIndex = 1
        Me.gbConstraints.TabStop = False
        Me.gbConstraints.Text = "Subject to constraints"
        '
        'gbConstraint
        '
        Me.gbConstraint.Controls.Add(Me.btnAddConstraint)
        Me.gbConstraint.Controls.Add(Me.btnUpdateConstraint)
        Me.gbConstraint.Controls.Add(Me.cbEquality)
        Me.gbConstraint.Controls.Add(Me.btnConstraintReference)
        Me.gbConstraint.Controls.Add(Me.btnReference)
        Me.gbConstraint.Controls.Add(Me.tbConstraintReference)
        Me.gbConstraint.Controls.Add(Me.tbReference)
        Me.gbConstraint.Location = New System.Drawing.Point(6, 268)
        Me.gbConstraint.Name = "gbConstraint"
        Me.gbConstraint.Size = New System.Drawing.Size(319, 54)
        Me.gbConstraint.TabIndex = 7
        Me.gbConstraint.TabStop = False
        Me.gbConstraint.Text = "Constraint"
        '
        'btnAddConstraint
        '
        Me.btnAddConstraint.Location = New System.Drawing.Point(250, 18)
        Me.btnAddConstraint.Name = "btnAddConstraint"
        Me.btnAddConstraint.Size = New System.Drawing.Size(62, 23)
        Me.btnAddConstraint.TabIndex = 8
        Me.btnAddConstraint.Text = "Add"
        Me.btnAddConstraint.UseVisualStyleBackColor = True
        '
        'btnUpdateConstraint
        '
        Me.btnUpdateConstraint.Location = New System.Drawing.Point(250, 19)
        Me.btnUpdateConstraint.Name = "btnUpdateConstraint"
        Me.btnUpdateConstraint.Size = New System.Drawing.Size(62, 22)
        Me.btnUpdateConstraint.TabIndex = 9
        Me.btnUpdateConstraint.Text = "Update"
        Me.btnUpdateConstraint.UseVisualStyleBackColor = True
        '
        'cbEquality
        '
        Me.cbEquality.FormattingEnabled = True
        Me.cbEquality.Items.AddRange(New Object() {"=", ">=", "<="})
        Me.cbEquality.Location = New System.Drawing.Point(107, 19)
        Me.cbEquality.Name = "cbEquality"
        Me.cbEquality.Size = New System.Drawing.Size(32, 21)
        Me.cbEquality.TabIndex = 7
        '
        'btnConstraintReference
        '
        Me.btnConstraintReference.Location = New System.Drawing.Point(221, 19)
        Me.btnConstraintReference.Name = "btnConstraintReference"
        Me.btnConstraintReference.Size = New System.Drawing.Size(28, 20)
        Me.btnConstraintReference.TabIndex = 6
        Me.btnConstraintReference.Text = "..."
        Me.btnConstraintReference.UseVisualStyleBackColor = True
        '
        'btnReference
        '
        Me.btnReference.Location = New System.Drawing.Point(80, 20)
        Me.btnReference.Name = "btnReference"
        Me.btnReference.Size = New System.Drawing.Size(27, 19)
        Me.btnReference.TabIndex = 5
        Me.btnReference.Text = "..."
        Me.btnReference.UseVisualStyleBackColor = True
        '
        'tbConstraintReference
        '
        Me.tbConstraintReference.Location = New System.Drawing.Point(144, 19)
        Me.tbConstraintReference.Name = "tbConstraintReference"
        Me.tbConstraintReference.Size = New System.Drawing.Size(75, 20)
        Me.tbConstraintReference.TabIndex = 4
        '
        'tbReference
        '
        Me.tbReference.Location = New System.Drawing.Point(4, 19)
        Me.tbReference.Name = "tbReference"
        Me.tbReference.Size = New System.Drawing.Size(77, 20)
        Me.tbReference.TabIndex = 3
        '
        'btnDeleteConstraint
        '
        Me.btnDeleteConstraint.Enabled = False
        Me.btnDeleteConstraint.Location = New System.Drawing.Point(254, 66)
        Me.btnDeleteConstraint.Name = "btnDeleteConstraint"
        Me.btnDeleteConstraint.Size = New System.Drawing.Size(62, 23)
        Me.btnDeleteConstraint.TabIndex = 2
        Me.btnDeleteConstraint.Text = "Delete"
        Me.btnDeleteConstraint.UseVisualStyleBackColor = True
        '
        'btnEditConstraint
        '
        Me.btnEditConstraint.Enabled = False
        Me.btnEditConstraint.Location = New System.Drawing.Point(254, 29)
        Me.btnEditConstraint.Name = "btnEditConstraint"
        Me.btnEditConstraint.Size = New System.Drawing.Size(62, 23)
        Me.btnEditConstraint.TabIndex = 1
        Me.btnEditConstraint.Text = "Edit"
        Me.btnEditConstraint.UseVisualStyleBackColor = True
        '
        'gbVariables
        '
        Me.gbVariables.Controls.Add(Me.btnConsolidate)
        Me.gbVariables.Controls.Add(Me.btnGuess)
        Me.gbVariables.Controls.Add(Me.btnDeleteVariable)
        Me.gbVariables.Controls.Add(Me.btnEditVariable)
        Me.gbVariables.Controls.Add(Me.gbVariable)
        Me.gbVariables.Controls.Add(Me.lbVariables)
        Me.gbVariables.Location = New System.Drawing.Point(346, 6)
        Me.gbVariables.Name = "gbVariables"
        Me.gbVariables.Size = New System.Drawing.Size(250, 328)
        Me.gbVariables.TabIndex = 2
        Me.gbVariables.TabStop = False
        Me.gbVariables.Text = "By adjusting variables:"
        '
        'btnConsolidate
        '
        Me.btnConsolidate.Enabled = False
        Me.btnConsolidate.Location = New System.Drawing.Point(164, 140)
        Me.btnConsolidate.Name = "btnConsolidate"
        Me.btnConsolidate.Size = New System.Drawing.Size(70, 23)
        Me.btnConsolidate.TabIndex = 5
        Me.btnConsolidate.Text = "Consolidate"
        Me.btnConsolidate.UseVisualStyleBackColor = True
        '
        'btnGuess
        '
        Me.btnGuess.Enabled = False
        Me.btnGuess.Location = New System.Drawing.Point(164, 101)
        Me.btnGuess.Name = "btnGuess"
        Me.btnGuess.Size = New System.Drawing.Size(70, 23)
        Me.btnGuess.TabIndex = 4
        Me.btnGuess.Text = "Guess"
        Me.btnGuess.UseVisualStyleBackColor = True
        '
        'btnDeleteVariable
        '
        Me.btnDeleteVariable.Enabled = False
        Me.btnDeleteVariable.Location = New System.Drawing.Point(164, 66)
        Me.btnDeleteVariable.Name = "btnDeleteVariable"
        Me.btnDeleteVariable.Size = New System.Drawing.Size(70, 23)
        Me.btnDeleteVariable.TabIndex = 3
        Me.btnDeleteVariable.Text = "Delete"
        Me.btnDeleteVariable.UseVisualStyleBackColor = True
        '
        'btnEditVariable
        '
        Me.btnEditVariable.Enabled = False
        Me.btnEditVariable.Location = New System.Drawing.Point(164, 29)
        Me.btnEditVariable.Name = "btnEditVariable"
        Me.btnEditVariable.Size = New System.Drawing.Size(70, 23)
        Me.btnEditVariable.TabIndex = 2
        Me.btnEditVariable.Text = "Edit"
        Me.btnEditVariable.UseVisualStyleBackColor = True
        '
        'gbVariable
        '
        Me.gbVariable.Controls.Add(Me.btnAddVariable)
        Me.gbVariable.Controls.Add(Me.btnUpdateVariable)
        Me.gbVariable.Controls.Add(Me.btnVariable)
        Me.gbVariable.Controls.Add(Me.tbVariable)
        Me.gbVariable.Location = New System.Drawing.Point(8, 269)
        Me.gbVariable.Name = "gbVariable"
        Me.gbVariable.Size = New System.Drawing.Size(236, 53)
        Me.gbVariable.TabIndex = 1
        Me.gbVariable.TabStop = False
        Me.gbVariable.Text = "Variable"
        '
        'btnAddVariable
        '
        Me.btnAddVariable.Location = New System.Drawing.Point(160, 18)
        Me.btnAddVariable.Name = "btnAddVariable"
        Me.btnAddVariable.Size = New System.Drawing.Size(70, 23)
        Me.btnAddVariable.TabIndex = 3
        Me.btnAddVariable.Text = "Add"
        Me.btnAddVariable.UseVisualStyleBackColor = True
        '
        'btnUpdateVariable
        '
        Me.btnUpdateVariable.Location = New System.Drawing.Point(161, 18)
        Me.btnUpdateVariable.Name = "btnUpdateVariable"
        Me.btnUpdateVariable.Size = New System.Drawing.Size(70, 23)
        Me.btnUpdateVariable.TabIndex = 2
        Me.btnUpdateVariable.Text = "Update"
        Me.btnUpdateVariable.UseVisualStyleBackColor = True
        '
        'btnVariable
        '
        Me.btnVariable.Location = New System.Drawing.Point(110, 19)
        Me.btnVariable.Name = "btnVariable"
        Me.btnVariable.Size = New System.Drawing.Size(33, 20)
        Me.btnVariable.TabIndex = 1
        Me.btnVariable.Text = "..."
        Me.btnVariable.UseVisualStyleBackColor = True
        '
        'tbVariable
        '
        Me.tbVariable.Location = New System.Drawing.Point(11, 19)
        Me.tbVariable.Name = "tbVariable"
        Me.tbVariable.Size = New System.Drawing.Size(93, 20)
        Me.tbVariable.TabIndex = 0
        '
        'lbVariables
        '
        Me.lbVariables.FormattingEnabled = True
        Me.lbVariables.Location = New System.Drawing.Point(8, 25)
        Me.lbVariables.Name = "lbVariables"
        Me.lbVariables.Size = New System.Drawing.Size(149, 238)
        Me.lbVariables.TabIndex = 0
        '
        'btnResolve
        '
        Me.btnResolve.Enabled = False
        Me.btnResolve.Location = New System.Drawing.Point(602, 12)
        Me.btnResolve.Name = "btnResolve"
        Me.btnResolve.Size = New System.Drawing.Size(78, 26)
        Me.btnResolve.TabIndex = 3
        Me.btnResolve.Text = "Resolve"
        Me.btnResolve.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(602, 52)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(78, 26)
        Me.btnClose.TabIndex = 4
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'btnOptions
        '
        Me.btnOptions.Location = New System.Drawing.Point(602, 92)
        Me.btnOptions.Name = "btnOptions"
        Me.btnOptions.Size = New System.Drawing.Size(78, 26)
        Me.btnOptions.TabIndex = 5
        Me.btnOptions.Text = "Options"
        Me.btnOptions.UseVisualStyleBackColor = True
        '
        'btnLoadModel
        '
        Me.btnLoadModel.Location = New System.Drawing.Point(602, 132)
        Me.btnLoadModel.Name = "btnLoadModel"
        Me.btnLoadModel.Size = New System.Drawing.Size(78, 26)
        Me.btnLoadModel.TabIndex = 6
        Me.btnLoadModel.Text = "Load Model"
        Me.btnLoadModel.UseVisualStyleBackColor = True
        '
        'btnSaveModel
        '
        Me.btnSaveModel.Enabled = False
        Me.btnSaveModel.Location = New System.Drawing.Point(602, 172)
        Me.btnSaveModel.Name = "btnSaveModel"
        Me.btnSaveModel.Size = New System.Drawing.Size(78, 26)
        Me.btnSaveModel.TabIndex = 7
        Me.btnSaveModel.Text = "Save Model"
        Me.btnSaveModel.UseVisualStyleBackColor = True
        '
        'btnResetAll
        '
        Me.btnResetAll.Location = New System.Drawing.Point(602, 212)
        Me.btnResetAll.Name = "btnResetAll"
        Me.btnResetAll.Size = New System.Drawing.Size(78, 26)
        Me.btnResetAll.TabIndex = 8
        Me.btnResetAll.Text = "Reset All"
        Me.btnResetAll.UseVisualStyleBackColor = True
        '
        'btnHelp
        '
        Me.btnHelp.Location = New System.Drawing.Point(602, 291)
        Me.btnHelp.Name = "btnHelp"
        Me.btnHelp.Size = New System.Drawing.Size(78, 26)
        Me.btnHelp.TabIndex = 9
        Me.btnHelp.Text = "Help"
        Me.btnHelp.UseVisualStyleBackColor = True
        '
        'btnRegister
        '
        Me.btnRegister.Location = New System.Drawing.Point(603, 252)
        Me.btnRegister.Name = "btnRegister"
        Me.btnRegister.Size = New System.Drawing.Size(77, 25)
        Me.btnRegister.TabIndex = 10
        Me.btnRegister.Text = "Register"
        Me.btnRegister.UseVisualStyleBackColor = True
        '
        'frmProblem
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(689, 340)
        Me.Controls.Add(Me.btnRegister)
        Me.Controls.Add(Me.btnHelp)
        Me.Controls.Add(Me.btnResetAll)
        Me.Controls.Add(Me.btnSaveModel)
        Me.Controls.Add(Me.btnLoadModel)
        Me.Controls.Add(Me.btnOptions)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnResolve)
        Me.Controls.Add(Me.gbVariables)
        Me.Controls.Add(Me.gbConstraints)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmProblem"
        Me.Text = "Resolver Parameters"
        Me.gbConstraints.ResumeLayout(False)
        Me.gbConstraint.ResumeLayout(False)
        Me.gbConstraint.PerformLayout()
        Me.gbVariables.ResumeLayout(False)
        Me.gbVariable.ResumeLayout(False)
        Me.gbVariable.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents lbConstraints As System.Windows.Forms.ListBox
    Friend WithEvents gbConstraints As System.Windows.Forms.GroupBox
    Friend WithEvents gbConstraint As System.Windows.Forms.GroupBox
    Friend WithEvents btnUpdateConstraint As System.Windows.Forms.Button
    Friend WithEvents btnAddConstraint As System.Windows.Forms.Button
    Friend WithEvents cbEquality As System.Windows.Forms.ComboBox
    Friend WithEvents btnConstraintReference As System.Windows.Forms.Button
    Friend WithEvents btnReference As System.Windows.Forms.Button
    Friend WithEvents tbConstraintReference As System.Windows.Forms.TextBox
    Friend WithEvents tbReference As System.Windows.Forms.TextBox
    Friend WithEvents btnDeleteConstraint As System.Windows.Forms.Button
    Friend WithEvents btnEditConstraint As System.Windows.Forms.Button
    Friend WithEvents gbVariables As System.Windows.Forms.GroupBox
    Friend WithEvents btnConsolidate As System.Windows.Forms.Button
    Friend WithEvents btnGuess As System.Windows.Forms.Button
    Friend WithEvents btnDeleteVariable As System.Windows.Forms.Button
    Friend WithEvents btnEditVariable As System.Windows.Forms.Button
    Friend WithEvents gbVariable As System.Windows.Forms.GroupBox
    Friend WithEvents btnAddVariable As System.Windows.Forms.Button
    Friend WithEvents btnUpdateVariable As System.Windows.Forms.Button
    Friend WithEvents btnVariable As System.Windows.Forms.Button
    Friend WithEvents tbVariable As System.Windows.Forms.TextBox
    Friend WithEvents lbVariables As System.Windows.Forms.ListBox
    Friend WithEvents btnResolve As System.Windows.Forms.Button
    Friend WithEvents btnClose As System.Windows.Forms.Button
    Friend WithEvents btnOptions As System.Windows.Forms.Button
    Friend WithEvents btnLoadModel As System.Windows.Forms.Button
    Friend WithEvents btnSaveModel As System.Windows.Forms.Button
    Friend WithEvents btnResetAll As System.Windows.Forms.Button
    Friend WithEvents btnHelp As System.Windows.Forms.Button
    Friend WithEvents btnRegister As System.Windows.Forms.Button
End Class
