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
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006



''' <summary>
''' TripleResult, wie Triple zusätzlich mit der Enumeration FoundResult
''' </summary>
Public Class TripleR
    Inherits Triple  ' <-- Vererbung von Triple

    ''' <summary>
    ''' Ergebnis der Suche nach einem freiem Platz für einen Stein.
    ''' </summary>
    Public Enum Result
        NotSet
        FoundFreePlace
        NoFundamentFound 'in der Schicht darunter gibt es keinen
        '                 Stein, auf den gebaut werden könnte.
        OutsideBorder    'Spielfeldrand erreicht.
    End Enum

    Public Property Found As Result

    Sub New()
        MyBase.New() ' ruft den Konstruktor der Basisklasse Triple auf
    End Sub

    Sub New(x As Integer, y As Integer, z As Integer)
        MyBase.New(x, y, z) ' Konstruktor von Triple aufrufen für x,y,z
        Me.Found = Result.NotSet
    End Sub
    Sub New(x As Integer, y As Integer, z As Integer, result As Result)
        MyBase.New(x, y, z) ' Konstruktor von Triple aufrufen für x,y,z
        Me.Found = result
    End Sub

    Sub New(triple As Triple, fr As Result)
        MyBase.New(triple.x, triple.y, triple.z)
        Me.Found = fr
    End Sub

    Public Function ToTriple() As Triple
        Return New Triple(x, y, z)
    End Function

    ' Alle anderen Methoden von Triple sind automatisch auch in TripleR verfügbar
    ' Methoden in Triple, die in TripleR anders sein sollen, überschreiben mit Overrides.


    Public Overloads ReadOnly Property DeepCopy As TripleR
        Get
            Return New TripleR(x, y, z, Found)
        End Get
    End Property

End Class
