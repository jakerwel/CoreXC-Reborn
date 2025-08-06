Imports GMap.NET
Imports GMap.NET.MapProviders
Imports GMap.NET.WindowsForms
Imports CoreXC_Reborn.GMapThunderforestWinForms
Imports System.Net
Imports System.Text.Json


Namespace GMapTileHelperLib
    Public Class GMapHelper

        Public Shared Async Function CreateMapControl(lat As Double, lon As Double, zoom As Integer) As Task(Of GMapControl)
            Dim gmap As New GMapControl With {
                .Dock = DockStyle.Fill,
                .MapProvider = ThunderforestProvider.Instance,
                .MinZoom = 1,
                .MaxZoom = 20,
                .Zoom = zoom
            }

            Await CenterMapFromIP(gmap, lat, lon)
            GMaps.Instance.Mode = AccessMode.ServerOnly

            Return gmap
        End Function

        Private Shared Async Function CenterMapFromIP(gmap As GMapControl, lat As Double, lon As Double) As Task
            Dim _lat As Double = lat
            Dim _lon As Double = lon

            Try
                If _lat = 0.0 AndAlso _lon = 0.0 Then
                    Using client As New WebClient()
                        Dim json As String = Await client.DownloadStringTaskAsync("http://ip-api.com/json")
                        Dim doc As JsonDocument = JsonDocument.Parse(json)
                        _lat = doc.RootElement.GetProperty("lat").GetDouble()
                        _lon = doc.RootElement.GetProperty("lon").GetDouble()
                    End Using
                End If

                gmap.Position = New PointLatLng(_lat, _lon)

            Catch ex As Exception
                MessageBox.Show("Error retrieving location: " & ex.Message)
            End Try
        End Function

    End Class
End Namespace

