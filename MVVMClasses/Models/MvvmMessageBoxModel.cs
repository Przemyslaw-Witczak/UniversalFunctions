using System;
using System.Windows;
using System.Windows.Input;

namespace MVVMClasses.Models
{
    /// <summary>
    /// Enum opisujący typ okna
    /// </summary>
    public enum MvvmMessageBoxTypeEnum
    {
        /// <summary>
        /// Formularz wyświetla przyciski Tak oraz Nie
        /// </summary>
        Question,
        /// <summary>
        /// Formularz wyświetla przycisk OK
        /// </summary>
        Confirmation
    };
    /// <summary>
    /// Model danych dla okna komunikatu TAK/NIE/OK
    /// </summary>
    public class MvvmMessageBoxModel : ModelBase
    {
        /// <summary>
        /// Konstruktor modelu
        /// </summary>
        public MvvmMessageBoxModel()
        {
            _messageBoxType = MvvmMessageBoxTypeEnum.Confirmation;  
            KomunikatLinia1 = String.Empty;
            KomunikatLinia2 = String.Empty;
            _windowTitle = String.Empty;
        }

        /// <summary>
        /// Właściwośc określająca widoczność przycisku TAK/NIE
        /// </summary>
        public Visibility BtnYesNoVisible { get { return MessageBoxType == MvvmMessageBoxTypeEnum.Question ? Visibility.Visible : Visibility.Hidden; } }

        /// <summary>
        /// Właściwość określająca widoczność przycisku OK
        /// </summary>
        public Visibility BtnOkVisible { get { return MessageBoxType == MvvmMessageBoxTypeEnum.Confirmation ? Visibility.Visible : Visibility.Hidden; } }

        /// <summary>
        /// Zmienna prywatna właściwości określającej rodzaj okna komunikatu
        /// </summary>
        private MvvmMessageBoxTypeEnum _messageBoxType;

        /// <summary>
        /// Właściwość określająca rodzaj okna komunikatu
        /// </summary>
        public MvvmMessageBoxTypeEnum MessageBoxType
        {
            get { return _messageBoxType; }
            set
            {
                if (_messageBoxType != value)
                {
                    _messageBoxType = value;
                    NotifyPropertyChanged("MessageBoxType");
                    NotifyPropertyChanged("BtnYesNoVisible");
                    NotifyPropertyChanged("BtnOkVisible");
                }
            }
        }

        private string _komunikatLinia1;
        public string KomunikatLinia1
        {
            get { return _komunikatLinia1; }
            set
            {
                _komunikatLinia1 = value;
                NotifyPropertyChanged("KomunikatLinia1");
            }
        }

        private string _komunikatLinia2;
        public string KomunikatLinia2
        {
            get { return _komunikatLinia2; }
            set
            {
                _komunikatLinia2 = value;
                NotifyPropertyChanged("KomunikatLinia2");
            }
        }

        private ICommand _returnTrue = null;
        /// <summary>
        /// Komenda kliknięcia na przyciski TAK lub OK
        /// </summary>
        public ICommand ReturnTrue
        {
            get
            {
                return _returnTrue = _returnTrue ?? new MvvmCommand(
                    (param) =>
                    {
                        CloseViewWithOKResult();
                    });
            }
        }


        private ICommand _returnFalse = null;
        private string _windowTitle;

        public string WindowTitle
        {
            get { return MessageBoxTypeConverter(_windowTitle); }
            set
            {
                if (_windowTitle != value)
                {
                    _windowTitle = value;
                    NotifyPropertyChanged("WindowTitle");
                }

            }
        }

        private string MessageBoxTypeConverter(string windowTitle)
        {
            if (_messageBoxType == MvvmMessageBoxTypeEnum.Question)
            {
                return $"Pytanie {windowTitle}";
            }
            else
            {
                return $"Potwierdzenie {windowTitle}";
            }
        }

        /// <summary>
        /// Komenda kliknięcia na przyciski TAK lub OK
        /// </summary>
        public ICommand ReturnFalse
        {
            get
            {
                return _returnFalse = _returnFalse ?? new MvvmCommand(
                    (param) =>
                    {
                        CloseViewWithFalseResult();
                    });
            }
        }

        public event Action CloseViewWithOKResult;
        public event Action CloseViewWithFalseResult;        

    }
}
