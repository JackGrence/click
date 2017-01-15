Imports System.Xml, System.IO
Public Class Form1

    Declare Sub mouse_event Lib "user32" Alias "mouse_event" (ByVal dwFlags As Integer, ByVal dx As Integer, ByVal dy As Integer, ByVal cButtons As Integer, ByVal dwExtraInfo As Integer)
    Declare Sub GetCursorPos Lib "user32" (ByRef msp As Point)
    Declare Function GetAsyncKeyState Lib "user32" (ByVal key As Integer) As Short
    Private Const MOUSEEVENTF_LEFTDOWN = &H2
    Private Const MOUSEEVENTF_LEFTUP = &H4
    Private formpos As Point
    Private mouseDownPos As Point
    Private mspos As Point
    Private mousecatch As Boolean = False
    Private mouseSW As Boolean = False
    Private mouseUD As Boolean = False
    Private Dchange As Boolean = False
    Private Cchange As Boolean = False
    Private time As Integer = 0
    Private MD As Integer = 192
    Private MC As Integer = 20
    Private MD_KEY As String = "Oemtilde"
    Private MC_KEY As String = "Capital"
    Private speed As Integer = 100


    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        If GetAsyncKeyState(MD) Then 'f10 = 121
            GetCursorPos(mspos)
            waitkeyup(MD)
            If mousecatch Then
                mousecatch = False
                mouse_event(MOUSEEVENTF_LEFTUP, mspos.X, mspos.Y, 1, 0)
                Debug.Print("up")
            Else
                mousecatch = True
                mouse_event(MOUSEEVENTF_LEFTDOWN, mspos.X, mspos.Y, 1, 0)
                Debug.Print("down")
            End If
        ElseIf GetAsyncKeyState(MC) Then
            GetCursorPos(mspos)
            waitkeyup(MC)
            If mouseSW Then
                mouseSW = False
                mouseUD = False
                mouse_event(MOUSEEVENTF_LEFTUP, mspos.X, mspos.Y, 1, 0)
                Timer2.Enabled = False
            Else
                speed = Val(TextBox1.Text)
                If speed <= 1 Then
                    TextBox1.Text = 100
                    speed = 100
                    MsgBox("別亂來", , "別")
                    Return
                End If
                mouseSW = True
                mouseUD = True
                Timer2.Interval = speed / 2
                Timer2.Enabled = True
            End If
        End If




    End Sub


    Private Sub Timer2_Tick(sender As System.Object, e As System.EventArgs) Handles Timer2.Tick
        If mouseUD Then
            mouse_event(MOUSEEVENTF_LEFTDOWN, mspos.X, mspos.Y, 1, 0)
        Else
            mouse_event(MOUSEEVENTF_LEFTUP, mspos.X, mspos.Y, 1, 0)
        End If
        mouseUD = Not mouseUD
        'Debug.Print("mouseUD" & mouseUD)
    End Sub

    Private Sub cls()
        For i = 1 To 1000
            GetAsyncKeyState(121)
        Next

    End Sub

    Private Sub Form1_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim reader As XmlTextReader = New XmlTextReader("config.xml")
        Dim judge As String
        If File.Exists("config.xml") Then
            While (reader.Read())
                Select Case reader.NodeType
                    Case XmlNodeType.Element
                        judge = reader.Name
                    Case XmlNodeType.Text
                        Select Case judge
                            Case "x"
                                Me.Location = New Point(Int(reader.Value), Me.Location.Y)
                            Case "y"
                                Me.Location = New Point(Me.Location.X, Int(reader.Value))
                            Case "MD"
                                MD = Int(reader.Value)
                            Case "MC"
                                MC = Int(reader.Value)
                            Case "speed"
                                speed = Int(reader.Value)
                            Case "MD_KEY"
                                MD_KEY = reader.Value
                            Case "MC_KEY"
                                MC_KEY = reader.Value
                        End Select
                End Select

            End While
            MouseD.Text = MD_KEY
            MouseC.Text = MC_KEY
            TextBox1.Text = speed
            formpos = New Point(Me.Location)
            reader.Close()
        Else
            creatxml()
            Form1_Load(sender, e)
        End If
    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        creatxml()
        Me.Close()
    End Sub

    Private Sub creatxml()
        Dim st As String
        st = _
        "<?xml version=""1.0""?>" & Environment.NewLine & _
        "<form>" & Environment.NewLine & _
        "<pos>" & Environment.NewLine & _
        "<x>" & Me.Location.X & "</x>" & Environment.NewLine & _
        "<y>" & Me.Location.Y & "</y>" & Environment.NewLine & _
        "</pos>" & Environment.NewLine & _
        "<key>" & Environment.NewLine & _
        "<MD>" & MD & "</MD>" & Environment.NewLine & _
        "<MC>" & MC & "</MC>" & Environment.NewLine & _
        "<MD_KEY>" & MD_KEY & "</MD_KEY>" & Environment.NewLine & _
        "<MC_KEY>" & MC_KEY & "</MC_KEY>" & Environment.NewLine & _
        "<speed>" & speed & "</speed>" & Environment.NewLine & _
        "</key>" & Environment.NewLine & _
        "</form>"
        File.WriteAllText("config.xml", st)
    End Sub

    Private Sub waitkeyup(ByVal k As Integer)
        While (GetAsyncKeyState(k) <> 0)

        End While
    End Sub

    Private Sub Form1_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
        'mouseDownPos = New Point(MousePosition)
        'Debug.Print(mouseDownPos.X & "  " & mouseDownPos.Y)
    End Sub

    Private Sub Form1_MouseMove(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        Dim displacement As Point
        If Me.Capture Then
            displacement = e.Location - mouseDownPos
            'Debug.Print("displace" & displacement.ToString)
            Me.Location = formpos + displacement
        Else
            mouseDownPos = e.Location
            'Debug.Print("downpos" & mouseDownPos.ToString)
        End If
        formpos = Me.Location
    End Sub

    Private Sub Form1_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
        'formpos = Me.Location
    End Sub

    Private Sub MouseD_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles MouseD.KeyDown
        If Dchange Then
            MD = e.KeyCode
            MD_KEY = e.KeyCode.ToString
            MouseD.Text = MD_KEY
            Dchange = False
        End If
    End Sub

    Private Sub MouseC_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles MouseC.KeyDown
        If Cchange Then
            MC = e.KeyCode
            MC_KEY = e.KeyCode.ToString
            MouseC.Text = MC_KEY
            Cchange = False
        End If
    End Sub

    Private Sub DCkeyup(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles MouseD.KeyUp, MouseC.KeyUp
        waitkeyup(MC)
        waitkeyup(MD)
        If Dchange = False And Cchange = False Then
            Timer1.Enabled = True
        End If
    End Sub

    Private Sub MouseD_MouseClick(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles MouseD.MouseClick

        MouseD.Text = "請按下熱鍵"
        Dchange = True
        Timer1.Enabled = False
    End Sub

    Private Sub MouseC_Click(sender As Object, e As System.EventArgs) Handles MouseC.Click

        MouseC.Text = "請按下熱鍵"
        Cchange = True
        Timer1.Enabled = False
    End Sub


End Class
