using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

namespace WindowsFormsTest
{
    public partial class MainForm : Form
    {
        private System.Timers.Timer _timer;
        private System.Timers.Timer _timerDrawBorder;
        private bool _changeBorder;

        public MainForm()
        {
            InitializeComponent();
            InitializeTimers();

            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
        }

        private void InitializeTimers()
        {
            // Таймер для тика нажатой кнопки мышки.
            _timer = new System.Timers.Timer
            {
                Interval = 500,
                Enabled = false,
                AutoReset = true
            };
            _timer.Elapsed += OnTimerElapsed;

            // Таймер для кратковременной отрисовки рамки
            _timerDrawBorder = new System.Timers.Timer
            {
                Interval = 100,
                Enabled = false,
                AutoReset = true
            };
            _timerDrawBorder.Elapsed += OnTimerDrawBorderElapsed;
        }

        private void OnTimerElapsed(Object source, ElapsedEventArgs e)
        {
            _changeBorder = true;

            // Выполняем в потоке диспатчера.
            Invoke((MethodInvoker)delegate
            {
                if (string.IsNullOrWhiteSpace(tbInput.Text))
                {
                    Clipboard.SetText(tbInput.Text, TextDataFormat.UnicodeText);
                }

                label.Text = "Скопировано в буфер обмена";

                Refresh();
            });

            _timer.Stop();
            _timerDrawBorder.Enabled = true;
        }

        private void OnTimerDrawBorderElapsed(Object source, ElapsedEventArgs e)
        {
            _changeBorder = false;

            // Выполняем в потоке диспатчера.
            Invoke((MethodInvoker)delegate
            {
                label.Text = string.Empty;

                Refresh();
            });

            _timerDrawBorder.Stop();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            _timer.Enabled = true;
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            _timer.Enabled = false;
            _timerDrawBorder.Enabled = false;

            // Сбрасываем рамку, если отжали кнопку раньше.
            _changeBorder = false;
            Refresh();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            ChangeBorder(e, Color.Red);
        }

        private void ChangeBorder(PaintEventArgs e, Color color)
        {
            if (_changeBorder)
            {
                ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle,
                      Color.Red, 2, ButtonBorderStyle.Solid,
                      Color.Red, 2, ButtonBorderStyle.Solid,
                      Color.Red, 2, ButtonBorderStyle.Solid,
                      Color.Red, 2, ButtonBorderStyle.Solid);
            }
        }
    }
}

