using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Xaml.Behaviors;

namespace WHS.DEVICE.MAPDESIGN.Commons
{
    /// <summary>
    /// 验证行为类,可以获得附加到的对象
    /// </summary>
    public class ValidationExceptionBehavior : Behavior<FrameworkElement>
    {

        #region 字段
        /// <summary>
        /// 错误计数器
        /// </summary>
        private int _validationExceptionCount = 0;

        private Dictionary<UIElement, NotifyAdorner> _adornerCache;
        #endregion

        #region 方法

        #region 重写方法
        /// <summary>
        /// 附加对象时
        /// </summary>
        protected override void OnAttached()
        {
            _adornerCache = new Dictionary<UIElement, NotifyAdorner>();

            //附加对象时，给对象增加一个监听验证错误事件的能力，注意该事件是冒泡的
            this.AssociatedObject.AddHandler(System.Windows.Controls.Validation.ErrorEvent, new EventHandler<ValidationErrorEventArgs>(this.OnValidationError));
        }

        #endregion

        #region 私有方法

        #region 获取实现接口的对象

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        private IValidationExceptionHandler GetValidationExceptionHandler()
        {
            if (this.AssociatedObject.DataContext is IValidationExceptionHandler)
            {
                var handler = this.AssociatedObject.DataContext as IValidationExceptionHandler;

                return handler;
            }

            return null;
        }

        #endregion

        #region 显示Adorner

        /// <summary>
        /// 显示Adorner
        /// </summary>
        /// <param name="element"></param>
        /// <param name="errorMessage"></param>
        private void ShowAdorner(UIElement element, string errorMessage)
        {
            NotifyAdorner adorner = null;

            //先去缓存找
            if (_adornerCache.ContainsKey(element))
            {
                adorner = _adornerCache[element];

                //找到了，修改提示信息
                adorner.ChangeToolTip(errorMessage);
            }
            //没有找到，那就New一个，加入到缓存
            else
            {
                adorner = new NotifyAdorner(element, errorMessage);

                _adornerCache.Add(element, adorner);
            }

            //将Adorner加入到
            if (adorner != null)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(element);

                adornerLayer.Add(adorner);
            }
        }

        #endregion

        #region 移除Adorner

        /// <summary>
        /// 移除Adorner
        /// </summary>
        /// <param name="element"></param>
        private void HideAdorner(UIElement element)
        {
            //移除Adorner
            if (_adornerCache.ContainsKey(element))
            {
                var adorner = _adornerCache[element];

                var adornerLayer = AdornerLayer.GetAdornerLayer(element);

                adornerLayer.Remove(adorner);
            }
        }

        #endregion

        #region 验证事件方法

        /// <summary>
        /// 验证事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnValidationError(object sender, ValidationErrorEventArgs e)
        {
            try
            {
                var handler = GetValidationExceptionHandler();

                var element = e.OriginalSource as UIElement;

                if (handler == null || element == null)
                    return;

                if (e.Action == ValidationErrorEventAction.Added)
                {
                    _validationExceptionCount++;

                    ShowAdorner(element, e.Error.ErrorContent.ToString());
                }
                else if (e.Action == ValidationErrorEventAction.Removed)
                {
                    _validationExceptionCount--;

                    HideAdorner(element);
                }

                handler.IsValid = _validationExceptionCount == 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #endregion

        #endregion


    }
}
