Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006


Namespace Spielfeld
    '
    ''' <summary>
    ''' Hier sind Testdaten drin, und Code, den ich zur Erstellung dieser Testdaten
    ''' bei der Programmentwicklung gebraucht habe, die aber im fertigem Programm ohne
    ''' Bedeutung sind.
    ''' </summary>
    Module Testdaten

        Public Sub TestDaten_Spielfeld_3_x_3_x_1()

            Dim newSpielfeldInfo As New SpielfeldInfo(New Triple(3, 3, 1))

            With newSpielfeldInfo
                Dim centerXyz As Triple = .GetSpielfeldCenter(0)
                .AddSteinToSpielfeld(SteinIndexEnum.Bambus1, centerXyz)

                Dim tplr As TripleR
                tplr = .SearchPlace(centerXyz, Direction.Left)
                If tplr.Found = TripleR.Result.FoundFreePlace Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus2, tplr)
                End If

                tplr = .SearchPlace(centerXyz, Direction.LeftUp)
                If tplr.Found = TripleR.Result.FoundFreePlace Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus3, tplr)
                End If

                tplr = .SearchPlace(centerXyz, Direction.RightDown)
                If tplr.Found = TripleR.Result.FoundFreePlace Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus4, tplr)
                End If

                tplr = .SearchPlace(tplr, Direction.Up)
                If tplr.Found = TripleR.Result.FoundFreePlace Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus5, tplr)
                End If

                tplr = .SearchPlace(tplr, Direction.Up)
                If tplr.Found = TripleR.Result.FoundFreePlace Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus6, tplr)
                End If

            End With

            Spielfeld.aktSpielfeldInfo = newSpielfeldInfo
            Spielfeld.PaintLimiterModul.PaintSpielfeld_AktPermission = True


            'DebugHilfen.Print3DArrayToTxtOutputForm(aktSpielfeldInfo)
            '   DebugShowArrFBMain(arrFBMain, 0)

        End Sub
        Public Sub TestDaten_Spielfeld_3_x_3_x_3()

            Dim newSpielfeldInfo As New SpielfeldInfo(New Triple(3, 3, 1))

            With newSpielfeldInfo

                Dim tplr As New TripleR

                tplr = New TripleR(1, 1, 0)
                tplr = .SearchPlace(tplr, Direction.Right)
                If tplr.Found = TripleR.Result.FoundFreePlace Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus1, tplr)
                End If

                tplr = .SearchPlace(tplr, Direction.Right)
                If tplr.Found = TripleR.Result.FoundFreePlace Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus2, tplr)
                End If

                tplr = .SearchPlace(tplr, Direction.Right)
                If tplr.Found = TripleR.Result.FoundFreePlace Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus3, tplr)
                End If
                '
                tplr = New TripleR(3, 1, 0)
                tplr = .SearchPlace(tplr, Direction.Right)
                If tplr.Found = TripleR.Result.FoundFreePlace Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus4, tplr)
                End If

                tplr = .SearchPlace(tplr, Direction.Right)
                If tplr.Found = TripleR.Result.FoundFreePlace Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus5, tplr)
                End If

                tplr = .SearchPlace(tplr, Direction.Right)
                If tplr.Found = TripleR.Result.FoundFreePlace Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus6, tplr)
                End If

                tplr = New TripleR(5, 1, 0)
                tplr = .SearchPlace(tplr, Direction.Right)
                If tplr.Found = TripleR.Result.FoundFreePlace Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus7, tplr)
                End If

                tplr = .SearchPlace(tplr, Direction.Right)
                If tplr.Found = TripleR.Result.FoundFreePlace Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus8, tplr)
                End If

                tplr = .SearchPlace(tplr, Direction.Right)
                If tplr.Found = TripleR.Result.FoundFreePlace Then
                    .AddSteinToSpielfeld(SteinIndexEnum.Bambus9, tplr)
                End If

            End With

            'Spielfeld.aktSpielfeldInfo = newSpielfeldInfo
            'Spielfeld.PaintLimiterModul.PaintSpielfeld_AktPermission = True


            DebugHilfen.Print3DArrayToTxtOutputForm(newSpielfeldInfo)
            '   DebugShowArrFBMain(arrFBMain, 0)

        End Sub



    End Module
End Namespace
