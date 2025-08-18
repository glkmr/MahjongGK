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

Imports System.Runtime.CompilerServices

Namespace Spielfeld
    Public Module SpielfeldHelper


#Region "Sicherheitsgurt"

        Sub New()

            'Wenn die Enumeration "Stein" geändert wird, muß die Lockuptabelle GruppeLookup
            'in der #Region "Sonstige Helfer" angepasst werden.
            'Diese Prüfung erinnert daran :-)
            Dim enumLength As Integer = [Enum].GetValues(GetType(SteinIndexEnum)).Length
            If GruppeLookup.Length <> enumLength Then
                Throw New InvalidOperationException(
                $"Programmierfehler! GruppeLookup hat {GruppeLookup.Length} Einträge, Enum SteinIndexEnum hat {enumLength}. Tabelle anpassen!")
            End If
        End Sub

#End Region



#Region "Hilfsfunktionen"

        '''' <summary>
        '''' Gibt eine einzelne Ziffer aus einer Long-Zahl zurück.
        '''' </summary>
        '''' <param name="fb"></param>
        '''' <param name="divDigit"></param>
        '''' <returns></returns>
        '<MethodImpl(MethodImplOptions.AggressiveInlining)>
        'Private Function GetDigit(fb As Long, divDigit As Long) As Integer
        '    Return CInt((fb \ divDigit) Mod 10)
        'End Function
        ''
        '''' <summary>
        '''' Setzt eine einzelne Ziffer in einer Long-Zahl
        '''' </summary>
        '''' <param name="fb"></param>
        '''' <param name="divDigit"></param>
        '''' <param name="digitVal"></param>
        '''' <returns></returns>
        '<MethodImpl(MethodImplOptions.AggressiveInlining)>
        'Private Function SetDigit(fb As Long, divDigit As Long, digitVal As Integer) As Long
        '    If digitVal < 0 Then digitVal = 0
        '    If digitVal > 9 Then digitVal = 9
        '    fb -= ((fb \ divDigit) Mod 10) * divDigit
        '    fb += CLng(digitVal) * divDigit
        '    Return fb
        'End Function
        ''
        ''
        ''Ich betrachte die Zehn- und Hunderttausenderstelle des Feldbeschreibers als Bitfeld,
        ''in dem 8 verschiedene Flags (BIT0 bis BIT7) gespeichert sind.
        ''
        '' --- Bitfeld auslesen (8 Bits = 0–255) ---
        '<MethodImpl(MethodImplOptions.AggressiveInlining)>
        'Public Function GetBitfeld(ByVal zahl As Long) As Integer
        '    Return CInt((zahl \ DIV_DIG5) Mod 100)
        'End Function

        '' --- Einzelnes Bit setzen ---
        '<MethodImpl(MethodImplOptions.AggressiveInlining)>
        'Public Function SetBit(ByVal zahl As Long, ByVal bitMaske As Integer) As Long
        '    Dim feld As Integer = GetBitfeld(zahl)
        '    feld = feld Or bitMaske
        '    Return (zahl - (GetBitfeld(zahl) * DIV_DIG5)) + (feld * DIV_DIG5)
        'End Function

        '' --- Einzelnes Bit löschen ---
        '<MethodImpl(MethodImplOptions.AggressiveInlining)>
        'Public Function ClearBit(ByVal zahl As Long, ByVal bitMaske As Integer) As Long
        '    Dim feld As Integer = GetBitfeld(zahl)
        '    feld = feld And Not bitMaske
        '    Return (zahl - (GetBitfeld(zahl) * DIV_DIG5)) + (feld * DIV_DIG5)
        'End Function

        '' --- Prüfen, ob ein Bit gesetzt ist ---
        '<MethodImpl(MethodImplOptions.AggressiveInlining)>
        'Public Function IsBitSet(ByVal zahl As Long, ByVal bitMaske As Integer) As Boolean
        '    Return (GetBitfeld(zahl) And bitMaske) <> 0
        'End Function

        '' --- Direktwert ins Bitfeld schreiben (0–255) ---
        '<MethodImpl(MethodImplOptions.AggressiveInlining)>
        'Public Function SetBitfeld(ByVal zahl As Long, ByVal feldwert As Integer) As Long
        '    Return (zahl - (GetBitfeld(zahl) * DIV_DIG5)) + ((feldwert And &HFF) * DIV_DIG5)
        'End Function

#End Region


#Region "Funktionen zum Spielfeld aufbauen"


        '

#End Region

#Region "Zufallsfunktionen"

        Private ReadOnly _zufall As New Random()
        '
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetZufallszahl(min As Integer, max As Integer) As Integer
            Return _zufall.Next(min, max + 1)
        End Function

        Public Function GetZufallsStein() As SteinIndexEnum
            Return CType(_zufall.Next(SteinIndexEnum.Punkt1, SteinIndexEnum.JahrWinter + 1), SteinIndexEnum)
        End Function

        Dim _nextStein As SteinIndexEnum = SteinIndexEnum.Dummy
        '
        ''' <summary>
        ''' Stellt sicher, daß beim nächsten Aufruf von GetZufallsSteinPaarweise auch
        ''' wirklich der erste Stein eines Paares zurück gegeben wird.  
        ''' </summary>
        Public Sub ResetZufallsSteinPaarweise()
            _nextStein = SteinIndexEnum.Dummy
        End Sub
        '
        ''' <summary>
        ''' Liefert einen Zufallsstein und beim nächsten Aufruf einen Stein aus der gleichen Steingruppe.
        ''' Meistens ist das der gleiche Stein wie beim vorhergehendem Aufruf, nur bei den Sondersteinen
        ''' Stein.JahrXxx, Stein.WindXxx, Stein.DrachenXxx und Stein.BlüteXxxx wird ein anderer Stein der
        ''' Gruppe zurückgegeben.
        ''' </summary>
        ''' <returns></returns>
        Public Function GetZufallsSteinPaarweise() As SteinIndexEnum

            Dim stein As SteinIndexEnum

            If _nextStein <> SteinIndexEnum.Dummy Then

                stein = _nextStein
                _nextStein = SteinIndexEnum.Dummy
                Return stein

            Else

                stein = CType(_zufall.Next(SteinIndexEnum.Punkt1, SteinIndexEnum.JahrWinter + 1), SteinIndexEnum)

                Select Case stein
                    Case SteinIndexEnum.JahrFrühling, SteinIndexEnum.JahrHerbst, SteinIndexEnum.JahrSommer, SteinIndexEnum.JahrWinter
                        Do
                            _nextStein = CType(_zufall.Next(SteinIndexEnum.JahrFrühling, SteinIndexEnum.JahrWinter + 1), SteinIndexEnum)
                            If _nextStein <> stein Then
                                Return stein
                            End If
                        Loop

                    Case SteinIndexEnum.WindOst, SteinIndexEnum.WindNord, SteinIndexEnum.WindSüd, SteinIndexEnum.WindWest
                        Do
                            _nextStein = CType(_zufall.Next(SteinIndexEnum.WindOst, SteinIndexEnum.WindNord + 1), SteinIndexEnum)
                            If _nextStein <> stein Then
                                Return stein
                            End If
                        Loop

                    Case SteinIndexEnum.DrachenGrün, SteinIndexEnum.DrachenRot, SteinIndexEnum.DrachenWeiß
                        Do
                            _nextStein = CType(_zufall.Next(SteinIndexEnum.DrachenRot, SteinIndexEnum.DrachenWeiß + 1), SteinIndexEnum)
                            If _nextStein <> stein Then
                                Return stein
                            End If
                        Loop

                    Case SteinIndexEnum.BlüteBambus, SteinIndexEnum.BlüteChrisantheme, SteinIndexEnum.BlüteOrchidee, SteinIndexEnum.BlütePflaume
                        Do
                            _nextStein = CType(_zufall.Next(SteinIndexEnum.BlütePflaume, SteinIndexEnum.BlüteBambus + 1), SteinIndexEnum)
                            If _nextStein <> stein Then
                                Return stein
                            End If
                        Loop

                    Case Else
                        _nextStein = stein
                        Return stein
                End Select

            End If

        End Function
        '
        ''' <summary>
        ''' Gibt die Anzehl paare von Steinen zurück.
        ''' Wenn paare = 0, wird paare = 1
        ''' </summary>
        ''' <param name="paare"></param>
        ''' <returns></returns>
        Public Function GetZufallsSteinePaarweise(paare As Integer, Optional mischen As Boolean = True) As SteinIndexEnum()

            ResetZufallsSteinPaarweise()

            If paare = 0 Then
                paare = 1
            End If

            Dim arrPaare(paare * 2 - 1) As SteinIndexEnum
            For idx As Integer = 0 To arrPaare.GetUpperBound(0)
                arrPaare(idx) = GetZufallsSteinPaarweise()
            Next

            If mischen AndAlso paare > 1 Then
                For idx As Integer = arrPaare.GetUpperBound(0) To 1 Step -1
                    Dim idx2 As Integer = _zufall.Next(0, idx + 1)
                    Dim temp As SteinIndexEnum = arrPaare(idx)
                    arrPaare(idx2) = arrPaare(idx)
                    arrPaare(idx2) = temp
                Next
            End If

            Return arrPaare

        End Function

        Public Function GetZufallTrueFalse() As Boolean
            'Die "2" ist ausgeschlossen. Next(0, 2) muss ich mir vorstellen wie:
            'von 0.00000000 bis 1.99999999 ergibt abgeschnitten (Floor) 0 oder 1.
            'Verteilung True zu False ist 50:50
            Return _zufall.Next(0, 2) = 1
        End Function
        '
        Public Function GetZufallMove() As Move
            Return CType(_zufall.Next(0, 3), Move)
        End Function

        Public Function GetZufall0To9() As Integer
            Return _zufall.Next(0, 10)
        End Function

        ''' <summary>
        ''' Gibt eine Zufallszahl größer als 0 und kleiner als 1 zurück
        ''' </summary>
        ''' <returns></returns>
        Public Function GetZufallDouble0To1() As Double
            Return _zufall.NextDouble
        End Function

        Public Function GetZufallDirection() As Direction
            Return CType(_zufall.Next(0, 8), Direction)
        End Function


#End Region

#Region "Sonstige Helfer"

        ' Lookup-Tabelle: Index = Enumwert, Inhalt = Gruppennummer
        ' Muss in der Reihenfolge der Enum-Deklaration stehen!
        ' Hintergrund: Die Regel Steine mit gleichem Symbol können paarweise
        ' entfernt werden gilt meistens, aber nicht immer.
        ' Die Drachen- Blüten und Jahressymbole sind optisch unterschiedlich, können
        ' aber paarweise entfernt werden.
        ' Deshalb gibt es den Gruppenindex und das hier ist die Übersetzungstabelle
        ' vom Steinindex (Enumeration Stein) zum Gruppenindex.

        Private ReadOnly GruppeLookup As Integer() = {
        0,  ' Dummy_0
        1,  ' Punkt1_1
        2,  ' Punkt2_2
        3,  ' Punkt3_3
        4,  ' Punkt4_4
        5,  ' Punkt5_5
        6,  ' Punkt6_6
        7,  ' Punkt7_7
        8,  ' Punkt8_8
        9,  ' Punkt9_9
        10, ' Bambus1_10
        11, ' Bambus2_11
        12, ' Bambus3_12
        13, ' Bambus4_13
        14, ' Bambus5_14
        15, ' Bambus6_15
        16, ' Bambus7_16
        17, ' Bambus8_17
        18, ' Bambus9_18
        19, ' Symbol1_19
        20, ' Symbol2_20
        21, ' Symbol3_21
        22, ' Symbol4_22
        23, ' Symbol5_23
        24, ' Symbol6_24
        25, ' Symbol7_25
        26, ' Symbol8_26
        27, ' Symbol9_27
        28, ' WindOst_28
        28, ' WindSüd_28
        28, ' WindWest_28
        28, ' WindNord_28
        29, ' DrachenRot_29
        29, ' DrachenGrün_29
        29, ' DrachenWeiß_29
        30, ' BlütePflaume_30
        30, ' BlüteOrchidee_30
        30, ' BlüteChrisantheme_30
        30, ' BlüteBambus_30
        31, ' JahrFrühling_31
        31, ' JahrSommer_31
        31, ' JahrHerbst_31
        31  ' JahrWinter_31
    }

        Public Function GetGruppe(stein As SteinIndexEnum) As Integer
            Return GruppeLookup(CInt(stein))
        End Function


#End Region

    End Module

End Namespace