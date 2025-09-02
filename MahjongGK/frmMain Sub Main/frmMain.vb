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

Public Class frmMain


#Region "Tests während der Programmenticklung"

    'Public Sub xxx() Handles Me.Load
    '    AddHandler INI.Editor_UsingEditorAllowed_Changed, AddressOf Test1
    'End Sub

    'Public Sub yyy() Handles Me.Disposed
    '    RemoveHandler INI.Editor_UsingEditorAllowed_Changed, AddressOf Test1
    'End Sub
    Private Sub Toggle()
        If Spielfeld.AktRendering = Rendering.Spielfeld Then
            Spielfeld.AktRendering = Rendering.Werkbank
        Else
            Spielfeld.AktRendering = Rendering.Spielfeld
        End If
    End Sub

    Private Sub Test1(value As Boolean)
        Dim icr As New IniCommentReader(INI.AllIniManagersFullPath)
        Dim sb As New System.Text.StringBuilder
        sb.Append(icr.GetKommentar(NameOf(INI.Editor_UsingEditor), seperator:=CommentSeperator.CrLf))
        sb.Append(icr.GetKommentar(NameOf(INI.IfRunningInIDE_Grafik16x16Directory), seperator:=CommentSeperator.Tilde))
        sb.Append(icr.GetKommentar(NameOf(INI.Rendering_BitmapHighQuality), seperator:=CommentSeperator.Tilde))
        sb.Append(icr.GetKommentar(NameOf(INI.Sonstiges_AppGrafikSatz), seperator:=CommentSeperator.Tilde))
        MessageBoxFormatiert.ShowInfo(sb.ToString, "Kommentare")
    End Sub

    Private Sub Test2()
        INI.Editor_UsingEditorAllowed = Not INI.Editor_UsingEditorAllowed
    End Sub

    Private Sub Go()
        Spielfeld.TestDaten_Spielfeld_Methodenaufruf_zum_Debuggen()
    End Sub

    Private Sub ToolStripSplitButtonTest1_ButtonClick(sender As Object, e As EventArgs)

        'Using tst As New MahjongGKSymbolFactory.TileStyleTuner
        '    tst.ShowDialog()
        'End Using




        'SpielfeldTest_SpielsteinGenerator.RunAll()

        'Dim gen1 As New SpielsteinGenerator(visibleAreaMaxLength:=30, generatorMode:=GeneratorModi.StoneStream_Base152_Continuous)
        'Dim gen2 As New SpielsteinGenerator(visibleAreaMaxLength:=30, generatorMode:=GeneratorModi.StoneSet_144)

        'Dim stat As New Statistik(gen1.Vorrat, gen2.Vorrat)
        'MessageBoxFormatiert.ShowInfoMonoSpaced(stat.ToString(deltaProz144:=True), "Spielsteinverteilung")
        'MessageBoxFormatiert.ShowInfoMonoSpaced(Spielfeld.DebugKonstantenString, "Spielsteinvariable")
    End Sub

    Private Sub Test2ToolStripMenuItem_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Test3ToolStripMenuItem_Click(sender As Object, e As EventArgs)



        'AppDataFullPath(AppDataSubDir.Diverses,
        '                                                    AppDataSubSubDir.Diverses_ScreenShots,
        '                                                    AppDataFileName.ScreeenShot_png,
        '                                                    AppDataTimeStamp.Add,
        '                                                    maxFiles:=20), ImageFormat.Png)
    End Sub

    Private Sub ToolStripSplitButtonIniEditor_ButtonClick(sender As Object, e As EventArgs)

        Spielfeld.PaintSpielfeld_BeginPause()

        Using f As New FrmIniEditor()
            f.ShowDialog()
        End Using

        Dim rs As RefreshSummary = INI.RefreshCaches()
        Debug.Print(rs.ToString())

        Spielfeld.PaintSpielfeld_EndPause(startIniUpdate:=True, raiseIniEvents:=IniEvents.OnUpdate)
    End Sub

#End Region



    'Im Hauptformular sind In im Me.Load- Ereignis einige wenige Initialisierungen angesiedelt. 
    'Außerdem die Erstellung und Verwaltung des Hauptmenues, das dann die Befehle an die einzenen UserControls weitergibt,
    'sowie das Ein- und Aushängen (= sichtbar/unsichtbar machen) der verschiedenen UserControls In das Hauptformular,
    'alles also reine Verwaltungsarbeit.
    'Die Programmlogik ist komplett In den verschiedenen UserControls angesiedelt, von wo aus auf einen gemeinsamen Pool zugegriffen
    'wird, der sich überwiegend im Verzeichnis "Namespace Spielfeld" befindet.

#Region "Initialisierungen"

    Public Enum VisibleUserControl
        None = -1
        Spielfeld
        Editor
        Einstellungen
        SpielfeldWählen
        Hilfe
        Über
    End Enum

    Private Enum Info
        AutoSave
    End Enum

    ''' <summary>
    ''' Das TabControlMain ist nur im Designer sichtbar, damit die UserControls
    ''' im Designer angezeigt werden können.
    ''' Zur Laufzeit wird es abgeschaltet, und das gerade benötigte UserControls
    ''' wird dem FrmMain zugeordnet. Die UserControls werden hier gespeichert
    ''' </summary>
    Private VisibleUserControls As New List(Of Control)
    Private _isRefreshing As Boolean 'bezieht sich auf das ToolStrip nach Änderungen in der Ini

    Sub New()

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.

    End Sub


    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load

        'Aktualisierung der Titelzeile
        If My.Application.IsNetworkDeployed = True Then
            Me.Text &= " " & My.Application.Deployment.CurrentVersion.ToString
        Else
            Me.Text &= " " & Helper.ReadClickOnceVersionFromManifest()
        End If
        'stellt ggf "[IDE]" der Titelzeile voran.
        Helper.IsRunningInIDE(Me)

        'Prüfung, ob die Form auf den Bildschirm passt und ggf Anpassung. 
        Dim wa As Rectangle = Screen.FromControl(Me).WorkingArea
        Dim maxWidth As Integer = CInt(wa.Width * 0.8)
        Dim maxHeight As Integer = CInt(wa.Height * 0.8)

        If Me.Width > maxWidth Then Me.Width = maxWidth
        If Me.Height > maxHeight Then Me.Height = maxHeight

        Me.MinimumSize = Me.SizeFromClientSize(New Size(MJ_SPIELFELD_MIN_WIDTH, MJ_SPIELFELD_MIN_HEIGHT))
        Me.AutoScaleMode = AutoScaleMode.Dpi

        'Die UserControls müssen in der Reihenfolge der Enumeration VisibleUserControl
        'in die Liste eingefügt werden, damit sie später in der richtigen Reihenfolge
        'aufgerufen werden können.
        VisibleUserControls.Add(UCtlSpielfeldMain)
        UCtlSpielfeldMain.Parent = Nothing

        VisibleUserControls.Add(UCtlEditorMain)
        UCtlEditorMain.Parent = Nothing

        VisibleUserControls.Add(UCtlEinstellungenMain)
        UCtlEinstellungenMain.Parent = Nothing

        '
        'Hier weitere UserControls hinzufügen, die im TabControlMain
        'im Designer sichtbar sind.

        'Das TabControlMain entsorgen
        TabControlMain.Parent = Nothing
        TabControlMain.Dispose()

        'und das erste UserControl anzeigen

        AktVisibleUserControl = VisibleUserControl.Spielfeld


        AddHandler INI.ToggleValue_RefreshUINachIniÄnderung_Event, AddressOf RefreshUINachIniÄnderung
        '()

        'Das Menue wird dynamisch erzeugt, damit es
        'übersichtlicher wird, als die statische Erzeugung im Designer.
        BuildMenu(Me.MenuStripMain)

        BuildBottomToolStrip()

        ' Startzustand setzen

        'Spielfeld.TestDaten_StatischesSpielfeld_EineEbenen

        If Debugger.IsAttached Then
            Me.KeyPreview = True
            DebugStep.Attach(Me)
        End If
        '
        'Die INI ist bereits initialisiert, das passiert beim allererstem Zugriff auf einen Wert automatisch.
        'hier geht es um um eine Reinitialisierung mit Werfen der IniEvents.
        INI.Initialisierung(update:=True, raiseIniEventsDefault:=IniEvents.OnChangeValue)

    End Sub

    Private Sub frmMain_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        RemoveHandler INI.ToggleValue_RefreshUINachIniÄnderung_Event, AddressOf RefreshUINachIniÄnderung
    End Sub

#End Region

#Region "HauptMenue"

    ' --- Properties, Felder, Enums ---
    Private menuEnableBindings As New List(Of Tuple(Of ToolStripMenuItem, Func(Of Boolean)))
    Private menuVisibleBindings As New List(Of Tuple(Of ToolStripMenuItem, Func(Of Boolean)))


    Private Sub ChangeVisibleControl(ctrl As VisibleUserControl)
        AktVisibleUserControl = ctrl
    End Sub

    Private _aktVisibleUserControlValue As VisibleUserControl = VisibleUserControl.None
    ' Property mit automatischem Menü-Refresh
    Private Property AktVisibleUserControl As VisibleUserControl
        Get
            Return _aktVisibleUserControlValue
        End Get

        Set(value As VisibleUserControl)

            SuspendLayout()

            'Ein eventuell noch laufendes Spielfeld abschalten
            Spielfeld.PaintSpielfeld_DeInitialisierung()


            'Das bisherige UserControl aushängen. Beim Initialisieren ist das bisherige 
            'VisibleUserControl.None
            Try
                If _aktVisibleUserControlValue <> VisibleUserControl.None Then
                    If Not IsNothing(VisibleUserControls(_aktVisibleUserControlValue)) Then
                        VisibleUserControls(_aktVisibleUserControlValue).Parent = Nothing
                    End If
                End If
            Catch ex As Exception

            End Try

            Try
                'Und das neue Control einhängen.
                VisibleUserControls(value).Parent = Me.PanelFrmMainUGrd
                _aktVisibleUserControlValue = value

            Catch ex As Exception
                If Debugger.IsAttached Then
                    Stop 'Das UserControl existiert noch nicht.
                End If
            End Try

            RefreshMenuStates()

            'Erläuterung siehe PaintLimiterErläuterung.txt
            If value = VisibleUserControl.Spielfeld OrElse
                value = VisibleUserControl.Editor Then
                Spielfeld.PaintSpielfeld_Initialisierung(VisibleUserControls(value), value)
            End If

            ResumeLayout(True)

        End Set
    End Property

    ' --- Spezial-Label für rechtsbündige Menüs ---
    Private Class ToolStripSpringLabel
        Inherits ToolStripLabel
        Protected Overrides Sub OnLayout(e As LayoutEventArgs)
            MyBase.OnLayout(e)
            If Me.Owner IsNot Nothing Then
                Dim springSpace As Integer = Me.Owner.DisplayRectangle.Width
                For Each item As ToolStripItem In Me.Owner.Items
                    If item IsNot Me AndAlso item.Alignment = ToolStripItemAlignment.Left Then
                        springSpace -= item.Width
                    End If
                Next
                Me.Width = Math.Max(springSpace, 0)
            End If
        End Sub
    End Class

    ' --- Menüaufbau ---
    Private Sub BuildMenu(ms As MenuStrip)

        ms.Items.Clear()
        menuEnableBindings.Clear()
        menuVisibleBindings.Clear()

        ' === Datei ===
        Dim mnuDatei As New ToolStripMenuItem("Datei")
        mnuDatei.DropDownItems.Add(CreateMenuItem("Letzten Spielstand laden", Sub() SpielstandLoad(False)))
        mnuDatei.DropDownItems.Add(CreateMenuItem("Spielstand laden", Sub() SpielstandLoad(True)))
        mnuDatei.DropDownItems.Add(New ToolStripSeparator())
        mnuDatei.DropDownItems.Add(CreateMenuItem("Spielstand speichern", Sub() SpielstandSave(False)))
        mnuDatei.DropDownItems.Add(CreateMenuItem("Spielstand speichern unter", Sub() SpielstandSave(True)))
        mnuDatei.DropDownItems.Add(CreateCheckMenuItem("Automatisch speichern",
                                                       INI.Spielbetrieb_AutoSave,
                                                       Sub(sender)
                                                           Dim itm As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
                                                           INI.Spielbetrieb_AutoSave = itm.Checked
                                                       End Sub))
        mnuDatei.DropDownItems.Add(CreateMenuItem("Info", Sub() ShowInfo(Info.AutoSave)))

        ' === Spiel ===
        Dim mnuSpiel As New ToolStripMenuItem("Spiel")
        mnuSpiel.DropDownItems.Add(CreateMenuItem("Spielen",
                                                  Sub() ChangeVisibleControl(VisibleUserControl.Spielfeld),
                                                   Function() As Boolean
                                                       Return AktVisibleUserControl <> VisibleUserControl.Spielfeld
                                                   End Function))

        mnuSpiel.DropDownItems.Add(CreateMenuItem("Spielfeld wählen",
                                                  Sub() ChangeVisibleControl(VisibleUserControl.SpielfeldWählen),
                                                  Function() As Boolean
                                                      Return AktVisibleUserControl <> VisibleUserControl.SpielfeldWählen
                                                  End Function))

        mnuSpiel.DropDownItems.Add(CreateMenuItem("Spielfeld zufällig wählen", Sub() SelectRandomSpielfeld()))


        Dim mnuEditor As New ToolStripMenuItem("Editor")

        Dim editorItem As ToolStripMenuItem = CreateMenuItem("Editor",
                                                         Sub() ChangeVisibleControl(VisibleUserControl.Editor),
                                                         Function() As Boolean
                                                             Return AktVisibleUserControl <> VisibleUserControl.Editor
                                                         End Function)



        ' Hier wird geprüft, ob der Editor verwendet werden darf
        menuVisibleBindings.Add(New Tuple(Of ToolStripMenuItem, Func(Of Boolean))(
                                                    editorItem,
                                                    Function() As Boolean
                                                        Return INI.Editor_UsingEditorAllowed
                                                    End Function
                                                    ))
        mnuEditor.DropDownItems.Add(editorItem)

        ' === Einstellungen ===
        Dim mnuEinstellungen As New ToolStripMenuItem("Einstellungen")
        AddHandler mnuEinstellungen.Click, Sub() ChangeVisibleControl(VisibleUserControl.Einstellungen)

        menuEnableBindings.Add(New Tuple(Of ToolStripMenuItem, Func(Of Boolean))(
                                            mnuEinstellungen,
                                            Function() As Boolean
                                                Return AktVisibleUserControl <> VisibleUserControl.Einstellungen
                                            End Function))

        ' === Rechtsbündiger Teil ===
        Dim spring As New ToolStripSpringLabel() With {.AutoSize = False}

        ' === Hilfe ===
        Dim mnuHilfe As New ToolStripMenuItem("Hilfe")
        mnuHilfe.DropDownItems.Add(CreateMenuItem("Hilfe",
                                                  Sub() ChangeVisibleControl(VisibleUserControl.Hilfe),
                                                   Function() As Boolean
                                                       Return AktVisibleUserControl <> VisibleUserControl.Hilfe
                                                   End Function))

        mnuHilfe.DropDownItems.Add(CreateMenuItem("Über",
                                                  Sub() ChangeVisibleControl(VisibleUserControl.Über),
                                                   Function() As Boolean
                                                       Return AktVisibleUserControl <> VisibleUserControl.Über
                                                   End Function))

        If INI.Editor_UsingEditorAllowed Then
            ' --- Menüs hinzufügen ---
            ms.Items.AddRange({mnuDatei, mnuSpiel, mnuEditor, mnuEinstellungen, spring, mnuHilfe})
        Else
            ms.Items.AddRange({mnuDatei, mnuSpiel, mnuEinstellungen, spring, mnuHilfe})
        End If

    End Sub

    ' --- Hilfsfunktionen ---
    Private Function CreateMenuItem(text As String, action As Action, Optional enabledCondition As Func(Of Boolean) = Nothing) As ToolStripMenuItem
        Dim itm As New ToolStripMenuItem(text)
        AddHandler itm.Click, Sub(sender, e) action()
        If enabledCondition IsNot Nothing Then
            menuEnableBindings.Add(New Tuple(Of ToolStripMenuItem, Func(Of Boolean))(itm, enabledCondition))

        End If
        Return itm
    End Function

    Private Function CreateCheckMenuItem(text As String, isChecked As Boolean, onCheckedChanged As Action(Of Object)) As ToolStripMenuItem
        Dim itm As New ToolStripMenuItem(text) With {.CheckOnClick = True, .Checked = isChecked}
        AddHandler itm.CheckedChanged,
            Sub(sender, e)
                onCheckedChanged(sender)
            End Sub
        Return itm
    End Function

    Private Sub RefreshMenuStates()
        For Each bindingtpl As Tuple(Of ToolStripMenuItem, Func(Of Boolean)) In menuEnableBindings
            'Debug.WriteLine($"Type von Item2: {bindingtpl.Item2.GetType().FullName}")
            'Das hier geht nicht, die IDE meckert
            'Der Wert vom Typ "Func(Of Boolean)" kann nicht in "Boolean" konvertiert werden.
            'bindingtpl.Item1.Enabled = bindingtpl.Item2()
            'Nach langer Suche:
            Dim func As Func(Of Boolean) = bindingtpl.Item2
            Dim result As Boolean = func()
            bindingtpl.Item1.Enabled = result
        Next
        For Each bindingtpl As Tuple(Of ToolStripMenuItem, Func(Of Boolean)) In menuVisibleBindings
            'Problem wie oben.
            Dim func As Func(Of Boolean) = bindingtpl.Item2
            Dim result As Boolean = func()
            bindingtpl.Item1.Enabled = result
        Next
    End Sub

    ' --- Platzhalter-Subs ---
    Private Sub SpielstandLoad(forceDialog As Boolean)
        MessageBox.Show("SpielstandLoad(" & forceDialog & ")")
    End Sub

    Private Sub SpielstandSave(forceDialog As Boolean)
        MessageBox.Show("SpielstandSave(" & forceDialog & ")")
    End Sub

    Private Sub ShowInfo(infoType As Info)
        MessageBox.Show("Info: " & infoType.ToString())
    End Sub



    Private Sub SelectRandomSpielfeld()
        MessageBox.Show("Zufälliges Spielfeld wählen")
    End Sub



#End Region

#Region "ToolStrip unten Initialisierung"


    ' --- Aufruf z. B. im Load-Event:
    ' Private Sub frmMain_Load(...) Handles MyBase.Load
    '     InitBottomToolStrip()
    ' End Sub

    Private Sub BuildBottomToolStrip()
        ' Basis
        With ToolStripMain
            .Dock = DockStyle.Bottom
            .GripStyle = ToolStripGripStyle.Hidden
            .AutoSize = False
            .Height = 28
            .CanOverflow = False
            .LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow
            .ImageScalingSize = New Size(16, 16)
            .ShowItemToolTips = INI.Sonstiges_ShowToolTips
            .SuspendLayout()
            .Items.Clear()
        End With

        ' =======================
        ' Debug-Gruppe (links) – nur wenn Debugger.IsAttached
        ' =======================
        If Debugger.IsAttached Then
            ToolStripMain.Items.Add(MkBtnText("dbg_ini", "INI",
            Sub() INI.IniEditieren(),
            If(INI.Sonstiges_ShowToolTips, "INI während der Laufzeit bearbeiten", Nothing)))

            'ToolStripMain.Items.Add(MkBtnText("dbg_explorer", "Explorer",
            'Sub() Process.Start("explorer.exe", AppDataDirectory),
            'If(INI.Sonstiges_ShowToolTips, "Öffnet AppData-Verzeichnis", Nothing)))

            ToolStripMain.Items.Add(MkBtnText("dbg_go", "Go",
            Sub() Go(),
            If(INI.Sonstiges_ShowToolTips, "frmMain Sub Go ausführen", Nothing)))

            ToolStripMain.Items.Add(MkBtnText("dbg_toggle", "Tgl",
            Sub() Toggle(),
            If(INI.Sonstiges_ShowToolTips, "frmMain Sub Toggle ausführen", Nothing)))

            ToolStripMain.Items.Add(MkBtnText("dbg_test1", "T1",
            Sub() Test1(True),
            If(INI.Sonstiges_ShowToolTips, "frmMain Sub Test1 ausführen", Nothing)))

            ToolStripMain.Items.Add(MkBtnText("dbg_test2", "T2",
            Sub() Test2(),
            If(INI.Sonstiges_ShowToolTips, "frmMain Sub Test2 ausführen", Nothing)))

            '  ToolStripMain.Items.Add(New ToolStripSeparator())

            ' ---- Dateiexplorer aufrufen ----
            Dim cbo1 As New ToolStripComboBox("dbg_fileexpl") With {
                .DropDownStyle = ComboBoxStyle.DropDownList,
                .AutoSize = False,
                .Width = 100
            }
            'Anzeige, Farbe(ohne #), optionales Thema/Query
            Dim entries As (Label As String, Query As String)() = {
                ("AppDataDir", AppDataDirectory),
                ("Download", INI.IfRunningInIDE_DownloadDirectory),
                ("Gfx 16x16", INI.IfRunningInIDE_Grafik16x16Directory)
            }

            ' Items befüllen
            For Each e As (Label As String, Query As String) In entries
                cbo1.Items.Add(e.Label)
            Next
            If cbo1.Items.Count > 0 Then cbo1.SelectedIndex = 0
            If INI.Sonstiges_ShowToolTips Then
                cbo1.ToolTipText = "Windows Dateiexplorer mit vorgegebenem Pfad."
            End If

            AddHandler cbo1.SelectedIndexChanged,
                Sub()
                    Dim idx As Integer = cbo1.SelectedIndex
                    If idx >= 0 AndAlso idx < entries.Length Then
                        Dim e As (Label As String, Query As String) = entries(idx)
                        Process.Start("explorer.exe", e.Query)
                    End If
                End Sub

            ToolStripMain.Items.Add(cbo1)



            ' ---- Icon-Kategorien → Material Icons mit Farbvorgabe öffnen ----
            Dim cbo2 As New ToolStripComboBox("dbg_iconcats") With {
                .DropDownStyle = ComboBoxStyle.DropDownList,
                .AutoSize = False,
                .Width = 160
            }
            ' Anzeige, Farbe (ohne #), optionales Thema/Query
            Dim entries2 As (Label As String, Hex As String, Query As String)() = {
                ("Neutral (Anthrazit)", "404040", ""),
                ("Checked (Schwarz)", "000000", ""),
                ("UnChecked (Grau)", "A0A0A0", ""),
                ("Error / Stop (Rot)", "C00000", "error"),
                ("Warnung (Orange)", "CC6600", "warning"),
                ("Bestätigen / Redo (Grün)", "008000", "redo"),
                ("Tipps / Info (Blau)", "0066CC", "help info tip"),
                ("Screenshot / Tools (Violett)", "663399", "screenshot camera"),
                ("Optionen (Grau-Blau)", "3A6E7F", "settings options"),
                ("Hellgrau", "B0B0B0", "outline"),
                ("Weiß", "FFFFFF", "filled"),
                ("Schwarz", "000000", "solid"),
                ("Thema: Undo/Redo (Neutral)", "404040", "undo redo"),
                ("Thema: Editor/Werkzeuge (Neutral)", "404040", "edit build tools"),
                ("Thema: Tipps/Hinweise (Neutral)", "404040", "lightbulb tips"),
                ("Thema: Anzeige/Ansicht (Neutral)", "404040", "visibility view"),
                ("Thema: Ordner/Datei (Neutral)", "404040", "folder file")
            }

            ' Items befüllen
            For Each e As (Label As String, Hex As String, Query As String) In entries2
                cbo2.Items.Add(e.Label)
            Next
            If cbo2.Items.Count > 0 Then cbo2.SelectedIndex = 0
            If INI.Sonstiges_ShowToolTips Then
                cbo2.ToolTipText = "Google Material Icons mit vorgewählter Farbe und optionalem Thema öffnen"
            End If

            AddHandler cbo2.SelectedIndexChanged,
                Sub()
                    Dim idx As Integer = cbo2.SelectedIndex
                    If idx >= 0 AndAlso idx < entries2.Length Then
                        Dim e As (Label As String, Hex As String, Query As String) = entries2(idx)
                        Dim url As String = BuildMaterialIconsUrl(e.Hex, e.Query)
                        OpenUrlInBrowser(url)
                    End If
                End Sub

            ToolStripMain.Items.Add(cbo2)
            ToolStripMain.Items.Add(New ToolStripSeparator())

        End If

        ' =======================
        ' Editor-Gruppe (links), nur wenn Allowed
        ' =======================
        If INI.Editor_UsingEditorAllowed Then
            ToolStripMain.Items.Add(MkBtnImg("grpEditor_player", GetAppGrafik(AppGrafikName.Spieler), Sub() DoPlayer(),
            If(INI.Sonstiges_ShowToolTips, "Ruft das Spiel auf.", Nothing)))
            ToolStripMain.Items.Add(MkBtnImg("grpEditor_editor", GetAppGrafik(AppGrafikName.Editor), Sub() DoEditor(),
            If(INI.Sonstiges_ShowToolTips, "Ruft den Editor auf.", Nothing)))
            ToolStripMain.Items.Add(MkBtnImg("grpEditor_werkbank", GetAppGrafik(AppGrafikName.Werkbank), Sub() DoWerkbank(),
            If(INI.Sonstiges_ShowToolTips, "Ruft die Werkbank auf.", Nothing)))
            ToolStripMain.Items.Add(MkBtnImg("grpEditor_toolbox", GetAppGrafik(AppGrafikName.Werkzeugkiste), Sub() DoWerkzeugkiste(),
            If(INI.Sonstiges_ShowToolTips, "Ruft die Werkzeugkiste auf.", Nothing)))
            ToolStripMain.Items.Add(New ToolStripSeparator())

            ' Enabled-Zustand abhängig von UsingEditor
            SetGroupEnabled("grpEditor", INI.Editor_UsingEditor)
        End If


        ' =======================
        ' Status-Gruppe (links)
        ' =======================
        ToolStripMain.Items.Add(MkLbl("stat_title", "Steine:"))
        ToolStripMain.Items.Add(New ToolStripSeparator())
        ToolStripMain.Items.Add(MkLbl("stat_total", "0 Gesamt"))
        ToolStripMain.Items.Add(New ToolStripSeparator())
        ToolStripMain.Items.Add(MkLbl("stat_current", "0 Aktuell"))
        ToolStripMain.Items.Add(New ToolStripSeparator())
        ToolStripMain.Items.Add(MkLbl("stat_sel", "0 wählbar"))
        ToolStripMain.Items.Add(New ToolStripSeparator())
        ToolStripMain.Items.Add(MkLbl("stat_pairs", "0 Paare"))
        ToolStripMain.Items.Add(New ToolStripSeparator())



        ' =======================
        ' Rechte Gruppe (Alignment = Right)
        ' Einfüge-Reihenfolge = von rechts nach links
        ' =======================

        ToolStripMain.Items.Add(MkBtnImgRight("act_screenshot", GetAppGrafik(AppGrafikName.Screenshot), Sub() DoTakeScreenShot(),
        If(INI.Sonstiges_ShowToolTips, "Erzeugt einen ScreenShot vom Spiel/Editor/Werkbank.", Nothing)))

        ToolStripMain.Items.Add(MkBtnImgRight("act_statistik", GetAppGrafik(AppGrafikName.Statistik), Sub() DoStatistik(),
            If(INI.Sonstiges_ShowToolTips, "Statistische Daten zum Spiel", Nothing)))

        ToolStripMain.Items.Add(MkSepRight())

        ' '' 4) Label "Zeige:"
        ''ToolStripMain.Items.Add(MkLblRight("act_show_lbl", "Zeige:"))

        ToolStripMain.Items.Add(MkBtnImgTextRight("act_tip1", GetAppGrafik(AppGrafikName.Tip), "",
            Sub() DoTipEinzel(),
            If(INI.Sonstiges_ShowToolTips, "Schalter: Zeigt permanent wählbare Steine an.", Nothing)))


        Dim chkWinds As ToolStripButton = MkToggleImgRight("opt_winds_onegrp",
            GetAppGrafik(AppGrafikName.WindsChecked), GetAppGrafik(AppGrafikName.WindsUnChecked),
            INI.Spielbetrieb_WindsAreInOneClickGroup,
            Sub(checked)
                If _isRefreshing Then Return
                INI.Spielbetrieb_WindsAreInOneClickGroup = checked
                ' ggf. Regeln neu anwenden/Refresh
            End Sub,
            If(INI.Sonstiges_ShowToolTips, "Schalter vereinfachte Spielregel: Alle Winde können untereinander Paare bilden.", Nothing))

        ToolStripMain.Items.Add(chkWinds)

        ToolStripMain.Items.Add(MkSepRight())

        ToolStripMain.Items.Add(MkBtnImgTextRight("act_wählbar", GetAppGrafik(AppGrafikName.Tipps), "",
            Sub() DoTipAlle(),
            If(INI.Sonstiges_ShowToolTips, "Tip: Zeigt alle wählbaren Paare an.", Nothing)))

        Dim chkSel As ToolStripButton = MkToggleImgRight("opt_show_sel",
                GetAppGrafik(AppGrafikName.ShowSelectableChecked), GetAppGrafik(AppGrafikName.ShowSelectableUnChecked),
                INI.Spielbetrieb_ShowSelectableStones,
                Sub(checked)
                    If _isRefreshing Then Return
                    INI.Spielbetrieb_ShowSelectableStones = checked
                    ' ggf. Refresh/Neuzeichnen hier
                End Sub,
                If(INI.Sonstiges_ShowToolTips, "Tip: Zeigt alle wählbaren Steine an.", Nothing))
        ToolStripMain.Items.Add(chkSel)


        ToolStripMain.Items.Add(MkSepRight())

        ToolStripMain.Items.Add(MkBtnImgRight("act_restart", GetAppGrafik(AppGrafikName.Restart), Sub() DoReDo(),
            If(INI.Sonstiges_ShowToolTips, "Stell das Spiel auf die Ausgangsstellung zurück", Nothing)))

        ToolStripMain.Items.Add(MkBtnImgRight("act_redo", GetAppGrafik(AppGrafikName.Redo), Sub() DoReDo(),
            If(INI.Sonstiges_ShowToolTips, "Arbeitet wieder vorwärts, solange das noch möglich ist", Nothing)))

        ToolStripMain.Items.Add(MkBtnImgRight("act_undo", GetAppGrafik(AppGrafikName.Undo), Sub() DoUndo(),
            If(INI.Sonstiges_ShowToolTips, "Setzt das letzte Steinpaar wieder auf das Spielfeld", Nothing)))

        ToolStripMain.Items.Add(MkSepRight())

        ToolStripMain.ResumeLayout()
        ToolStripMain.PerformLayout()
    End Sub

    ' --- Helper: Labels ---
    Private Function MkLbl(name As String, text As String) As ToolStripLabel
        Return New ToolStripLabel(text) With {.Name = name}
    End Function

    Private Function MkLblRight(name As String, text As String) As ToolStripLabel
        Return New ToolStripLabel(text) With {.Name = name, .Alignment = ToolStripItemAlignment.Right}
    End Function

    ' --- Helper: Buttons (Text / Image / Image+Text) ---
    Private Function MkBtnText(name As String, caption As String, onClick As Action, tip As String) As ToolStripButton
        Dim b As New ToolStripButton(caption) With {.Name = name, .DisplayStyle = ToolStripItemDisplayStyle.Text}
        If tip IsNot Nothing Then b.ToolTipText = tip
        AddHandler b.Click, Sub() onClick()
        Return b
    End Function

    Private Function MkBtnImg(name As String, img As Image, onClick As Action, tip As String) As ToolStripButton
        Dim b As New ToolStripButton() With {.Name = name, .Image = img, .DisplayStyle = ToolStripItemDisplayStyle.Image}
        If tip IsNot Nothing Then b.ToolTipText = tip
        AddHandler b.Click, Sub() onClick()
        Return b
    End Function

    Private Function MkBtnImgRight(name As String, img As Image, onClick As Action, tip As String) As ToolStripButton
        Dim b As ToolStripButton = MkBtnImg(name, img, onClick, tip)
        b.Alignment = ToolStripItemAlignment.Right
        Return b
    End Function

    Private Function MkBtnImgTextRight(name As String, img As Image, caption As String, onClick As Action, tip As String) As ToolStripButton
        Dim b As New ToolStripButton(caption) With {
        .Name = name,
        .Image = img,
        .DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
        .ImageAlign = ContentAlignment.MiddleLeft
    }
        b.Alignment = ToolStripItemAlignment.Right
        If tip IsNot Nothing Then b.ToolTipText = tip
        AddHandler b.Click, Sub() onClick()
        Return b
    End Function

    ' --- Helper: Toggle-Buttons mit Bildwechsel (als „Checkbox mit Grafik“) ---
    Private Function MkToggleImgRight(name As String,
                                  imgUnchecked As Image,
                                  imgChecked As Image,
                                  initialChecked As Boolean,
                                  onChanged As Action(Of Boolean),
                                  tip As String) As ToolStripButton
        Dim b As New ToolStripButton() With {
        .Name = name,
        .CheckOnClick = True,
        .Checked = initialChecked,
        .Image = If(initialChecked, imgChecked, imgUnchecked),
        .Alignment = ToolStripItemAlignment.Right,
        .DisplayStyle = ToolStripItemDisplayStyle.Image
    }
        If tip IsNot Nothing Then b.ToolTipText = tip
        AddHandler b.CheckedChanged,
        Sub()
            If _isRefreshing Then Return 'richtig? (bin mir unsicher)
            b.Image = If(b.Checked, imgChecked, imgUnchecked)
            onChanged(b.Checked)
        End Sub
        Return b
    End Function

    ' --- Helper: Separator rechts ---
    Private Function MkSepRight() As ToolStripSeparator
        Return New ToolStripSeparator() With {.Alignment = ToolStripItemAlignment.Right}
    End Function

    ' --- Gruppe aktivieren/deaktivieren (per Namenspräfix) ---
    Private Sub SetGroupEnabled(prefix As String, enabled As Boolean)
        For Each it As ToolStripItem In ToolStripMain.Items
            If it.Name IsNot Nothing AndAlso it.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) Then
                it.Enabled = enabled
            End If
        Next
    End Sub

    ' --- API: von außen Editor-Gruppe schalten (und später erweitern) ---
    Public Sub UsingEditor(enabled As Boolean)
        SetGroupEnabled("grpEditor", enabled)
        ' hier ggf. eigene Ergänzungen (Ansichten umschalten etc.)
    End Sub

    ' --- Platzhaltergrafik 16x16 ---
    Private Function DummyGrafik() As Image
        Dim bmp As New Bitmap(16, 16)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.Clear(Color.Transparent)
            ' kleiner Rahmen + Diagonale, damit man "etwas" sieht
            Using p As New Pen(Color.Gray)
                g.DrawRectangle(p, 0, 0, 15, 15)
                g.DrawLine(p, 0, 15, 15, 0)
            End Using
        End Using
        Return bmp
    End Function

    ''' <summary>
    ''' Aktualisiert die Statuswerte (Gesamt, Aktuell, Wählbar, Paare).
    ''' </summary>
    Public Sub UpdateStatus(gesamt As Integer, aktuell As Integer, waehlbar As Integer, paare As Integer)
        Dim lblGesamt As ToolStripLabel = TryCast(ToolStripMain.Items("stat_total"), ToolStripLabel)
        Dim lblAktuell As ToolStripLabel = TryCast(ToolStripMain.Items("stat_current"), ToolStripLabel)
        Dim lblWaehlbar As ToolStripLabel = TryCast(ToolStripMain.Items("stat_sel"), ToolStripLabel)
        Dim lblPaare As ToolStripLabel = TryCast(ToolStripMain.Items("stat_pairs"), ToolStripLabel)

        If lblGesamt IsNot Nothing Then lblGesamt.Text = $"{gesamt} Gesamt"
        If lblAktuell IsNot Nothing Then lblAktuell.Text = $"{aktuell} Aktuell"
        If lblWaehlbar IsNot Nothing Then lblWaehlbar.Text = $"{waehlbar} wählbar"
        If lblPaare IsNot Nothing Then lblPaare.Text = $"{paare} Paare"
    End Sub
    ''' <summary>
    ''' Zieht nach INI-Änderungen die UI-Zustände im ToolStrip nach:
    ''' - ToolTips zeigen/unterdrücken
    ''' - Toggle-Buttons (grafische "Checkboxen") setzen
    ''' - Editor-Gruppe aktivieren/deaktivieren
    ''' </summary>
    Public Sub RefreshUINachIniÄnderung()
        If ToolStripMain Is Nothing Then Exit Sub

        _isRefreshing = True
        Try
            ' 1) ToolTips (global für ToolStrip-Items)
            ToolStripMain.ShowItemToolTips = INI.Sonstiges_ShowToolTips

            ' 2) Toggle: "alle wählbaren Steine anzeigen"
            Dim btnShowSel As ToolStripButton = TryCast(ToolStripMain.Items("opt_show_sel"), ToolStripButton)
            If btnShowSel IsNot Nothing Then
                btnShowSel.Checked = INI.Spielbetrieb_ShowSelectableStones
                ' Bild entsprechend dem Zustand (hier Dummy – bei dir echte Grafiken einsetzen)
                btnShowSel.Image = If(btnShowSel.Checked, GetAppGrafik(AppGrafikName.ShowSelectableChecked), GetAppGrafik(AppGrafikName.ShowSelectableUnChecked))
                If INI.Sonstiges_ShowToolTips Then
                    btnShowSel.ToolTipText = "Zeigt alle wählbaren Steine an."
                Else
                    btnShowSel.ToolTipText = Nothing
                End If
            End If

            ' 3) Toggle: "Winde bilden gemeinsame Paargruppe"
            Dim btnWinds As ToolStripButton = TryCast(ToolStripMain.Items("opt_winds_onegrp"), ToolStripButton)
            If Not IsNothing(btnWinds) Then
                btnWinds.Checked = INI.Spielbetrieb_WindsAreInOneClickGroup
                btnWinds.Image = If(btnWinds.Checked, GetAppGrafik(AppGrafikName.WindsChecked), GetAppGrafik(AppGrafikName.WindsUnChecked))
                If INI.Sonstiges_ShowToolTips Then
                    btnWinds.ToolTipText = "Vereinfachte Spielregel: Alle Winde können Paare bilden."
                Else
                    btnWinds.ToolTipText = Nothing
                End If
            End If

            ' 4) Editor-Gruppe (Allowed = Neustartpflicht, daher hier nur Enabled)
            UsingEditor(INI.Editor_UsingEditor)

            ' 5) Optional: ToolTips der restlichen Items je nach Setting unterdrücken/setzen
            If Not INI.Sonstiges_ShowToolTips Then
                ' Wer mag, kann hier weitere ToolTips leeren:
                ' SafeSetTip("act_undo", Nothing)
                ' SafeSetTip("act_redo", Nothing)
                ' ...
            End If

            ToolStripMain.PerformLayout()

        Finally
            _isRefreshing = False
        End Try
    End Sub

    ' Hilfsfunktion: ToolTipText sicher setzen (falls Item existiert)
    Private Sub SafeSetTip(itemName As String, tip As String)
        Dim it As ToolStripItem = ToolStripMain.Items(itemName)
        If it IsNot Nothing Then it.ToolTipText = tip
    End Sub

    Private Sub OpenUrlInBrowser(url As String)
        Try
            Dim psi As New ProcessStartInfo(url) With {.UseShellExecute = True}
            Process.Start(psi)
        Catch
            ' Fallback
            Process.Start("explorer.exe", url)
        End Try
    End Sub

    Private Function BuildMaterialIconsUrl(hexNoHash As String, query As String) As String
        Dim clean As String = hexNoHash.Trim().TrimStart("#"c)
        Dim baseUrl As String = $"https://fonts.google.com/icons?icon.size=16&icon.color=%23{clean}"
        If Not String.IsNullOrWhiteSpace(query) Then
            Dim q As String = Uri.EscapeDataString(query.Trim())
            ' Material Icons nutzt die Such-Query im Pfad-Param "q"
            baseUrl &= $"&q={q}"
        End If
        ' Voreinstellung irgendeines Icons bleibt optional; Suche überschreibt die Ansicht
        Return baseUrl
    End Function

    Private Function BuildMaterialIconsUrl(hexNoHash As String) As String
        ' Verwendet dein Muster; nur die Farbe wird ersetzt.
        ' icon.size=16 bleibt, Farbe als %23RRGGBB kodiert.
        Dim clean As String = hexNoHash.Trim().TrimStart("#"c)
        Return $"https://fonts.google.com/icons?icon.size=16&icon.color=%23{clean}&selected=Material+Symbols+Outlined:search:FILL@0;wght@400;GRAD@0;opsz@20"
    End Function


#End Region

#Region "ToolStrip unten Event-Verarbeitung"

    Private Sub DoPlayer()

    End Sub

    Private Sub DoEditor()

    End Sub

    Private Sub DoWerkbank()

    End Sub

    Private Sub DoWerkzeugkiste()

    End Sub

    Private Sub DoTakeScreenShot()
        Spielfeld.PaintSpielfeld_CreateScreenShot()
        MsgBox("Screnshot erzeugt.", MsgBoxStyle.Information)
    End Sub

    Private Sub DoTipEinzel()

    End Sub

    Private Sub DoTipAlle()

    End Sub

    Private Sub DoReDo()

    End Sub

    Private Sub DoUndo()

    End Sub

    Private Sub DoStatistik()

    End Sub



#End Region


End Class