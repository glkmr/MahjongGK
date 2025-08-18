Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
'
' SPDX-License-Identifier: GPL-3.0-or-later
'###########################################################################
'#                                                                         #
'#   Copyright © 2025–2026 Götz Kircher <mahjonggk@t-online.de>            #
'#                                                                         #
'#                     MahjongGK  -  Mahjong Solitär                       #
'#                                                                         #
'#   This program is free software: you can redistribute it and/or modify  #
'#   it under the terms of the GNU General Public License as published by  #
'#   the Free Software Foundation, either version 3 of the License, or     #
'#   at your option any later version.                                     #
'#                                                                         #
'#   This program is distributed in the hope that it will be useful,       #
'#   but WITHOUT ANY WARRANTY; without even the implied warranty of        #
'#   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the          #
'#   GNU General Public License for more details.                          #
'#   https://www.gnu.org/licenses/gpl-3.0.html                             #
'#                                                                         #
'###########################################################################
'
'
'



#Disable Warning IDE0079
#Disable Warning IDE1006



Public Class UCtlSpielfeld

#Region "Code, der nicht entfernt werden darf (nicht zum TestCode gehörend)"

    Public Sub New()
        InitializeComponent()

        Me.Dock = DockStyle.Fill

        ' Wichtig gegen Flackern:
        Me.DoubleBuffered = True
        Me.SetStyle(ControlStyles.UserPaint Or
                ControlStyles.AllPaintingInWmPaint Or
                ControlStyles.OptimizedDoubleBuffer Or
                ControlStyles.ResizeRedraw, True)
        Me.UpdateStyles()

        If Not Debugger.IsAttached Then
            'den Button links oben rausnehmen.
            btnTestCodeAufrufen.Parent = Nothing
        End If

    End Sub

    ''<DebuggerStepThrough>
    'Private Sub UCtlSpielfeld_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint

    '    If Spielfeld.PaintLimiter.BeginPaint Then
    '        Spielfeld.PaintSpielfeld_Paint(frmMain.VisibleUserControl.Spielfeld, e, New Rectangle(0, 0, Width, Height), Spielfeld.PaintLimiter.TimeDifferenzFaktor)
    '    End If

    'End Sub

    ' Du füllst komplett selbst → Hintergrund NICHT automatisch löschen
    Protected Overrides Sub OnPaintBackground(pevent As PaintEventArgs)
        ' absichtlich leer
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        ' Fläche komplett selbst füllen
        e.Graphics.Clear(Me.BackColor)

        ' dein Rendercode
        Spielfeld.PaintSpielfeld_Paint(frmMain.VisibleUserControl.Spielfeld,
        e,
        New Rectangle(0, 0, Me.Width, Me.Height),
        Spielfeld.PaintLimiter.TimeDifferenzFaktor)
    End Sub

#End Region

#Region "Testcode während der Programmentwicklung"

    'Hier können verschiedene Tests zum Debuggen aufgerufen werden.
    'Der Button ist nur innerhalb der IDE sichtbar.

    Private Sub btnTestCodeAufrufen_Click(sender As Object, e As EventArgs) Handles btnTestCodeAufrufen.Click


#Region "Verschiedene Spielfeldszenarien"

        Spielfeld.TestDaten_Spielfeld_3_x_3_x_3()

#End Region



#Region "Steine kontrollieren"
        'BitmapContainer.ChangeImagesSize(steinWidth:=50, steinHeight:=CInt(50 * MJ_GRAFIK_FAKTOR_W_TO_H))
        'DebugShowArrayBitmaps(BitmapContainer.GetOriginalBitmaps(SteinStatus.Selected))
        'DebugShowArrayBitmaps(BitmapContainer.GetScaledBitmaps(SteinStatus.Normal))
        '/Steine kontrollieren
#End Region

    End Sub


#End Region


End Class
