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

Imports System.Windows.Threading

Public Class winNotify

    Private tmrSTimer As DispatcherTimer = New DispatcherTimer()
    Private counter As Integer = 61


    Private Sub recBtnOk_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs) _
        Handles recBtnOk.MouseLeftButtonUp, lblCancel.MouseLeftButtonUp
        MainWindow.WinNotifyIcon.Visible = False
        End
    End Sub

    Private Sub winNotify_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized
        AddHandler tmrSTimer.Tick, AddressOf tmrSTimer_tick

    End Sub

    Private Sub tmrSTimer_tick()
        counter -= 1

        If counter < 0 Then

            WhatDoEvent()

        Else
            lblInfoText.Content = _
    "System will" & WhatDoStr & "automatically in " & counter & " second(s)." _
& vbNewLine & "To deactivate timer just click on Cancel."
        End If



    End Sub

    Private Sub winNotify_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        tmrSTimer.Interval = New TimeSpan(0, 0, 1)
        tmrSTimer.Start()
    End Sub
End Class
