using System;
using FileUtil;
using FileUtil.File;
using FileUtil.Information;
using FileUtil.Directory;
using CoreUtil;

namespace TestUtilClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //DirectoryObject directory = new DirectoryObject("C:\\")
            //    .SubDirectory("Users")
            //    .SubDirectory("Jon")
            //    .SubDirectory("Desktop")
            //    .SubDirectory("DataUtil");
            //directory.AllFiles[0].Content.Lines = new System.Collections.Generic.List<string>() { "A", "B", "C" };
            //directory.AllFiles[0].Content.Value = "a\rb\rc";
            //directory.AllFiles[0].Content.MemoryStream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes("1\r2\r3"));
            //FileObjectList listDesktopDirectories = directory.AllDirectories.AllFiles;
            
            // Okay, Im Looking For Files With Lots Of Constraints.. So Overload The Hell Out Of It
            SearchInformation searchInfo = new SearchInformation(true, "*.cs", true, 
                FileInformation.FileDatePropertyType.CreationDate, new DateTime(2015, 1, 1), DateTime.Now,
                FileInformation.FileSizeType.KiloBytes, 0, 10);

            // But Check Back In With Me Every Now And Then
            SearchInformation.SearchCallback callback =
                new SearchInformation.SearchCallback(Callback);

            // Now, Go Get Me Stuff And Lemme Know What's Goin On Every 5 Seconds
            FileObjectList files = new DirectoryObject("C:\\Users\\Jon\\Desktop\\DataUtil")
                .GetFilesAsync(searchInfo, callback, 5);       
        }

        private static void Callback(FileObjectList files, DirectoryObjectList directories)
        {
            DateTime dt = DateTime.Now;
            Console.WriteLine(
                "Time: " + DateTime.Now.ToLongTimeString() + " | Files: " + files.Count + " | Directories Searched: " + directories.Count);           
        }        

        private void Test()
        {
            DirectoryObject directory = new DirectoryObject("C:\\")
                .SubDirectory("Users")
                .SubDirectory("Jon")
                .SubDirectory("Desktop");

            //Console.WriteLine("Search Parameters - Pattern: " + searchInfo.SearchPattern + " | " +  searchInfo.MinimumDate.ToShortDateString() + " | " + "Max Size: " + searchInfo.MaximumSize);

            FileObject file = directory.File("TestRights.txt");

            Globals.ResultType archiveResult = file.Archive(System.IO.Compression.CompressionLevel.NoCompression);
        }
    }
}
