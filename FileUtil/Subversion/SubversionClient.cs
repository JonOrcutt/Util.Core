using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SharpSvn;
using CoreUtil;

namespace FileUtil.Subversion
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SubversionClient
    {
        #region Properties

        private SubversionConnection m_Connection = null;
        /// <summary>
        /// Subversion connection
        /// </summary>
        public SubversionConnection Connection
        {
            get
            {
                return this.m_Connection;
            }
        }

        /// <summary>
        /// Target information type
        /// </summary>
        public enum TargetInformationType : int
        {
            /// <summary>
            /// Unknown
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// ChangeList
            /// </summary>
            ChangeList = 1,

            /// <summary>
            /// Checksum
            /// </summary>
            Checksum = 2,

            /// <summary>
            /// Conflicts
            /// </summary>
            Conflicts = 3,

            /// <summary>
            /// ContentTime
            /// </summary>
            ContentTime = 4,

            /// <summary>
            /// Depth
            /// </summary>
            Depth = 5,

            /// <summary>
            /// FullPath
            /// </summary>
            FullPath = 6,

            /// <summary>
            /// LastChangeAuthor
            /// </summary>
            LastChangeAuthor = 7,

            /// <summary>
            /// LastChangeRevision
            /// </summary>
            LastChangeRevision = 8,

            /// <summary>
            /// LastChangeTime
            /// </summary>
            LastChangeTime = 9,

            /// <summary>
            /// Uri
            /// </summary>
            Uri = 10,

            /// <summary>
            /// MovedFrom
            /// </summary>
            MovedFrom = 11,

            /// <summary>
            /// MovedTo
            /// </summary>
            MovedTo = 12,

            /// <summary>
            /// NodeKind
            /// </summary>
            NodeKind = 13,

            /// <summary>
            /// Path
            /// </summary>
            Path = 14,

            /// <summary>
            /// RepositoryId
            /// </summary>
            RepositoryId = 15,

            /// <summary>
            /// RepositoryRoot
            /// </summary>
            RepositoryRoot = 16,

            /// <summary>
            /// RepositorySize
            /// </summary>
            RepositorySize = 17,

            /// <summary>
            /// Revision
            /// </summary>
            Revision = 18,

            /// <summary>
            /// Schedule
            /// </summary>
            Schedule = 19,

            /// <summary>
            /// TreeConflict
            /// </summary>
            TreeConflict = 20,

            /// <summary>
            /// WorkingCopySize
            /// </summary>
            WorkingCopySize = 21
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="connection">Subversion client to use</param>
        public SubversionClient(SubversionConnection connection)
        {
            this.m_Connection = connection;
        }

        #endregion

        #region Download

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFileName"></param>
        /// <param name="strOutputFileDirectory"></param>
        /// <param name="strOutputName"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public Globals.ResultType DownloadFile(string strFileName, string strOutputFileDirectory, string strOutputName, ref string strError)
        {
            try
            {
                // Validation
                if (strFileName == "" || strOutputFileDirectory == "" || strOutputName == "") { return Globals.ResultType.Failure; }
                
                SvnUriTarget uri = new SvnUriTarget(strFileName);
                string strOutputFileName = strOutputFileDirectory + strOutputName;

                // Download File
                System.Net.WebClient client = new System.Net.WebClient();

                // Download File
                client.DownloadFile(strFileName, strOutputFileName);
                
                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                Console.WriteLine(strError);

                return Globals.ResultType.Failure;
            }
        }

        #endregion
        
        #region Create Functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSvnDirectoryUrl"></param>
        /// <param name="strNewFolderName"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public Globals.ResultType CreateDirectory(string strSvnDirectoryUrl, string strNewFolderName, ref string strError)
        {
            try
            {
                // Validation
                if (strSvnDirectoryUrl == "" || strNewFolderName == "") { return Globals.ResultType.Failure; }
                
                string strSvnTagDirectory = strSvnDirectoryUrl + strNewFolderName;
                
                // Create DIrectory
                this.Connection.CreateDirectory(strSvnTagDirectory);
                
                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                Console.WriteLine(strError);

                return Globals.ResultType.Failure;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSvnBranchDirectoryUrl"></param>
        /// <param name="strSvnTagDirectoryUrl"></param>
        /// <param name="strNewTagFolderName"></param>
        /// <param name="strWorkingDirectory"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public Globals.ResultType CreateTag(string strSvnBranchDirectoryUrl, string strSvnTagDirectoryUrl, string strNewTagFolderName, string strWorkingDirectory, ref string strError)
        {
            try
            {
                // Validation
                if (strSvnTagDirectoryUrl == "" || strNewTagFolderName == "" || strWorkingDirectory == "") { return Globals.ResultType.Failure; }
                
                // Commit Branch
                Globals.ResultType commitDirectoryResultType = this.CommitDirectory(strWorkingDirectory, ref strError);

                // Validation
                if (commitDirectoryResultType == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }

                // Create New Tag Directory
                Globals.ResultType createDirectoryResultType = this.CreateDirectory(strSvnTagDirectoryUrl, strNewTagFolderName, ref strError);

                // Validation
                if (createDirectoryResultType == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }

                // Copy Branch to New Tag Folder
                Globals.ResultType copyDirectoryResultType = this.CopyDirectory(strSvnBranchDirectoryUrl, strSvnTagDirectoryUrl, ref strError);

                // Validation
                if (createDirectoryResultType == Globals.ResultType.Failure || strError != "") { return Globals.ResultType.Failure; }
                
                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                Console.WriteLine(strError);

                return Globals.ResultType.Failure;
            }
        }
                
        #endregion

        #region Get File / Directory Paths
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSvnDirectoryUrl"></param>
        /// <param name="boolRecurse"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public List<string> GetPaths(string strSvnDirectoryUrl, bool boolRecurse, ref string strError)
        {
           try
           {
               Collection<SvnListEventArgs> listArgs;
               List<string> listFiles = new List<string>();
                
               // Get Encoded URL
               strSvnDirectoryUrl = this.GetUrlEncoded(strSvnDirectoryUrl);

               // Get File Path List
               bool boolRetrievedPaths = this.Connection.GetList(new Uri(strSvnDirectoryUrl), out listArgs);

               // Validation
               if (boolRetrievedPaths == false) { return null; }

               // Loop Arguments
               foreach (SvnListEventArgs item in listArgs)
               {
                   listFiles.Add(item.Uri.AbsoluteUri);

                   // If Recursive And Directory
                   if (boolRecurse == true && item.Uri.AbsoluteUri != strSvnDirectoryUrl && item.Entry.NodeKind.ToString() == "Directory")
                   {
                       // Check Recurse and Get Child Paths
                       List<string> listChildPaths = this.GetPaths(item.Uri.AbsoluteUri, boolRecurse, ref strError);

                       // Validation
                       if (listChildPaths == null || strError != "") { return listFiles; }

                       listChildPaths = listChildPaths.Where(strPath => strPath != "" && listFiles.Contains(strPath) == false).ToList();

                       listFiles.AddRange(listChildPaths.ToArray());
                   }
               }

               return listFiles;
           }
            catch (Exception ex)
           {
               strError = ex.ToString();
               Console.WriteLine(strError);

               return null;
           }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSvnDirectoryUrl"></param>
        /// <param name="strFind"></param>
        /// <param name="boolMatchCase"></param>
        /// <param name="boolRecurse"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public List<string> GetPaths(string strSvnDirectoryUrl, string strFind, bool boolMatchCase, bool boolRecurse, ref string strError)
        {
           try
           {
               // Get Directory Paths
               List<string> listFiles = this.GetPaths(strSvnDirectoryUrl, boolRecurse, ref strError);

               listFiles = listFiles.Where(strPath => ((boolMatchCase == true) ? 
                   strPath.Contains(strFind) == true : 
                   strPath.ToLower().Contains(strFind.ToLower()) == true) == true
               ).ToList();

               return listFiles;
           }
            catch (Exception ex)
           {
               strError = ex.ToString();
               Console.WriteLine(strError);

               return null;
           }
        }             

        #endregion

        #region Get Information

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFileName"></param>
        /// <param name="infomationType"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public string GetTargetInformation(string strFileName, TargetInformationType infomationType, ref string strError)
        {
            try
            {
                // Validation
                if (infomationType == TargetInformationType.Unknown) { return ""; }

                SvnTarget target = SvnTarget.FromString(strFileName);

                SvnInfoEventArgs args;

                // Get Target Info
                bool boolRetrievedInfo = this.Connection.GetInfo(target, out args);

                // Validation
                if (boolRetrievedInfo == false || args == null) { return ""; }
                                
                // Determine Information Type
                switch (infomationType)
                {
                    case TargetInformationType.ChangeList:
                        {
                            return args.ChangeList;
                        }
                    case TargetInformationType.Checksum:
                        {
                            return args.Checksum;
                        }
                    case TargetInformationType.Conflicts:
                        {
                            List<string> listConflicts = (args.Conflicts != null) ?
                            args.Conflicts.Select(conflict =>
                                "Name: " + conflict.Name +
                                ". Type: " + conflict.ConflictType.ToString() +
                                ". Reason: " + conflict.ConflictReason.ToString() +
                                ". Action: " + conflict.ConflictAction.ToString()).ToList() :
                           new List<string>();

                            return string.Join("\r\n", listConflicts.ToArray());
                        }
                    case TargetInformationType.ContentTime:
                        {
                            return args.ContentTime.ToString();
                        }
                    case TargetInformationType.Depth:
                        {
                            return args.Depth.ToString();
                        }
                    case TargetInformationType.FullPath:
                        {
                            return args.FullPath;
                        }
                    case TargetInformationType.LastChangeAuthor:
                        {
                            return args.LastChangeAuthor;
                        }
                    case TargetInformationType.LastChangeRevision:
                        {
                            return args.LastChangeRevision.ToString();
                        }
                    case TargetInformationType.LastChangeTime:
                        {
                            return args.LastChangeTime.ToString();
                        }
                    case TargetInformationType.MovedFrom:
                        {
                            return args.MovedFrom;
                        }
                    case TargetInformationType.MovedTo:
                        {
                            return args.MovedTo;
                        }
                    case TargetInformationType.NodeKind:
                        {
                            return args.NodeKind.ToString();
                        }
                    case TargetInformationType.Path:
                        {
                            return args.Path;
                        }
                    case TargetInformationType.RepositoryId:
                        {
                            return args.RepositoryId.ToString();
                        }
                    case TargetInformationType.RepositoryRoot:
                        {
                            return args.RepositoryRoot.AbsolutePath;
                        }
                    case TargetInformationType.RepositorySize:
                        {
                            return args.RepositorySize.ToString();
                        }
                    case TargetInformationType.Revision:
                        {
                            return args.Revision.ToString();
                        }
                    case TargetInformationType.Schedule:
                        {
                            return args.Schedule.ToString();
                        }
                    case TargetInformationType.TreeConflict:
                        {
                            return args.TreeConflict.FullPath;
                        }
                    case TargetInformationType.Uri:
                        {
                            return args.Uri.AbsolutePath;
                        }
                    case TargetInformationType.WorkingCopySize:
                        {
                            return args.WorkingCopySize.ToString();
                        }
                }

                return "";
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                Console.WriteLine(strError);

                return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFileName"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public SvnInfoEventArgs GetTargetInformation(string strFileName, ref string strError)
        {
            try
            {              
                // Create Target Information
                SvnTarget target = SvnTarget.FromString(strFileName);

                SvnInfoEventArgs args;

                // Get Target Information
                bool boolRetrievedInfo = this.Connection.GetInfo(target, out args);

                // Validation
                if (boolRetrievedInfo == false || args == null) { return null; }
                                
                return args;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                Console.WriteLine(strError);

                return null;
            }
        }

        #endregion

        #region Get File Content

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFileName"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        public string GetFileContent(string strFileName, ref string strError)
        {
            try
            {
                // Validation
                if (strFileName == "") { return ""; }
                
                // Create New Web Request
                System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(strFileName);
                request.Credentials = this.m_Connection.Credential;
                request.Method = "GET";
                request.CookieContainer = new System.Net.CookieContainer();
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:37.0) Gecko/20100101 Firefox/37.0";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                //request.Host = "subversion";

                // Retrieve Response
                System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse();
                System.IO.Stream dataStream = response.GetResponseStream();
                System.IO.StreamReader reader = new System.IO.StreamReader(dataStream);
                string strResponse = reader.ReadToEnd();

                return strResponse;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                Console.WriteLine(strError);

                return "";
            }
        }

        #endregion

        #region Conversions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        private string GetUrlEncoded(string strUrl)
        {
            // Create Mapped Characters
            List<KeyValuePair<string, string>> listMappedChars = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(" ", "%20"),
                new KeyValuePair<string, string>("!", "%21"),
                new KeyValuePair<string, string>("\"", "%22"),
                new KeyValuePair<string, string>("#", "%23"),
                new KeyValuePair<string, string>("$", "%24"),
                new KeyValuePair<string, string>("&", "%26"),
                new KeyValuePair<string, string>("+", "%2B")
            };

            // Loop Character Mappings
            foreach (KeyValuePair<string, string> kvp in listMappedChars)
            {
                //Replace Key With Value
                strUrl = strUrl.Replace(kvp.Key, kvp.Value);
            }

            return strUrl;
        }

        #endregion

        #region Methods To Be Reviewed For Implementation

        #region Check Out

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSvnDirectoryUrl"></param>
        /// <param name="strOutputDirectory"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        internal Globals.ResultType CheckoutProject(string strSvnDirectoryUrl, string strOutputDirectory, ref string strError)
        {
            try
            {
                // Create URL Target
                SvnUriTarget uri = new SvnUriTarget(strSvnDirectoryUrl);                

                // Check Out URL
                bool boolCheckedOut = this.Connection.CheckOut(uri, strOutputDirectory);

                // Validation
                Globals.ResultType checkOutResultType = (boolCheckedOut == true) ? Globals.ResultType.Success : Globals.ResultType.Failure;

                return checkOutResultType;
            }
            catch(Exception ex)
            {
                strError = ex.ToString();
                Console.WriteLine(strError);

                return Globals.ResultType.Failure;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSvnDirectoryUrl"></param>
        /// <param name="intRevisionNumber"></param>
        /// <param name="strOutputDirectory"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        internal Globals.ResultType CheckoutProject(string strSvnDirectoryUrl, long intRevisionNumber, string strOutputDirectory, ref string strError)
        {
            try                
            {
                // Create Target URL
                SvnUriTarget uri = new SvnUriTarget(strSvnDirectoryUrl, intRevisionNumber);                

                // Check Out Target URL
                bool boolCheckedOut = this.Connection.CheckOut(uri, strOutputDirectory);

                // Validation
                Globals.ResultType checkOutResultType = (boolCheckedOut == true) ? Globals.ResultType.Success : Globals.ResultType.Failure;

                return checkOutResultType;
            }
            catch(Exception ex)
            {
                strError = ex.ToString();
                Console.WriteLine(strError);

                return Globals.ResultType.Failure;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSvnDirectoryUrl"></param>
        /// <param name="dtRevisionDate"></param>
        /// <param name="strOutputDirectory"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        internal Globals.ResultType CheckoutProject(string strSvnDirectoryUrl, DateTime dtRevisionDate, string strOutputDirectory, ref string strError)
        {
            try
            {
                // Create Target URL
                SvnUriTarget uri = new SvnUriTarget(strSvnDirectoryUrl, dtRevisionDate);

                // Check Out Target URL
                bool boolCheckedOut = this.Connection.CheckOut(uri, strOutputDirectory);

                // Validation
                Globals.ResultType checkOutResultType = (boolCheckedOut == true) ? Globals.ResultType.Success : Globals.ResultType.Failure;

                return checkOutResultType;
            }
            catch(Exception ex)
            {
                strError = ex.ToString();
                Console.WriteLine(strError);

                return Globals.ResultType.Failure;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="svnDirectoryUri"></param>
        /// <param name="strOutputDirectory"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        internal SvnUpdateResult CheckoutProject(SvnUriTarget svnDirectoryUri, string strOutputDirectory, ref string strError)
        {
            try
            {
                SvnUpdateResult result;
                
                // Checkout Project
                bool boolCheckedOut = this.Connection.CheckOut(svnDirectoryUri, strOutputDirectory, out result);

                return result;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                Console.WriteLine(strError);

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="svnDirectoryUri"></param>
        /// <param name="intRevisionNumber"></param>
        /// <param name="strOutputDirectory"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        internal SvnUpdateResult CheckoutProject(SvnUriTarget svnDirectoryUri, long intRevisionNumber, string strOutputDirectory, ref string strError)
        {
            try
            {
                SvnUpdateResult result;

                // Checkout Project
                bool boolCheckedOut = this.Connection.CheckOut(svnDirectoryUri, strOutputDirectory, out result);

                return result;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                Console.WriteLine(strError);

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="svnDirectoryUri"></param>
        /// <param name="dtRevisionDate"></param>
        /// <param name="strOutputDirectory"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        internal SvnUpdateResult CheckoutProject(SvnUriTarget svnDirectoryUri, DateTime dtRevisionDate, string strOutputDirectory, ref string strError)
        {
            try
            {
                SvnUpdateResult result;

                // Checkout Project
                bool boolCheckedOut = this.Connection.CheckOut(svnDirectoryUri, strOutputDirectory, out result);

                return result;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                Console.WriteLine(strError);

                return null;
            }
        }
        
        #endregion 
        
        #region Commit Functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSvnBranchDirectoryUrl"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        internal Globals.ResultType CommitDirectory(string strSvnBranchDirectoryUrl, ref string strError)
        {
            try
            {
                // Commit Project Revisions
                this.Connection.Commit(strSvnBranchDirectoryUrl);

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                Console.WriteLine(strError);

                return Globals.ResultType.Failure;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSvnBranchDirectoryUrl"></param>
        /// <param name="strError"></param>
        /// <param name="boolIgnore"></param>
        /// <returns></returns>
        internal SvnCommitResult CommitDirectory(string strSvnBranchDirectoryUrl, ref string strError, bool boolIgnore = false)
        {
            try
            {
                SvnCommitResult result;

                // Commit Project Revisions
                this.Connection.Commit(strSvnBranchDirectoryUrl, out result);

                return result;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                Console.WriteLine(strError);

                return null;
            }
        }

        #endregion

        #region Copy Functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSvnBranchDirectoryUrl"></param>
        /// <param name="strSvnTagDirectoryUrl"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        internal Globals.ResultType CopyDirectory(string strSvnBranchDirectoryUrl, string strSvnTagDirectoryUrl, ref string strError)
        {
            try
            {
                // Validation
                if (strSvnTagDirectoryUrl == "") { return Globals.ResultType.Failure; }

                // Create Tag
                bool boolTagCreated = this.Connection.Copy(strSvnBranchDirectoryUrl, strSvnTagDirectoryUrl);

                // Validation
                if (boolTagCreated == false || strError != "") { return Globals.ResultType.Failure; }

                Globals.ResultType copyResultType = (boolTagCreated == true) ? Globals.ResultType.Success : Globals.ResultType.Failure;

                return copyResultType;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                Console.WriteLine(strError);

                return Globals.ResultType.Failure;
            }
        }

        #endregion

        #region Get File Differences

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFileFrom"></param>
        /// <param name="strFileTo"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        private bool CheckFileDifference(string strFileFrom, string strFileTo, ref string strError)
        {
            try
            {
                bool boolIsDifferent = false;

                // Create New Memory Stream
                using (System.IO.MemoryStream result = new System.IO.MemoryStream())
                {                    

                    // Create New SVN Target
                    SvnUriTarget fileFrom = new SvnUriTarget(strFileFrom);

                    // Get Target
                    SvnUriTarget fileTo = new SvnUriTarget(strFileTo);

                    // Check Differences
                    boolIsDifferent = this.Connection.Diff(fileFrom, fileTo, result);
                    System.IO.StreamReader strReader = new System.IO.StreamReader(result);

                    // Read File
                    string str = strReader.ReadToEnd();                  
                }

                return boolIsDifferent;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                Console.WriteLine(strError);

                return false;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listFileNames"></param>
        /// <param name="strError"></param>
        /// <returns></returns>
        private string GetMostRecentCheckedInFilePath(List<string> listFileNames, ref string strError)
        {
            try
            {
                DateTime dtMostRecent = DateTime.MinValue;
                string strMostRecentFileName = "";

                // Loop FileNames
                foreach (string strFileName in listFileNames)
                {
                    // Get Target Information
                    string strDateTime = this.GetTargetInformation(strFileName, TargetInformationType.LastChangeTime, ref strError);
                    DateTime dt = (DateTime.TryParse(strDateTime, out dt) == true) ? DateTime.Parse(strDateTime) : DateTime.MinValue;

                    // Validation
                    if (dt == DateTime.MinValue || strError != "") { return ""; }

                    // Check Date Greater Than Most Recent
                    if (dt >= dtMostRecent)
                    {
                        dtMostRecent = dt;
                        strMostRecentFileName = strFileName;
                    }
                }

                return strMostRecentFileName;
            }
            catch (Exception ex)
            {
                strError = ex.ToString();
                Console.WriteLine(strError);

                return "";
            }
        }

        #endregion

        #endregion
    }
}
