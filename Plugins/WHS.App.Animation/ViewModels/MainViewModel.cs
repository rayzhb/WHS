using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using LottieSharp;
using Microsoft.Win32;
using WHS.App.Animation.Model;

namespace WHS.App.Animation.ViewModels
{
    public class MainViewModel : Screen
    {
        public LottieAnimationView TestLottie
        {
            get; set;
        }

        private int _animationIndex;

        public int AdnimationIndex
        {
            get
            {
                return _animationIndex;
            }
            set
            {
                Set(ref _animationIndex, value);
            }
        }


        public IObservableCollection<ComboBoxModel> AnimationSource
        {
            get;
        }

        public MainViewModel()
        {

            var dir = Directory.GetParent(AppAnimationPluginDefinition.s_filename)+"\\Assets";
            var files = Directory.GetFiles(dir);
            AnimationSource =new BindableCollection<ComboBoxModel>();
            foreach (var file in files)
            {
                AnimationSource.Add(new ComboBoxModel()
                {
                    Text=System.IO.Path.GetFileNameWithoutExtension(file),
                    Value=file
                });
            }
            AdnimationIndex=0;

        }

        protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            TestLottie?.Dispose();
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        public void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!double.IsNaN(e.NewValue))
                TestLottie.Scale = (float)e.NewValue;
        }

        public void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TestLottie.PauseAnimation();
            TestLottie.Progress = (float)(e.NewValue / 1000);
        }

        public void PauseAnimation_Click(object sender, RoutedEventArgs e)
        {
            TestLottie.PauseAnimation();
        }
        public void StartAnimation_Click(object sender, RoutedEventArgs e)
        {
            TestLottie.PlayAnimation();
        }

        public void LoadAnimation_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.DefaultExt = ".json";
            openFileDialog.Filter = "Json files|*.json|All files|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                TestLottie.PauseAnimation();
                TestLottie.FileName = openFileDialog.FileName;
                TestLottie.PlayAnimation();
            }
        }
    }
}
