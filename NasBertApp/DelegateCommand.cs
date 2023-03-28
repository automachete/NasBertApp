using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NasBertApp
{
    internal class DelegateCommand : ICommand
    {
        /// <summary>
        /// コマンド実行時の処理内容を保持します。
        /// </summary>
        private Action<object> _execute;

        /// <summary>
        /// コマンド実行可能判別の処理内容を保持します。
        /// </summary>
        private Func<object, bool> _canExecute;

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="execute">コマンド実行処理を指定します。</param>
        public DelegateCommand(Action<object> execute)
        : this(execute, null)
        {
        }

        /// <summary>
        /// 新しいインスタンスを生成します。
        /// </summary>
        /// <param name="execute">コマンド実行処理を指定します。</param>
        /// <param name="canExecute">コマンド実行可能判別処理を指定します。</param>
        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        #region ICommand の実装
        /// <summary>
        /// コマンドが実行可能かどうかの判別処理をおこないます。
        /// </summary>
        /// <param name="parameter">判別処理に対するパラメータを指定します。</param>
        /// <returns>実行可能な場合に true を返します。</returns>
        public bool CanExecute(object parameter)
        {
            return (this._canExecute != null) ? this._canExecute(parameter) : true;
        }

        /// <summary>
        /// 実行可能かどうかの判別処理に関する状態が変更されたときに発生します。
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// CanExecuteChanged イベントを発行します。
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var h = this.CanExecuteChanged;
            if (h != null) h(this, EventArgs.Empty);
        }

        /// <summary>
        /// コマンドが実行されたときの処理をおこないます。
        /// </summary>
        /// <param name="parameter">コマンドに対するパラメータを指定します。</param>
        public void Execute(object parameter)
        {
            if (this._execute != null)
                this._execute(parameter);
        }
        #endregion ICommand の実装
    }
}
