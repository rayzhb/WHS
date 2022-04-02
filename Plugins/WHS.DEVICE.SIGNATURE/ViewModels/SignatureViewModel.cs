using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WHS.Infrastructure;
using WHS.Infrastructure.Events;

namespace WHS.DEVICE.SIGNATURE.ViewModels
{
    public class SignatureViewModel : Screen
    {
        private static NLog.ILogger logger = NLog.LogManager.GetCurrentClassLogger();
        private Stack<Stroke> _undoList;
        private StrokeCollection _strokes;
        /// <summary>
        /// 白板内容
        /// </summary>
        public StrokeCollection Strokes
        {
            get
            {
                if (_strokes == null)
                {
                    _strokes = new StrokeCollection();
                }
                return _strokes;
            }
        }

        public SignatureViewModel()
        {
            _undoList = new Stack<Stroke>();
        }

        protected override Task OnActivateAsync(CancellationToken cancellationToken)
        {

            return Task.FromResult(1);
        }

        public void Clear()
        {
            _undoList.Clear();
            Strokes.Clear();
        }
        public void Redo()
        {
            if (_undoList != null && _undoList.Count > 0)
            {
                Strokes.Add(_undoList.Pop());
            }
        }
        public void Undo()
        {
            if (Strokes.Any())
            {
                _undoList.Push(Strokes[Strokes.Count - 1]);
                Strokes.RemoveAt(Strokes.Count - 1);
            }
        }
        public void Confirm(System.Windows.Controls.InkCanvas inkCanvas)
        {
            if (inkCanvas.Strokes == null || inkCanvas.Strokes.Count == 0)
            {
                //GlobalContext.EventAggregator.PublishOnUIThreadAsync
                //      (new EventMessageDialog()
                //      {
                //          Message = "请画一些信息",
                //          Title = "警告",
                //          MessageDialogStyle = MahApps.Metro.Controls.Dialogs.MessageDialogStyle.Affirmative
                //      });
                return;
            }
            var renderBitmap = new RenderTargetBitmap((int)inkCanvas.ActualWidth + 100, (int)inkCanvas.ActualHeight, 72d, 72d, PixelFormats.Pbgra32);
            renderBitmap.Render(inkCanvas);

            var dir = Directory.GetParent(SignaturePluginDefinition.s_filename) + "\\Signature";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            string path = dir + "\\" + Guid.NewGuid().ToString() + ".png";
            using (FileStream fs = File.Create(path))
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                encoder.Save(fs);
            }
            var result=GlobalContext.WindowManager.ShowDialogAsync(new DialogViewModel("提示", "文件保存成功，地址:" + path),
                null, null).Result;
            _undoList.Clear();
            Strokes.Clear();
            //GlobalContext.EventAggregator.PublishOnUIThreadAsync
            //          (new EventMessage()
            //          {
            //              EventDefinition = EventDefinition.弹出框,
            //              EventData = new EventMessageDialog()
            //              {
            //                  Message = "文件保存成功，地址:" + path,
            //                  Title = "提示",
            //                  MessageDialogStyle = MahApps.Metro.Controls.Dialogs.MessageDialogStyle.Affirmative
            //              }
            //          });
        }

    }
}
