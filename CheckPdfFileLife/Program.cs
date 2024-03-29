﻿using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using static System.Console;
using System.Linq;
using System.Text;

namespace CheckPdfFileLife
{
    class Program
    {
        private static PdfChecker _PdfChecker;
        static void Main(string[] args)
        {
            ForegroundColor = ConsoleColor.Green;

            _PdfChecker = new PdfChecker();
            _PdfChecker.ProgramInfo();
            _PdfChecker.GetUserInputs();

        }
    }

    public class PdfChecker
    {
        private string strDirectory;
        private List<string> lsDirectories;
        private List<String> lsPDF_FilesPaths;
        private bool bSinglePath;
        private List<String> lsCorrupted;
        private double dWastedSpace;


        /// <summary>
        /// Constructor.
        /// </summary>
        public PdfChecker()
        {
            strDirectory = "";
            lsDirectories = new List<string>();
            lsPDF_FilesPaths = new List<string>();
            lsCorrupted = new List<string>();
            dWastedSpace = 0;
        }

        /// <summary>
        /// Tool info.
        /// </summary>
        public void ProgramInfo()
        {
            WriteLine("\t\t\t\tPDF Sweeper");
            WriteLine("\t\tPDF Sweeper developed to help users cleaning up thier computers from corrupted PDF");
            WriteLine("\t\tto save some more space.");
            WriteLine("\t\t\tDeveloped By: Mohamed Abdelaziz");
            WriteLine("\t\t\tE-mail: mohamedsaleh1984@hotmail.com\n\n");
        }

        /// <summary>
        /// Get PDF Dir or Dirs from user.
        /// </summary>
        public void GetUserInputs()
        {
            WriteLine("Do you want to Sweep single directory? (Y/N)");
            char cUserChoice = ReadKey().KeyChar;

            if (cUserChoice.Equals('Y') || cUserChoice.Equals('y'))
            {
                GetDirectory();
                bSinglePath = true;
                Clear();
                Sweep();
            }
            else if (cUserChoice.Equals('N') || cUserChoice.Equals('n'))
            {
                bSinglePath = false;
                Clear();
                GetDirectoriesFromUser();
                Sweep();
            }
            else
            {
                Write("not valid value.");
            }
        }

        /// <summary>
        /// Fetch pdf directory path from user.
        /// </summary>
        private void GetDirectory()
        {
            try
            {
                WriteLine("\nPlease enter pdf directory.");
                strDirectory = ReadLine();
                strDirectory = Path.GetFullPath(strDirectory);
            }
            catch (Exception)
            {
                WriteLine("Not valid path..Sorry !!");
            }
        }

        /// <summary>
        /// Start Sweeping Process
        /// </summary>
        public void Sweep()
        {
            if (bSinglePath)
            {
                GetPdfFilesFromDirectory();
            }
            else
            {
                GetPdfFilesFromDirectories();
            }

            if (lsPDF_FilesPaths.Count > 0)
            {

                dWastedSpace = 0;
                WriteLine("PDF Sweeping Process has started..Please wait.");
                WriteLine("The expected time to finish the process depends on..\nthe number of files in the given directory.");

                foreach (var strFilePath in lsPDF_FilesPaths)
                {
                    if (IsPDFcorrupted(strFilePath))
                    {
                        lsCorrupted.Add(strFilePath);
                        dWastedSpace += GetFileSize(strFilePath);
                    }
                }
                WriteLine("PDF Sweeping Process has finished successfully.");

                WriteLine("=================================================");
                WriteLine("Total wasted space :" + toFileSize(dWastedSpace));
                WriteLine("=================================================");

                WriteLine();

                if (lsCorrupted.Count > 0)
                {
                    WriteLine("1) Do you want to Delete corrupted PDF files?");
                    WriteLine("2) Do you want to Delete corrupted PDF files and Export their names?");
                    WriteLine("3) Do you want to Export their names?");
                    WriteLine("4) Leave everything as it is.");
                    WriteLine("");


                    char cUserChoice = ReadKey().KeyChar;

                    if (cUserChoice.Equals('1'))
                    {
                        DeleteCorruptedFiles();                  
                    }
                    else if (cUserChoice.Equals('2'))
                    {
                        DeleteCorruptedFiles();
                        ExportCorruptedFilesNames();
                    }
                    else if (cUserChoice.Equals('3'))
                    {
                        ExportCorruptedFilesNames();
                    }
                    else if (cUserChoice.Equals('4'))
                    {
                        WriteLine("\n=================================================");
                        WriteLine("Total wasted space :" + toFileSize(dWastedSpace) + ", Your files remained safe.");
                        WriteLine("======================================================");
                    }
                }
                else
                {
                    WriteLine("\n=================================================");
                    WriteLine("There is no corrupted PDF files in given dir/directories");
                    WriteLine("=================================================");
                }
            }
            else
            {
                WriteLine("There is no pdf files in selected path/paths.");
            }
            WriteLine();
            WriteLine("\n=================================================");
            WriteLine("Hope you enjoyed using the program.");
            WriteLine("=================================================");
            ReadLine();
        }

        /// <summary>
        /// Export Corrupted Files Names.
        /// </summary>
        public void ExportCorruptedFilesNames()
        {
            if (lsCorrupted.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();

                foreach (var item in lsCorrupted)
                    stringBuilder.AppendLine(Path.GetFileName(item));

                File.WriteAllText("Exported.txt", stringBuilder.ToString());

                WriteLine("\n=================================================");
                WriteLine("Exported.txt has been generated.");
                WriteLine("=================================================");

            }
            else
            {
                WriteLine("There is no corrupted pdf files in given path/paths.");
            }
        }

        /// <summary>
        /// Delete Corrupted Files
        /// </summary>
        private void DeleteCorruptedFiles()
        {
            if (lsCorrupted.Count > 0)
            {
                foreach (var item in lsCorrupted)
                    File.Delete(item);

                WriteLine("\n======================================================");
                WriteLine(lsCorrupted.Count.ToString() + " have been deleted successfully");
                WriteLine("=================================================");
                WriteLine("Total freed space :" + toFileSize(dWastedSpace));
                WriteLine("======================================================");
            }
            else
            {
                WriteLine("There is no corrupted pdf files in given path/paths.");
            }

        }
        /// <summary>
        /// Get PDF Directories from User.
        /// </summary>
        /// <returns></returns>
        private List<String> GetDirectoriesFromUser()
        {
            lsDirectories = new List<string>();
            char cUserChoice = '\0';
            string strPath;
            do
            {
                cUserChoice = '\0';
                WriteLine("Please enter directory path.");
                try
                {
                    strPath = ReadLine();
                    strPath = Path.GetFullPath(strPath);
                    if (Directory.Exists(strPath))
                    {
                        lsDirectories.Add(strPath);
                        WriteLine("Do you want to add another path ? (Y/N)");
                        cUserChoice = ReadKey().KeyChar;
                    }
                    else
                    {
                        throw new Exception("Directory is not exists.");
                    }

                }
                catch (IOException)
                {
                    WriteLine("Please re-enter directory path.");
                }


            } while (cUserChoice.Equals('y') || cUserChoice.Equals('Y'));

            return lsDirectories;

        }

        /// <summary>
        /// Return list of PDF files from given directory and sub directories.
        /// </summary>
        /// <param name="strDir"></param>
        /// <returns></returns>
        public List<String> GetPdfFilesFromDirectory()
        {
            WriteLine("\nPDF Fetching Files Process has started successfully.");

            lsPDF_FilesPaths = new List<string>();
            lsPDF_FilesPaths = Directory.GetFiles(strDirectory, "*.*", SearchOption.AllDirectories).Where(f => f.EndsWith(".pdf") || f.EndsWith(".PDF")).ToList();

            WriteLine("PDF Fetching Files Process has finished successfully.");

            return lsPDF_FilesPaths;
        }

        /// <summary>
        /// Return list of PDF files from given directories and sub directories.
        /// </summary>
        /// <param name="lsDirectories">Directory List</param>
        /// <returns></returns>
        public List<String> GetPdfFilesFromDirectories()
        {
            lsPDF_FilesPaths = new List<string>();
            WriteLine("\nPDF Fetching Files Process has started successfully.");

            foreach (var strDir in lsDirectories)
                lsPDF_FilesPaths.InsertRange(0, Directory.GetFiles(strDir, "*.*", SearchOption.AllDirectories).Where(f => f.EndsWith(".pdf") || f.EndsWith(".PDF")).ToList());

            WriteLine("PDF Fetching Files Process has finished successfully.");

            return lsPDF_FilesPaths;
        }

        /// <summary>
        /// Check if given pdf file is Corrupted or not.
        /// </summary>
        /// <param name="filePath">PDF file Path</param>
        /// <returns>true otherwise; false</returns>
        private bool IsPDFcorrupted(string filePath)
        {
            bool bResult = false;
            PdfReader reader = null;
            try
            {
                reader = new PdfReader(filePath);
                reader.Close();

            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("PDF header signature not found."))
                {
                    bResult = true;
                }
            }
            return bResult;
        }

        /// <summary>
        /// Return file size.
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <returns></returns>
        private long GetFileSize(string strFilePath)
        {
            return new FileInfo(strFilePath).Length; // :D
        }

        /// <summary>
        /// eturn a string describing the value as a file size.
        /// For example, 1.23 MB.
        /// Ref: http://csharphelper.com/blog/2014/07/format-file-sizes-in-kb-mb-gb-and-so-forth-in-c/
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string toFileSize(double value)
        {
            string[] suffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            for (int i = 0; i < suffixes.Length; i++)
            {
                if (value <= (Math.Pow(1024, i + 1)))
                {
                    return ThreeNonZeroDigits(value / Math.Pow(1024, i)) + " " + suffixes[i];
                }
            }
            return ThreeNonZeroDigits(value / Math.Pow(1024, suffixes.Length - 1)) + " " + suffixes[suffixes.Length - 1];
        }

        /// <summary>
        /// Return the value formatted to include at most three
        /// non-zero digits and at most two digits after the
        /// decimal point.Examples:
        /// 1
        /// 123
        /// 12.3
        /// 1.23
        /// 0.12
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string ThreeNonZeroDigits(double value)
        {
            if (value >= 100)
            {
                // No digits after the decimal.
                return value.ToString("0,0");
            }
            else if (value >= 10)
            {
                // One digit after the decimal.
                return value.ToString("0.0");
            }
            else
            {
                // Two digits after the decimal.
                return value.ToString("0.00");
            }
        }
    }
}
