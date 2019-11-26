using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Key_Generator_by_mask.Annotations;
using System.Windows.Input;
using Key_Generator_by_mask.Model;
using Microsoft.Win32;

namespace Key_Generator_by_mask.ViewModel
{
    public class MainVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ObservableCollection<CharMaskModel> CharMaskList { get; set; } = new ObservableCollection<CharMaskModel>();
        public ObservableCollection<MaskModel> MaskList { get; set; } = new ObservableCollection<MaskModel>();
        public ObservableCollection<string> KeyList { get; set; } = new ObservableCollection<string>();
        public SynchronizationContext sync;
        public Random rand;
        public SaveFileDialog saveDialog = new SaveFileDialog()
        {
            Filter = "Текстовый документ | *.txt",
            DefaultExt = "txt"
        };

        public MainVM()
        {
            sync = SynchronizationContext.Current;
            rand = new Random();
            CharMaskList.CollectionChanged += (s, e) =>
            {
                if(e.Action == NotifyCollectionChangedAction.Add)
                    foreach (CharMaskModel item in e.NewItems)
                        item.VM = this;

                //OnPropertyChanged(nameof(CharMaskList));
            };

            MaskList.CollectionChanged += (s, e) =>
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                    foreach (MaskModel item in e.NewItems)
                        item.VM = this;

                //OnPropertyChanged(nameof(MaskList));
            };

            //KeyList.CollectionChanged += (s, e) => OnPropertyChanged(nameof(KeyList));
            

            MaskList.Add(new MaskModel{ Mask = "%%-$$$$-$$$$-@@"});

            CharMaskList.Add(new CharMaskModel
            {
                Char = '$',
                Chars = "1234567890"
            });
            CharMaskList.Add(new CharMaskModel
            {
                Char = '@',
                Chars = "qwertyuiopasdfghjklzxcvbnm"
            });
            CharMaskList.Add(new CharMaskModel
            {
                Char = '%',
                Chars = "QWERTYUIOPASDFGHJKLZXCVBNM"
            });
        }
        
        private ICommand _DellCharMaskCommand;
        public ICommand DellCharMaskCommand => _DellCharMaskCommand ?? (_DellCharMaskCommand = new RelayCommand(p =>
        {
            CharMaskList.Remove(p as CharMaskModel);
        }, o => !IsGenKeys));

        private ICommand _AddCharMaskCommand;
        public ICommand AddCharMaskCommand => _AddCharMaskCommand ?? (_AddCharMaskCommand = new RelayCommand(p =>
        {
            if (p is CharMaskModel model)
                CharMaskList.Add(model);
        }, o => !IsGenKeys));

        private ICommand _DellMaskCommand;
        public ICommand DellMaskCommand => _DellMaskCommand ?? (_DellMaskCommand = new RelayCommand(p =>
        {
            MaskList.Remove(p as MaskModel);
        }, o => !IsGenKeys));

        private ICommand _AddMaskCommand;
        public ICommand AddMaskCommand => _AddMaskCommand ?? (_AddMaskCommand = new RelayCommand(p =>
        {
            var mask = (string)p;
            if (string.IsNullOrWhiteSpace(mask))
                return;
            MaskList.Add(new MaskModel { Mask = mask });
        }, o => !IsGenKeys));

        private bool _IsGenKeys;
        public bool IsGenKeys
        {
            get => _IsGenKeys;
            set
            {
                _IsGenKeys = value;
                OnPropertyChanged();
            }
        }

        private ICommand _GenKeysCommand;
        public ICommand GenKeysCommand => _GenKeysCommand ?? (_GenKeysCommand = new RelayCommand(async p =>
        {
            IsGenKeys = true;
            KeyList.Clear();
            await Task.Factory.StartNew(() =>
            {
                Dictionary<char, string> CharValue = new Dictionary<char, string>();

                foreach (var v in CharMaskList)
                {
                    if (!CharValue.ContainsKey(v.Char))
                        CharValue[v.Char] = "";
                    CharValue[v.Char] += v.Chars;
                }

                for (int i = 0; i < KeyCount && IsGenKeys; i++)
                {
                    string mask = MaskList[i % MaskList.Count].Mask;

                    string key = string.Empty;

                    foreach (char c in mask)
                    {
                        if (CharValue.ContainsKey(c))
                        {
                            string values = CharValue[c];
                            key += values[rand.Next(0, values.Length)];
                        }
                        else key += c;
                    }

                    sync.Send(q => KeyList.Add(key), null);
                }

                sync.Send(q => IsGenKeys = false, null);
            });
        }, o => !IsGenKeys && MaskList.Count > 0 && CharMaskList.Count > 0));

        private ICommand _StopGenKeysCommand;
        public ICommand StopGenKeysCommand => _StopGenKeysCommand ?? (_StopGenKeysCommand = new RelayCommand(p =>
        {
            IsGenKeys = false;
        }, o => IsGenKeys));

        private ICommand _SaveKeysCommand;
        public ICommand SaveKeysCommand => _SaveKeysCommand ?? (_SaveKeysCommand = new RelayCommand(p =>
        {
            if (saveDialog.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(saveDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, KeyList));
                    fs.Write(bytes, 0, bytes.Length);
                }
                MessageBox.Show("Сохранено!");
            }
        }, o => !IsGenKeys));

        private int _KeyCount = 100;
        public int KeyCount
        {
            get => _KeyCount;
            set
            {
                _KeyCount = value;
                OnPropertyChanged();
            }
        }
    }
}
