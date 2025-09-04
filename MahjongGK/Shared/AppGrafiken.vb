Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On
Imports System.Collections.Concurrent
Imports System.Drawing.Imaging
Imports System.IO

' ###########################################################
' App-Grafiken (16x16) – Mehrere Sätze + Fallback auf Ressourcen
' ###########################################################
' Annahmen (gemäß Vorgabe):
' - Default-Satz liegt in den Ressourcen (My.Resources).
' - Weitere Sätze liegen als *.png in:
'     AppDataDirectory(AppDataSubDir.Grafiken,
'                      AppDataSubSubDir.Grafiken_AppGrafiken16x16,
'                      AppGrafikName.XYZ.ToString)
'   und die Datei heißt:   <AppGrafikSatz>.png
'   Beispiel:  ...\Grafiken\Grafiken_AppGrafiken16x16\Undo\Satz1.png
' - ErrorGrafik liegt IMMER in den Ressourcen und ist der "Notnagel".
'
' Hinweise:
' - Du kannst überall GetAppGrafik(satz, name) aufrufen.
' - Wenn du einen "aktuellen" Satz global hast (z. B. INI.AppGrafik_AktiverSatz),
'   mach dir einfach einen Wrapper, der den Satz aus INI zieht.
' ###########################################################

Public Module AppGrafiken

    ' Optionales Cache (Thread-safe)
    Private ReadOnly _cache As New ConcurrentDictionary(Of String, Image)(StringComparer.OrdinalIgnoreCase)

    ''' <summary>
    ''' Liefert eine 16x16-Grafik nach Satz/Name mit Fallback:
    ''' Datei (AppData) → Default-Ressource → ErrorGrafik-Ressource.
    ''' </summary>
    Public Function GetAppGrafik(satz As AppGrafikSatz, name As AppGrafikName) As Image
        Dim key As String = $"{satz}|{name}"
        Dim img As Image = Nothing

        If _cache.TryGetValue(key, img) Then
            Return img
        End If

        ' 1) Dateisystem (nur wenn NICHT Default)
        If satz <> AppGrafikSatz.Default Then
            Dim appDataSub3 As String = name.ToString() ' gemäß Vorgabe
            Dim dirPath As String = AppDataDirectory(AppDataSubDir.Grafiken,
                                                     AppDataSubSubDir.Grafiken_AppGrafiken16x16,
                                                     appDataSub3)
            Dim filePath As String = Path.Combine(dirPath, $"{satz}.png")
            img = TryLoadPng(filePath)
            If img IsNot Nothing Then
                img = Ensure16x16(img)
                _cache.TryAdd(key, img)
                Return img
            End If
        End If

        ' 2) Default-Ressource
        img = TryLoadResourceByName(name.ToString())
        If img IsNot Nothing Then
            img = Ensure16x16(img)
            _cache.TryAdd(key, img)
            Return img
        End If

        ' 3) ErrorGrafik (Ressource)
        Dim errImg As Image = TryLoadResourceByName(AppGrafikName.ErrorGrafik.ToString())
        If errImg IsNot Nothing Then
            errImg = Ensure16x16(errImg)
            _cache.TryAdd(key, errImg)
            Return errImg
        End If

        ' 4) Letzter Notnagel (falls sogar ErrorGrafik fehlt)
        Dim fallback As Image = MakeFallbackImage()
        _cache.TryAdd(key, fallback)
        Return fallback
    End Function

    ''' <summary>
    ''' Komfort-Overload: Zieht den aktiven Satz z. B. aus deiner INI.
    ''' Passe die Quelle an (INI.AppGrafik_AktiverSatz o.ä.).
    ''' </summary>
    Public Function GetAppGrafik(name As AppGrafikName) As Image
        Dim aktiverSatz As AppGrafikSatz = GetAktiverGrafikSatzAusIni()
        Return GetAppGrafik(aktiverSatz, name)
    End Function

    ''' <summary>
    ''' Cache leeren (z. B. nach Satzwechsel).
    ''' </summary>
    Public Sub ClearAppGrafikCache()
        For Each kv As KeyValuePair(Of String, Image) In _cache
            Try
                If kv.Value IsNot Nothing Then
                    kv.Value.Dispose()
                End If
            Catch ex As ObjectDisposedException
                ' schon freigegeben -> ignorieren
            Catch ex As Exception
                ' andere Fehler vorsichtshalber auch ignorieren
                ' (z. B. falls ein Image handle-gebunden ist)
            End Try
        Next
        _cache.Clear()
    End Sub

    ' ########## interne Helfer ##########

    Private Function TryLoadPng(fullPath As String) As Image
        Try
            If File.Exists(fullPath) Then
                ' Image.FromFile hält die Datei gelockt → besser per Stream klonen
                Using fs As New FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read)
                    Using tmp As Image = Image.FromStream(fs)
                        Return New Bitmap(tmp)
                    End Using
                End Using
            End If
        Catch
            ' Ignorieren → Fallback greifen lassen
        End Try
        Return Nothing
    End Function

    Private Function TryLoadResourceByName(resName As String) As Image
        Try
            Dim obj As Object = My.Resources.ResourceManager.GetObject(resName, My.Resources.Culture)
            If obj IsNot Nothing Then
                Dim img As Image = TryCast(obj, Image)
                If img IsNot Nothing Then
                    Return New Bitmap(img) ' Kopie erstellen (Ressourcen-Instanz unberührt lassen)
                End If
            End If
        Catch
            ' Ignorieren → Fallback greifen lassen
        End Try
        Return Nothing
    End Function

    Private Function Ensure16x16(src As Image) As Image
        If src.Width = 16 AndAlso src.Height = 16 Then
            Return src
        End If
        ' sanft skalieren auf 16x16
        Dim bmp As New Bitmap(16, 16, PixelFormat.Format32bppPArgb)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
            g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
            g.SmoothingMode = Drawing2D.SmoothingMode.HighQuality
            g.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
            g.DrawImage(src, New Rectangle(0, 0, 16, 16))
        End Using
        src.Dispose()
        Return bmp
    End Function

    Private Function MakeFallbackImage() As Image
        Dim bmp As New Bitmap(16, 16, PixelFormat.Format32bppPArgb)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.Clear(Color.Transparent)
            Using p As New Pen(Color.Red, 2.0F)
                g.DrawRectangle(p, 1, 1, 14, 14)
                g.DrawLine(p, 2, 14, 14, 2)
            End Using
        End Using
        Return bmp
    End Function

    Private Function GetAktiverGrafikSatzAusIni() As AppGrafikSatz
        ' Passe das an deine INI an. Temporär: Default, wenn nicht vorhanden.
        ' Beispiel (Pseudo): Return DirectCast(INI.AppGrafik_AktiverSatz, AppGrafikSatz)
        Return AppGrafikSatz.Default
    End Function

End Module

