using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Key_Generator_by_mask.Annotations;
using Key_Generator_by_mask.ViewModel;

namespace Key_Generator_by_mask.Model
{
    public class CharMaskModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public MainVM VM { get; set; }

        public CharMaskModel()
        {
            THIS = this;
        }
        private char _Char;
        public char Char
        {
            get => _Char;
            set
            {
                _Char = value;
                OnPropertyChanged();
            }
        }

        private string _Chars;
        public string Chars
        {
            get => _Chars;
            set
            {
                _Chars = value;
                OnPropertyChanged();
            }
        }

        public CharMaskModel THIS { get; set; }
    }
}
