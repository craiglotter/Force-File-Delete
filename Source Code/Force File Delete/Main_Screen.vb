Imports System.IO
Public Class Main_Screen

    Private lastinputline As String = ""
    Private inputlines As Long = 0
    Private highestPercentageReached As Integer = 0
    Private inputlinesprecount As Long = 0
    Private pretestdone As Boolean = False
    Private primary_PercentComplete As Integer = 0
    Private percentComplete As Integer
    Private error_encountered As Boolean = False


    Private Cancelled As Boolean

    Private Sub Error_Handler(ByVal ex As Exception, Optional ByVal identifier_msg As String = "")
        Try
            If My.Computer.FileSystem.FileExists((Application.StartupPath & "\Sounds\UHOH.WAV").Replace("\\", "\")) = True Then
                My.Computer.Audio.Play((Application.StartupPath & "\Sounds\UHOH.WAV").Replace("\\", "\"), AudioPlayMode.Background)
            End If
            If ex.Message.IndexOf("Thread was being aborted") < 0 Then
                Dim Display_Message1 As New Display_Message()
                Display_Message1.Message_Textbox.Text = "The Application encountered the following problem: " & vbCrLf & identifier_msg & ":" & ex.Message.ToString
                Display_Message1.Timer1.Interval = 1000
                Display_Message1.ShowDialog()
                Dim dir As System.IO.DirectoryInfo = New System.IO.DirectoryInfo((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs")
                If dir.Exists = False Then
                    dir.Create()
                End If
                dir = Nothing
                Dim filewriter As System.IO.StreamWriter = New System.IO.StreamWriter((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs\" & Format(Now(), "yyyyMMdd") & "_Error_Log.txt", True)
                filewriter.WriteLine("#" & Format(Now(), "dd/MM/yyyy hh:mm:ss tt") & " - " & identifier_msg & ":" & ex.ToString)
                filewriter.Flush()
                filewriter.Close()
                filewriter = Nothing
            End If
        Catch exc As Exception
            MsgBox("An error occurred in the application's error handling routine. The application will try to recover from this serious error.", MsgBoxStyle.Critical, "Critical Error Encountered")
        End Try
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            Label5.Text = "Initializing Operation Variables"
            If OpenFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then
                Textbox1.Text = OpenFileDialog1.FileName

                Button1.Enabled = False
                Button2.Visible = True
                Button3.Visible = True
                Button2.Enabled = True
                Button3.Enabled = True
                Me.ControlBox = False
                Cancelled = False
                Label5.Text = "Operation Initiated"
                BackgroundWorker1.RunWorkerAsync()

            Else
                Label5.Text = "Operation Request Cancelled"
            End If

        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Private Sub DoWork(ByVal sender As Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Try

            ProgressBar1.Value = 0

            If Cancelled = True Then
                Exit Sub
            End If

            inputlinesprecount = 1
            inputlines = 0
            lastinputline = OpenFileDialog1.FileName

            ' Report progress as a percentage of the total task.
            percentComplete = 0
            If inputlinesprecount > 0 Then
                percentComplete = CSng(inputlines) / CSng(inputlinesprecount) * 100
            Else
                percentComplete = 100
            End If
            primary_PercentComplete = percentComplete
            If percentComplete > highestPercentageReached Then
                highestPercentageReached = percentComplete
                BackgroundWorker1.ReportProgress(percentComplete)
            End If


            Try
                Dim finfo As FileInfo
                finfo = New FileInfo(OpenFileDialog1.FileName)
                finfo.Delete()
                finfo = Nothing
            Catch ex As Exception
                error_encountered = True
                Error_Handler(ex)
            End Try
            


            inputlines = inputlines + 1
            ' Report progress as a percentage of the total task.
            percentComplete = 0
            If inputlinesprecount > 0 Then
                percentComplete = CSng(inputlines) / CSng(inputlinesprecount) * 100
            Else
                percentComplete = 100
            End If
            primary_PercentComplete = percentComplete
            If percentComplete > highestPercentageReached Then
                highestPercentageReached = percentComplete
                BackgroundWorker1.ReportProgress(percentComplete)
            End If






        Catch ex As Exception
            error_encountered = True
            Error_Handler(ex)
        End Try
    End Sub



    Private Sub ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        Try
            ProgressBar1.Value = e.ProgressPercentage
            Label5.Text = "Removed " & lastinputline
        Catch ex As Exception
            error_handler(ex)
        End Try
    End Sub

    Private Sub RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        Try
            If e.Cancelled = True Or Cancelled = True Then

                If My.Computer.FileSystem.FileExists((Application.StartupPath & "\Sounds\HEEY.WAV").Replace("\\", "\")) = True Then
                    My.Computer.Audio.Play((Application.StartupPath & "\Sounds\HEEY.WAV").Replace("\\", "\"), AudioPlayMode.Background)
                End If
                MsgBox("Operation Cancelled.")
                Button1.Enabled = True
                Button2.Visible = False
                Button3.Visible = False
                Button2.Enabled = False
                Button3.Enabled = False
                Me.ControlBox = True
                ProgressBar1.Value = 100
            Else
                If My.Computer.FileSystem.FileExists((Application.StartupPath & "\Sounds\VICTORY.WAV").Replace("\\", "\")) = True Then
                    My.Computer.Audio.Play((Application.StartupPath & "\Sounds\VICTORY.WAV").Replace("\\", "\"), AudioPlayMode.Background)
                End If

                Button1.Enabled = True
                Button2.Visible = False
                Button3.Visible = False
                Button2.Enabled = False
                Button3.Enabled = False
                Me.ControlBox = True
                If error_encountered = False Then
                    Label5.Text = "Operation Completed"
                    MsgBox("Operation Complete: " & vbCrLf & """" & Textbox1.Text & """ has been removed", MsgBoxStyle.Information, "Operation Complete")
                Else
                    ProgressBar1.Value = 100
                    Label5.Text = "Operation Failed"
                    MsgBox("Operation Failed: " & vbCrLf & """" & Textbox1.Text & """ could not be removed in the end. It is suggested that you restart your machine in order to try and force Windows to unlock the file.", MsgBoxStyle.Information, "Operation Failed")
                End If

            End If
        Catch ex As Exception
            Error_Handler(ex)
        End Try
    End Sub

    Private Sub Form1_Closing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        Try
            Label5.Text = "Application Shutdown Initiated"
            Dim finfo As FileInfo = New FileInfo(Textbox1.Text)
            If finfo.Exists = True Then
                My.Settings("SourceDirectory") = finfo.DirectoryName
            End If
            finfo = Nothing
            My.Settings.Save()
        Catch ex As Exception
            Error_Handler(ex, "Application Closed")
        End Try
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Me.Text = My.Application.Info.ProductName & " " & My.Application.Info.Version.Major & Format(My.Application.Info.Version.Minor, "00") & Format(My.Application.Info.Version.Build, "00") & "." & My.Application.Info.Version.Revision
            Control.CheckForIllegalCrossThreadCalls = False
            If Not My.Settings Is Nothing Then
                If My.Settings("SourceDirectory").ToString.Length > 0 Then
                    If My.Computer.FileSystem.DirectoryExists(My.Settings("SourceDirectory")) = True Then
                        OpenFileDialog1.InitialDirectory = My.Settings("SourceDirectory")
                    End If
                End If
                
            End If
            Label5.Text = "Application Successfully Loaded"
        Catch ex As Exception
            Error_Handler(ex, "Application Load")
        End Try

    End Sub

  

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Try
            Me.WindowState = FormWindowState.Minimized
        Catch ex As Exception
            Error_Handler(ex, "Minimize Window")
        End Try
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Try
            Button2.Enabled = False
            BackgroundWorker1.CancelAsync()
            Cancelled = True
            Label5.Text = "Operation Cancelled"
        Catch ex As Exception
            Error_Handler(ex, "Operation Cancel")
        End Try
    End Sub

    Private Sub Main_Screen_DragDrop(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles MyBase.DragDrop
        Try
            If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                Dim myfiles() As String
               
                myfiles = e.Data.GetData(DataFormats.FileDrop)
                If myfiles.Length > 0 Then
                    Dim finfo As FileInfo = New FileInfo(myfiles(0))
                    If finfo.Exists = True Then
                        Textbox1.Text = finfo.FullName
                        OpenFileDialog1.FileName = finfo.FullName

                        Button1.Enabled = False
                        Button2.Visible = True
                        Button3.Visible = True
                        Button2.Enabled = True
                        Button3.Enabled = True
                        Me.ControlBox = False
                        Cancelled = False
                        Label5.Text = "Operation Initiated"
                        BackgroundWorker1.RunWorkerAsync()
                    Else
                        MsgBox("Please note that only files are accepted by this application.", MsgBoxStyle.Information, "Invalid Drop")
                    End If
                    finfo = Nothing

                End If
               
            End If
        Catch ex As Exception
            Error_Handler(ex, "Main_Screen_DragDrop")
        End Try
    End Sub

    Private Sub Main_Screen_DragEnter(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles MyBase.DragEnter
        Try
            If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                e.Effect = DragDropEffects.All
            End If
        Catch ex As Exception
            Error_Handler(ex, "Main_Screen_DragEnter")
        End Try
    End Sub
End Class
