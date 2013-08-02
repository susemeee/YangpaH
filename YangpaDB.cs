using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SQLite;
using System.Security.Cryptography;

namespace YangpaH
{
    /// <summary>
    /// superclass of data model
    /// </summary>
    abstract class YangpaData
    {
        protected const string CONST_CONFIG_FILEPATH = @".\config.yangpa";
        protected const string CONST_DB_FILEPATH = @".\databases.yangpa";
        protected static readonly string CONST_DB_MIRROR_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"./yangpa/");
        protected static Encoding CONST_DEFAULT_ENCODING = Encoding.UTF8;

        #region internal function
        /// <summary>
        /// removes keyword from config file
        /// </summary>
        /// <param name="spacedstring"></param>
        /// <returns></returns>
        protected static string Raw(string spacedstring)
        {
            return spacedstring.Substring(spacedstring.IndexOf(' ')).Replace(" ", "");
        }

        /// <summary>
        /// converts list array to commaed string
        /// commaed string : [[AAA,BBB,CCC,DDD....]]
        /// </summary>
        /// <param name="lis"></param>
        /// <returns></returns>
        public static string ConvertArrayToComma(List<string> lis)
        {
            try
            {
                string res = string.Empty;
                foreach (string li in lis)
                {
                    res += ConvertSQLDangerousData(li.Replace("!", "")) + ",";
                }
                return res.Remove(res.LastIndexOf(','));
            }
            catch (NullReferenceException)
            {
                return "";
            }
            catch (ArgumentOutOfRangeException)
            {
                return "a";
            }
        }

        /// <summary>
        /// converts commaed string to list array
        /// commaed string : [[AAA,BBB,CCC,DDD....]]
        /// </summary>
        /// <param name="csv"></param>
        /// <returns></returns>
        public static List<string> ConvertCommaToArray(string csv)
        {
            string[] v1 = csv.Split(',');
            List<string> lis = new List<string>();
            for (int i = 0; i < v1.Length; i++)
            {
                if ((v1[i] = v1[i].Replace(" ", "").Replace("!", "")) != "")
                    lis.Add(v1[i]);
            }
            return lis;
        }

        /// <summary>
        /// prevents SQL Injection
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ConvertSQLDangerousData(string data)
        {
            return data.Replace("#", "").Replace("--", "").Replace("\'", "").Replace("\"", "");
        }

        public static int[] ExplodeScore(string arg0)
        {
            char Cooooooooooommmmmmmmmmmmmmna = ',';
            string[] OrigInAlArrAieD = arg0.Split(Cooooooooooommmmmmmmmmmmmmna);
            int[] ConVerTeDArrAifd = new int[OrigInAlArrAieD.Length];
            
            for (int complicatedindex1 = 0; complicatedindex1 < OrigInAlArrAieD.Length; complicatedindex1++)
            {
                int argument233413 = int.Parse(OrigInAlArrAieD[complicatedindex1].Replace("-", ""));
                if(argument233413 > 100) throw new OverflowException();
                else
                    ConVerTeDArrAifd[complicatedindex1] = argument233413;
            }
            return ConVerTeDArrAifd;
        }

        public static string MD5(string str)
        {
            StringBuilder MD5Str = new StringBuilder();
            byte[] byteArr = Encoding.ASCII.GetBytes(str);
            byte[] resultArr = (new MD5CryptoServiceProvider()).ComputeHash(byteArr);

            for (int cnti = 0; cnti < resultArr.Length; cnti++)
            {
                MD5Str.Append(resultArr[cnti].ToString("X2"));
            }
            return MD5Str.ToString();
        }
        #endregion
    }

    abstract class YangpaConfig : YangpaData
    {
        /// <summary>
        /// creates new file
        /// </summary>
        public static void CreateFile()
        {
            File.Create(CONST_CONFIG_FILEPATH);
        }

        public static bool CheckFileExists()
        {
            return File.Exists(CONST_CONFIG_FILEPATH);
        }

        public static List<SClass> GetConfigFromDB()
        {
            if (!CheckFileExists())
                throw new FileNotFoundException("File not exists.");

            List<SClass> classes = new List<SClass>();

            string[] rawdata = File.ReadAllText(CONST_CONFIG_FILEPATH, CONST_DEFAULT_ENCODING).Split(Environment.NewLine.ToCharArray());
            SClass scl = null;

            for (int i = 0; i < rawdata.Length; i++)
            {
                string line = rawdata[i];
                if (line.StartsWith("#") || line == "") continue;
                else
                {
                    if (line.StartsWith(YangpaConstants.Class))
                    {
                        scl = new SClass();
                        scl.Name = Raw(line);
                    }
                    else if (line.StartsWith(YangpaConstants.ClassEnd))
                    {
                        if (scl.Captains == null || scl.Students == null)
                            throw new FileLoadException();
                        else
                            classes.Add(scl);
                    }
                    else if (line.StartsWith(YangpaConstants.Students))
                        scl.Students = ConvertCommaToArray(Raw(line));
                    else if (line.StartsWith(YangpaConstants.CaptainPresent))
                        scl.isCaptainPresent = Boolean.Parse(Raw(line));
                    else if (line.StartsWith(YangpaConstants.Captains))
                        scl.Captains = ConvertCommaToArray(Raw(line));
                    else throw new FileLoadException("Invalid config file");
                }
            }
            return classes;

        }
        public static void SetConfigToDB(List<SClass> clss)
        {
            File.Delete(CONST_CONFIG_FILEPATH);
            FileStream fs = File.Create(CONST_CONFIG_FILEPATH);
            using(StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine("#YangpaDB AutoGenerated File, Last Modified at " + DateTime.Now.ToShortDateString());
                sw.WriteLine("#This line should be ignored by program."+Environment.NewLine);
            foreach (SClass cls in clss)
            {
                sw.WriteLine(YangpaConstants.Class+" "+cls.Name);
                sw.WriteLine(YangpaConstants.Students+" "+ConvertArrayToComma(cls.Students));
                sw.WriteLine(YangpaConstants.CaptainPresent+" "+cls.isCaptainPresent.ToString());
                sw.WriteLine(YangpaConstants.Captains+" "+ConvertArrayToComma(cls.Captains));
                sw.WriteLine(YangpaConstants.ClassEnd+Environment.NewLine);
            }
                sw.Flush();
                sw.Close();
            }
        }
    }

    abstract class YangpaDB : YangpaData
    {
        static SQLiteCommand command;
        static SQLiteConnection con = new SQLiteConnection("Data Source=" + CONST_DB_FILEPATH + ";Version=3;");
        static string sqltext;

        public static bool CheckFileExists()
        {
            return File.Exists(CONST_DB_FILEPATH);
        }

        /// <summary>
        /// DB가 없을 경우 새로 만듭니다.
        /// </summary>
        /// <returns></returns>
        public static int CreateDB()
        {
            SQLiteConnection.CreateFile(CONST_DB_FILEPATH);
            
            if (con.State == System.Data.ConnectionState.Closed)
                con.Open();
            sqltext = @"CREATE TABLE IF NOT EXISTS " + YangpaConstants.DB_Tablename + @" (
  `ID` varchar(60) NOT NULL PRIMARY KEY,
  `Name` varchar(50) NOT NULL,
  `Class` varchar(3) NOT NULL,
  `Date` date NOT NULL,
  `1JoMember` varchar(30) NOT NULL,
  `2JoMember` varchar(30) NOT NULL,
  `3JoMember` varchar(30) NOT NULL,
  `4JoMember` varchar(30) NOT NULL,
  `5JoMember` varchar(30) NOT NULL,
  `6JoMember` varchar(30) NOT NULL,
  `1JoScore` varchar(12) NOT NULL,
  `2JoScore` varchar(12) NOT NULL,
  `3JoScore` varchar(12) NOT NULL,
  `4JoScore` varchar(12) NOT NULL,
  `5JoScore` varchar(12) NOT NULL,
  `6JoScore` varchar(12) NOT NULL
);";
            command = new SQLiteCommand(sqltext, con);
            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// 내역의 변경 사항을 저장합니다.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string SaveDB(SInstance instance, bool memberUpdate)
        {
            try
            {
                //if(instance.nonexists) throw new Exception("NO record found, use Insert instead.");
                if (con.State == System.Data.ConnectionState.Closed)
                    con.Open();
                if (memberUpdate)
                {
                    sqltext = string.Format(@"UPDATE " + YangpaConstants.DB_Tablename + @" SET
`Name` = '{0}',  
`1JoMember` = '{1}',  
`2JoMember` = '{2}', 
`3JoMember` = '{3}', 
`4JoMember` = '{4}', 
`5JoMember` = '{5}', 
`6JoMember` = '{6}', 
`1JoScore` = '{7}', 
`2JoScore` = '{8}', 
`3JoScore` = '{9}', 
`4JoScore` = '{10}', 
`5JoScore` = '{11}', 
`6JoScore` = '{12}'
 WHERE  `ID` = '{13}'", instance.Name,
                    YangpaData.ConvertArrayToComma(instance.JoMember[0]), YangpaData.ConvertArrayToComma(instance.JoMember[1]),
                    YangpaData.ConvertArrayToComma(instance.JoMember[2]), YangpaData.ConvertArrayToComma(instance.JoMember[3]),
                    YangpaData.ConvertArrayToComma(instance.JoMember[4]), YangpaData.ConvertArrayToComma(instance.JoMember[5]),
                    instance.JoScoreToString(1), instance.JoScoreToString(2), instance.JoScoreToString(3),
                    instance.JoScoreToString(4), instance.JoScoreToString(5), instance.JoScoreToString(6), instance.Id);

                }
                else
                {
                    sqltext = string.Format(@"UPDATE " + YangpaConstants.DB_Tablename + @" SET
`Name` = '{0}',
`1JoScore` = '{1}', 
`2JoScore` = '{2}', 
`3JoScore` = '{3}', 
`4JoScore` = '{4}', 
`5JoScore` = '{5}', 
`6JoScore` = '{6}'
 WHERE `ID` = '{7}'", instance.Name, instance.JoScoreToString(1), instance.JoScoreToString(2), instance.JoScoreToString(3),
                    instance.JoScoreToString(4), instance.JoScoreToString(5), instance.JoScoreToString(6), instance.Id);
                }

                command = new SQLiteCommand(sqltext, con);
                return command.ExecuteNonQuery().ToString();
            }
            catch (SQLiteException e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// 새 조별활동 내역을 추가합니다.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string InsertDB(SInstance instance)
        {
            try
            {
                if (con.State == System.Data.ConnectionState.Closed)
                    con.Open();
                sqltext = string.Format(@"INSERT INTO `" + YangpaConstants.DB_Tablename + @"` 
(`ID`, `Name`, `Class`, `Date`, `1JoMember`, `2JoMember`, `3JoMember`, `4JoMember`, `5JoMember`, `6JoMember`, `1JoScore`, `2JoScore`, `3JoScore`, `4JoScore`, `5JoScore`, `6JoScore`) 
VALUES ('{15}', '{0}', '{1}', '{14}', '{2}',  '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}'
)"
                , instance.Name, instance.Class,
                YangpaData.ConvertArrayToComma(instance.JoMember[0]), YangpaData.ConvertArrayToComma(instance.JoMember[1]),
                YangpaData.ConvertArrayToComma(instance.JoMember[2]), YangpaData.ConvertArrayToComma(instance.JoMember[3]),
                YangpaData.ConvertArrayToComma(instance.JoMember[4]), YangpaData.ConvertArrayToComma(instance.JoMember[5]),
                instance.JoScoreToString(1), instance.JoScoreToString(2), instance.JoScoreToString(3),
                instance.JoScoreToString(4), instance.JoScoreToString(5), instance.JoScoreToString(6), instance.Date, instance.Id);

                command = new SQLiteCommand(sqltext, con);
                command.ExecuteNonQuery();

                return string.Empty;
            }
            catch (SQLiteException e)
            {
                if (e.Message.Contains("unique")) return YangpaConstants.MSG_COL_IS_NOT_UNIQ;
                else return e.Message;
            }
        }

        public static List<string> LoadAllDBName(string clsname)
        {
            List<string> instances = new List<string>();
            try
            {
                if (con.State == System.Data.ConnectionState.Closed)
                    con.Open();
                sqltext = "SELECT Name FROM " + YangpaConstants.DB_Tablename + " WHERE Class='" + YangpaData.ConvertSQLDangerousData(clsname) + "'";

                command = new SQLiteCommand(sqltext, con);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    instances.Add(reader["Name"].ToString());
                }
            }
            catch (SQLiteException e)
            {
                //return e.Message;
            }
            return instances;
        }

        /// <summary>
        /// 해당 반의 모든 활동 내역을 가져옵니다.
        /// </summary>
        /// <param name="clsname">반 이름</param>
        /// <returns></returns>
        public static List<SInstance> LoadAllDB(string clsname)
        {
            List<SInstance> instances = new List<SInstance>();
            //            instances.Clear();
            try
            {
                if (con.State == System.Data.ConnectionState.Closed)
                    con.Open();
                sqltext = "SELECT * FROM " + YangpaConstants.DB_Tablename + " WHERE Class='" + YangpaData.ConvertSQLDangerousData(clsname) + "'";

                command = new SQLiteCommand(sqltext, con);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    instances.Add(ReadertoInst(reader));
                }
            }
            catch (SQLiteException e)
            {
                //return e.Message;
            }
            return instances;
        }
        /// <summary>
        /// 저장된 DB에서 해당 이름에 해당하는 활동을 가져옵니다.
        /// </summary>
        /// <param name="instName"></param>
        /// <returns></returns>
        public static SInstance LoadDB(string clsName, string instName)
        {
            try
            {
                if (con.State == System.Data.ConnectionState.Closed)
                    con.Open();
                sqltext = "SELECT * FROM " + YangpaConstants.DB_Tablename + " WHERE ID='" +YangpaData.MD5(instName + clsName) + "'";

                command = new SQLiteCommand(sqltext, con);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    return ReadertoInst(reader);
                }
            }
            catch (SQLiteException e)
            {
                //return e.Messagel
            }
            return null;
        }

        public static SInstance ReadertoInst(SQLiteDataReader reader)
        {
            string[] jm = new string[6];
            string[] js = new string[6];

            for (int i = 0; i < jm.Length; i++)
            {
                jm[i] = reader[(i + 1) + "JoMember"].ToString();
            }
            for (int i = 0; i < js.Length; i++)
            {
                js[i] = reader[(i + 1) + "JoScore"].ToString();
            }

            SInstance inst = new SInstance(reader["Name"].ToString(), reader["Class"].ToString(), jm, js, reader["Date"].ToString());
            return inst;
        }

        /// <summary>
        /// delete instance by name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string DeleteDB(string id)
        {
            try
            {
                if (con.State == System.Data.ConnectionState.Closed)
                    con.Open();
                sqltext = "DELETE FROM " + YangpaConstants.DB_Tablename + " WHERE ID='" + YangpaData.ConvertSQLDangerousData(id) + "'";

                command = new SQLiteCommand(sqltext, con);
                command.ExecuteNonQuery();
                return string.Empty;
            }
            catch (SQLiteException e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// Mirrors a DB in a specific path.
        /// </summary>
        public static void MirrorDB()
        {
            if (!Directory.Exists(CONST_DB_MIRROR_PATH))
                Directory.CreateDirectory(CONST_DB_MIRROR_PATH);
            File.Copy(CONST_DB_FILEPATH, Path.Combine(CONST_DB_MIRROR_PATH+"databases.backup.yangpa"), true);
        }

        /// <summary>
        /// Loads DB from specific path.
        /// </summary>
        public static bool LoadDBFromMirror()
        {
            if (File.Exists(Path.Combine(CONST_DB_MIRROR_PATH + "databases.backup.yangpa")))
            {
                File.Copy(Path.Combine(CONST_DB_MIRROR_PATH + "databases.backup.yangpa"), CONST_DB_FILEPATH, true);
                return true;
            }
            else
                return false;
        }

    }

    /// <summary>
    /// instance wrapper
    /// </summary>
    public class SInstance
    {
        private string _date;
        private string _class;
        private string _name;
        public int[][] JoScore
        {
            /*  i-index:selective jo index(0~5) -> 1~6 jo
             *  j-index:ordered by yangpa index
             *  0-normal yangpa, 1-red yangpa, 2-rotten yangpa, 3-pig yangpa
             */
            get;
            private set;
        }
        public List<string>[] JoMember { get; set; }
        public string Class { get { return _class; } }
        public string Name { get { return _name; } }
        public string Date { get { return _date; } }
        public string Id { get { return YangpaData.MD5(_name + _class); } }

        /// <summary>
        /// constructor for newly assigned object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sclass"></param>
        public SInstance(string name, string sclass)
        {
            JoMember = new List<string>[6];
            JoScore = new int[6][];
            //default: 0,0,0,0
            for(int i=0; i<JoScore.Length; i++)
            {
                JoScore[i] = new int[4];
            }
            _name = name;
            _date = DateTime.Now.ToShortDateString();
            _class = sclass;
        }

        /// <summary>
        /// constructor for existing DB
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sclass"></param>
        /// <param name="jm">JoMember array</param>
        /// <param name="js">JoScore array</param>
        /// <param name="dt">DateTime</param>
        public SInstance(string name, string sclass, string[] jm, string[] js, string dt)
        {
            JoMember = new List<string>[6];
            for (int i = 0; i < JoMember.Length; i++)
            {
                JoMember[i] = YangpaData.ConvertCommaToArray(jm[i]);
            }
            JoScore = new int[6][];
            for (int i = 0; i < JoScore.Length; i++)
            {
                JoScore[i] = YangpaData.ExplodeScore(js[i]);
            }
            _name = name;
            _date = dt;
            _class = sclass;
        }

        public string JoScoreToString(int jindex)
        {
            string res = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                res += JoScore[jindex-1][i].ToString()+",";
            }
            return res.Remove(res.LastIndexOf(','));
        }

        public string JoScoreToActualScore(int jindex)
        {
            int[] js = JoScore[jindex];

            return (js[0] + 2 * js[1] - js[2] + 3 * js[3]).ToString();
        }
    }
    /// <summary>
    /// Class wrapper
    /// </summary>
    public class SClass
    {
        public string Name { get; set; }
        public List<string> Students { get; set; }
        public bool isCaptainPresent { get; set; }
        public List<string> Captains { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

}
