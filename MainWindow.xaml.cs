using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using System.IO;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows.Media;

namespace YangpaH
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Elysium.Controls.Window
    {
        private YangpaCore core;
        private Label[] labelJo = new Label[6];
        private Label[] labelJs = new Label[6];
        private Label[] labelJo_b = new Label[6];
        private Rectangle[] rectJo = new Rectangle[6];
        private DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            //Didn't use Reflection due to performance issue
            labelJo[0] = lbl_Jo1; labelJo[1] = lbl_Jo2; labelJo[2] = lbl_Jo3; labelJo[3] = lbl_Jo4; labelJo[4] = lbl_Jo5; labelJo[5] = lbl_Jo6;
            labelJs[0] = lbl_Js1; labelJs[1] = lbl_Js2; labelJs[2] = lbl_Js3; labelJs[3] = lbl_Js4; labelJs[4] = lbl_Js5; labelJs[5] = lbl_Js6;
            rectJo[0] = rectJo1; rectJo[1] = rectJo2; rectJo[2] = rectJo3; rectJo[3] = rectJo4; rectJo[4] = rectJo5; rectJo[5] = rectJo6;
            labelJo_b[0] = lbl_Jo_b0; labelJo_b[1] = lbl_Jo_b1; labelJo_b[2] = lbl_Jo_b2; labelJo_b[3] = lbl_Jo_b3; labelJo_b[4] = lbl_Jo_b4; labelJo_b[5] = lbl_Jo_b5;
            
            this.Title = YangpaConstants.AppTitle;
            for (int i = 0; i < labelJo.Length; i++)
            {
                labelJo[i].Content = (i + 1) + "조";
            }
            for (int i = 0; i < labelJs.Length; i++)
            {
                labelJs[i].Content = "점";
            }
            foreach(Label l in labelJo_b)
            {
                l.Content = "";
            }

            lbl_Debug.Content = "";
            lbl_Debug_Version.Content = YangpaConstants.Version;
            lbl_Info_Ver.Content = "Version " + YangpaConstants.Version;

            cb_Instance.Text = DateTime.Now.ToShortDateString();
            //header:prompt to add new object
            cb_Class.Items.Add("새로 추가...");

            //timer for alternate random
            timer.Tick += timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(100);
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            //should use delegate;
            this.core = new YangpaCore(this);
            //loads class and init
            try
            {
                foreach (SClass cl in core.LoadClasses())
                {
                    cb_Class.Items.Add(cl);
                }
            }
            catch (FileLoadException)
            {
                MessageBox.Show(YangpaConstants.MSG_INV_CONF, YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
            //add some yangpa    
            YangpaObject.SetEnvironment(canv_YpHome, this, new YangpaObject.SimpleDelegate(Yangpa_Created), new YangpaObject.SimpleDelegate(Yangpa_Collided), rectJo);
            for (int i = 0; i < 4; i++)
            {
                YangpaObject ypo = new YangpaObject((YangpaObject.YType)i, new Point(80 * i, 10 + 20 * (i % 2)));
                canv_YpHome.Children.Add(ypo);
            }
            //YType.Half is 99
            //canv_YpHome.Children.Add(new YangpaObject(YangpaObject.YType.Half, new Point(240, 30)));
        }

        /// <summary>
        /// Called when YangpaObject delegate is collided
        /// </summary>
        /// <param name="y"></param>
        /// <param name="c"></param>
        private void Yangpa_Collided(YangpaObject y, int c)
        {
            //prevents dragging before class select
            if (core.instance_Current == null)
            {
                if (y != null)
                    ClearAllYangpa();
            }
            else if (core.instance_Current.JoMember[0].Count < 1 && y != null)
            {
                MessageBox.Show(YangpaConstants.MSG_SET_MEMBER_BEFORE_MOVE, YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                ClearAllYangpa();
            }
            else //starts code here
            {
                int[][] ia = core.instance_Current.JoScore;
                //initializes array once more
                for (int i = 0; i < ia.Length; i++)
                    for (int j = 0; j < ia[i].Length; j++)
                        ia[i][j] = 0;

                bool hasHalf = false;
                foreach (UIElement o in canv_YpHome.Children)
                {
                    YangpaObject yangpa = o as YangpaObject;
                    if (yangpa != null)
                    {
                        if (yangpa.Type == YangpaObject.YType.Half)
                            hasHalf = true;
                        else
                        {
                            int jindex = yangpa.CheckifinJorect();
                            if (jindex != -1)
                                ia[jindex][(int)yangpa.Type] += 1;
                        }
                    }
                }
                for (int i = 0; i < 6; i++)
                    labelJs[i].Content = core.instance_Current.JoScoreToActualScore(i) + "점";
                YangpaDB.SaveDB(core.instance_Current, false);

                if (hasHalf)
                    UpdateHalfYangpa();
            }
        }

        private void Yangpa_Created(YangpaObject ypo, int z)
        {
            canv_YpHome.Children.Add(ypo);
            Canvas.SetZIndex(ypo, z);
        }

        /// <summary>
        /// timer_tick for alternate random
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                Random r = new Random();
                //appropriate stop for prevents stack overflow
                bool flag = true;
                for (int i = 0; i < core.instance_Current.JoMember.Length; i++)
                {
                    List<string> member = core.instance_Current.JoMember[i];
                    //instance jomember is equal to label count
                    string lbl = labelJo[i].Content.ToString();
                    //remove last space (bug#20130501)
                    if (lbl.Remove(lbl.LastIndexOf(' ')).Split(' ').Length <1+ member.Count)
                        flag = false;
                }
                if (!flag)
                {
                    int cindex = r.Next(0, 6);
                    List<string> one = core.instance_Current.JoMember[cindex];
                    foreach (string n in one)
                    {
                        if (!labelJo[cindex].Content.ToString().Contains(n))
                        {
                            labelJo[cindex].Content += n + " ";
                            return;
                        }
                    }
                 //   timer_Tick(null, null);
                }
                else
                {
//                    MessageBox.Show("OK");
                    timer.Stop();
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Bug #1 NullReferenceException at timer_tick", YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Updates a label view.
        /// </summary>
        /// <param name="alt">parameter for triggering alternate random</param>
        public void UpdateView(bool alt)
        {
            if (alt)
            {
                for (int k = 0; k < core.instance_Current.JoMember.Length; k++)
                {
                    labelJo[k].Content = (k + 1) + "조 ";
                }

                //재밌는 랜덤 코드 구현
                if (timer.IsEnabled)
                    timer.Stop();

                timer.Start();
            }
            else
            {
                for (int i = 0; i < core.instance_Current.JoMember.Length; i++)
                {
                    List<String> one = core.instance_Current.JoMember[i];
                    string value = (i + 1) + "조 " + YangpaData.ConvertArrayToComma(one);
                    labelJo[i].Content = value;
                }
                for (int i = 0; i < labelJs.Length; i++)
                {
                    labelJs[i].Content = core.instance_Current.JoScoreToActualScore(i) + "점";
                }
            }

            for(int i = 0; i < labelJo_b.Length; i++)
            {
                labelJo_b[i].Content = YangpaData.ConvertArrayToComma(core.instance_Current.JoMember[i]);
            }
        }
        /// <summary>
        /// Updates a Yangpa display.
        /// </summary>
        private void UpdateYangpa()
        {
            //should clear it first
            ClearAllYangpa();
            /*
             * q=JoIndex, i=YangpaType, j=count
             */
            for (int q = 0; q < core.instance_Current.JoScore.Length; q++)
            {
                int[] ia = core.instance_Current.JoScore[q];
                for (int i = 0; i < ia.Length; i++)
                {
                    for (int j = 1; j <= ia[i]; j++)
                    {
                        //TODO : need to be improved
                        Point point = new Point(20+ (j + i) * 30 + (q % 2 == 0 ? -270 : 60), 170 + 140 * (q / 2));
                        YangpaObject yo = new YangpaObject((YangpaObject.YType)i, point);
                        canv_YpHome.Children.Add(yo);
                    }
                }
            }
        }

        /// <summary>
        /// converts two half yangpa to one yangpa
        /// </summary>
        private void UpdateHalfYangpa()
        {
        }

        private void ClearAllYangpa()
        {
            //버그방지용 임시 코드
            for (int j = 0; j < 4; j++)
            {
                for (int i = 0; i < canv_YpHome.Children.Count; i++)
                {
                    YangpaObject yangpa = canv_YpHome.Children[i] as YangpaObject;
                    if (yangpa != null)
                    {
                        if (!yangpa.CheckifinHomerect())
                        {
                            yangpa.Source = null;
                            canv_YpHome.Children.RemoveAt(i);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// clears view
        /// </summary>
        private void ClearBase()
        {
            core.instance_Current = null;
            lbl_Debug_Version.Content = "instance : ";
            for (int i = 0; i < labelJo.Length; i++)
            {
                labelJo[i].Content = (i + 1) + "조";
            }
            ClearAllYangpa();
        }

        /// <summary>
        /// callback function for member alteration command
        /// </summary>
        /// <param name="newclass"></param>
        private void SetMember_Finished(SClass newclass)
        {
            bool flag = false;
            foreach (SClass cl in core.Classes)
            {
                if (cl.Name.Equals(newclass.Name))
                {
                    MessageBox.Show("수정 완료", YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                    flag = true;
                    cl.Students = newclass.Students;
                    cl.Captains = newclass.Captains;
                }
            }
            cb_Class.Text = newclass.Name;
            if (!flag)
            {
                MessageBox.Show("추가 완료", YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                core.Classes.Add(newclass);
                cb_Class.Items.Add(newclass);
            }
            core.SaveClasses();
        }

        private void OpenSetmemberW(SClass cls)
        {
            Window_SetMember wsm = new Window_SetMember(cls, new Window_SetMember.SimpleDelegate(SetMember_Finished));
            wsm.ShowDialog();
        }
        private void OpenSetmemberW()
        {
            Window_SetMember wsm = new Window_SetMember(new Window_SetMember.SimpleDelegate(SetMember_Finished));
            wsm.ShowDialog();
        }

        private void TranslateTimerColor(bool toStart)
        {
            Color blue = Color.FromArgb(204, 51, 153, 255);
            Color green = Color.FromArgb(200, 0, 255, 100);
            //            Color pink = Color.FromArgb(200, 255, 0, 100);
            var anim = new ColorAnimation
            {
                From = toStart ? blue : ((SolidColorBrush)Canvas_Timer.Background).Color,
                To = toStart ? green : blue,
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            var storyboard = new Storyboard();

            storyboard.Children.Add(anim);
            Storyboard.SetTarget(anim, Canvas_Timer);
            Storyboard.SetTargetProperty(anim, new PropertyPath("Background.Color"));

            storyboard.Begin();
        }

        /// <summary>
        /// Load 'ONE' instance by intname
        /// </summary>
        /// <param name="intname"></param>
        /// <returns></returns>
        private SInstance LoadInstanceByName(string intname)
        {
            return YangpaDB.LoadDB(core.class_Current.Name, intname);
        }

        /// <summary>
        /// Load 'ALL' instances used at combobox
        /// </summary>
        /// <param name="clsname"></param>
        /// <returns></returns>
        private void LoadInstances(string clsname)
        {
            cb_Instance.Items.Clear();
            if (core.class_Current == null)
                MessageBox.Show(YangpaConstants.MSG_CLS_NOT_SELECTED, YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            else
            {
                foreach (string sint in YangpaDB.LoadAllDBName(clsname))
                {
                    cb_Instance.Items.Add(sint);
                }
            }
        }

        /// <summary>
        /// Create new int at specific name
        /// </summary>
        /// <param name="intname"></param>
        private bool CreateNewInstance(string intname)
        {
            if (core.class_Current == null)
            {
                MessageBox.Show(YangpaConstants.MSG_CLS_NOT_SELECTED, YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            else
            {
                core.instance_Current = new SInstance(intname, core.class_Current.Name);
                string sqlres = YangpaDB.InsertDB(core.instance_Current);

                if (sqlres != "")
                {
                    MessageBox.Show(sqlres, YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    core.instance_Current = null;
                    return false;
                }
                else
                {
                    cb_Instance.Items.Add(cb_Instance.Text);
                    return true;
                }
            }
        }

        /// <summary>
        /// instance value > tb text
        /// </summary>
        private void ManipulateInstanceReverse()
        {
            TextBox[] tbjm = new TextBox[6];
            tbjm[0] = tb_JoMember1; tbjm[1] = tb_JoMember2; tbjm[2] = tb_JoMember3; tbjm[3] = tb_JoMember4; tbjm[4] = tb_JoMember5; tbjm[5] = tb_JoMember6;
            TextBox[] tbjs = new TextBox[6];
            tbjs[0] = tb_JoScore1; tbjs[1] = tb_JoScore2; tbjs[2] = tb_JoScore3; tbjs[3] = tb_JoScore4; tbjs[4] = tb_JoScore5; tbjs[5] = tb_JoScore6;

            for (int i = 0; i < tbjm.Length; i++)
            {
                tbjm[i].Text = YangpaData.ConvertArrayToComma(core.instance_Current.JoMember[i]);
            }
            for (int i = 0; i < tbjs.Length; i++)
            {
                //JoscoreToString starts at 1
                tbjs[i].Text = core.instance_Current.JoScoreToString(i+1);
            }
        }
        /// <summary>
        /// tb text > instance value
        /// </summary>
        /// <returns></returns>
        private bool ManipulateInstance()
        {
            string[] tbjm = new string[6];
            tbjm[0] = tb_JoMember1.Text; tbjm[1] = tb_JoMember2.Text; tbjm[2] = tb_JoMember3.Text; tbjm[3] = tb_JoMember4.Text; tbjm[4] = tb_JoMember5.Text; tbjm[5] = tb_JoMember6.Text;
            string[] tbjs = new string[6];
            tbjs[0] = tb_JoScore1.Text; tbjs[1] = tb_JoScore2.Text; tbjs[2] = tb_JoScore3.Text; tbjs[3] = tb_JoScore4.Text; tbjs[4] = tb_JoScore5.Text; tbjs[5] = tb_JoScore6.Text;

            for (int i = 0; i < tbjm.Length; i++)
            {
                if (string.IsNullOrEmpty(tbjm[i].Replace(" ", "")))
                    return false;
                else
                {
                    List<string> slist = YangpaData.ConvertCommaToArray(tbjm[i]);
                    foreach (string s in slist)
                    {
                        if (!core.class_Current.Students.Contains(s))
                        {
                            if (MessageBox.Show(YangpaConstants.MSG_INST_NO_MEMBER_IN_CLASS, YangpaConstants.AppTitle, MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No)
                                return false;
                            else
                                break;
                        }
                    }
                    core.instance_Current.JoMember[i] = slist;
                }
            }

            try
            {
                for (int i = 0; i < tbjs.Length; i++)
                {
                    if (string.IsNullOrEmpty(tbjs[i].Replace(" ", "")))
                        return false;
                    else if (tbjs[i].Contains("-"))
                        return false;
                    else
                    {
                        int[] ia = YangpaData.ExplodeScore(tbjs[i]);
                        //점수 칸의 항목이 4보다 크면 안됨
                        if (ia.Length > 4) throw new OverflowException();
                        else core.instance_Current.JoScore[i] = ia;
                    }
                }
            }
            catch (FormatException)
            { return false; }
            catch (OverflowException)
            { return false; }

            YangpaDB.SaveDB(core.instance_Current, true);
            UpdateView(false);
            UpdateYangpa();
            return true;
        }

    }
}
