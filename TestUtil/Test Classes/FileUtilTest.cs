using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileUtil;
using FileUtil.File;
using FileUtil.Directory;
using CoreUtil;

namespace TestUtil
{
    [TestClass]
    class FileUtilTest
    {
        DirectoryObject directory = new DirectoryObject("C:\\")
            .SubDirectory("Users")
            .SubDirectory("Jon")
            .SubDirectory("Desktop");
        
    }
}
