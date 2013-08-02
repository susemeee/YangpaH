using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace YangpaH
{
    /// <summary>
    /// Window_SetMember.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Window_SetMember : Window
    {
        public delegate void SimpleDelegate(SClass cls);
        private SimpleDelegate setMemberCallback;
        private SClass curclass;

        /// <summary>
        /// When modifys existing class
        /// </summary>
        /// <param name="curclass"></param>
        /// <param name="del"></param>
        public Window_SetMember(SClass curclass, SimpleDelegate del)
        {
            InitializeComponent();
            this.Title = "반 정보 수정";
            this.setMemberCallback = del;
            this.curclass = curclass;
            tb_ClassMember.Text = YangpaData.ConvertArrayToComma(curclass.Students);
            tb_ClassCaptain.Text = YangpaData.ConvertArrayToComma(curclass.Captains);
            tb_ClassName.Text = curclass.Name;
        }

        /// <summary>
        /// When creates a new class
        /// </summary>
        /// <param name="del"></param>
        public Window_SetMember(SimpleDelegate del)
        {
            curclass = new SClass();
            InitializeComponent();
            this.Title = "반 정보 추가";
            setMemberCallback = del;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (isValid())
            {
                curclass.Captains = YangpaData.ConvertCommaToArray(tb_ClassCaptain.Text);
                curclass.Name = tb_ClassName.Text;
                curclass.Students = YangpaData.ConvertCommaToArray(tb_ClassMember.Text);
                setMemberCallback.Invoke(curclass);
                this.Close();
            }
            else
            {
//                MessageBox.Show("잘못된 값이 있습니다.");
            }
        }

        private bool isValid()
        {
            List<string> check = YangpaData.ConvertCommaToArray(tb_ClassCaptain.Text);
            
            //조장이 6명이 되어야 함
            if (check.Count != 6)
            {
                MessageBox.Show(YangpaConstants.MSG_CAP_NOT_SIX, YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            //조장은 반 인원의 일부여야 함
            foreach (string sc in check)
            {
                if (!tb_ClassMember.Text.Contains(sc))
                {
                    MessageBox.Show(YangpaConstants.MSG_CAP_IS_NOT_MEMBER, YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return false;
                }
            }
            
            //중복 방지
            check.Sort();
            for (int i = 1; i <= check.Count - 1; i++)
            {
                if (check[i].ToString() == check[i - 1].ToString())
                {
                    MessageBox.Show(YangpaConstants.MSG_HAS_DUPLICATE, YangpaConstants.AppTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return false;
                }
            }

            return true;
        }

        private void cb_Class_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Window_KeyDown_1(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    this.Close();
                    break;
                case Key.Enter:
                    this.Button_Click_1(sender, null);
                    break;
            }
        }
    }
}
