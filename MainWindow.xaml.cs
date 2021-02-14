using Nager.Date;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        private void button_Generate_Click(object sender, RoutedEventArgs e)
        {
            var year = textbox_Year.Text;
            var month = textbox_Month.Text;
            var days = textbox_ignoreList.Text;
            var list = workDay(year, month, days);
            string filename = year + "년 평일.xlsx";
            writeExcel(list, filename, year, month, true);
            writeExcel(list, filename, year, month, false);
            if (MessageBox.Show("완료되었습니다.\n파일을 여시겠습니까?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Process.Start(filename);
        }
        private void writeExcel(List<string> list, string filename, string year, string month, bool vertical)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var fi = new FileInfo(filename);
            int row = 2;
            int col = 1;
            if (vertical)
            {
                row = 1;
                col = 2;
            }
            string sheetname = year + "-" + month;
            using (ExcelPackage package = new ExcelPackage(fi))
            {
                var wb = package.Workbook;
                ExcelWorksheet ws;
                if (hasWorksheet(filename, sheetname))
                    ws = package.Workbook.Worksheets[sheetname];
                else
                    ws = package.Workbook.Worksheets.Add(sheetname);
                foreach (var str in list)
                {
                    ws.Cells[row, col].Value = str;

                    if (vertical)
                        row++;
                    else
                        col++;
                }
                package.Save();
            }
        }
        private bool hasWorksheet(string filepath, string sheetname)
        {
            var fi = new FileInfo(filepath);
            using (ExcelPackage package = new ExcelPackage(fi))
            {
                var wb = package.Workbook;
                foreach (var sheet in wb.Worksheets)
                {
                    if (sheet.Name.Equals(sheetname))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private List<string> workDay(string strYear, string strMonth, string days)
        {
            int year = Convert.ToInt32(strYear);
            int month = Convert.ToInt32(strMonth);
            var igDays = days.Split(' ');
            int lastDay = DateTime.DaysInMonth(year, month);
            List<string> workdayList = new List<string>();

            for (int day = 1; day < lastDay; day++)
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
                int day = Convert.ToInt32(igDay);
                var date = new DateTime(year, month, day).ToString("yyyy-MM-dd");
                if (workdayList.Contains(date))
                    workdayList.Remove(date);
            }
            return workdayList;
        }

        private void textbox_Year_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = IsTextAllowed(e.Text);
        }
        private static readonly Regex _regex = new Regex("[^0-9]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return _regex.IsMatch(text);
        }

        private void textbox_Month_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = IsTextAllowed(e.Text);
        }

        private void AcrylicWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.firstRun == true)
            {
                MessageBox.Show("주말, 공휴일, 대체공휴일을 제외한 평일을 엑셀파일로 만들어줍니다.\n선거일같은 규칙없는 휴일은 수동추가 바랍니다.", "도움말");
                Properties.Settings.Default.firstRun = false;
                Properties.Settings.Default.Save();
            }
            var location = Properties.Settings.Default.location;
            this.Left = location.X;
            this.Top = location.Y;

            textbox_Year.MaxLength = 4;
            textbox_Month.MaxLength = 2;

            var now = System.DateTime.Now;
            string year = now.ToString("yyyy");
            string month = now.AddMonths(+1).ToString("MM");

            textbox_Year.Text = year;
            textbox_Month.Text = month;
        }

        private void AcrylicWindow_Closed(object sender, EventArgs e)
        {
            var location = new Point(this.Left, this.Top);
            Properties.Settings.Default.location = location;
            Properties.Settings.Default.Save();
        }

        private void textbox_ignoreList_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = IsTextAllowed(e.Text);
        }

        private void button_Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("주말, 공휴일, 대체공휴일을 제외한 평일을 엑셀파일로 만들어줍니다.\n선거일같은 규칙없는 휴일은 수동추가 바랍니다.", "도움말");
        }
    }
}
