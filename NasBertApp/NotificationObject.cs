using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NasBertApp
{
    internal abstract class NotificationObject : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged の実装
        /// <summary>
        /// プロパティに変更があった場合に発生します。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// PropertyChanged イベントを発行します。
        /// </summary>
        /// <param name="propertyName">プロパティ値に変更があったプロパティ名を指定します。</param>
        protected void RaiseProeprtyChanged([CallerMemberName] string propertyName = null)
        {
            var h = this.PropertyChanged;
            if (h != null) h(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// プロパティ値を変更するヘルパです。
        /// </summary>
        /// <typeparam name="T">プロパティの型を表します。</typeparam>
        /// <param name="target">変更するプロパティの実体を ref 指定します。</param>
        /// <param name="value">変更後の値を指定します。</param>
        /// <param name="propertyName">プロパティ名を指定します。</param>
        /// <returns>プロパティ値に変更があった場合に true を返します。</returns>
        protected bool SetProperty<T>(ref T target, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(target, value))
                return false;
            target = value;
            RaiseProeprtyChanged(propertyName);
            return true;
        }
        #endregion INotifyPropertyChanged の実装
    }
}
