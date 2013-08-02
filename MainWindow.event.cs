using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace YangpaH
{
    public partial class MainWindow : Elysium.Controls.Window
    {

        private void btn_ExportasExcel_Click(object sender, RoutedEventArgs e)
        {
            if (core.class_Current == null)
                MessageBox.Show(YangpaConstants.MSG_CLS_NOT_SELECTED, YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            else
            {
                var result = YangpaExcelManager.SaveExcelDatas(YangpaDB.LoadAllDB(core.class_Current.Name), core.class_Current.Students);
                if (result == YangpaConstants.MSG_EXC_EXP_SUCCESS)
                    MessageBox.Show(result
                , YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show(result
            , YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btn_ModifyInstance_Click(object sender, RoutedEventArgs e)
        {
            if (core.instance_Current == null)
                MessageBox.Show(YangpaConstants.MSG_INS_NOT_SELECTED, YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            else
            {
                if (Canvas_ModityInst.Visibility == Visibility.Hidden)
                {
                    ManipulateInstanceReverse();
                    Canvas_ModityInst.Visibility = Visibility.Visible;
                    var anim = new DoubleAnimation
                    {
                        From = 0.0,
                        To = 1.0,
                        BeginTime = TimeSpan.FromSeconds(0),
                        Duration = new Duration(TimeSpan.FromSeconds(0.3))
                    };
                    var storyboard = new Storyboard();

                    storyboard.Children.Add(anim);
                    Storyboard.SetTarget(anim, Canvas_ModityInst);
                    Storyboard.SetTargetProperty(anim, new PropertyPath(OpacityProperty));

                    storyboard.Begin();
                    btn_GiantTransparentCancel.Visibility = Visibility.Visible;
                }
            }
        }

        private void btn_GiantTransparentCancel_Click(object sender, RoutedEventArgs e)
        {
            btn_GiantTransparentCancel.Visibility = Visibility.Hidden;
            var anim = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = new Duration(TimeSpan.FromSeconds(0.3))
            };
            var storyboard = new Storyboard();

            storyboard.Children.Add(anim);
            Storyboard.SetTarget(anim, Canvas_ModityInst);
            Storyboard.SetTargetProperty(anim, new PropertyPath(OpacityProperty));
            storyboard.Completed += (sender1, e1) =>
            {
                Canvas_ModityInst.Visibility = Visibility.Hidden;
            };
            storyboard.Begin();
        }

        private void btn_Modify_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(YangpaConstants.MSG_CONFIRM_DELETE_INST, YangpaConstants.AppTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                string res = YangpaDB.DeleteDB(core.instance_Current.Id);
                if (res == string.Empty)
                {
                    cb_Instance.Items.Remove(core.instance_Current.Name);
                    this.ClearBase();
                    Canvas_ModityInst.Visibility = Visibility.Hidden;
                    btn_GiantTransparentCancel.Visibility = Visibility.Hidden;
                    MessageBox.Show("삭제 완료", YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                    MessageBox.Show(res, YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_Modify_Save_Click(object sender, RoutedEventArgs e)
        {
            if (ManipulateInstance())
            {
                MessageBox.Show("수정 완료", YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Information);
                Canvas_ModityInst.Visibility = Visibility.Hidden;
                btn_GiantTransparentCancel.Visibility = Visibility.Hidden;
            }
            else
            {
                MessageBox.Show(YangpaConstants.MSG_EMPTY_VALUE, YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Canvas_Timer_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.btn_Info_Click(null, null);
        }

        private void btn_Info_Click(object sender, RoutedEventArgs e)
        {
            //Image_Info.SetValue(OpacityProperty, 0.0);
            //lbl_Info_Ver.SetValue(OpacityProperty, 0.0);

            var anim = new DoubleAnimation
            {
                From = Canvas_Timer.Visibility == Visibility.Visible ? 0.95 : 0.0,
                To = Canvas_Timer.Visibility == Visibility.Visible ? 0.0 : 0.95,
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = new Duration(TimeSpan.FromSeconds(0.4))
            };
            /*var anim1 = new DoubleAnimation
            {
                From =  0.0,
                To = 1.0,
                BeginTime = TimeSpan.FromSeconds(0.7),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            var anim2 = anim1.Clone();
            */
            var storyboard = new Storyboard();
            storyboard.Children.Add(anim);
            Storyboard.SetTarget(anim, Canvas_Timer);
            Storyboard.SetTargetProperty(anim, new PropertyPath(OpacityProperty));
            if (Canvas_Timer.Visibility == Visibility.Visible)
                storyboard.Completed += delegate { Canvas_Timer.Visibility = Visibility.Hidden; };
            else
                Canvas_Timer.Visibility = Visibility.Visible;

            //should not run when driven by mouseclick event below
            /*if (e != null)
            {
                storyboard.Children.Add(anim1);
                storyboard.Children.Add(anim2);
                Storyboard.SetTarget(anim1, Image_Info);
                Storyboard.SetTarget(anim2, lbl_Info_Ver);
                Storyboard.SetTargetProperty(anim1, new PropertyPath(OpacityProperty));
                Storyboard.SetTargetProperty(anim2, new PropertyPath(OpacityProperty));
            }*/
            storyboard.Begin();

        }

        private void Image_Info_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.btn_Info_Click(null, null);
        }

        private void btn_DeleteYangpa_Click(object sender, RoutedEventArgs e)
        {
            this.ClearAllYangpa();
            this.Yangpa_Collided(null, 0);
        }

        private void cb_Class_DropDownClosed(object sender, EventArgs e)
        {
            //not a good code, why should I use try-catch>???????
            try
            {
                if (timer != null)
                    timer.Stop();

                if (cb_Class.SelectedItem.ToString().Contains("추가"))
                {
                    this.OpenSetmemberW();
                }
                else
                {
                    ClearBase();

                    core.class_Current = (cb_Class.SelectedItem as SClass);
                    lbl_Debug.Content = core.class_Current.Name + ": " + core.class_Current.Students.Count + "명";

                    //It causes a FileNotfoundException!
                    this.LoadInstances(core.class_Current.Name);
                    this.cb_Instance.IsEnabled = true;
                }
            }
            catch (IndexOutOfRangeException)
            { }
            catch (NullReferenceException)
            {

            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.ToString());
            }
        }

        /// <summary>
        /// Combobox InstanceChanged 대신 DropdownClosed 사용
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_Instance_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                //not a good code
                if (timer != null)
                    timer.Stop();

                if (string.IsNullOrEmpty(cb_Instance.Text.Replace(" ", "")))
                { }
                else
                {

                    //신의 한수
                    if (cb_Instance.SelectedItem != null)
                    {
                        SInstance si = LoadInstanceByName(cb_Instance.Text);
                        if (si == null)
                            MessageBox.Show(YangpaConstants.MSG_DB_NOT_EXIST, YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                        else
                        {
                            core.instance_Current = si;
                            UpdateView(false);
                            UpdateYangpa();
                            lbl_Debug_Version.Content = "instance : " + core.instance_Current.Name;
                        }
                    }
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Bug #2 cb_Instance is null", YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btn_SetCaptain_Click(object sender, RoutedEventArgs e)
        {
            if (core.class_Current == null)
                MessageBox.Show(YangpaConstants.MSG_CLS_NOT_SELECTED, YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            else
                this.OpenSetmemberW(core.class_Current);
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (btn_SetCaptain != null)
                btn_SetCaptain.IsEnabled = cb_Captainfixed.IsChecked.HasValue ? cb_Captainfixed.IsChecked.Value : true;
        }

        private void btn_StartTimer_Click(object sender, RoutedEventArgs e)
        {
            //Timer related func - NOT USED
            /*var anim = new DoubleAnimation
            {
                From = Canvas_Timer.Visibility == Visibility.Visible ? 1.0 : 0.0,
                To = Canvas_Timer.Visibility == Visibility.Visible ? 0.0 : 1.0,
                //FillBehavior = FillBehavior.Stop,
                BeginTime = TimeSpan.FromSeconds(0),
                Duration = new Duration(TimeSpan.FromSeconds(0.5))
            };
            var storyboard = new Storyboard();

            storyboard.Children.Add(anim);
            Storyboard.SetTarget(anim, Canvas_Timer);
            Storyboard.SetTargetProperty(anim, new PropertyPath(OpacityProperty));

            if (Canvas_Timer.Visibility == Visibility.Visible)
                storyboard.Completed += delegate { Canvas_Timer.Visibility = Visibility.Hidden; };
            else
                Canvas_Timer.Visibility = Visibility.Visible;
            storyboard.Begin();*/
        }

        private void btn_Setrdm_Click(object sender, RoutedEventArgs e)
        {
            if (core.class_Current == null)
                MessageBox.Show(YangpaConstants.MSG_CLS_NOT_SELECTED, YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            else if (core.instance_Current == null)
                MessageBox.Show(YangpaConstants.MSG_INS_NOT_SELECTED, YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            else
            {
                if (canv_YpHome.Children.Count > 4)
                {
                    if(MessageBox.Show(YangpaConstants.MSG_ASK_SCRAMBLE_WARN, YangpaConstants.AppTitle, MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                        core.DoRandom(cb_Captainfixed.IsChecked.Value, cb_NewRandom.IsChecked.Value);
                }
                else
                    core.DoRandom(cb_Captainfixed.IsChecked.Value, cb_NewRandom.IsChecked.Value);
            }
        }

        /// <summary>
        /// Enter 키를 누름으로 새로 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_Instance_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void btn_NewInstance_Click(object sender, RoutedEventArgs e)
        {
            cb_Instance.Text = YangpaData.ConvertSQLDangerousData(cb_Instance.Text);
            if (string.IsNullOrEmpty(cb_Instance.Text.Replace(" ", "")))
                MessageBox.Show(YangpaConstants.MSG_ENTER_NAME, YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
            else
            {
                if (CreateNewInstance(cb_Instance.Text))
                {
                    //autoselects newly created instance
                    cb_Instance.SelectedItem = cb_Instance.Text;
                    this.cb_Instance_DropDownClosed(null, null);
                }
            }
        }

        private void btn_TmrStart_Click(object sender, RoutedEventArgs e)
        {/*
            float span = 0f;
            if (btn_TmrStart.Content.ToString() == "중지")
            {
                TranslateTimerColor(false);
                btn_TmrStart.Content = "시작";
                YangpaTimer.Stop();
            }
            else
            {
                TranslateTimerColor(true);
                btn_TmrStart.Content = "중지";
                YangpaTimer.CountHandler = new EventHandler((object senders, EventArgs e1) =>
                {
                    span = (YangpaTimer.CONST_COUNT_DESTINATION - YangpaTimer.counter)*255/420;
                    if (span < 0) span = 0;

//                    this.lbl_TmrDebug.Content = "dest : " + YangpaTimer.CONST_COUNT_DESTINATION + "\nctr : " + YangpaTimer.counter + "\nspan : " + span;
                   
                    this.lbl_Tmr.Content = "00 : " + YangpaTimer.Minute + " : " + YangpaTimer.Second;
                    this.Canvas_Timer.Background = new SolidColorBrush(Color.FromArgb(204, (byte)(255 - span), (byte)span, 100));
                });
                YangpaTimer.StoppedHandler = new EventHandler((object senders, EventArgs e1) =>
                {
                    this.lbl_Tmr.Content = "00 : 00 : 00";
                });
                YangpaTimer.Start();
            }*/
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            YangpaDB.MirrorDB();
        }
        
    }
}
