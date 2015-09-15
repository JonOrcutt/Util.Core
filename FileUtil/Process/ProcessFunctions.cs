using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.DirectoryServices;
using CoreUtil;

namespace FileUtil.Processes
{
    /// <summary>
    /// This class provides methods for dealing with system processes
    /// </summary>
    public static class ProcessFunctions
    {        
        #region Execute Files
                     
        /// <summary>
        /// Execute a file, wait for the file execution to cease, and return its exit code
        /// </summary>
        /// <param name="processInformation">Process information object</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <param name="intTimeoutSeconds">Number of seconds to wait before timing out</param>
        /// <param name="intDefaultFailure">Default return code</param>
        /// <returns></returns>
        public static int ExecuteFileWaitForExit(ProcessInformation processInformation, ref string strError, int intTimeoutSeconds = 3600, int intDefaultFailure = 1)
        {
            try
            {

                // Create Process
                Process process = new Process();
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.UseShellExecute = processInformation.UseShellExecute;
                processInfo.FileName = processInformation.FileName;
                processInfo.Arguments = processInformation.Arguments;
                processInfo.UserName = (processInformation.Username != "") ? processInformation.Username : processInfo.UserName;
                processInfo.Password = (processInformation.Password != null) ? processInformation.Password : processInfo.Password;

                // Loop Environment Variables
                foreach (KeyValuePair<string, string> kvp in processInformation.EnvironmentVariables)
                {
                    if (processInfo.EnvironmentVariables.ContainsKey(kvp.Key) == true) { continue; }
                    processInfo.EnvironmentVariables.Add(kvp.Key, kvp.Value);
                }

                process.StartInfo = processInfo;

                DateTime dtStarted = DateTime.Now;

                // Start Process
                bool boolStarted = process.Start();

                // Validation
                if (boolStarted == false) { return intDefaultFailure; }

                // Set Wait For Exit
                process.WaitForExit(intTimeoutSeconds * 1000);

                // Check Process Has Exited
                if (process.HasExited == false)
                {
                    TimeSpan ts = DateTime.Now - dtStarted;
                    
                    strError = "Error: Process Has Not Exited Yet. Total Seconds Elapsed: " + ts.TotalSeconds;

                    return intDefaultFailure;
                }

                // Get Return Code
                int intReturnCode = process.ExitCode;

                return intReturnCode;
            }
            catch (Exception ex)
            {
                strError += "\r\n" + "Error: Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + "The File '" + processInformation.FileName + "' was not executed successfully. " + ex.ToString();
                //ErrorList.Add(new KeyValuePair<FileInformation.FileErrorType, string>(FileInformation.FileErrorType.ExceptionThrown, strError));

                return intDefaultFailure;
            }
        }
                
        #endregion

        #region Get Processes

        /// <summary>
        /// Retrieve a running process
        /// </summary>
        /// <param name="strProcessName"></param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public static Process GetProcess(string strProcessName, ref string strError)
        {
            try
            {
                // Get Processes
                List<Process> listProcesses = Process.GetProcesses().ToList();

                // Filter Processes by Name
                listProcesses = listProcesses.Where(process => process.ProcessName.ToLower().Trim() == strProcessName.ToLower().Trim()).ToList();

                // Validation
                if (listProcesses == null || listProcesses.Count == 0) { return null; }
                
                return listProcesses[0];
            }
            catch (Exception ex)
            {
                strError += "\r\n" + "Error: Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + "Could not retrieve Process. Detail: " + ex.ToString();
                //ErrorList.Add(new KeyValuePair<FileInformation.FileErrorType, string>(FileInformation.FileErrorType.ExceptionThrown, strError));

                return null;
            }
        }
        
        //public static ManagementObjectCollection GetProcessArguments(string strProcessName, ref string strError, bool boolIgnoreMe = true)
        //{
        //    try
        //    {
        //        string strWmiQuery = string.Format("select CommandLine from Win32_Process where Name='{0}'", strProcessName);

        //        System.Management.ManagementObjectSearcher searcher = new ManagementObjectSearcher(strWmiQuery);
        //        ManagementObjectCollection listArguments = searcher.Get();
        //        if (listArguments == null) { return null; }

        //        return listArguments;
        //    }
        //    catch (Exception ex)
        //    {
        //        strError += "\r\n" + "Error: Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + "Could not retrieve Process Arguments" + ex.ToString();
        //        //ErrorList.Add(new KeyValuePair<FileInformation.FileErrorType, string>(FileInformation.FileErrorType.ExceptionThrown, strError));

        //        return null;
        //    }
        //}
        
        /// <summary>
        /// Retrieve a list of running processes by machine name
        /// </summary>
        /// <param name="strMachineName">Machine name to retrieve processes for</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public static List<Process> GetProcessesByMachineName(string strMachineName, ref string strError)
        {
            try
            {
                // Get Processes
                List<Process> listProcesses = Process.GetProcesses().ToList();

                // Filter Processes by Name
                listProcesses = listProcesses.Where(process => process.MachineName.ToLower().Trim() == strMachineName.ToLower().Trim()).ToList();

                // Validation
                if (listProcesses == null || listProcesses.Count == 0) { return null; }

                return listProcesses;
            }
            catch (Exception ex)
            {
                strError += "\r\n" + "Error: Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + "Could not Retrieve Process Machine Name" + ex.ToString();
                //ErrorList.Add(new KeyValuePair<FileInformation.FileErrorType, string>(FileInformation.FileErrorType.ExceptionThrown, strError));

                return null;
            }
        }
        

        /// <summary>
        /// Retrieve a list of running processes on a remote machine
        /// </summary>
        /// <param name="strMachineName">Machine name to query</param>
        /// <param name="strDomainName">Machine domain name</param>
        /// <param name="strUserName">Username</param>
        /// <param name="strPassword">User password</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public static List<ManagementObject> GetRemoteProcesses(string strMachineName, string strDomainName, string strUserName, string strPassword, ref string strError)
        {
            try
            {
                ConnectionOptions connOptions = new ConnectionOptions();
                ManagementScope myScope = new ManagementScope();

                connOptions.Impersonation = ImpersonationLevel.Impersonate;
                connOptions.EnablePrivileges = true;
                if (strMachineName.ToUpper() == Environment.MachineName.ToUpper())
                {
                    myScope = new ManagementScope(@"\ROOT\CIMV2", connOptions);
                }
                else
                {
                    connOptions.Username = strDomainName + "\\" + strUserName;
                    connOptions.Password = strPassword;
                    myScope = new ManagementScope(@"\\" + strMachineName + @"\ROOT\CIMV2", connOptions);
                }             
                
                myScope.Connect();
                ManagementObjectSearcher objSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Process");
                ManagementOperationObserver opsObserver = new ManagementOperationObserver();
                objSearcher.Scope = myScope;
                string[] sep = { "\n", "\t" };
     
                List<ManagementObject> listObjects = new List<ManagementObject>();

                // Loop Mamagement Objects
                foreach (ManagementObject obj in objSearcher.Get())
                {
                    listObjects.Add(obj);
                }

                return listObjects;

            }
            catch (Exception ex)
            {
                strError += "\r\n" + "Error: Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + "Could not Retrieve Process Machine Name" + ex.ToString();
                //ErrorList.Add(new KeyValuePair<FileInformation.FileErrorType, string>(FileInformation.FileErrorType.ExceptionThrown, strError));

                return null;
            }
        }

        #endregion

        #region Teminate Process

        /// <summary>
        /// Terminate a process
        /// </summary>
        /// <param name="strProgramName">Program name</param>
        /// <param name="strMachineName">Machine name</param>
        /// <param name="strDomainName">Domain name</param>
        /// <param name="strUserName">Username</param>
        /// <param name="strPassword">User password</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public static Globals.ResultType TerminateRemoteProcess(string strProgramName, string strMachineName, string strDomainName, string strUserName, string strPassword, ref string strError)
        {
            try
            {
                ConnectionOptions connOptions = new ConnectionOptions();
                ManagementScope myScope = new ManagementScope();

                connOptions.Impersonation = ImpersonationLevel.Impersonate;
                connOptions.EnablePrivileges = true;
                if (strMachineName.ToUpper() == Environment.MachineName.ToUpper())
                {
                    myScope = new ManagementScope(@"\ROOT\CIMV2", connOptions);
                }
                else
                {
                    connOptions.Username = strDomainName + "\\" + strUserName;
                    connOptions.Password = strPassword;
                    myScope = new ManagementScope(@"\\" + strMachineName + @"\ROOT\CIMV2", connOptions);
                }

                myScope.Connect();
                ManagementObjectSearcher objSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Process");
                ManagementOperationObserver opsObserver = new ManagementOperationObserver();
                objSearcher.Scope = myScope;
                string[] sep = { "\n", "\t" };

                List<ManagementObject> listObjects = new List<ManagementObject>();

                // Loop Management Objects
                foreach (ManagementObject obj in objSearcher.Get())
                {
                    obj.InvokeMethod("Terminate", null);
                }
                return Globals.ResultType.Success;

            }
            catch (Exception ex)
            {
                strError += "\r\n" + "Error: Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + "Could not Retrieve Process Machine Name" + ex.ToString();
                //ErrorList.Add(new KeyValuePair<FileInformation.FileErrorType, string>(FileInformation.FileErrorType.ExceptionThrown, strError));

                return Globals.ResultType.Failure;
            }
        }

        #endregion

        #region File Locking

        /// <summary>
        /// Get a list of processes locking files in a specified directory
        /// </summary>
        /// <param name="filePath">Directory to check for locked files</param>
        /// <returns></returns>
        public static List<Process> GetProcessesLockingFile(string filePath)
        {
            var listProcesses = new List<Process>();

            var listProcessesSnapshot = Process.GetProcesses();
            foreach (var process in listProcessesSnapshot)
            {
                // Validation
                if (process.Id <= 4) { continue; } // system processes

                // Get Locked Files
                var files = GetFilesLockedByProcess(process);

                // Validation
                if (files.Contains(filePath))
                {
                    listProcesses.Add(process);
                }
            }

            return listProcesses;
        }

        /// <summary>
        /// Get files locked by a specific process
        /// </summary>
        /// <param name="process">Process to examine</param>
        /// <returns></returns>
        public static List<string> GetFilesLockedByProcess(Process process)
        {
            var listFiles = new List<string>();

            // Create New ThreadStart Delegate
            ThreadStart ts = delegate
            {
                try
                {
                    // Get Unsafe Files Locked
                    listFiles = UnsafeGetFilesLockedBy(process);
                }
                catch (Exception ex)
                {
                // To Be Implemented: Throw Custom Exception...
                Console.WriteLine(ex.ToString());
                }
            };

            try
            {
                var t = new Thread(ts);
                t.IsBackground = true;

                // Start Thread
                t.Start();

                // Check Join Time
                if (!t.Join(250))
                {
                    try
                    {
                        // Interrupt Process
                        t.Interrupt();

                        // Abort Process
                        t.Abort();
                    }
                    catch (Exception ex)
                    {
                        // To Be Implemented: Throw Custom Exception...
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                // To Be Implemented: Throw Custom Exception...
                Console.WriteLine(ex.ToString());
            }

            return listFiles;
        }

        /// <summary>
        /// Get files locked by the current running process
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFilesLockedByCurrentProcess()
        {
            // Get Current Process
            Process process = Process.GetCurrentProcess();

            // Get Files Locked By Current Process
            List<string> listFiles = GetFilesLockedByProcess(process);

            return listFiles;
        }
        
        #endregion

        #region Get Process Information

        /// <summary>
        /// Get arguments passed to a running process
        /// </summary>
        /// <param name="strProcessName">Process name</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public static List<string> GetProcessArguments(string strProcessName, ref string strError)
        {
            try
            {
                List<string> listArgs = new List<string>();

                strProcessName = (strProcessName.Contains(".exe") == false) ? strProcessName + ".exe" : strProcessName;

                // Create Management Searcher
                string strWmiQuery = string.Format("select CommandLine from Win32_Process where Name='{0}'", strProcessName);
                System.Management.ManagementObjectSearcher searcher = new ManagementObjectSearcher(strWmiQuery);

                // Get Management Objects
                ManagementObjectCollection listArguments = searcher.Get();

                // Validation
                if (listArguments == null) { return null; }

                // Loop Arguments
                foreach (ManagementObject arg in listArguments)
                {
                    string strArgument = arg["CommandLine"].ToString();
                    listArgs.Add(strArgument);
                }

                return listArgs;
            }
            catch (Exception ex)
            {
                strError += "\r\n" + "Error: Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + "Could not retrieve Process Arguments. Detail: " + ex.ToString();
                //ErrorList.Add(new KeyValuePair<FileInformation.FileErrorType, string>(FileInformation.FileErrorType.ExceptionThrown, strError));

                return null;
            }
        }

        /// <summary>
        /// Retrieve the exit code from a running process
        /// </summary>
        /// <param name="strProcessName">Process name</param>
        /// <param name="intTimeoutSeconds">Number of seconds to wait before timing out</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public static int GetProcessExitCode(string strProcessName, int intTimeoutSeconds, ref string strError)
        {
            try
            {
                intTimeoutSeconds = intTimeoutSeconds * 1000;

                // Get Processes
                List<Process> listProcesses = Process.GetProcesses().ToList();

                // Filter Processes by Name
                listProcesses = listProcesses.Where(process => process.ProcessName.ToLower().Trim() == strProcessName.ToLower().Trim()).ToList();

                // Validation
                if (listProcesses == null || listProcesses.Count == 0) { return -1; }

                listProcesses[0].WaitForExit(intTimeoutSeconds);

                // Get Process Exit Code
                int intExitCode = listProcesses[0].ExitCode;

                return intExitCode;
            }
            catch (Exception ex)
            {
                strError += "\r\n" + "Error: Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + "Could not Retrieve Process Exit Code. Detail: " + ex.ToString();
                //ErrorList.Add(new KeyValuePair<FileInformation.FileErrorType, string>(FileInformation.FileErrorType.ExceptionThrown, strError));

                return -1;
            }
        }

        /// <summary>
        /// Retrieve console output from a running process
        /// </summary>
        /// <param name="strProcessName">Process name</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public static string GetProcessConsoleOutput(string strProcessName, ref string strError)
        {
            try
            {
                // Get Processes
                Process process = GetProcess(strProcessName, ref strError);

                // Validation
                if (process == null || strError != "") { return ""; }

                process.BeginOutputReadLine();
                string strConsoleOutput = process.StandardOutput.ReadToEnd();

                return strConsoleOutput;
            }
            catch (Exception ex)
            {
                strError += "\r\n" + "Error: Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + "Could not Retrieve Process Console Output. Detail: " + ex.ToString();
                //ErrorList.Add(new KeyValuePair<FileInformation.FileErrorType, string>(FileInformation.FileErrorType.ExceptionThrown, strError));

                return "";
            }
        }
        
        /// <summary>
        /// Retrieve console error output from a running process
        /// </summary>
        /// <param name="strProcessName">Process name</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public static string GetProcessConsoleError(string strProcessName, ref string strError)
        {
            try
            {                
                // Get Processes
                Process process = GetProcess(strProcessName, ref strError);

                // Validation
                if (process == null || strError != "") { return ""; }

                process.BeginErrorReadLine();
                string strConsoleError = process.StandardError.ReadToEnd();

                return strConsoleError;
            }
            catch (Exception ex)
            {
                strError += "\r\n" + "Error: Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + "Could not Retrieve Process Console Error. Detail: " + ex.ToString();
                //ErrorList.Add(new KeyValuePair<FileInformation.FileErrorType, string>(FileInformation.FileErrorType.ExceptionThrown, strError));

                return "";
            }
        }

        /// <summary>
        /// Retrieve the start time for a running process
        /// </summary>
        /// <param name="strProcessName">Process name</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public static DateTime GetProcessStartTime(string strProcessName, ref string strError)
        {
            try
            {
                // Get Processes
                Process process = GetProcess(strProcessName, ref strError);

                // Validation
                if (process == null || strError != "") { return DateTime.MinValue; }

                DateTime dtStarted = process.StartTime;

                return dtStarted;
            }
            catch (Exception ex)
            {
                strError += "\r\n" + "Error: Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + "Could not Retrieve Process Start Time. Detail: " + ex.ToString();
                //ErrorList.Add(new KeyValuePair<FileInformation.FileErrorType, string>(FileInformation.FileErrorType.ExceptionThrown, strError));

                return DateTime.MinValue;
            }
        }
        
        #endregion

        #region Kill Processes
        
        /// <summary>
        /// Kill a running process
        /// </summary>
        /// <param name="strProcessName">Process name</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public static Globals.ResultType KillProcess(string strProcessName, ref string strError)
        {
            try
            {
                // Get Processes
                List<Process> listProcesses = Process.GetProcesses().ToList();

                // Filter Processes by Name
                listProcesses = listProcesses.Where(process => process.ProcessName.ToLower().Trim() == strProcessName.ToLower().Trim()).ToList();

                // Validation
                if (listProcesses == null || listProcesses.Count == 0) { return Globals.ResultType.Failure; }

                // Loop Processes Found
                foreach (Process process in listProcesses)
                {
                    // Kill Process
                    listProcesses[0].Kill();
                }

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                strError += "\r\n" + "Error: Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + "Could not Kill Process. Detail: " + ex.ToString();
                //ErrorList.Add(new KeyValuePair<FileInformation.FileErrorType, string>(FileInformation.FileErrorType.ExceptionThrown, strError));

                return Globals.ResultType.Failure;
            }
        }
        
        #endregion

        #region Set Process Priority
        
        /// <summary>
        /// Set the priority of a specified process
        /// </summary>
        /// <param name="strProcessName">Process name</param>
        /// <param name="priority">Priority</param>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public static Globals.ResultType SetProcessPriority(string strProcessName, ProcessPriorityClass priority, ref string strError)
        {
            try
            {
                // Get Processes
                List<Process> listProcesses = Process.GetProcesses().ToList();

                // Filter Processes by Name
                listProcesses = listProcesses.Where(process => process.ProcessName.ToLower().Trim() == strProcessName.ToLower().Trim()).ToList();

                // Validation
                if (listProcesses == null || listProcesses.Count == 0) { return Globals.ResultType.Failure; }

                // Loop Processes Found
                foreach (Process process in listProcesses)
                {
                    // Set Process Priority
                    process.PriorityClass = priority;
                }

                return Globals.ResultType.Success;
            }
            catch (Exception ex)
            {
                strError += "\r\n" + "Error: Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + "Could not Set Process Priority. Detail: " + ex.ToString();
                //ErrorList.Add(new KeyValuePair<FileInformation.FileErrorType, string>(FileInformation.FileErrorType.ExceptionThrown, strError));

                return Globals.ResultType.Failure;
            }
        }


        #endregion

        #region Get Machine Info

        /// <summary>
        /// Retrieve a list of machine names
        /// </summary>
        /// <param name="strError">Error string containing any Error message encountered</param>
        /// <returns></returns>
        public static List<string> GetMachineNames(ref string strError)
        {
            DirectoryEntry entry = new DirectoryEntry("LDAP://" + "blues.healthnow.local");

            // Create a user searcher by using filter
            DirectorySearcher userSearcher = new DirectorySearcher(entry);

            // Set Filter
            userSearcher.Filter = ("(objectclass=user)");

            // Find All
            SearchResultCollection src = userSearcher.FindAll();            

            // Get all computers
            DirectorySearcher searcher = new DirectorySearcher(entry);

            // Set Search Filter
            searcher.Filter = ("(objectclass=computer)");

            List<string> listEntries = new List<string>();

            try
            {
                // Get Search Results
                SearchResultCollection results = searcher.FindAll();

                // Loop Search Results
                foreach (SearchResult sr in results)
                {
                    // Get Directory Entry
                    DirectoryEntry de = sr.GetDirectoryEntry();
                    listEntries.Add(de.Name.Remove(0, 3));
                }

                return listEntries;
            }
            catch (Exception ex)
            {
                strError += "\r\n" + "Error: Method Name: " + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + "Could not Set Process Priority. Detail: " + ex.ToString();
                //ErrorList.Add(new KeyValuePair<FileInformation.FileErrorType, string>(FileInformation.FileErrorType.ExceptionThrown, strError));

                return null;
            }        
        }

        #endregion


        #region Private - Core

        #region Properties

        const int CNST_SYSTEM_HANDLE_INFORMATION = 16;

        #endregion
        
        private static List<string> UnsafeGetFilesLockedBy(Process process)
        {
            try
            {
                var handles = GetHandles(process);
                var files = new List<string>();

                foreach (var handle in handles)
                {
                    var file = GetFilePath(handle, process);
                    if (file != null) files.Add(file);
                }

                return files;
            }
            catch
            {
                return new List<string>();
            }
        }

        private static string GetFilePath(ProcessWin32Information.SYSTEM_HANDLE_INFORMATION systemHandleInformation, Process process)
        {
            var ipProcessHwnd = ProcessWin32Information.OpenProcess(ProcessWin32Information.ProcessAccessFlags.All, false, process.Id);
            var objBasic = new ProcessWin32Information.OBJECT_BASIC_INFORMATION();
            var objObjectType = new ProcessWin32Information.OBJECT_TYPE_INFORMATION();
            var objObjectName = new ProcessWin32Information.OBJECT_NAME_INFORMATION();
            var strObjectName = "";
            var nLength = 0;
            IntPtr ipTemp, ipHandle;

            if (!ProcessWin32Information.DuplicateHandle(ipProcessHwnd, systemHandleInformation.Handle, ProcessWin32Information.GetCurrentProcess(), out ipHandle, 0, false, ProcessWin32Information.DUPLICATE_SAME_ACCESS))
                return null;

            IntPtr ipBasic = Marshal.AllocHGlobal(Marshal.SizeOf(objBasic));
            ProcessWin32Information.NtQueryObject(ipHandle, (int)ProcessWin32Information.ObjectInformationClass.ObjectBasicInformation, ipBasic, Marshal.SizeOf(objBasic), ref nLength);
            objBasic = (ProcessWin32Information.OBJECT_BASIC_INFORMATION)Marshal.PtrToStructure(ipBasic, objBasic.GetType());
            Marshal.FreeHGlobal(ipBasic);

            IntPtr ipObjectType = Marshal.AllocHGlobal(objBasic.TypeInformationLength);
            nLength = objBasic.TypeInformationLength;
            // this one never locks...
            while ((uint)(ProcessWin32Information.NtQueryObject(ipHandle, (int)ProcessWin32Information.ObjectInformationClass.ObjectTypeInformation, ipObjectType, nLength, ref nLength)) == ProcessWin32Information.STATUS_INFO_LENGTH_MISMATCH)
            {
                if (nLength == 0)
                {
                    return null;
                }
                Marshal.FreeHGlobal(ipObjectType);
                ipObjectType = Marshal.AllocHGlobal(nLength);
            }

            objObjectType = (ProcessWin32Information.OBJECT_TYPE_INFORMATION)Marshal.PtrToStructure(ipObjectType, objObjectType.GetType());
            if (Is64Bits())
            {
                ipTemp = new IntPtr(Convert.ToInt64(objObjectType.Name.Buffer.ToString(), 10) >> 32);
            }
            else
            {
                ipTemp = objObjectType.Name.Buffer;
            }

            var strObjectTypeName = Marshal.PtrToStringUni(ipTemp, objObjectType.Name.Length >> 1);
            Marshal.FreeHGlobal(ipObjectType);
            if (strObjectTypeName != "File")
                return null;

            nLength = objBasic.NameInformationLength;

            var ipObjectName = Marshal.AllocHGlobal(nLength);

            // ...this call sometimes hangs. Is a Windows error.
            while ((uint)(ProcessWin32Information.NtQueryObject(ipHandle, (int)ProcessWin32Information.ObjectInformationClass.ObjectNameInformation, ipObjectName, nLength, ref nLength)) == ProcessWin32Information.STATUS_INFO_LENGTH_MISMATCH)
            {
                Marshal.FreeHGlobal(ipObjectName);
                if (nLength == 0)
                {                    
                    return null;
                }
                ipObjectName = Marshal.AllocHGlobal(nLength);
            }
            objObjectName = (ProcessWin32Information.OBJECT_NAME_INFORMATION)Marshal.PtrToStructure(ipObjectName, objObjectName.GetType());

            if (Is64Bits())
            {
                ipTemp = new IntPtr(Convert.ToInt64(objObjectName.Name.Buffer.ToString(), 10) >> 32);
            }
            else
            {
                ipTemp = objObjectName.Name.Buffer;
            }

            if (ipTemp != IntPtr.Zero)
            {

                var baTemp = new byte[nLength];
                try
                {
                    Marshal.Copy(ipTemp, baTemp, 0, nLength);

                    strObjectName = Marshal.PtrToStringUni(Is64Bits() ? new IntPtr(ipTemp.ToInt64()) : new IntPtr(ipTemp.ToInt32()));
                }
                catch (AccessViolationException)
                {
                    return null;
                }
                finally
                {
                    Marshal.FreeHGlobal(ipObjectName);
                    ProcessWin32Information.CloseHandle(ipHandle);
                }
            }

            string path = GetRegularFileNameFromDevice(strObjectName);
            try
            {
                return path;
            }
            catch
            {
                return null;
            }
        }

        private static string GetRegularFileNameFromDevice(string strRawName)
        {
            string strFileName = strRawName;
            foreach (string strDrivePath in Environment.GetLogicalDrives())
            {
                var sbTargetPath = new StringBuilder(ProcessWin32Information.MAX_PATH);
                if (ProcessWin32Information.QueryDosDevice(strDrivePath.Substring(0, 2), sbTargetPath, ProcessWin32Information.MAX_PATH) == 0)
                {
                    return strRawName;
                }
                string strTargetPath = sbTargetPath.ToString();
                if (strFileName.StartsWith(strTargetPath))
                {
                    strFileName = strFileName.Replace(strTargetPath, strDrivePath.Substring(0, 2));
                    break;
                }
            }
            return strFileName;
        }

        private static IEnumerable<ProcessWin32Information.SYSTEM_HANDLE_INFORMATION> GetHandles(Process process)
        {
            var nHandleInfoSize = 0x10000;
            var ipHandlePointer = Marshal.AllocHGlobal(nHandleInfoSize);
            var nLength = 0;
            IntPtr ipHandle;

            while (ProcessWin32Information.NtQuerySystemInformation(CNST_SYSTEM_HANDLE_INFORMATION, ipHandlePointer, nHandleInfoSize, ref nLength) == ProcessWin32Information.STATUS_INFO_LENGTH_MISMATCH)
            {
                nHandleInfoSize = nLength;
                Marshal.FreeHGlobal(ipHandlePointer);
                ipHandlePointer = Marshal.AllocHGlobal(nLength);
            }

            var baTemp = new byte[nLength];
            Marshal.Copy(ipHandlePointer, baTemp, 0, nLength);

            long lHandleCount;
            if (Is64Bits())
            {
                lHandleCount = Marshal.ReadInt64(ipHandlePointer);
                ipHandle = new IntPtr(ipHandlePointer.ToInt64() + 8);
            }
            else
            {
                lHandleCount = Marshal.ReadInt32(ipHandlePointer);
                ipHandle = new IntPtr(ipHandlePointer.ToInt32() + 4);
            }

            var lstHandles = new List<ProcessWin32Information.SYSTEM_HANDLE_INFORMATION>();

            for (long lIndex = 0; lIndex < lHandleCount; lIndex++)
            {
                var shHandle = new ProcessWin32Information.SYSTEM_HANDLE_INFORMATION();
                if (Is64Bits())
                {
                    shHandle = (ProcessWin32Information.SYSTEM_HANDLE_INFORMATION)Marshal.PtrToStructure(ipHandle, shHandle.GetType());
                    ipHandle = new IntPtr(ipHandle.ToInt64() + Marshal.SizeOf(shHandle) + 8);
                }
                else
                {
                    ipHandle = new IntPtr(ipHandle.ToInt64() + Marshal.SizeOf(shHandle));
                    shHandle = (ProcessWin32Information.SYSTEM_HANDLE_INFORMATION)Marshal.PtrToStructure(ipHandle, shHandle.GetType());
                }
                if (shHandle.ProcessID != process.Id) continue;
                lstHandles.Add(shHandle);
            }
            return lstHandles;
        }

        private static bool Is64Bits()
        {
            return Marshal.SizeOf(typeof(IntPtr)) == 8;
        }

        #endregion                
    }
}
