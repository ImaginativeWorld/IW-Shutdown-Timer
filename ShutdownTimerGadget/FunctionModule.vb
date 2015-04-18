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

Module FunctionModule

    Dim MainWindow As Window = New GadgetMainWindow

    'Lock
    Public Declare Function LockWorkStation Lib "user32.dll" () As Long

    Public DoWhatName As Integer

    'Update
    Public STUDownload, STNewVersion As String

    Public UpdateCheckStart As Boolean = False
    Public UpdateCheckComplete As Boolean = False
    Public UpdateFound As Boolean = False
    Public UpdateTrmIvalCng As Boolean = False
    Public IsUpdateFirstTime As Boolean = True


    Public Sub CheckUpdate()

        UpdateCheckStart = True

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

                    UpdateFound = True

                End If

                STNewVersion = Content(0).ToString
                STUDownload = Content(1).ToString

                UpdateCheckComplete = True

            Catch Ex As Exception

                'UpdateError = True
                'STUpdateErr = Ex.ToString
                UpdateCheckStart = False
                UpdateTrmIvalCng = True
            End Try

        Else

            UpdateCheckStart = False
            UpdateTrmIvalCng = True
        End If


    End Sub

    Public Sub ErrorMsg(ByVal Exc As Exception)
        'Dim ex As New Exception

        MsgBox(Exc.ToString & _
       (Chr(13)) & _
       (Chr(13)) & _
       (Chr(13)) & "We are extremely sorry for this problem." & _
       (Chr(13)) & "Please let us know about this problem.", _
       vbCritical, MainWindow.Content & " :: Error")
    End Sub

    Public Sub DoWhatCommand()

        Select Case DoWhatName
            Case 1
                If Version.Parse(My.Computer.Info.OSVersion) >= Version.Parse("6.2.9200.0") Then
                    Process.Start("shutdown", "-s -Hybrid -f -t 0") 'Shutdown (Hybrid)
                Else
                    Process.Start("shutdown", "-s -f -t 0") 'Shutdown
                End If

                MainWindow.Close()

            Case 2
                Process.Start("shutdown", "-l -f") 'Log off

                MainWindow.Close()

            Case 3
                Process.Start("shutdown", "-r -t 0") 'Restart

                MainWindow.Close()

        End Select
    End Sub
End Module
