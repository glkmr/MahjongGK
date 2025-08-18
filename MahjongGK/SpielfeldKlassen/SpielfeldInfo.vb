Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports MahjongGK.Spielfeld


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


''' <summary>
''' Xml-Container für die Klasse Steininfo, die die Eigenschaften der einzelnen Steine kapselt.  
''' </summary>
Public Class SpielfeldInfo

    Sub New()

    End Sub

    ''' <summary>
    ''' spielSize ist die Anzahl der Steine, die maximal nebeneinander und untereinander
    ''' auf dem Feld Platz haben. Z ist die Anzahl der Schichten übereinander.
    ''' </summary>
    ''' <param name="spielSize"></param>
    Sub New(spielSize As Triple)

        If spielSize.x < 3 OrElse spielSize.y < 3 OrElse spielSize.z < 1 Then
            Throw New Exception("Spielgröße ist die Größe des Spielfeld in Steinen, " &
                                "mindestens drei Steine breit, drei Steine tief und eine Schicht hoch." &
                                spielSize.ToString)
        End If

        With spielSize
            xMin = 1
            yMin = 1
            zMin = 0
            xMax = .x * 2
            yMax = .y * 2
            zMax = .z - 1

            xUBnd = xMax + 1
            yUBnd = yMax + 1
            zUBnd = zMax
        End With

        ReDim arrFB(xUBnd, yUBnd, zUBnd)
        SteinInfos = New List(Of SteinInfo)
    End Sub

    Public XmlInfo_SteinInfo_Count As Integer

    Public Name As String

    Public SteinInfos As List(Of SteinInfo)

    Public arrFB(,,) As Integer


    Public xMin As Integer = 1
    Public yMin As Integer = 1
    Public zMin As Integer = 0
    Public xMax As Integer
    Public yMax As Integer
    Public zMax As Integer

    Public xUBnd As Integer
    Public yUBnd As Integer
    Public zUBnd As Integer

    ''' <summary>
    ''' Vergleicht diese Instanz mit einer anderen SpielfeldInfo.
    ''' Es wird nur geprüft, ob beide SpielfeldInfo dieselbe Instanz sind.
    ''' spielfeldinfo_Other darf Nothing sein, dann sind sie unterschiedlich.
    ''' Vorsicht Falle: CompareSpielfeldInfo darf natürlich nicht aufgerufen
    ''' werden, wenn die eigene Instanz Nothing ist.
    ''' Das geht mit der anderen Überladung.
    ''' </summary>
    Public Function CompareSpielfeldInfo(spielfeldinfo_Other As SpielfeldInfo) As Boolean
        Return Me Is spielfeldinfo_Other
    End Function
    '
    ''' <summary>
    ''' Vergleicht zwei SpielfeldInfo-Instanzen.
    ''' Prüft:
    '''   - ob beide Nothing sind (--> Exception("Beide SpielfeldInfo sind Nothing (Initialisierungsfehler).")
    '''   - ob sie auf dieselbe Instanz zeigen
    ''' </summary>
    ''' <param name="spielfeldinfoA">Erste SpielfeldInfo</param>
    ''' <param name="spielfeldinfoB">Zweite SpielfeldInfo</param>
    ''' <returns>True, wenn beide dieselbe Instanz sind, False sonst.</returns>
    Public Shared Function CompareSpielfeldInfo(spielfeldinfoA As SpielfeldInfo, spielfeldinfoB As SpielfeldInfo) As Boolean
        If spielfeldinfoA Is Nothing AndAlso spielfeldinfoB Is Nothing Then
            Throw New Exception("Beide SpielfeldInfo sind Nothing (Initialisierungsfehler).")
        End If

        Return spielfeldinfoA Is spielfeldinfoB
    End Function
    '


#Region "Manipulationen des Spielfeldes"

    ''' <summary>
    ''' Setzt die X-Y-OffsetWerte der childFB des Feldbeschreibers und den Index des Feldbeschreibers selber. 
    ''' </summary>
    ''' <param name="infoFBx"></param>
    ''' <param name="infoFBY"></param>
    ''' <param name="infoFBz"></param>
    Public Sub CopySteinIndexToSpielfeldPos3DAndSetOffsetXY(infoFBx As Integer, infoFBY As Integer, infoFBz As Integer, SteinInfoIndex As Integer)
        '
        'Vom FB links über dem infoFB geht es zum infoFB, indem zu beiden Koordinaten
        '1 addiert wird. Daher:
        SetOffsetX(arrFB(infoFBx - 1, infoFBY - 1, infoFBz), True)
        SetOffsetY(arrFB(infoFBx - 1, infoFBY - 1, infoFBz), True)
        '
        'Der FB genau über dem infoFB zum infoFB geht mit OffsetX = 0 und OffsetY = 1
        SetOffsetX(arrFB(infoFBx, infoFBY - 1, infoFBz), False)
        SetOffsetY(arrFB(infoFBx, infoFBY - 1, infoFBz), True)
        '
        'Jetzt die mittlere Zeile
        'Vom FB links vom infoFB zum infoFB: OffsetX = +1, OffsetY = 0
        SetOffsetX(arrFB(infoFBx - 1, infoFBY, infoFBz), True)
        SetOffsetY(arrFB(infoFBx - 1, infoFBY, infoFBz), False)

        'Der infoFB hat keinen Offset, daher beide 0 (Verweis auf sich selber)
        SetOffsetX(arrFB(infoFBx, infoFBY, infoFBz), False)
        SetOffsetY(arrFB(infoFBx, infoFBY, infoFBz), False)
        '
        SetIndexStein(arrFB(infoFBx, infoFBY, infoFBz), SteinInfoIndex)

    End Sub

    Public Sub CopySteinIndexToSpielfeldPos3DAndSetOffsetXY(infoFBTriple As Triple, steinInfoIndex As Integer)
        CopySteinIndexToSpielfeldPos3DAndSetOffsetXY(infoFBTriple.x, infoFBTriple.y, infoFBTriple.z, steinInfoIndex)
    End Sub

    ''' <summary>
    ''' Setzt einen Stein auf das Spielfeld und Added einen Stein mit Basisinformationen
    ''' zu den SteinInfos. Wenn OK, gibt die Funktion True zurück.
    ''' Sind die Koordinaten des infoFBTriple ungültig, wird der Stein als Stein.Dummy
    ''' (Fehler-Grafik) auf die erste gefundene Position als frei schwebender Stein in
    ''' der obersten Ebene geparkt. Ungültig heißt, es würde ein OutOfRange-Error entstehen.
    ''' Das ist ein Sicherheitsgurt während der Programmentwicklung.
    ''' Die Funktion gibt dann False zurück.
    ''' </summary>
    ''' <param name="steinPos3D"></param>
    ''' <returns></returns>
    Public Function AddSteinToSpielfeld(steinIndex As SteinIndexEnum, steinPos3D As Triple) As Boolean

        'Der steinInfoIndex wird hier gesichert, obwohl er gleichlautend ist mit dem
        'Index in SteinInfos. Grund: werden später Steine im Editor entfernt, verschieben sich die
        'Indexnummern in SteinInfos und da muss arrFB aktualisert werden. Dazu braucht man den
        '"alten" steinInfoIndex, eben diesen steinInfoIndex.
        Dim newSteinInfo As New SteinInfo(steinInfoIndex:=SteinInfos.Count, steinIndex, steinPos3D)

        If Not steinPos3D.IsInsideSpielfeldBounds(arrFB) Then
            'Falsche Positionsangabe.
            'Kein Throw Nex Exception, sondern Anzeige auf dem Spielfeld
            'Fehlergrafik in die oberste Ebene als frei schwebender Stein.
            'Im fertig entwickeltem Programm sollte dieser Teil nicht mehr
            'aufgerufen werden :-)
            '
            'Linke obere Ecke der obersten Ebene
            Dim tpl As New Triple(1, 1, arrFB.GetUpperBound(2))

            Do
                Dim tplR As TripleR = SearchPlace(tpl, direction:=Direction.Right)
                Select Case tplR.Found
                    Case TripleR.Result.NoFundamentFound   'Zeilenende erreicht.
                        tpl.y += 2 'Eine Steinreihe tiefer weitersuchen
                        tpl.x = 1
                        If tpl.y > arrFB.GetUpperBound(1) - 1 Then
                            'Die ganze oberste Ebene ist vollgepflastert mit Steinen.
                            'Da das ziemlich sicher nicht absichlich geschehen ist,
                            'unterstelle ich, das da viele Fehlergrafiken dabei sind
                            'und breche ab ohne weitere Prüfung und setze den
                            'Stein auf die Position links oben etwas versetzt,
                            'also optisch auf eine Ebene, die es garnicht gibt.
                            tpl.x = 2
                            tpl.y = 2

                            'SetStein rekursiv, aber mit jetzt gültigen Werten aufrufen
                            AddSteinToSpielfeld(SteinIndexEnum.Dummy, tpl)
                            Return False
                        End If

                    Case TripleR.Result.FoundFreePlace, TripleR.Result.NoFundamentFound
                        ' FoundResult.NoFundament ist in diesem Fall OK, er wird zum freischwebendem Stein.
                        'SetStein rekursiv, aber mit jetzt gültigen Werten aufrufen
                        AddSteinToSpielfeld(SteinIndexEnum.Dummy, tplR.ToTriple)
                        Return False
                End Select
            Loop
        End If
        '
        'Hier ist jetzt die Normal-Routine
        '
        'SteinInfos.Count ist der Index, den der Stein in SteinInfos haben wird.
        CopySteinIndexToSpielfeldPos3DAndSetOffsetXY(steinPos3D, steinInfoIndex:=SteinInfos.Count)
        '() nicht trennen, der Index muss stimmen. Späterer Zugriff über
        'Dim aktSteininfo As SteinInfo = SteinInfos(indexSteinInfo)
        SteinInfos.Add(newSteinInfo)

        Return True

    End Function

    ''' <summary>
    ''' Entfernt einen Stein vollständig, d.h. einschließlich aller Daten vom Spielfeld
    ''' und aus steininfos
    ''' Hinweis: Um ihn nicht mehr anzuzeigen muß er nicht entfernt werden. Dazu wird
    ''' lediglich sein SteinStatus geändert.
    ''' </summary>
    ''' <param name="arrFB"></param>
    ''' <param name="steininfos"></param>
    ''' <param name="steininfo"></param>
    Public Sub RemoveSteinFromSpielfeld(arrFB(,,) As Integer, steininfos As SpielfeldInfo, steininfo As SteinInfo)

        'TODO
        ''prüfen, ob der Stein nicht bereits entfernt wurde.
        'Dim fRemove As Boolean = True
        'If Not steininfos.SteinInfo.Contains(steininfo) Then
        '    If Debugger.IsAttached  And Not IfRunningInIDE_ShowErrorMsgInsteadOfException Then
        '        Throw New Exception("Programmierfehler. Zu entfernende SteinInfo in SteinInfos nicht gefunden. SpielfeldHelper.RemoveSteinFromSpielfeld")
        '    End If
        '    fRemove = False
        'End If
        ''
        ''Prüfen, ob er auf dem Spielfeld noch vorhanden ist
        'Dim index As Integer = GetIndexStein(arrFB, steininfo.Postion3D)

        'If index < 0 Then
        '    If Debugger.IsAttached  And Not IfRunningInIDE_ShowErrorMsgInsteadOfException Then
        '        Throw New Exception($"Programmierfehler. Der Platz im arrFB {New Triple(steininfo.X, steininfo.Y, steininfo.Z).ToString} ist leer. SpielfeldHelper.RemoveSteinFromSpielfeld")
        '    End If
        '    fRemove = False
        'End If

        'If index <> steininfo.Index Then
        '    If Debugger.IsAttached  And Not IfRunningInIDE_ShowErrorMsgInsteadOfException Then
        '        Throw New Exception($"Programmierfehler. Index im FB <> Index im steininfo ({index} <> {steininfo.Index}). SpielfeldHelper.RemoveSteinFromSpielfeld")
        '    End If
        '    fRemove = False
        'End If
        ''



        ''Der Knackpunkt ist, das sich die Indexnummern ändern, wenn ein Stein entnommene wird.
        ''diese müssen daher angepasst werden.

        'Dim idxRemove As Integer = steininfo.Index

        ''Den FB links über dem infoFB
        'arrFB(infoFBx - 1, infoFBY - 1, infoFBz) = 0
        ''
        ''Den FB genau über dem infoFB
        'arrFB(infoFBx, infoFBY - 1, infoFBz) = 0
        ''
        ''Jetzt die mittlere Zeile
        ''Den FB links vom infoFB 
        'arrFB(infoFBx - 1, infoFBY, infoFBz) = 0

        ''Der infoFB 
        'arrFB(infoFBx, infoFBY, infoFBz) = 0
        ''
    End Sub

    Public Sub RemoveLastSteinFromSpielfeld(arrFB(,,) As Long, steininfos As SpielfeldInfo)

    End Sub

#End Region

#Region "Fragen an das Spielfeld"

    ''' <summary>
    ''' Prüft, ob der Platz frei ist
    ''' </summary>
    ''' <param name="infoFBTriple"></param>
    ''' <returns></returns>
    Public Function IsFreePlace(infoFBTriple As Triple) As Boolean
        Return IsFreePlace(infoFBTriple.x, infoFBTriple.y, infoFBTriple.z)
    End Function

    Public Function IsFreePlace(infoFBX As Integer, infoFBY As Integer, infoFBZ As Integer) As Boolean

        '
        'die obere Zeile
        'Den FB links über dem infoFB
        If arrFB(infoFBX - 1, infoFBY - 1, infoFBZ) <> 0 Then Return False
        '
        'Den FB genau über dem infoFB
        If arrFB(infoFBX, infoFBY - 1, infoFBZ) <> 0 Then Return False

        'Jetzt die untere Zeile
        'Den FB links vom infoFB 
        If arrFB(infoFBX - 1, infoFBY, infoFBZ) <> 0 Then Return False

        'Der infoFB 
        If arrFB(infoFBX, infoFBY, infoFBZ) <> 0 Then Return False
        '
        Return True

    End Function

    ''' <summary>
    ''' Prüft, ob der Stein auf der Grundläche oder vollständig 
    ''' auf anderen Steinen stehen würde.
    ''' </summary>
    ''' <param name="infoFBTriple"></param>
    ''' <returns></returns>
    Public Function HasFundament(infoFBTriple As Triple) As Boolean
        Return HasFundament(infoFBTriple.x, infoFBTriple.y, infoFBTriple.z)
    End Function

    Public Function HasFundament(infoFBx As Integer, infoFBY As Integer, infoFBz As Integer) As Boolean

        'Im Unterschied zu IsFreePlace wird hier in der Ebene unter dem Stein gesucht,
        'nicht in der Steinebene.

        If infoFBz = 0 Then
            'Das ist die Grundfläche des Spieles.
            Return True
        Else
            'Alle Plätze des infoFB in der Ebene drunter müssen belegt sein.
            '(infoFBx, infoFBY und infoFBz sind die Koordinaten des abgefragten infoFB)
            '
            'genau eine Ebene tiefer gehen.
            infoFBz -= 1
            '
            'die obere Zeile
            'Den FB links über dem infoFB
            If arrFB(infoFBx - 1, infoFBY - 1, infoFBz) = 0 Then Return False
            '
            'Den FB genau über dem infoFB
            If arrFB(infoFBx, infoFBY - 1, infoFBz) = 0 Then Return False
            '
            'Jetzt die untere Zeile
            'Den FB links vom infoFB 
            If arrFB(infoFBx - 1, infoFBY, infoFBz) = 0 Then Return False

            'Der infoFB 
            If arrFB(infoFBx, infoFBY, infoFBz) = 0 Then Return False

            Return True

        End If

    End Function

    Public Function IncDirection(tripleR As TripleR, direction As Direction, Optional [Step] As Integer = 1) As TripleR
        Return New TripleR(IncDirection(New Triple(tripleR.x, tripleR.y, tripleR.z), direction, [Step]), fr:=TripleR.Result.NotSet)
    End Function
    ''' <summary>
    ''' Incrementiert die X- und Y-Koordinaten des Triple in die vorgegebene (Himmels-) Richtung.
    ''' </summary>
    ''' <param name="triple"></param>
    ''' <param name="direction"></param>
    ''' <param name="[Step]"></param>
    ''' <returns></returns>
    Public Function IncDirection(triple As Triple, direction As Direction, Optional [Step] As Integer = 1) As Triple

        'DeepCopy erstellen um sicherzustellen, dass der Ausgangswert unverändert bleibt.
        Dim newTriple As Triple = triple.DeepCopy

        With newTriple
            Select Case direction
                Case Direction.Up
                    .y -= [Step]

                Case Direction.UpRight
                    .y -= [Step]
                    .x += [Step]

                Case Direction.Right
                    .x += [Step]

                Case Direction.DownRight
                    .y += [Step]
                    .x += [Step]

                Case Direction.Down
                    .y += [Step]

                Case Direction.DownLeft
                    .y += [Step]
                    .x -= [Step]

                Case Direction.Left
                    .x -= [Step]

                Case Direction.UpLeft
                    .y -= [Step]
                    .x -= [Step]

            End Select
        End With

        Return newTriple

    End Function

    ''' <summary>
    ''' Sucht von der Startposition des centralFD (Koordinaten in infoFBTriple)
    ''' in die angegebene (Himmels-) Richtung nach dem nächsten freiem Platz.
    ''' Wenn z = 0 (also auf der Grundfläche) wird jeder freie Platz zurückgegeben.
    ''' Wenn z > 0 nur Positionen, wo der Stein vollständig auf anderen Steinen
    ''' steht. 
    ''' Ob was gefunden wurde und wenn nicht, warum, steht in TripleR.fr As SearchResult
    ''' </summary>
    ''' <param name="infoFBTriple"></param>
    ''' <param name="direction"></param>
    ''' <returns></returns>
    Public Function SearchPlace(infoFBTriple As TripleR, direction As Direction) As TripleR
        Return SearchPlace(New Triple(infoFBTriple.x, infoFBTriple.y, infoFBTriple.z), direction)
    End Function

    ''' <summary>
    ''' Sucht von der Startposition (Koordinaten in infoFBTriple)
    ''' in die angegebene  Richtung nach dem nächsten freiem Platz.
    ''' Wenn z = 0 (also auf der Grundfläche) wird jeder freie Platz zurückgegeben.
    ''' Wenn z > 0 nur Positionen, wo der Stein vollständig auf anderen Steinen
    ''' steht. 
    ''' Ob was gefunden wurde und wenn nicht, warum, steht in TripleR.fr As SearchResult
    ''' </summary>
    ''' <param name="infoFBTriple"></param>
    ''' <param name="direction"></param>
    ''' <returns></returns>
    Public Function SearchPlace(infoFBTriple As Triple, direction As Direction) As TripleR

        'DeepCopy um Seiteneffekte zu verhindern
        Dim tpl As Triple = infoFBTriple.DeepCopy

        'nicht mögliche Startpositionen anpassen
        If tpl.x <= 1 Then tpl.x = 2
        If tpl.y <= 1 Then tpl.y = 2

        If tpl.z = 0 Then
            Do
                If Not tpl.IsInsideSpielfeldBounds(arrFB) Then
                    Return New TripleR(tpl, TripleR.Result.OutsideBorder)
                Else
                    If IsFreePlace(tpl) Then
                        Return New TripleR(tpl, TripleR.Result.FoundFreePlace)
                    End If
                End If
                tpl = IncDirection(tpl, direction)
            Loop
        Else
            Dim fFoundFreePlace As Boolean = False
            Do

                If Not tpl.IsInsideSpielfeldBounds(arrFB) Then
                    If fFoundFreePlace Then
                        Return New TripleR(tpl, TripleR.Result.NoFundamentFound)
                    Else
                        Return New TripleR(tpl, TripleR.Result.OutsideBorder)
                    End If
                Else
                    Dim fFound As Boolean = IsFreePlace(tpl)

                    If fFound And Not fFoundFreePlace Then
                        fFoundFreePlace = True
                    End If

                    If fFound AndAlso HasFundament(tpl) Then
                        Return New TripleR(tpl, TripleR.Result.FoundFreePlace)
                    End If

                End If

                tpl = IncDirection(tpl, direction)
            Loop
        End If

    End Function
    '
    ''' <summary>
    ''' Zunächst wie die erste Überladung von SearchPlace, als die Suche nach einem freienm Platz.
    ''' Wenn einer gefunden wurde, wird der Platz verschoben nach Angabe von moveX und moveX
    ''' unter Berücksichtigung von 
    ''' </summary>
    ''' <param name="infoFBTriple"></param>
    ''' <param name="direction"></param>
    ''' <param name="moveX"></param>
    ''' <param name="moveY"></param>
    ''' <param name="stepHalberStein"></param>
    ''' <returns></returns>
    Public Function SearchPlace(infoFBTriple As Triple, direction As Direction, moveX As Move, moveY As Move, stepHalberStein As Boolean) As TripleR

        Dim tr As TripleR = SearchPlace(infoFBTriple, direction)

        If tr.Found = TripleR.Result.FoundFreePlace Then

        End If

    End Function

    Public Function SearchPlace(infoFBx As Integer, infoFBY As Integer, infoFBz As Integer, direction As Direction) As TripleR
        Return SearchPlace(New Triple(infoFBx, infoFBY, infoFBz), direction)
    End Function


    ''' <summary>
    ''' Gibt die die Koordinaten der Spielfeldmitte in einem Triple zurück.
    ''' Brauchbar um den den ersten Stein zu verlegen oder als Startposition
    ''' zur Suche nach einem freiem Platz.
    ''' Wird Ebene zu hoch angegeben, wird die oberste Ebene eingestellt.
    ''' </summary>
    ''' <param name="ebene"></param>
    ''' <returns></returns>
    Public Function GetSpielfeldCenter(ebene As Integer) As Triple

        If ebene > arrFB.GetUpperBound(2) Then
            ebene = arrFB.GetUpperBound(2)
        ElseIf ebene < 0 Then
            ebene = 0
        End If

        Return New Triple(xMax \ 2 + 1, yMax \ 2 + 1, ebene)

    End Function

#End Region

#Region "Manipulation des Feldbeschreibers FB"

    ' Kopiervorlagen der Funktionen und Methoden dieser Region als InlineCode
    '
    ' Lesen:
    ' Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
    ' Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
    ' Dim toggleFlag As Boolean = (fb And FLAG_ToggleFlag) <> 0
    ' Dim index As Integer = fb \ 1000
    '
    ' Schreiben:
    ' If value0or1 <> 0 Then fb = fb Or FLAG_XOffset Else fb = fb And Not FLAG_XOffset
    ' If value0or1 <> 0 Then fb = fb Or FLAG_YOffset Else fb = fb And Not FLAG_YOffset
    '
    ' If toggleValue Then
    '    fb = fb Or FLAG_ToggleFlag
    ' Else
    '    fb = fb And Not FLAG_ToggleFlag
    ' End If
    '
    ' Dim flags As Integer = fb And &HFF
    ' fb = newValue * 1000 + flags
    '
    ' fb = fb Xor FLAG_ToggleFlag ' Toggle
    '
    '
    ''' <summary>
    ''' Diese Überladung holt des Index direkt aus dem arrFB.
    ''' Wenn auf der Position kein Stein setht, wird -1 zurückgegeben
    ''' Jeder Stein belegt 4 Felder in arrFB. Die 4 Felder müssen alle einen Wert ungleich 0 haben,
    ''' damit sie als "belegt" erkannt werden können. Außerdem muss eines dieser Felder den Index
    ''' transportieren, der auf die SteinInformationen in SteinInfos zeigt.
    ''' Der Index steht nur im Feld rechts unten der 4 Felder. Zudem ist er um 1 erhöht, damit
    ''' der Index immer ungleich 0 ist, und mit 100 multipliziert, damit die beiden niederwertigsten
    ''' Ziffern frei sind. Die verwende ich als Flagfeld und betrachte den Bereich bitweise.
    ''' In den drei anderen Feldern sind dann zwei Flags gesetzt, die besagen, ob der Felbeschreiber
    ''' rechts, drunter oder rechtsdrunter steht, daraus werden die Offsets gebildet,
    ''' und erneut auf den Array zugegriffen. Eine Unterscheidung, ob beide Offsetz = 0 sind, ist nicht
    ''' nötig, Koordinate + 0 bleibt Koordinate. Weil im Feld rechts-unten die Offsetzs 0 sind,
    ''' muß der Index immer größer null sein, damit die "belegt" Bedingung erfüllt ist. 
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <param name="z"></param>
    ''' <returns></returns>
    Public Function GetIndexStein(x As Integer, y As Integer, z As Integer) As Integer
        Dim fb As Integer = arrFB(x, y, z)
        Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
        Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
        Return (arrFB(x + offsetX, y + offsetY, z) \ 1000) - 1
        'Auf ein unbelegtes Feld angewendet passiert folgendes:
        'offsetX und Offset Y sind 0, d.h. x und y werden nicht verändert. 
        'Der Index  (arrFB(x + offsetX, y + offsetY, z) \ 1000) ist auch 0
        '1 ab, gibt minus 1. ==> es muss nicht extra auf arrFB(x, y, z) = 0 geprüft werden,
        'ob ein Feld leer ist, es kann immer gleich nach dem SteinIndex gefragt werden.
    End Function
    '
    ''' <summary>
    ''' Diese Überladung holt des Index direkt aus dem arrFB unter Berücksichtigung der OffsetXY
    ''' Siehe die erste Überladung.
    ''' </summary>
    ''' <param name="tripl"></param>
    ''' <returns></returns>
    Public Function GetIndexStein(tripl As Triple) As Integer
        'Weil zeitkritisch doppelter Code
        With tripl
            Dim fb As Integer = arrFB(.x, .y, .z)
            Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
            Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
            Return (arrFB(.x + offsetX, .y + offsetY, .z) \ -1)
        End With
    End Function


    Public Shared Function GetIndexStein(fb As Integer) As Integer
        'Der Index wird um 1 erhöht gespeichert, weil index = 0 gleichbedeutend
        'wäre mit "freier Platz"
        Return (fb \ 1000) - 1  ' Wert ab dritter Dezimalstelle (ab Hunderterstelle)
    End Function


    Public Shared Function GetOffsetX(fb As Integer) As Integer
        Return If((fb And FLAG_XOffset) <> 0, 1, 0)
    End Function

    Public Shared Function GetOffsetY(fb As Integer) As Integer
        Return If((fb And FLAG_YOffset) <> 0, 1, 0)
    End Function
    '
    ''' <summary>
    ''' Holt den Wert des ToggleFlags koorigiert um den OffsetXY direkt aus dem arrFB 
    ''' </summary>
    ''' <param name="x"></param>
    ''' <param name="y"></param>
    ''' <param name="z"></param>
    ''' <returns></returns>
    Public Function GetToggleFlag(x As Integer, y As Integer, z As Integer) As Boolean
        'Weil zeitkritisch doppelter Code
        Dim fb As Integer = arrFB(x, y, z)
        Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
        Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
        Return (arrFB(x + offsetX, y + offsetY, z) And FLAG_ToggleFlag) <> 0
    End Function
    '
    ''' <summary>
    ''' Holt den Wert des ToggleFlags koorigiert um den OffsetXY direkt aus dem arrFB
    ''' </summary>
    ''' <param name="tripl"></param>
    ''' <returns></returns>
    Public Function GetToggleFlag(tripl As Triple) As Boolean
        'Weil zeitkritisch doppelter Code
        With tripl
            Dim fb As Integer = arrFB(.x, .y, .z)
            Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
            Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
            Return (arrFB(.x + offsetX, .y + offsetY, .z) And FLAG_ToggleFlag) <> 0
        End With
    End Function
    '
    ''' <summary>
    ''' Seperate Gewinnung des Wertes.
    ''' </summary>
    ''' <param name="fb"></param>
    ''' <returns></returns>
    Public Function GetToggleFlag(fb As Integer) As Boolean
        Return (fb And FLAG_ToggleFlag) <> 0
    End Function
    '
    ''' <summary>
    ''' Zum Syncronisieren von ToggleFlag und VergleichsToggleFlag muss das
    ''' VergleichsToggleFlag auf den Zustand im Feld gesetz werden. (sicherheitshalber)
    ''' Dazu wird das erste Toggleflag im Feld gesucht und ausgelesen.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetFirstToggleFlagValue() As Boolean

        For z As Integer = 0 To zMax 'auf der untersten Ebene beginnen. (da wird immer was gefunden)
            For x As Integer = 0 To xMax
                For y As Integer = 0 To yMax
                    Dim fb As Integer = arrFB(x, y, z)
                    If fb <> 0 Then
                        'das erste gefundene belegte Feld auswerten
                        Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
                        Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
                        Return (arrFB(x + offsetX, y + offsetY, z) And FLAG_ToggleFlag) <> 0
                        ''Wenn der Code ausgetestet ist, kann verkürzt werden auf
                        'If fb <> 0 Then
                        '    Return (arrFB(x + 1, y + 1, z) And FLAG_ToggleFlag) <> 0
                        'End If
                        'denn es wird immer ein linker oberer Quadrant gefunden
                        'und da sind beide Offsets immer 1.
                    End If
                Next
            Next
        Next
        Return False 'leeres Feld
    End Function

    '
    'Hinweis: Die Setter-Prozeduren sind ByRef, damit fb geändert wird.
    Public Sub SetOffsetX(ByRef fb As Integer, value0or1 As Integer)
        If value0or1 <> 0 Then
            fb = fb Or FLAG_XOffset
        Else
            fb = fb And Not FLAG_XOffset
        End If
    End Sub

    Public Sub SetOffsetX(ByRef fb As Integer, value As Boolean)
        If value Then
            fb = fb Or FLAG_XOffset
        Else
            fb = fb And Not FLAG_XOffset
        End If
    End Sub

    Public Sub SetOffsetY(ByRef fb As Integer, value0or1 As Integer)
        If value0or1 <> 0 Then
            fb = fb Or FLAG_YOffset
        Else
            fb = fb And Not FLAG_YOffset
        End If
    End Sub

    Public Sub SetOffsetY(ByRef fb As Integer, value As Boolean)
        If value Then
            fb = fb Or FLAG_YOffset
        Else
            fb = fb And Not FLAG_YOffset
        End If
    End Sub

    Public Sub SetToggleFlag(x As Integer, y As Integer, z As Integer, value As Boolean)
        'Weil zeitkritisch doppelter Code
        Dim fb As Integer = arrFB(x, y, z)
        Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
        Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
        If value Then
            arrFB(x + offsetX, y + offsetY, z) = arrFB(x + offsetX, y + offsetY, z) Or FLAG_ToggleFlag
        Else
            arrFB(x + offsetX, y + offsetY, z) = arrFB(x + offsetX, y + offsetY, z) And Not FLAG_ToggleFlag
        End If
    End Sub

    Public Sub SetToggleFlag(tripl As Triple, value As Boolean)
        'Weil zeitkritisch doppelter Code
        With tripl
            Dim fb As Integer = arrFB(.x, .y, .z)
            Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
            Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
            If value Then
                arrFB(.x + offsetX, .y + offsetY, .z) = arrFB(.x + offsetX, .y + offsetY, .z) Or FLAG_ToggleFlag
            Else
                arrFB(.x + offsetX, .y + offsetY, .z) = arrFB(.x + offsetX, .y + offsetY, .z) And Not FLAG_ToggleFlag
            End If
        End With
    End Sub

    Public Shared Sub SetToggleFlag(ByRef fb As Integer, value As Boolean)
        If value Then
            fb = fb Or FLAG_ToggleFlag
        Else
            fb = fb And Not FLAG_ToggleFlag
        End If
    End Sub

    Public Shared Sub SetIndexStein(ByRef fb As Integer, value As Integer)
        ' Wert ab vierten Dezimalstelle setzen (ab Tausenderstelle)
        ' Flags in den unteren Bits behalten
        Dim flags As Integer = fb And &HFF  ' Bits 0-15, Dezimal 0 bi 255
        'Der Index wird um 1 erhöht gespeichert, weil index = 0 gleichbedeutend
        'wäre mit "freier Platz" (zumindest, wenn flags = 0, was möglich ist.)
        fb = (value + 1) * 1000 + flags
    End Sub

    Public Sub ToggleToggleFlag(x As Integer, y As Integer, z As Integer)
        'Weil zeitkritisch doppelter Code
        Dim fb As Integer = arrFB(x, y, z)
        Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
        Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
        arrFB(x + offsetX, y + offsetY, z) = arrFB(x + offsetX, y + offsetY, z) Xor FLAG_ToggleFlag
    End Sub

    Public Sub ToggleToggleFlag(tripl As Triple)
        'Weil zeitkritisch doppelter Code
        With tripl
            Dim fb As Integer = arrFB(.x, .y, .z)
            Dim offsetX As Integer = If((fb And FLAG_XOffset) <> 0, 1, 0)
            Dim offsetY As Integer = If((fb And FLAG_YOffset) <> 0, 1, 0)
            arrFB(.x + offsetX, .y + offsetY, .z) = arrFB(.x + offsetX, .y + offsetY, .z) Xor FLAG_ToggleFlag
        End With
    End Sub

    Public Shared Sub ToggleToggleFlag(ByRef fb As Integer)
        fb = fb Xor FLAG_ToggleFlag
    End Sub


#End Region


#Region "Debug-Funktionen"
    '
    ''' <summary>
    ''' Die Funktion arbeitet nur, wenn Debugger.IsAttached, also nur innerhalb der IDE.
    ''' Prüft, ob alle Randfelder auf 0 sind. Wenn nicht, ist ein Stein falsch
    ''' eingetragen worden. Programmierfehler! Die Funktion wirft dann eine Exception,
    ''' </summary>
    Public Sub CheckRand()

        If Not Debugger.IsAttached Then
            Exit Sub
        End If

        ' Prüft den Rand in X- und Y-Richtung für alle Z-Ebenen
        For z As Integer = 0 To zMax
            ' Linker Rand (x=0)
            For y As Integer = 0 To yMax + 1
                If arrFB(0, y, z) <> 0 Then
                    Throw New Exception($"Programmierfehler: Linker Rand ist nicht 0 in x=0 y={y} z={z}")
                End If
            Next

            ' Rechter Rand (x = xMax+1)
            For y As Integer = 0 To yMax + 1
                If arrFB(xMax + 1, y, z) <> 0 Then
                    Throw New Exception($"Programmierfehler: Rechter Rand ist nicht 0 in x={xMax + 1} y={y} z={z}")
                End If
            Next

            ' Oberer Rand (y = 0)
            For x As Integer = 0 To xMax + 1
                If arrFB(x, 0, z) <> 0 Then
                    Throw New Exception($"Programmierfehler: Oberer Rand ist nicht 0 in x={x} y=0 z={z}")
                End If
            Next

            ' Unterer Rand (y = yMax+1)
            For x As Integer = 0 To xMax + 1
                If arrFB(x, yMax + 1, z) <> 0 Then
                    Throw New Exception($"Programmierfehler: Unterer Rand ist nicht 0 in x={x} y={yMax + 1} z={z}")
                End If
            Next
        Next
    End Sub

#End Region


#Region "Load, Save"

    '################################################################################################

    ''' <summary>
    ''' zLwpD = myIO.GetPathInAnwendungsdatenII("Suche", GetType(IndexData).Name + ".xml")
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function Load(zLwpD As String) As SpielfeldInfo

        If IO.File.Exists(zLwpD) Then
            Try
                Using reader As New IO.StreamReader(zLwpD)
                    Dim serializer As New Xml.Serialization.XmlSerializer(GetType(SpielfeldInfo))
                    Dim daten As SpielfeldInfo = CType(serializer.Deserialize(reader), SpielfeldInfo)
                    Return daten

                End Using
            Catch
                ' Fehler beim Laden – neue Instanz zurückgeben
            End Try
        End If

        Dim neu As New SpielfeldInfo()
        Return neu

    End Function

    Public Sub Save(zLwpD As String)

        XmlInfo_SteinInfo_Count = SteinInfos.Count
        Try
            Using writer As New IO.StreamWriter(zLwpD)
                Dim serializer As New Xml.Serialization.XmlSerializer(Me.GetType())
                serializer.Serialize(writer, Me)
            End Using
        Catch ex As Exception
            'TODO Fehlerbehandlung
        End Try
    End Sub

#End Region

End Class
