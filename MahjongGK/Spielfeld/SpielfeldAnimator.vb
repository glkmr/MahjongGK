
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006

Namespace Spielfeld

    'Private rnd As New Random()

    'Public Sub SetRandomAnimation(ByRef feld As FeldBeschreiber)
    '    feld.AnimID = CType(rnd.Next(1, 13), AnimationType) ' 1–12
    '    feld.AktStep = 0
    '    feld.MaxStep = 350 ' oder je nach Effekt
    'End Sub

    ''' <summary>
    ''' Hier sind alle Funktionen zur Animation angesiedelt einschließlich deren Verwaltung.
    ''' </summary>
    Public Module SpielfeldAnimator

        'Grundregeln aller Funktionen:
        'bmp ist das Original.
        'aktStep ist 0…maxStep.
        'Jede Funktion gibt ein neues Bitmap zurück (nicht das Original verändern).
        'Transparenz bleibt erhalten.
        'Alle Funktionen sind so geschrieben, dass sie sich sowohl für Erscheinen als auch für
        'Verschwinden anpassen lassen (Parameter-Logik umkehren).

        'Animations-Set (12 Stück)

        Public Enum AnimationType
            None = 0
            ScaleDown = 1
            ScaleUp = 2
            Rotate = 3
            RotateShrink = 4
            FlipX = 5
            FlipY = 6
            FlipShrink = 7
            Pulse = 8
            SlideLeft = 9
            SlideUp = 10
            ScaleSlide = 11
            RotatePulse = 12
        End Enum

        Public Function RunAnimation(animType As AnimationType, bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Select Case animType
                Case AnimationType.ScaleDown : Return Animation_ScaleDown(bmp, aktStep, maxStep)
                Case AnimationType.ScaleUp : Return Animation_ScaleUp(bmp, aktStep, maxStep)
                Case AnimationType.Rotate : Return Animation_Rotate(bmp, aktStep, maxStep)
                Case AnimationType.RotateShrink : Return Animation_RotateShrink(bmp, aktStep, maxStep)
                Case AnimationType.FlipX : Return Animation_FlipX(bmp, aktStep, maxStep)
                Case AnimationType.FlipY : Return Animation_FlipY(bmp, aktStep, maxStep)
                Case AnimationType.FlipShrink : Return Animation_FlipShrink(bmp, aktStep, maxStep)
                Case AnimationType.Pulse : Return Animation_Pulse(bmp, aktStep, maxStep)
                Case AnimationType.SlideLeft : Return Animation_SlideLeft(bmp, aktStep, maxStep)
                Case AnimationType.SlideUp : Return Animation_SlideUp(bmp, aktStep, maxStep)
                Case AnimationType.ScaleSlide : Return Animation_ScaleSlide(bmp, aktStep, maxStep)
                Case AnimationType.RotatePulse : Return Animation_RotatePulse(bmp, aktStep, maxStep)
                Case Else
                    Return bmp ' keine Animation
            End Select
        End Function



        ''' <summary>
        ''' Einfaches Schrumpfen
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_ScaleDown(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim factor As Single = CSng(1.0F - (aktStep / maxStep))
            Return ScaleBitmap(bmp, factor, factor)
        End Function
        '
        ''' <summary>
        ''' Einfaches Wachsen
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_ScaleUp(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim factor As Single = CSng(aktStep / maxStep)
            Return ScaleBitmap(bmp, factor, factor)
        End Function
        '
        ''' <summary>
        ''' Rotation
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_Rotate(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim angle As Single = CSng(360.0F * (aktStep / maxStep))
            Return RotateBitmap(bmp, angle, 1.0F)
        End Function
        '
        ''' <summary>
        ''' Dehnen und Schrumpfen
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_RotateShrink(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim progress As Single = CSng(aktStep / maxStep)
            Dim angle As Single = 360.0F * progress
            Dim scale As Single = 1.0F - progress
            Return RotateBitmap(bmp, angle, scale)
        End Function
        '
        ''' <summary>
        ''' Horizontaler Überschlag
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_FlipX(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim factorX As Single = CSng(Math.Abs(Math.Cos((aktStep / maxStep) * Math.PI)))
            Return ScaleBitmap(bmp, factorX, 1.0F)
        End Function
        '
        ''' <summary>
        ''' Vertikaler Überschlag
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_FlipY(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim factorY As Single = CSng(Math.Abs(Math.Cos((aktStep / maxStep) * Math.PI)))
            Return ScaleBitmap(bmp, 1.0F, factorY)
        End Function
        '
        ''' <summary>
        ''' Flip plus Schrumpfen
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_FlipShrink(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim p As Single = CSng(aktStep / maxStep)
            Dim factorX As Single = CSng(Math.Abs(Math.Cos(p * Math.PI)))
            Dim scale As Single = 1.0F - p
            Return ScaleBitmap(bmp, factorX * scale, scale)
        End Function
        '
        ''' <summary>
        ''' Pulsieren
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_Pulse(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim factor As Single = CSng(1.0F + 0.2F * Math.Sin((aktStep / maxStep) * Math.PI * 2))
            Return ScaleBitmap(bmp, factor, factor)
        End Function
        '
        ''' <summary>
        ''' Seitliches Wegschieben
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_SlideLeft(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim offsetX As Integer = CInt((aktStep / maxStep) * bmp.Width)
            Return TranslateBitmap(bmp, -offsetX, 0)
        End Function
        '
        ''' <summary>
        ''' Nach oben wegschieben
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_SlideUp(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim offsetY As Integer = CInt((aktStep / maxStep) * bmp.Height)
            Return TranslateBitmap(bmp, 0, -offsetY)
        End Function
        '
        ''' <summary>
        ''' Schrumpfen plus Wegschieben
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_ScaleSlide(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim p As Single = CSng(aktStep / maxStep)
            Dim scale As Single = 1.0F - p
            Dim offsetX As Integer = CInt(p * bmp.Width)
            Return ScaleTranslateBitmap(bmp, scale, scale, offsetX, 0)
        End Function
        '
        ''' <summary>
        ''' Drehen und pulsieren
        ''' </summary>
        ''' <param name="bmp"></param>
        ''' <param name="aktStep"></param>
        ''' <param name="maxStep"></param>
        ''' <returns></returns>
        Public Function Animation_RotatePulse(bmp As Bitmap, aktStep As Integer, maxStep As Integer) As Bitmap
            Dim p As Single = CSng(aktStep / maxStep)
            Dim angle As Single = 360.0F * p
            Dim scale As Single = CSng(1.0F + 0.1F * Math.Sin(p * Math.PI * 4))
            Return RotateBitmap(bmp, angle, scale)
        End Function
        '
        '
        ''########
        'Toolkit Hilfsfunktionen

        Private Function ScaleBitmap(src As Bitmap, scaleX As Single, scaleY As Single) As Bitmap
            Dim w As Integer = Math.Max(1, CInt(src.Width * scaleX))
            Dim h As Integer = Math.Max(1, CInt(src.Height * scaleY))
            Dim newBmp As New Bitmap(w, h)
            newBmp.MakeTransparent()
            Using g As Graphics = Graphics.FromImage(newBmp)
                g.InterpolationMode = INI.Spielfeld_InterpolationMode
                g.DrawImage(src, 0, 0, w, h)
            End Using
            Return newBmp
        End Function

        Private Function RotateBitmap(src As Bitmap, angle As Single, scale As Single) As Bitmap
            Dim w As Integer = src.Width
            Dim h As Integer = src.Height
            Dim newBmp As New Bitmap(w, h)
            newBmp.MakeTransparent()
            Using g As Graphics = Graphics.FromImage(newBmp)
                g.InterpolationMode = INI.Spielfeld_InterpolationMode
                g.TranslateTransform(w / 2.0F, h / 2.0F)
                g.RotateTransform(angle)
                g.ScaleTransform(scale, scale)
                g.TranslateTransform(-w / 2.0F, -h / 2.0F)
                g.DrawImage(src, 0, 0, w, h)
            End Using
            Return newBmp
        End Function

        Private Function TranslateBitmap(src As Bitmap, offsetX As Integer, offsetY As Integer) As Bitmap
            Dim newBmp As New Bitmap(src.Width, src.Height)
            newBmp.MakeTransparent()
            Using g As Graphics = Graphics.FromImage(newBmp)
                g.InterpolationMode = INI.Spielfeld_InterpolationMode
                g.DrawImage(src, offsetX, offsetY, src.Width, src.Height)
            End Using
            Return newBmp
        End Function

        Private Function ScaleTranslateBitmap(src As Bitmap, scaleX As Single, scaleY As Single, offsetX As Integer, offsetY As Integer) As Bitmap
            Dim w As Integer = Math.Max(1, CInt(src.Width * scaleX))
            Dim h As Integer = Math.Max(1, CInt(src.Height * scaleY))
            Dim newBmp As New Bitmap(src.Width, src.Height)
            newBmp.MakeTransparent()
            Using g As Graphics = Graphics.FromImage(newBmp)
                g.InterpolationMode = INI.Spielfeld_InterpolationMode
                Dim x As Integer = (src.Width - w) \ 2 + offsetX
                Dim y As Integer = (src.Height - h) \ 2 + offsetY
                g.DrawImage(src, x, y, w, h)
            End Using
            Return newBmp
        End Function


    End Module
End Namespace