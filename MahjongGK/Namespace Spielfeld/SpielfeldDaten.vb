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

Namespace Spielfeld

    ''' <summary>
    ''' Hier sind alle Daten des aktuellen Spiels untergebracht.
    ''' Es ist bewußt ein Modul, da von vorneherein feststand, daß es nur eine einzige
    ''' Instanz geben wird. Die verschiedenen Module im Verzeichnis Spielfeld
    ''' sind im Namespace Spielfeld gekapselt. 
    ''' </summary>
    Public Module SpielfeldDaten

        ''' <summary>
        ''' Eine Enumeration, mit der das aktuelle UserControl, auf das gezeichnet wird,
        ''' indentifiniert werden kann.
        ''' </summary>
        ''' <returns></returns>
        Public Property VisibleUserControl As frmMain.VisibleUserControl
        '
        ''' <summary>
        ''' Das aktuelle Spielfeld As SpielfeldInfo.
        ''' Das Setzen startet die Animation, wenn vorher von irgendwo anders
        ''' Spielfeld.PaintLimiterModul.PaintSpielfeld_GivePermissionIfPossible = True gesetzt wurde.
        ''' (Dadurch wird Spielfeld.SpielfeldVerwaltung.UpdateSpielfeld_GivePermissionIfPossible
        ''' im Rendertakt aufgerufen, bis aktSpielfeld ungleich Nothing ist.)
        ''' Wird aktSpielfeld = Nothing gesetzt, schaltet sich die Animation auch ab,
        ''' aber mit einer Fehlermeldung direkt auf dem Bildschirm
        ''' </summary>
        Public Property AktSpielfeldInfo As SpielfeldInfo = Nothing
        Public Property PlayerSpielfeldInfo As SpielfeldInfo = Nothing
        Public Property EditorSpielfeldInfo As SpielfeldInfo = Nothing
        Public Property WerkbankSpielfeldInfo As SpielfeldInfo = Nothing

        Public Property AktRendering As Rendering
            Get
                Return RuntimeOnly_AktRendering
            End Get
            Set(value As Rendering)
                RuntimeOnly_AktRendering = value
            End Set
        End Property

        Public Property FrozenBitmapPlayerSpielfeld As Bitmap = Nothing
        Public Property FrozenBitmapEditorSpielfeld As Bitmap = Nothing
        Public Property FrozenBitmapWerkbankSpielfeld As Bitmap = Nothing

        Private _UseFrozenBitmapPlayerSpielfeld As Boolean
        Public Property UseFrozenBitmapPlayerSpielfeld As Boolean
            Get
                If IsNothing(FrozenBitmapPlayerSpielfeld) Then
                    Return False
                Else
                    Return _UseFrozenBitmapPlayerSpielfeld
                End If
            End Get
            Set(value As Boolean)
                _UseFrozenBitmapPlayerSpielfeld = value
            End Set
        End Property

        Private _UseFrozenBitmapEditorSpielfeld As Boolean
        Public Property UseFrozenBitmapEditorSpielfeld As Boolean
            Get
                If IsNothing(FrozenBitmapEditorSpielfeld) Then
                    Return False
                Else
                    Return _UseFrozenBitmapEditorSpielfeld
                End If
            End Get
            Set(value As Boolean)
                _UseFrozenBitmapEditorSpielfeld = value
            End Set
        End Property

        Private _UseFrozenBitmapWerkbankSpielfeld As Boolean
        Public Property UseFrozenBitmapWerkbankSpielfeld As Boolean
            Get
                If IsNothing(FrozenBitmapWerkbankSpielfeld) Then
                    Return False
                Else
                    Return _UseFrozenBitmapWerkbankSpielfeld
                End If
            End Get
            Set(value As Boolean)
                _UseFrozenBitmapWerkbankSpielfeld = value
            End Set
        End Property



        Public xMin As Integer = 1
        Public yMin As Integer = 1
        Public zMin As Integer = 0
        Public xMax As Integer
        Public yMax As Integer
        Public zMax As Integer
        '
        ''' <summary>
        ''' Die Zuweisung einer der Werte X, Y oder Z speichert diese in der INI
        ''' was dort ein Event auslößt, was wiederum die Anzeige in der Oberfläche
        ''' aktualisiert. Da eine Änderung der Anzeige von Spielfeld, Editor und Werkbank
        ''' stets einze Zuweisung nach sich zieht, ist die Anzeige immer aktuell.
        ''' </summary>
        ''' <returns></returns>
        Public Property xMaxSteine As Integer
            Get
                Return INI.Rendering_AktMaxSteineX
            End Get
            Set(value As Integer)
                INI.Rendering_AktMaxSteineX = value
            End Set
        End Property
        Public Property yMaxSteine As Integer
            Get
                Return INI.Rendering_AktMaxSteineY
            End Get
            Set(value As Integer)
                INI.Rendering_AktMaxSteineY = value
            End Set
        End Property
        Public Property zMaxSteine As Integer
            Get
                Return INI.Rendering_AktMaxSteineZ
            End Get
            Set(value As Integer)
                INI.Rendering_AktMaxSteineZ = value
            End Set
        End Property

        Public renderRectLeft As Integer
        Public renderRectTop As Integer
        Public renderRectWidth As Integer
        Public renderRectHeight As Integer
        Public renderRect As Rectangle

        ''' <summary>
        ''' Das ist die aktuelle Breite der Steine
        ''' </summary>
        Public steinWidth As Integer
        '
        ''' <summary>
        ''' Das ist die aktuelle Höhe der Steine
        ''' </summary>
        Public steinHeight As Integer
        ''' <summary>
        ''' Das ist die halbe aktuelle Breite der Steine
        ''' </summary>
        Public steinWidthHalf As Integer
        '
        ''' <summary>
        ''' Das ist die halbe aktuelle Höhe der Steine
        ''' </summary>
        Public steinHeightHalf As Integer

        Public offset3DLeftJeEbene As Integer
        Public offset3DTopJeEbene As Integer

        Public offset3DLeftSumme As Integer
        Public offset3DTopSumme As Integer

        Public Property WidthSpielfeld As Integer
        Public Property HeightSpielfeld As Integer
        Public Property LeftSpielfeld As Integer
        Public Property TopSpielfeld As Integer

        Public Property RectSpielfeld As Rectangle
        '
        Public steinWidthLastCreated As Integer
        Public steinHeightLastCreated As Integer



    End Module

End Namespace