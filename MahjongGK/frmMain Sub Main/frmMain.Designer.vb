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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.MenuStripMain = New System.Windows.Forms.MenuStrip()
        Me.ToolStripMain = New System.Windows.Forms.ToolStrip()
        Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
        Me.PanelFrmMainUGrd = New System.Windows.Forms.Panel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.lblInksOben = New System.Windows.Forms.Label()
        Me.TabControlMain = New System.Windows.Forms.TabControl()
        Me.TabPageSpielfeld = New System.Windows.Forms.TabPage()
        Me.TabPageEinstellungen = New System.Windows.Forms.TabPage()
        Me.TabPageEditor = New System.Windows.Forms.TabPage()
        Me.ToolTipMain = New System.Windows.Forms.ToolTip(Me.components)
        Me.UCtlSpielfeldMain = New MahjongGK.UCtlSpielfeld()
        Me.UCtlEinstellungenMain = New MahjongGK.UctlEinstellungen()
        Me.UCtlEditorMain = New MahjongGK.UCtlEditor()
        Me.ToolStripMain.SuspendLayout()
        Me.PanelFrmMainUGrd.SuspendLayout()
        Me.TabControlMain.SuspendLayout()
        Me.TabPageSpielfeld.SuspendLayout()
        Me.TabPageEinstellungen.SuspendLayout()
        Me.TabPageEditor.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStripMain
        '
        Me.MenuStripMain.Location = New System.Drawing.Point(0, 0)
        Me.MenuStripMain.Name = "MenuStripMain"
        Me.MenuStripMain.Size = New System.Drawing.Size(1084, 24)
        Me.MenuStripMain.TabIndex = 3
        Me.MenuStripMain.Text = "MenuStrip1"
        '
        'ToolStripMain
        '
        Me.ToolStripMain.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.ToolStripMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripButton1})
        Me.ToolStripMain.Location = New System.Drawing.Point(0, 466)
        Me.ToolStripMain.Name = "ToolStripMain"
        Me.ToolStripMain.Size = New System.Drawing.Size(1084, 25)
        Me.ToolStripMain.TabIndex = 4
        Me.ToolStripMain.Text = "ToolStrip1"
        '
        'ToolStripButton1
        '
        Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton1.Image = CType(resources.GetObject("ToolStripButton1.Image"), System.Drawing.Image)
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton1.Text = "ToolStripButton1"
        '
        'PanelFrmMainUGrd
        '
        Me.PanelFrmMainUGrd.Controls.Add(Me.Label2)
        Me.PanelFrmMainUGrd.Controls.Add(Me.lblInksOben)
        Me.PanelFrmMainUGrd.Controls.Add(Me.TabControlMain)
        Me.PanelFrmMainUGrd.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PanelFrmMainUGrd.Location = New System.Drawing.Point(0, 24)
        Me.PanelFrmMainUGrd.Name = "PanelFrmMainUGrd"
        Me.PanelFrmMainUGrd.Size = New System.Drawing.Size(1084, 442)
        Me.PanelFrmMainUGrd.TabIndex = 5
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(996, 420)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(80, 13)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "lblRechtsUnten"
        '
        'lblInksOben
        '
        Me.lblInksOben.AutoSize = True
        Me.lblInksOben.Location = New System.Drawing.Point(8, 7)
        Me.lblInksOben.Name = "lblInksOben"
        Me.lblInksOben.Size = New System.Drawing.Size(63, 13)
        Me.lblInksOben.TabIndex = 3
        Me.lblInksOben.Text = "lblInksOben"
        '
        'TabControlMain
        '
        Me.TabControlMain.Controls.Add(Me.TabPageSpielfeld)
        Me.TabControlMain.Controls.Add(Me.TabPageEinstellungen)
        Me.TabControlMain.Controls.Add(Me.TabPageEditor)
        Me.TabControlMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControlMain.Location = New System.Drawing.Point(0, 0)
        Me.TabControlMain.Name = "TabControlMain"
        Me.TabControlMain.SelectedIndex = 0
        Me.TabControlMain.Size = New System.Drawing.Size(1084, 442)
        Me.TabControlMain.TabIndex = 2
        '
        'TabPageSpielfeld
        '
        Me.TabPageSpielfeld.Controls.Add(Me.UCtlSpielfeldMain)
        Me.TabPageSpielfeld.Location = New System.Drawing.Point(4, 22)
        Me.TabPageSpielfeld.Name = "TabPageSpielfeld"
        Me.TabPageSpielfeld.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageSpielfeld.Size = New System.Drawing.Size(1076, 416)
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
        Me.TabPageEinstellungen.Size = New System.Drawing.Size(900, 416)
        Me.TabPageEinstellungen.TabIndex = 1
        Me.TabPageEinstellungen.Text = "Einstellungen"
        Me.TabPageEinstellungen.UseVisualStyleBackColor = True
        '
        'TabPageEditor
        '
        Me.TabPageEditor.Controls.Add(Me.UCtlEditorMain)
        Me.TabPageEditor.Location = New System.Drawing.Point(4, 22)
        Me.TabPageEditor.Name = "TabPageEditor"
        Me.TabPageEditor.Size = New System.Drawing.Size(900, 416)
        Me.TabPageEditor.TabIndex = 2
        Me.TabPageEditor.Text = "Editor"
        Me.TabPageEditor.UseVisualStyleBackColor = True
        '
        'UCtlSpielfeldMain
        '
        Me.UCtlSpielfeldMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UCtlSpielfeldMain.Location = New System.Drawing.Point(3, 3)
        Me.UCtlSpielfeldMain.Name = "UCtlSpielfeldMain"
        Me.UCtlSpielfeldMain.Size = New System.Drawing.Size(1070, 410)
        Me.UCtlSpielfeldMain.TabIndex = 0
        '
        'UCtlEinstellungenMain
        '
        Me.UCtlEinstellungenMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.UCtlEinstellungenMain.Location = New System.Drawing.Point(3, 3)
        Me.UCtlEinstellungenMain.Name = "UCtlEinstellungenMain"
        Me.UCtlEinstellungenMain.Size = New System.Drawing.Size(894, 410)
        Me.UCtlEinstellungenMain.TabIndex = 0
        '
        'UCtlEditorMain
        '
        Me.UCtlEditorMain.Location = New System.Drawing.Point(0, 0)
        Me.UCtlEditorMain.Name = "UCtlEditorMain"
        Me.UCtlEditorMain.Size = New System.Drawing.Size(892, 441)
        Me.UCtlEditorMain.TabIndex = 0
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1084, 491)
        Me.Controls.Add(Me.PanelFrmMainUGrd)
        Me.Controls.Add(Me.ToolStripMain)
        Me.Controls.Add(Me.MenuStripMain)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStripMain
        Me.Name = "frmMain"
        Me.Text = "MahjongGK - by Götz Kircher - Version: "
        Me.ToolStripMain.ResumeLayout(False)
        Me.ToolStripMain.PerformLayout()
        Me.PanelFrmMainUGrd.ResumeLayout(False)
        Me.PanelFrmMainUGrd.PerformLayout()
        Me.TabControlMain.ResumeLayout(False)
        Me.TabPageSpielfeld.ResumeLayout(False)
        Me.TabPageEinstellungen.ResumeLayout(False)
        Me.TabPageEditor.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStripMain As MenuStrip
    Friend WithEvents ToolStripMain As ToolStrip
    Friend WithEvents PanelFrmMainUGrd As Panel
    Friend WithEvents Label2 As Label
    Friend WithEvents lblInksOben As Label
    Friend WithEvents TabControlMain As TabControl
    Friend WithEvents TabPageSpielfeld As TabPage
    Friend WithEvents UCtlSpielfeldMain As UCtlSpielfeld
    Friend WithEvents TabPageEinstellungen As TabPage
    Friend WithEvents UCtlEinstellungenMain As UctlEinstellungen
    Friend WithEvents TabPageEditor As TabPage
    Friend WithEvents UCtlEditorMain As UCtlEditor
    Friend WithEvents ToolStripButton1 As ToolStripButton
    Friend WithEvents ToolTipMain As ToolTip
End Class
