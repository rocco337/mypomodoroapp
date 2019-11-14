using PomodoroApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PomodoroGui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PomodoroApp.PomodoroApp _pomodoro;
        public MainWindow()
        {
            InitializeComponent();
            DbInit();
            actionButtonStart.Visibility = Visibility.Visible;
            actionButtonStop.Visibility = Visibility.Hidden;
            this.Title = "Pomodoro App";

            _pomodoro = new PomodoroApp.PomodoroApp(25, 5, (interval) =>
            {               
                Dispatcher.BeginInvoke(new ThreadStart(() => {
                    interval.Comment = _commentWindow?.txtComment.Text;

                    if (this.StopClicked)
                    {
                        this.StopClicked = false;
                        _pomodoro.Stop(interval);
                    }

                    intervalType.Text = interval.Type.ToString();
                    countDown.Text = interval.CountDown.ToString("mm:ss");
                    this.Title = $"[{countDown.Text}] - Pomodoro App";
                    session.Text = $"{interval.SessionIndex}/{_pomodoro.NumberOfWorkIntervalsBeforeLongBreak}";

                    ShowStop();
                }));
            },
            (interval, nextIntervalType) =>{                
                if(nextIntervalType == IntervalType.Work)
                {
                    Dispatcher.BeginInvoke(new ThreadStart(() =>
                    {
                        if (_commentWindow != null)
                        {
                            _commentWindow.txtComment.Text = string.Empty;
                        }
                        countDown.Text = "00:00";
                        ShowStart();
                    }));
                }
            });

        }

        public static void DbInit()
        {
            using (var dbContext = new PomodoroDbContext())
            {
                //Ensure database is created
                dbContext.Database.EnsureCreated();                
            }
        }

        private void actionButtonStart_Click(object sender, RoutedEventArgs e)
        {
            _pomodoro.Start();
            ShowStop();
        }
        private bool StopClicked = false;
        private void actionButtonStop_Click(object sender, RoutedEventArgs e)
        {
            StopClicked = true;            
            countDown.Text = "00:00";
            ShowStart();
        }

        private void ShowStart()
        {
            actionButtonStart.Visibility = Visibility.Visible;
            actionButtonStop.Visibility = Visibility.Hidden;
        }

        private void ShowStop()
        {
            actionButtonStart.Visibility = Visibility.Hidden;
            actionButtonStop.Visibility = Visibility.Visible;
        }
       
        private CommentWindow _commentWindow = new CommentWindow();
        private void commentButton_Click(object sender, RoutedEventArgs e)
        {
            _commentWindow = new CommentWindow();
            _commentWindow.Show();
        }

    }
}
