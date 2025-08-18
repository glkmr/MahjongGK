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
    Module SpielfeldManager

        'Hinweis: Die Daten, die hier nicht deklariert sind
        'stehen alle im Modul "SpielfeldDaten"

        ''' <summary>
        ''' Das ist die Initialisierung. Wird auf frmMain aufgerufen,
        ''' sobald das UserControl, auf das gezeichnet wird,
        ''' in frmMain eingehängt wird.
        ''' Wird solange im Frame-Takt aufgerufen, bis PaintSpielfeld_AktPermission = True gesetzt wird.
        ''' Wesentliche Aufgabe: Warten bis Daten da sind.
        ''' </summary>
        ''' <param name="rectAusgabe"></param>
        Public Sub UpdateSpielfeld_GivePermissionIfPossible(rectAusgabe As Rectangle)

            If Not IsNothing(aktSpielfeldInfo) AndAlso PaintSpielfeld_AktPermission Then

                UpdateSpielfeld(RectSpielfeld, UpdateSrc.Initialisierung)
            End If

        End Sub

        '
        Public Sub UpdateSpielfeld(rectSpielfeld As Rectangle, source As UpdateSrc)

            If IsNothing(aktSpielfeldInfo) Then
                Throw New Exception("Programmierfehler: aktSpielfeldInfo ist Nothing in SpielfeldManager.UpdateSpielfeld")
            End If


            With aktSpielfeldInfo
                xMax = .xMax 'das sind die Felder
                yMax = .yMax
                zMax = .zMax
                xMaxSteine = xMax \ 2 'jeder Stein belegt 2 Felder --> \ 2
                yMaxSteine = yMax \ 2
            End With

            If xMaxSteine < MJ_STEINE_SIDEBYSIDE_MIN OrElse yMaxSteine < MJ_STEINE_OVERANOTHER_MIN Then
                PaintSpielfeld_AktPermission = False
                If Debugger.IsAttached And Not IfRunningInIDE_ShowErrorMsgInsteadOfException Then
                    Throw New Exception($"Spielfeld Dimensionierung zu klein (xMax={xMax},yMax={yMax}) in SpielfeldManager.UpdateSpielfeld")
                Else
                    PaintSpielfeld_ErrorOccured = True
                    PaintSpielfeld_ShowErrorMessage = True
                    PaintSpielfeld_ErrorMessage = "Fehler in der Spielfelddimensionierung.||Wählen Sie ein anderes Spiel."
                    Exit Sub
                End If
            End If
            If xMaxSteine > MJ_STEINE_SIDEBYSIDE_MAX OrElse yMaxSteine > MJ_STEINE_OVERANOTHER_MAX Then
                PaintSpielfeld_AktPermission = False
                If Debugger.IsAttached And Not IfRunningInIDE_ShowErrorMsgInsteadOfException Then
                    Throw New Exception($"Spielfeld Dimensionierung zu zu groß (xMax={xMax},yMax={yMax}) in SpielfeldManager.UpdateSpielfeld")
                Else
                    PaintSpielfeld_ErrorOccured = True
                    PaintSpielfeld_ShowErrorMessage = True
                    PaintSpielfeld_ErrorMessage = "Fehler in der Spielfelddimensionierung.||Wählen Sie ein anderes Spiel."
                    Exit Sub
                End If
            End If

            Dim insideWidth As Integer = rectSpielfeld.Width - MJ_MARGIN_ABSOLUT_LEFT - MJ_MARGIN_ABSOLUT_RIGHT
            Dim insideHeight As Integer = rectSpielfeld.Height - MJ_MARGIN_ABSOLUT_TOP - MJ_MARGIN_ABSOLUT_BOTTOM

            'Das sind Startwerte
            steinWidth = insideWidth \ xMaxSteine + 2 'mindestens aufrunden + 1
            steinHeight = insideHeight \ yMaxSteine + 2

            'Dte Steinabmessungen müssen interativ berechnet werden, weil links
            'und unten Platz sein muss für die 3D-Verschiebung der Steine.
            'Der benötigte Platzbedarf ist abhängig von den Steinabmessungen.
            '
            Dim summeWidth As Integer
            Dim summeHeight As Integer

            Dim offset3DLeftSumme As Integer
            Dim offset3DTopSumme As Integer

            Do
                steinWidth -= 1
                steinHeight -= 1

                If steinWidth < MJ_GRAFIK_MIN_WIDTH OrElse steinHeight < MJ_GRAFIK_MIN_HEIGHT Then
                    If Debugger.IsAttached And Not IfRunningInIDE_ShowErrorMsgInsteadOfException Then
                        Throw New Exception($"Steine zu klein (steinWidth={steinWidth},steinHeight={steinHeight}) in SpielfeldManager.UpdateSpielfeld")
                    Else
                        PaintSpielfeld_ErrorOccured = True
                        PaintSpielfeld_ShowErrorMessage = True
                        PaintSpielfeld_ErrorMessage = $"TODO: Programmverhalten bei|Steine zu klein (steinWidth={steinWidth},steinHeight={steinHeight})|in SpielfeldManager.UpdateSpielfeld"
                        Exit Sub
                    End If

                End If

                offset3DLeftJeEbene = Math.Max(CInt(MJ_OFFSET3DFAKTOR_MAX_LEFT * steinWidth), MJ_OFFSET3D_MIN_LEFT)
                offset3DTopJeEbene = Math.Max(CInt(MJ_OFFSET3DFAKTOR_MAX_TOP * steinHeight), MJ_OFFSET3D_MIN_TOP)

                offset3DLeftSumme = offset3DLeftJeEbene * zMax
                offset3DTopSumme = offset3DTopJeEbene * zMax

                summeWidth = steinWidth * xMaxSteine + offset3DLeftSumme
                summeHeight = steinHeight * yMaxSteine + offset3DTopSumme

                If summeWidth <= insideWidth AndAlso summeHeight <= insideHeight Then
                    Exit Do
                End If
            Loop


            'Die Proportion der Steine ist jetzt willkürlich, da nur die maximal mögliche
            'Größe getrennt in den beiden Dimensionen berechnet ist.
            'Das jeweils andere Maß berechnen aus den Proportionen der Origiale.
            Dim propWidth As Integer = CInt(steinHeight * MJ_GRAFIK_FAKTOR_H_TO_W - 0.5) 'abrunden
            Dim propHeight As Integer = CInt(steinWidth * MJ_GRAFIK_FAKTOR_W_TO_H - 0.5)
            '
            If propWidth > steinWidth Then
                'dann bleibt die steinWidth und die SteinHeight wird angepasst
                steinHeight = propHeight
            Else
                'umgekehrt.
                steinWidth = propWidth
            End If
            '
            'Deckeln
            steinWidth = Math.Min(steinWidth, MJ_GRAFIK_MAX_WIDTH)
            steinHeight = Math.Min(steinHeight, MJ_GRAFIK_MAX_HEIGHT)
            '
            'und die Zahlen "gerade" machen, ggf 1 abziehen!
            'Jeder Stein belegt 4 Quadranten. Der Offset auf dem Spielfeld wird nach
            'Quadranten berechnet, also nach halben Steinen. Da ich mit Integer arbeite
            '(=Pixel) vermeide ich halbe Pixel, wenn ein Maß ungerade ist.
            steinWidth = steinWidth And Not 1
            steinHeight = steinHeight And Not 1

            steinWidthHalf = steinWidth \ 2
            steinHeightHalf = steinHeight \ 2
            '
            'neu berechnen
            summeWidth = steinWidth * xMaxSteine + offset3DLeftSumme
            summeHeight = steinHeight * yMaxSteine + offset3DTopSumme

            Dim deltaWidth As Integer = insideWidth - summeWidth
            Dim deltaHeigh As Integer = insideHeight - summeHeight

            'Ausgabefeld zentrieren
            renderRectLeft = deltaWidth \ 2 + MJ_MARGIN_ABSOLUT_LEFT
            renderRectTop = deltaHeigh \ 2 + MJ_MARGIN_ABSOLUT_TOP
            renderRectWidth = summeWidth
            renderRectHeight = summeHeight

            renderRect = New Rectangle(renderRectLeft, renderRectTop, renderRectWidth, renderRectHeight)

            If steinWidthLastCreated <> steinWidth OrElse steinHeightLastCreated <> steinHeight Then
                BitmapContainer.ChangeImagesSize(steinWidth, steinHeight)
                steinWidthLastCreated = steinWidth
                steinHeightLastCreated = steinHeight
            End If

        End Sub


    End Module
End Namespace
