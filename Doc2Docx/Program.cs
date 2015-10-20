﻿/*
Copyright 2015 Softcom

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

**/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Word = Microsoft.Office.Interop.Word;
using System.Diagnostics;

namespace Doc2Docx
{
    class Program
    {
        static void Main(string[] args)
        {
            FileInfo file = new FileInfo(@args[0]);

            if (file.Extension.ToLower() == ".doc" || file.Extension.ToLower() == ".xml" || file.Extension.ToLower() == ".wml")
            {

                //Set the Word Application Window Title
                string wordAppId = "" + DateTime.Now.Ticks;

                Console.WriteLine("Starting document conversion...");

                Word.Application word = new Word.Application();
                word.Application.Caption = wordAppId;
                word.Application.Visible = true;
                int processId = GetProcessIdByWindowTitle(wordAppId);

                try
                {

                    object fileformat = Word.WdSaveFormat.wdFormatXMLDocument;

                    object filename = file.FullName;
                    object newfilename = Path.ChangeExtension(file.FullName, ".docx");
                    Word._Document document = word.Documents.OpenNoRepairDialog(filename);
                    Console.WriteLine("Converting document '{0}' to DOCX.", file);

                    document.Convert();
                    document.SaveAs(newfilename, fileformat);
                    document.Close();
                    document = null;

                    word.Quit();
                    word = null;
                    Console.WriteLine("Success, quitting Word.");

                }
                catch (Exception e)
                {
                    Console.WriteLine("Error ocurred: {0}", e);
                }
                finally
                {

                    // Terminate Winword instance by PID.
                    Console.WriteLine("Terminating Winword process with the Windowtitle '{0}' and the Application ID: '{1}'.", wordAppId, processId);
                    Process process = Process.GetProcessById(processId);
                    process.Kill();

                }
            }
            else
            {
                Console.WriteLine("Only DOC / WML / XML files possible.");
            }

        }

        public static int GetProcessIdByWindowTitle(string paramWordAppId)
        {
            Process[] P_CESSES = Process.GetProcessesByName("WINWORD");
            for (int p_count = 0; p_count < P_CESSES.Length; p_count++)
            {
                if (P_CESSES[p_count].MainWindowTitle.Equals(paramWordAppId))
                {
                    return P_CESSES[p_count].Id;
                }
            }
            return Int32.MaxValue;
        }

    }
}
