'
' SPDX-License-Identifier: GPL-3.0-or-later
'###########################################################################
'#                                                                         #
'#   Copyright © 2025–2026 Götz Kircher <mahjonggk@t-online.de>            #
'#                                                                         #
'#                     MahjongGK  -  Mahjong Solitär                       #
'#                                                                         #
'#   This program is free software: you can redistribute it and/or modify  #
'#   it under the terms of the GNU General Public License as published by  #
'#   the Free Software Foundation, either version 3 of the License, or     #
'#   at your option any later version.                                     #
'#                                                                         #
'#   This program is distributed in the hope that it will be useful,       #
'#   but WITHOUT ANY WARRANTY; without even the implied warranty of        #
'#   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the          #
'#   GNU General Public License for more details.                          #
'#   https://www.gnu.org/licenses/gpl-3.0.html                             #
'#                                                                         #
'###########################################################################
'
'

Imports System.Reflection

''' <summary>
''' Hier sind Funktionen angesiedelt, um Daten aus einem Modul in eine XML-Datei
''' zu speichern/laden.
''' Aufruf Speichern: PersistHelper.Save(GetType(ModulName), "Pfad\daten.xml")
''' Aufruf Laden: PersistHelper.Load(GetType(ModulName), "Pfad\daten.xml")
''' Es sind nicht alle Daten speicherbar, ggf muss erweitert werden.
''' Siehe SaveFieldValue und LoadFieldValue.
''' Es werden nur die Daten gespeichert, die im Modul mit dem Attribut
''' &lt;Persist&gt; versehen sind.
''' </summary>
Public Module PersistHelper

    Public Sub Save(moduleType As Type, filePath As String)
        Dim root As New XElement("PersistedData")

        For Each fi As FieldInfo In moduleType.GetFields(BindingFlags.Public Or BindingFlags.Static)
            If fi.GetCustomAttribute(Of PersistAttribute)() IsNot Nothing Then
                SaveFieldValue(fi, root)
            End If
        Next

        root.Save(filePath)
    End Sub

    Private Sub SaveFieldValue(fi As FieldInfo, parentXml As XElement)
        Dim val As Object = fi.GetValue(Nothing)
        Dim fieldElem As New XElement(fi.Name)

        If val Is Nothing Then
            parentXml.Add(fieldElem)
            Return
        End If

        Dim ft As Type = fi.FieldType

        If ft.IsArray Then
            Dim arr As Array = DirectCast(val, Array)
            Dim rank As Integer = arr.Rank
            fieldElem.SetAttributeValue("Dim", rank)
            For d As Integer = 0 To rank - 1
                fieldElem.SetAttributeValue($"Length{d}", arr.GetLength(d))
            Next
            SaveArrayItems(arr, fieldElem, New Integer(rank - 1) {}, 0)

        ElseIf ft.IsGenericType AndAlso ft.GetGenericTypeDefinition() Is GetType(List(Of )) Then
            If ft.GetGenericArguments()(0) Is GetType(Triple) Then
                Dim list As List(Of Triple) = DirectCast(val, List(Of Triple))
                For Each t As Triple In list
                    Dim tripleElem As New XElement("Triple",
                                      New XElement("x", t.x),
                                      New XElement("y", t.y),
                                      New XElement("z", t.z))
                    fieldElem.Add(tripleElem)
                Next
            Else
                Throw New NotSupportedException("Nur List(Of Triple) wird unterstützt")
            End If

        Else
            fieldElem.Value = val.ToString()
        End If

        parentXml.Add(fieldElem)
    End Sub

    Private Sub SaveArrayItems(arr As Array, parent As XElement, indices() As Integer, dimIndex As Integer)
        If dimIndex = arr.Rank Then
            Dim val As Object = arr.GetValue(indices)
            Dim itemElem As New XElement("Item", val.ToString())
            For i As Integer = 0 To indices.Length - 1
                itemElem.SetAttributeValue($"Index{i}", indices(i))
            Next
            parent.Add(itemElem)
        Else
            For i As Integer = arr.GetLowerBound(dimIndex) To arr.GetUpperBound(dimIndex)
                indices(dimIndex) = i
                SaveArrayItems(arr, parent, indices, dimIndex + 1)
            Next
        End If
    End Sub

    Public Sub Load(moduleType As Type, filePath As String)
        If Not IO.File.Exists(filePath) Then Return
        Dim doc As XElement = XElement.Load(filePath)

        For Each fi As FieldInfo In moduleType.GetFields(BindingFlags.Public Or BindingFlags.Static)
            If fi.GetCustomAttribute(Of PersistAttribute)() IsNot Nothing Then
                Dim elem As XElement = doc.Element(fi.Name)
                If elem IsNot Nothing Then
                    LoadFieldValue(fi, elem)
                End If
            End If
        Next
    End Sub

    Private Sub LoadFieldValue(fi As FieldInfo, elem As XElement)
        Dim ft As Type = fi.FieldType

        Try
            If ft.IsArray Then
                Dim rank As Integer = CInt(elem.Attribute("Dim"))
                Dim lengths(rank - 1) As Integer
                For d As Integer = 0 To rank - 1
                    lengths(d) = CInt(elem.Attribute($"Length{d}"))
                Next

                Dim arr As Array = Array.CreateInstance(ft.GetElementType(), lengths)

                For Each itemElem As XElement In elem.Elements("Item")
                    Dim indices(rank - 1) As Integer
                    For d As Integer = 0 To rank - 1
                        indices(d) = CInt(itemElem.Attribute($"Index{d}"))
                    Next
                    Dim valStr As String = itemElem.Value
                    Dim val As Object = ConvertStringToType(valStr, ft.GetElementType())
                    arr.SetValue(val, indices)
                Next

                fi.SetValue(Nothing, arr)

            ElseIf ft.IsGenericType AndAlso ft.GetGenericTypeDefinition() Is GetType(List(Of )) Then
                If ft.GetGenericArguments()(0) Is GetType(Triple) Then
                    Dim list As New List(Of Triple)
                    For Each tripleElem As XElement In elem.Elements("Triple")
                        Dim x As Integer = Integer.Parse(tripleElem.Element("x").Value)
                        Dim y As Integer = Integer.Parse(tripleElem.Element("y").Value)
                        Dim z As Integer = Integer.Parse(tripleElem.Element("z").Value)
                        list.Add(New Triple(x, y, z))
                    Next
                    fi.SetValue(Nothing, list)
                Else
                    Throw New NotSupportedException("Nur List(Of Triple) wird unterstützt")
                End If

            Else
                Dim valStr As String = elem.Value
                Dim val As Object = ConvertStringToType(valStr, ft)
                fi.SetValue(Nothing, val)
            End If
        Catch ex As Exception
            Throw New Exception($"Fehler beim Laden von Feld {fi.Name}: {ex.Message}", ex)
        End Try
    End Sub

    Private Function ConvertStringToType(valStr As String, targetType As Type) As Object
        If targetType Is GetType(String) Then
            Return valStr
        ElseIf targetType Is GetType(Integer) Then
            Return Integer.Parse(valStr)
        ElseIf targetType Is GetType(Long) Then
            Return Long.Parse(valStr)
        ElseIf targetType Is GetType(Double) Then
            Return Double.Parse(valStr)
        Else
            Return Convert.ChangeType(valStr, targetType)
        End If
    End Function

End Module

