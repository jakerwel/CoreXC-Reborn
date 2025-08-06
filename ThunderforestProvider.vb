Imports System
Imports GMap.NET
Imports GMap.NET.MapProviders
Imports GMap.NET.Projections

Namespace GMapThunderforestWinForms
    Public Class ThunderforestProvider
        Inherits GMapProvider

        Public Shared ReadOnly Instance As ThunderforestProvider

        Private ReadOnly apiKey As String = "9488d8f1f44142489555183db1934dc7" ' Replace with your actual API key
        Private ReadOnly maxZoom As Integer = 22
        Private ReadOnly minZoom As Integer = 0

        Shared Sub New()
            Instance = New ThunderforestProvider()
        End Sub

        Public Overrides ReadOnly Property Id As Guid
            Get
                Return New Guid("a1b2c3d4-e5f6-7890-1234-56789abcdef0")
            End Get
        End Property

        Public Overrides ReadOnly Property Name As String
            Get
                Return "Thunderforest"
            End Get
        End Property

        Public Overrides ReadOnly Property Projection As PureProjection
            Get
                Return MercatorProjection.Instance
            End Get
        End Property

        Public Overrides ReadOnly Property Overlays As GMapProvider()
            Get
                Return New GMapProvider() {Me}
            End Get
        End Property

        Public ReadOnly Property MaxZoomLevel As Integer
            Get
                Return maxZoom
            End Get
        End Property

        Public ReadOnly Property MinZoomLevel As Integer
            Get
                Return minZoom
            End Get
        End Property

        Public Overrides Function GetTileImage(pos As GPoint, zoom As Integer) As PureImage
            If zoom < minZoom OrElse zoom > maxZoom Then
                Return Nothing
            End If

            Try
                Dim style As String = "outdoors" ' Can be changed to other Thunderforest styles
                Dim url As String = $"https://tile.thunderforest.com/{style}/{zoom}/{pos.X}/{pos.Y}.png?apikey={apiKey}"
                Return GetTileImageUsingHttp(url)
            Catch
                Return Nothing
            End Try
        End Function

    End Class
End Namespace

