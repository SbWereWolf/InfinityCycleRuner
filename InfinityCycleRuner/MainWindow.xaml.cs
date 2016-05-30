using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Xceed.Wpf.Toolkit;

namespace InfinityCycleRuner
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        bool BreakeCommandExecution = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DoRunCommandButton_Click(object sender, RoutedEventArgs e)
        {
            var commandLineText = string.Empty ;
            var commandTextBox = this.CommandTextBox ;
            if ( commandTextBox != null )
            {
                commandTextBox.IsEnabled = false ;
                commandLineText = commandTextBox.Text;
            }

            var cooldown = 0 ;
            var colldownIntegerUpDown = this.ColldownIntegerUpDown ;
            if ( colldownIntegerUpDown?.Value != null )
            {
                cooldown = ( int ) colldownIntegerUpDown.Value ;
            }
            if ( !string.IsNullOrWhiteSpace (commandLineText))
            {
                this.BreakeCommandExecution = false ;
                this.RunCommandLoop(commandLineText, cooldown);
            }
            
        }

        private void RunCommandLoop
            (
            string commandLineText ,
            int cooldown )
        {
            this.RunCommand (
                commandLineText ) ;

            // ReSharper disable LoopVariableIsNeverChangedInsideLoop
            while (cooldown > 0)
                // ReSharper restore LoopVariableIsNeverChangedInsideLoop
            {

                System.Threading.Thread.Sleep
                    (
                        cooldown * 1000 ) ;
                this.RunCommand(
                    commandLineText);
            }
        }

        private void RunCommand
            (
            string commandLineText )
        {
            var comandlineProcess = new System.Diagnostics.Process ( ) ;
            if ( ( commandLineText != null )
                && !this.BreakeCommandExecution )
            {
                try
                {
                    comandlineProcess = System.Diagnostics.Process.Start
                    (
                        commandLineText);
                }
                catch ( Exception )
                {

                    comandlineProcess = null;
                }
                
            }

            var isProcessExited = true ;
            if ( comandlineProcess != null )
            {
                comandlineProcess.EnableRaisingEvents = true ;
                try
                {
                    isProcessExited = comandlineProcess.HasExited;
                }
                catch ( Exception )
                {

                    isProcessExited = true;
                }
                
            }

            while ( !isProcessExited )
            {
                System.Threading.Thread.Sleep
                    (
                        100 ) ;
                isProcessExited = comandlineProcess.HasExited ;
            }
        }

        private void DoStopCommandButton_Copy_Click(object sender, RoutedEventArgs e)
        {
            this.BreakeCommandExecution = true ;
        }
    }
}
