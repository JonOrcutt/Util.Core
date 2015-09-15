//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using System.IO.Compression;
//using FileUtil.File;
//using FileUtil.Directory;
//using CoreUtil;

//namespace FileUtil.Archiving
//{
//    /// <summary>
//    /// Class for generic archive functions
//    /// </summary>
//    public static class ArchiveFunctions
//    {

//#if NET45

//        #region Create Archive

//        /// <summary>
//        /// Create a zip file archive
//        /// </summary>
//        /// <param name="strZipFileName">Zipfile name</param>
//        /// <param name="strError">Error string containing any Error message encountered</param>
//        /// <returns></returns>
//        public static Globals.ResultType CreateArchive(string strZipFileName, ref string strError)
//        {
//            try
//            {
//                // Create New Archive File
//                FileStream streamZipFile = new FileStream(strZipFileName, FileMode.CreateNew);

//                // Create New ZipArchive
//                ZipArchive zipArchive = new ZipArchive(streamZipFile, ZipArchiveMode.Create);

//                // Close The Zip File
//                streamZipFile.Close();

//                return Globals.ResultType.Success;
//            }
//            catch (Exception ex)
//            {
//                strError = ex.ToString();
//                Console.Write(strError);

//                return Globals.ResultType.Failure;
//            }
//        }

//        #endregion

//        #region Extract Archive

//        /// <summary>
//        /// Extract a zip file archive
//        /// </summary>
//        /// <param name="strZipFileName">Zipfile name</param>
//        /// <param name="strDirectory">Directory to extract the archive to</param>
//        /// <param name="strError">Error string containing any Error message encountered</param>
//        /// <returns></returns>
//        public static Globals.ResultType ExtractArchive(string strZipFileName, string strDirectory, ref string strError)
//        {
//            try
//            {
//                // Check Zip File Exists
//                bool boolZipFileExists = FileFunctions.CheckFileExists(strZipFileName, ref strError);

//                // Validation
//                if (boolZipFileExists == false || strError != "") { return Globals.ResultType.Failure; }

//                // Check Output Directory Exists
//                bool boolDirectoryExists = DirectoryFunctions.CheckDirectoryExists(strDirectory, ref strError);

//                // Validation
//                if (boolZipFileExists == false || strError != "") { return Globals.ResultType.Failure; }

//                // Create File Stream
//                using (FileStream fileStream = new FileStream(strZipFileName, FileMode.OpenOrCreate))
//                {
//                    // Create New ZipArchive
//                    using (ZipArchive zipEntry = new ZipArchive(fileStream, ZipArchiveMode.Update))
//                    {
//                        // Extract Zip File to Directory
//                        zipEntry.ExtractToDirectory(strDirectory);
//                    }
//                }

//                return Globals.ResultType.Success;
//            }
//            catch (Exception ex)
//            {
//                strError = ex.ToString();
//                Console.Write(strError);

//                return Globals.ResultType.Failure;
//            }
//        }

//        #endregion

//        #region Archive File

//        /// <summary>
//        /// Archive a file
//        /// </summary>
//        /// <param name="strZipFileName">Zipfile name</param>
//        /// <param name="strFileName">Filename to archive</param>
//        /// <param name="compressionLevel">File compression level</param>
//        /// <param name="strError">Error string containing any Error message encountered</param>
//        /// <param name="boolCreateIfNotExists">Flag to create the archive file if it does not exist</param>
//        /// <returns></returns>
//        public static Globals.ResultType ArchiveFile(string strZipFileName, string strFileName, CompressionLevel compressionLevel, ref string strError, bool boolCreateIfNotExists = true)
//        {
//            try
//            {
//                string strTempError = "";

//                // Check Zip File Exists
//                bool boolZipFileExists = FileFunctions.CheckFileExists(strZipFileName, ref strTempError);

//                // Validation
//                if (boolZipFileExists == false && boolCreateIfNotExists == true)
//                {
//                    // Attempt Create Zip File
//                    Globals.ResultType createResultType = CreateArchive(strZipFileName, ref strError);

//                    // Validation
//                    if (createResultType == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }
//                }
//                else if (boolZipFileExists == false && boolCreateIfNotExists == false) { return Globals.ResultType.Failure; }

//                // Check File Exists
//                bool boolFileExists = FileFunctions.CheckFileExists(strFileName, ref strError);

//                // Validation
//                if (boolFileExists == false || strError != "") { return Globals.ResultType.Failure; }

//                // Get File Short Name
//                string strShortFileName = FileFunctions.GetFileNameWithExtenstion(strFileName, ref strError);

//                // Validation
//                if (strShortFileName == "" || strError != "") { return Globals.ResultType.Failure; }

//                // Get File Content
//                string strFileContent = FileFunctions.GetFileContentAsString(strFileName, ref strError);

//                // Validation
//                if (strError != "") { return Globals.ResultType.Failure; }

//                // Create File Stream
//                using (FileStream fileStream = new FileStream(strZipFileName, FileMode.OpenOrCreate))
//                {
//                    // Get Zip Entry
//                    using (ZipArchive zipEntry = new ZipArchive(fileStream, ZipArchiveMode.Update))
//                    {
//                        // Create File
//                        ZipArchiveEntry readmeEntry = zipEntry.CreateEntry(strShortFileName, compressionLevel);

//                        // Create new StreamWriter
//                        using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
//                        {
//                            // Write File Content
//                            writer.Write(strFileContent);
//                        }
//                    }
//                }

//                return Globals.ResultType.Success;
//            }
//            catch (Exception ex)
//            {
//                strError = ex.ToString();
//                Console.Write(strError);

//                return Globals.ResultType.Failure;
//            }
//        }

//        /// <summary>
//        /// Archive a list of files retaining the folder hierarchy
//        /// </summary>
//        /// <param name="strZipFileName">Zipfile name</param>
//        /// <param name="strFileName">Filename to archive</param>
//        /// <param name="strStartFolderName">Zipfile base folder name</param>
//        /// <param name="compressionLevel">File compression level</param>
//        /// <param name="strError">Error string containing any Error message encountered</param>
//        /// <returns></returns>
//        public static Globals.ResultType ArchiveFileWithFolderStructure(string strZipFileName, string strFileName, string strStartFolderName, CompressionLevel compressionLevel, ref string strError)
//        {
//            try
//            {
//                string strRelativeFolderPath = "";

//                if (strStartFolderName != "")
//                {
//                    // Get Relative File Path to Create
//                    strRelativeFolderPath = DirectoryFunctions.GetFileRelativeFolderPath(strFileName, strStartFolderName, true, ref strError);

//                    // Validation
//                    if (strRelativeFolderPath == "" || strError != "")
//                    {
//                        return Globals.ResultType.Failure;
//                    }
//                }

//                // Create File Stream
//                using (FileStream fileStream = new FileStream(strZipFileName, FileMode.OpenOrCreate))
//                {
//                    // Get Zip Entry
//                    using (ZipArchive zipEntry = new ZipArchive(fileStream, ZipArchiveMode.Update))
//                    {
//                        // Get File Info
//                        FileInfo fileInfo = new FileInfo(strFileName);

//                        // Check Create Folder
//                        if (strRelativeFolderPath != "")
//                        {
//                            // Create Zip File Entry
//                            zipEntry.CreateEntry(strRelativeFolderPath, compressionLevel);
//                        }

//                        // Get File Short Name
//                        string strShortFileName = FileFunctions.GetFileNameWithExtenstion(strFileName, ref strError);

//                        // Validation
//                        if (strShortFileName == "" || strError != "") { return Globals.ResultType.Failure; }

//                        // Get File Content
//                        string strFileContent = FileFunctions.GetFileContentAsString(strFileName, ref strError);

//                        // Validation
//                        if (strError != "") { return Globals.ResultType.Failure; }

//                        // Create File
//                        ZipArchiveEntry readmeEntry = zipEntry.CreateEntry(strRelativeFolderPath + strShortFileName, compressionLevel);

//                        // Create New StreamWriter
//                        using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
//                        {
//                            // Write File Content
//                            writer.Write(strFileContent);
//                        }
//                    }
//                }

//                return Globals.ResultType.Success;
//            }
//            catch (Exception ex)
//            {
//                strError = ex.ToString();
//                Console.Write(strError);

//                return Globals.ResultType.Failure;
//            }
//        }

//        #endregion

//        #region Archive File List

//        /// <summary>
//        /// Archive a list of files
//        /// </summary>
//        /// <param name="strZipFileName">Zipfile name</param>
//        /// <param name="listFileNames">List of filenames to archive</param>
//        /// <param name="compressionLevel">File compression level</param>
//        /// <param name="strError">Error string containing any Error message encountered</param>
//        /// <param name="boolCreateIfNotExists">Flag to create the archive file if it does not exist</param>
//        /// <returns></returns>
//        public static Globals.ResultType ArchiveFileList(string strZipFileName, List<string> listFileNames, CompressionLevel compressionLevel, ref string strError, bool boolCreateIfNotExists = true)
//        {
//            try
//            {
//                // Loop Files
//                foreach (string strFile in listFileNames)
//                {
//                    // Archive File
//                    Globals.ResultType archiveFileResultType = ArchiveFile(strZipFileName, strFile, compressionLevel, ref strError, boolCreateIfNotExists);

//                    // Validation
//                    if (archiveFileResultType == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }
//                }

//                return Globals.ResultType.Success;
//            }
//            catch (Exception ex)
//            {
//                strError = ex.ToString();
//                Console.Write(strError);

//                return Globals.ResultType.Failure;
//            }
//        }

//        /// <summary>
//        /// Archive a list of files without retaining the folder hierarchy
//        /// </summary>
//        /// <param name="strZipFileName">Zipfile name</param>
//        /// <param name="listFiles">List of filenames to archive</param>
//        /// <param name="strStartFolderName">Zipfile base folder name</param>
//        /// <param name="compressionLevel">File compression level</param>
//        /// <param name="strError">Error string containing any Error message encountered</param>
//        /// <returns></returns>
//        public static Globals.ResultType ArchiveFileListWithFolderStructure(string strZipFileName, List<string> listFiles, string strStartFolderName, CompressionLevel compressionLevel, ref string strError)
//        {
//            try
//            {
//                // Loop Files
//                foreach (string strFileName in listFiles)
//                {
//                    // Create File Stream
//                    using (FileStream fileStream = new FileStream(strZipFileName, FileMode.OpenOrCreate))
//                    {
//                        // Get Zip Entry
//                        using (ZipArchive zipEntry = new ZipArchive(fileStream, ZipArchiveMode.Update))
//                        {
//                            // Get File Info
//                            FileInfo fileInfo = new FileInfo(strFileName);

//                            // Check Create Folder
//                            if (strRelativeFolderPath != "")
//                            {
//                                zipEntry.CreateEntry(strRelativeFolderPath);
//                            }

//                            // Get File Short Name
//                            string strShortFileName = FileFunctions.GetFileNameWithExtenstion(strFileName, ref strError);

//                            // Validation
//                            if (strShortFileName == "" || strError != "") { return Globals.ResultType.Failure; }

//                            // Get File Content
//                            string strFileContent = FileFunctions.GetFileContentAsString(strFileName, ref strError);

//                            // Validation
//                            if (strError != "") { return Globals.ResultType.Failure; }

//                            // Create File
//                            ZipArchiveEntry readmeEntry = zipEntry.CreateEntry(strRelativeFolderPath + strShortFileName, compressionLevel);

//                            // Create New StreamWriter
//                            using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
//                            {
//                                // Write File Content
//                                writer.Write(strFileContent);
//                            }
//                        }
//                    }

//                }

//                return Globals.ResultType.Success;
//            }
//            catch (Exception ex)
//            {
//                strError = ex.ToString();
//                Console.Write(strError);

//                return Globals.ResultType.Failure;
//            }
//        }

//        #endregion

//        #region Archive Directory

//        /// <summary>
//        /// Archive an entire directory
//        /// </summary>
//        /// <param name="strZipFileName">Zipfile name</param>
//        /// <param name="strDirectory">Directory to archive</param>
//        /// <param name="boolRecursive">Flag for whether or not to retrieve files to archive recursively (including sub folder files)</param>
//        /// <param name="compressionLevel">File compression level</param>
//        /// <param name="strError">Error string containing any Error message encountered</param>
//        /// <param name="boolCreateIfNotExists">Flag to create the archive file if it does not exist</param>
//        /// <returns></returns>
//        public static Globals.ResultType ArchiveDirectory(string strZipFileName, string strDirectory, bool boolRecursive, CompressionLevel compressionLevel, ref string strError, bool boolCreateIfNotExists = true)
//        {
//            try
//            {
//                // Retrieve Directory Files
//                List<string> listFiles = DirectoryFunctions.GetDirectoryFiles(strDirectory, boolRecursive, ref strError);

//                // Validation
//                if (listFiles == null || strError != "") { return Globals.ResultType.Failure; }

//                // Archive Files
//                Globals.ResultType archiveDirectoryResultType = ArchiveFileList(strZipFileName, listFiles, compressionLevel, ref strError, boolCreateIfNotExists);

//                // Validation
//                if (archiveDirectoryResultType == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }

//                return Globals.ResultType.Success;
//            }
//            catch (Exception ex)
//            {
//                strError = ex.ToString();
//                Console.Write(strError);

//                return Globals.ResultType.Failure;
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="strZipFileName">The File Path of the Zip File to be Archived</param>
//        /// <param name="strDirectory">The Directory to Archive</param>
//        /// <param name="strStartFolderName">The Folder in the Directory Path to start with in the Root of the Zip File</param>
//        /// <param name="boolRecursive">Whether or not to Recursively Retrieve Files</param>
//        /// <param name="strError">Error string containing any Error message encountered</param>
//        /// <param name="compressionLevel">File Compression Level</param>
//        /// <returns>Success or Failure</returns>
//        public static Globals.ResultType ArchiveDirectoryWithFolderStructure(string strZipFileName, string strDirectory, string strStartFolderName, bool boolRecursive, CompressionLevel compressionLevel, ref string strError)
//        {
//            try
//            {
//                // Check Directory Exists
//                bool boolDirectoryExists = DirectoryFunctions.CheckDirectoryExists(strDirectory, ref strError);

//                // Validation
//                if (boolDirectoryExists == false || strError != "") { return Globals.ResultType.Failure; }

//                // Retrieve Directory Files
//                List<string> listFiles = DirectoryFunctions.GetDirectoryFiles(strDirectory, boolRecursive, ref strError);

//                // Validation
//                if (listFiles == null || strError != "") { return Globals.ResultType.Failure; }

//                // Archive Files
//                Globals.ResultType archiveResultType = ArchiveFileListWithFolderStructure(strZipFileName, listFiles, strStartFolderName, CompressionLevel.Fastest, ref strError);

//                // Validation
//                if (archiveResultType == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }

//                return Globals.ResultType.Success;
//            }
//            catch (Exception ex)
//            {
//                strError = ex.ToString();
//                Console.Write(strError);

//                return Globals.ResultType.Failure;
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="strZipFileName">Zipfile name</param>
//        /// <param name="strDirectory">The directory to archive</param>
//        /// <param name="boolIncludeBaseDirectory">Flag to include the base directory in the archive</param>
//        /// <param name="compressionLevel">File compression level</param>
//        /// <param name="strError">Error string containing any Error message encountered</param>
//        /// <returns></returns>
//        public static Globals.ResultType ArchiveDirectoryWithFolderStructure(string strZipFileName, string strDirectory, bool boolIncludeBaseDirectory, CompressionLevel compressionLevel, ref string strError)
//        {
//            try
//            {
//                // Check Directory Exists
//                bool boolDirectoryExists = DirectoryFunctions.CheckDirectoryExists(strDirectory, ref strError);

//                // Validation
//                if (boolDirectoryExists == false || strError != "") { return Globals.ResultType.Failure; }

//                // Create Zip File From Directory
//                ZipFile.CreateFromDirectory(strDirectory, strZipFileName, compressionLevel, boolIncludeBaseDirectory);

//                return Globals.ResultType.Success;
//            }
//            catch (Exception ex)
//            {
//                strError = ex.ToString();
//                Console.Write(strError);

//                return Globals.ResultType.Failure;
//            }
//        }

//        #endregion

//#endif
//    }
//}