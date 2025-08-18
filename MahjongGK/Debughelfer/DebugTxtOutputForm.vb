

Public Class DebugTxtOutputForm
    Inherits Form

    Public ReadOnly TextBoxOutput As New TextBox With {
        .Multiline = True,
        .ScrollBars = ScrollBars.Both,
        .Dock = DockStyle.Fill,
        .Font = New Font("Consolas", 10),
        .WordWrap = False
    }

    Public Sub New()
        Me.Text = "Debug-Ausgabe"
        Me.Size = New Size(900, 600)
        Me.StartPosition = FormStartPosition.Manual
        Me.TopMost = True
        Me.Controls.Add(TextBoxOutput)
    End Sub

    Public Sub AppendText(text As String)
        If Me.InvokeRequired Then
            Me.Invoke(Sub() AppendText(text))
        Else
            TextBoxOutput.AppendText(text & Environment.NewLine)
        End If
    End Sub

    Public Sub SetText(text As String)
        If Me.InvokeRequired Then
            Me.Invoke(Sub() SetText(text))
        Else
            TextBoxOutput.Text = text
        End If
    End Sub
End Class
