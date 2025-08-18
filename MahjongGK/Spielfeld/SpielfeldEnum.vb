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


'Zentrale Deklaration programmweit geltender Enumerationen
Namespace Spielfeld

    ''' <summary>
    ''' Enumeration der verwendeten Unterverzeichnisse in "C:\Users\aktueller User\MahjongGK\SubDefault.value.ToString"
    ''' Verwendung: Entweder über Dim Path As String = INI.AppDataDefault(.....)
    ''' oder durch Nutzung (im Modul INI) einer Public Property Kopier_VorlageFürPfade As String
    ''' Die gewünschten Pfade werden automatisch angelegt.
    ''' </summary>
    Public Enum AppDataSubDir
        None
        INI
        Steine
    End Enum
    ''' <summary>
    ''' Enumeration der verwendeten Unterverzeichnisse in "C:\Users\aktueller User\MahjongGK\SubDefault.value.ToString\SubSubDefault.value.ToString"
    ''' Verwendung wie AppDataSubDir
    ''' </summary>
    Public Enum AppDataSubSubDir
        None
        letztesSpiel
        Layout
    End Enum

    ''' <summary>
    ''' Enumeration der verwendeten Dateinamen.
    ''' Die Endung mit einem Unterstrich abtrennen.
    ''' Die Endung muss 3 Zeichen lang sein.
    ''' </summary>
    Public Enum AppDataFileName
        None
        Steininfos_xml
    End Enum

    Public Enum AppDataTimeStamp
        None
        Add
        LookForLastTimeStamp
    End Enum

    Public Enum SteinIndexEnum
        '        _x ist die Gruppenzugehörigkeit
        '        siehe FeldbeschreiberHelper.GetGruppe
        '        undFeldbeschreiberHelper.GruppeLookup 
        Dummy '  _0
        Punkt1 ' _1
        Punkt2 ' _2
        Punkt3 ' _3
        Punkt4 ' _4
        Punkt5 ' _5
        Punkt6 ' _6
        Punkt7 ' _7
        Punkt8 ' _8
        Punkt9 ' _9
        Bambus1 ' _10
        Bambus2 ' _11
        Bambus3 ' _12
        Bambus4 ' _13
        Bambus5 ' _14
        Bambus6 ' _15
        Bambus7 ' _16
        Bambus8 ' _17
        Bambus9 ' _18
        Symbol1 ' _19
        Symbol2 ' _20
        Symbol3 ' _21
        Symbol4 ' _22
        Symbol5 ' _23
        Symbol6 ' _24
        Symbol7 ' _25
        Symbol8 ' _26
        Symbol9 ' _27
        WindOst ' _28
        WindSüd ' _28
        WindWest ' _28
        WindNord ' _28
        DrachenRot ' _29
        DrachenGrün ' _29
        DrachenWeiß ' _29
        BlütePflaume ' _30
        BlüteOrchidee ' _30
        BlüteChrisantheme ' _30
        BlüteBambus ' _30
        JahrFrühling ' _31
        JahrSommer ' _31
        JahrHerbst ' _31
        JahrWinter ' _31
    End Enum

    Public Enum SteinStatus
        ''' <summary>
        ''' Wenn das Programm innerhalb der IDE läuft, kann das Programmverhalten
        ''' über den Schalter "unsichtbare Steine sichtbar machen" geändert werden.
        ''' Es werden dann halbtransparente graue Steine mit Rotem Kreis und Indexnummer
        ''' angezeigt.
        ''' </summary>
        Unsichtbar          ' nicht sichtbar (Geistergrafik möglich)
        Normal
        Selected
        ClickableOne        ' einzeln klickbar
        ClickablePartOfPair ' klickbar und Teil eines gültigen Paars
        Locked
        NotUnsed            ' nur für Schwierigkeitslevel-Auswahl
        MissingSecond       ' im Editor, wenn Partnerstein fehlt
        Reserve1            ' Geistergrafik mit grünem Kreis
        Reserve2            ' Geistergrafik mit blauem Kreis
    End Enum

    Public Enum VisiblePart
        none ' nicht sichtbar
        OL ' oben links, linke oberstes Viertel sichtbar
        OM ' oben Mitte, obere Hälfte sichtbar
        [OR] ' oben rechts, rechtes oberstes Viertel sichtbar
        ML ' Mitte links, linke Hälfte sichtbar
        MM ' Mitte Mitte, alles sichtbar
        MR ' Mitte rechts, rechte Hälfte sichtbar
        UL ' unten links, linkes unterstes Viertel sichtbar
        UM ' unten Mitte, untere Hälfte sichtbar
        UR ' unten rechts, rechtes unterstes Viertel sichtbar
    End Enum
    '
    ''' <summary>
    ''' Richtung der Suche nach einem freiem Platz für einen
    ''' Stein ausgehend von einer Ausgangsstellung.
    ''' </summary>
    Public Enum Direction
        Up
        UpRight
        Right
        DownRight
        Down
        DownLeft
        Left
        UpLeft
        RightUp = UpRight
        RightDown = DownRight
        LeftDown = DownLeft
        LeftUp = UpLeft
        LBnd = Up    'um in einer ForNext-Schleife alle Himmelsrichtungen durchzugehen.
        UBnd = UpLeft
    End Enum

    Public Enum Move
        NoMove
        LeftOrUp
        RightOrDown
    End Enum

    Public Enum Animation
        None
        Erscheinen
        Verblassen
        Wachsen
        ErscheinenOhneUGrd
        VerschwindenUniUGrdfarbe
        Schrumpfen
        TaumelSchrumpfen
        WachsenPauseVerblassen
        DehnSchrumpfen
        DehnRotierSchrumpfen
        DehnTaumelSchrumpfen
        'ZweiBilderSchrumpfen
        'ZweiBilderDehnSchrumpfen
        'ZweiBilderDehnRotierSchrumpfen
        'ZweiBilderDehnTaumelSchrumpfen
        'ZweiBilderDehnZufallsanimation
        'ZweiBilderErscheinenOhneUGrd
        'ZweiBilderVerschwindenUniUGrdfarbe
        Test
    End Enum

    Public Enum Verdeckt
        Keine = 0  '0000
        LinksOben = 1  '0001
        RechtsOben = 2  '0010
        LinksUnten = 4  '0100
        RechtsUnten = 8  '1000
        Alle = 15 '1111
        OhneLinksOben = 14 '1110
        OhneRechtsOben = 13 '1101
        OhneLinksUnten = 11 '1011 
        OhneRechtsUnten = 7  '0111
        NurLinks = 5  '0101
        NurRechts = 10 '1010
        NurOben = 3  '0011
        NurUnten = 12 '1100
        LinksObenRechtsUnten = 6 '0110
        LinksUntenRechtsOben = 9 '1001
    End Enum

    'Die Umkehrung ist diese Enumeration

    Public Enum Sichtbar
        Keine = 15 '1111
        LinksOben = 14 '1110
        RechtsOben = 13 '1101
        LinksUnten = 11 '1011
        RechtsUnten = 7  '0111
        Alle = 0  '0000
        OhneLinksOben = 1  '0001
        OhneRechtsOben = 2  '0010
        OhneLinksUnten = 4  '0100 
        OhneRechtsUnten = 8  '1000
        NurLinks = 10 '1010
        NurRechts = 5  '0101
        NurOben = 12 '1100
        NurUnten = 3  '0011
        LinksObenRechtsUnten = 9 '1001
        LinksUntenRechtsOben = 6 '0110
    End Enum

    Public Enum Quadrant
        LO = 1
        RO = 2
        LU = 4
        RU = 8
    End Enum

    Public Enum UpdateSrc
        Initialisierung
        PaintSpielfeld_UpdteSpielfeld_IsSet
        PaintEvent
    End Enum

End Namespace