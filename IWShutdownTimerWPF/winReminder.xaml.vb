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

Public Class winReminder

    Private WithEvents tmrSound As DispatcherTimer = New DispatcherTimer()

    Private Sub btnStopTimer_Click(sender As Object, e As RoutedEventArgs) Handles btnStopTimer.Click
        tmrSound.Stop()
        mElement.Stop()

        Me.Hide()

    End Sub

    Private Sub winReminder_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        txtRInfoMsg.Text = MainWindow.SettedTime.ToString

        tmrSound.Interval = New TimeSpan(1000)

        mElement.Volume = SoundVolume

        If SoundFileDir <> Nothing Then

            Try
                Dim mUri As Uri = New Uri(SoundFileDir)
                mElement.Source = mUri

                mElement.Play()
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try

        End If

    End Sub

    Private Sub tmrSound_Tick(sender As Object, e As EventArgs) Handles tmrSound.Tick
        Try
            Dim mUri As Uri = New Uri(SoundFileDir)
            mElement.Source = mUri

            mElement.Play()
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try

        tmrSound.Stop()

    End Sub

    Private Sub mElement_MediaEnded(sender As Object, e As RoutedEventArgs) Handles mElement.MediaEnded
        tmrSound.Start()
    End Sub
End Class
