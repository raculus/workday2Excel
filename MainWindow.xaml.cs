using Nager.Date;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;

namespace workday2Excel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private List<string> workDay(DateTime dt, string days)
        {
            int year = dt.Year;
            int month = dt.Month;
            var igDays = days.Split(' ');
            int lastDay = DateTime.DaysInMonth(year, month);
            List<string> workdayList = new List<string>();

            for (int day = 1; day < lastDay+1; day++)
            {
                var date = new DateTime(year, month, day);
                if (DateSystem.IsPublicHoliday(date, CountryCode.KR))
                    continue;
                else if (DateSystem.IsWeekend(date, CountryCode.KR))
                    continue;
                workdayList.Add(date.ToString("yyyy-MM-dd"));
            }
            foreach (var igDay in igDays)
            {
                try
                {
                    int day = Int32.Parse(igDay);
                    var date = new DateTime(year, month, day).ToString("yyyy-MM-dd");
                    if (workdayList.Contains(date))
                        workdayList.Remove(date);
                }
                catch
                {
                    if (igDay != "")
                    {
                        MessageBox.Show("제외할 날짜가 올바르지 않습니다.\n정상적인 날짜의 숫자를 입력해주십시오.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    }
                    else if (igDay == "")
                        break;
                }
            }
            return workdayList;
        }
                
        private static bool IsTextAllowed(string text)
        {
            var regex = new Regex(@"[^0-9.-]+");
            return regex.IsMatch(text);
        }

        private void AcrylicWindow_Loaded(object sender, RoutedEventArgs e)
        {
            datePicker.SelectedDate = DateTime.Now.AddMonths(1);    //올해 다음달 설정
            var date = (DateTime)datePicker.SelectedDate;
            label_date.Content = date.ToString("yyyy년MM월"); //라벨 수정

            /* 이전 창 위치 복원 */
            var location = Properties.Settings.Default.location;
            this.Left = location.X;
            this.Top = location.Y;

            refresh_workdays();
        }

        private void AcrylicWindow_Closed(object sender, EventArgs e)
        {
            var location = new Point(this.Left, this.Top);
            Properties.Settings.Default.location = location;
            Properties.Settings.Default.Save();
        }

        private void textbox_ignoreList_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (IsTextAllowed(e.Text))
            {
                e.Handled = true;                
            }
            else
            {
                e.Handled = false;
            }
        }

        private void button_Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("주말, 공휴일, 대체공휴일을 제외한 평일 텍스트를 만들어줍니다.\n선거일같은 규칙없는 휴일은 수동추가 바랍니다.", "도움말");
        }

        private void datePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            var date = (DateTime)datePicker.SelectedDate;
            label_date.Content = date.ToString("yyyy년MM월");

            refresh_workdays();
        }

        private void refresh_workdays()
        {
            var date = (DateTime)datePicker.SelectedDate;
            var workList = workDay(date, textbox_ignoreList.Text);
            label_workCount.Content = String.Format("평일: {0}개", workList.Count);
            textbox_H.Text = String.Join("	", workList);   //탭문자
            textbox_V.Text = String.Join("\n", workList);   //개행
        }

        private void button_CopyH_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(textbox_H.Text);
        }

        private void button_CopyV_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(textbox_V.Text);
        }

        private void textbox_ignoreList_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            refresh_workdays();
        }
    }
}
