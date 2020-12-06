using System;
using System.Collections.Generic;
using System.IO;

namespace anodern.mclog {
    class Program {
        static void Main(string[] args) {
            List<FileInfo> aa = getFile("./logs", ".gz");

            foreach(FileInfo fi in aa) {
                //Console.WriteLine(fi.FullName);
                LogAnalyse la = new LogAnalyse(fi.FullName);
                la.Analyse();
                //Console.WriteLine();
            }
            
        }

        static List<FileInfo> getFile(string path, string extName) {
            try {
                List<FileInfo> lst = new List<FileInfo>();
                string[] dir = Directory.GetDirectories(path); //文件夹列表   
                DirectoryInfo fdir = new DirectoryInfo(path);
                FileInfo[] file = fdir.GetFiles();
                foreach(FileInfo f in file) { //显示当前目录所有文件   
                    if(extName.ToLower().IndexOf(f.Extension.ToLower()) >= 0) {
                        lst.Add(f);
                    }
                }
                return lst;
            } catch(Exception) {
                throw;
            }
        }
    }
}
