﻿Option Compare Text
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
Imports System.Drawing.Imaging



#Disable Warning IDE0079
#Disable Warning IDE1006


Namespace Spielfeld

    Public Module RenderingTaktgeberModul

        Public FrameScheduler As New RenderingTaktgeberClass()

        Private _currentControl As Control = Nothing
        Private _currentControlSic As Control = Nothing
        '        das "originale" visibleUserControl ist das hier: SpielfeldDaten.VisibleUserControl
        Private _visibleUserControlSic As frmMain.VisibleUserControl
        Private _visibleUserControlOnBeginnPause As frmMain.VisibleUserControl
        Private _bmpFrozen As Bitmap = Nothing

        Private _lastRendering As Rendering

        Private _lastRectOutput As New Rectangle

        Private _initialisierungLäuft As Boolean
        Private _updateSpielfeldIsDone As Boolean

        Private _createScreenShot As Boolean

        Private _BeginnPause As Boolean
        Private _ContinuePause As Boolean
        Private _EndePause As Boolean
        Private _forceUpdate As Boolean
        Private _startIniUpdate As Boolean
        Private _takteAussetzen As Integer
        Private _readNewIni As Boolean
        Private _raiseIniEvents As IniEvents

        Public Sub PaintSpielfeld_CreateScreenShot()
            _createScreenShot = True
        End Sub

        '
        ''' <summary>
        ''' Nach dem Initialisieren läuft der Taktgeber
        ''' müssen mindestens eines von:
        ''' SpielfeldDaten.PlayerSpielfeldInfo, SpielfeldDaten.EditorSpielfeldInfo
        ''' oder SpielfeldDaten.WerkbankSpielfeldInfo zugewiesen werden
        '''  As
        ''' werkbankSpielfeldInfo 
        ''' </summary>
        ''' <param name="ctrl"></param>
        ''' <param name="visibleUCtl"></param>
        Public Sub PaintSpielfeld_Initialisierung(ctrl As Control, visibleUCtl As frmMain.VisibleUserControl)
            _currentControl = ctrl
            _currentControlSic = ctrl
            SpielfeldDaten.VisibleUserControl = visibleUCtl
            _visibleUserControlSic = visibleUCtl
            FrameScheduler.Reset()
            RenderTimer.Start()
            SpielfeldDaten.AktRendering = Rendering.None
            _lastRendering = Rendering.None
            _initialisierungLäuft = True
        End Sub
        Public Sub PaintSpielfeld_DeInitialisierung()
            _currentControl = Nothing
            SpielfeldDaten.VisibleUserControl = frmMain.VisibleUserControl.None
            RenderTimer.Stop()
            SpielfeldDaten.AktRendering = Rendering.None
            _lastRendering = Rendering.None
            _initialisierungLäuft = False
        End Sub
        '
        ''' <summary>
        ''' Um nach einer PaintSpielfeld_DeInitialisierung 
        ''' (Anhalten des Polling der Änderung von SpielfeldDaten.AktRendering)
        ''' wieder zu starten.
        ''' </summary>
        Public Sub PaintSpielfeld_ReInitialisierung()
            _currentControl = _currentControlSic
            SpielfeldDaten.VisibleUserControl = _visibleUserControlSic
            FrameScheduler.Reset()
            RenderTimer.Start()
            SpielfeldDaten.AktRendering = Rendering.None
            _lastRendering = Rendering.None
            _initialisierungLäuft = True
        End Sub

        ''' <summary>
        ''' Hält die Renderung an um z.B. die INI-Datei im laufendem Betrieb zu editieren.
        ''' </summary>
        Public Sub PaintSpielfeld_BeginPause()
            _ContinuePause = True
            _BeginnPause = True
            _EndePause = False
            _visibleUserControlOnBeginnPause = VisibleUserControl
            If Not IsNothing(_bmpFrozen) Then
                _bmpFrozen.Dispose()
            End If
        End Sub
        Public Sub PaintSpielfeld_EndPause(Optional startIniUpdate As Boolean = False, Optional raiseIniEvents As IniEvents = IniEvents.None)
            _ContinuePause = False
            _EndePause = True
            _readNewIni = raiseIniEvents = IniEvents.OnUpdate
            _visibleUserControlOnBeginnPause = frmMain.VisibleUserControl.None
            _startIniUpdate = startIniUpdate
            _raiseIniEvents = raiseIniEvents
            If Not IsNothing(_bmpFrozen) Then
                _bmpFrozen.Dispose()
            End If
        End Sub

        ' „Wecker“ (10–15ms ist gut),  = 1 ergibt einen etwas stabileren Takt von 15 ms,
        ' hat aber Auswirkung auf alle Timer und kostet mehr Energie.
        ' Werte über 30 verlangsamen die Geschwindigkeit, was zu einer Vergröberung
        ' der Animation führt. (Die Gesamtlänge der Animation bleibt, die Schritte
        ' werde größer, bis es irgendwann kippt und die Anumationsdauer sich verlängert.) 
        Private WithEvents RenderTimer As New Timer With {
            .Interval = INI.Rendering_RenderTimerInterval}

        Private Sub RenderTimer_Tick(sender As Object, e As EventArgs) Handles RenderTimer.Tick

            If _currentControl Is Nothing OrElse _currentControl.IsDisposed Then
                Exit Sub
            End If
            If Not _currentControl.Visible OrElse
                _currentControl.Width = 0 OrElse
                _currentControl.Height = 0 Then
                Exit Sub
            End If

            ' Der eigentliche Takt kommt hier von der Stopwatch im Scheduler:
            If FrameScheduler.TryNextFrame() Then
                _currentControl.Invalidate()
#If DEBUGFRAME Then
                Debug.Print("TryNextFrame = True")
            Else
                Debug.Print("TryNextFrame = False")
#End If
            End If
        End Sub


        ''' <summary>
        ''' Dieses Sub wird vom PaintEvent der Zeichenfläche (UCtlSpielfeld und UCtlEdtor) getaktet
        ''' aufgerufen. Es ist ein Verteiler, der entweder das Update des Spielfeldes oder das Zeichnen selber aufruft.
        ''' </summary>
        ''' <param name="visibleUserControl"></param>
        ''' <param name="e"></param>
        ''' <param name="rectOutput"></param>
        ''' <param name="timeDifferenzFaktor"></param>
        Public Sub PaintSpielfeld_Paint(visibleUserControl As frmMain.VisibleUserControl, e As PaintEventArgs, rectOutput As Rectangle, timeDifferenzFaktor As Double)

            If _takteAussetzen > 0 Then
                _takteAussetzen -= 1
                Exit Sub
                'Nötig, wenn die INI im laufendem Betriebgeändert wird, (was nur wärend der Programmentwicklung
                'möglich ist und auch dann nur, wenn das Programm in der IDE läuft), weil die Ini dann diverse
                'Events auslöst, die erst abgearbeitet werden müssen. Um Seiteneffekte zu vermeiden, muß das
                'Rendern ausgesetzt werden. Das führt natürlich zum kurzer schwarzer Anzeige, aber das ist
                'egal, das sieht ja nur der Programmierer und der weis spätestens jetzt, warum das so ist.
                'Die Taktzahl ist bewußt viel zu hoch angesetzt (etwa eine Sekunde), damit erübrigt sich es,
                'darüber nachzudenken, ob es notwendig wird, die Taktzahl später nochmal zu erhöhen. 
            End If


            If _ContinuePause OrElse _createScreenShot Then
                If _BeginnPause OrElse _createScreenShot Then
                    _bmpFrozen = New Bitmap(rectOutput.Width, rectOutput.Height, PixelFormat.Format32bppArgb)
                    Dim gfx As Graphics = Graphics.FromImage(_bmpFrozen)
                    DoPaintSpielfeld_Paint(gfx, rectOutput, timeDifferenzFaktor, clear:=False)
                    gfx.Dispose()
                    _BeginnPause = False
                    If _createScreenShot Then
                        _createScreenShot = False
                        Try
                            _bmpFrozen.Save(AppDataFullPath(AppDataSubDir.Diverses,
                                                            AppDataSubSubDir.Diverses_ScreenShots,
                                                            AppDataFileName.ScreeenShot_png,
                                                            AppDataTimeStamp.Add,
                                                            maxFiles:=INI.Sonstiges_ScreenshotMaxCount), ImageFormat.Png)
                            If Not _ContinuePause Then
                                _bmpFrozen.Dispose()
                            End If
                        Catch ex As Exception
                            'dann gibt es keinen Screenshot
                        End Try
                    End If
                Else
                    If _visibleUserControlOnBeginnPause = visibleUserControl Then
                        If Not IsNothing(_bmpFrozen) Then
                            e.Graphics.DrawImage(_bmpFrozen, rectOutput.Location)
                        End If
                    End If
                End If
                Exit Sub
            End If

            If _EndePause Then
                'Hier wird das Update der INI durchgeführt.
                _EndePause = False
                If _startIniUpdate Then
                    _startIniUpdate = False
                    Application.DoEvents()
                    INI.UpDateIni(_raiseIniEvents, _readNewIni)
                    _raiseIniEvents = IniEvents.None
                    _readNewIni = False
                    _forceUpdate = True
                    Application.DoEvents()
                    _takteAussetzen = 30
                    Exit Sub
                End If
            End If

            'Sicherheitsgurt
            If Not (visibleUserControl = frmMain.VisibleUserControl.Spielfeld Or
                    visibleUserControl = frmMain.VisibleUserControl.Editor Or
                visibleUserControl = frmMain.VisibleUserControl.Werkbank) Then
                Exit Sub
            End If

            If _initialisierungLäuft And AktRendering <> Rendering.None Then
                'Es muss sichergestellt sein, daß _initialisierungLäuft korrekt rückgestellt wird
                'deshalb wird hier die Prüfung vorab durchgeführt, die in UpdateSpielfeld
                'auch durchgeführt wird, aber ohne Möglichkeit im Fehlerfall das Flag stehen zu lassen.
                Select Case AktRendering
                    Case Rendering.Spielfeld
                        If Not IsNothing(SpielfeldDaten.PlayerSpielfeldInfo) Then
                            If Not IsNothing(SpielfeldDaten.PlayerSpielfeldInfo.SteinInfos) Then
                                _initialisierungLäuft = False
                                _updateSpielfeldIsDone = True
                                UpdateSpielfeld(rectOutput)
                                DoPaintSpielfeld_Paint(e.Graphics, rectOutput, timeDifferenzFaktor, clear:=False)
                                Exit Sub

                            End If
                        End If

                    Case Rendering.Werkbank
                        If Not IsNothing(SpielfeldDaten.WerkbankSpielfeldInfo) Then
                            If Not IsNothing(SpielfeldDaten.WerkbankSpielfeldInfo.SteinInfos) Then
                                _initialisierungLäuft = False
                                _updateSpielfeldIsDone = True
                                UpdateSpielfeld(rectOutput)
                                DoPaintSpielfeld_Paint(e.Graphics, rectOutput, timeDifferenzFaktor, clear:=False)
                                Exit Sub

                            End If
                        End If

                    Case Rendering.Editor
                        If Not IsNothing(SpielfeldDaten.EditorSpielfeldInfo) Then
                            If Not IsNothing(SpielfeldDaten.EditorSpielfeldInfo.SteinInfos) Then
                                _initialisierungLäuft = False
                                _updateSpielfeldIsDone = True
                                UpdateSpielfeld(rectOutput)
                                DoPaintSpielfeld_Paint(e.Graphics, rectOutput, timeDifferenzFaktor, clear:=False)
                                Exit Sub

                            End If
                        End If
                End Select
            End If

            If Not _updateSpielfeldIsDone Then
                UpdateSpielfeld(rectOutput, _forceUpdate)
                DoPaintSpielfeld_Paint(e.Graphics, rectOutput, timeDifferenzFaktor, clear:=False)
            End If


            _forceUpdate = False
            _updateSpielfeldIsDone = False
        End Sub

    End Module

End Namespace
