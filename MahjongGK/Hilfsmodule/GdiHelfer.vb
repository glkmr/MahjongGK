
Imports System.Drawing.Drawing2D
Imports System.Drawing.Text

Public Module GdiHelfer

    ''' <summary>
    ''' Wendet hochwertige Render-Settings auf ein vorhandenes Graphics an (z. B. im Paint-Event).
    ''' </summary>
    Public Sub ConfigureHighQuality(g As Graphics)
        g.SmoothingMode = SmoothingMode.AntiAlias
        g.InterpolationMode = InterpolationMode.HighQualityBicubic
        g.PixelOffsetMode = PixelOffsetMode.HighQuality
        g.CompositingQuality = CompositingQuality.HighQuality
        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit
    End Sub

End Module
