Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions


Public Enum CommentSeperator
    None
    CrLf
    Tilde
End Enum
''' <summary>
''' Liest Kommentarblöcke (beginnend mit ';') aus einer oder mehreren INI-Dateien,
''' ordnet sie dem jeweils folgenden Key in der aktuellen Section zu und stellt
''' sie über GetKommentar(NameOf(INI.Section_Key)) bereit.
''' </summary>
Public NotInheritable Class IniCommentReader

    Private ReadOnly _paths As String()
    ' Schlüssel: "Section|Key"  -> Liste von Kommentarzeilen (ohne Spacer-Zeile)
    Private ReadOnly _comments As New Dictionary(Of String, List(Of String))(StringComparer.OrdinalIgnoreCase)
    Private ReadOnly _sectionsSeen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

    ' Regex-Helfer
    Private Shared ReadOnly RxSection As New Regex("^\s*\[(?<sec>[^\]]+)\]\s*$", RegexOptions.Compiled)
    Private Shared ReadOnly RxKeyLine As New Regex("^\s*(?<key>[A-Za-z0-9_]+)\s*=", RegexOptions.Compiled)
    Private Shared ReadOnly RxComment As New Regex("^\s*;(?<txt>.*)$", RegexOptions.Compiled)

    Sub New()

    End Sub

    ''' <summary>
    ''' Erzeugt einen Leser und parst alle angegebenen INI-Dateien sofort.
    ''' Reihenfolge der Pfade = Reihenfolge der Auswertung.
    ''' </summary>
    Public Sub New(ParamArray fullPaths As String())
        If fullPaths Is Nothing OrElse fullPaths.Length = 0 Then
            Throw New ArgumentException("Mindestens eine INI-Datei angeben.", NameOf(fullPaths))
        End If
        For Each p As String In fullPaths
            If String.IsNullOrWhiteSpace(p) Then Throw New ArgumentException("Leerer INI-Pfad übergeben.", NameOf(fullPaths))
            If Not File.Exists(p) Then Throw New FileNotFoundException("INI nicht gefunden.", p)
        Next
        _paths = CType(fullPaths.Clone(), String())
        ParseAll()
    End Sub

    ''' <summary>
    ''' Kommentar zum Property-Namen im Schema NameOf(INI.Section_Key).
    ''' Beispiel: GetKommentar(NameOf(INI.Rendering_UseGrafikOrgSize))
    ''' </summary>
    ''' <param name="propertyNameOfIni">NameOf(INI.Section_Key)</param>
    ''' <param name="seperator">Zeilenumbrüche: ohne, Tilde ("~") oder CrLf</param>
    Public Function GetKommentar(propertyNameOfIni As String,
                                 Optional seperator As CommentSeperator = CommentSeperator.CrLf) As String
        If String.IsNullOrWhiteSpace(propertyNameOfIni) Then Return String.Empty

        ' Split am ersten Unterstrich: vor = Section, nach = Key
        Dim underscoreIdx As Integer = propertyNameOfIni.IndexOf("_"c)
        If underscoreIdx <= 0 OrElse underscoreIdx >= propertyNameOfIni.Length - 1 Then
            Return String.Empty
        End If

        Dim section As String = propertyNameOfIni.Substring(0, underscoreIdx)
        Dim key As String = propertyNameOfIni.Substring(underscoreIdx + 1)
        Dim dictKey As String = MakeDictKey(section, key)

        Dim lines As List(Of String) = Nothing
        If _comments.TryGetValue(dictKey, lines) AndAlso lines IsNot Nothing AndAlso lines.Count > 0 Then
            Select Case seperator
                Case CommentSeperator.CrLf
                    Return String.Join(vbCrLf, lines)
                Case CommentSeperator.Tilde
                    Return String.Join("~", lines)
                Case CommentSeperator.None
                    Return String.Join(" ", lines)
            End Select
        End If
        Return String.Empty
    End Function

    ''' <summary>
    ''' Neu einlesen (falls Dateien sich geändert haben).
    ''' </summary>
    Public Sub Reload()
        _comments.Clear()
        _sectionsSeen.Clear()
        ParseAll()
    End Sub

    ' -------------------- intern --------------------

    Private Shared Function MakeDictKey(section As String, key As String) As String
        Return $"{section}|{key}"
    End Function

    Private Sub ParseAll()
        Dim currentSection As String = ""
        Dim pendingComments As New List(Of String)()
        Dim haveSeenAnyComment As Boolean = False

        For Each Path As String In _paths
            Dim lines As String() = File.ReadAllLines(Path, Encoding.UTF8)

            For Each raw As String In lines
                Dim line As String = raw

                ' Section?
                Dim mSec As Match = RxSection.Match(line)
                If mSec.Success Then
                    Dim sec As String = mSec.Groups("sec").Value.Trim()

                    ' Sektion darf ini-übergreifend nur einmal existieren
                    If _sectionsSeen.Contains(sec) Then
                        Throw New InvalidOperationException($"Section ""{sec}"" ist mehrfach definiert (Datei: {Path}).")
                    End If

                    _sectionsSeen.Add(sec)
                    currentSection = sec
                    pendingComments.Clear()
                    haveSeenAnyComment = False
                    Continue For
                End If

                ' Kommentarzeile?
                Dim mCom As Match = RxComment.Match(line)
                If mCom.Success Then
                    Dim text As String = mCom.Groups("txt").Value
                    If haveSeenAnyComment Then
                        ' ab der zweiten Kommentarzeile sammeln (erste = Spacer)
                        pendingComments.Add(text.Trim())
                    Else
                        haveSeenAnyComment = True
                    End If
                    Continue For
                End If

                ' Key-Zeile?
                Dim mKey As Match = RxKeyLine.Match(line)
                If mKey.Success Then
                    Dim key As String = mKey.Groups("key").Value.Trim()

                    If pendingComments.Count > 0 Then
                        Dim dictKey As String = MakeDictKey(currentSection, key)
                        ' Nur setzen, wenn noch nicht vorhanden – "erste Definition gewinnt"
                        If Not _comments.ContainsKey(dictKey) Then
                            ' leere Zeilen am Ende/Anfang entfernen
                            Dim cleaned As List(Of String) =
                                pendingComments.Where(Function(s) s IsNot Nothing).Select(Function(s) s.Trim()) _
                                               .ToList()
                            ' Leeren Kommentarblock ignorieren
                            If cleaned.Count > 0 AndAlso cleaned.Any(Function(s) s.Length > 0) Then
                                _comments(dictKey) = cleaned
                            End If
                        End If
                    End If

                    ' Puffer leeren für nächsten Block
                    pendingComments.Clear()
                    haveSeenAnyComment = False
                    Continue For
                End If

                ' Andere Zeilen: ggf. Kommentarblock beenden
                If pendingComments.Count > 0 OrElse haveSeenAnyComment Then
                    pendingComments.Clear()
                    haveSeenAnyComment = False
                End If
            Next
        Next
    End Sub

End Class
