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
    ''' Hier sind die Zeichenroutinen angesiedelt.
    ''' Auch das ist keine Klasse, weil von vorneherein feststeht, daß keine
    ''' Instanzierung notwenig ist.
    ''' </summary>
    Module SpielfeldRenderer

        'Hinweis: Die Daten, auf die hier ohne Deklaration zugegriffen wird, stehen
        'im Modul "SpielfeldDaten"

        Public Sub DoPaintSpielfeld_Paint(e As PaintEventArgs, rectOutput As Rectangle, timeDifferenzFaktor As Double)

            If INI.Spielfeld_DrawBackgroundColor Then
                e.Graphics.Clear(INI.Spielfeld_BackgroundColor)
            End If

            If INI.Spielfeld_DrawFraming Then
                Dim shrink As Single = INI.Spielfeld_FramingThickness / 2.0F
                Dim shrinkLeft As Single = MJ_MARGIN_ABSOLUT_LEFT_HALF + shrink
                Dim shrinkRight As Single = -shrinkLeft - MJ_MARGIN_ABSOLUT_RIGHT_HALF - shrink
                Dim shrinkTop As Single = MJ_MARGIN_ABSOLUT_TOP_HALF + shrink
                Dim shrinkBottom As Single = -shrinkTop - MJ_MARGIN_ABSOLUT_BOTTOM_HALF - shrink

                Dim shrinkRect As New RectangleF(rectOutput.X + shrinkLeft, rectOutput.Y + shrinkTop, rectOutput.Width + shrinkRight, rectOutput.Height + shrinkBottom)

                Using pen As New Pen(INI.Spielfeld_FramingColor, INI.Spielfeld_FramingThickness)
                    e.Graphics.DrawRectangle(pen, shrinkRect.X, shrinkRect.Y, shrinkRect.Width, shrinkRect.Height)
                End Using
            End If

            If INI.Spielfeld_DrawRenderRect Then
                Using pen As New Pen(INI.Spielfeld_RenderRectColor)
                    e.Graphics.DrawRectangle(pen, renderRect)
                End Using
            End If

            Dim aktSteinInfo As SteinInfo

            Dim toggleVergleichsflag As Boolean = aktSpielfeldInfo.GetFirstToggleFlagValue

            'Die Vaiable, auf die hier zugegriffen wird, und die nicht hier
            'deklariert sind, stehen alle im Modul SpielfeldDaten.
            With aktSpielfeldInfo

                'Dim debugInfo As String = String.Empty

                For z As Integer = zMin To zMax
                    For x As Integer = xMin To xMax
                        For y As Integer = yMin To yMax

                            If .arrFB(x, y, z) = 0 Then
                                'unbelegtes Feld
                                Continue For
                            End If

                            'Was hier am Anfang passiert ist etwas komplexer, tricky und schnell.
                            'Es ist genau beschrieben unter Spielfeld/liesmich.txt
                            'Stichworte: XOffset, YOffset, ToggleFlag
                            If toggleVergleichsflag <> .GetToggleFlag(x, y, z) Then
                                'bereits verarbeitetes Feld
                                Continue For
                            End If

                            'als bearbeitet markieren
                            .ToggleToggleFlag(x, y, z)
                            '
                            aktSteinInfo = .SteinInfos(.GetIndexStein(x, y, z))

                            With aktSteinInfo
                                If .AnimShowAnimated Then
                                    PaintAnimatedStein(e, rectOutput, timeDifferenzFaktor, aktSteinInfo, New Triple(x, y, z))
                                Else
                                    Dim left As Integer = renderRectLeft + (steinWidthHalf * (x - 1) - (offset3DLeftJeEbene * z))
                                    Dim top As Integer = renderRectTop + (steinHeightHalf * (y - 1) - (offset3DTopJeEbene * z))
                                    ''Debug
                                    'debugInfo &= $"x={x},y={y},z={z},arrFB-SteinIdx={ aktSpielfeldInfo.GetIndexStein(x, y, z)},SII={aktSteinInfo.SteinInfoIndex},SI={aktSteinInfo.SteinIndex}|"
                                    ''/Debug
                                    e.Graphics.DrawImage(BitmapContainer.GetBitmap(.SteinStatusUsed, .SteinIndex), left, top)
                                End If
                            End With
                        Next
                    Next
                Next
                'Debug.Print(debugInfo)
            End With
        End Sub

        Private Sub PaintAnimatedStein(e As PaintEventArgs, rectOutput As Rectangle, timeDifferenzFaktor As Double, aktSteinInfo As SteinInfo, pos3D As Triple)

            Dim left As Integer = renderRectLeft + (steinWidthHalf * pos3D.x) - (offset3DLeftJeEbene * pos3D.z)
            Dim top As Integer = renderRectTop + (steinHeightHalf * pos3D.y) - (offset3DTopJeEbene * pos3D.z)

            Dim rectStein As New Rectangle(left, top, steinWidth, steinHeight)

        End Sub

    End Module
End Namespace