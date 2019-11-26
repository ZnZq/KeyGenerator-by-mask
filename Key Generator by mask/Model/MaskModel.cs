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
    public class MaskModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public MaskModel()
        {
            THIS = this;
        }

        public MainVM VM { get; set; }
        public MaskModel THIS { get; set; }

        private string _Mask;
        public string Mask
        {
            get => _Mask;
            set
            {
                _Mask = value;
                OnPropertyChanged();
            }
        }
    }
}
