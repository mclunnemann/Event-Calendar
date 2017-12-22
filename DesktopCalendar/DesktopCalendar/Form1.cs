using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace DesktopCalendar
{
    public partial class Form1 : Form
    {
        Dictionary<DateTime, string> CalendarInfo = new Dictionary<DateTime, string>();
        DateTime date = new DateTime();
        string dateInfo = "";
        DateTime StartDate = new DateTime();
        DateTime EndingDate = new DateTime();
        DateTime[] dt;        
        string file = "TEST_DateAppointments.txt";
        string tempFile = "DateAppointments_TEMP.txt";
        public Form1()
        {
            InitializeComponent();
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            date = e.Start;
            foreach (KeyValuePair<DateTime, string> entry in CalendarInfo)
            {
                if (e.Start == entry.Key)
                {
                    rtbInfo.Text = entry.Value;
                    lblDisplayInfo.Visible = true;
                    lblDisplayInfo.Text = entry.Value;
                    ResizeForm();
                    btnRemove.Enabled = true;
                    break;
                }
                else
                {
                    rtbInfo.Text = "";
                    lblDisplayInfo.Visible = false;
                    btnRemove.Enabled = false;
                    ResizeForm();
                }
            }

            string strDate = Convert.ToString(date.Month + "/" + date.Day + "/" + date.Year);
            label1.Text = Convert.ToString("Events on " + strDate + ":");
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string strDate = Convert.ToString(date.Month + "/" + date.Day + "/" + date.Year);
            dateInfo = rtbInfo.Text;
            //MessageBox.Show(strDate + " - " + dateInfo);
            CalendarInfo[Convert.ToDateTime(strDate)] = dateInfo;
            lblDisplayInfo.Text = dateInfo;
            WriteToFile();
            formLoad();
            ResizeForm();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            formLoad();
            date = monthCalendar1.TodayDate;
        }

        private void rtbInfo_TextChanged(object sender, EventArgs e)
        {
            if (rtbInfo.Text != "")
            {
                btnSubmit.Enabled = true;              
            }
            else
            {
                btnSubmit.Enabled = false;
            }
        }

        private void WriteToFile()
        {
            using (StreamWriter writer = new StreamWriter(file)) {
                foreach (KeyValuePair<DateTime, string> entry in CalendarInfo)
                {
                    string replacement = Regex.Replace(entry.Value, @"\t|\n|\r", "!NEWLINE!");
                    writer.WriteLine(entry.Key + "|" + replacement);
                }
                writer.Close();
            }
        }

        private void ReadFile()
        {
            string line;
            DateTime cutOffDate = (monthCalendar1.TodayDate.AddDays(-30));
            if (!File.Exists(file))
            {
                File.Create(file);
            }
            else
            {
                if (new FileInfo(file).Length != 0)
                {
                    using (StreamReader reader = new StreamReader(file))
                    {
                        while ((line = reader.ReadLine()) != null)
                        {
                            string[] arr = line.Split('|');
                            if (Convert.ToDateTime(arr[0]) >= cutOffDate)
                            {
                                string replacement = Regex.Replace(arr[1],"!NEWLINE!","\n");
                                CalendarInfo[Convert.ToDateTime(arr[0])] = replacement;                                
                            }
                        }
                        reader.Close();
                    }
                }
            }            
        }

        public void DeleteFromFile()
        {            
            if (new FileInfo(file).Length != 0)
            {
                using (StreamReader reader = new StreamReader(file))
                using (StreamWriter writer = new StreamWriter(tempFile))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] arr = line.Split('|');
                        if (Convert.ToDateTime(arr[0]) != monthCalendar1.SelectionStart)
                        {
                            writer.WriteLine(line);
                        }
                    }
                }
                File.Delete(file);
                File.Move(tempFile, file);
            }
            formLoad();
        }
        
        public void formLoad()
        {
            btnSubmit.Enabled = false;
            btnRemove.Enabled = false;
            lblDisplayInfo.Visible = false;
            ReadFile();
            StartDate = (monthCalendar1.TodayDate.AddDays(-29));
            EndingDate = (monthCalendar1.TodayDate.AddDays(60));
            dt = new DateTime[CalendarInfo.Count];
            int i = 0;
            foreach (KeyValuePair<DateTime, string> entry in CalendarInfo)
            {
                if (entry.Key >= StartDate && entry.Key <= EndingDate)
                {
                    dt[i] = entry.Key;
                    i++;
                }
            }
            monthCalendar1.BoldedDates = dt;
            monthCalendar1.UpdateBoldedDates();
            ;//breakpoint
            monthCalendar1.SelectionStart = monthCalendar1.TodayDate;
            date = monthCalendar1.SelectionStart;
            if (CalendarInfo.ContainsKey(monthCalendar1.TodayDate))
            {
                if (CalendarInfo[monthCalendar1.TodayDate] != "")
                {
                    rtbInfo.Text = CalendarInfo[monthCalendar1.TodayDate];
                    btnRemove.Enabled = true;
                    lblDisplayInfo.Visible = true;
                    lblDisplayInfo.Text = CalendarInfo[monthCalendar1.TodayDate];
                }
            }
            else
            {
                lblDisplayInfo.Text = "";
            }
            string strDate = Convert.ToString(date.Month + "/" + date.Day + "/" + date.Year);
            label1.Text = Convert.ToString("Events on " + strDate + ":");
            ResizeForm();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            CalendarInfo.Remove(monthCalendar1.SelectionStart);
            DeleteFromFile();
            rtbInfo.Text = "";
            formLoad();
        }

        private void ResizeForm()
        {
            this.Update();
            this.Size = new System.Drawing.Size(this.Width, ((monthCalendar1.Height + 10) + (label1.Height + 10) + (lblDisplayInfo.Height + 50)));
            this.Padding = new Padding(0, 0, 0, 10);
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Update();
        }

    }
}
