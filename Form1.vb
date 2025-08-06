Imports Gmap.net
Imports Gmap.net.WindowsForms
Imports GMap.NET.MapProviders
Imports CoreXC_Reborn.GMapTileHelperLib
Imports System.Threading.Tasks


Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AddHandler ChannelAnalyzer.SelectedIndexChanged, AddressOf TabControl_SelectedIndexChanged

    End Sub

    Private Sub GroupBox1_Enter(sender As Object, e As EventArgs)

    End Sub

    Private Sub TabPage3_Click(sender As Object, e As EventArgs)

    End Sub
    Private Async Sub TabControl_SelectedIndexChanged(sender As Object, e As EventArgs)

        If ChannelAnalyzer.SelectedTab Is TabPage3 OrElse ChannelAnalyzer.SelectedTab Is TabPage4 Then

            Dim gmap As GMapControl = Await GMapHelper.CreateMapControl(0, 0, 10)
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
End Class
