namespace InfinityCycleRuner
{

    public  class DownloadWebLinkParameters
    {
        public string Weblink { get ; }
        public int Cooldown { get; }
        public MainWindow ParentForm { get; }

        public DownloadWebLinkParameters(
            string weblink
            , int cooldown
            , MainWindow parentForm)
        {
            this.Weblink = weblink;
            this.Cooldown = cooldown ;
            this.ParentForm = parentForm;

        }

    }
    public class CommandLoopParameters
    {
        public string CommandLineText { get ; }
        public int Cooldown { get ; }
        public MainWindow ParentForm { get ; }

        public CommandLoopParameters(
            string commandLineText 
            , int cooldown
            , MainWindow parentForm)
        {
            this.CommandLineText = commandLineText ;
            this.Cooldown = cooldown ;
            this.ParentForm = parentForm ;

        }

    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private bool _breakeCommandExecution = true;
        private bool _breakeWeblinkExecution = true;
        

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

            var commandTextBox = this.CommandTextBox ;
            var doRunCommandButton = this.DoRunCommandButton;
            var doStopCommandButton = this.DoStopCommandButton;
            var colldownIntegerUpDown = this.CooldownIntegerUpDown;

            if ( ( commandTextBox != null ) 
                && ( doRunCommandButton != null )
                && (doStopCommandButton != null))
            {

                doRunCommandButton.IsEnabled = false;
                doStopCommandButton.IsEnabled = true ;
                commandLineText = commandTextBox.Text;
                if (colldownIntegerUpDown?.Value != null)
                {
                    cooldown = (int)colldownIntegerUpDown.Value;
                }
            }
            
            CommandLoopParameters commandLoopParameters = null ;

            var parentForm = this ;

            if ( !string.IsNullOrWhiteSpace (commandLineText) )
            {

                commandLoopParameters = new CommandLoopParameters
                    (
                    commandLineText ,
                    cooldown ,
                    parentForm ) ;
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

            var commandLoopParameters = ( CommandLoopParameters ) runCommandLoopParameters ;

            if ( commandLoopParameters != null )
            {
                var commandLineText = commandLoopParameters.CommandLineText ;
                var cooldown = commandLoopParameters.Cooldown ;

                this.RunCommand
                    (
                        commandLoopParameters.CommandLineText ) ;

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

            var onCommandFinish = commandLoopParameters?.ParentForm?.OnCommandFinish;
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

            var doDownloadWeblinkButton = this.DoDownloadWeblinkButton ;
            var doGetWeblinkButton = this.DoGetWeblinkButton;
            var doPostWeblinkButton = this.DoPostWeblinkButton;
            var doStopWebLinkButton = this.DoStopWebLinkButton;
            var weblinkTextBox = this.WeblinkTextBox ;
            var colldownIntegerUpDown = this.CooldownIntegerUpDown;
            if (( weblinkTextBox!= null ) 
                && ( doDownloadWeblinkButton != null )
                && ( doGetWeblinkButton != null )
                && ( doPostWeblinkButton != null ) 
                && ( doStopWebLinkButton != null ) )
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
            }

            DownloadWebLinkParameters downloadWebLinkParameters = null ;
            if ( !string.IsNullOrWhiteSpace ( weblink ))
            {
                downloadWebLinkParameters = new DownloadWebLinkParameters 
                    (
                    weblink,
                    cooldown,
                    this
                    );
            }

            if (downloadWebLinkParameters != null)
            {

                this._breakeWeblinkExecution = false;

                var runWeblinkLoopThread = new System.Threading.Thread(this.RunDownloadWebLink);
                runWeblinkLoopThread.Start(downloadWebLinkParameters);
            }

        }

        private void RunDownloadWebLink
            (
            object webLinkParameters )
        {

            var weblink = string.Empty ;
            var cooldown = 0 ;

            var linkParameters = ( DownloadWebLinkParameters ) webLinkParameters ;

            if ( linkParameters != null )
            {
                weblink = linkParameters.Weblink ;
                cooldown = linkParameters.Cooldown ;
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

            var onWeblinkFinish = linkParameters?.ParentForm?.OnWeblinkFinish;
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
    }
}
