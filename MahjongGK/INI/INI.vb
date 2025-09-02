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

Imports System.Reflection

Public Module INI

    '' Kopiervorlage für die Pfad/ Dateinamen-Enumeration
    '' (Die verwendeten Enumerationen sind in Shared/Enumerationen)
    ''
    '' ''' <summary>
    '' ''' Enumeration der verwendeten Unterverzeichnisse in "C:\Users\aktueller User\MahjongGK\SubDefault.value.ToString"
    '' ''' Verwendung: Entweder über Dim Path As String = INI.AppDataDefault(.....)
    '' ''' oder durch Nutzung (im Modul INI) einer Public Property Kopier_VorlageFürPfade As String
    '' ''' Die gewünschten Pfade werden automatisch angelegt.
    '' ''' </summary>
    '' Public Enum AppDataSubDir
    ''     None
    ''     INI
    ''     Steine
    '' End Enum
    '' ''' <summary>
    '' ''' Enumeration der verwendeten Unterverzeichnisse in "C:\Users\aktueller User\MahjongGK\SubDefault.value.ToString\SubSubDefault.value.ToString"
    '' ''' Verwendung wie AppDataSubDir
    '' ''' </summary>
    '' Public Enum AppDataSubSubDir
    ''     None
    ''     letztesSpiel
    ''     Layout
    '' End Enum

    '' ''' <summary>
    '' ''' Enumeration der verwendeten Dateinamen.
    '' ''' Die Endung mit einem Unterstrich abtrennen.
    '' ''' Die Endung muss 3 Zeichen lang sein.
    '' ''' </summary>
    '' Public Enum AppDataFileName
    ''     None
    ''     Steininfos_xml
    '' End Enum

    '' Public Enum AppDataTimeStamp
    ''     None
    ''     Add
    ''     LookForLastTimeStamp
    '' End Enum

    ''' <summary>
    ''' In dieser Enum kann ein Pattern verschlüsselt werden.
    ''' Es gilt: 
    ''' _Q_ = ? (Question, Fragezeichen),
    ''' _N_ = # (Number),
    ''' _S_ = * (Stern, Star),
    ''' _D_ = . (Dot, Punkt).
    ''' _SD_ = *.
    ''' _SDS_ = *.*
    ''' Beispiel: Dateiname_S__D_ext --> Dateiname*.ext
    ''' </summary>
    Public Enum AppDataFilePattern
        None
        Steininfos_xml
    End Enum

    ' Zentrale Sammlung aller IniManager
    'Wird aus New IniManager("Basis.ini") heraus gefüllt.
    Public ReadOnly AllIniManagers As New List(Of IniManager)

    Public ReadOnly BasisIni As IniManager
    Public ReadOnly Spieler1Ini As IniManager
    Public ReadOnly Spieler2Ini As IniManager

    Sub New()
        'Instanzen für verschiedene INIs
        BasisIni = New IniManager("Basis.ini")

        'Spieler1Ini = New IniManager("Spieler1.ini")
        'Spieler2Ini = New IniManager("Spieler2.ini")
        InitAllDefaults()
    End Sub

    Public Sub Initialisierung()
        'Irgendwelcher Code ist hier nicht notwendig.
        'Sub New() wird aufgerufen, sobald der erste Zugriff auf INI
        'erfolgt.
    End Sub

    ''' <summary>
    ''' Muss aus frmMain.FormClosing heraus aufgerufen werden, um sicherzustellen,
    ''' dass die INI-Daten alle gespeichert werden.
    ''' </summary>
    Public Sub DisposeIniManager()
        BasisIni.Dispose()
        'Spieler1Ini.Dispose()
        'Spieler2Ini.Dispose()
    End Sub

    'In den Properties muss dann nur Return BasisIni.ReadValue(... und
    'BasisIni.WriteValue(... angepasst werden, auf welche Instanz
    'zugegriffen werden soll.
    '
    'Die Verwaltung um Spieler1Ini und Spieler2Ini auszutauschen ist noch nicht geschrieben.
    '
    'Vorbemerkung:
    'Das Anlegen einer Property für jede Eigenschaft, die in die INI gespeichert wird,
    'lohnt sich meiner Erfahrung nach, da die Namen während der Programmentwicklung
    'selten so bleiben, wie sie angelegt wurden, und so die Namen in der INI automatisch
    'mit geändert werden.
    'Zudem lässt sich weitere Funktionalität gleich mit einbauen.
    'Das System hat auch Nachteile, aber für mich überwiegen die Vorteile.
    'Ich nutze das System seit es VB.Net gibt.
    '
    'WICHTIG:
    'Die Namen der Properties müssen alle mindestens einen Unterstrich haben!
    'Das vor dem (erstem) Unterstrich ist der Folder-Name, das dahinter der Key.
    '
    'Die erzeugten Dateien sind normale txt-Dateien.
    'Hat während der Programmentwicklung den Vorteil, dass Werte geändert werden können,
    'obwohl die "Einstellungen" noch nicht programmiert sind.

    'Ich nutze das Modul INI auch, um wichtige globale Werte zu speichern, die
    'nicht über das Programmende hinaus gespeichert werden müssen, oder für
    'ReadOnly Properies, deren Wert abhängig ist von anderen gespeicherten
    'Ini-Werten.

    '
    ' Jedem Wert kann ein Kommentar hinzugefügt werden, der dann in der INI
    ' erscheint. Update und Löschung der Kommentare werden automatisch aktualisiert.
    ' Der Zeilentrenner für mehrzeilige Kommentare ist die Tilde "~".
    '----------------------------------
    ' Wrapper-Properties für Basisdaten
    '----------------------------------

#Region "Kopiervorlagen"

    '--------------------------
    '--- Strings für PFADE ----
    '--------------------------
    '    Pfade haben eine eigene Vorlage, da sie eine absolut/relativ-Konvertierung
    '    durchlaufen, um die Pfade zu unterschiedlichen Computern kompatibel zu machen.
    '    (Weitergeben der Dateien unterhalb des Programmverzeichnis, des Dokumenten-
    '    Verzeichnisses und der INI-Datei)
    '    Bitte weiterlesen, es gibt noch eine komfortable Alternative.

    ' ''' <summary>
    ' ''' Hinweis 1: Vor Verwendung prüfen, ob nicht AppDataFullPath verwendet werden kann.
    ' ''' Vorteil: Die Enumerationen  AppDataSubDir, AppDataSubSubDir, AppDataFileName
    ' ''' und AppDataFilePattern können verwendet werden. Es ist ein OpenFileDialog und andere
    ' ''' Funktionen integriert.
    ' ''' Hinweis 2: Die Pfade werden als relative Pfade gespeichert, sofern sie sich unterhalb
    ' ''' Environment.SpecialFolder.MyDocuments oder unterhalb des Programmverzeichnisses
    ' ''' befinden. (Ein Verzeichnis über MyDocuments und dort das Verzeichnis MahjongGK)
    ' ''' </summary>
    ' ''' <returns>Default: Environment.SpecialFolder.MyDocuments </returns>
    ' 'Public Property Kopier_VorlageFürPfade As String
    '    Get
    '        Dim [Default] As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadPath(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As String)
    '        BasisIni.WritePath(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '    End Set
    'End Property

    '---------------------------
    '--- für normale STRINGS ---
    '---------------------------

    'Public Property Kopier_Vorlage As String
    '    Get
    '        Dim [Default] As String = Nothing
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As String)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '    End Set
    'End Property

    ''Für zeitkritische häufige Abfragen
    'Private _Kopier_Vorlage As String = Nothing
    'Private _Kopier_Vorlage_Loaded As Boolean = False

    'Public Property Kopier_Vorlage As String
    '    Get
    '        If Not _Kopier_Vorlage_Loaded Then
    '            Dim [Default] As String = "" ' oder Nothing oder Wert
    '            Dim comment As String = Nothing
    '            _Kopier_Vorlage = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '            _Kopier_Vorlage_Loaded = True
    '        End If
    '        Return _Kopier_Vorlage
    '    End Get
    '    Set(value As String)
    '        BasisIni.WriteValue(
    '        FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name),
    '        value)
    '        ' Cache sofort aktualisieren, kein Re-Read:
    '        _Kopier_Vorlage = value
    '        _Kopier_Vorlage_Loaded = True
    '    End Set
    'End Property
    '
    '------------
    '--- CHAR ---
    '------------

    'Public Property Kopier_Vorlage As Char
    '    Get
    '        Dim [Default] As Char = ControlChars.NullChar
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Char)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '    End Set
    'End Property

    '--- Boolean ----

    'Public Property Kopier_Vorlage As Boolean
    '    Get
    '        Dim [Default] As Boolean = False
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Boolean)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
    '    End Set
    'End Property

    ''Für zeitkritische häufige Abfragen
    'Private _Kopier_Vorlage As Boolean?
    'Public Property Kopier_Vorlage As Boolean
    '    Get
    '        If IsNothing(_Kopier_Vorlage) Then
    '            Dim [Default] As Boolean = False
    '            Dim comment As String = Nothing
    '            _Kopier_Vorlage = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        End If
    '        Return CBool(_Kopier_Vorlage)
    '    End Get
    '    Set(value As Boolean)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
    '        _Kopier_Vorlage = Nothing
    '    End Set
    'End Property

    '---------------
    '--- INTEGER ---
    '---------------

    '    Public Property Kopier_Vorlage As Integer
    '        Get
    '            Dim [Default] As Integer = 0
    '            Dim comment As String = Nothing
    '            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        End Get
    '        Set(value As Integer)
    '            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
    '        End Set
    '    End Property

    ''für zeitkritische Abfragen
    'Private _Kopier_Vorlage As Integer?
    'Public Property Kopier_Vorlage As Integer
    '    Get
    '        If Not _Kopier_Vorlage.HasValue Then
    '            Dim [Default] As Integer = 0
    '            Dim comment As String = Nothing
    '            _Kopier_Vorlage = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        End If
    '        Return _Kopier_Vorlage.Value
    '    End Get
    '    Set(value As Integer)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '        _Kopier_Vorlage = Nothing
    '    End Set
    'End Property
    '
    '------------
    '--- LONG ---
    '------------
    '
    'Public Property Kopier_Vorlage As Long
    '    Get
    '        Dim [Default] As Long = 0
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Long)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
    '    End Set
    'End Property
    '
    'für zeitkritische Abfragen
    'Private _Kopier_Vorlage_Long As Long?
    'Public Property Kopier_Vorlage As Long
    '    Get
    '        If Not _Kopier_Vorlage_Long.HasValue Then
    '            Dim [Default] As Long = 0
    '            Dim comment As String = Nothing
    '            _Kopier_Vorlage_Long = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        End If
    '        Return _Kopier_Vorlage_Long.Value
    '    End Get
    '    Set(value As Long)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '        _Kopier_Vorlage_Long = Nothing
    '    End Set
    'End Property
    '
    '--------------
    '--- SINGLE ---
    '--------------
    '
    'Public Property Kopier_Vorlage As Single
    '    Get
    '        Dim [Default] As Single = 0
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Single)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
    '    End Set
    'End Property

    'Private _Kopier_Vorlage As Single?
    'Public Property Kopier_Vorlage As Single
    '    Get
    '        If Not _Kopier_Vorlage.HasValue Then
    '            Dim [Default] As Single = 0
    '            Dim comment As String = Nothing
    '            _Kopier_Vorlage = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        End If
    '        Return _Kopier_Vorlage.Value
    '    End Get
    '    Set(value As Single)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '        _Kopier_Vorlage = Nothing
    '    End Set
    'End Property
    '
    '--------------
    '--- DOUBLE ---
    '--------------
    '
    'Public Property Kopier_Vorlage As Double
    '    Get
    '        Dim [Default] As Double = 0
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Double)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
    '    End Set
    'End Property

    'Private _Kopier_Vorlage As Double?
    'Public Property Kopier_Vorlage As Double
    '    Get
    '        If Not _Kopier_Vorlage.HasValue Then
    '            Dim [Default] As Double = 0
    '            Dim comment As String = Nothing
    '            _Kopier_Vorlage = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        End If
    '        Return _Kopier_Vorlage.Value
    '    End Get
    '    Set(value As Double)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '        _Kopier_Vorlage = Nothing
    '    End Set
    'End Property
    '
    '---------------
    '--- DECIMAL ---
    '---------------
    '
    'Public Property Kopier_Vorlage As Decimal
    '    Get
    '        Dim [Default] As Decimal = 0
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Decimal)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
    '    End Set
    'End Property

    'Private _Kopier_Vorlage As Decimal?
    'Public Property Kopier_Vorlage As Decimal
    '    Get
    '        If Not _Kopier_Vorlage.HasValue Then
    '            Dim [Default] As Decimal = 0
    '            Dim comment As String = Nothing
    '            _Kopier_Vorlage = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        End If
    '        Return _Kopier_Vorlage.Value
    '    End Get
    '    Set(value As Decimal)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '        _Kopier_Vorlage = Nothing
    '    End Set
    'End Property
    '
    '------------
    '--- DATE ---
    '------------

    'Public Property Kopier_Vorlage As Date
    '    Get
    '        Dim [Default] As Date = Date.MinValue
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Date)
    '        WriteDate(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '    End Set
    'End Property
    '
    '-------------
    '--- COLOR ---
    '-------------
    '
    ''für zeitkritische Abfragen
    'Private _Kopier_Vorlage As Color
    'Public Property Kopier_Vorlage As Color
    '    Get
    '        If _Kopier_Vorlage.IsEmpty Then
    '            Dim [Default] As Color = Color.Black
    '            'alternativ
    '            'Dim [Default] As Color = IniManager.CvtHexStringToColor("FF000000")
    '            Dim comment As String = Nothing
    '            _Kopier_Vorlage = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        End If
    '        Return _Kopier_Vorlage
    '    End Get
    '    Set(value As Color)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '        _Kopier_Vorlage = Color.Empty
    '    End Set
    'End Property
    '
    ''für nicht zeitkritische Abfragen
    'Public Property Kopier_Vorlage2 As Color
    '    Get
    '        Dim [Default] As Color = Color.Black
    '        'alternativ
    '        'Dim [Default] As Color = IniManager.CvtHexStringToColor("FF000000")
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Color)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '    End Set
    'End Property
    '
    '--------------------
    '--- POINT POINTF ---
    '--------------------
    '
    'Public Property Kopier_Vorlage As Point 'Für PointF Point durch PontF ersetzten
    '    Get
    '        Dim [Default] As New Point(100, 100)
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Point)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '    End Set
    'End Property
    '
    '------------------
    '--- SIZE SIZEF ---
    '------------------
    '
    'Public Property Kopier_Vorlage As Size 'Für SizeF Size durch SizeF ersetzten.
    '    Get
    '        Dim [Default] As New Size(100,100)
    '        Dim comment As String = Nothing
    '        return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Size)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '    End Set
    'End Property
    '
    '----------------------------
    '--- RECTANGLE RECTANGLEF ---
    '----------------------------
    '
    'Public Property Kopier_Vorlage As Rectangle 'Für RectangleF Rectangle durch RectangleF ersetzten.
    '    Get
    '        Dim [Default] As New Rectangle(0,0,100,100)
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Rectangle)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '    End Set
    'End Property
    '
    '------------
    '--- FONT ---
    '------------
    '
    'Public Property Kopier_Vorlage As Font
    '    Get
    '        Dim [Default] As New Font("Arial", 8.25F, FontStyle.Regular)
    '        Dim comment As String = Nothing
    '        Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '    End Get
    '    Set(value As Font)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
    '    End Set
    'End Property
    '
    '------------
    '--- ENUM ---
    '------------
    '
    'Enumeration
    '' Die eENUM durch den Namen der Enumeration ersetzten.
    '' eENUM ist der Name der Enumeration (Kommt vier mal vor)
    '' eENUM.DEFAULT eben der Default (kommt einmal vor)
    ''
    'Public Property Kopier_Vorlage As eENUM
    '    Get
    '        Dim [Default] As String = eENUM.DEFAULT.ToString
    '        Dim comment As String = Nothing
    '        Dim zRetVal As String = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
    '        Dim aRetVal As MySendKeyEnum.eSendKeyDstApp = CType(System.Enum.Parse(aRetVal.GetType(), zRetVal), eENUM)
    '        Return aRetVal
    '    End Get
    '    Set(value As eENUM)
    '        BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name, value.ToString)
    '    End Set
    'End Property

#End Region

    'Hinweis:
    'Die Reihenfolge beachten.
    'Aus ihr ergibt sich die Reihenfolge der Werte in der INI.
    '
    ''' <summary>
    ''' Dies ist die erste Property und damit der erste Eintrag
    ''' </summary>
    ''' <returns></returns>
    Public Property Hinweis_Bitte_lesen As String
        Get
            Dim [Default] As String = "Ende des Hinweis."
            Dim comment As String = "Sie können hier Programmeinstellungen direkt ändern, bzw. Einstellungen ändern, die sich sonst nirgens ändern lassen." &
                                    "~Sorgfältig arbeiten! Bei Fehlern arbeitet das Programm unvorhersehbar. Nicht bei laufendem Programm ändern." &
                                    "~Sie können diese Ini-Datei einfach löschen. Sie wird dann mit Default-Werten neu erzeugt."

            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As String)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property


    Private _IfRunningInIDE_ShowAllStones As Boolean?
    ''' <summary>
    ''' Gibt außerhalb der IDE immer False zurück
    ''' </summary>
    ''' <returns></returns>
    Public Property IfRunningInIDE_ShowAllStones As Boolean
        Get

            If IsNothing(_IfRunningInIDE_ShowAllStones) Then
                Dim [Default] As Boolean = False
                Dim comment As String = Nothing
                _IfRunningInIDE_ShowAllStones = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            'musss hier stehen, sonst wird der Wert nicht initialisiert und kann in der INI manuell nicht geändert werden.
            If Not Debugger.IsAttached() Then
                Return False
            End If

            Return CBool(_IfRunningInIDE_ShowAllStones)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _IfRunningInIDE_ShowAllStones = Nothing
        End Set
    End Property

    Private _IfRunningInIDE_InsertStoneIndex As Boolean?
    ''' <summary>
    ''' Gibt außerhalb der IDE immer False zurück
    ''' </summary>
    ''' <returns></returns>
    Public Property IfRunningInIDE_InsertStoneIndex As Boolean
        Get
            If IsNothing(_IfRunningInIDE_InsertStoneIndex) Then
                Dim [Default] As Boolean = False
                Dim comment As String = Nothing
                _IfRunningInIDE_InsertStoneIndex = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If

            'muss hier stehen, sonst wird der Wert nicht initialisiert und kann in der INI manuell nicht geändert werden.
            If Not Debugger.IsAttached() Then
                Return False
            End If

            Return CBool(_IfRunningInIDE_InsertStoneIndex)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _IfRunningInIDE_InsertStoneIndex = Nothing
        End Set
    End Property

    Public Property IfRunningInIDE_ShowErrorMsgInsteadOfException As Boolean
        Get
            Dim [Default] As Boolean = False
            Dim comment As String = Nothing
            Dim retval As Boolean = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            'muss hier stehen, sonst wird der Wert nicht initialisiert und kann in der INI manuell nicht geändert werden.
            If Not Debugger.IsAttached() Then
                Return False
            Else
                Return retval
            End If
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    ''' <summary>
    ''' Gibt an, ob das Spiel automatisch gespeichert werden soll.
    ''' </summary>
    ''' <returns>True, wenn AutoSave aktiv ist.</returns>
    ''' <remarks>Standardmäßig ist AutoSave aktiv.</remarks>
    Public Property Spiel_AutoSave As Boolean
        Get
            Dim [Default] As Boolean = True
            Dim comment As String = ""
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    ''' <summary>
    ''' Gibt an, ob der Anwender den Editor verwenden darf.
    ''' </summary>
    ''' <returns>True, wenn der Editor den Editor verwenden darf.</returns>
    ''' <remarks>Standardmäßig ist der Editor aktiv.</remarks>
    Public Property Editor_UsingEditorAllowed As Boolean
        Get
            Dim [Default] As Boolean = True
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    Public Property Editor_VerhältnisNormalsteineZuSondersteine As Double
        Get
            Dim [Default] As Double = 17
            Dim comment As String = "Sondersteine sind die 4 Blumen und die 4 Jahreszeiten." &
                                    "~Hiermit wird gesteuert, auf wieviel Normalsteinpaare ein Sondersteinpaar kommt." &
                                    "~Sollen alle Steine gleichhäufig vorkommen, ist der Wert 17, sollen die Sondersteine" &
                                    "~nur halb so häufig vorkommen (wie im Spiel mit 144 Steinen) ist der Wert 34. Default: 17"
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Double)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property



    Public Property Editor_VorratNoSortAreaEndIndexDefault As Integer
        Get
            Dim [Default] As Integer = 9
            Dim comment As String = "In Edtor läßt sich die Vorratskiste jederzeit neu mischen." &
                                    "~Davon ausgenommen sind die Steine bis zum hier angegebenem Index." &
                                    "~Default: 9 (=10 Steine), abschalten mit -1"
            'Rückgabe 
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property


    ' Die eENUM durch den Namen der Enumeration ersetzten.
    ' eENUM ist der Name der Enumeration (Kommt vier mal vor)
    ' eENUM.DEFAULT eben der Default (kommt einmal vor)
    '
    Public Property Kopier_GeneratorModusDefault As GeneratorModi
        Get
            Dim [Default] As String = GeneratorModi.StoneSet_144.ToString
            Dim comment As String = Nothing
            Dim zRetVal As String = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            Dim aRetVal As GeneratorModi = CType(System.Enum.Parse(aRetVal.GetType(), zRetVal), GeneratorModi)
            Return aRetVal
        End Get
        Set(value As GeneratorModi)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property


    Public Property Editor_VorratMaxUBoundDefault As Integer
        Get
            Dim [Default] As Integer = MJ_STEINE_VORRATMAXDEFAULT
            Dim comment As String = "Die Anzahl der Steine in der Vorratskiste die ""pro Portion"" erzeugt werden." &
                                    "~Default: " & MJ_STEINE_VORRATMAXDEFAULT.ToString
            'Rückgabe 
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property Editor_VorratNachschubschwelleDefault As Integer
        Get
            Dim [Default] As Integer = MJ_STEINE_VORRATNACHSCHUBSCHWELLEDEFAULT
            Dim comment As String = "Unterschreitet die Anzahl der Steine in der Vorratskiste diesen Wert," &
                                    "~wird Nachschub erzeugt. Default: " & MJ_STEINE_VORRATNACHSCHUBSCHWELLEDEFAULT.ToString

            'Rückgabe 
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

    Public Property Editor_SteinGeneratorDebugMode As Integer
        Get
            Dim [Default] As Integer = 0
            Dim comment As String = "Mit einer bliebigen Zahl <> 0 erzeugt der SteinGenerator immer wieder die gleichen" &
                                    "~Steinfolgen. Zum Austesten gedacht. Default = 0 (normaler Spielbetrieb)"
            'Rückgabe 
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

#Region "Spielfeld"


    Private _Spielfeld_BitmapHighQuality As Boolean?
    Public Property Spielfeld_BitmapHighQuality As Boolean
        Get
            If IsNothing(_Spielfeld_BitmapHighQuality) Then
                Dim [Default] As Boolean = False
                Dim comment As String = Nothing
                _Spielfeld_BitmapHighQuality = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_Spielfeld_BitmapHighQuality)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielfeld_BitmapHighQuality = Nothing
        End Set
    End Property

    Public ReadOnly Property Spielfeld_InterpolationMode As Drawing2D.InterpolationMode
        Get
            If IsNothing(_Spielfeld_BitmapHighQuality) Then
                'Initialisieren
                Dim dummy As Boolean = Spielfeld_BitmapHighQuality
            End If

            If _Spielfeld_BitmapHighQuality Then
                Return Drawing2D.InterpolationMode.HighQualityBicubic
            Else
                Return Drawing2D.InterpolationMode.HighQualityBilinear
            End If
        End Get
    End Property

    Private _Spielfeld_DrawBackgroundBitmap As Boolean?
    Public Property Spielfeld_DrawBackgroundBitmap As Boolean
        Get
            If IsNothing(_Spielfeld_DrawBackgroundBitmap) Then
                Dim [Default] As Boolean = False
                Dim comment As String = Nothing
                _Spielfeld_DrawBackgroundBitmap = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_Spielfeld_DrawBackgroundBitmap)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielfeld_DrawBackgroundBitmap = Nothing
        End Set
    End Property

    Private _Spielfeld_BackgroundBitmapIndex As Integer?
    Public Property Spielfeld_BackgroundBitmapIndex As Integer
        Get
            If Not _Spielfeld_BackgroundBitmapIndex.HasValue Then
                Dim [Default] As Integer = 0
                Dim comment As String = Nothing
                _Spielfeld_BackgroundBitmapIndex = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Spielfeld_BackgroundBitmapIndex.Value
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielfeld_BackgroundBitmapIndex = Nothing
        End Set
    End Property

    Private _Spielfeld_DrawBackgroundColor As Boolean?
    Public Property Spielfeld_DrawBackgroundColor As Boolean
        Get
            If IsNothing(_Spielfeld_DrawBackgroundColor) Then
                Dim [Default] As Boolean = True
                Dim comment As String = Nothing
                _Spielfeld_DrawBackgroundColor = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_Spielfeld_DrawBackgroundColor)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielfeld_DrawBackgroundColor = Nothing
        End Set
    End Property

    Private _Spielfeld_BackgroundColor As Color
    Public Property Spielfeld_BackgroundColor As Color
        Get
            If _Spielfeld_BackgroundColor.IsEmpty Then
                Dim [Default] As Color = IniManager.CvtHexStringToColor("FFC0C0C0")
                Dim comment As String = Nothing
                _Spielfeld_BackgroundColor = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Spielfeld_BackgroundColor
        End Get
        Set(value As Color)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielfeld_BackgroundColor = Color.Empty
        End Set
    End Property


    Private _Spielfeld_DrawFraming As Boolean?
    Public Property Spielfeld_DrawFraming As Boolean
        Get
            If IsNothing(_Spielfeld_DrawFraming) Then
                Dim [Default] As Boolean = True
                Dim comment As String = Nothing
                _Spielfeld_DrawFraming = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_Spielfeld_DrawFraming)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielfeld_DrawFraming = Nothing
        End Set
    End Property

    Private _Spielfeld_FramingColor As Color
    Public Property Spielfeld_FramingColor As Color
        Get
            If _Spielfeld_FramingColor.IsEmpty Then
                Dim [Default] As Color = Color.Black
                Dim comment As String = Nothing
                _Spielfeld_FramingColor = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Spielfeld_FramingColor
        End Get
        Set(value As Color)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielfeld_FramingColor = Color.Empty
        End Set
    End Property

    Private _Spielfeld_FramingThickness As Single?
    Public Property Spielfeld_FramingThickness As Single
        Get
            If Not _Spielfeld_FramingThickness.HasValue Then
                Dim [Default] As Single = 2
                Dim comment As String = Nothing
                _Spielfeld_FramingThickness = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Spielfeld_FramingThickness.Value
        End Get
        Set(value As Single)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielfeld_FramingThickness = Nothing
        End Set
    End Property

    Private _Spielfeld_DrawRenderRect As Boolean?
    Public Property Spielfeld_DrawRenderRect As Boolean
        Get
            If IsNothing(_Spielfeld_DrawRenderRect) Then
                Dim [Default] As Boolean = Debugger.IsAttached
                Dim comment As String = Nothing
                _Spielfeld_DrawRenderRect = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return CBool(_Spielfeld_DrawRenderRect)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielfeld_DrawRenderRect = Nothing
        End Set
    End Property

    Private _Spielfeld_RenderRectColor As Color
    Public Property Spielfeld_RenderRectColor As Color
        Get
            If _Spielfeld_RenderRectColor.IsEmpty Then
                Dim [Default] As Color = Color.Black
                Dim comment As String = Nothing
                _Spielfeld_RenderRectColor = BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
            End If
            Return _Spielfeld_RenderRectColor
        End Get
        Set(value As Color)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
            _Spielfeld_RenderRectColor = Color.Empty
        End Set
    End Property

    Public Property Spielfeld_RenderTimerInterval As Integer
        Get
            Dim [Default] As Integer = 15
            Dim comment As String = "Normal 15 bis 20 (Einheit Millisekunden). Werte über 30 für schwache Rechner," &
                                    "~= 1 führt zu einem stabilerem Takt aller Timer auf dem Computer und zu" &
                                    "~etwas höherem Energieverbrauch." &
                                    "~Zu hohe Werte verlangsamen und verlängern die Animation."
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Integer)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

#Region "Regeln"

    Public Property Regeln_WindsAreInOneClickGroup As Boolean
        Get
            Dim [Default] As Boolean = False
            Dim comment As String = "Wenn True ist das eine starke Spielregelvereinfachung:" &
                                    "~Die 4 Winde können in beliebiger Kombination paarweise entnommen werden."
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Boolean)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value.ToString)
        End Set
    End Property

#End Region

#Region "InfoMessageBox"

    Public Property InfoMessageBox_FontHeader As Font
        Get
            Dim [Default] As New Font("Segoe UI", 12.0F, FontStyle.Bold)
            Dim comment As String = "Andere serifenlose Standardschriften: Arial, Segoe UI, Calibri, Tahoma, Verdana, sans-serif. Default Segoe UI;12;Bold"
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Font)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    Public Property InfoMessageBox_FontMessage As Font
        Get
            Dim [Default] As New Font("Segoe UI", 12.0F, FontStyle.Regular)
            Dim comment As String = "Andere serifenlose Standardschriften: Arial, Segoe UI, Calibri, Tahoma, Verdana, sans-serif. Default Segoe UI;12;Regular"
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Font)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    Public Property InfoMessageBox_FontHeaderMonoSpaced As Font
        Get
            Dim [Default] As New Font("Cascadia Mono", 12.0F, FontStyle.Bold)
            Dim comment As String = "Andere diktengleiche Fonts: Cascadia Mono, Consolas, Lucida Console, Courier New, monospace. Default: Cascadia Mono;12;Bold"
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Font)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    Public Property InfoMessageBox_FontMessageMonoSpaced As Font
        Get
            Dim [Default] As New Font("Cascadia Mono", 12.0F, FontStyle.Regular)
            Dim comment As String = "Andere diktengleiche Fonts: Cascadia Mono, Consolas, Lucida Console, Courier New, monospace. Default: Cascadia Mono;12;Regular"
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Font)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

    Public Property InfoMessageBox_FormBackColor As Color
        Get
            Dim [Default] As Color = Color.Silver
            'alternativ
            'Dim [Default] As Color = IniManager.CvtHexStringToColor("FF000000")
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Color)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property
    Public Property InfoMessageBox_TextBackColor As Color
        Get
            Dim [Default] As Color = Color.Beige
            'alternativ
            'Dim [Default] As Color = IniManager.CvtHexStringToColor("FF000000")
            Dim comment As String = Nothing
            Return BasisIni.ReadValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), [Default], comment)
        End Get
        Set(value As Color)
            BasisIni.WriteValue(FolderAndKeyFrom(MethodBase.GetCurrentMethod().Name), value)
        End Set
    End Property

#End Region


#End Region

#Region "AppDataFullPath-Funktionen"

    '
    ' Hier ist die vom Programm genutzte Logik zur Bereitstellung von Pfaden
    ' mit und ohne Dateinamen angesiedelt.
    '
    ' Der Vorteil meines Ansatzes:
    ' Alle internen Zugriffe auf Dateien arbeiten immer mit absoluten Pfaden ==> kein Risiko,
    ' dass irgendwo ein falscher relativer Pfad verwendet wird.
    ' INI-Dateien speichern nur relativ ==> Portabilität zwischen Rechnern ist gegeben.
    ' Alle Daten im Verzeichnis "C:\Users\aktueller Anwender\MahjongGK"
    ' und alle vom User im Verzeichnis  "C:\Users\aktueller Anwender\Dokumente\???"
    ' gespeicherten Daten können portiert werden.

    ' Die Konvertierungslogik ist klar getrennt von den normalen Pfadzugriffen

    ' Hinweis:
    ' Hier werden die Enumerationen AppDataSubDir und AppDataSubSubDir verwendet,
    ' sie sind im Modul INI
    ' Aufbau:
    ' ''' <summary>
    ' ''' Enumeration der verwendeten Unterverzeichnisse in "C:\Users\aktueller User\MahjongGK\SubDefault.value.ToString"
    ' ''' Verwendung: Entweder über Dim Path As String = INI.AppDataDefault(.....)
    ' ''' oder durch Nutzung (im Modul INI) einer Public Property Kopier_VorlageFürPfade As String
    ' ''' Die gewünschten Pfade werden automatisch angelegt.
    ' ''' </summary>
    ' Public Enum AppDataSubDir
    '     None
    '     ...
    '     ...
    ' End Enum
    ' ''' <summary>
    ' ''' Enumeration der verwendeten Unterverzeichnisse in "C:\Users\aktueller User\MahjongGK\SubDefault.value.ToString\SubSubDefault.value.ToString"
    ' ''' </summary>
    ' Public Enum AppDataSubSubDir
    '     None
    '     ...
    '     ...
    ' End Enum

    ''' <summary>
    ''' Montiert den kompletten Pfad aus den Enumerationen und fügt ggf. den aktuellen Zeitstempel hinzu.
    ''' Bei timestamp = AppDataTimeStamp.LookForLastTimeStamp wird nach der jüngsten Datei gesucht.
    ''' Gibt es keine Datei, wird String.Empty zurückgegeben!
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.Add und räumt
    ''' "on the Fly" auf, indem alle Dateien über maxFiles hinaus gelöscht werden.
    ''' </summary>
    Public Function AppDataFullPath(filename As AppDataFileName,
                                 Optional timestamp As AppDataTimeStamp = AppDataTimeStamp.None,
                                 Optional maxFiles As Integer = Integer.MaxValue) As String
        '
        'Ruft die gleiche Überladung in BasisIni auf.
        Return BasisIni.AppDataFullPath(filename, timestamp, maxFiles)

    End Function
    '
    ''' <summary>
    ''' Montiert den kompletten Pfad aus den Enumerationen und fügt ggf. den aktuellen Zeitstempel hinzu.
    ''' Bei timestamp = AppDataTimeStamp.LookForLastTimeStamp wird nach der jüngsten Datei gesucht.
    ''' Gibt es keine Datei, wird String.Empty zurückgegeben!
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.Add und räumt
    ''' "on the Fly" auf, indem alle Dateien über maxFiles hinaus gelöscht werden.
    ''' </summary>
    Public Function AppDataFullPath(subdir As AppDataSubDir,
                                    filename As AppDataFileName,
                                    Optional timestamp As AppDataTimeStamp = AppDataTimeStamp.None,
                                    Optional maxFiles As Integer = Integer.MaxValue) As String
        '
        'Ruft die gleiche Überladung in BasisIni auf.
        Return BasisIni.AppDataFullPath(subdir, filename, timestamp, maxFiles)

    End Function

    ''' <summary>
    ''' Montiert den kompletten Pfad aus und fügt ggf. den aktuellen Zeitstempel hinzu.
    ''' Bei timestamp = AppDataTimeStamp.LookForLastTimeStamp wird nach der jüngsten Datei gesucht.
    ''' Gibt es keine Datei, wird String.Empty zurückgegeben!
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.Add und räumt
    ''' "on the Fly" auf, indem alle Dateien über maxFiles hinaus gelöscht werden.
    ''' </summary>
    Public Function AppDataFullPath(subdir As AppDataSubDir,
                                    filename As String,
                                    Optional timestamp As AppDataTimeStamp = AppDataTimeStamp.None,
                                    Optional maxFiles As Integer = Integer.MaxValue) As String
        '


        Return BasisIni.AppDataFullPath(If(subdir <> AppDataSubDir.None, subdir.ToString, String.Empty),
            String.Empty,
            filename,
            timestamp,
            maxFiles
            )

    End Function

    '
    ''' <summary>
    ''' Montiert den kompletten Pfad aus den Enumerationen und fügt ggf. den aktuellen Zeitstempel hinzu.
    ''' Bei timestamp = AppDataTimeStamp.LookForLastTimeStamp wird nach der jüngsten Datei gesucht.
    ''' Gibt es keine Datei, wird String.Empty zurückgegeben!
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.Add und räumt
    ''' "on the Fly" auf, indem alle Dateien über maxFiles hinaus gelöscht werden.
    ''' </summary>
    Public Function AppDataFullPath(subdir As AppDataSubDir,
                                    subsubdir As AppDataSubSubDir,
                                    filename As AppDataFileName,
                                    Optional timestamp As AppDataTimeStamp = AppDataTimeStamp.None,
                                    Optional maxFiles As Integer = Integer.MaxValue) As String
        '
        'Ruft die gleiche Überladung in BasisIni auf.
        Return BasisIni.AppDataFullPath(subdir, subsubdir, filename, timestamp, maxFiles)

    End Function
    '
    ''' <summary>
    ''' Montiert den kompletten Pfad aus und fügt ggf. den aktuellen Zeitstempel hinzu.
    ''' Bei timestamp = AppDataTimeStamp.LookForLastTimeStamp wird nach der jüngsten Datei gesucht.
    ''' Gibt es keine Datei, wird String.Empty zurückgegeben!
    ''' maxFiles arbeitet nur in Verbindung mit timestamp = AppDataTimeStamp.Add und räumt
    ''' "on the Fly" auf, indem alle Dateien über maxFiles hinaus gelöscht werden.
    ''' </summary>
    Public Function AppDataFullPath(subdir As AppDataSubDir,
                                    subsubdir As AppDataSubSubDir,
                                    filename As String,
                                    Optional timestamp As AppDataTimeStamp = AppDataTimeStamp.None,
                                    Optional maxFiles As Integer = Integer.MaxValue) As String
        '
        Return BasisIni.AppDataFullPath(
            If(subdir <> AppDataSubDir.None, subdir.ToString, String.Empty),
            If(subsubdir <> AppDataSubSubDir.None, subsubdir.ToString, String.Empty),
            filename,
            timestamp,
            maxFiles
            )

    End Function
    '
    '-------------------------------
    '
    '
    Public Function AppDataFullPathWithOpenFileDialog(subdir As AppDataSubDir,
                                    pattern As AppDataFilePattern,
                                    Optional header As String = Nothing) As String
        '
        Dim path As String = BasisIni.AppDataFullPath(subdir, AppDataFileName.None, AppDataTimeStamp.None)

        Return BasisIni.GetFullpathFromSelectedFile(path, PatternFromEnum(pattern), header)

    End Function
    '
    Public Function AppDataFullPathWithOpenFileDialog(subdir As AppDataSubDir,
                                    pattern As String,
                                    Optional header As String = Nothing) As String
        '
        Dim path As String = BasisIni.AppDataFullPath(subdir, AppDataFileName.None, AppDataTimeStamp.None)

        Return BasisIni.GetFullpathFromSelectedFile(path, pattern, header)

    End Function
    '
    Public Function AppDataFullPathWithOpenFileDialog(subdir As AppDataSubDir,
        subsubdir As AppDataSubSubDir,
        pattern As AppDataFilePattern,
                                    Optional header As String = Nothing) As String
        '
        Dim path As String = BasisIni.AppDataFullPath(subdir, subsubdir, AppDataFileName.None, AppDataTimeStamp.None)

        Return BasisIni.GetFullpathFromSelectedFile(path, PatternFromEnum(pattern), header)

    End Function
    '
    Public Function AppDataFullPathWithOpenFileDialog(subdir As AppDataSubDir,
                                    subsubdir As AppDataSubSubDir,
                                    pattern As String,
                                    Optional header As String = Nothing) As String
        '
        Dim path As String = BasisIni.AppDataFullPath(subdir, subsubdir, AppDataFileName.None, AppDataTimeStamp.None)

        Return BasisIni.GetFullpathFromSelectedFile(path, pattern, header)

    End Function

    Public Function PatternFromEnum(value As AppDataFilePattern) As String
        Dim s As String = value.ToString()
        Return s.Replace("_D_", ".") _
            .Replace("_Q_", "?") _
            .Replace("_N_", "#") _
            .Replace("_S_", "*") _
            .Replace("_SD_", "*.") _
            .Replace("_SDS_", "*.*")

    End Function


#End Region

#Region "Hilfsfunktionen"
    Private Function FolderAndKeyFrom(funktionsname As String) As (folder As String, key As String)

        ' Getter-/Setter-Präfixe entfernen
        If funktionsname.StartsWith("get_") Then
            funktionsname = funktionsname.Substring(4)
        ElseIf funktionsname.StartsWith("set_") Then
            funktionsname = funktionsname.Substring(4)
        End If

        ' Position des Unterstrichs suchen
        Dim iZeiPos As Integer = funktionsname.IndexOf("_"c)

        If iZeiPos < 0 Then
            ' Kein Unterstrich -> Sonderfall
            Return ("UnknownFolderFromFunktionsname", funktionsname)
        Else
            ' Alles vor dem Unterstrich = Folder
            ' Alles nach dem Unterstrich = Key
            Dim folder As String = funktionsname.Substring(0, iZeiPos)
            Dim key As String = funktionsname.Substring(iZeiPos + 1)
            Return (folder, key)
        End If
    End Function
#End Region

#Region "Initialisierung"


    ''' <summary>
    ''' Schaltet InitialisierungAktiv für alle IniManager um.
    ''' </summary>
    Public Sub SetInitialisierungAktivForAll(value As Boolean)
        For Each m As IniManager In AllIniManagers
            m.InitialisierungAktiv = value
        Next
    End Sub

    Public Sub InitAllDefaults()

        SetInitialisierungAktivForAll(True)

        ' Hole alle öffentlichen Shared Methoden/Properties des Moduls
        Dim methods As MemberInfo() = GetType(INI).GetMembers(BindingFlags.Public Or BindingFlags.Static)

        ' Filtere nur Property-Getter oder Methoden mit Unterstrich im Namen
        Dim gefilterte As New List(Of MemberInfo)
        For Each item As MemberInfo In methods
            If item.Name.Contains("_") AndAlso
                Not item.Name.StartsWith("get_") AndAlso
                Not item.Name.StartsWith("set_") AndAlso
                (TypeOf item Is MethodInfo AndAlso
                CType(item, MethodInfo).GetParameters().Length = 0 OrElse
                TypeOf item Is PropertyInfo) Then
                gefilterte.Add(item)
            End If
        Next

        If gefilterte.Count = 0 Then
            Exit Sub

        End If

        gefilterte.OrderBy(Function(m) m.Name)

        ' Jetzt rufe sie alle auf
        For Each member As MemberInfo In gefilterte
            Try
                If TypeOf member Is PropertyInfo Then
                    Dim p As PropertyInfo = CType(member, PropertyInfo)
                    p.GetValue(Nothing, Nothing) ' Aufruf des Getters
                ElseIf TypeOf member Is MethodInfo Then
                    Dim mi As MethodInfo = CType(member, MethodInfo)
                    mi.Invoke(Nothing, Nothing)
                End If
            Catch ex As Exception
                ' Optional Logging
                Console.WriteLine("Fehler beim Aufruf von " & member.Name & ": " & ex.Message)
            End Try
        Next

        SetInitialisierungAktivForAll(False)

    End Sub

#End Region

End Module

