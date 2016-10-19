namespace InfinityCycleRuner
{

    public  class CycleRunParameters
    {
        public string ExecutableString { get ; }
        public int RunCooldown { get; }
        public MainWindow UserInterfaceForm { get; }
        public bool LetShowLog { get; }

        public CycleRunParameters(
            string executableString
            , int runCooldown
            , MainWindow userInterfaceForm
            , bool letShowLog)
        {
            this.ExecutableString = executableString;
            this.RunCooldown = runCooldown ;
            this.UserInterfaceForm = userInterfaceForm;
            this.LetShowLog = letShowLog;

        }

    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private bool _breakeCommandExecution = true;
        private bool _breakeWeblinkExecution = true;
        private bool _letShowLog = true;
        private int _cooldown = 0;


        public delegate void MethodContainer();

        public event MethodContainer OnCommandFinish ;
        private delegate void CommandFinishDelegate();

        public MainWindow()
        {
            this.InitializeComponent();
            this.OnCommandFinish += this.CommandFinishWatchDog ;
            this.OnWeblinkFinish += this.WeblinkFinishWatchDog ;
        }

        private void CommandFinishWatchDog ( )
        {
            var enableCommand = new CommandFinishDelegate(this.CommandFinish);
            var dispatcher = this.Dispatcher ;
            dispatcher?.BeginInvoke
                (
                    enableCommand ) ;
        }

        private void CommandFinish ( )
        {
            
            var doRunCommandButton = this.DoRunCommandButton;
            var doStopCommandButton = this.DoStopCommandButton;
            

            if ( (doRunCommandButton != null)
                && (doStopCommandButton != null))
            {

                doRunCommandButton.IsEnabled = true;
                doStopCommandButton.IsEnabled = false;
            }

        }

        public event MethodContainer OnWeblinkFinish;
        private delegate void WeblinkFinishDelegate();

        private void WeblinkFinish()
        {
            var doDownloadWeblinkButton = this.DoDownloadWeblinkButton;
            var doGetWeblinkButton = this.DoGetWeblinkButton;
            var doPostWeblinkButton = this.DoPostWeblinkButton;
            var doStopWebLinkButton = this.DoStopWebLinkButton;

            if ((doDownloadWeblinkButton != null)
                && (doGetWeblinkButton != null)
                && (doPostWeblinkButton != null)
                && (doStopWebLinkButton != null))
            {
                doDownloadWeblinkButton.IsEnabled = true;
                doGetWeblinkButton.IsEnabled = true;
                doPostWeblinkButton.IsEnabled = true;
                doStopWebLinkButton.IsEnabled = false;
            }
        }

        private void WeblinkFinishWatchDog()
        {
            var enableCommand = new WeblinkFinishDelegate(this.WeblinkFinish);
            var dispatcher = this.Dispatcher;
            dispatcher?.BeginInvoke
                (
                    enableCommand);
        }

        private void DoRunCommandButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var commandLineText = string.Empty ;
            var cooldown = 0;
            var letShowLog = false;

            var commandTextBox = this.CommandTextBox ;
            var doRunCommandButton = this.DoRunCommandButton;
            var doStopCommandButton = this.DoStopCommandButton;
            var colldownIntegerUpDown = this.CooldownIntegerUpDown;
            var showLogCheckBox = this.ShowLogCheckBox;

            if ( ( commandTextBox != null ) 
                && ( doRunCommandButton != null )
                && (doStopCommandButton != null)
                && (showLogCheckBox != null))
            {

                doRunCommandButton.IsEnabled = false;
                doStopCommandButton.IsEnabled = true ;
                commandLineText = commandTextBox.Text;
                if (colldownIntegerUpDown?.Value != null)
                {
                    cooldown = (int)colldownIntegerUpDown.Value;
                }
                if (showLogCheckBox.IsChecked != null)
                {
                    letShowLog = showLogCheckBox.IsChecked.Value;
                }
            }

            CycleRunParameters commandLoopParameters = null ;

            var parentForm = this ;

            if ( !string.IsNullOrWhiteSpace (commandLineText) )
            {

                commandLoopParameters = new CycleRunParameters
                    (
                    commandLineText ,
                    cooldown ,
                    parentForm,
                    letShowLog) ;
            }

            if(commandLoopParameters != null )
            {

                this._breakeCommandExecution = false;

                var runCommandLoopThread = new System.Threading.Thread(this.RunCommandLoop);
                runCommandLoopThread.Start(commandLoopParameters);
            }
        }



        private void RunCommandLoop
            (
            object runCommandLoopParameters )
        {

            var commandLoopParameters = (CycleRunParameters) runCommandLoopParameters ;

            if ( commandLoopParameters != null )
            {
                var commandLineText = commandLoopParameters.ExecutableString ;
                var cooldown = commandLoopParameters.RunCooldown ;

                this.RunCommand
                    (
                        commandLineText) ;

                // ReSharper disable LoopVariableIsNeverChangedInsideLoop
                while ( ( cooldown > 0 )
                    && !this._breakeCommandExecution )
                    // ReSharper restore LoopVariableIsNeverChangedInsideLoop
                {

                    System.Threading.Thread.Sleep
                        (
                            cooldown * 1000 ) ;
                    this.RunCommand
                        (
                            commandLineText ) ;
                }
            }

            var onCommandFinish = commandLoopParameters?.UserInterfaceForm?.OnCommandFinish;
            onCommandFinish?.Invoke();
        }

        private void RunCommand
            (
            string commandLineText )
        {
            if (commandLineText != null)
            {
                try
                {
                    using (var comandlineProcess = System.Diagnostics.Process.Start
                                (
                                    commandLineText))
                    {
                        var isProcessExited = true;
                        if (comandlineProcess != null)
                        {
                            comandlineProcess.EnableRaisingEvents = true;
                            try
                            {
                                isProcessExited = comandlineProcess.HasExited;
                            }
                            catch (System.Exception)
                            {

                                isProcessExited = true;
                            }

                        }

                        while (!isProcessExited)
                        {
                            System.Threading.Thread.Sleep
                                (
                                    100);
                            isProcessExited = comandlineProcess.HasExited;

                            //if (this.BreakeCommandExecution)
                            //{
                            //    comandlineProcess.Close (  );
                            //    comandlineProcess.Kill (  );
                            //    break;
                            //}
                        }
                    }

                }
                catch ( System.Exception )
                {

                    // epic fail ;
                }
            }

        }

        private void DoStopCommandButton_Copy_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this._breakeCommandExecution = true ;
        }

        private void DoDownloadWeblinkButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var weblink = string.Empty ;
            var cooldown = 0 ;
            var letShowLog = false;

            var doDownloadWeblinkButton = this.DoDownloadWeblinkButton ;
            var doGetWeblinkButton = this.DoGetWeblinkButton;
            var doPostWeblinkButton = this.DoPostWeblinkButton;
            var doStopWebLinkButton = this.DoStopWebLinkButton;
            var weblinkTextBox = this.WeblinkTextBox ;
            var colldownIntegerUpDown = this.CooldownIntegerUpDown;
            var showLogCheckBox = this.ShowLogCheckBox ;
            if (( weblinkTextBox!= null ) 
                && ( doDownloadWeblinkButton != null )
                && ( doGetWeblinkButton != null )
                && ( doPostWeblinkButton != null ) 
                && ( doStopWebLinkButton != null ) 
                && (showLogCheckBox != null ) )
            {
                doDownloadWeblinkButton.IsEnabled = false ;
                doGetWeblinkButton.IsEnabled = false;
                doPostWeblinkButton.IsEnabled = false;
                doStopWebLinkButton.IsEnabled = true ;
                weblink = weblinkTextBox.Text ;
                if (colldownIntegerUpDown?.Value != null)
                {
                    cooldown = (int)colldownIntegerUpDown.Value;
                }
                if ( showLogCheckBox.IsChecked != null )
                {
                    letShowLog = showLogCheckBox.IsChecked.Value;
                }
            }

            CycleRunParameters cycleRunParameters = null ;
            if ( !string.IsNullOrWhiteSpace ( weblink ))
            {
                cycleRunParameters = new CycleRunParameters 
                    (
                    weblink,
                    cooldown,
                    this,
                    letShowLog
                    );
            }

            if (cycleRunParameters != null)
            {

                this._breakeWeblinkExecution = false;

                var runWeblinkLoopThread = new System.Threading.Thread(this.RunDownloadWebLink);
                runWeblinkLoopThread.Start(cycleRunParameters);
            }

        }

        private void RunDownloadWebLink
            (
            object webLinkParameters )
        {

            var weblink = string.Empty ;
            var cooldown = 0 ;

            var linkParameters = ( CycleRunParameters ) webLinkParameters ;

            if ( linkParameters != null )
            {
                weblink = linkParameters.ExecutableString ;
                cooldown = linkParameters.RunCooldown ;
            }

            MainWindow.DownloadWeblink
                (
                    weblink ) ;

            while ( ( cooldown > 0 )
                    && !this._breakeWeblinkExecution )
            {
                System.Threading.Thread.Sleep
                    (
                        cooldown * 1000 ) ;
                MainWindow.DownloadWeblink
                    (
                        weblink ) ;
            }

            var onWeblinkFinish = linkParameters?.UserInterfaceForm?.OnWeblinkFinish;
            onWeblinkFinish?.Invoke();
        }

        // ReSharper disable UnusedMethodReturnValue.Local
        private static string DownloadWeblink
            // ReSharper restore UnusedMethodReturnValue.Local
            (
            string weblink)
        { 
            var client = new System.Net.WebClient ( ) ;

            var downloadString = string.Empty ;

            if ( !string.IsNullOrWhiteSpace
                      (
                          weblink ) )
            {
                try
                {
                    downloadString = client.DownloadString
                                        (
                                            weblink);
                }
                catch ( System.Exception e )
                {

                    downloadString = e.Message ;
                }
                
            }

            return downloadString ;
        }


        private void DoGetWeblinkButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void DoStopWebLinkButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this._breakeWeblinkExecution = true;
        }

        private void ShowLogCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            this._letShowLog = true;
        }

        private void ShowLogCheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            this._letShowLog = false;
        }

        private void CooldownIntegerUpDown_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            this._cooldown = 0;
            var cooldownIntegerUpDown = this.CooldownIntegerUpDown ;
            if ( cooldownIntegerUpDown?.Value != null )
            {
                this._cooldown = cooldownIntegerUpDown.Value.Value;
            }
        }
    }
}
