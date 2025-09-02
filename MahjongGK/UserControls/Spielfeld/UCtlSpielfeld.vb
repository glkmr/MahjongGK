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
Imports MahjongGK.Spielfeld



#Disable Warning IDE0079
#Disable Warning IDE1006



Public Class UCtlSpielfeld

#Region "Code, der nicht entfernt werden darf (nicht zum TestCode gehörend)"

    Public Sub New()
        InitializeComponent()
        Me.Dock = DockStyle.Fill
        Me.DoubleBuffered = True
        Me.SetStyle(ControlStyles.UserPaint Or
                    ControlStyles.AllPaintingInWmPaint Or
                    ControlStyles.OptimizedDoubleBuffer Or
                    ControlStyles.ResizeRedraw, True)
        ' Optional, wenn wirklich der kompletten Bereich selber gezeichnet wird:
        Me.SetStyle(ControlStyles.Opaque, True)
        Me.UpdateStyles()


    End Sub



    ' Ich fülle komplett selbst -> Hintergrund NICHT automatisch löschen
    Protected Overrides Sub OnPaintBackground(pevent As PaintEventArgs)
        ' absichtlich leer
    End Sub

    Private stopwatch As New Stopwatch

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        ' Immer malen, Factor kommt vom Scheduler (Stopwatch-basiert):
        Dim factor As Double = FrameSchedulerModul.FrameScheduler.TimeDifferenzFaktor

        Spielfeld.PaintSpielfeld_Paint(frmMain.VisibleUserControl.Spielfeld,
                                       e,
                                       New Rectangle(0, 0, Me.Width, Me.Height),
                                       factor)
#If DEBUGFRAME Then
        If stopwatch.IsRunning Then
            Debug.WriteLine("Zeit seit letztem Paint: " & stopwatch.ElapsedMilliseconds & " ms, factor=" & factor.ToString("0.00"))
        End If
#End If
        stopwatch.Restart()

    End Sub

#End Region

#Region "Testcode während der Programmentwicklung"
    'Hier können verschiedene Tests zum Debuggen aufgerufen werden.
    'Der Button ist nur innerhalb der IDE sichtbar.
    Private Sub btnTestCodeAufrufen_Click(sender As Object, e As EventArgs) Handles btnTestCodeAufrufen.Click

        Spielfeld.TestDaten_Spielfeld_Variablel_zum_Debuggen()

        'SpielfeldTest_SpielsteinGenerator.RunAll()

        'Dim gen1 As New SpielsteinGenerator(visibleAreaMaxLength:=30, generatorMode:=GeneratorModi.StoneStream_Base152_Continuous)
        'Dim gen2 As New SpielsteinGenerator(visibleAreaMaxLength:=30, generatorMode:=GeneratorModi.StoneSet_144)

        'Dim stat As New Statistik(gen1.Vorrat, gen2.Vorrat)
        'MessageBoxFormatiert.ShowInfoMonoSpaced(stat.ToString(deltaProz144:=True), "Spielsteinverteilung")
        'MessageBoxFormatiert.ShowInfoMonoSpaced(Spielfeld.DebugKonstantenString, "Spielsteinvariable")

    End Sub

#End Region


#Region "Verschiedene Spielfeldszenarien"



#Region "Steine kontrollieren"
    'BitmapContainer.ChangeImagesSize(steinWidth:=50, steinHeight:=CInt(50 * MJ_GRAFIK_FAKTOR_W_TO_H))
    'DebugShowArrayBitmaps(BitmapContainer.GetOriginalBitmaps(SteinStatus.Selected))
    'DebugShowArrayBitmaps(BitmapContainer.GetScaledBitmaps(SteinStatus.Normal))
    '/Steine kontrollieren
#End Region



#End Region


End Class
