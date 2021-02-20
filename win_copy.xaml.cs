using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace workday2Excel
{
    /// <summary>
    /// Interaction logic for test.xaml
    /// </summary>
    public partial class win_copy : Window
    {
        private string[] list;
        public win_copy(List<string> liStr)
        {
            InitializeComponent();
            list = liStr.ToArray();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            label_dayCnt.Content = String.Format("평일: {0}개", list.Length);
            textbox_h.Text = String.Join("	", list);
            textbox_v.Text = String.Join("\n", list);
        }

        private void button_h_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(textbox_h.Text);
            var prevStr = button_h.Content;
            button_h.Content = "복사 됨";
            Thread.Sleep(500);
            button_h.Content = prevStr;
        }

        private void button_v_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(textbox_v.Text);
            var prevStr = button_v.Content;
            button_v.Content = "복사 됨";
            Thread.Sleep(500);
            button_v.Content = prevStr;
        }
    }
}
