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

#Disable Warning IDE0079
#Disable Warning IDE1006

Public Class frmToolBox

    Public Shared FrmToolboxWidth As Integer = frmToolBox.Width
    Public Shared FrmToolboxHeight As Integer = frmToolBox.Height

    Private Sub frmToolBox_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Me.Location = INI.ToolBox_Rectangle.Location
        Me.EnsureLocationVisibleOnAnyScreen()
    End Sub
    Private Sub frmToolBox_LocationChanged(sender As Object, e As EventArgs) Handles Me.LocationChanged
        INI.ToolBox_Rectangle = New Rectangle(Me.Location, Me.Size)
    End Sub

End Class