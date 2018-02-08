Imports System.Text.RegularExpressions
Imports System.Runtime.InteropServices
Imports System.IO

Public Class Form1


    Public SavePath As String = "C:\Stanza Typewriter\Saves\"

    Dim panelBounds As Rectangle
    Public isExplorer = True

    Private ListViewHwnd As Integer = -1

    Private ListViewButtonList As List(Of Button) = New List(Of Button)()

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Panel1.Top = Me.Size.Height - 200
        Dim AppName As String = My.Application.Info.AssemblyName
        Dim Root As String = "HKEY_CURRENT_USER\"
        Dim Key As String = "Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION"
        Dim CurrentSetting As String = CStr(Microsoft.Win32.Registry.CurrentUser.OpenSubKey(Key).GetValue(AppName & ".exe"))

        Microsoft.Win32.Registry.SetValue(Root & Key, AppName & ".exe", 11001)
        Microsoft.Win32.Registry.SetValue(Root & Key, AppName & ".vshost.exe", 11001)




        'get explorer directories
        For Each dics As String In My.Computer.FileSystem.GetDirectories(SavePath)
            ListBox1.Items.Add(dics.Replace(SavePath, ""))
        Next
        ListBox1.SelectedIndex = 0
    End Sub

    Private Sub Form1_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If isExplorer = True Then
            tmrExplandExplorer.Enabled = True
        End If

    End Sub





    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Panel1.Top = Me.Size.Height - 200
    End Sub

    Private Sub tmrExplandExplorer_Tick(sender As Object, e As EventArgs) Handles tmrExplandExplorer.Tick
        If isExplorer = True Then
            Panel1.Location = New Point(Panel1.Location.X, Panel1.Location.Y + 5)
            If Panel1.Location.Y >= Me.Size.Height - 70 Then
                isExplorer = False
                tmrExplandExplorer.Enabled = False
            End If
        Else
            Panel1.Location = New Point(Panel1.Location.X, Panel1.Location.Y - 5)

            If Panel1.Location.Y <= Me.Size.Height - 200 Then
                isExplorer = True
                tmrExplandExplorer.Enabled = False
            End If
        End If
    End Sub



    Private Sub ListBox1_DrawItem(sender As Object, e As DrawItemEventArgs) Handles ListBox1.DrawItem
        ' e.DrawBackground()




        Dim isItemSelected As Boolean = ((e.State And DrawItemState.Selected) = DrawItemState.Selected)
        Dim itemIndex As Integer = e.Index

        If itemIndex >= 0 AndAlso itemIndex < ListBox1.Items.Count Then
            Dim g As Graphics = e.Graphics
            Dim backgroundColorBrush As SolidBrush = New SolidBrush(If((isItemSelected), Color.Lime, Color.Black))

            g.FillRectangle(backgroundColorBrush, e.Bounds)
            Dim itemText As String = ListBox1.Items(itemIndex).ToString()
            Dim itemTextColorBrush As SolidBrush = If((isItemSelected), New SolidBrush(Color.White), New SolidBrush(Color.Lime))
            g.DrawString(itemText, e.Font, itemTextColorBrush, ListBox1.GetItemRectangle(itemIndex).Location)
            backgroundColorBrush.Dispose()
            itemTextColorBrush.Dispose()
        End If

        e.DrawFocusRectangle()
    End Sub

    Private Sub ListBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles ListBox1.MouseMove
        Dim item As Integer
        item = ListBox1.IndexFromPoint(New Point(e.X, e.Y))
        ListBox1.SelectedIndex = item
    End Sub



    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        ListView1.Items.Clear()
        For Each file As String In My.Computer.FileSystem.GetFiles(SavePath & "/" & ListBox1.SelectedItem)

            'modified date

            Dim t As TimeSpan = DateTime.Now - System.IO.File.GetLastWriteTime(file.ToString())
            Dim modifieddate As String = "Null"
            If t.TotalSeconds < 60 Then
                modifieddate = "Less than a minute ago"
            ElseIf t.TotalMinutes < 60 Then
                modifieddate = t.Minutes.ToString() + " minutes ago"
            ElseIf t.Hours < 24 Then
                modifieddate = t.Hours.ToString() + " hours ago"
            ElseIf t.Days < 7 Then
                modifieddate = t.Days.ToString() + " days ago"
            Else
                modifieddate = System.IO.File.GetLastWriteTime(file.ToString())
            End If






            ' Reading text from a file
            Dim text = System.IO.File.ReadAllText(file)
            Dim words = text.Split(" "c)
            Dim wordCount = words.Length
            Dim sentences = text.Split("."c, "!"c, "?"c)
            Dim sentenceCount = sentences.Length

            Dim str(5) As String
            Dim itm As ListViewItem
            str(0) = (file.Replace(SavePath & ListBox1.SelectedItem & "\", "")).Replace(".txt", "")
            str(1) = modifieddate
            str(2) = wordCount - 2
            str(3) = "🔒"
            str(4) = "✖"

            itm = New ListViewItem(str)
            ListView1.Items.Insert(0, itm)
        Next

    End Sub

    Private Sub listView1_DrawColumnHeader(ByVal sender As Object, ByVal e As DrawListViewColumnHeaderEventArgs) Handles ListView1.DrawColumnHeader

        Dim strFormat As New StringFormat()


        If e.Header.TextAlign = HorizontalAlignment.Center Then
            strFormat.Alignment = StringAlignment.Center
        ElseIf e.Header.TextAlign = HorizontalAlignment.Right Then
            strFormat.Alignment = StringAlignment.Far
        End If

        e.DrawBackground()
        e.Graphics.FillRectangle(Brushes.Black, e.Bounds)
        Dim headerFont As New Font("Microsoft Sans Serif", 8, FontStyle.Regular)

        e.Graphics.DrawString(e.Header.Text, headerFont, Brushes.Lime, e.Bounds, strFormat)




    End Sub

    Private Sub ListView1_DrawItem(sender As Object, e As DrawListViewItemEventArgs) Handles ListView1.DrawItem
        ' e.DrawDefault = True

        If e.Item.Selected = True Then
            e.Graphics.FillRectangle(Brushes.Lime, e.Bounds)
            e.DrawText(TextFormatFlags.TextBoxControl)
            e.Item.ForeColor = Color.White
        Else
            e.Graphics.FillRectangle(Brushes.Black, e.Bounds)
            e.DrawText(TextFormatFlags.TextBoxControl)
            e.Item.ForeColor = Color.Lime
        End If


    End Sub
    Public Function CountWords(ByVal value As String) As Integer
        ' Count matches.
        Dim collection As MatchCollection = Regex.Matches(value, "\S+")
        Return collection.Count
    End Function
    Function RemoveWhitespace(fullString As String) As String
        Return New String(fullString.Where(Function(x) Not Char.IsWhiteSpace(x)).ToArray())
    End Function
    Private Sub tmrStatus_Tick(sender As Object, e As EventArgs) Handles tmrStatus.Tick
        Dim myElement As HtmlElement
        myElement = WebBrowser1.Document.GetElementById("editor")
        Dim text As String = myElement.GetAttribute("value")

        Dim WordCound As String = CountWords(text)
        Dim CharCount As String = RemoveWhitespace(text).Length


        lblCharCount.Text = "| Characters : " & CharCount & " | Words : " & WordCound & " |"
    End Sub

    Private Sub Panel1_MouseMove(sender As Object, e As MouseEventArgs) Handles Panel1.MouseMove
        If isExplorer = True Then
            tmrExplandExplorer.Enabled = True
        Else
            tmrExplandExplorer.Enabled = True
        End If
    End Sub
    '  Private previousListViewItem As ListViewItem = Nothing

    Private Sub ListView1_DrawSubItem(sender As Object, e As DrawListViewSubItemEventArgs) Handles ListView1.DrawSubItem
        If e.Item.Selected = True Then
            e.Graphics.FillRectangle(Brushes.Lime, e.Bounds)
            e.DrawText(TextFormatFlags.TextBoxControl)
            e.SubItem.ForeColor = Color.White

        Else
            e.Graphics.FillRectangle(Brushes.Black, e.Bounds)
            e.DrawText(TextFormatFlags.TextBoxControl)
            e.SubItem.ForeColor = Color.Lime
        End If

    End Sub


    Private Sub ListView1_ItemMouseHover(sender As Object, e As ListViewItemMouseHoverEventArgs) Handles ListView1.ItemMouseHover
        'If previousListViewItem IsNot Nothing Then
        '    previousListViewItem.ForeColor = Color.White
        '    previousListViewItem.BackColor = Color.Lime
        'End If

        e.Item.ForeColor = Color.FromKnownColor(KnownColor.White)
        e.Item.BackColor = Color.FromKnownColor(KnownColor.Lime)

        'previousListViewItem = e.Item


    End Sub

    Private Sub ListView1_MouseUp(sender As Object, e As MouseEventArgs) Handles ListView1.MouseUp
        Try
            Dim hti As ListViewHitTestInfo = ListView1.HitTest(e.Location)
            Dim subitemindex As Integer = hti.Item.SubItems.IndexOf(hti.SubItem)
            Dim val As String = hti.Item.SubItems(subitemindex).Text
            If Not subitemindex = Nothing Then
                If subitemindex.ToString = "4" Then 'delete
                    If ListBox1.SelectedItem.ToString = "Trash" Then 'trash can
                        Try
                            My.Computer.FileSystem.DeleteFile(SavePath & "Trash\" & ListView1.SelectedItems(0).Text & ".txt")
                        Catch ex As Exception
                            MsgBox(ex.Message)
                        End Try
                    Else
                        Try
                            '    My.Computer.FileSystem.DeleteFile()
                            My.Computer.FileSystem.MoveFile(SavePath & ListBox1.SelectedItem.ToString & "\" & ListView1.SelectedItems(0).Text & ".txt", SavePath & "Trash\" & ListView1.SelectedItems(0).Text & "@" & ListBox1.SelectedItem.ToString & ".txt")
                        Catch ex As Exception
                            MsgBox(ex.Message)
                        End Try
                    End If
                End If
            Else
                If subitemindex.ToString = "4" Then 'delete
                    If ListBox1.SelectedItem.ToString = "Trash" Then 'trash can
                        Try
                            My.Computer.FileSystem.DeleteFile(SavePath & "Trash\" & ListView1.SelectedItems(0).Text & ".txt")
                        Catch ex As Exception
                            MsgBox(ex.Message)
                        End Try
                    Else
                        Try
                            '    My.Computer.FileSystem.DeleteFile()
                            My.Computer.FileSystem.MoveFile(SavePath & ListBox1.SelectedItem.ToString & "\" & ListView1.SelectedItems(0).Text & ".txt", SavePath & "Trash\" & ListView1.SelectedItems(0).Text & "@" & ListBox1.SelectedItem.ToString & ".txt")
                        Catch ex As Exception
                            MsgBox(ex.Message)
                        End Try
                    End If
                End If
            End If

        Catch ex As Exception

        End Try
        Dim x = ListBox1.SelectedIndex
        If Not x = 0 Then
            ListBox1.SelectedIndex = 0
        Else
            ListBox1.SelectedIndex = 1
        End If

        ListBox1.SelectedIndex = x

    End Sub


    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged

    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click
        Dim myElement As HtmlElement
        myElement = WebBrowser1.Document.GetElementById("editor")
        Dim text As String = myElement.GetAttribute("value")
        If Not text = "" Then
            If MsgBox("Do you need to save?", MsgBoxStyle.YesNoCancel) = MsgBoxResult.Yes Then


                Dim strxFile As String
                If text.Length > 25 Then
                    strxFile = SavePath & ListBox1.SelectedItem.ToString & "\" & text.Substring(0, 25).Replace("~", "").Replace("#", "").Replace("%", "").Replace("&", "").Replace("*", "").Replace("{", "").Replace("}", "").Replace("\", "").Replace(":", "").Replace("<", "").Replace(">", "").Replace("?", "").Replace("/", "").Replace("+", "").Replace("|", "").Replace("""", "") & ".txt"
                Else
                    strxFile = SavePath & ListBox1.SelectedItem.ToString & "\" & text.Replace("~", "").Replace("#", "").Replace("%", "").Replace("&", "").Replace("*", "").Replace("{", "").Replace("}", "").Replace("\", "").Replace(":", "").Replace("<", "").Replace(">", "").Replace("?", "").Replace("/", "").Replace("+", "").Replace("|", "").Replace("""", "") & ".txt"
                End If


                Dim fileExists As Boolean = File.Exists(strxFile)
                Using sw As New StreamWriter(File.Open(strxFile, FileMode.OpenOrCreate))
                    sw.WriteLine( _
                        IIf(fileExists, _
                            "Error Message in  Occured at-- " & DateTime.Now, _
                            "Start Error Log for today"))
                End Using



            End If

        End If
    End Sub
End Class


