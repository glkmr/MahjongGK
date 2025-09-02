Option Strict Off
Option Explicit Off
Option Infer Off

Imports System.Reflection

' -------------------------------------------------------------------------
'  IniCacheReset: setzt Puffer indirekt zurück, indem Werte identisch
'  ins jeweilige Property zurückgeschrieben werden (Getter -> Setter).
'  Zieltyp ist standardmäßig das Modul INI (Public Shared Properties).
' -------------------------------------------------------------------------
Public Module IniCacheReset


    '' Verwendung
    '' Standard: nur Properties mit Unterstrich, bei Fehlern weiter, kein DryRun
    '  Dim sum = IniCacheReset.RefreshIniPropertyCaches(
    '       New IniCacheReset.ResetOptions With {
    '           .OnlyWithUnderscore = True,
    '           .ContinueOnError = True,
    '           .DryRun = False,
    '           .Logger = Sub(msg) Debug.Print(msg) ' oder AddressOf Console.WriteLine
    '       }
    '  )
    '  Debug.Print(sum.ToString())
    '
    ' Varianten:
    '   Nur bestimmte Properties refreshen:
    '       .IncludeOnly = {"Selection_Font", "Key_Timeout"}
    '   Bestimmte Properties ausschließen:
    '       .Exclude = {"Selection_Experimental"}
    '   Dry-Run (nur anzeigen, nichts setzen):
    '       .DryRun = True

    'Warum das für alle deine Typen funktioniert

    'Du liest und schreibst denselben Wert zurück. Reflection kümmert sich um Boxing/Unboxing
    '(ValueTypes wie Integer, Double, Date, Color, Point, Size, Rectangle, die …F-Varianten, Enum)
    'genau so wie um Referenztypen (String, Font).
    'Dein Setter wird dadurch immer ausgeführt und setzt den jeweiligen Cache zurück.
    'Wenn dein Setter intern „nichts tun“ würde, wenn old = new, kannst du an der Stelle
    'wahlweise einen Minimal-„Nudge“ implementieren (z. B. SetValue(Nothing, current)
    'triggert ja bereits, oder im Setter bewusst Cache-Reset unabhängig vom Vergleich).

    'Wenn du statt INI mal einen anderen Typ resetten willst, nutze direkt
    '   Dim sum = IniCacheReset.RefreshPropertyCaches(GetType(AndererTyp), options)


    ' Optionen für den Lauf
    Public NotInheritable Class ResetOptions
        ' Nur Properties mit Unterstrich im Namen? (Standard: True)
        Public Property OnlyWithUnderscore As Boolean = True
        ' Bei Fehlern weitermachen? (Standard: True)
        Public Property ContinueOnError As Boolean = True
        ' Nur diese Property-Namen berücksichtigen (optional, Case-Insensitive)
        Public Property IncludeOnly As IEnumerable(Of String) = Nothing
        ' Diese Property-Namen überspringen (optional, Case-Insensitive)
        Public Property Exclude As IEnumerable(Of String) = Nothing
        ' Trockenlauf ohne Setzen (nur Auflistung)
        Public Property DryRun As Boolean = False
        ' Optionales Logging (z. B. Debug.Print)
        Public Property Logger As Action(Of String) = Nothing
    End Class

    ' Bequemer Default
    Private ReadOnly _defaultOptions As New ResetOptions()

    ' Hauptmethode – Version für dein Modul INI
    Public Function RefreshIniPropertyCaches(Optional opts As ResetOptions = Nothing) As RefreshSummary
        Return RefreshPropertyCaches(GetType(INI), opts)
    End Function

    ' Hauptmethode – generisch für jeden Typ (falls du später was anderes resetten willst)
    Public Function RefreshPropertyCaches(targetType As Type, Optional opts As ResetOptions = Nothing) As RefreshSummary
        Dim o As ResetOptions = If(opts, _defaultOptions)
        Dim log = If(o.Logger, Sub(_msg As String) Debug.WriteLine(_msg))

        Dim sw As Stopwatch = Stopwatch.StartNew()
        Dim total As Integer = 0, skipped As Integer = 0, resetOk As Integer = 0, failed As Integer = 0

        Dim includeSet As HashSet(Of String) = ToSet(o.IncludeOnly)
        Dim excludeSet As HashSet(Of String) = ToSet(o.Exclude)

        ' Nur Public Shared Properties (Module -> Shared)
        Dim flags As BindingFlags = BindingFlags.Public Or BindingFlags.Static
        Dim props = targetType.GetProperties(flags)

        For Each p In props
            ' skip: Indexer, ReadOnly, WriteOnly (nur lesen+schreiben, keine Indexparameter)
            If p.GetIndexParameters().Length <> 0 Then
                skipped += 1 : Continue For
            End If
            If Not p.CanRead OrElse Not p.CanWrite Then
                skipped += 1 : Continue For
            End If

            ' Filter: Include/Exclude by name (case-insensitive)
            If includeSet IsNot Nothing AndAlso includeSet.Count > 0 AndAlso Not includeSet.Contains(p.Name) Then
                skipped += 1 : Continue For
            End If
            If excludeSet IsNot Nothing AndAlso excludeSet.Contains(p.Name) Then
                skipped += 1 : Continue For
            End If

            ' Filter: Unterstrich im Namen (falls gewünscht)
            If o.OnlyWithUnderscore AndAlso p.Name.IndexOf("_"c) < 0 Then
                skipped += 1 : Continue For
            End If

            total += 1

            Try
                ' Lesen (Shared -> target = Nothing)
                Dim current As Object = p.GetValue(Nothing, Nothing)

                If o.DryRun Then
                    log?.Invoke($"[DRY] {p.DeclaringType.Name}.{p.Name} :: {TypeNameOf(p.PropertyType)}")
                Else
                    ' Zurückschreiben des identischen Wertes -> Setter setzt Puffer zurück
                    p.SetValue(Nothing, current, Nothing)
                    resetOk += 1
                    log?.Invoke($"[OK]  {p.DeclaringType.Name}.{p.Name} :: {TypeNameOf(p.PropertyType)}")
                End If

            Catch ex As TargetInvocationException
                failed += 1
                log?.Invoke($"[ERR] {p.DeclaringType.Name}.{p.Name}: {ex.InnerException?.Message}")
                If Not o.ContinueOnError Then Exit For
            Catch ex As Exception
                failed += 1
                log?.Invoke($"[ERR] {p.DeclaringType.Name}.{p.Name}: {ex.Message}")
                If Not o.ContinueOnError Then Exit For
            End Try
        Next

        sw.Stop()
        Return New RefreshSummary With {
            .TargetType = targetType,
            .Matched = total,
            .Skipped = skipped,
            .ResetOk = resetOk,
            .Failed = failed,
            .Elapsed = sw.Elapsed
        }
    End Function

    ' Zusammenfassung fürs Logging/Debug
    Public NotInheritable Class RefreshSummary
        Public Property TargetType As Type
        Public Property Matched As Integer
        Public Property Skipped As Integer
        Public Property ResetOk As Integer
        Public Property Failed As Integer
        Public Property Elapsed As TimeSpan

        Public Overrides Function ToString() As String
            Return $"{TargetType.Name}: matched={Matched}, resetOK={ResetOk}, failed={Failed}, skipped={Skipped}, in {Elapsed.TotalMilliseconds:F0} ms"
        End Function
    End Class

    ' Helpers
    Private Function ToSet(names As IEnumerable(Of String)) As HashSet(Of String)
        If names Is Nothing Then Return Nothing
        Dim setCI As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        For Each n In names
            If Not String.IsNullOrWhiteSpace(n) Then setCI.Add(n.Trim())
        Next
        Return setCI
    End Function

    Private Function TypeNameOf(t As Type) As String
        ' nur fürs Log hübscher
        If t.IsEnum Then Return $"Enum({t.Name})"
        Return t.Name
    End Function

End Module

