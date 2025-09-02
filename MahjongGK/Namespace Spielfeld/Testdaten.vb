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
    '
    ''' <summary>
    ''' Hier sind Testdaten drin, und Code, den ich zur Erstellung dieser Testdaten
    ''' bei der Programmentwicklung gebraucht habe, die aber im fertigem Programm ohne
    ''' Bedeutung sind.
    ''' </summary>
    Module Testdaten

        Public Sub TestDaten_Spielfeld_3_x_3_x_1()

            Dim newSpielfeldInfo As New SpielfeldInfo(New Triple(3, 3, 1))

            With newSpielfeldInfo
                Dim centerXyz As Triple = .GetSpielfeldCenter(0)
                .AddSteinToSpielfeld(SteinIndexEnum.Bambus1, centerXyz)

                Dim tplr As Triple
                tplr = .SearchPlace(centerXyz, Direction.Left)
                If tplr.Valide = ValidePlace.Yes Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus2, tplr)
                End If

                tplr = .SearchPlace(centerXyz, Direction.LeftUp)
                If tplr.Valide = ValidePlace.Yes Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus3, tplr)
                End If

                tplr = .SearchPlace(centerXyz, Direction.RightDown)
                If tplr.Valide = ValidePlace.Yes Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus4, tplr)
                End If

                tplr = .SearchPlace(tplr, Direction.Up)
                If tplr.Valide = ValidePlace.Yes Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus5, tplr)
                End If

                tplr = .SearchPlace(tplr, Direction.Up)
                If tplr.Valide = ValidePlace.Yes Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus6, tplr)
                End If

            End With




            'DebugHilfen.Print3DArrayToTxtOutputForm(aktSpielfeldInfo)
            '   DebugShowArrFBMain(arrFBMain, 0)

        End Sub
        Public Sub TestDaten_Spielfeld_Variablel_zum_Debuggen()

            Dim newSpielfeldInfo As New SpielfeldInfo(New Triple(30, 10, 10))

            Spielfeld.aktSpielfeldInfo = newSpielfeldInfo
            Spielfeld.FrameSchedulerModul.PaintSpielfeld_AktPermission = True

            Dim wbs As Werkstück = Umfeld.Werkstück_Rechteck(New Triple(30, 10, 10))

            newSpielfeldInfo.AddWerkstückToSpielfeld(wbs, New Triple(1, 1, 0))

            'newSpielfeldInfo.AddWerkstückToSpielfeld(wbs, New Triple(15, 5, 0))
            'With newSpielfeldInfo

            '    Dim tplr As New Triple

            '    For idx As Integer = PositionEnum.LBnd To PositionEnum.UBnd
            '        Dim tpl As Triple = .GetPositionImSpielfeld(DirectCast(idx, PositionEnum), New Triple(5, 5, 0), 5, 5)
            '        .AddSteinToSpielfeld(DirectCast(idx + 1, SteinIndexEnum), tpl, tmpDebug:=idx)
            '        DebugStep.WaitForStep()
            '    Next
            'End With
        End Sub

        'DebugHilfen.Print3DArrayToTxtOutputForm(newSpielfeldInfo)


    End Module
End Namespace
