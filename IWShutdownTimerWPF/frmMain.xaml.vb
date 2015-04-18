''' <aboutDev>
''' 
''' Project:
'''     IW Shutdown Timer
'''
''' Documentation, Graphics and Coding:
'''     Md. Mahmudul Hasan Shohag
'''     Founder, CEO of Imaginative World
'''     http://shohag.imaginativeworld.org
'''     
''' Lisence:
'''     Opensource project lisense under MPL 2.0.
'''     Copyright © Imaginative World. All rights researved.
'''     http://imaginativeworld.org
''' 
''' **************************************************
'''     This Source Code Form is subject to the
'''     terms of the Mozilla Public License, v.
'''     2.0. If a copy of the MPL was not
'''     distributed with this file, You can obtain
'''     one at http://mozilla.org/MPL/2.0/.
''' **************************************************
''' 
''' </aboutDev>

Imports System
Imports System.Windows.Threading 'For Timer
Imports System.Threading
Imports System.Drawing
Imports System.Windows.Media.Animation
Imports System.Net

' dotNET Fremework 4 Client Profile Download Link :
' http://www.microsoft.com/en-us/download/details.aspx?id=24872

Class MainWindow

    'Own Derectory
    Public OwnAppDir As String = My.Application.Info.DirectoryPath

    'Main Timer
    Private dt As DispatcherTimer = New DispatcherTimer()

    'App close Effect Timer
    Private FormCloseEffectTimer As DispatcherTimer = New DispatcherTimer()

    'Update Check Timer
    Private tmrCheckUpdate As DispatcherTimer = New DispatcherTimer()

    'Like Us Giving Timer
    Private tmrLikeUs As DispatcherTimer = New DispatcherTimer()

    'Box Change Motion Timer
    Private tmrEffectMotion As DispatcherTimer = New DispatcherTimer()

    'Highresulation Stopwatch Timer
    Dim StopWatch As New Diagnostics.Stopwatch

    'Stopwatch tick view timer
    Private WithEvents tmrStopWatch As New System.Timers.Timer

    'Variables
    Private SetTimeH, SetTimeM As Integer
    Private StopWatchStr As Integer
    Private TimerMode As Integer = 0
    Private HourInt, MinuteInt As Integer
    Public Shared DoWhatInt As Integer = 1
    Private IsShowAbout As Boolean = False
    Private isAboutShowing As Boolean = False
    Private intSWhour As Integer = 0
    Private intSWmin As Integer = 0
    Private intSWsec As Integer = 0
    Private intSWnenosec As Integer = 0

    'Like Us
    Private LikeUsShow As Boolean = My.Settings.LikeUsShow

    'Setted Time
    Public Shared SettedTime As String

    'NotifyIcon
    Public Shared WinNotifyIcon As System.Windows.Forms.NotifyIcon = New System.Windows.Forms.NotifyIcon()

    Private OpacityInt As Double = 0
    Public IsShow As Boolean

    'Error Window
    Private winnotify As Window = New winNotify
    Public Shared isErrorWindowShow As Boolean
    Public Shared ErrMsgStr As String = "Something Worong! Err Code: 2x002UNK" '2 = Error no. 2; UNK = Unknown
    Public Shared ErrMsgType As Integer

    'Reminder Window
    Private ReminderWindow As Window = New winReminder

    'Effect
    ' Create a storyboard to contain the animations.
    Dim storyboard As New Storyboard()
    Dim duration As New TimeSpan(0, 0, 0, 0, 250)

    Dim EffectGridName As String

    ' Create a DoubleAnimation to fade the not selected option control
    Dim animation As New DoubleAnimation()

    'Update
    Public Shared IsSenderButton As Boolean = False

    'Mutex
    Public STm As New Mutex(False, "IWShutdownTimer")

    'Open Dlg
    Private OpenDlg As System.Windows.Forms.OpenFileDialog = New Forms.OpenFileDialog

    'Sound file
    Private SoundFileName As String

    Private Sub MainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized


        AddHandler dt.Tick, AddressOf dispatcherTimer_Tick
        AddHandler tmrStopWatch.Elapsed, AddressOf tmrStopWatch_Tick
        'AddHandler EffectTimer.Tick, AddressOf EffectTimer_Tick
        AddHandler FormCloseEffectTimer.Tick, AddressOf FromCloseEffectTimer_Tick
        AddHandler tmrCheckUpdate.Tick, AddressOf tmrCheckUpdate_Tick
        AddHandler tmrLikeUs.Tick, AddressOf tmrLikeUs_Tick

        AddHandler WinNotifyIcon.Click, AddressOf WinNotifyIcon_Click

        AddHandler tmrEffectMotion.Tick, AddressOf tmrEffectMotion_Tick

        'Show-Hide
        GridSettingBox.Visibility = Windows.Visibility.Hidden
        GridAboutBox.Visibility = Windows.Visibility.Hidden
        GStopWatch.Visibility = Windows.Visibility.Hidden
        GridBoxOptions.Visibility = Windows.Visibility.Hidden


        GridClockB.IsEnabled = False
        GridTimerB.IsEnabled = False

        txtTimerMag.Visibility = Windows.Visibility.Hidden

        'Change Version
        txtVersion.Content = "Version : " & My.Application.Info.Version.ToString
        txtTitleVer.Content = My.Application.Info.Version.ToString

        'Desktop Gadget
        If My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run", _
                                      "IW Shutdown Timer Gadget", "Not Found").ToString = "Not Found" Then

            Dim BColor As SolidColorBrush = New SolidColorBrush(Colors.Red)
            lblGStatus.Foreground = BColor
            lblGStatus.Content = "OFF"
        Else
            Dim BColor As SolidColorBrush = New SolidColorBrush(Colors.GreenYellow)
            lblGStatus.Foreground = BColor
            lblGStatus.Content = "ON"
        End If

        'NotifyIcon
        WinNotifyIcon.Icon = My.Resources.ST_Icon_02_01_16_32_48
        WinNotifyIcon.Text = Me.Title & vbNewLine & txtTimerInfo.Content.ToString
        WinNotifyIcon.Visible = True

        'Check Update
        tmrCheckUpdate.Interval = New TimeSpan(0, 0, 5)

        Dim newThread As Thread = New Thread(AddressOf CheckUpdate)
        newThread.IsBackground = True
        newThread.Name = "IW Shutdown Timer Update Check"
        newThread.Start()

        If LikeUsShow = False Then
            tmrLikeUs.Interval = New TimeSpan(0, 0, 30)
            tmrLikeUs.Start()
        End If

        ' Stop Watch
        Dim BrushColor As SolidColorBrush = New SolidColorBrush(Colors.Gray)
        lblFullTime.Foreground = BrushColor
        lblnanoSec.Foreground = BrushColor


    End Sub


    Private Sub lblGStatus_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs) Handles lblGStatus.MouseLeftButtonUp
        If lblGStatus.Content.ToString = "ON" Then

            Try
                My.Computer.Registry.CurrentUser _
    .OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run", True) _
    .DeleteValue("IW Shutdown Timer Gadget", False)
            Catch ex As Exception
                ErrorMsg(ex)
            End Try

            Dim BColor As SolidColorBrush = New SolidColorBrush(Colors.Red)
            lblGStatus.Foreground = BColor
            lblGStatus.Content = "OFF"
        Else

            Try
                My.Computer.Registry.SetValue _
    ("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run", _
    "IW Shutdown Timer Gadget", """" & OwnAppDir & "\IW Shutdown Timer Gadget.exe""", _
    Microsoft.Win32.RegistryValueKind.String)
            Catch ex As Exception
                ErrorMsg(ex)
            End Try

            Dim BColor As SolidColorBrush = New SolidColorBrush(Colors.GreenYellow)
            lblGStatus.Foreground = BColor
            lblGStatus.Content = "ON"
        End If
    End Sub

    Private Sub tmrLikeUs_Tick()
        If My.Computer.Network.IsAvailable Then
            Dim MsgBoxRslt As MsgBoxResult
            MsgBoxRslt = MsgBox("Thank you for using " & My.Application.Info.AssemblyName & "." & _
                                vbNewLine & _
                                vbNewLine & _
                                "Feel free to Feedback us : info@imaginativeworld.org" & _
                                vbNewLine & vbNewLine & _
                                "If you have time Like us on Facebook." & _
                                vbNewLine & "Do you want to go our Facebook Page now?" & _
                                vbNewLine & "(www.facebook.com/Imaginative.World.BD)", _
                                MsgBoxStyle.Information + MsgBoxStyle.YesNo, _
                                My.Application.Info.AssemblyName & " :: Thank you")

            If MsgBoxRslt = MsgBoxResult.Yes Then
                Process.Start(New ProcessStartInfo( _
                              "http://www.facebook.com/Imaginative.World.BD"))
            End If

            My.Settings.LikeUsShow = True
            My.Settings.Save()

            tmrLikeUs.Stop()
        End If
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        tmrCheckUpdate.Start()
    End Sub


    Private Sub btnSetTmr_Click(sender As Object, e As RoutedEventArgs) Handles btnSetTmr.Click
        If Not TimerMode = 0 Then

            Select Case DoWhatInt
                Case 1
                    WhatDoStr = " Shutdown "
                Case 2
                    WhatDoStr = " Hibernate "
                Case 3
                    WhatDoStr = " Restart "
                Case 4
                    WhatDoStr = " Log off / Sign Out "
                Case 5
                    WhatDoStr = " Locked "

            End Select

            If rBtnClockBase.IsChecked Then
                TimerMode = 1
                If cmbBoxAP.SelectionBoxItem.ToString = "PM" Then
                    If txtH.Text < 12 Then
                        HourInt = txtH.Text + 12
                    Else
                        HourInt = txtH.Text
                    End If

                Else
                    If txtH.Text = 12 Then
                        HourInt = 0
                    Else
                        HourInt = txtH.Text
                    End If

                End If

                If txtM.Text = 60 Then
                    MinuteInt = 0
                Else
                    MinuteInt = txtM.Text
                End If

                SetTimeH = HourInt
                SetTimeM = MinuteInt

                dt.Interval = New TimeSpan(0, 0, 1)

                dt.Start()

                If DoWhatInt = 6 Then

                    lblTimerDes.Content = "Reminder alarm time set to " _
                        & txtH.Text & ":" & txtM.Text & " " & cmbBoxAP.SelectionBoxItem.ToString

                Else
                    lblTimerDes.Content = "Your Computer will" & WhatDoStr & "at " _
                        & txtH.Text & ":" & txtM.Text & " " & cmbBoxAP.SelectionBoxItem.ToString

                End If


            ElseIf rBtnTimerBase.IsChecked Then
                TimerMode = 2

                StopWatchStr = txtTime.Text * 60

                dt.Interval = New TimeSpan(0, 0, 1)
                dt.Start()

                lblTimerDes.Content = ""


            ElseIf rBtnOptNetOff.IsChecked Then
                TimerMode = 3

                StopWatchStr = 0

                dt.Interval = New TimeSpan(0, 0, 1)
                dt.Start()

                lblTimerDes.Content = ""


                If DoWhatInt = 6 Then

                    lblTimerDes.Content = _
                        "Reminder alarm will notify you when your internet disconnected."

                Else
                    lblTimerDes.Content = _
                        "Your Computer will" & WhatDoStr & "when your internet disconnected."

                End If

            End If

            'Button Events
            showGrid(GridMsg.Name, GridBoxOptions.Name)

            btnShowHide.Content = "Stop Timer"

            btnAbout.IsEnabled = True
            btnStopWatch.IsEnabled = True

            If txtRemindMsg.Text = "Massage Here" Then
                SettedTime = "No massage!"
            Else
                SettedTime = txtRemindMsg.Text
            End If

            txtTimerMag.Text = SettedTime
            txtTimerMag.Visibility = Windows.Visibility.Visible

            txtTimerInfo.Content = "Timer is Activated"

            WinNotifyIcon.Text = Me.Title & vbNewLine & txtTimerInfo.Content
        Else

            DoErrMsgWindowShow("Set All filled correctely!", 2)

        End If

    End Sub

    Private Sub dispatcherTimer_Tick()
        If TimerMode = 1 Then
            If SetTimeH = TimeOfDay.Hour Then
                If SetTimeM = TimeOfDay.Minute Then

                    Me.Visibility = Windows.Visibility.Hidden
                    If DoWhatInt = 6 Then
                        ChangeMainWinDefault()
                        ReminderWindow.ShowDialog()
                    Else
                        WinNotifyIcon.Visible = False
                        winnotify.ShowDialog()
                    End If

                    dt.Stop()
                End If
            End If

        ElseIf TimerMode = 2 Then

            If StopWatchStr <= 0 Then

                Me.Visibility = Windows.Visibility.Hidden

                If DoWhatInt = 6 Then
                    ChangeMainWinDefault()
                    ReminderWindow.ShowDialog()

                Else
                    WinNotifyIcon.Visible = False
                    winnotify.ShowDialog()
                End If

                dt.Stop()
            Else
                StopWatchStr -= 1


                Dim convertintoInt As TimeSpan = New TimeSpan(0, 0, StopWatchStr)

                If DoWhatInt = 6 Then
                    lblTimerDes.Content = "Reminder alarm time left " & _
                    convertintoInt.Minutes & " minute(s) and " & convertintoInt.Seconds & " second(s)"
                Else
                    lblTimerDes.Content = "Your Computer will be" & WhatDoStr & "after " & _
    convertintoInt.Minutes & " minute(s) and " & convertintoInt.Seconds & " second(s)"

                End If

            End If

        ElseIf TimerMode = 3 Then

            If My.Computer.Network.IsAvailable = False Then

                Me.Visibility = Windows.Visibility.Hidden

                If DoWhatInt = 6 Then
                    ChangeMainWinDefault()
                    ReminderWindow.ShowDialog()
                Else
                    WinNotifyIcon.Visible = False
                    winnotify.ShowDialog()
                End If

                dt.Stop()

            End If


        End If


    End Sub

    Private Sub btnShowHide_Click(sender As Object, e As RoutedEventArgs) Handles btnShowHide.Click
        If btnShowHide.Content.ToString = "Cancel" Then
            'IsShow = False
            GridSettingBox.Visibility = Windows.Visibility.Hidden
            showGrid(GridMsg.Name, GridBoxOptions.Name)

            btnShowHide.Content = "Set Timer"

            btnAbout.IsEnabled = True
            btnStopWatch.IsEnabled = True

        ElseIf btnShowHide.Content.ToString = "Set Timer" Then
            'IsShow = True

            showGrid(GridSettingBox.Name, GridMsg.Name)

            btnShowHide.Content = "Cancel"

            btnAbout.IsEnabled = False
            btnStopWatch.IsEnabled = False

        ElseIf btnShowHide.Content.ToString = "Stop Timer" Then
            dt.Stop()
            ChangeMainWinDefault()

        End If

    End Sub

    Public Sub ChangeMainWinDefault()
        txtTimerMag.Visibility = Windows.Visibility.Hidden
        btnShowHide.Content = "Set Timer"
        txtTimerInfo.Content = "Timer is not Activated"
        lblTimerDes.Content = "To setup Timer click 'Set Timer' button"
        WinNotifyIcon.Text = Me.Title & vbNewLine & txtTimerInfo.Content.ToString
    End Sub


    Private Sub btnAbout_Click(sender As Object, e As RoutedEventArgs) Handles btnAbout.Click

        If GridAboutBox.Visibility = Windows.Visibility.Hidden Then

            btnStopWatch.IsEnabled = False
            btnShowHide.IsEnabled = False

            showGrid(GridAboutBox.Name, GridMsg.Name)

        Else

            showGrid(GridMsg.Name, GridAboutBox.Name)

            btnStopWatch.IsEnabled = True
            btnShowHide.IsEnabled = True

        End If

    End Sub

    Private Sub showGrid(ShowElementName As String, HideElementName As String)
        FindName(HideElementName).Visibility = Windows.Visibility.Hidden

        FindName(ShowElementName).Margin = New Thickness(35, 75, 7, 75)
        FindName(ShowElementName).Visibility = Windows.Visibility.Visible
        FindName(ShowElementName).Opacity = 0.1
        EffectGridName = ShowElementName
        tmrEffectMotion.Interval = New TimeSpan(0, 0, 0, 0, 1)
        tmrEffectMotion.Start()
    End Sub

    Private Sub btnExit_Click(sender As Object, e As RoutedEventArgs) Handles btnExit.Click

        WinNotifyIcon.Visible = False

        FormCloseEffectTimer.Interval = New TimeSpan(0, 0, 0, 0, 10)
        FormCloseEffectTimer.Start()

    End Sub

    Private Sub FromCloseEffectTimer_Tick()
        If GridMain.Opacity > 0 Then
            GridMain.Opacity -= 0.1
        Else
            Me.Close()
        End If

    End Sub

    Private Sub rBtnClockBase_Checked(sender As Object, e As RoutedEventArgs) _
        Handles rBtnClockBase.Checked, rBtnTimerBase.Checked, rBtnOptNetOff.Checked

        Select Case sender.name.ToString
            Case "rBtnClockBase"
                TimerMode = 1
                GridClockB.IsEnabled = True
                GridTimerB.IsEnabled = False

            Case "rBtnTimerBase"
                TimerMode = 2
                GridClockB.IsEnabled = False
                GridTimerB.IsEnabled = True

            Case "rBtnOptNetOff"
                TimerMode = 3
                GridClockB.IsEnabled = False
                GridTimerB.IsEnabled = False

        End Select

    End Sub


    Private Sub cmbBoxWhatDo_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cmbBoxWhatDo.SelectionChanged

        Select Case cmbBoxWhatDo.SelectedIndex
            Case 0 '"Shutdown"
                DoWhatInt = 1

            Case 1 '"Hibernet"
                DoWhatInt = 2

            Case 2 '"Restart"
                DoWhatInt = 3

            Case 3 '"Log off"
                DoWhatInt = 4

            Case 4 '"Lock"
                DoWhatInt = 5

            Case 5 '"Reminder"
                DoWhatInt = 6

        End Select

        If DoWhatInt = 6 Then
            btnSoundBrowse.IsEnabled = True
            cmbBoxVolume.IsEnabled = True
            btnPlay.IsEnabled = True
        Else
            btnSoundBrowse.IsEnabled = False
            cmbBoxVolume.IsEnabled = False
            btnPlay.IsEnabled = False
        End If

    End Sub

    Private Sub txtH_LostFocus(sender As Object, e As RoutedEventArgs) Handles txtH.LostFocus
        If txtH.Text = "" Then

            txtH.Text = "12"
        Else
            If Not IsNumeric(txtH.Text) Then
                DoErrMsgWindowShow("Invalid Hour!", 2)

                txtH.Text = "12"
            Else

                If txtH.Text > 12 Then
                    DoErrMsgWindowShow("Shutdown Timer Only support 12 Hour system.", 2)

                    txtH.Text = "12"
                End If
            End If
        End If
    End Sub


    Private Sub txtM_LostFocus(sender As Object, e As RoutedEventArgs) Handles txtM.LostFocus
        If txtM.Text = "" Then
            txtM.Text = "00"
        Else
            If Not IsNumeric(txtM.Text) Then
                DoErrMsgWindowShow("Invalid Minutes!", 2)

                txtM.Text = "00"
            Else
                If txtM.Text > 60 Then
                    DoErrMsgWindowShow("Invalid Minutes!", 2)

                    txtM.Text = "00"
                End If
            End If
        End If


    End Sub



    Public Sub WinNotifyIcon_Click(sender As Object, e As EventArgs)
        If Me.Visibility = Windows.Visibility.Hidden Then
            Me.Visibility = Windows.Visibility.Visible

            Me.Activate()
        End If
    End Sub

    Private Sub btnGotoTray_Click(sender As Object, e As RoutedEventArgs) Handles btnGotoTray.Click
        Me.Visibility = Windows.Visibility.Hidden
    End Sub


    Private Sub GridTitle_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs) Handles GridTitle.MouseLeftButtonDown
        e.Handled = True
        Me.DragMove()
    End Sub

    Private Sub imgFB_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs) _
        Handles imgFB.MouseLeftButtonDown, imgGPlus.MouseLeftButtonDown

    End Sub

    Private Sub imgFB_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs) _
        Handles imgFB.MouseLeftButtonUp, imgGPlus.MouseLeftButtonUp, _
        imgTwitter.MouseLeftButtonUp, imgWeb.MouseLeftButtonUp

        Select Case sender.name.ToString
            Case "imgFB"
                Process.Start(New ProcessStartInfo("https://www.facebook.com/Imaginative.World.BD"))
            Case "imgGPlus"
                Process.Start(New ProcessStartInfo("http://plus.google.com/109977063486458536095/"))
            Case "imgTwitter"
                Process.Start(New ProcessStartInfo("http://www.twitter.com/IW_BD"))
            Case "imgWeb"
                Process.Start(New ProcessStartInfo("http://imaginativeworld.org"))
        End Select

        e.Handled = True
    End Sub

    Private Sub txtTime_LostFocus(sender As Object, e As RoutedEventArgs) Handles txtTime.LostFocus
        If txtTime.Text = "" Then
            txtTime.Text = "5"
        Else
            If Not IsNumeric(txtTime.Text) Then
                txtTime.Text = "5"
            End If
        End If
    End Sub


    Private Sub Hyperlink_RequestNavigate(ByVal sender As Object, ByVal e As RequestNavigateEventArgs)

        Process.Start(New ProcessStartInfo(e.Uri.AbsoluteUri))

        e.Handled = True

    End Sub

    Private Sub btnChkUpdate_Click(sender As Object, e As RoutedEventArgs) Handles btnChkUpdate.Click

        btnChkUpdate.IsEnabled = False

        'Check Update
        Dim newThread As Thread = New Thread(AddressOf CheckUpdate)
        newThread.IsBackground = True
        newThread.Name = "IW Shutdown Timer Update Check"
        newThread.Start()

        IsSenderButton = True

        tmrCheckUpdate.Start()

    End Sub

    Private Sub tmrCheckUpdate_Tick()
        Dim winupdateinfo As Window = New winUpdateInfo

        If UpdateCheckComplete Then
            If UpdateError = False Then
                If UpdateAvailable Then

                    winupdateinfo.ShowDialog()
                Else
                    If IsSenderButton Then
                        winupdateinfo.ShowDialog()
                        IsSenderButton = False
                    End If
                End If
            Else
                If IsSenderButton Then
                    DoErrMsgWindowShow("Error! Check your Internet Connection.", 1)
                    IsSenderButton = False
                End If

            End If

            UpdateCheckComplete = False

            tmrCheckUpdate.Stop()

            btnChkUpdate.IsEnabled = True
        End If
    End Sub



    Private Sub btnSoundBrowse_Click(sender As Object, e As RoutedEventArgs) Handles btnSoundBrowse.Click
        OpenDlg.Reset()
        OpenDlg.Multiselect = False
        OpenDlg.Title = "Select a Sound file"
        OpenDlg.InitialDirectory = _
            My.Computer.FileSystem.SpecialDirectories.ProgramFiles.First & _
               ":\Windows\Media"

        OpenDlg.ShowDialog()

        If OpenDlg.FileName <> "" Then
            If OpenDlg.FileName.EndsWith(".mp3") Or _
                OpenDlg.FileName.EndsWith(".wav") Or _
                OpenDlg.FileName.EndsWith(".mid") Then


                SoundFileDir = OpenDlg.FileName
                SoundFileName = OpenDlg.SafeFileName

                If OpenDlg.FileName.EndsWith(".mp3") Then
                    lblSound.Content = "MP3 File"
                ElseIf OpenDlg.FileName.EndsWith(".wav") Then
                    lblSound.Content = "WAV File"
                ElseIf OpenDlg.FileName.EndsWith(".mid") Then
                    lblSound.Content = "MID File"
                End If


            Else
                MsgBox("Only support MP3, MID and WAV sound files.")
            End If
        Else
            SoundFileDir = Nothing
            SoundFileName = Nothing
            lblSound.Content = "No sound"
        End If

    End Sub

    Private Sub btnPlay_Click(sender As Object, e As RoutedEventArgs) Handles btnPlay.Click
        If btnPlay.Content.ToString = "▶" Then
            If SoundFileDir <> Nothing Then
                Try
                    Dim mUri As Uri = New Uri(SoundFileDir)
                    mElement.Source = mUri
                    mElement.Play()

                    btnPlay.Content = "■"

                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try

            Else
                MsgBox("No sound file selected!")
            End If

        Else
            mElement.Stop()
            btnPlay.Content = "▶"
        End If

    End Sub

    Private Sub mElement_MediaEnded(sender As Object, e As RoutedEventArgs) Handles mElement.MediaEnded
        btnPlay.Content = "▶"
    End Sub

    Private Sub cmbBoxVolume_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles cmbBoxVolume.SelectionChanged
        Select Case cmbBoxVolume.SelectedIndex
            Case 0 'Full
                SoundVolume = 1
                mElement.Volume = SoundVolume
            Case 1 'Medium
                SoundVolume = 0.5
                mElement.Volume = SoundVolume
            Case 2 'Low
                SoundVolume = 0.25
                mElement.Volume = SoundVolume
        End Select

    End Sub


    Private Sub tmrStopWatch_Tick()

        Dispatcher.BeginInvoke(New Action(AddressOf UpdateTimerTxt))

    End Sub

    Private Sub UpdateTimerTxt()
        Dim elapsed As TimeSpan = Me.StopWatch.Elapsed

        lblFullTime.Content = String.Format("{0:00}:{1:00}:{2:00}", elapsed.Hours, elapsed.Minutes, elapsed.Seconds)

    End Sub

    Private Sub btnSWStart_Click(sender As Object, e As RoutedEventArgs) Handles btnSWStart.Click

        If btnSWStart.Content.ToString = "Start" Then
            btnSWStart.Content = "Stop"
            Dim BrushColor As SolidColorBrush = New SolidColorBrush(Colors.Gray)
            lblnanoSec.Foreground = BrushColor

            BrushColor = New SolidColorBrush(Colors.White)
            lblFullTime.Foreground = BrushColor

            tmrStopWatch.Interval = 50
            tmrStopWatch.Start()
            Me.StopWatch.Start()

        Else
            btnSWStart.Content = "Start"
            tmrStopWatch.Stop()
            Me.StopWatch.Stop()
            Dim elapsed As TimeSpan = Me.StopWatch.Elapsed
            lblFullTime.Content = String.Format("{0:00}:{1:00}:{2:00}", elapsed.Hours, elapsed.Minutes, elapsed.Seconds)
            Dim BrushColor As SolidColorBrush = New SolidColorBrush(Colors.White)
            lblnanoSec.Foreground = BrushColor
            lblnanoSec.Content = String.Format("{0:00}", elapsed.Milliseconds / 10)
        End If


    End Sub

    Private Sub btnSWReset_Click(sender As Object, e As RoutedEventArgs) Handles btnSWReset.Click
        ResetStopWatch()
    End Sub

    Private Sub ResetStopWatch()
        tmrStopWatch.Stop()
        StopWatch.Reset()

        Dim BrushColor As SolidColorBrush = New SolidColorBrush(Colors.Gray)
        lblFullTime.Foreground = BrushColor
        lblFullTime.Content = "00:00:00"
        lblnanoSec.Foreground = BrushColor
        lblnanoSec.Content = "00"

        btnSWStart.Content = "Start"
    End Sub

    Private Sub btnStopWatch_Click(sender As Object, e As RoutedEventArgs) Handles btnStopWatch.Click
        If btnStopWatch.Content.ToString = "Stopwatch" Then
            btnStopWatch.Content = "Back"

            showGrid(GStopWatch.Name, GridMsg.Name)

            btnShowHide.IsEnabled = False
            btnAbout.IsEnabled = False
        Else

            btnStopWatch.Content = "Stopwatch"

            showGrid(GridMsg.Name, GStopWatch.Name)

            btnShowHide.IsEnabled = True
            btnAbout.IsEnabled = True

        End If
    End Sub

    Private Sub btnNext01_Click(sender As Object, e As RoutedEventArgs) Handles btnNext01.Click

        showGrid(GridBoxOptions.Name, GridSettingBox.Name)

    End Sub

    Private Sub tmrEffectMotion_Tick()

        If FindName(EffectGridName).Margin.Left > 21 Then
            FindName(EffectGridName).Margin =
                New Thickness(FindName(EffectGridName).Margin.Left - 2, 75,
                              FindName(EffectGridName).Margin.Right + 2, 75)
            FindName(EffectGridName).Opacity += 0.1
        Else
            FindName(EffectGridName).Opacity = 1
            tmrEffectMotion.Stop()
        End If

    End Sub

    Private Sub btnBack01_Click(sender As Object, e As RoutedEventArgs) Handles btnBack01.Click

        GridBoxOptions.Visibility = Windows.Visibility.Hidden

        GridSettingBox.Margin = New Thickness(35, 75, 7, 75)
        GridSettingBox.Visibility = Windows.Visibility.Visible
        GridSettingBox.Opacity = 0.1
        EffectGridName = "GridSettingBox"
        tmrEffectMotion.Interval = New TimeSpan(0, 0, 0, 0, 1)
        tmrEffectMotion.Start()

    End Sub
End Class
