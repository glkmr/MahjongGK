﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class UctlEinstellungen
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(UctlEinstellungen))
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPageIfRunningIDE = New System.Windows.Forms.TabPage()
        Me.TabPageAllgemein = New System.Windows.Forms.TabPage()
        Me.TabPageInfo = New System.Windows.Forms.TabPage()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.CheckBox1 = New System.Windows.Forms.CheckBox()
        Me.CheckBox2 = New System.Windows.Forms.CheckBox()
        Me.TabControl1.SuspendLayout()
        Me.TabPageIfRunningIDE.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPageIfRunningIDE)
        Me.TabControl1.Controls.Add(Me.TabPageAllgemein)
        Me.TabControl1.Controls.Add(Me.TabPageInfo)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(800, 400)
        Me.TabControl1.TabIndex = 0
        '
        'TabPageIfRunningIDE
        '
        Me.TabPageIfRunningIDE.Controls.Add(Me.CheckBox2)
        Me.TabPageIfRunningIDE.Controls.Add(Me.CheckBox1)
        Me.TabPageIfRunningIDE.Controls.Add(Me.Label1)
        Me.TabPageIfRunningIDE.Location = New System.Drawing.Point(4, 22)
        Me.TabPageIfRunningIDE.Name = "TabPageIfRunningIDE"
        Me.TabPageIfRunningIDE.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageIfRunningIDE.Size = New System.Drawing.Size(792, 374)
        Me.TabPageIfRunningIDE.TabIndex = 0
        Me.TabPageIfRunningIDE.Text = "IfRunningIDE"
        Me.TabPageIfRunningIDE.UseVisualStyleBackColor = True
        '
        'TabPageAllgemein
        '
        Me.TabPageAllgemein.Location = New System.Drawing.Point(4, 22)
        Me.TabPageAllgemein.Name = "TabPageAllgemein"
        Me.TabPageAllgemein.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPageAllgemein.Size = New System.Drawing.Size(792, 374)
        Me.TabPageAllgemein.TabIndex = 1
        Me.TabPageAllgemein.Text = "Allgemein"
        Me.TabPageAllgemein.UseVisualStyleBackColor = True
        '
        'TabPageInfo
        '
        Me.TabPageInfo.Location = New System.Drawing.Point(4, 22)
        Me.TabPageInfo.Name = "TabPageInfo"
        Me.TabPageInfo.Size = New System.Drawing.Size(792, 374)
        Me.TabPageInfo.TabIndex = 2
        Me.TabPageInfo.Text = "Info"
        Me.TabPageInfo.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(16, 15)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(729, 39)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = resources.GetString("Label1.Text")
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Location = New System.Drawing.Point(19, 82)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(81, 17)
        Me.CheckBox1.TabIndex = 1
        Me.CheckBox1.Text = "CheckBox1"
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'CheckBox2
        '
        Me.CheckBox2.AutoSize = True
        Me.CheckBox2.Location = New System.Drawing.Point(19, 105)
        Me.CheckBox2.Name = "CheckBox2"
        Me.CheckBox2.Size = New System.Drawing.Size(81, 17)
        Me.CheckBox2.TabIndex = 2
        Me.CheckBox2.Text = "CheckBox2"
        Me.CheckBox2.UseVisualStyleBackColor = True
        '
        'UctlEinstellungen
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.TabControl1)
        Me.Name = "UctlEinstellungen"
        Me.Size = New System.Drawing.Size(800, 400)
        Me.TabControl1.ResumeLayout(False)
        Me.TabPageIfRunningIDE.ResumeLayout(False)
        Me.TabPageIfRunningIDE.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPageIfRunningIDE As TabPage
    Friend WithEvents TabPageAllgemein As TabPage
    Friend WithEvents TabPageInfo As TabPage
    Friend WithEvents Label1 As Label
    Friend WithEvents CheckBox2 As CheckBox
    Friend WithEvents CheckBox1 As CheckBox
End Class
