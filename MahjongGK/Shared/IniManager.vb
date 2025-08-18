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


Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Threading

Public Class IniManager
    Implements IDisposable


    Private ReadOnly _fileFullPath As String
    Private ReadOnly lines As New List(Of String)


    Public Sub New(fileName_ext As String)
        _fileFullPath = AppDataFileINI(fileName_ext)
        Load()
    End Sub
    '

#Region "Write/Read"

    ' Für die Standardwerte: 
    ' Decimal, Double, Single, Long, Integer,
    ' Boolean
    ' Color
    ' Size, Point, Rectangle
    ' String, Char
    ' Font
    ' gibt es passsende Überladungen, die gleich
    ' das richtige Format zurückgeben.
    ' Für alle anderen Werte sind die Konvertierungsfunktionen Cvt...
    ' zu ergänzen, die sich ganz unten am Ende des Moduls befinden,
    ' und und die Write- und Read Funktionen zu ergänzen.


#Disable Warning IDE0079 ' Unnötige Unterdrückung entfernen
#Disable Warning IDE0051 ' Nicht verwendete private Member entfernen

    '
    '--- String
    Public Sub WriteValue(folderAndKey As (folder As String, key As String), ByVal value As String)
        WriteValueToINI(folderAndKey, value)
    End Sub

    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As String) As String
        Get
            Try
                Return ReadValueFromINI(folderAndKey, [default])
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property
    '
    'Sonderfall String als Path
    '--- String
    Public Sub WritePath(folderAndKey As (folder As String, key As String), ByVal value As String)
        WriteValueToINI(folderAndKey, CvtPathToRelPath(value))
    End Sub

    Public ReadOnly Property ReadPath(folderAndKey As (folder As String, key As String), [default] As String) As String
        Get
            Try
                Return CvtRelPathToPath(ReadValueFromINI(folderAndKey, CvtPathToRelPath([default])))
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property

    '
    '--- Char
    Public Sub WriteValue(folderAndKey As (folder As String, key As String), ByVal value As Char)
        WriteValueToINI(folderAndKey, value.ToString)
    End Sub

    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Char) As Char
        Get
            Try
                Dim s As String = ReadValueFromINI(folderAndKey, [default].ToString)
                If String.IsNullOrEmpty(s) Then
                    Return [default]
                Else
                    Return s(0) 'nur das erste Zeichen
                End If
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property
    '
    '--- Decimal
    Public Sub WriteValue(folderAndKey As (folder As String, key As String), ByVal value As Decimal)
        WriteValueToINI(folderAndKey, CvtDecimalToString(value))
    End Sub
    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Decimal) As Decimal

        Get
            Try
                Return CvtStringToDecimal(ReadValueFromINI(folderAndKey, CvtDecimalToString([default])), [default])
            Catch ex As Exception
                Return [default]
            End Try

        End Get
    End Property
    '
    '--- Double
    Public Sub WriteValue(folderAndKey As (folder As String, key As String), ByVal value As Double)
        WriteValueToINI(folderAndKey, CvtDoubleToString(value))
    End Sub
    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Double) As Double

        Get
            Try
                Return CvtStringToDouble(ReadValueFromINI(folderAndKey, CvtDoubleToString([default])), [default])
            Catch ex As Exception
                Return [default]
            End Try

        End Get
    End Property
    '
    ' --- Single
    Public Sub WriteValue(folderAndKey As (folder As String, key As String), ByVal value As Single)
        WriteValueToINI(folderAndKey, CvtSingleToString(value))
    End Sub
    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Single) As Single

        Get
            Try
                Return CvtStringToSingle(ReadValueFromINI(folderAndKey, CvtSingleToString([default])), [default])
            Catch ex As Exception
                Return [default]
            End Try

        End Get
    End Property
    '
    'Long
    Public Sub WriteValue(folderAndKey As (folder As String, key As String), ByVal value As Long)
        WriteValueToINI(folderAndKey, CvtLongToString(value))
    End Sub

    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Long) As Long
        Get
            Try
                Return CvtStringToLong(ReadValueFromINI(folderAndKey, CvtLongToString([default])), [default])
            Catch ex As Exception
                Return [default]
            End Try

        End Get
    End Property
    '
    ' Integer
    Public Sub WriteValue(folderAndKey As (folder As String, key As String), ByVal value As Integer)
        WriteValueToINI(folderAndKey, CvtIntegerToString(value))
    End Sub

    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Integer) As Integer
        Get
            Try
                Return CvtStringToInteger(ReadValueFromINI(folderAndKey, CvtIntegerToString([default])), [default])
            Catch ex As Exception
                Return [default]
            End Try

        End Get
    End Property
    '
    '--- Boolean
    Public Sub WriteValue(folderAndKey As (folder As String, key As String), ByVal value As Boolean)
        WriteValueToINI(folderAndKey, CvtBooleanToString(value))
    End Sub
    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Boolean) As Boolean
        Get
            Try
                Return CvtStringToBoolean(ReadValueFromINI(folderAndKey, CvtBooleanToString([default])), [default])
            Catch ex As Exception
                Return [default]
            End Try

        End Get
    End Property

    '
    '--- Color
    Public Sub WriteValue(folderAndKey As (folder As String, key As String), ByVal value As Color)
        WriteValueToINI(folderAndKey, CvtColorToHexString(value))
    End Sub

    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Color) As Color
        Get
            Try
                Return CvtHexStringToColor(ReadValueFromINI(folderAndKey, CvtColorToHexString([default])), [default])
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property
    '
    '--- Date
    Public Sub WriteValue(folderAndKey As (folder As String, key As String), ByVal value As Date)
        WriteValueToINI(folderAndKey, CvtDateToString(value))
    End Sub

    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Date) As Date
        Get
            Try
                Return CvtStringToDate(ReadValueFromINI(folderAndKey, CvtDateToString([default])), [default])
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property
    '
    '--- Point
    Public Sub WriteValue(folderAndKey As (folder As String, key As String), value As Point)
        WriteValueToINI(folderAndKey, CvtPointToString(value))
    End Sub

    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Point) As Point
        Get
            Try
                Return CvtStringToPoint(ReadValueFromINI(folderAndKey, CvtPointToString([default])), [default])
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property
    '
    '--- Size
    Public Sub WriteValue(folderAndKey As (folder As String, key As String), value As Size)
        WriteValueToINI(folderAndKey, CvtSizeToString(value))
    End Sub

    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Size) As Size
        Get
            Try
                Return CvtStringToSize(ReadValueFromINI(folderAndKey, CvtSizeToString([default])), [default])
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property
    '
    '--- Rectangle
    Public Sub WriteValue(folderAndKey As (folder As String, key As String), value As Rectangle)
        WriteValueToINI(folderAndKey, CvtRectToString(value))
    End Sub
    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Rectangle) As Rectangle
        Get
            Try
                Return CvtStringToRect(ReadValueFromINI(folderAndKey, CvtRectToString([default])), [default])
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property
    '
    '--- Font
    Public Sub WriteValue(folderAndKey As (folder As String, key As String), value As Font)
        WriteValueToINI(folderAndKey, CvtFontToString(value))
    End Sub
    Public ReadOnly Property ReadValue(folderAndKey As (folder As String, key As String), [default] As Font) As Font
        Get
            Try
                Return CvtStringToFont(ReadValueFromINI(folderAndKey, CvtFontToString([default])), [default])
            Catch ex As Exception
                Return [default]
            End Try
        End Get
    End Property


    ''' <summary>
    ''' Liest einen Wert aus, oder legt ihn mit Default neu an.
    ''' </summary>
    Private Function ReadValueFromINI(folderAndKey As (folder As String, key As String), defaultValue As String) As String

        Dim idx As Integer = FindKeyLine(folderAndKey)
        If idx < 0 Then
            WriteValueToINI(folderAndKey, defaultValue)
            Return defaultValue
        End If

        Dim line As String = lines(idx)
        Dim pos As Integer = line.IndexOf("="c)
        If pos < 0 OrElse pos = line.Length - 1 Then
            Return ""
        End If

        Return CvtINIValueToStringValue(line.Substring(pos + 1), defaultValue)

    End Function

    ''' <summary>
    ''' Schreibt einen Wert (fügt an, falls er nicht existiert).
    ''' </summary>
    Private Sub WriteValueToINI(folderAndKey As (folder As String, key As String), value As String)

        value = CvtStringValueToINIValue(value)

        Dim folderLine As Integer = FindFolderLine(folderAndKey.folder)

        ' Folder nicht vorhanden -> neu anlegen
        If folderLine < 0 Then
            lines.Add("[" & folderAndKey.folder & "]")
            lines.Add($"{folderAndKey.key}={value}")
            MarkChanged()
            Return
        End If

        ' Schlüssel im Folder suchen
        For i As Integer = folderLine + 1 To lines.Count - 1
            Dim l As String = lines(i)

            If String.IsNullOrWhiteSpace(l) OrElse l.StartsWith(";") Then
                Continue For
            End If

            If l.StartsWith("[") Then
                ' nächster Abschnitt erreicht -> einfügen
                lines.Insert(i, $"{folderAndKey.key}={value}")
                MarkChanged()
                Return
            End If

            If l.StartsWith(folderAndKey.key & "=", StringComparison.OrdinalIgnoreCase) Then
                lines(i) = $"{folderAndKey.key}={value}"
                MarkChanged()
                Return
            End If
        Next

        ' Key nicht gefunden, also am Ende des Folders anhängen
        lines.Add($"{folderAndKey.key}={value}")
        MarkChanged()
    End Sub

#End Region

#Region "Konvertierungen"



    'In die INI werden nur einzeilige Strings geschrieben, hier sind die Konvertierungsroutinen.
    'Den "zurück"-Konvertierungen wird immer ein Default mitgegeben.
    'Das ist als Sicherungsnetz zu verstehen für den Fall, daß der Anwender in der INI herumpfuscht
    'und Werte nicht mehr lesbar sind.

    Public Function CvtSingleToString(value As Single) As String
        Return value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function CvtStringToSingle(s As String, [default] As Single) As Single
        Dim result As Single
        If Single.TryParse(s, NumberStyles.Float Or NumberStyles.AllowThousands,
                           CultureInfo.InvariantCulture, result) Then
            Return result
        End If
        Return [default]
    End Function

    Public Function CvtDoubleToString(value As Double) As String
        Return value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function CvtStringToDouble(s As String, [default] As Double) As Double
        Dim result As Double
        If Double.TryParse(s, NumberStyles.Float Or NumberStyles.AllowThousands,
                           CultureInfo.InvariantCulture, result) Then
            Return result
        End If
        Return [default]
    End Function

    Public Function CvtDecimalToString(value As Decimal) As String
        Return value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function CvtStringToDecimal(s As String, [default] As Decimal) As Decimal
        Dim result As Decimal
        If Decimal.TryParse(s, NumberStyles.Float Or NumberStyles.AllowThousands,
                            CultureInfo.InvariantCulture, result) Then
            Return result
        End If
        Return [default]
    End Function

    Public Function CvtIntegerToString(value As Integer) As String
        Return value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function CvtStringToInteger(s As String, [default] As Integer) As Integer
        Dim result As Integer
        If Integer.TryParse(s, NumberStyles.Integer,
                            CultureInfo.InvariantCulture, result) Then
            Return result
        End If
        Return [default]
    End Function

    Public Function CvtLongToString(value As Long) As String
        Return value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function CvtStringToLong(s As String, [default] As Long) As Long
        Dim result As Long
        If Long.TryParse(s, NumberStyles.Integer,
                         CultureInfo.InvariantCulture, result) Then
            Return result
        End If
        Return [default]
    End Function

    '
    ' === Boolean ===
    ' Speicherung als "True"/"False" (invariant)
    '
    Public Function CvtBooleanToString(value As Boolean) As String
        Return value.ToString(CultureInfo.InvariantCulture)
    End Function

    Public Function CvtStringToBoolean(s As String, [default] As Boolean) As Boolean
        Dim result As Boolean
        If Boolean.TryParse(s, result) Then
            Return result
        End If
        Return [default]
    End Function

    ''' <summary>
    ''' Wandelt eine Color in einen Hex-String nach dem Muster AARRGGBB um.
    ''' </summary>
    Public Shared Function CvtColorToHexString(color As Color) As String
        Return color.A.ToString("X2") &
               color.R.ToString("X2") &
               color.G.ToString("X2") &
               color.B.ToString("X2")
    End Function

    ''' <summary>
    ''' Wandelt einen Hex-String (AARRGGBB) zurück in eine Color.
    ''' </summary>
    Public Function CvtHexStringToColor(hex As String, [default] As Color) As Color
        Try
            Dim val As Integer = Convert.ToInt32(hex, 16)
            Return Color.FromArgb((val >> 24) And &HFF,
                          (val >> 16) And &HFF,
                          (val >> 8) And &HFF,
                          val And &HFF)
        Catch ex As Exception
            Return [default]
        End Try
    End Function
    '
    ''' <summary>
    ''' Überladung ohne Default. (nötig, um den Default festzulegen)
    ''' </summary>
    ''' <param name="hex"></param>
    ''' <returns></returns>
    Public Shared Function CvtHexStringToColor(hex As String) As Color
        Try
            Dim val As Integer = Convert.ToInt32(hex, 16)
            Return Color.FromArgb((val >> 24) And &HFF,
                          (val >> 16) And &HFF,
                          (val >> 8) And &HFF,
                          val And &HFF)
        Catch ex As Exception
            Throw New Exception($"Vermutlicher Programmierfehler: ""{hex}"" kann nicht in Color konvertiert werden in INI.CvtHexStringToColor")
        End Try
    End Function
    ''' <summary>
    ''' Wandelt ein Date in einen kulturunabhängigen String um (ISO 8601).
    ''' </summary>
    Public Function CvtDateToString(d As Date) As String
        ' "s" = Sortable DateTime Pattern: yyyy-MM-ddTHH:mm:ss
        Return d.ToString("s", System.Globalization.CultureInfo.InvariantCulture)
    End Function

    ''' <summary>
    ''' Wandelt einen String (wie von DateToString) wieder in ein Date um.
    ''' </summary>
    Public Function CvtStringToDate(s As String, [default] As Date) As Date
        Try
            Return Date.ParseExact(s, "s", System.Globalization.CultureInfo.InvariantCulture)
        Catch ex As Exception
            Return [default]
        End Try
    End Function

    ''' <summary>
    ''' Alternative Speicherung über Ticks (exakt, auch für Millisekunden)
    ''' </summary>
    Public Function CvtDateToTicksString(d As Date) As String
        Return d.Ticks.ToString()
    End Function

    ''' <summary>
    ''' Lädt ein Date aus dem Ticks-String.
    ''' </summary>
    Public Function CvtTicksStringToDate(s As String) As Date
        Return New Date(Long.Parse(s))
    End Function

    ' ----------------- Point -----------------
    Public Function CvtPointToString(p As Point) As String
        Return $"{p.X},{p.Y}"
    End Function

    Public Function CvtStringToPoint(s As String, [default] As Point) As Point
        Try
            Dim parts() As String = s.Split(","c)
            Return New Point(CInt(parts(0)), CInt(parts(1)))
        Catch ex As Exception
            Return [default]
        End Try
    End Function

    ' ----------------- Size -----------------
    Public Function CvtSizeToString(sz As Size) As String
        Return $"{sz.Width},{sz.Height}"
    End Function

    Public Function CvtStringToSize(s As String, [default] As Size) As Size
        Try
            Dim parts() As String = s.Split(","c)
            Return New Size(CInt(parts(0)), CInt(parts(1)))
        Catch ex As Exception
            Return [default]
        End Try
    End Function

    ' ----------------- Rectangle -----------------
    Public Function CvtRectToString(r As Rectangle) As String
        Return $"{r.X},{r.Y},{r.Width},{r.Height}"
    End Function

    Public Function CvtStringToRect(s As String, [default] As Rectangle) As Rectangle
        Try
            Dim parts() As String = s.Split(","c)
            Return New Rectangle(CInt(parts(0)), CInt(parts(1)), CInt(parts(2)), CInt(parts(3)))
        Catch ex As Exception
            Return [default]
        End Try
    End Function

    ' ----------------- Font -----------------
    Public Function CvtFontToString(f As Font) As String
        ' Beispiel: "Arial,12,Regular"
        Return $"{f.Name},{f.Size},{f.Style}"
    End Function

    Public Function CvtStringToFont(s As String, defaultFont As Font) As Font
        Try
            If String.IsNullOrWhiteSpace(s) Then
                Return defaultFont
            End If

            Dim parts() As String = s.Split(","c)
            If parts.Length < 3 Then
                Return defaultFont
            End If

            Dim name As String = parts(0).Trim()
            Dim size As Single
            Dim style As FontStyle

            If Not Single.TryParse(parts(1).Trim(), Globalization.NumberStyles.Float,
                                   Globalization.CultureInfo.InvariantCulture, size) Then
                Return defaultFont
            End If

            If Not [Enum].TryParse(parts(2).Trim(), True, style) Then
                Return defaultFont
            End If

            Return New Font(name, size, style)

        Catch
            ' Falls wirklich irgendetwas knallt -> Default zurück
            Return defaultFont
        End Try
    End Function


#End Region

#Region "String"

    ' ---- Helfer ----

    ' Darf roh (unquoted) geschrieben werden?
    Private Function IsSafePlain(s As String) As Boolean
        If s Is Nothing OrElse s.Length = 0 Then Return True
        If s.StartsWith(" "c) OrElse s.EndsWith(" "c) Then Return False
        For Each ch As Char In s
            Dim code As Integer = AscW(ch)
            If ch = "\"c OrElse ch = """"c Then Return False
            If code < 32 Then
                Return False ' Steuerzeichen \0..\x1F
            End If
        Next
        Return True
    End Function

    ' Sichtbar-escapen: \n \r \t \\ \" \0, sonst \uXXXX für Steuerzeichen
    Private Function EscapeForIni(s As String) As String
        If s Is Nothing Then Return ""
        Dim sb As New StringBuilder(s.Length + 8)
        For Each ch As Char In s
            Select Case ch
                Case ControlChars.Lf : sb.Append("\n")
                Case ControlChars.Cr : sb.Append("\r")
                Case ControlChars.Tab : sb.Append("\t")
                Case ControlChars.NullChar : sb.Append("\0")
                Case "\"c : sb.Append("\\")
                Case """"c : sb.Append("\""")
                Case Else
                    Dim code As Integer = AscW(ch)
                    If code < 32 Then
                        sb.Append("\u").Append(code.ToString("X4", CultureInfo.InvariantCulture))
                    Else
                        sb.Append(ch)
                    End If
            End Select
        Next
        Return sb.ToString()
    End Function

    ' Unescape: versteht quoted ("...") + \n \r \t \0 \\ \" \uXXXX
    Private Function UnescapeFromIni(quotedContent As String) As String

        Dim sb As New StringBuilder(quotedContent.Length)
        Dim i As Integer = 0
        While i < quotedContent.Length
            Dim ch As Char = quotedContent(i)
            If ch <> "\"c Then
                sb.Append(ch)
                i += 1
                Continue While
            End If

            ' Escape-Sequenz
            i += 1
            If i >= quotedContent.Length Then
                ' einzelner Backslash am Ende – roh übernehmen
                sb.Append("\"c)
                Exit While
            End If

            Select Case quotedContent(i)
                Case "n"c : sb.Append(ControlChars.Lf) : i += 1
                Case "r"c : sb.Append(ControlChars.Cr) : i += 1
                Case "t"c : sb.Append(ControlChars.Tab) : i += 1
                Case "0"c : sb.Append(ControlChars.NullChar) : i += 1
                Case "\"c : sb.Append("\"c) : i += 1
                Case """"c : sb.Append(""""c) : i += 1
                Case "u"c
                    ' \uXXXX (4 Hex-Zeichen)
                    If i + 4 < quotedContent.Length Then
                        Dim hex As String = quotedContent.Substring(i + 1, 4)
                        Dim code As Integer
                        If Integer.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, code) Then
                            sb.Append(ChrW(code))
                            i += 5
                        Else
                            sb.Append("\u")
                            i += 1
                        End If
                    Else
                        sb.Append("\u")
                        i += 1
                    End If
                Case Else
                    ' unbekanntes Escape – roh übernehmen
                    sb.Append("\"c).Append(quotedContent(i))
                    i += 1
            End Select
        End While
        Return sb.ToString()
    End Function

    ' ---- Kernfunktionen ----

    Public Function CvtStringValueToINIValue(value As String) As String
        If value Is Nothing Then
            Return ""
        End If
        If IsSafePlain(value) Then
            Return value
        End If
        Return """" & EscapeForIni(value) & """"
    End Function

    Public Function CvtINIValueToStringValue(stored As String, [default] As String) As String
        If stored Is Nothing Then
            Return If([default], "")
        End If
        If stored.Length >= 2 AndAlso stored(0) = """"c AndAlso stored(stored.Length - 1) = """"c Then
            Dim inner As String = stored.Substring(1, stored.Length - 2)
            Try
                Return UnescapeFromIni(inner)
            Catch
                Return If([default], "")
            End Try
        End If
        ' Unquoted -> roh zurückgeben
        Return stored

    End Function


#End Region

#Region "Hilfsfunktionen"

    Private Function FindFolderLine(folder As String) As Integer
        Dim search As String = "[" & folder & "]"
        For i As Integer = 0 To lines.Count - 1
            If lines(i).Equals(search, StringComparison.OrdinalIgnoreCase) Then
                Return i
            End If
        Next
        Return -1
    End Function

    Private Function FindKeyLine(folderAndKey As (folder As String, key As String)) As Integer
        Dim folderLine As Integer = FindFolderLine(folderAndKey.folder)

        If folderLine < 0 Then Return -1

        For i As Integer = folderLine + 1 To lines.Count - 1
            Dim l As String = lines(i)
            If l.StartsWith("[") Then Return -1
            If l.StartsWith(folderAndKey.key & "=", StringComparison.OrdinalIgnoreCase) Then
                Return i
            End If
        Next
        Return -1
    End Function

#End Region

#Region "Pfade und Dateinamen"


    Private Const REL_PREFIX_APPROOT As String = "®"
    Private Const REL_PREFIX_USERROOT As String = "©"
    '
    ' Das ist das Verzeichnis eine Ebene unterhalb von "Dokumente" 
    ' und dort das Verzeichnis MahjongGK.
    ' Gleichzeitig ist hier der zentrale Ort im Programm, wo der
    ' Speicherpfad aller Daten festgelegt wird. (Ausgenommen:
    ' Für "speichern unter ..." wird ein Pfad vorgeschlagen aus diesem
    ' Bereich, den der Anwender aber ändern kann.")
    ' Eine Änderung hier wirkt sich auf das gesamte Programm aus.
    Public ReadOnly Property AppRoot As String
        Get
            Dim appname As String = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name
            If appname = "MahjongGK" Then
                Return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), appname)
            Else
                'In anderen Projekten als MahjongGK verwende ich:
                Return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Anwendungsdaten II", appname)
            End If
        End Get
    End Property

    '
    'Das ist das Verzeichnis "Dokumente" des aktuellen Benutzers.
    Private ReadOnly Property UserRoot As String =
                      Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)

    ' Merkt sich, ob wir schon im Notfallmodus sind
    Private _useFallback As Boolean? = Nothing  ' Nothing = noch nicht entschieden
    Private disposedValue As Boolean

    Public Function AppDataDirectory(Optional subdir As AppDataSubDir = AppDataSubDir.None,
                                     Optional subsubdir As AppDataSubSubDir = AppDataSubSubDir.None) As String

        Return AppDataDirectory(
           If(subdir <> AppDataSubDir.None, subdir.ToString, String.Empty),
           If(subsubdir <> AppDataSubSubDir.None, subsubdir.ToString, String.Empty)
           )

    End Function

    Public Function AppDataDirectory(Optional subdir As String = Nothing,
                                     Optional subsubdir As String = Nothing) As String

        Dim aktpath As String = If(_useFallback.HasValue AndAlso _useFallback.Value,
                                   GetFallbackRoot(),
                                   AppRoot)

        If Not String.IsNullOrEmpty(subdir) Then
            aktpath = Path.Combine(aktpath, subdir)
        End If
        If Not String.IsNullOrEmpty(subsubdir) Then
            aktpath = Path.Combine(aktpath, subsubdir)
        End If

        'zum testen
        'aktpath = "C:\?:\invalid\path" 'wirft "ungültiger Pfadnahme" ,

        Try
            Directory.CreateDirectory(aktpath)
        Catch ex As Exception
            If Not _useFallback.HasValue Then
                ' Benutzer fragen (nur beim ersten Mal)
                Dim msg As String =
                    "Der Speicherordner für Spieldaten konnte nicht erstellt werden:" & vbCrLf &
                    aktpath & vbCrLf & vbCrLf &
                    "Fehler: " & ex.Message & vbCrLf & vbCrLf &
                    "Soll im Notfallmodus weitergespielt werden?" & vbCrLf & vbCrLf &
                    "Die Speicherung erfolgt dann nur temporär und kann jederzeit verloren gehen. " &
                    "Andernfalls wird das Programm beendet."

                Dim result As DialogResult = MessageBox.Show(msg,
                                System.Reflection.Assembly.GetExecutingAssembly().GetName().Name _ 'Programmname
                                & " - Speicherfehler",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning,
                                MessageBoxDefaultButton.Button2)

                If result = DialogResult.Yes Then
                    _useFallback = True
                    aktpath = GetFallbackRoot()
                Else
                    Environment.Exit(1)
                End If

            ElseIf _useFallback.Value Then
                aktpath = GetFallbackRoot()
            Else
                ' Benutzer hatte "Nein" gesagt -> sofort beenden
                Environment.Exit(1)
            End If

            ' Fallback-Ordner erstellen
            Try
                Directory.CreateDirectory(aktpath)
            Catch
                MessageBox.Show("Auch der Notfall-Speicherpfad konnte nicht erstellt werden." & vbCrLf &
                                "Das Spiel wird beendet.",
                                "Kritischer Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Environment.Exit(1)
            End Try
        End Try

        Return aktpath

    End Function

    Private Function GetFallbackRoot() As String
        Return Path.Combine(Path.GetTempPath(),
                            System.Reflection.Assembly.GetExecutingAssembly().GetName().Name)
    End Function

    '
    Public ReadOnly Property AppDataFileINI(dateiname_ext As String) As String
        Get
            'Dim path As String = path.Combine(AppDataFolder, "\Allgemeine-Einstellungen.ini")
            Return Path.Combine(AppDataDirectory(AppDataSubDir.INI.ToString), dateiname_ext)
        End Get
    End Property

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

        Return AppDataFullPath(
            String.Empty,
            String.Empty,
            If(filename <> AppDataFileName.None, filename.ToString, String.Empty),
            timestamp
            )

    End Function

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

        Return AppDataFullPath(
            If(subdir <> AppDataSubDir.None, subdir.ToString, String.Empty),
            String.Empty,
            If(filename <> AppDataFileName.None, filename.ToString, String.Empty),
            timestamp
            )

    End Function

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

        Return AppDataFullPath(
            If(subdir <> AppDataSubDir.None, subdir.ToString, String.Empty),
            If(subsubdir <> AppDataSubSubDir.None, subsubdir.ToString, String.Empty),
            If(filename <> AppDataFileName.None, filename.ToString, String.Empty),
            timestamp
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
    ''' <param name="subdir"></param>
    ''' <param name="subsubdir"></param>
    ''' <param name="filename"></param>
    ''' <param name="timestamp"></param>
    ''' <returns></returns>
    Public Function AppDataFullPath(Optional subdir As String = Nothing,
                                    Optional subsubdir As String = Nothing,
                                    Optional filename As String = Nothing,
                                    Optional timestamp As AppDataTimeStamp = AppDataTimeStamp.None,
                                    Optional maxFiles As Integer = Integer.MaxValue) As String

        Dim basePath As String = AppDataDirectory(subdir, subsubdir)

        ' Kein Dateiname -> nur Verzeichnis
        If String.IsNullOrEmpty(filename) Then
            Return basePath
        End If

        'den Unterstrich in der Enumeration ändern. 
        If filename.Length > 3 AndAlso filename(filename.Length - 4) = "_"c Then
            filename = filename.Substring(0, filename.Length - 4) &
                "." &
                filename.Substring(filename.Length - 3)
        End If

        ' Normale Kombination
        Dim fullpath As String = Path.Combine(basePath, filename)

        If timestamp = AppDataTimeStamp.Add Then
            fullpath = CreateFullPathWithTimeStamp(fullpath, maxFiles)
        ElseIf timestamp = AppDataTimeStamp.LookForLastTimeStamp Then
            fullpath = GetFullpathFromLastTimeStamp(fullpath)
        End If

        Return fullpath

    End Function

    ''' <summary>
    ''' Wandelt einen absoluten Pfad in einen relativen Pfad zu _appRoot oder _userRoot um.
    ''' Liegt der Pfad außerhalb beider Wurzeln, wird der Originalpfad zurückgegeben.
    ''' Relative Pfade erhalten das Präfix ® oder ©.
    ''' </summary>
    Public Function CvtPathToRelPath(path As String) As String
        Dim absPath As String = IO.Path.GetFullPath(path)
        Dim rootApp As String = IO.Path.GetFullPath(AppRoot)
        Dim rootUser As String = IO.Path.GetFullPath(UserRoot)

        ' Prüfen auf _appRoot
        If String.Equals(absPath, rootApp, StringComparison.OrdinalIgnoreCase) OrElse
               absPath.StartsWith(rootApp & IO.Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) Then
            Dim rel As String = absPath.Substring(rootApp.Length)
            If rel.StartsWith(IO.Path.DirectorySeparatorChar) OrElse rel.StartsWith(IO.Path.AltDirectorySeparatorChar) Then
                rel = rel.Substring(1)
            End If
            Return REL_PREFIX_APPROOT & rel
        End If

        ' Prüfen auf _userRoot
        If String.Equals(absPath, rootUser, StringComparison.OrdinalIgnoreCase) OrElse
                absPath.StartsWith(rootUser & IO.Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) Then
            Dim rel As String = absPath.Substring(rootUser.Length)
            If rel.StartsWith(IO.Path.DirectorySeparatorChar) OrElse rel.StartsWith(IO.Path.AltDirectorySeparatorChar) Then
                rel = rel.Substring(1)
            End If
            Return REL_PREFIX_USERROOT & rel
        End If

        ' Pfad außerhalb der Wurzeln -> Original zurückgeben
        Return path
    End Function

    ''' <summary>
    ''' Wandelt einen Pfad aus der INI zurück in einen absoluten Pfad.
    ''' Erkennt automatisch das Präfix ® (AppRoot) oder © (UserRoot) und hängt die richtige Wurzel davor.
    ''' </summary>
    Public Function CvtRelPathToPath(relPath As String) As String
        If String.IsNullOrEmpty(relPath) Then Return AppRoot

        If relPath.StartsWith(REL_PREFIX_APPROOT) Then
            Dim rel As String = relPath.Substring(REL_PREFIX_APPROOT.Length)
            Return Path.Combine(AppRoot, rel)
        ElseIf relPath.StartsWith(REL_PREFIX_USERROOT) Then
            Dim rel As String = relPath.Substring(REL_PREFIX_USERROOT.Length)
            Return Path.Combine(UserRoot, rel)
        Else
            ' Kein Präfix -> Pfad unverändert
            Return relPath
        End If
    End Function

    ' Letzte Datei anhand Timestamp finden
    Private Function GetFullpathFromLastTimeStamp(fullPath As String) As String
        Dim files As List(Of (Datum As DateTime, FullPath As String)) = GetSortedTimeStampFiles(fullPath)
        If files.Count > 0 Then
            Return files(0).FullPath  ' Jüngste zuerst
        Else
            Return String.Empty
        End If
    End Function


    ''' <summary>
    ''' Aus dem fullPath (einschließich Dateiname) wird der Dateiname separiert und das Pattern
    ''' eines Timestamp vorangestellt und im Verzeichnis alle Dateien gesucht, die diesem
    ''' Pattern entsprechen. Anschließend wird ein Dialog aufgerufen und der Anwender kann wählen.
    ''' </summary>
    ''' <param name="fullPath"></param>
    ''' <param name="header"></param>
    ''' <returns></returns>
    Public Function GetFullpathFromSelectedTimeStamp(fullPath As String, Optional header As String = Nothing) As String

        Dim files As List(Of (Datum As DateTime, FullPath As String)) = GetSortedTimeStampFiles(fullPath)
        If files.Count = 0 Then Return String.Empty

        Using frm As New Form With {
            .Text = If(String.IsNullOrEmpty(header), "Datei mit Zeitstempel auswählen", header),
            .Width = 500,
            .Height = 300,
            .StartPosition = FormStartPosition.CenterScreen
        }
            Dim lb As New ListBox With {
            .Dock = DockStyle.Fill
        }
            Dim found As Boolean
            ' Datum/Uhrzeit + Original-Dateiname anzeigen
            For Each entry As (Datum As DateTime, FullPath As String) In files
                Dim origName As String = Path.GetFileName(entry.FullPath).Substring(20) ' skip yyyy-mm-dd-hh-mm-ss_
                lb.Items.Add($"{entry.Datum:yyyy-MM-dd HH:mm:ss}  {origName}")
                found = True
            Next

            If Not found Then
                MsgBox("Es gibt hier (noch) keine Dateien", MsgBoxStyle.Information, If(String.IsNullOrEmpty(header), "Datei mit Zeitstempel auswählen", header))
                Return String.Empty

            End If

            frm.Controls.Add(lb)

            Dim btnOK As New Button With {
            .Text = "OK",
            .Dock = DockStyle.Bottom
        }
            frm.Controls.Add(btnOK)

            AddHandler btnOK.Click, Sub() frm.DialogResult = DialogResult.OK

            If frm.ShowDialog() = DialogResult.OK AndAlso lb.SelectedIndex >= 0 Then
                Return files(lb.SelectedIndex).FullPath
            End If
        End Using

        Return String.Empty

    End Function

    ' Hilfsfunktion: Timestamp-Dateien sortiert zurückgeben
    Private Function GetSortedTimeStampFiles(fullPath As String) As List(Of (Datum As DateTime, FullPath As String))
        Dim result As New List(Of (Datum As DateTime, FullPath As String))
        If String.IsNullOrWhiteSpace(fullPath) Then Return result

        Dim dir As String = Path.GetDirectoryName(fullPath)
        Dim fileName As String = Path.GetFileName(fullPath)
        If Not Directory.Exists(dir) Then Return result

        Dim pattern As String = "????-??-??-??-??-??_" & fileName

        Dim tmp As New List(Of (Datum As DateTime, FullPath As String))

        For Each File As String In Directory.GetFiles(dir, pattern)
            Dim baseName As String = Path.GetFileName(File)
            Dim tsPart As String = baseName.Substring(0, 19) ' yyyy-MM-dd-HH-mm-ss
            Dim ts As DateTime
            If DateTime.TryParseExact(tsPart, "yyyy-MM-dd-HH-mm-ss",
                                  CultureInfo.InvariantCulture,
                                  DateTimeStyles.None, ts) Then
                tmp.Add((ts, File))
            End If
        Next

        ' Sortierung: jüngste zuerst
        result = tmp.OrderByDescending(Function(x) x.Datum).ToList()
        Return result
    End Function

    ' Neue Datei mit Timestamp erzeugen
    Private Function CreateFullPathWithTimeStamp(fullPath As String, Optional maxFiles As Integer = Integer.MaxValue) As String

        If String.IsNullOrWhiteSpace(fullPath) Then
            Return String.Empty
        End If

        Dim dir As String = Path.GetDirectoryName(fullPath)
        Dim fileName As String = Path.GetFileName(fullPath)
        Dim timeStamp As String = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")
        Dim newFilePath As String = Path.Combine(dir, $"{timeStamp}_{fileName}")

        ' Optionales Aufräumen
        If maxFiles > 0 Then
            Dim files As List(Of (Datum As DateTime, FullPath As String)) = GetSortedTimeStampFiles(fullPath)
            If files.Count >= maxFiles Then
                ' Älteste löschen, bis die Anzahl passt
                For Each oldFile As (Datum As DateTime, FullPath As String) In files.Skip(maxFiles - 1)
                    Try
                        File.Delete(oldFile.FullPath)
                    Catch ex As Exception
                        ' Ignorieren, wenn nicht löschbar
                    End Try
                Next
            End If
        End If

        Return newFilePath

    End Function

    ''' <summary>
    ''' Aus dem fullPath (einschließlich Dateiname) wird das Verzeichnis ermittelt und alle Dateien,
    ''' die auf das angegebene Pattern passen, alphabetisch sortiert angezeigt. 
    ''' Der Benutzer kann eine Datei auswählen.
    ''' </summary>
    ''' <returns>Vollständiger Pfad der gewählten Datei oder String.Empty</returns>
    Public Function GetFullpathFromSelectedFile(path As String,
                                            pattern As String,
                                            Optional header As String = Nothing) As String

        Dim files As List(Of String) = GetSortedFiles(path, pattern)
        If files.Count = 0 Then Return String.Empty

        Using frm As New Form With {
        .Text = If(String.IsNullOrEmpty(header), "Datei auswählen", header),
        .Width = 500,
        .Height = 300,
        .StartPosition = FormStartPosition.CenterScreen
    }
            Dim lb As New ListBox With {
            .Dock = DockStyle.Fill
        }
            Dim found As Boolean

            For Each f As String In files
                lb.Items.Add(IO.Path.GetFileName(f))
                found = True
            Next

            If Not found Then
                MsgBox("Es gibt hier (noch) keine Dateien", MsgBoxStyle.Information,
                   If(String.IsNullOrEmpty(header), "Datei auswählen", header))
                Return String.Empty
            End If

            frm.Controls.Add(lb)

            Dim btnOK As New Button With {
            .Text = "OK",
            .Dock = DockStyle.Bottom
        }
            frm.Controls.Add(btnOK)

            AddHandler btnOK.Click, Sub() frm.DialogResult = DialogResult.OK

            If frm.ShowDialog() = DialogResult.OK AndAlso lb.SelectedIndex >= 0 Then
                Return files(lb.SelectedIndex)
            End If
        End Using

        Return String.Empty

    End Function

    ''' <summary>
    ''' Holt alphabetisch sortierte Dateien im angegebenen Verzeichnis, die auf das Pattern passen.
    ''' </summary>
    Private Function GetSortedFiles(path As String, pattern As String) As List(Of String)
        Dim result As New List(Of String)
        If String.IsNullOrWhiteSpace(path) Then Return result

        If Not Directory.Exists(path) Then Return result

        result = Directory.GetFiles(path, pattern).OrderBy(Function(x) x).ToList()
        Return result
    End Function

#End Region

#Region "Load Save einschließlich der verzögerten Speicherung"


    Private changed As Boolean = False
    Private saveCts As CancellationTokenSource
    Private ReadOnly saveDebounceMs As Integer = 500

    'da wird etwas zugewiesen – nur eben kein „sichtbarer Inhalt“,
    'sondern ein anonymer Platzhalter-Object, der als Schloss dient,
    'damit andere Threads den Code zwischen SyncLock ind End SyncLock
    'nicht ausführen.
    Private ReadOnly gate As New Object()

    ' Muss threadsicher aufgerufen werden können
    Private Sub MarkChanged()

        SyncLock gate
            changed = True

            ' alte CTS abbrechen & verwerfen (Dispose später im Task)
            Dim oldCts As CancellationTokenSource = saveCts
            If oldCts IsNot Nothing Then
                Try : oldCts.Cancel()
                Catch : End Try
            End If

            ' neue CTS anlegen
            saveCts = New CancellationTokenSource()
            Dim localCts As CancellationTokenSource = saveCts

            ' Entprell-Aufgabe starten
            Task.Run(
                Async Function()
                    Try
                        Await Task.Delay(saveDebounceMs, localCts.Token).ConfigureAwait(False)

                        ' Nur speichern, wenn diese CTS noch die aktuelle ist
                        Dim isCurrent As Boolean
                        SyncLock gate
                            isCurrent = Object.ReferenceEquals(localCts, saveCts)
                        End SyncLock

                        If isCurrent AndAlso changed Then
                            ' Optional: changed vor/nach Save setzen – je nach Logik
                            Save() ' oder: Await SaveAsync().ConfigureAwait(False)
                            SyncLock gate
                                changed = False
                            End SyncLock
                        End If

                    Catch ex As TaskCanceledException
                        ' Abbruch = neue Änderung kam -> nichts tun
                    Finally
                        localCts.Dispose()
                    End Try
                End Function)
        End SyncLock
    End Sub

    ' Beispiel: beim Beenden oder Dispose aufrufen
    Public Sub Flush()

        Dim cts As CancellationTokenSource = Nothing
        SyncLock gate
            cts = saveCts
        End SyncLock

        ' laufende Debounce-Phase abbrechen und direkt speichern
        If cts IsNot Nothing Then
            Try
                cts.Cancel()
            Finally
                cts.Dispose()
            End Try
        End If

        If changed Then
            Save()
            SyncLock gate
                changed = False
            End SyncLock
        End If
    End Sub

    ''' <summary>
    ''' Speichert die Datei sofort.
    ''' </summary>
    Public Sub Save()
        If Not changed Then Return

        Try
            Using sw As New StreamWriter(_fileFullPath, False, Encoding.UTF8)
                For Each l As String In lines
                    sw.WriteLine(l)
                Next
            End Using
            changed = False
        Catch ex As Exception
            'TODO Logging/Fehlermeldung 
        End Try
    End Sub

    ''' <summary>
    ''' Lädt die Datei oder erzeugt neue.
    ''' </summary>
    Private Sub Load()
        lines.Clear()
        If File.Exists(_fileFullPath) Then
            For Each l As String In File.ReadAllLines(_fileFullPath)
                Dim trimmed As String = l.Trim()
                If trimmed <> "" Then
                    lines.Add(trimmed)
                End If
            Next
        End If
    End Sub

#End Region

#Region "Dispose"

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                Flush()
            End If
            disposedValue = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub

#End Region

End Class

