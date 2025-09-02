<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.TabControlMain = New System.Windows.Forms.TabControl()
        Me.TabPageSpielfeld = New System.Windows.Forms.TabPage()
        Me.TabPageEinstellungen = New System.Windows.Forms.TabPage()
        Me.UCtlSpielfeldMain = New MahjongGK.UCtlSpielfeld()
        Me.UCtlEinstellungenMain = New MahjongGK.UctlEinstellungen()
        Me.TabControlMain.SuspendLayout()
        Me.TabPageSpielfeld.SuspendLayout()
        Me.TabPageEinstellungen.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControlMain
        '
        Me.TabControlMain.Controls.Add(Me.TabPageSpielfeld)
        Me.TabControlMain.Controls.Add(Me.TabPageEinstellungen)
        Me.TabControlMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControlMain.Location = New System.Drawing.Point(0, 0)
        Me.TabControlMain.Name = "TabControlMain"
        Me.TabControlMain.SelectedIndex = 0
        Me.TabControlMain.Size = New System.Drawing.Size(908, 491)
        Me.TabControlMain.TabIndex = 2
        '
        'TabPageSpielfeld
        '
        Me.TabPageSpielfeld.Controls.Add(Me.UCtlSpielfeldMain)
        Me.TabPageSpielfeld.Location = New System.Drawing.Point(4, 22)
        Me.TabPageSpielfeld.Name = "TabPageSpielfeld"
        Me.TabPageSpielfeld.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageSpielfeld.Size = New System.Drawing.Size(900, 465)
        Me.TabPageSpielfeld.TabIndex = 0
        Me.TabPageSpielfeld.Text = "SpielsteinGenerator_zlv"
        Me.TabPageSpielfeld.UseVisualStyleBackColor = True
        '
        'TabPageEinstellungen
        '
        Me.TabPageEinstellungen.Controls.Add(Me.UCtlEinstellungenMain)
        Me.TabPageEinstellungen.Location = New System.Drawing.Point(4, 22)
        Me.TabPageEinstellungen.Name = "TabPageEinstellungen"
        Me.TabPageEinstellungen.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageEinstellungen.Size = New System.Drawing.Size(900, 465)
        Me.TabPageEinstellungen.TabIndex = 1
        Me.TabPageEinstellungen.Text = "Einstellungen"
        Me.TabPageEinstellungen.UseVisualStyleBackColor = True
        '
        'UCtlSpielfeldMain
        '
        Me.UCtlSpielfeldMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UCtlSpielfeldMain.Location = New System.Drawing.Point(3, 3)
        Me.UCtlSpielfeldMain.Name = "UCtlSpielfeldMain"
        Me.UCtlSpielfeldMain.Size = New System.Drawing.Size(894, 459)
        Me.UCtlSpielfeldMain.TabIndex = 0
        '
        'UCtlEinstellungenMain
        '
        Me.UCtlEinstellungenMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UCtlEinstellungenMain.Location = New System.Drawing.Point(3, 3)
        Me.UCtlEinstellungenMain.Name = "UCtlEinstellungenMain"
        Me.UCtlEinstellungenMain.Size = New System.Drawing.Size(894, 459)
        Me.UCtlEinstellungenMain.TabIndex = 0
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(908, 491)
        Me.Controls.Add(Me.TabControlMain)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMain"
        Me.Text = "MahjongGK - by Götz Kircher - Version: "
        Me.TabControlMain.ResumeLayout(False)
        Me.TabPageSpielfeld.ResumeLayout(False)
        Me.TabPageEinstellungen.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents TabControlMain As TabControl
    Friend WithEvents TabPageSpielfeld As TabPage
    Friend WithEvents TabPageEinstellungen As TabPage
    Friend WithEvents UCtlEinstellungenMain As UctlEinstellungen
    Friend WithEvents UCtlSpielfeldMain As UCtlSpielfeld
End Class
