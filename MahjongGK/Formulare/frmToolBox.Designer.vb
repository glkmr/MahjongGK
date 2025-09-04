<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmToolBox
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.Num2UpDown1 = New MahjongGK.Num2UpDown()
        Me.SuspendLayout()
        '
        'Num2UpDown1
        '
        Me.Num2UpDown1.Location = New System.Drawing.Point(52, 159)
        Me.Num2UpDown1.MinimumSize = New System.Drawing.Size(65, 24)
        Me.Num2UpDown1.Name = "Num2UpDown1"
        Me.Num2UpDown1.Size = New System.Drawing.Size(65, 28)
        Me.Num2UpDown1.TabIndex = 0
        '
        'frmToolBox
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(234, 461)
        Me.Controls.Add(Me.Num2UpDown1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmToolBox"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.Manual
        Me.Text = "frmToolBox"
        Me.TopMost = True
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Num2UpDown1 As Num2UpDown
End Class
