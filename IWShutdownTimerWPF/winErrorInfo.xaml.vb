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

Public Class ErrorInfoWindow

    Private tmrErrShowHide As DispatcherTimer = New DispatcherTimer()

    Private Sub recBtnOk_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs) _
        Handles recBtnOk.MouseLeftButtonUp, lblOK.MouseLeftButtonUp

        MainWindow.isErrorWindowShow = False

        tmrErrShowHide.Interval = New TimeSpan(0, 0, 0, 0, 10)
        tmrErrShowHide.Start()

    End Sub

    Private Sub ErrorInfoWindow_Initialized(sender As Object, e As EventArgs) Handles Me.Initialized

        AddHandler tmrErrShowHide.Tick, AddressOf tmrErrShowHide_Tick

        Me.Opacity = 0



    End Sub

    Private Sub tmrErrShowHide_Tick()
        If MainWindow.isErrorWindowShow = True Then
            If Me.Opacity < 1 Then
                Me.Opacity += 0.1
            Else
                Me.Opacity = 1
                MainWindow.isErrorWindowShow = False
                tmrErrShowHide.Stop()
            End If
        Else
            If Me.Opacity > 0 Then
                Me.Opacity -= 0.1
            Else
                Me.Opacity = 0
                MainWindow.isErrorWindowShow = True
                tmrErrShowHide.Stop()
                Me.Close()
            End If
        End If

    End Sub

    Private Sub ErrorInfoWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If MainWindow.ErrMsgType = 1 Then
            lblErrInfo.Content = MainWindow.ErrMsgStr & vbNewLine & vbNewLine & _
    "We are extremely sorry for this problem." & vbNewLine & _
    "Please let us know about this problem."
        Else
            lblErrInfo.Content = MainWindow.ErrMsgStr
        End If


        tmrErrShowHide.Interval = New TimeSpan(0, 0, 0, 0, 10)
        tmrErrShowHide.Start()
    End Sub
End Class
