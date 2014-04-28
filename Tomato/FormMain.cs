using Microsoft.WindowsAPICodePack.Taskbar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tomato.Properties;

namespace Tomato
{
    public partial class FormMain : Form
    {
        private int pomodoroCount = 0;
        private Timer timer = new Timer();
        private bool pomodoroBreak = false;

        public FormMain()
        {
            InitializeComponent();

            timer.Interval = 1000;
            timer.Tick += timer_Tick;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Hide();
            
            timer.Enabled = true;
            lblStatus.Visible = true;

            if (pomodoroBreak)
            {
                lblStatus.Text = "Break time!";
            }
            else
            {
                lblStatus.Text = "Pomodoro in progress";
            }

            int minutes = GetMinutes();
            int seconds = GetSeconds();

            progressBarTimer.Show();
            progressBarTimer.Maximum = CalculateTotalSeconds(minutes, seconds);
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.Normal);
        }

        private static int CalculateTotalSeconds(int minutes, int seconds)
        {
            var totalSeconds = seconds + minutes * 60;
            return totalSeconds;
        }

        private int GetSeconds()
        {
            int seconds = Convert.ToInt32(Regex.Match(lblTimer.Text, @":(\d+)").Groups[1].Value);
            return seconds;
        }

        private int GetMinutes()
        {
            int minutes = Convert.ToInt32(Regex.Match(lblTimer.Text, @"(^\d+)").Groups[1].Value);
            return minutes;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            int minutes = GetMinutes();
            int seconds = GetSeconds();
            
            if (seconds == 0)
            {
                if (minutes > 0)
                {
                    minutes--;
                }
                
                seconds = 59;
            }
            else
            {
                seconds--;
            }

            UpdateProgress(minutes, seconds);

            if (minutes == 0 && seconds == 0)
            {
                btnStart.Show();
                ResetProgress();
                timer.Enabled = false;
                progressBarTimer.Hide();
                lblStatus.Visible = false;

                // times up!
                PlaySound();

                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new MethodInvoker(() => { NewPomodoro(); }));
                }
                else
                {
                    NewPomodoro();
                }
            }
        }

        private void NewPomodoro()
        {
            if (pomodoroBreak)
            {
                pomodoroBreak = false;
                btnStart.Text = "Start";
                lblTimer.Text = "25:00";
            }
            else
            {
                pomodoroCount++;
                pomodoroBreak = true;
                btnStart.Text = "Break";

                if (pomodoroCount == 4)
                {
                    pomodoroCount = 0;

                    // 15 minute break
                    lblTimer.Text = "15:00";
                }
                else
                {
                    // 3 minute break
                    lblTimer.Text = "03:00";
                }
            }

            PopupWindow();
        }

        private void PopupWindow()
        {
            this.TopMost = true;
            this.TopMost = false;
            this.Activate();
            this.WindowState = FormWindowState.Normal;
        }

        private static void PlaySound()
        {
            var player = new SoundPlayer();
            player.Stream = Resources.ResourceManager.GetStream("EggTimerRinging");
            player.Play();
        }

        private void ResetProgress()
        {
            progressBarTimer.Value = 0;
            TaskbarManager.Instance.SetProgressState(TaskbarProgressBarState.NoProgress);
        }

        private void UpdateProgress(int minutes, int seconds)
        {
            progressBarTimer.Value += 1;
            TaskbarManager.Instance.SetProgressValue(progressBarTimer.Value, progressBarTimer.Maximum);
            lblTimer.Text = string.Format("{0}:{1}", minutes < 10 ? "0" + minutes.ToString() : minutes.ToString(), seconds < 10 ? "0" + seconds.ToString() : seconds.ToString());
        }
    }
}
