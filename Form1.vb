Imports System.Net
Imports System.Threading.Tasks
Imports CoreXC_Reborn.GMapTileHelperLib
Imports GMap.NET
Imports GMap.NET.WindowsForms
Imports GMap.NET.MapProviders


Public Class Form1

    Dim gmap As GMapControl
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AddHandler ChannelAnalyzer.SelectedIndexChanged, AddressOf TabControl_SelectedIndexChanged

    End Sub

    Private Sub GroupBox1_Enter(sender As Object, e As EventArgs)

    End Sub

    Private Sub TabPage3_Click(sender As Object, e As EventArgs)

    End Sub
    Private Async Sub TabControl_SelectedIndexChanged(sender As Object, e As EventArgs)

        If ChannelAnalyzer.SelectedTab Is TabPage3 OrElse ChannelAnalyzer.SelectedTab Is TabPage4 Then

            If (gmap Is Nothing) Then
                gmap = Await GMapHelper.CreateMapControl(0, 0, 10)
            End If
            If ChannelAnalyzer.SelectedTab Is TabPage3 Then
                GroupBox24.Controls.Add(gmap)
            End If

            If ChannelAnalyzer.SelectedTab Is TabPage4 Then
                GroupBox26.Controls.Add(gmap)
            End If

        End If
    End Sub
    Private Sub TabPage1_Click(sender As Object, e As EventArgs) Handles TabPage1.Click

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

    End Sub

    Private Sub Chart1_Click(sender As Object, e As EventArgs) Handles Chart1.Click

    End Sub

    Private Sub DataGridView4_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView4.CellContentClick

    End Sub

    Private Sub downloader1_Click(sender As Object, e As EventArgs) Handles downloader1.Click
        Dim folder = "C:\MyMap\Tiles"
        If Not IO.Directory.Exists(folder) Then IO.Directory.CreateDirectory(folder)

        'Thunderforest API key
        Dim apiKey = "9488d8f1f44142489555183db1934dc7"
        DownloadTiles(gmap.Position.Lat - 0.02, gmap.Position.Lng - 0.02, gmap.Position.Lat + 0.02, gmap.Position.Lng + 0.02, gmap.MinZoom, gmap.MaxZoom, folder, apiKey)

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
        ' Create a bitmap with the same size as the control
        Dim bmp As New Bitmap(gmap.Width, gmap.Height)

        ' Draw the control's content into the bitmap
        gmap.DrawToBitmap(bmp, New Rectangle(0, 0, gmap.Width, gmap.Height))

        ' Save the bitmap to a file (e.g., PNG)
        Dim saveFilePath As String = IO.Path.Combine(savePath, "map_view.png")
        bmp.Save(saveFilePath, System.Drawing.Imaging.ImageFormat.Png)

        MessageBox.Show("Map saved to: " & savePath)


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

        Dim folder = "C:\MyMap\screenshot"
        If Not IO.Directory.Exists(folder) Then IO.Directory.CreateDirectory(folder)
        CurrentDownloadTiles(gmap, folder)
    End Sub
End Class
