﻿'
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

' kein Namespace 
''' <summary>
''' Container mit allen benötigten Daten.
''' </summary>
Public Class Werkstück

    Sub New()

    End Sub

    Sub New(SteinInfos As List(Of SteinInfo), arrFB(,,) As Integer)
        _SteinInfos = SteinInfos
        _arrFB = arrFB
    End Sub

    Private _SteinInfos As List(Of SteinInfo) = Nothing

    Public ReadOnly Property SteinInfos As List(Of SteinInfo)
        Get
            Return _SteinInfos
        End Get
    End Property

    Private ReadOnly _arrFB(,,) As Integer

    Public ReadOnly Property ArrFB() As Integer(,,)
        Get
            Return _arrFB
        End Get
    End Property

    Public ReadOnly Property SteineNeeded() As Integer
        Get
            If SteinInfos Is Nothing Then
                Return -1
            Else
                Return SteinInfos.Count
            End If
        End Get
    End Property

    Public ReadOnly Property ResultSteinInfosArrFB() As (steinInfos As List(Of SteinInfo), arrFB As Integer(,,))
        Get
            Return (SteinInfos, ArrFB)
        End Get
    End Property

    ''' <summary>
    ''' Übergibt den Steinvorrat und startet den Austauschvorgang.
    ''' Vorher CountSteineNeeded aufrufen und die genaue Anzahl von 
    ''' Steinen im Vorrat übergeben.
    ''' </summary>
    ''' <param name="vorrat"></param>
    Public Sub SetSteinVorrat(vorrat As List(Of SteinIndexEnum))

        If SteineNeeded <= 0 Then
            _resultOK = False
            Exit Sub
        End If

        'Wenn die der Baustein aus der Werkstatt kommt, ist
        '.CountSteineNeeded = SteineNeeded
        If vorrat.Count <> SteineNeeded Then
            _resultOK = False
            Exit Sub
        End If

        'Modus alles austauschen, denn der Baustein kommt aus der Werkstatt.
        '(Der andere Modus ist nur nötig, wenn die Steine unverändert
        'ohne SpielfeldSteineAustauscher eingebaut werden. Der
        'Austauscher erkennt, welche Steine geändert werden müssen.)
        Dim ssa As New SpielfeldSteineAustauscher(SteinInfos, vorrat)
        _resultOK = True ' ist immer True, weil SteineNeeded > 0.

        _SteinInfos = ssa.ChangedSteinInfos

    End Sub

    Private _resultOK As Boolean
    Public ReadOnly Property ResultOK As Boolean
        Get
            Return _resultOK
        End Get
    End Property
End Class
