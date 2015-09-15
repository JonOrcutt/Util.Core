using System;
using System.IO;
using System.IO.Compression;
using FileUtil.Foundation;
using FileUtil.Information;
using FileUtil.Directory;
using CoreUtil;

namespace FileUtil.File
{
    /// <summary>
    /// This class represents a file system file
    /// </summary>
    public sealed class FileObject : FileSystemObject, IFileObject
    {
        #region Properties

        #region File Path

        private string m_FullName = "";
        /// <summary>
        /// The short filename relative to the parent directory including the file extension
        /// </summary>
        public string FullName
        {
            get
            {
                return this.m_FullName;
            }
        }

        private string m_Extension = "";
        /// <summary>
        /// The type of extension the file has
        /// </summary>
        public string Extension
        {
            get
            {
                return this.m_Extension;
            }
        }

        #endregion

        #region Content

        private FileContent m_Content;
        /// <summary>
        /// The contents of the file
        /// </summary>
        public FileContent Content
        {
            get
            {
                // Validation
                if (this.m_Content != null && this.m_Content.Value == "")
                {
                    // Load Content
                    this.m_Content.Load();
                }

                return this.m_Content;
            }
        }

        #endregion

        #region Validation
                
        /// <summary>
        /// Overrides the base method and refreshes the exists property
        /// </summary>
        public sealed override bool Exists
        {
            get
            {
                // Refresh Exists Property
                return this.CheckExists();
            }
        }

        #endregion

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="strFileName">Filename to use for this object</param>
        public FileObject(string strFileName)
            : base(strFileName)
        {
            // Set File Properties
            this.LoadProperties();
        }

        /// <summary>
        /// Load the properties for this file
        /// </summary>
        /// <returns></returns>
        private Globals.ResultType LoadProperties()
        {
            try
            {                
                #region Validation

                // Validation
                if (this.Exists == false) { return Globals.ResultType.Failure; }

                #endregion

                #region Path

                // Get File Name With Extension
                this.m_FullName = this.GetNameWithExtenstion();

                // Get File Name Without Extension
                this.Name = this.GetNameWithOutExtenstion();

                // Get File Extention
                this.m_Extension = this.GetExtension();

                #endregion

                #region Size

                // Get Directory Size In Bytes
                decimal decSize = new FileInfo(this.FilePath).Length;

                // Get Directory Size In Bytes
                this.Size = new FileSizeInformation(decSize);

                #endregion

                #region Content

                // Set File Content Information
                this.m_Content = new FileContent(this);

                #endregion

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                string strError = "An Error Occurred In Method '" + System.Reflection.MethodBase.GetCurrentMethod().Name + "'. Detail: " + ex.Message;
                Console.WriteLine(strError);

                return Globals.ResultType.Failure; 
            }
        }

        #endregion
        
        #region Refresh
        
        /// <summary>
        /// Refresh the properties of this file
        /// </summary>
        /// <returns></returns>
        public sealed override Globals.ResultType Refresh()
        {
            // Delete File
            return this.LoadProperties();
        }

        #endregion

        #region Validation

        /// <summary>
        /// Check to see if the file exists
        /// </summary>
        /// <returns></returns>
        private bool CheckExists()
        {
            try
            {
                // Check File Exists
                bool boolExists = System.IO.File.Exists(this.FilePath);

                return boolExists;
            }
            catch (Exception ex)
            {
                // To Be Implemented: Throw Custom Exception...
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        #endregion

        #region Actions

        #region Archive

#if NET45
        /// <summary>
        /// Archive a file
        /// </summary>
        /// <param name="compressionLevel">File compression level</param>
        /// <param name="fileMode">File mode. Default is to create a new archive file or add to an existing archive</param>
        /// <returns></returns>
        public Globals.ResultType Archive(CompressionLevel compressionLevel, FileMode fileMode = FileMode.OpenOrCreate)
        {
            try
            {
                // Get Zip FileName
                string strPath = Path.Combine(this.ParentDirectory.FilePath, this.Name + ".zip");

                // Create New Archive File
                // FileStream streamZipFile = new FileStream(strPath, FileMode.CreateNew);

                // Create File Stream With Open Accessor
                using (FileStream streamZipFile = new FileStream(strPath, fileMode))
                {
                    // Create New ZipArchive
                    ZipArchive zipArchive = new ZipArchive(streamZipFile, ZipArchiveMode.Create);

                    // Get Zip Entry
                    using (ZipArchive zipEntry = new ZipArchive(streamZipFile, ZipArchiveMode.Update))
                    {
                        // Create File
                        ZipArchiveEntry readmeEntry = zipEntry.CreateEntry(this.FullName, compressionLevel);

                        // Create new StreamWriter
                        using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                        {
                            // Write File Content
                            writer.Write(this.Content.Value);
                        }
                    }

                    // Close The Zip File
                    streamZipFile.Close();
                }

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                // To Be Implemented: Throw Custom Exception...
                Console.WriteLine(ex.ToString());
                return Globals.ResultType.Failure;
            }
        }

#endif

        #endregion

        #region Delete

        /// <summary>
        /// Delete this file
        /// </summary>
        /// <returns></returns>
        public sealed override Globals.ResultType Delete()
        {
            try
            {
                // Attempt File Deletion
                System.IO.File.Delete(this.FilePath);

                // Refresh Object
                this.Refresh();

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                // To Be Implemented: Throw Custom Exception...
                Console.WriteLine(ex.ToString());
                return Globals.ResultType.Failure;
            }
        }

        #endregion

        #region Rename

        /// <summary>
        /// Rename this file
        /// </summary>
        /// <param name="strNewFileName">New filename</param>
        /// <returns></returns>
        public sealed override Globals.ResultType Rename(string strNewFileName)
        {
            try
            {
                // Get New File Path
                string strNewPath = Path.Combine(this.ParentDirectory.FilePath, strNewFileName);

                // Copy Existing File With New File Name
                System.IO.File.Copy(this.FilePath, strNewPath);

                // Delete Original File
                System.IO.File.Delete(this.FilePath);

                // Set New File Path
                this.FilePath = strNewFileName;

                // Refresh File
                this.Refresh();

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                // To Be Implemented: Throw Custom Exception...
                Console.WriteLine(ex.ToString());
                return Globals.ResultType.Failure;
            }
        }

        #endregion

        #region Move

        /// <summary>
        /// Move a file to a different directory
        /// </summary>
        /// <param name="directoryObject">Directory to be copied to</param>
        /// <returns></returns>
        public sealed override Globals.ResultType Move(DirectoryObject directoryObject)
        {
            return this.Move(directoryObject, this.FullName);
        }

        /// <summary>
        /// Move a file to a different directory
        /// </summary>
        /// <param name="directoryObject">Directory to be copied to</param>
        /// <param name="strDestinationFileName">Destination filename</param>
        /// <returns></returns>
        public Globals.ResultType Move(DirectoryObject directoryObject, string strDestinationFileName)
        {
            try
            {
                string strPath = Path.Combine(directoryObject.FilePath, strDestinationFileName);

                // Attempt Move File
                System.IO.File.Move(this.FilePath, strDestinationFileName);

                // RefreshFile Object
                this.FilePath = strPath;
                this.Refresh();

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                // To Be Implemented: Throw Custom Exception...
                Console.WriteLine(ex.ToString());
                return Globals.ResultType.Failure;
            }
        }

        #endregion

        #region Copy

        /// <summary>
        /// Copy a file to a different directory
        /// </summary>
        /// <param name="directoryObject">Directory to be copied to</param>
        /// <param name="boolOverWrite">Whether or not to overwrite any existing file of the same name</param>
        /// <param name="boolDeleteOriginal">Optional: Delete the original file being copied</param>
        /// <returns></returns>
        public Globals.ResultType Copy(DirectoryObject directoryObject, bool boolOverWrite, bool boolDeleteOriginal = false)
        {
            return this.Copy(directoryObject, this.FullName, boolOverWrite, boolDeleteOriginal);
        }

        /// <summary>
        /// Copy a file to a different directory
        /// </summary>
        /// <param name="directoryObject">Directory to be copied to</param>
        /// <param name="strNewFileName">New filename to be copied to</param>
        /// <param name="boolOverWrite">Whether or not to overwrite any existing file of the same name</param>
        /// <param name="boolDeleteOriginal">Optional: Delete the original file being copied</param>
        /// <returns></returns>
        public Globals.ResultType Copy(DirectoryObject directoryObject, string strNewFileName, bool boolOverWrite, bool boolDeleteOriginal = false)
        {
            try
            {
                string strNewPath = Path.Combine(directoryObject.FilePath, strNewFileName);

                // Copy File              
                System.IO.File.Copy(this.FilePath, strNewPath, boolOverWrite);

                // Check Delete Original
                if (boolDeleteOriginal == true)
                {
                    System.IO.File.Delete(this.FilePath);
                }

                // Refresh Path
                this.FilePath = strNewPath;
                this.Refresh();

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                // To Be Implemented: Throw Custom Exception...
                Console.WriteLine(ex.ToString());
                return Globals.ResultType.Failure;
            }
        }

        #endregion

        #region Create

        /// <summary>
        /// Create a new file
        /// </summary>
        /// <param name="strFileContent"></param>
        /// <returns></returns>
        public Globals.ResultType Create(string strFileContent)
        {
            Globals.ResultType createResult = this.Write(strFileContent);

            // Refresh File Object
            this.Refresh();

            return createResult;
        }

        #endregion

        #region Write

        /// <summary>
        /// Write to a file.
        /// </summary>
        /// <param name="strFileContent">File content to be written</param>
        /// <returns></returns>
        public Globals.ResultType Write(string strFileContent)
        {
            try
            {
                // OverWrite File
                System.IO.File.WriteAllText(this.FilePath, strFileContent);

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                // To Be Implemented: Throw Custom Exception...
                Console.WriteLine(ex.ToString());
                return Globals.ResultType.Failure;
            }
        }

        #endregion

        #region Append

        /// <summary>
        /// Append Value To File. Either At The Beginning Or End Of The File
        /// </summary>
        /// <param name="strFileContent">File content to be appended</param>
        /// <param name="appendLocation">The location to append content to</param>
        /// <returns></returns>
        public Globals.ResultType Append(string strFileContent, FileInformation.FileAppendLocationType appendLocation)
        {
            try
            {
                // Determine Append Location
                switch (appendLocation)
                {
                    case FileInformation.FileAppendLocationType.Top:
                        this.Content.Value = strFileContent + this.Content.Value;
                        break;
                    case FileInformation.FileAppendLocationType.Bottom:
                        this.Content.Value += strFileContent;
                        break;
                    case FileInformation.FileAppendLocationType.Unknown:
                        return Globals.ResultType.Failure;
                }

                // Append File
                System.IO.StreamWriter writer = System.IO.File.AppendText(this.FilePath);
                writer.Write(strFileContent);
                writer.Flush();
                writer.Close();

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                // To Be Implemented: Throw Custom Exception...
                Console.WriteLine(ex.ToString());
                return Globals.ResultType.Failure;
            }
        }

        #endregion

        #region Show / Hide

        /// <summary>
        /// Set a file to be visible in the file system
        /// </summary>
        /// <returns></returns>
        public Globals.ResultType SetVisible()
        {
            try
            {
                // Get File Attributes
                System.IO.FileAttributes attributes = System.IO.File.GetAttributes(this.FilePath);
                attributes = attributes & ~FileAttributes.Hidden;

                // Set File Attributes
                this.SetAttributes(attributes);

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                // To Be Implemented: Throw Custom Exception...
                Console.WriteLine(ex.ToString());
                return Globals.ResultType.Failure;
            }
        }

        /// <summary>
        /// Set a file to be hidden in the file system
        /// </summary>
        /// <returns></returns>
        public Globals.ResultType SetHidden()
        {
            try
            {
                // Get File Attributes
                System.IO.FileAttributes attributes = System.IO.File.GetAttributes(this.FilePath);
                attributes = attributes | FileAttributes.Hidden;

                // Set File Attributes
                this.SetAttributes(attributes);

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                // To Be Implemented: Throw Custom Exception...
                Console.WriteLine(ex.ToString());
                return Globals.ResultType.Failure;
            }
        }

        /// <summary>
        /// Set an attribute on a file
        /// </summary>
        /// <param name="fileAttributes"></param>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        private Globals.ResultType SetAttributes(System.IO.FileAttributes fileAttributes)
        {
            try
            {
                // Set File Attributes
                System.IO.File.SetAttributes(this.FilePath, fileAttributes);

                // Refresh File Object
                this.Refresh();

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                // To Be Implemented: Throw Custom Exception...
                Console.WriteLine(ex.ToString());
                return Globals.ResultType.Failure;
            }
        }

        #endregion

        #region Change Extension

        /// <summary>
        /// Change a file extension to a new extension
        /// </summary>        
        /// <param name="strExtension"></param>
        /// <returns></returns>
        public Globals.ResultType ChangeExtension(string strExtension)
        {
            try
            {
                strExtension = (strExtension.Contains(".") == false) ? "." + strExtension : strExtension;

                // Change Path
                Path.ChangeExtension(this.FilePath, strExtension);
                
                // Set New File Path
                this.FilePath = Path.Combine(this.ParentDirectory.FilePath, this.Name, strExtension);

                // Refresh File Object
                this.Refresh();

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                // To Be Implemented: Throw Custom Exception...
                Console.WriteLine(ex.ToString());
                return Globals.ResultType.Failure;
            }
        }

        #endregion

        #endregion
    }
}
