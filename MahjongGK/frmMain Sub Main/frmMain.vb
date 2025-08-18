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

''' <summary>
''' Im Hauptformular sind in im Me.Load- Ereignis einige wenige
''' Initialisierungen angesiedelt. 
''' Außerdem die Erstellung und Verwaltung des Hauptmenues, das
''' dann die Befehle an die einzenen UserControls weitergibt,
''' sowie das Ein- und Aushängen (= sichtbar/unsichtbar machen) 
''' der verschiedenen UserControls in das Hauptformular,
''' alles also reine Verwaltungsarbeit.
''' Die Programmlogik ist komplett in den verschiedenen UserControls
''' angesiedelt, von wo aus auf einen gemeinsamen Pool zugegriffen
''' wird, der sich überwiegend im Verzeichnis "Shared" befindet.
''' </summary>
Public Class frmMain

#Region "Initialisierungen"

    Public Enum VisibleUserControl
        None = -1
        Spielfeld
        Einstellungen
        SpielfeldWählen
        SpielfeldEditor
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

        Me.MinimumSize = Me.SizeFromClientSize(New Size(Spielfeld.MJ_SPIELFELD_MIN_WIDTH, Spielfeld.MJ_SPIELFELD_MIN_HEIGHT))
        Me.AutoScaleMode = AutoScaleMode.Dpi

        'Die UserControls müssen in der Reihenfolge der Enumeration VisibleUserControl
        'in die Liste eingefügt werden, damit sie später in der richtigen Reihenfolge
        'aufgerufen werden können.
        VisibleUserControls.Add(UCtlSpielfeldMain)
        UCtlSpielfeldMain.Parent = Nothing

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

        INI.Initialisierung()

        'Das Menue wird dynamisch erzeugt, damit es
        'übersichtlicher wird, als die statische Erzeugung im Designer.
        Me.MainMenuStrip = New MenuStrip()
        BuildMenu(Me.MainMenuStrip)
        Me.Controls.Add(Me.MainMenuStrip)

        ' Startzustand setzen

        'Spielfeld.TestDaten_StatischesSpielfeld_EineEbenen


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

            'Ein eventuell noch laufendes Spielfeld abschalten
            Spielfeld.PaintSpielfeld_CancelGeneralPermission()

            'Erläuterung siehe PaintLimiterErläuterung.txt
            If value = VisibleUserControl.Spielfeld OrElse
                value = VisibleUserControl.SpielfeldEditor Then
                Spielfeld.PaintSpielfeld_GiveGeneralPermission(VisibleUserControls(value), value)
            End If

            'Das bisherige UserControl aushängen. Beim Initialisieren ist das bisherige 
            'VisibleUserControl.None, deshalb den Fehler einfach nicht beachten.
            Try
                If Not IsNothing(VisibleUserControls(_aktVisibleUserControlValue)) Then
                    VisibleUserControls(_aktVisibleUserControlValue).Parent = Nothing
                End If
            Catch ex As Exception
            End Try

            'Und das neue Control einhängen.
            VisibleUserControls(value).Parent = Me
            _aktVisibleUserControlValue = value


            RefreshMenuStates()

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
                                                       INI.Spiel_AutoSave,
                                                       Sub(sender)
                                                           Dim itm As ToolStripMenuItem = DirectCast(sender, ToolStripMenuItem)
                                                           INI.Spiel_AutoSave = itm.Checked
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

        Dim editorItem As ToolStripMenuItem = CreateMenuItem("Spielfeld-Editor",
                                                 Sub() ChangeVisibleControl(VisibleUserControl.Einstellungen),
                                                 Function() As Boolean
                                                     Return AktVisibleUserControl <> VisibleUserControl.Einstellungen
                                                 End Function)

        ' Hier wird geprüft, ob der Editor verwendet werden darf
        menuVisibleBindings.Add(New Tuple(Of ToolStripMenuItem, Func(Of Boolean))(
                                                editorItem,
                                                Function() As Boolean
                                                    Return INI.Editor_UsingEditorAllowed
                                                End Function
                                                ))
        mnuSpiel.DropDownItems.Add(editorItem)

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

        ' --- Menüs hinzufügen ---
        ms.Items.AddRange({mnuDatei, mnuSpiel, mnuEinstellungen, spring, mnuHilfe})

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
        AddHandler itm.CheckedChanged, Sub(sender, e) onCheckedChanged(sender)
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


End Class