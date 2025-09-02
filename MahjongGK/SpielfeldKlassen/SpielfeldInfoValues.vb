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

Public Class SpielfeldInfoValues

    Public Enum SfInfoValues
        Spielfeld
        Werkstatt
    End Enum

    Sub New()

    End Sub

    Public Property VisibleUserControl As frmMain.VisibleUserControl
    Public Property aktSpielfeldInfo As SpielfeldInfo = Nothing

    Public Property xMin As Integer
    Public Property yMin As Integer
    Public Property zMin As Integer
    Public Property xMax As Integer
    Public Property yMax As Integer
    Public Property xMaxSteine As Integer
    Public Property yMaxSteine As Integer
    Public Property zMax As Integer

    Public Property renderRectLeft As Integer
    Public Property renderRectTop As Integer
    Public Property renderRectWidth As Integer
    Public Property renderRectHeight As Integer
    Public Property renderRect As Rectangle

    Public Property steinWidth As Integer
    Public Property steinHeight As Integer
    Public Property steinWidthHalf As Integer
    Public Property steinHeightHalf As Integer
    Public Property offset3DLeftJeEbene As Integer
    Public Property offset3DTopJeEbene As Integer
    Public Property WidthSpielfeld As Integer
    Public Property HeightSpielfeld As Integer
    Public Property LeftSpielfeld As Integer
    Public Property TopSpielfeld As Integer

    Public Property RectSpielfeld As Rectangle
    '
    Public Property steinWidthLastCreated As Integer
    Public Property steinHeightLastCreated As Integer


    Public Sub CopySpielfeldInfoValues_To_SpielfeldDaten()

        Me.VisibleUserControl = VisibleUserControl
        Me.aktSpielfeldInfo = aktSpielfeldInfo

        Me.xMin = xMin
        Me.yMin = yMin
        Me.zMin = zMin
        Me.xMax = xMax
        Me.yMax = yMax
        Me.xMaxSteine = xMaxSteine
        Me.yMaxSteine = yMaxSteine
        Me.zMax = zMax

        Me.renderRectLeft = renderRectLeft
        Me.renderRectTop = renderRectTop
        Me.renderRectWidth = renderRectWidth
        Me.renderRectHeight = renderRectHeight
        Me.renderRect = renderRect

        Me.steinWidth = steinWidth
        Me.steinHeight = steinHeight
        Me.steinWidthHalf = steinWidthHalf
        Me.steinHeightHalf = steinHeightHalf
        Me.offset3DLeftJeEbene = offset3DLeftJeEbene
        Me.offset3DTopJeEbene = offset3DTopJeEbene
        Me.WidthSpielfeld = WidthSpielfeld
        Me.HeightSpielfeld = HeightSpielfeld
        Me.LeftSpielfeld = LeftSpielfeld
        Me.TopSpielfeld = TopSpielfeld

        Me.RectSpielfeld = RectSpielfeld

        Me.steinWidthLastCreated = steinWidthLastCreated
        Me.steinHeightLastCreated = steinHeightLastCreated

        VisibleUserControl = Me.VisibleUserControl
        aktSpielfeldInfo = Me.aktSpielfeldInfo

        xMin = Me.xMin
        yMin = Me.yMin
        zMin = Me.zMin
        xMax = Me.xMax
        yMax = Me.yMax
        xMaxSteine = Me.xMaxSteine
        yMaxSteine = Me.yMaxSteine
        zMax = Me.zMax

        renderRectLeft = Me.renderRectLeft
        renderRectTop = Me.renderRectTop
        renderRectWidth = Me.renderRectWidth
        renderRectHeight = Me.renderRectHeight
        renderRect = Me.renderRect

        steinWidth = Me.steinWidth
        steinHeight = Me.steinHeight
        steinWidthHalf = Me.steinWidthHalf
        steinHeightHalf = Me.steinHeightHalf
        offset3DLeftJeEbene = Me.offset3DLeftJeEbene
        offset3DTopJeEbene = Me.offset3DTopJeEbene
        WidthSpielfeld = Me.WidthSpielfeld
        HeightSpielfeld = Me.HeightSpielfeld
        LeftSpielfeld = Me.LeftSpielfeld
        TopSpielfeld = Me.TopSpielfeld

        RectSpielfeld = Me.RectSpielfeld

        steinWidthLastCreated = Me.steinWidthLastCreated
        steinHeightLastCreated = Me.steinHeightLastCreated

    End Sub
    Public Sub CopySpielfeldDaten_To_SpielfeldInfoValues_()

        VisibleUserControl = Me.VisibleUserControl
        aktSpielfeldInfo = Me.aktSpielfeldInfo

        xMin = Me.xMin
        yMin = Me.yMin
        zMin = Me.zMin
        xMax = Me.xMax
        yMax = Me.yMax
        xMaxSteine = Me.xMaxSteine
        yMaxSteine = Me.yMaxSteine
        zMax = Me.zMax

        renderRectLeft = Me.renderRectLeft
        renderRectTop = Me.renderRectTop
        renderRectWidth = Me.renderRectWidth
        renderRectHeight = Me.renderRectHeight
        renderRect = Me.renderRect

        steinWidth = Me.steinWidth
        steinHeight = Me.steinHeight
        steinWidthHalf = Me.steinWidthHalf
        steinHeightHalf = Me.steinHeightHalf
        offset3DLeftJeEbene = Me.offset3DLeftJeEbene
        offset3DTopJeEbene = Me.offset3DTopJeEbene
        WidthSpielfeld = Me.WidthSpielfeld
        HeightSpielfeld = Me.HeightSpielfeld
        LeftSpielfeld = Me.LeftSpielfeld
        TopSpielfeld = Me.TopSpielfeld

        RectSpielfeld = Me.RectSpielfeld

        steinWidthLastCreated = Me.steinWidthLastCreated
        steinHeightLastCreated = Me.steinHeightLastCreated

    End Sub

End Class
