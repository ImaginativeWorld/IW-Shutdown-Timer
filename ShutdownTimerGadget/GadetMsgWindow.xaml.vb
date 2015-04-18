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

Imports System.Threading
Imports System.Windows.Threading
Imports System.Windows.Media.Animation

Public Class GadgetMsgWindow

    'Mutex
    Public STGm As New Mutex(False, "IWShutdownTimerGadget")

    'Effect
    ' Create a storyboard to contain the animations.
    Dim storyboard As New Storyboard()
    Dim duration As New TimeSpan(0, 0, 0, 0, 250)
    ' Create a DoubleAnimation to fade the not selected option control
    Dim animation As New DoubleAnimation()

    Private tmrErrShowHide As DispatcherTimer = New DispatcherTimer()

    Private Sub lblNo_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs) Handles lblNo.MouseLeftButtonUp

        UnloadingEffect()

        tmrErrShowHide.Interval = New TimeSpan(0, 0, 0, 0, 10)
        tmrErrShowHide.Start()
    End Sub

    Private Sub GYes_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs) Handles GYes.MouseLeftButtonUp

        DoWhatCommand()

        Me.Close()

    End Sub

    Private Sub GadetMsgWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        AddHandler tmrErrShowHide.Tick, AddressOf tmrErrShowHide_Tick

    End Sub

    Private Sub UnloadingEffect()
        animation.From = 1.0
        animation.To = 0.0
        animation.Duration = New Duration(duration)

        ' Configure the animation to target de property Opacity
        storyboard.SetTargetName(animation, GadgetMsgWindow.Name)
        storyboard.SetTargetProperty(animation, New PropertyPath(Control.OpacityProperty))
        ' Add the animation to the storyboard
        storyboard.Children.Add(animation)

        'storyboard.RepeatBehavior = RepeatBehavior.Forever()
        'animation.AutoReverse = True

        ' Begin the storyboard
        storyboard.Begin(Me)
    End Sub

    Private Sub tmrErrShowHide_Tick()

        If GadgetMsgWindow.Opacity = 0 Then
            tmrErrShowHide.Stop()
            Me.Close()
        End If

    End Sub
End Class
