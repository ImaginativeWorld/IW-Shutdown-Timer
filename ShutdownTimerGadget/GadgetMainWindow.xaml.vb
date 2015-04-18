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

Imports System.Windows.Media.Animation
Imports System.Windows.Media.Drawing
Imports System.Threading
Imports System.Windows.Threading

' :: Notes ::
' Line 109

Class GadgetMainWindow

    Private BlurEffectVer As Effects.BlurEffect = New Effects.BlurEffect

    'Timer
    Private tmrCheckUpdate As DispatcherTimer = New DispatcherTimer()
    Private tmrLikeUs As DispatcherTimer = New DispatcherTimer()

    'Effect
    ' Create a storyboard to contain the animations.
    Dim storyboard As New Storyboard()
    Dim duration As New TimeSpan(0, 0, 0, 0, 500)
    ' Create a DoubleAnimation to fade the not selected option control
    Dim animation As New DoubleAnimation()

    'Button Color
    Private recBtnColorInt As Integer = My.Settings.btnColorInt
    Private recBtnBrush As LinearGradientBrush

    'Show Confirmmation Window
    Private ShowConfirmationWindow As Boolean = My.Settings.ShowConfirm

    'Own Derectory
    Public OwnAppDir As String = My.Application.Info.DirectoryPath

    'Like Us
    Private LikeUsShow As Integer = My.Settings.LikeUsShow

    'Mutex
    Public STGm As New Mutex(False, "IWShutdownTimerGadget")



    Private Sub recMainBG_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs) Handles _
        recMainBG.MouseLeftButtonDown, GBtn.MouseLeftButtonDown
        If My.Settings.LockMove = False Then
            e.Handled = True
            Me.DragMove()
            ClosingTasks()
        End If
    End Sub

    Private Sub elpSwitch_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs) Handles _
        recSwitch.MouseLeftButtonUp

        If btnShutdown.IsEnabled Then
            btnShutdown.IsEnabled = False
            btnLock.IsEnabled = False
            btnLogOff.IsEnabled = False

            BlurEffectVer.Radius = 10
            BlurEffectVer.KernelType = Effects.KernelType.Gaussian
            BlurEffectVer.RenderingBias = Effects.RenderingBias.Performance
            GBtn.Effect = BlurEffectVer
            'recSwitch.Fill = New SolidColorBrush(Color.FromRgb(108, 226, 108))
            HideElements()

        Else

            btnShutdown.IsEnabled = True
            btnLock.IsEnabled = True
            btnLogOff.IsEnabled = True

            BlurEffectVer.Radius = 0
            BlurEffectVer.KernelType = Effects.KernelType.Gaussian
            BlurEffectVer.RenderingBias = Effects.RenderingBias.Performance
            GBtn.Effect = BlurEffectVer
            'recSwitch.Fill = New SolidColorBrush(Colors.Red)
            ShowElements()
        End If

    End Sub

    Private Sub GadgetMainWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        'Mutex
        'CreateMutex(0&, 0&, "IWShutdownTimerGadget")

        'Timer
        AddHandler tmrCheckUpdate.Tick, AddressOf tmrCheckUpdate_Tick
        AddHandler tmrLikeUs.Tick, AddressOf tmrLikeUs_Tick

        btnShutdown.IsEnabled = False
        btnLock.IsEnabled = False
        btnLogOff.IsEnabled = False
        GMain.Opacity = 0.25
        BlurEffectVer.Radius = 10
        BlurEffectVer.KernelType = Effects.KernelType.Gaussian
        BlurEffectVer.RenderingBias = Effects.RenderingBias.Performance
        GBtn.Effect = BlurEffectVer

        Me.Top = My.Settings.WinTop
        Me.Left = My.Settings.WinLeft

        If My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run", _
                                       "IW Shutdown Timer Gadget", "Not Found") = "Not Found" Then
            mnuAutorun.IsChecked = False
        Else
            mnuAutorun.IsChecked = True
        End If

        If My.Settings.LockMove Then
            mnuLock.IsChecked = True
        End If

        ChangeRecBtnColor()

        'Check Update
        tmrCheckUpdate.Interval = New TimeSpan(0, 5, 0) '(0, 5, 0)

        If LikeUsShow = False Then
            tmrLikeUs.Interval = New TimeSpan(0, 0, 30)
            tmrLikeUs.Start()
        End If

        If ShowConfirmationWindow Then
            mnuShowConfirmation.IsChecked = True
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

    Private Sub GadgetMainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        tmrCheckUpdate.Start()
    End Sub

    Private Sub tmrCheckUpdate_Tick()

        If UpdateTrmIvalCng Then
            If IsUpdateFirstTime Then
                IsUpdateFirstTime = False

                tmrCheckUpdate.Interval = New TimeSpan(1, 0, 0)

            End If
        End If


        If UpdateCheckStart = False Then
            Dim newThread As Thread = New Thread(AddressOf CheckUpdate)
            newThread.Name = "ST Update Check"
            newThread.IsBackground = True
            newThread.Start()

        Else
            If UpdateCheckComplete Then
                tmrCheckUpdate.Stop()

                If UpdateFound Then
                    Dim UpdateMsgBox As MsgBoxResult

                    UpdateMsgBox = MsgBox("New Version " & STNewVersion & " available!" & _
                                        vbNewLine & vbNewLine & _
                                        "Would you like to download new version now?", _
                                         MsgBoxStyle.Information + MsgBoxStyle.YesNo, _
                                         My.Application.Info.AssemblyName & " :: Update")

                    If UpdateMsgBox = MsgBoxResult.Yes Then
                        Process.Start(New ProcessStartInfo("http://" & STUDownload))
                    End If
                End If

            End If

        End If

    End Sub

    Private Sub ShowElements()
        GMain.Opacity = 0.25
        GMain.Visibility = Windows.Visibility.Visible
        animation.From = 0.25
        animation.To = 1.0
        animation.Duration = New Duration(duration)

        ' Configure the animation to target de property Opacity
        storyboard.SetTargetName(animation, GMain.Name)
        storyboard.SetTargetProperty(animation, New PropertyPath(Control.OpacityProperty))
        ' Add the animation to the storyboard
        storyboard.Children.Add(animation)

        'storyboard.RepeatBehavior = RepeatBehavior.Forever()
        'animation.AutoReverse = True

        ' Begin the storyboard
        storyboard.Begin(Me)
    End Sub

    Private Sub HideElements()
        GMain.Opacity = 1
        GMain.Visibility = Windows.Visibility.Visible
        animation.From = 1.0
        animation.To = 0.25
        animation.Duration = New Duration(duration)

        ' Configure the animation to target de property Opacity
        storyboard.SetTargetName(animation, GMain.Name)
        storyboard.SetTargetProperty(animation, New PropertyPath(Control.OpacityProperty))
        ' Add the animation to the storyboard
        storyboard.Children.Add(animation)

        'storyboard.RepeatBehavior = RepeatBehavior.Forever()
        'animation.AutoReverse = True

        ' Begin the storyboard
        storyboard.Begin(Me)
    End Sub

    Private Sub mnuExit_Click(sender As Object, e As RoutedEventArgs) Handles mnuExit.Click
        ClosingTasks()
        Me.Close()
    End Sub

    Private Sub btnShutdown_Click(sender As Object, e As RoutedEventArgs) _
        Handles btnShutdown.Click, btnLock.Click, btnLogOff.Click

        'Process.Start("shutdown", "-h -f -t 00") 'Hibernet

        ClosingTasks()

        Select Case sender.name
            Case "btnShutdown"
                DoWhatName = 1

            Case "btnLogOff"
                DoWhatName = 2

            Case "btnLock"
                DoWhatName = 3

        End Select

        If ShowConfirmationWindow Then

            Dim MsgWindow As Window = New GadgetMsgWindow

            MsgWindow.ShowDialog()

        Else

            DoWhatCommand()

        End If

    End Sub

    Public Sub ClosingTasks()
        My.Settings.WinTop = Me.Top
        My.Settings.WinLeft = Me.Left
        My.Settings.Save()
    End Sub

    Private Sub mnuAutorun_Checked(sender As Object, e As RoutedEventArgs) Handles mnuAutorun.Checked
        Try
            My.Computer.Registry.SetValue _
("HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run", _
"IW Shutdown Timer Gadget", """" & OwnAppDir & "\IW Shutdown Timer Gadget.exe""", _
Microsoft.Win32.RegistryValueKind.String)
        Catch ex As Exception
            ErrorMsg(ex)
        End Try

    End Sub

    Private Sub mnuAutorun_Unchecked(sender As Object, e As RoutedEventArgs) Handles mnuAutorun.Unchecked
        Try
            My.Computer.Registry.CurrentUser _
.OpenSubKey("Software\Microsoft\Windows\CurrentVersion\Run", True) _
.DeleteValue("IW Shutdown Timer Gadget", False)
        Catch ex As Exception
            ErrorMsg(ex)
        End Try
    End Sub


    Private Sub mnuLock_Checked(sender As Object, e As RoutedEventArgs) Handles mnuLock.Checked
        My.Settings.LockMove = True

    End Sub

    Private Sub mnuLock_Unchecked(sender As Object, e As RoutedEventArgs) Handles mnuLock.Unchecked
        My.Settings.LockMove = False
    End Sub

    Private Sub mnuCngColor_Click(sender As Object, e As RoutedEventArgs) Handles mnuCngColor.Click
        Select Case recBtnColorInt
            Case 0
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(51, 196, 255), Color.FromRgb(0, 168, 236), 90) 'Sky Blue
                My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case 1
                recBtnBrush = New LinearGradientBrush( _
 Color.FromRgb(44, 164, 207), Color.FromRgb(25, 112, 143), 90) 'Blue

                My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case 2
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(0, 109, 85), Color.FromRgb(0, 88, 68), 90) 'Deep Green

                My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case 3
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(0, 147, 115), Color.FromRgb(0, 120, 86), 90) 'Lite Deep Green 

                My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case 4
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(118, 163, 72), Color.FromRgb(50, 99, 1), 90) 'Green 

                My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case 5
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(152, 213, 12), Color.FromRgb(113, 164, 0), 90) 'Light Green

                My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case 6
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(235, 219, 49), Color.FromRgb(217, 175, 49), 90) 'Gold Yello

                My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case 7
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(255, 173, 0), Color.FromRgb(255, 117, 0), 90) 'Orange

                My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case 8
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(198, 49, 64), Color.FromRgb(131, 25, 35), 90) 'Black Red

                My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case 9
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(194, 194, 194), Color.FromRgb(172, 172, 172), 90) 'Gray

                My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case 10
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(102, 102, 102), Color.FromRgb(51, 51, 51), 90) 'Black

                My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case Else
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(145, 1, 174), Color.FromRgb(78, 2, 160), 90) 'Violet

                My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt = 0

        End Select

        recSwitch.Fill = recBtnBrush

    End Sub

    Private Sub ChangeRecBtnColor()
        Select Case recBtnColorInt
            Case 0
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(51, 196, 255), Color.FromRgb(0, 168, 236), 90) 'Sky Blue
                'My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case 1
                recBtnBrush = New LinearGradientBrush( _
 Color.FromRgb(44, 164, 207), Color.FromRgb(25, 112, 143), 90) 'Blue

                'My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case 2
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(0, 109, 85), Color.FromRgb(0, 88, 68), 90) 'Deep Green

                '  My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case 3
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(0, 147, 115), Color.FromRgb(0, 120, 86), 90) 'Lite Deep Green 

                '  My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case 4
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(118, 163, 72), Color.FromRgb(50, 99, 1), 90) 'Green 

                '  My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case 5
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(152, 213, 12), Color.FromRgb(113, 164, 0), 90) 'Light Green

                '  My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case 6
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(235, 219, 49), Color.FromRgb(217, 175, 49), 90) 'Gold Yello

                '  My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case 7
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(255, 173, 0), Color.FromRgb(255, 117, 0), 90) 'Orange

                '  My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case 8
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(198, 49, 64), Color.FromRgb(131, 25, 35), 90) 'Black Red

                '  My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case 9
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(194, 194, 194), Color.FromRgb(172, 172, 172), 90) 'Gray

                '  My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case 10
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(102, 102, 102), Color.FromRgb(51, 51, 51), 90) 'Black

                '  My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt += 1

            Case Else
                recBtnBrush = New LinearGradientBrush( _
Color.FromRgb(145, 1, 174), Color.FromRgb(78, 2, 160), 90) 'Violet

                '  My.Settings.btnColorInt = recBtnColorInt
                recBtnColorInt = 0

        End Select

        recSwitch.Fill = recBtnBrush

    End Sub

    Private Sub mnuAbout_Click(sender As Object, e As RoutedEventArgs) Handles mnuAbout.Click
        MsgBox("IW Shutdown Timer Gadget" & vbNewLine & _
               "Version: " & My.Application.Info.Version.ToString & vbNewLine & _
               vbNewLine & _
               "This product comes with IW Shutdown Timer." & vbNewLine & _
               "An imagination of Imaginative World." & vbNewLine & _
               "This is an Open-source project under MPL 2.0." & vbNewLine & _
               vbNewLine & _
                "Feedback : info@imaginativeworld.org" & vbNewLine & _
                "Website : imaginativeworld.org" & vbNewLine & _
                "Facebook : fb.com/Imaginative.World.BD", _
                MsgBoxStyle.Information, My.Application.Info.Title & " :: About")
    End Sub

    Private Sub mnuShowConfirmation_Checked(sender As Object, e As RoutedEventArgs) Handles mnuShowConfirmation.Checked
        ShowConfirmationWindow = True
        My.Settings.ShowConfirm = True
    End Sub

    Private Sub mnuShowConfirmation_Unchecked(sender As Object, e As RoutedEventArgs) Handles mnuShowConfirmation.Unchecked
        ShowConfirmationWindow = False
        My.Settings.ShowConfirm = False
    End Sub

End Class
