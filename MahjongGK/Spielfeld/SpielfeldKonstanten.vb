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
    Module SpielfeldKonstanten


        'Die minimal mögliche Große von frmMain wird hierau bezogen.
        Public Const MJ_SPIELFELD_MIN_WIDTH As Integer = 600
        Public Const MJ_SPIELFELD_MIN_HEIGHT As Integer = 400

        Public Const MJ_STEINE_SIDEBYSIDE_MIN As Integer = 3
        Public Const MJ_STEINE_OVERANOTHER_MIN As Integer = 3

        'Nach meinen Recherchen nach haben Mahjong-Layouts haben typischerweise
        'max. 15 bis 20 Steine nebeneinander und 8–12 Steine übereinander.
        '
        'Die hier angegebenen Grenzen stellen sicher, daß das Feld noch angezeigt
        'werden kann. Ob das sinnvoll ist, ist eine andere Frage. 
        'Und ob der Rechner ein derart großes und vollgepflastertes Feld noch
        'verdauen kann, ist wieder eine andere Frage.

        'Steine nebeneinander
        Public Const MJ_STEINE_SIDEBYSIDE_MAX As Integer = 30

        'ergibt bei 30 Steinen nebeneinander 15 Steine übereinander
        Public Const MJ_STEINE_OVERANOTHER_MAX As Integer =
                    (MJ_STEINE_SIDEBYSIDE_MAX * MJ_GRAFIK_ORG_WIDTH * MJ_SPIELFELD_MIN_HEIGHT) \
                    (MJ_GRAFIK_ORG_HEIGHT * MJ_SPIELFELD_MIN_WIDTH)
        'Hinweis zu obiger Formel: Es wird hier mit reinen Integer-Operationen gearbeitet.
        'Hier kann es ganz schnell passieren, daß ein Zwischenergebnis klein wird und das
        'Endergebnis durch die Rundung des Zwischenergenisses zerschossen, oder erschossen (=0)
        'wird. Daher zuerst alles multiplizieren und dann erst dividieren. 


        Public Const MJ_GRAFIK_ORG_WIDTH As Integer = 198
        Public Const MJ_GRAFIK_ORG_HEIGHT As Integer = 252


        Public Const MJ_GRAFIK_MAX_WIDTH As Integer = 198
        Public Const MJ_GRAFIK_MAX_HEIGHT As Integer = 252

        'Das sind "Notbremswerte", für den Spielbetrieb unbrauchbar klein.
        Public Const MJ_GRAFIK_MIN_WIDTH As Integer = 10
        Public Const MJ_GRAFIK_MIN_HEIGHT As Integer = 12

        Public Const MJ_GRAFIK_FAKTOR_H_TO_W As Double = MJ_GRAFIK_ORG_WIDTH / MJ_GRAFIK_ORG_HEIGHT
        Public Const MJ_GRAFIK_FAKTOR_W_TO_H As Double = MJ_GRAFIK_ORG_HEIGHT / MJ_GRAFIK_ORG_WIDTH

        Public Const MJ_MARGIN_ABSOLUT_LEFT As Integer = 10 'geradzahlige Werte nehmen
        Public Const MJ_MARGIN_ABSOLUT_TOP As Integer = 10  'für alle 4
        Public Const MJ_MARGIN_ABSOLUT_RIGHT As Integer = 10
        Public Const MJ_MARGIN_ABSOLUT_BOTTOM As Integer = 10

        'Die Hälfte. Wird benötigt um den Rahmen (falls er gezeichnet wird)
        'um das Spielfeld mittig auf den Rand zu zeichnen
        Public Const MJ_MARGIN_ABSOLUT_LEFT_HALF As Single = MJ_MARGIN_ABSOLUT_LEFT / 2
        Public Const MJ_MARGIN_ABSOLUT_TOP_HALF As Single = MJ_MARGIN_ABSOLUT_TOP / 2
        Public Const MJ_MARGIN_ABSOLUT_RIGHT_HALF As Single = MJ_MARGIN_ABSOLUT_RIGHT / 2
        Public Const MJ_MARGIN_ABSOLUT_BOTTOM_HALF As Single = MJ_MARGIN_ABSOLUT_BOTTOM / 2

        Public Const MJ_COLOR_BG_DEFAULT As Integer = &HFFD0D0D0 'helleres Grau

        'Das ist die Verschiebung je Ebene der Steine bei maximaler Steingröße
        Public Const MJ_OFFSET3D_MAX_LEFT As Integer = 10
        Public Const MJ_OFFSET3D_MAX_TOP As Integer = (MJ_OFFSET3D_MAX_LEFT * MJ_GRAFIK_ORG_HEIGHT) \ MJ_GRAFIK_ORG_WIDTH
        Public Const MJ_OFFSET3D_MIN_LEFT As Integer = 1
        Public Const MJ_OFFSET3D_MIN_TOP As Integer = 1

        Public Const MJ_OFFSET3DFAKTOR_MAX_LEFT As Double = MJ_OFFSET3D_MAX_LEFT / MJ_GRAFIK_ORG_WIDTH
        Public Const MJ_OFFSET3DFAKTOR_MAX_TOP As Double = MJ_OFFSET3D_MAX_TOP / MJ_GRAFIK_ORG_HEIGHT



        Public Const DIV_DIG1 As Integer = 1                 ' 10^0
        Public Const DIV_DIG2 As Integer = 10                ' 10^1
        Public Const DIV_DIG3 As Integer = 100               ' 10^2
        Public Const DIV_DIG4 As Integer = 1000              ' 10^3
        Public Const DIV_DIG5 As Integer = 10000             ' 10^4
        Public Const DIV_DIG6 As Integer = 100000            ' 10^5
        Public Const DIV_DIG7 As Integer = 1000000            ' 10^6
        Public Const DIV_DIG8 As Integer = 10000000           ' 10^7
        Public Const DIV_DIG9 As Integer = 100000000          ' 10^8
        Public Const DIV_DIG10 As Long = 1000000000L        ' 10^9
        Public Const DIV_DIG11 As Long = 10000000000L       ' 10^10
        Public Const DIV_DIG12 As Long = 100000000000L      ' 10^11
        Public Const DIV_DIG13 As Long = 1000000000000L     ' 10^12
        Public Const DIV_DIG14 As Long = 10000000000000L    ' 10^13
        Public Const DIV_DIG17 As Long = 100000000000000000L    ' 10^17
        Public Const DIV_DIG18 As Long = 1000000000000000000L   ' 10^18


        ' Konstanten für einzelne Bits (Bitmasken)
        Public Const BIT0 As Integer = 1       ' Einerstelle Bit 0
        Public Const BIT1 As Integer = 2       ' Einerstelle Bit 1
        Public Const BIT2 As Integer = 4       ' Einerstelle Bit 2
        Public Const BIT3 As Integer = 8       ' Einerstelle Bit 3

        Public Const BIT4 As Integer = 16      ' Zehnerstelle Bit 0
        Public Const BIT5 As Integer = 32      ' Zehnerstelle Bit 1
        Public Const BIT6 As Integer = 64      ' Zehnerstelle Bit 2
        Public Const BIT7 As Integer = 128     ' Zehnerstelle Bit 3

        'Flags:
        Public Const FLAG_XOffset As Integer = BIT0
        Public Const FLAG_YOffset As Integer = BIT1
        Public Const FLAG_ToggleFlag As Integer = BIT2

        Public Const FLAG_Frei3 As Integer = BIT3
        Public Const FLAG_Frei4 As Integer = BIT4
        Public Const FLAG_Frei5 As Integer = BIT5
        Public Const FLAG_Frei6 As Integer = BIT6
        Public Const FLAG_Frei7 As Integer = BIT7



    End Module
End Namespace