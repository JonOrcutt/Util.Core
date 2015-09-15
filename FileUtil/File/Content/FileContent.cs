﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CoreUtil;

namespace FileUtil.File
{
    /// <summary>
    /// This class contains file content information
    /// </summary>
    public sealed class FileContent : IFileContent
    {
        #region Properties

        #region File

        private FileObject m_File;
        /// <summary>
        /// The file object this content resides in
        /// </summary>
        /// <returns></returns>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public FileObject File
        {
            get
            {
                return this.m_File;
            }
        }

        #endregion

        #region Content

        private string m_Value = "";
        /// <summary>
        /// Value of the file content
        /// </summary>
        /// <returns></returns>
        public string Value
        {
            get
            {
                return this.m_Value;
            }
            set
            {
                // Validation
                if (value == null)
                {
                    this.m_Value = "";
                    this.m_Lines = new List<string>();
                    this.m_MemoryStream = new MemoryStream();
                    return;
                }

                // Set Value
                this.m_Value = value;

                // Set Lines
                this.m_Lines = this.m_Value.Split('\r').ToList();

                // Set Memory Stream
                this.m_MemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(this.m_Value ?? ""));
            }
        }

        private List<string> m_Lines = new List<string>();
        /// <summary>
        /// The list of lines that make up the file content
        /// </summary>
        /// <returns></returns>
        public List<string> Lines
        {
            get
            {                
                return this.m_Lines;
            }
            set
            {
                // Validation
                if (value == null)
                {
                    this.m_Value = "";
                    this.m_Lines = new List<string>();
                    this.m_MemoryStream = new MemoryStream();
                    return;
                }
                
                // Get Memory Stream
                this.m_Lines = value;

                // Set Value
                this.m_Value = String.Join("\r", this.m_Lines.ToArray());

                // Set Memory Stream
                this.m_MemoryStream = new MemoryStream(Encoding.UTF8.GetBytes(this.m_Value ?? ""));
            }
        }

        private MemoryStream m_MemoryStream;
        /// <summary>
        /// The memory stream representing the value of the file content
        /// </summary>
        /// <returns></returns>
        public MemoryStream MemoryStream
        {
            get
            {
                return this.m_MemoryStream;
            }
            set
            {
                // Validation
                if (value == null)
                {
                    this.m_Value = "";
                    this.m_Lines = new List<string>();
                    this.m_MemoryStream = new MemoryStream();
                    return;
                }

                // Set Memory Stream
                this.m_MemoryStream = value;

                // Get Value 
                this.m_Value = new StreamReader(this.m_MemoryStream).ReadToEnd();

                // Get Lines
                this.m_Lines = this.m_Value.Split('\r').ToList();
            }
        }

        #endregion
        
        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="fileObject"></param>
        public FileContent(FileObject fileObject)
        {
            // Validation
            if (fileObject == null) { return; }

            // Set File Object
            this.m_File = fileObject;
        }

        #endregion

        #region Functions

        /// <summary>
        /// Load the file's content
        /// </summary>
        /// <returns></returns>
        public Globals.ResultType Load()
        {
            // Validation
            if (this.File == null || this.File.Exists == false || this.File.Permissions.CanRead == false)
            {
                return Globals.ResultType.Failure;
            }
            
            // Get File Content
            this.m_Value = this.GetContentAsString();

            // Get File Lines
            this.m_Lines = this.GetContentAsLineList();

            // Get Memory Stream
            this.m_MemoryStream = this.GetContentAsMemoryStream();

            // Validation
            if (this.m_Value == null) { return Globals.ResultType.Failure; }

            return Globals.ResultType.Success;
        }

        /// <summary>
        /// Save the file's content
        /// </summary>
        /// <returns></returns>
        public Globals.ResultType Save()
        {
            try
            {
                // Validation
                if (this.File == null || this.File.Exists == false || this.File.Permissions.CanRead == false)
                {
                    return Globals.ResultType.Failure;
                }
                
                // Save File Content
                Globals.ResultType saveResult = this.File.Write(this.Value);

                // Validation
                if (saveResult == Globals.ResultType.Failure) { return Globals.ResultType.Failure; }

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
        /// Refresh the file's content
        /// </summary>
        /// <returns></returns>
        public Globals.ResultType Refresh()
        {
            // Reload This File's Content
            return this.Load();
        }

        #endregion        
    }
}
