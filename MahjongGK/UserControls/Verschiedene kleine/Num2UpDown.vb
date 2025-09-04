Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

Imports System.ComponentModel

Public Class Num2UpDown
    Inherits UserControl

    Private Const HARD_MIN As Integer = 0
    Private Const HARD_MAX As Integer = 99

    Private ReadOnly _picLeft As New PictureBox()
    Private ReadOnly _lbl As New Label()
    Private ReadOnly _picRight As New PictureBox()

    Private _value As Integer = 0
    Private _minValue As Integer = HARD_MIN
    Private _maxValue As Integer = HARD_MAX

    Public Event ValueChanged()

    Public Sub New()
        MyBase.New()
        Me.MinimumSize = New Size(65, 24)
        Me.Size = New Size(65, 28)
        Me.DoubleBuffered = True
        Me.TabStop = True

        ' Pfeil links (dekrement)
        _picLeft.Size = New Size(16, 16)
        _picLeft.SizeMode = PictureBoxSizeMode.CenterImage
        _picLeft.Image = My.Resources.PfeilDn   ' beliebig: vorhandene Down-Grafik links
        AddHandler _picLeft.Click, Sub(_s, __) IncrementValue(-1)

        ' Label in der Mitte
        _lbl.AutoSize = False
        _lbl.TextAlign = ContentAlignment.MiddleCenter
        _lbl.Font = New Font("Segoe UI", 12.0F, FontStyle.Bold, GraphicsUnit.Point)
        _lbl.ForeColor = SystemColors.ControlText
        _lbl.BackColor = Color.Transparent
        _lbl.Text = FormatTwoDigits(_value)
        AddHandler _lbl.MouseWheel, AddressOf OnMouseWheelForwardFocus
        AddHandler _lbl.Click, Sub(_s, __) Me.Focus()

        ' Pfeil rechts (inkrement)
        _picRight.Size = New Size(16, 16)
        _picRight.SizeMode = PictureBoxSizeMode.CenterImage
        _picRight.Image = My.Resources.PfeilUp   ' vorhandene Up-Grafik rechts
        AddHandler _picRight.Click, Sub(_s, __) IncrementValue(+1)



        Controls.Add(_picLeft)
        Controls.Add(_lbl)
        Controls.Add(_picRight)


        PerformLayout()
    End Sub

    <Browsable(True), Category("Behavior"), DefaultValue(0)>
    Public Property Value As Integer
        Get
            Return _value
        End Get
        Set(ByVal v As Integer)
            ' zuerst hart clampen auf 0..99, dann auf Min..Max
            v = Math.Min(HARD_MAX, Math.Max(HARD_MIN, v))
            v = Math.Min(_maxValue, Math.Max(_minValue, v))
            _value = v
            _lbl.Text = FormatTwoDigits(_value)
            RaiseEvent ValueChanged() ' IMMER feuern – auch bei gleicher Zuweisung
        End Set
    End Property

    <Browsable(True), Category("Behavior"), DefaultValue(HARD_MIN)>
    Public Property MinValue As Integer
        Get
            Return _minValue
        End Get
        Set(value As Integer)
            ValidateBound(value, NameOf(MinValue))
            If value > _maxValue Then Throw New ArgumentOutOfRangeException(NameOf(MinValue), "MinValue darf nicht größer als MaxValue sein.")
            _minValue = value
            ' Value ggf. anpassen (setzt und feuert Event, falls sich ändert)
            If _value < _minValue Then Me.Value = _minValue Else Me.Value = _value ' immer Event
        End Set
    End Property

    <Browsable(True), Category("Behavior"), DefaultValue(HARD_MAX)>
    Public Property MaxValue As Integer
        Get
            Return _maxValue
        End Get
        Set(value As Integer)
            ValidateBound(value, NameOf(MaxValue))
            If value < _minValue Then Throw New ArgumentOutOfRangeException(NameOf(MaxValue), "MaxValue darf nicht kleiner als MinValue sein.")
            _maxValue = value
            ' Value ggf. anpassen (setzt und feuert Event, falls sich ändert)
            If _value > _maxValue Then Me.Value = _maxValue Else Me.Value = _value ' immer Event
        End Set
    End Property

    Private Shared Sub ValidateBound(b As Integer, paramName As String)
        If b < HARD_MIN OrElse b > HARD_MAX Then
            Throw New ArgumentOutOfRangeException(paramName, $"Wert muss zwischen {HARD_MIN} und {HARD_MAX} liegen.")
        End If
    End Sub

    ' Tastatur: Links = -, Rechts = +
    Protected Overrides Function IsInputKey(keyData As Keys) As Boolean
        If keyData = Keys.Up OrElse keyData = Keys.Down Then Return True
        Return MyBase.IsInputKey(keyData)
    End Function

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        MyBase.OnKeyDown(e)
        If e.KeyCode = Keys.Down Then
            IncrementValue(-1) : e.Handled = True
        ElseIf e.KeyCode = Keys.up Then
            IncrementValue(+1) : e.Handled = True
        End If
    End Sub

    ' Mausrad (wenn fokussiert)
    Protected Overrides Sub OnMouseWheel(e As MouseEventArgs)
        MyBase.OnMouseWheel(e)
        If Me.Focused Then IncrementValue(If(e.Delta > 0, +1, -1))
    End Sub

    Private Sub OnMouseWheelForwardFocus(sender As Object, e As MouseEventArgs)
        OnMouseWheel(e)
    End Sub

    Private Sub IncrementValue(stepBy As Integer)
        Dim newVal As Integer = _value + stepBy
        Me.Value = newVal ' Setter übernimmt Clamping & Event
        Me.Focus()
    End Sub

    Private Shared Function FormatTwoDigits(n As Integer) As String
        If n < HARD_MIN Then n = HARD_MIN
        If n > HARD_MAX Then n = HARD_MAX
        Return n.ToString("00")
    End Function

    Protected Overrides Sub OnLayout(levent As LayoutEventArgs)
        MyBase.OnLayout(levent)
        Dim w As Integer = Me.ClientSize.Width
        Dim h As Integer = Me.ClientSize.Height

        Dim centerY As Integer = (h - _picLeft.Height) \ 2
        _picLeft.Location = New Point(2, Math.Max(0, centerY) + 1)
        _picRight.Location = New Point(w - _picRight.Width - 2, Math.Max(0, centerY) - 1)

        Dim lblLeft As Integer = _picLeft.Right
        Dim lblRight As Integer = _picRight.Left
        Dim lblWidth As Integer = Math.Max(0, lblRight - lblLeft)
        _lbl.Bounds = New Rectangle(lblLeft, 0, lblWidth, h)
    End Sub

    Protected Overrides Sub OnPaint(pe As PaintEventArgs)
        MyBase.OnPaint(pe)
        If Me.Focused Then ControlPaint.DrawFocusRectangle(pe.Graphics, Me.ClientRectangle)
    End Sub

End Class
