
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006

Namespace Spielfeld

    Public Module PaintLimiterModul

        Public PaintLimiter As New PaintLimiterClass()

        Private CurrentControl As Control = Nothing
        Private WithEvents RenderTimer As New Timer With {.Interval = 1}


        Private _PaintSpielfeld_AktPermission As Boolean
        ''' <summary>
        ''' Nur wenn dieses Flag auf True steht, wird das Spielfeld gezeichnet.
        ''' </summary>
        ''' <returns></returns>
        Public Property PaintSpielfeld_AktPermission As Boolean
            Get
                Return _PaintSpielfeld_AktPermission
            End Get
            Set(value As Boolean)
                _PaintSpielfeld_AktPermission = value
                If value Then
                    PaintSpielfeld_GivePermissionIfPossible = False
                End If
            End Set
        End Property

        Public Property PaintSpielfeld_GivePermissionIfPossible As Boolean = False
        '
        ''' <summary>
        ''' Wird das Flag True gestellt, wird vor em nächsten Paint das Spielfeld
        ''' aktualisiert. UpdateSpielfeld wird dann mit 
        ''' Das Flag ist selbstrückstellend.
        ''' </summary>
        ''' <returns></returns>
        Public Property PaintSpielfeld_UpdteSpielfeld As Boolean = False

        ''' <summary>
        ''' Wird aus frmMain heraus aufgerufen mit der Angabe der Zeichenfläche.
        ''' Startet den Ausgabeprozess.
        ''' </summary>
        ''' <param name="currentControl"></param>
        Public Sub PaintSpielfeld_GiveGeneralPermission(currentControl As Control, visibleUserControl As frmMain.VisibleUserControl)

            ' Neues Control registrieren (altes wird automatisch abgemeldet)
            PaintLimiterModul.CurrentControl = currentControl
            RenderTimer.Start()
            PaintSpielfeld_GivePermissionIfPossible = True
            SpielfeldDaten.VisibleUserControl = visibleUserControl
        End Sub
        '
        ''' <summary>
        ''' Wird aus frmMain heraus aufgerufen. Stoppt den Ausgabeprozess.
        ''' Mehrfaches Aufrufen ist unschädlich.
        ''' </summary>
        Public Sub PaintSpielfeld_CancelGeneralPermission()
            RenderTimer.Stop()
            PaintSpielfeld_AktPermission = False
            PaintSpielfeld_GivePermissionIfPossible = False
            VisibleUserControl = frmMain.VisibleUserControl.None
        End Sub

        Private _lastRectOutput As Rectangle = New Rectangle
        ''' <summary>
        ''' Dieses Sub wird vom PaintEvent der Zeichenfläche (UCtlSpielfeld und UCtlEdtor) getaktet
        ''' aufgerufen. Es ist ein Verteiler, der entweder die Initialisierung des Spielfeldes,
        ''' Update des Spielfeldes oder das Zeichnen selber aufruft.
        ''' </summary>
        ''' <param name="visibleUserControl"></param>
        ''' <param name="e"></param>
        ''' <param name="rectOutput"></param>
        ''' <param name="timeDifferenzFaktor"></param>
        Public Sub PaintSpielfeld_Paint(visibleUserControl As frmMain.VisibleUserControl, e As PaintEventArgs, rectOutput As Rectangle, timeDifferenzFaktor As Double)

            If PaintSpielfeld_AktPermission Then

                If _lastRectOutput <> rectOutput Then
                    _lastRectOutput = rectOutput

                    UpdateSpielfeld(rectOutput, source:=UpdateSrc.PaintEvent)
                    '
                    'Erneute Abfrage. So bleibt die Möglichkeit offen in UpdateSpielfeld
                    'PaintSpielfeld_AktPermission = False und
                    'PaintSpielfeld_GivePermissionIfPossible = True
                    'zu setzen, das dann einen Takt später umgesetzt wird.
                    If PaintSpielfeld_AktPermission Then
                        DoPaintSpielfeld_Paint(e, rectOutput, timeDifferenzFaktor)
                    End If

                ElseIf PaintSpielfeld_UpdteSpielfeld Then
                    'Möglichkeit, um von irgendwoher ein Update zu erzwingen.
                    PaintSpielfeld_UpdteSpielfeld = False
                    UpdateSpielfeld(rectOutput, source:=UpdateSrc.PaintSpielfeld_UpdteSpielfeld_IsSet)
                    If PaintSpielfeld_AktPermission Then
                        DoPaintSpielfeld_Paint(e, rectOutput, timeDifferenzFaktor)
                    End If
                Else
                    DoPaintSpielfeld_Paint(e, rectOutput, timeDifferenzFaktor)
                End If


            ElseIf PaintSpielfeld_GivePermissionIfPossible Then

                'Wird solange bei jedem Takt aufgerufen, bis von irgendwoher
                'PaintSpielfeld_AktPermission = True gesetzt wird.
                UpdateSpielfeld_GivePermissionIfPossible(rectOutput)

                If PaintSpielfeld_AktPermission Then
                    DoPaintSpielfeld_Paint(e, rectOutput, timeDifferenzFaktor)
                End If

            ElseIf PaintSpielfeld_ErrorOccured Then

                PaintSpielfeld_AktPermission = False

                'sonst kommt die Meldung immer wieder
                PaintSpielfeld_ErrorOccured = False
                '
                If PaintSpielfeld_ShowErrorMessage AndAlso Not String.IsNullOrEmpty(PaintSpielfeld_ErrorMessage) Then

                    MsgBox(PaintSpielfeld_ErrorMessage.Replace("|", vbCrLf), MsgBoxStyle.Exclamation, "Fehlermeldung")

                End If

            End If

        End Sub



        Private _PaintSpielfeld_ErrorOccured As Boolean
        '
        ''' <summary>
        ''' Schaltet PaintSpielfeld_AktPermission auf False, d.h. es wird nicht
        ''' mehr neu gezeichnet.
        ''' Bewirkt, dass PaintSpielfeld_ShowErrorMessage abgefragt wird und ggf.
        ''' PaintSpielfeld_ErrorMessage links oben auf dem Spielfeld ausgegeben wird.
        ''' </summary>
        ''' <returns></returns>
        Public Property PaintSpielfeld_ErrorOccured As Boolean
            Get
                Return _PaintSpielfeld_ErrorOccured
            End Get
            Set(value As Boolean)
                _PaintSpielfeld_ErrorOccured = value
                If value Then
                    PaintSpielfeld_AktPermission = False
                End If
            End Set
        End Property
        '
        ''' <summary>
        ''' Dieser Text wird links oben auf dem Spielfeld anstelle des Spielfeldes
        ''' ausgegeben wenn PaintSpielfeld_ErrorOccured = True UND 
        ''' PaintSpielfeld_ShowErrorMessage = True sind.
        ''' </summary>
        ''' <returns></returns>
        Public Property PaintSpielfeld_ErrorMessage As String = Nothing
        '
        ''' <summary>
        ''' Siehe PaintSpielfeld_ErrorMessage
        ''' </summary>
        ''' <returns></returns>
        Public Property PaintSpielfeld_ShowErrorMessage As Boolean

        <DebuggerStepThrough>
        Private Sub RenderTimer_Tick(sender As Object, e As EventArgs) Handles RenderTimer.Tick
            If CurrentControl IsNot Nothing AndAlso Not CurrentControl.IsDisposed Then
                CurrentControl.Invalidate()
            End If
        End Sub

    End Module

End Namespace
