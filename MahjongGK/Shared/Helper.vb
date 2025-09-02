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
Option Compare Text
Option Explicit On
Option Infer Off
Option Strict On

#Disable Warning IDE0079
#Disable Warning IDE1006

Imports System.Xml
'
''' <summary>
''' Hier sind Helfer-Funktionen angesiedelt, die programmweit
''' aufgerufen werden können.
''' </summary>
Public Module Helper
    '
    Private _IsRunningInIDE As Boolean?
    ''' <summary>
    ''' Prüft, ob das Programm in der IDE (unter Debugger) läuft.
    ''' Ist zuverlässiger als #IF DEBUG, weil Letzteses nur so zuverlässig ist,
    ''' wie man selbst daran denkt, von Release auf Debug umzuschalten.
    ''' </summary>
    ''' <returns>True, wenn ein Debugger angehängt ist, sonst False.</returns>
    Public Function IsRunningInIDE() As Boolean
        If IsNothing(_IsRunningInIDE) Then
            _IsRunningInIDE = System.Diagnostics.Debugger.IsAttached
        End If
        Return CBool(_IsRunningInIDE)
    End Function

    ''' <summary>
    ''' Prüft, ob das Programm in der IDE läuft und markiert den Hauptfenstertitel.
    ''' </summary>
    ''' <param name="form">Form, deren Titel angepasst werden soll. Kann Nothing sein.</param>
    ''' <param name="prefix">Text, der vorangestellt werden soll. Standard = "[IDE] "</param>
    ''' <returns>True, wenn in der IDE.</returns>
    Public Function IsRunningInIDE(form As Form, Optional prefix As String = "[IDE] ") As Boolean
        Dim inIDE As Boolean = System.Diagnostics.Debugger.IsAttached
        If inIDE AndAlso form IsNot Nothing Then
            form.Text = prefix & form.Text
        End If
        Return inIDE
    End Function
    '
    ''' <summary>
    ''' Gibt die im Setup hinterlegte Versionsnummer der letzten Veröffentlichung zurück,
    ''' sofern der manifestPath korrekt angegeben wird und vorhanden ist.
    ''' Weitere Infos im FunktionsCode.
    ''' </summary>
    ''' <returns></returns>
    Public Function ReadClickOnceVersionFromManifest(Optional manifestPath As String = Nothing) As String

        'Die beiden Varianten ergeben zur Entwicklungszet keine brauchbare Information.
        'Dim version As String = FileVersionInfo.GetVersionInfo(Application.ExecutablePath).FileVersion
        'Dim version As String = Application.ProductVersion
        'Da ist die Versionsnummer der letzten Veröffentlichung hilfreicher,
        'die versucht wird hier auszulesen.

        If String.IsNullOrEmpty(manifestPath) Then
            'Das ist der Standardpfad auf meinem Rechner. Individuell anpassen!
            'Das Manifest steht dort, wo das Programm veröffentlicht wird. Diese
            'Funktion ist also nur nutzbar, wenn die Veröffentlichung lokal erfolgt.
            Dim appname As String = System.Reflection.Assembly.GetExecutingAssembly().GetName.Name
            manifestPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\Visual Studio Setups\" & appname & "\" & appname & ".application"
        End If


        If IO.File.Exists(manifestPath) Then
            Dim doc As New XmlDocument()
            doc.Load(manifestPath)

            ' Default-Namespace manuell registrieren mit Präfix "def"
            Dim nsmgr As New XmlNamespaceManager(doc.NameTable)
            nsmgr.AddNamespace("def", "urn:schemas-microsoft-com:asm.v1")

            ' XPath verwendet den neuen Präfix für das <assemblyIdentity>-Element
            Dim node As XmlNode = doc.SelectSingleNode("//def:assemblyIdentity", nsmgr)

            If node IsNot Nothing AndAlso node.Attributes("version") IsNot Nothing Then
                Return node.Attributes("version").Value
            Else
                Return "0.0.0.0"

            End If
        Else
            Return "0.0.0.0"

        End If

    End Function

End Module
