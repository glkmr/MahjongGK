''
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
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006


Public Class PaintLimiterClass

    ' ======= EINSTELLUNGEN =======
    Public Const FRAME_MS As Double = 25.0      ' Soll-Abstand in ms (40 FPS)
    Public Const MAX_FACTOR As Double = 2.0     ' Obergrenze für Faktor

    ' ======= INTERN =======
    Private lastPaintTicks As Long = 0

    ' Diese Funktion wird zu Beginn des Paint-Events aufgerufen
    Public Function BeginPaint() As Boolean


        Dim nowTicks As Long = Stopwatch.GetTimestamp()

        If lastPaintTicks = 0 Then
            ' Sanftstart
            lastPaintTicks = nowTicks
            _TimeDifferenzFaktor = 1.0
            Return True
        Else
            Dim deltaMs As Double = (nowTicks - lastPaintTicks) * 1000.0 / Stopwatch.Frequency

            ' Lange Pause → Sanftstart
            If deltaMs > FRAME_MS * MAX_FACTOR * 3 Then
                _TimeDifferenzFaktor = 1.0
                lastPaintTicks = nowTicks
                Return True
            End If

            ' FPS-Limit
            If deltaMs < FRAME_MS Then Return False

            ' Faktor berechnen und deckeln
            Dim rawFactor As Double = deltaMs / FRAME_MS
            _TimeDifferenzFaktor = Math.Min(rawFactor, MAX_FACTOR)

            lastPaintTicks = nowTicks
            Return True
        End If
    End Function

    ' Private Variable für TimeDifferenzFaktor
    Private _TimeDifferenzFaktor As Double = 1.0
    Public ReadOnly Property TimeDifferenzFaktor As Double
        Get
            Return _TimeDifferenzFaktor
        End Get
    End Property

End Class

