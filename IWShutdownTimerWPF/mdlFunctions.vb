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

Imports System.Windows.Forms

Module mdlFunctions
    'Error Code: 2x001WDE (001. WhatDoEvent)
    'Error code: 2x003NDA (003. NothingDoAnything)

    Public WhatDoStr As String

    Public UpdateAvailable As Boolean = False
    Public UpdateError As Boolean = False

    Public STUDownload, STNewVersion, STUpdateErr As String
    Public UpdateCheckComplete As Boolean = False

    'Sound file
    Public SoundFileDir As String
    Public SoundVolume As Double
    'Lock
    Public Declare Function LockWorkStation Lib "user32.dll" () As Long

    Public Sub ErrorMsg(ByVal Exc As Exception)
        'Dim ex As New Exception

        MsgBox(Exc.ToString & _
       (Chr(13)) & _
       (Chr(13)) & _
       (Chr(13)) & "We are extremely sorry for this problem." & _
       (Chr(13)) & "Please let us know about this problem.", _
       vbCritical, My.Application.Info.AssemblyName & " :: Error")
    End Sub


    Public Sub CheckUpdate()

        If My.Computer.Network.IsAvailable Then

            Dim sfile As String = "latestver.iwconf"
            Dim URL As String = "http://imaginativeworld.org/update/iwst/"
            'Dim URL As String = "http://localhost/update/iwst/" 'For test

            Dim myWebClient As New System.Net.WebClient

            Try
                Dim file As New System.IO.StreamReader(myWebClient.OpenRead(URL & sfile))

                Dim Contents As String = file.ReadToEnd()
                file.Close()
                Dim Content() As String = Contents.Split("|")

                '0.0.0.0|Dl Link

                '(0) = Application Version
                '(1) = Download Link

                If Version.Parse(Content(0)) > My.Application.Info.Version Then

                    UpdateAvailable = True

                Else

                    UpdateAvailable = False

                End If

                STNewVersion = Content(0).ToString
                STUDownload = Content(1).ToString


            Catch Ex As Exception

                UpdateError = True
                STUpdateErr = Ex.ToString

            End Try

        Else
            UpdateError = True
        End If

        UpdateCheckComplete = True

    End Sub



    Public Sub WhatDoEvent()
        Try
            Select Case MainWindow.DoWhatInt
                Case 1
                    If Version.Parse(My.Computer.Info.OSVersion) >= Version.Parse("6.2.9200.0") Then
                        Process.Start("shutdown", "-s -Hybrid -f -t 00") 'Shutdown (Hybrid)
                    Else
                        Process.Start("shutdown", "-s -f -t 00") 'Shutdown
                    End If
                Case 2
                    Process.Start("shutdown", "-h") 'Hibernet
                Case 3
                    Process.Start("shutdown", "-r -f -t 00") 'Restart
                Case 4
                    Process.Start("shutdown", "-l -f") 'Log off
                Case 5
                    LockWorkStation() 'Lock

                Case Else

                    DoErrMsgWindowShow("Somthing worong! Error Code: 2x003NDA", 1)

            End Select

        Catch ex As Exception
            DoErrMsgWindowShow("Somthing worong! Error Code: 2x001WDE", 1)
        End Try

        End

    End Sub


    Public Sub DoErrMsgWindowShow(Msg As String, Type As Integer)

        Dim ErrWindow As Window = New ErrorInfoWindow
        MainWindow.isErrorWindowShow = True
        MainWindow.ErrMsgType = Type
        MainWindow.ErrMsgStr = Msg
        ErrWindow.ShowDialog()

    End Sub


End Module
