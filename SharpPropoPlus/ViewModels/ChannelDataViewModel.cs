using SharpPropoPlus.Enums;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.ViewModels
{
    public class ChannelDataViewModel : BaseViewModel, IChannelData
    {
        private int _value;
        private string _toolTip;
        
        public int Value
        {
            get => _value;
            private set
            {
                if (value == _value)
                {
                    return;
                }

                _value = value;
                OnPropertyChanged();
            }
        }

        public string ToolTip
        {
            get => _toolTip;
            private set
            {
                _toolTip = value;
                OnPropertyChanged();
            }
        }

        public void SetValue(int value)
        {
            Value = value;
        }

        public ChannelDataViewModel(string toolTip, int value)
        {
            ToolTip = toolTip;
            Value = value;
        }
    }
}