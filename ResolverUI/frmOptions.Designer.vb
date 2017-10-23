<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmOptions
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmOptions))
        Me.tbMaxTime = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.tbIterations = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.tbPrecision = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.tbConvergence = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.cbOK = New System.Windows.Forms.Button()
        Me.cbCancel = New System.Windows.Forms.Button()
        Me.cbHelp = New System.Windows.Forms.Button()
        Me.cbInitializevalues = New System.Windows.Forms.CheckBox()
        Me.cbAssumeNonNegative = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'tbMaxTime
        '
        Me.tbMaxTime.Location = New System.Drawing.Point(83, 14)
        Me.tbMaxTime.Name = "tbMaxTime"
        Me.tbMaxTime.Size = New System.Drawing.Size(60, 20)
        Me.tbMaxTime.TabIndex = 0
        Me.tbMaxTime.Text = "10"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(7, 14)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(56, 13)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Max Time:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(149, 17)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(47, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "seconds"
        '
        'tbIterations
        '
        Me.tbIterations.Location = New System.Drawing.Point(83, 39)
        Me.tbIterations.Name = "tbIterations"
        Me.tbIterations.Size = New System.Drawing.Size(60, 20)
        Me.tbIterations.TabIndex = 3
        Me.tbIterations.Text = "20"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(8, 39)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(53, 13)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Iterations:"
        '
        'tbPrecision
        '
        Me.tbPrecision.Location = New System.Drawing.Point(84, 62)
        Me.tbPrecision.Name = "tbPrecision"
        Me.tbPrecision.Size = New System.Drawing.Size(58, 20)
        Me.tbPrecision.TabIndex = 5
        Me.tbPrecision.Text = "0.000001"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(7, 66)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(53, 13)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "Precision:"
        '
        'tbConvergence
        '
        Me.tbConvergence.Location = New System.Drawing.Point(84, 89)
        Me.tbConvergence.Name = "tbConvergence"
        Me.tbConvergence.Size = New System.Drawing.Size(57, 20)
        Me.tbConvergence.TabIndex = 7
        Me.tbConvergence.Text = "0.0001"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(7, 93)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(74, 13)
        Me.Label5.TabIndex = 8
        Me.Label5.Text = "Convergence:"
        '
        'cbOK
        '
        Me.cbOK.Location = New System.Drawing.Point(281, 7)
        Me.cbOK.Name = "cbOK"
        Me.cbOK.Size = New System.Drawing.Size(73, 21)
        Me.cbOK.TabIndex = 9
        Me.cbOK.Text = "OK"
        Me.cbOK.UseVisualStyleBackColor = True
        '
        'cbCancel
        '
        Me.cbCancel.Location = New System.Drawing.Point(281, 31)
        Me.cbCancel.Name = "cbCancel"
        Me.cbCancel.Size = New System.Drawing.Size(73, 23)
        Me.cbCancel.TabIndex = 10
        Me.cbCancel.Text = "Cancel"
        Me.cbCancel.UseVisualStyleBackColor = True
        '
        'cbHelp
        '
        Me.cbHelp.Location = New System.Drawing.Point(281, 57)
        Me.cbHelp.Name = "cbHelp"
        Me.cbHelp.Size = New System.Drawing.Size(73, 23)
        Me.cbHelp.TabIndex = 11
        Me.cbHelp.Text = "Help"
        Me.cbHelp.UseVisualStyleBackColor = True
        '
        'cbInitializevalues
        '
        Me.cbInitializevalues.AutoSize = True
        Me.cbInitializevalues.Location = New System.Drawing.Point(156, 65)
        Me.cbInitializevalues.Name = "cbInitializevalues"
        Me.cbInitializevalues.Size = New System.Drawing.Size(98, 17)
        Me.cbInitializevalues.TabIndex = 12
        Me.cbInitializevalues.Text = "Initialize Values"
        Me.cbInitializevalues.UseVisualStyleBackColor = True
        '
        'cbAssumeNonNegative
        '
        Me.cbAssumeNonNegative.AutoSize = True
        Me.cbAssumeNonNegative.Location = New System.Drawing.Point(156, 91)
        Me.cbAssumeNonNegative.Name = "cbAssumeNonNegative"
        Me.cbAssumeNonNegative.Size = New System.Drawing.Size(132, 17)
        Me.cbAssumeNonNegative.TabIndex = 13
        Me.cbAssumeNonNegative.Text = "Assume Non Negative"
        Me.cbAssumeNonNegative.UseVisualStyleBackColor = True
        '
        'frmOptions
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(361, 118)
        Me.Controls.Add(Me.cbAssumeNonNegative)
        Me.Controls.Add(Me.cbInitializevalues)
        Me.Controls.Add(Me.cbHelp)
        Me.Controls.Add(Me.cbCancel)
        Me.Controls.Add(Me.cbOK)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.tbConvergence)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.tbPrecision)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.tbIterations)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.tbMaxTime)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmOptions"
        Me.Text = "Options"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents tbMaxTime As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents tbIterations As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents tbPrecision As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents tbConvergence As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents cbOK As System.Windows.Forms.Button
    Friend WithEvents cbCancel As System.Windows.Forms.Button
    Friend WithEvents cbHelp As System.Windows.Forms.Button
    Friend WithEvents cbInitializevalues As System.Windows.Forms.CheckBox
    Friend WithEvents cbAssumeNonNegative As System.Windows.Forms.CheckBox
End Class
