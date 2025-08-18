Public Class DebugRtfOutputForm
    Inherits Form

    Public ReadOnly RichTextOutput As New RichTextBox With {
        .Dock = DockStyle.Fill,
        .Font = New Font("Consolas", 10),
        .ReadOnly = True,
        .WordWrap = False,
        .BackColor = Color.Black,
        .ForeColor = Color.White
    }

    Public Sub New()
        Me.Text = "Debug-Ausgabe mit Highlights"
        Me.Size = New Size(900, 600)
        Me.StartPosition = FormStartPosition.Manual
        Me.TopMost = True
        Me.Controls.Add(RichTextOutput)
    End Sub

    Public Sub AppendColoredText(text As String, foreColor As Color, Optional backColor As Color = Nothing)
        If Me.InvokeRequired Then
            Me.Invoke(Sub() AppendColoredText(text, foreColor, backColor))
            Return
        End If

        Dim start As Integer = RichTextOutput.TextLength
        RichTextOutput.AppendText(text)
        Dim [end] As Integer = RichTextOutput.TextLength

        RichTextOutput.Select(start, [end] - start)
        RichTextOutput.SelectionColor = foreColor
        If backColor <> Color.Empty Then
            RichTextOutput.SelectionBackColor = backColor
        Else
            RichTextOutput.SelectionBackColor = RichTextOutput.BackColor
        End If
        RichTextOutput.SelectionLength = 0
        RichTextOutput.SelectionStart = RichTextOutput.TextLength
        RichTextOutput.ScrollToCaret()
    End Sub

    Public Sub ClearText()
        If Me.InvokeRequired Then
            Me.Invoke(Sub() ClearText())
        Else
            RichTextOutput.Clear()
        End If
    End Sub
End Class
