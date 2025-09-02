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
        Public Property aktSpielfeldInfo As SpielfeldInfo = Nothing
        '

        Public xMin As Integer = 1
        Public yMin As Integer = 1
        Public zMin As Integer = 0
        Public xMax As Integer
        Public yMax As Integer
        Public xMaxSteine As Integer
        Public yMaxSteine As Integer
        Public zMax As Integer

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