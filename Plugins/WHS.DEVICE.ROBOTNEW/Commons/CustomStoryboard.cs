using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace WHS.DEVICE.ROBOTNEW.Commons
{
    public class CustomStoryboard : Storyboard, IDisposable
    {
        private object _data;

        public CustomStoryboard(object obj) : base()
        {
            _data = obj;
            base.Completed += CustomStoryboard_Completed;
        }

        public delegate void CustomEventHandler(object? sender, object? data, EventArgs e);

        public event CustomEventHandler CustomCompleted;

        private void OnCustomCompleted(object? sender, object? data, EventArgs e)
        {
            if (CustomCompleted != null)
                CustomCompleted(sender, data, e);
        }


        private void CustomStoryboard_Completed(object sender, EventArgs e)
        {
            OnCustomCompleted(sender, _data, e);
        }

        public void Dispose()
        {
            base.Completed -= CustomStoryboard_Completed;
            if (CustomCompleted != null)
                CustomCompleted = null;
            _data = null;

        }

    }
}
