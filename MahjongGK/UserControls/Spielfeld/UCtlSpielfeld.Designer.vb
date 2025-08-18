<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UCtlSpielfeld
    Inherits System.Windows.Forms.UserControl

    'UserControl überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnTestCodeAufrufen = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'btnTestCodeAufrufen
        '
        Me.btnTestCodeAufrufen.Location = New System.Drawing.Point(12, 13)
        Me.btnTestCodeAufrufen.Name = "btnTestCodeAufrufen"
        Me.btnTestCodeAufrufen.Size = New System.Drawing.Size(127, 33)
        Me.btnTestCodeAufrufen.TabIndex = 0
        Me.btnTestCodeAufrufen.Text = "TestCode aufrufen"
        Me.btnTestCodeAufrufen.UseVisualStyleBackColor = True
        '
        'UCtlSpielfeld
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.btnTestCodeAufrufen)
        Me.DoubleBuffered = True
        Me.Name = "UCtlSpielfeld"
        Me.Size = New System.Drawing.Size(843, 477)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents btnTestCodeAufrufen As Button
End Class
