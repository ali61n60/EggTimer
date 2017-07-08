using Android.App;
using Android.Media;
using Android.Widget;
using Android.OS;
using Android.Util;
using Android.Views;
using Java.Lang;

namespace EggTimer
{
    [Activity(Label = "EggTimer", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private SeekBar _seekBarTimer;
        private TextView _textViewTimer;
        private Button _buttonController;
        private EggCountDownTimer _eggCountDownTimer;
        private bool _counterIsActive = false;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);
            initFields();
        }

        private void initFields()
        {
            initSeekBar();

            _textViewTimer = FindViewById<TextView>(Resource.Id.textViewTimer);

            _buttonController = FindViewById<Button>(Resource.Id.buttonController);
            _buttonController.Click += (sender, args) =>
            {
                if (!_counterIsActive)
                   activateTimer();
                else
                   DeActivateTimer();
            };
        }
        private void activateTimer()
        {
            _counterIsActive = true;
            _seekBarTimer.Enabled = false;
            _buttonController.Text = "Stop";

            initEggCountDownTimer();
            _eggCountDownTimer.Start();
        }
        public void DeActivateTimer()
        {
            _counterIsActive = false;
            _eggCountDownTimer.Cancel();
            _seekBarTimer.Enabled = true;
            UpdateTimer(_seekBarTimer.Progress);
            _buttonController.Text = "Go";
        }
        private void initEggCountDownTimer()
        {
            _eggCountDownTimer?.Cancel();
            _eggCountDownTimer = new EggCountDownTimer(_seekBarTimer.Progress * 1000+100, 1000, this);
        }

        private void initSeekBar()
        {
            _seekBarTimer = FindViewById<SeekBar>(Resource.Id.seekBarTimer);
            _seekBarTimer.Max = 600;
            _seekBarTimer.Progress = 30;
            _seekBarTimer.ProgressChanged += (sender, args) =>
            {
                UpdateTimer(_seekBarTimer.Progress);
            };
        }

        public void EggTimerFinished()
        {
            _seekBarTimer.Visibility=ViewStates.Visible;
            _counterIsActive = false;
            _buttonController.Text = "Go";
        }

        public void UpdateTimer(int secondsLeft)
        {
            int minutes = secondsLeft / 60;
            int seconds = secondsLeft - minutes * 60;
            string secodndsInString = seconds.ToString();
            if (secodndsInString.Length == 1)
                secodndsInString = "0" + secodndsInString;

            _textViewTimer.Text = minutes + ":" + secodndsInString;
        }
    }

    public class EggCountDownTimer : CountDownTimer
    {
        private MainActivity _mainActivity;
        public EggCountDownTimer(long millisInFuture, long countDownInterval) : base(millisInFuture, countDownInterval)
        {
        }
        public EggCountDownTimer(long millisInFuture, long countDownInterval, MainActivity mainActivity) : base(millisInFuture, countDownInterval)
        {
            _mainActivity = mainActivity;
        }

        public override void OnFinish()
        {
            _mainActivity.UpdateTimer(0);
            _mainActivity.DeActivateTimer();
            MediaPlayer mediaPlayer=MediaPlayer.Create(_mainActivity,Resource.Raw.airhorn);
            mediaPlayer.Start();
        }

        public override void OnTick(long millisUntilFinished)
        {
            if(_mainActivity==null)
                throw new Exception("_mainActivity instance is null");
            
            _mainActivity.UpdateTimer((int)(millisUntilFinished / 1000));
        }
    }
}

