'***************************************************************************

' Project  : AvionicsInstrumentControlDemo                                  
' File     : VerticalSpeedIndicatorInstrumentControl.cs                     
' Version  : 1                                                             
' Language : C#                                                             
' Summary  : The vertical speed indicator instrument control                
' Creation : 19/06/2008                                                     
' Autor    : Guillaume CHOUTEAU                                             
' History  :                                                                

'***************************************************************************


Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Collections
Imports System.Drawing
Imports System.Text
Imports System.Data

Namespace AvionicsInstrumentControlDemo
    Class VerticalSpeedIndicatorInstrumentControl
        Inherits InstrumentControl
#Region "Fields"

        ' Parameters
        'Private verticalSpeed As Integer

        Private verticalSpeedTargetValue As Single
        Private verticalSpeedMoveIndex As Integer
        Private verticalSpeedStartingValue As Single


        ' Images
        Private bmpCadran As New Bitmap(HK_GCS_Lite.My.Resources.AvionicsInstrumentsControlsRessources.vert_bottom)
        Private bmpNeedle As New Bitmap(HK_GCS_Lite.My.Resources.AvionicsInstrumentsControlsRessources.needle_01)
        Private bmpGlass As New Bitmap(HK_GCS_Lite.My.Resources.AvionicsInstrumentsControlsRessources.glass_layer_3)
        'Private bmpBorder As New Bitmap(HK_GCS.My.Resources.AvionicsInstrumentsControlsRessources.outside_bg)

        Private sUpLabel As String = "up"
        Private sDownLabel As String = "down"
        Private sInstrumentLabel As String = "vertical speed"
        Private sUnitLabel As String = "100ft/min"

#End Region

#Region "Contructor"

        ''' <summary>
        ''' Required designer variable.
        ''' </summary>
        Private components As System.ComponentModel.Container = Nothing

        Public Sub New()
            ' Double bufferisation
            SetStyle(ControlStyles.DoubleBuffer Or ControlStyles.UserPaint Or ControlStyles.AllPaintingInWmPaint, True)
        End Sub

#End Region

#Region "Component Designer generated code"
        ''' <summary>
        ''' Required method for Designer support - do not modify 
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            components = New System.ComponentModel.Container()
        End Sub
#End Region

#Region "Paint"

        Protected Overrides Sub OnPaint(ByVal pe As PaintEventArgs)
            ' Calling the base class OnPaint
            MyBase.OnPaint(pe)

            ' Pre Display computings
            Dim ptRotation As New Point(150, 150)
            Dim ptimgNeedle As New Point(135, 36)

            Me.BackColor = GetSystemColor("F5F4F1")

            bmpCadran.MakeTransparent(Color.Yellow)
            bmpNeedle.MakeTransparent(Color.Yellow)

            Dim alphaNeedle As Double = InterpolPhyToAngle(GetCurrentEaseInstrument(verticalSpeedStartingValue, verticalSpeedTargetValue, verticalSpeedMoveIndex), -6000, 6000, 120, 420)

            Dim scale As Single = CSng(Me.Width) / bmpCadran.Width

            ' diplay mask
            Dim maskPen As New Pen(Me.BackColor, 30 * scale)
            pe.Graphics.DrawRectangle(maskPen, 0, 0, bmpCadran.Width * scale, bmpCadran.Height * scale)

            ' display border
            pe.Graphics.DrawImage(bmpBorder, 0, 0, CSng(bmpCadran.Width * scale), CSng(bmpCadran.Height * scale))

            ' display cadran
            pe.Graphics.DrawImage(bmpCadran, 0, 0, CSng(bmpCadran.Width * scale), CSng(bmpCadran.Height * scale))

            Dim fontSize As Single
            fontSize = Me.Width / 180 * 8
            Using the_font As New Font("Arial", fontSize, FontStyle.Bold)
                Using string_format As New StringFormat()
                    string_format.Alignment = StringAlignment.Center
                    string_format.LineAlignment = StringAlignment.Near
                    pe.Graphics.DrawString(sUpLabel, the_font, Brushes.Azure, bmpCadran.Height * scale * 0.5, bmpCadran.Height * scale * 0.28, string_format)
                    pe.Graphics.DrawString(sDownLabel, the_font, Brushes.Azure, bmpCadran.Height * scale * 0.5, bmpCadran.Height * scale * 0.62, string_format)
                End Using
            End Using

            fontSize = Me.Width / 180 * 6
            Using the_font As New Font("Small Fonts", fontSize, FontStyle.Regular)
                pe.Graphics.DrawString(sInstrumentLabel, the_font, Brushes.Azure, bmpCadran.Height * scale * 0.62, bmpCadran.Height * scale * 0.44)
                pe.Graphics.DrawString(sUnitLabel, the_font, Brushes.Azure, bmpCadran.Height * scale * 0.62, bmpCadran.Height * scale * 0.51)
            End Using

            ' display small needle
            RotateImage(pe, bmpNeedle, alphaNeedle, ptimgNeedle, ptRotation, scale)

            pe.Graphics.DrawImage(bmpGlass, 0, 0, CSng(bmpGlass.Width * scale), CSng(bmpGlass.Height * scale))

        End Sub

#End Region

#Region "Methods"


        ''' Define the physical value to be displayed on the indicator
        ''' The aircraft vertical speed in ft per minutes
        Public Sub SetVerticalSpeedIndicatorParameters(ByVal aircraftVerticalSpeed As Integer, ByVal instrumentName As String, ByVal upLabel As String, ByVal downString As String, ByVal unitString As String)
            If aircraftVerticalSpeed <> verticalSpeedTargetValue Then
                verticalSpeedStartingValue = GetCurrentEaseInstrument(verticalSpeedStartingValue, verticalSpeedTargetValue, verticalSpeedMoveIndex)
                verticalSpeedMoveIndex = 0
                verticalSpeedTargetValue = aircraftVerticalSpeed

                sInstrumentLabel = instrumentName
                sUpLabel = upLabel
                sDownLabel = downString
                sUnitLabel = unitString

                Me.Refresh()
            End If

            'verticalSpeed = aircraftVerticalSpeed
        End Sub

        Public Sub TickEase()
            Dim bFoundOne As Boolean = False
            If verticalSpeedMoveIndex <= g_EaseSteps - 2 Then
                bFoundOne = True
                verticalSpeedMoveIndex = verticalSpeedMoveIndex + 1
            End If

            If bFoundOne = True Then
                Me.Refresh()
            End If
        End Sub
#End Region

    End Class
End Namespace