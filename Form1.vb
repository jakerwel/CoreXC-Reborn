Imports System.Net
Imports System.IO
Imports System.Threading.Tasks
Imports CoreXC_Reborn.GMapTileHelperLib
Imports GMap.NET
Imports GMap.NET.WindowsForms
Imports System.Net.NetworkInformation
Imports GMap.NET.MapProviders


Public Class Form1

    Dim gmap As GMapControl
    Dim appFolder As String = Application.StartupPath
    Private imageToDraw As Image = Nothing

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AddHandler ChannelAnalyzer.SelectedIndexChanged, AddressOf TabControl_SelectedIndexChanged
        appFolder += "\download"

    End Sub

    Private Sub GroupBox1_Enter(sender As Object, e As EventArgs)

    End Sub

    Private Sub TabPage3_Click(sender As Object, e As EventArgs)

    End Sub
    Private Async Sub TabControl_SelectedIndexChanged(sender As Object, e As EventArgs)

        If ChannelAnalyzer.SelectedTab Is TabPage3 OrElse ChannelAnalyzer.SelectedTab Is TabPage4 Then

            If IsOnlineWithPing() Then
                If (gmap Is Nothing) Then
                    gmap = Await GMapHelper.CreateMapControl(0, 0, 10)
                End If

                If Not (gmap Is Nothing) Then
                    If ChannelAnalyzer.SelectedTab Is TabPage3 Then
                        GroupBox24.Controls.Add(gmap)
                    End If

                    If ChannelAnalyzer.SelectedTab Is TabPage4 Then
                        GroupBox26.Controls.Add(gmap)
                    End If

                End If

            End If
        End If
    End Sub

    Public Function IsOnlineWithPing() As Boolean
        Try
            Dim ping As New Ping()
            Dim reply = ping.Send("8.8.8.8", 1000) ' Google's DNS
            Return reply.Status = IPStatus.Success
        Catch
            Return False
        End Try
    End Function
    Private Sub TabPage1_Click(sender As Object, e As EventArgs) Handles TabPage1.Click

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

    End Sub

    Private Sub Chart1_Click(sender As Object, e As EventArgs) Handles Chart1.Click

    End Sub

    Private Sub DataGridView4_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView4.CellContentClick

    End Sub

    Private Sub downloader1_Click(sender As Object, e As EventArgs) Handles downloader1.Click

        Dim folder = appFolder + "\tiles"
        If Not IO.Directory.Exists(folder) Then IO.Directory.CreateDirectory(folder)


        'Thunderforest API key
        Dim apiKey = "9488d8f1f44142489555183db1934dc7"
        DownloadTiles(gmap.Position.Lat - 0.02, gmap.Position.Lng - 0.02, gmap.Position.Lat + 0.02, gmap.Position.Lng + 0.02, gmap.MinZoom, gmap.MaxZoom, appFolder, apiKey)

    End Sub

    Private Function LatLngToTileXY(lat As Double, lon As Double, zoom As Integer) As (Integer, Integer)
        Dim latRad = lat * Math.PI / 180
        Dim n = Math.Pow(2, zoom)
        Dim x = CInt(Math.Floor((lon + 180.0) / 360.0 * n))
        Dim y = CInt(Math.Floor((1.0 - Math.Log(Math.Tan(latRad) + 1.0 / Math.Cos(latRad)) / Math.PI) / 2.0 * n))
        Return (x, y)
    End Function
    Private Sub DownloadTilesCache(lat As Double, lon As Double, zoom As Integer)
        GMaps.Instance.Mode = AccessMode.ServerAndCache

        ' Define small area around center
        Dim area = New RectLatLng(lat + 0.02, lon - 0.02, 0.04, 0.04)
        Dim tiles = gmap.MapProvider.Projection.GetAreaTileList(area, zoom, 0)

        For Each tile In tiles
            gmap.MapProvider.GetTileImage(tile, zoom)
        Next
    End Sub
    Private Sub CurrentDownloadTiles(gmap As GMap.NET.WindowsForms.GMapControl, savePath As String)

        Dim latValue As Double
        Dim lonValue As Double
        Dim fileName As String

        If Not (Double.TryParse(lat.Text, latValue)) Then
            latValue = 0
        End If
        If Not (Double.TryParse(lon.Text, lonValue)) Then
            lonValue = 0
        End If
        fileName = lat.Text + "-" + lon.Text + ".png"
        ' Create a bitmap with the same size as the control
        Dim bmp As New Bitmap(gmap.Width, gmap.Height)

        ' Draw the control's content into the bitmap
        gmap.DrawToBitmap(bmp, New Rectangle(0, 0, gmap.Width, gmap.Height))

        ' Save the bitmap to a file (e.g., PNG)
        Dim saveFilePath As String = IO.Path.Combine(savePath, fileName)
        bmp.Save(saveFilePath, System.Drawing.Imaging.ImageFormat.Png)

        MessageBox.Show("Map saved to: " & saveFilePath)


    End Sub
    Private Sub DownloadTiles(minLat As Double, minLon As Double, maxLat As Double, maxLon As Double, minZoom As Integer, maxZoom As Integer, saveFolder As String, apiKey As String)
        Dim client As New WebClient()
        Dim total As Integer = (maxZoom - minZoom + 1)
        Dim current As Integer = 0

        ProgressBar1.Minimum = 0
        ProgressBar1.Maximum = total
        ProgressBar1.Value = 0

        For Zoom = minZoom To maxZoom
            ' Get tile XY range for the bounding box at this zoom
            Dim result1 = LatLngToTileXY(maxLat, minLon, Zoom) ' maxLat for minY because Y axis inverted
            Dim result2 = LatLngToTileXY(minLat, maxLon, Zoom)
            Dim minX As Integer = result1.Item1
            Dim minY As Integer = result1.Item2
            Dim maxX As Integer = result2.Item1
            Dim maxY As Integer = result2.Item2

            current += 1
            ProgressBar1.Value = current
            For x = minX To maxX
                For y = minY To maxY
                    Try
                        Dim url = $"https://tile.thunderforest.com/outdoors/{Zoom}/{x}/{y}.png?apikey={apiKey}"
                        Dim filename = IO.Path.Combine(saveFolder, $"{Zoom}_{x}_{y}.png")
                        If Not IO.File.Exists(filename) Then
                            client.DownloadFile(url, filename)
                            Console.WriteLine($"Downloaded: {filename}")
                        End If
                    Catch ex As Exception
                        Console.WriteLine($"Failed to download tile {Zoom}/{x}/{y}: {ex.Message}")
                    End Try


                Next
            Next
        Next

    End Sub

    Private Sub Offlineview_Click(sender As Object, e As EventArgs) Handles ScreenShot.Click

        'Dim folder = "C:\MyMap\screenshot"
        If Not IO.Directory.Exists(appFolder) Then IO.Directory.CreateDirectory(appFolder)
        CurrentDownloadTiles(gmap, appFolder)
    End Sub

    Private Async Sub viewmap_Click(sender As Object, e As EventArgs) Handles viewmap.Click

        Dim latValue As Double
        Dim lonValue As Double

        GroupBox24.Controls.Remove(gmap)
        If Not (Double.TryParse(lat.Text, latValue)) Then
            latValue = 0
        End If
        If Not (Double.TryParse(lon.Text, lonValue)) Then
            lonValue = 0
        End If

        If IsOnlineWithPing() Then

            gmap = Await GMapHelper.CreateMapControl(latValue, lonValue, 10)
            GroupBox24.Controls.Add(gmap)
        Else

            Dim fileName = appFolder + "\" + lat.Text + "-" + lon.Text + ".png"

            If File.Exists(fileName) Then

                imageToDraw = Image.FromFile(fileName)
                GroupBox24.Invalidate()
            Else
                MsgBox("Now offline state and not found saved map file.")
            End If

        End If

    End Sub
    Private Sub GroupBox24_Paint(sender As Object, e As PaintEventArgs) Handles GroupBox24.Paint
        If imageToDraw IsNot Nothing Then
            Dim g As Graphics = e.Graphics
            Dim w As Integer = GroupBox24.ClientSize.Width - 10
            Dim h As Integer = GroupBox24.ClientSize.Height - 10
            g.DrawImage(imageToDraw, 5, 5, w, h)
        End If
    End Sub


End Class
