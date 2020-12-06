using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using anodern.mclog.log;

namespace anodern.mclog {
    class LogAnalyse {
        private string path;
        private string date;
        public List<Log> list;

        public LogAnalyse(string logPath) {
            path = logPath;
            Match match = Regex.Match(logPath, @"^.*(....-..-..)-(\w+)\.log\.gz$");
            date = match.Groups[1].Value;
            list = new List<Log>();
        }

        public void Analyse() {
            StreamReader sr = new StreamReader(new GZipStream(new FileStream(path, FileMode.Open, FileAccess.ReadWrite), CompressionMode.Decompress));

            string line;
            Match match;
            while(!sr.EndOfStream) {
                line = sr.ReadLine();

                match = Regex.Match(line, @"^\[(..:..:..)\]\u0020\[(.+)/(.+)\]:\u0020(.+)$");
                if(!match.Success) {
                    Console.WriteLine("NO");
                    continue;
                }


                Log log = RouteLog(match);
                if(log is VillagerLog log1) {
                    Console.WriteLine(log1);
                    //Console.WriteLine();
                }
                if(log is LoginLog log2) {
                    Console.WriteLine(log2);
                    //Console.WriteLine();
                }
                //list.Add(log);
                //Console.WriteLine(log);

                //Console.WriteLine();

            }

            sr.Close();
        }

        private Log RouteLog(Match matchLine) {
            string text = matchLine.Groups[4].Value;
            Match match;

            match=Regex.Match(text, @"^Villager EntityVillager\['(\w+)'/\d+, l='(\S+)', x=(.+), y=(.+), z=(.+)\] died, message: '(.+)'$");
            if(match.Success) {
                VillagerLog log = new VillagerLog();
                InitLog(log, matchLine);
                log.Job = match.Groups[1].Value;
                log.World = match.Groups[2].Value;
                log.X = float.Parse(match.Groups[3].Value);
                log.Y = float.Parse(match.Groups[4].Value);
                log.Z = float.Parse(match.Groups[5].Value);
                log.Message = match.Groups[6].Value;
                return log;
            }

            match=Regex.Match(text, @"^(\S+)\[/(\S+)\] logged in with entity id \d+ at \(\[(\S+)\](\S+), (\S+), (\S+)\)$");
            if(match.Success) {
                LoginLog log = new LoginLog();
                InitLog(log, matchLine);
                log.Name = match.Groups[1].Value;
                log.IP = match.Groups[2].Value;
                log.World = match.Groups[3].Value;
                log.X = double.Parse(match.Groups[4].Value);
                log.Y = double.Parse(match.Groups[5].Value);
                log.Z = double.Parse(match.Groups[6].Value);
                return log;
            }


            return new Log {
                Time = Convert.ToDateTime(date+" "+matchLine.Groups[1].Value),
                Source = matchLine.Groups[2].Value,
                Level = matchLine.Groups[3].Value,
                Text = text
            };
        }

        private void InitLog(Log log,Match matchLine) {
            log.Time = Convert.ToDateTime(date+" "+matchLine.Groups[1].Value);
            log.Source = matchLine.Groups[2].Value;
            log.Level = matchLine.Groups[3].Value;
            log.Text = matchLine.Groups[4].Value;
        }
    }
}
