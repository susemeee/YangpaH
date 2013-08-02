using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace YangpaH
{
    
    /// <summary>
    /// provides random dispatching and Database Wrapper
    /// </summary>
    class YangpaCore
    {
        private Random r = new Random();
        private MainWindow inst;
        
        public List<SClass> Classes { get; private set; }     //all loaded classes from file
        public SClass class_Current { get; set; }                   //selected current class
        public SInstance instance_Current { get; set; }             //current instance

        public YangpaCore(MainWindow tsni)
        {
            inst = tsni;
        }

        /// <summary>
        /// Load classes from file
        /// </summary>
        /// <returns></returns>
        public List<SClass> LoadClasses()
        {
            if (!YangpaDB.CheckFileExists())
            {
                //DB가 존재하지 않을 떄 백업 DB 사용ㄴ
                if (MessageBox.Show(YangpaConstants.MSG_SHOULD_USE_MIRROR, YangpaConstants.AppTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (!YangpaDB.LoadDBFromMirror())
                    {
                        MessageBox.Show("백업 DB가 존재하지 않습니다");
                        YangpaDB.CreateDB();
                    }
                }
                else
                    YangpaDB.CreateDB();
            }

            if (!YangpaConfig.CheckFileExists())
            {
                MessageBox.Show(YangpaConstants.MSG_FIRST_TIME, "", MessageBoxButton.OK, MessageBoxImage.Information);
                YangpaConfig.CreateFile();
                Classes = new List<SClass>();
            }
            else
            {
                Classes = YangpaConfig.GetConfigFromDB();
            }
            return Classes;
        }

        /// <summary>
        /// Save current modified class to file
        /// </summary>
        public void SaveClasses()
        {
            YangpaConfig.SetConfigToDB(Classes);
        }

        public void DoRandom(bool isStaticCaptain, bool alt)
        {
            int ctr = 0;
            int offset = r.Next(1, 6);
            
            for (int i = 0; i < instance_Current.JoMember.Length; i++)
            {
                //조장 랜덤
                int cindex = (i + offset) % 6;
                if (instance_Current.JoMember[i] == null)
                {
                    instance_Current.JoMember[i] = new List<string>();
                }
                //clear
                instance_Current.JoMember[i].Clear();
                
                //마지막 조에는 한 명 이상이 들어갈 수 있음.
                int jml = (isStaticCaptain ? 5 : 6);
                if (i >= instance_Current.JoMember.Length - 1)
                    jml = jml + 5;

                if (isStaticCaptain)
                {
                    //prevents index out exception
                    if (class_Current.Captains.Count < 6)
                    {
                        //should not use MessageBox here
                        MessageBox.Show(YangpaConstants.MSG_CAP_NOT_SIX, YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        break;
                    }
                    else
                    {
                        instance_Current.JoMember[i].Add(class_Current.Captains[cindex]);
                    }
                }
                for (int j = 0; j < jml; j++)
                {
                    int rb = r.Next(0, class_Current.Students.Count);
                    string student = class_Current.Students[rb];

                    //prevents infinite loop
                    if (++ctr > 400) break;
                    
                    //prevents duplicate
                    else if (student.Contains("!") || 
                        (class_Current.Captains.Exists(delegate(string value) { return value == student; })
/*search for captain.matches student?*/ && isStaticCaptain))
                    {
                        j--; continue;
                    }
                    else
                    {
                        if (instance_Current.JoMember[i].Exists(
                            delegate(string value) { return ("김희재" == value || "이수호" == value); })
                            && (student == "이수호" || student == "김희재"))
                        {
                            j--; continue;
                        }
                        else
                        {
                            instance_Current.JoMember[i].Add(student);
                            class_Current.Students[rb] += "!";
                        }
                    }
                }
            }
            inst.UpdateView(alt);
            for (int i = 0; i < class_Current.Students.Count; i++)
            {
                class_Current.Students[i] = class_Current.Students[i].Replace("!", "");
            }
            YangpaDB.SaveDB(instance_Current, true);
        }
    }

    /// <summary>
    /// 7분 타이머 구현
    /// </summary>
    class YangpaTimer
    {
        public const int CONST_COUNT_DESTINATION = 60 * 7;
        public static int counter { get; private set; }
        private static int counter_sec;
        private static int counter_min;
        private static DispatcherTimer timer = new DispatcherTimer();
        public static EventHandler CountHandler { get; set; }
        public static EventHandler StoppedHandler { get; set; }
        private static EventHandler ElapsedHandler = new EventHandler((object sender, EventArgs e)
                =>
            {
                counter++;
                counter_sec = counter % 60;
                counter_min = counter / 60;
                CountHandler.Invoke(sender, e);
            });

        public static string Second
        {
            get
            {
                if (counter_sec < 10) return "0" + counter_sec.ToString();
                else return counter_sec.ToString();
            }
        }
        public static string Minute
        {
            get
            {
                if (counter_min < 10) return "0" + counter_min.ToString();
                else return counter_min.ToString();
            }
        }
        internal static void Stop()
        {
            StoppedHandler.Invoke(null, null);
            timer.Stop();
            counter = 0;
            timer.Tick -= ElapsedHandler;
            timer.Tick -= CountHandler;
        }

        internal static void Start()
        {
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += ElapsedHandler;
            timer.Tick += CountHandler;
            timer.Start();
        }
    }
}
