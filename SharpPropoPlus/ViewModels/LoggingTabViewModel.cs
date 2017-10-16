using System;
using System.IO;
using System.Reflection;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.SaveFile;
using SharpPropoPlus.Decoder.EventArguments;
using SharpPropoPlus.Events;
using SharpPropoPlus.Interfaces;

namespace SharpPropoPlus.ViewModels
{
    public class LoggingTabViewModel : BaseViewModel, ILoggingTabViewModel
    {
        private readonly IDialogService _dialogService;

        public LoggingTabViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        private FileStream _pulseStream;
        private StreamWriter _pulseWriter;

        private bool _isPulseRecordingEnabled;

        public bool IsPulseRecordingEnabled
        {
            get => _isPulseRecordingEnabled;

            set
            {
                if (_isPulseRecordingEnabled == value)
                {
                    return;
                }

                _isPulseRecordingEnabled = value;

                if (_isPulseRecordingEnabled)
                {
                    var settings = new SaveFileDialogSettings
                    {
                        Title = "Save As",
                        Filter = "Text Documents (*.txt)|*.txt|All Files (*.*)|*.*",
                        FileName = "SharpPropoPlus_Debug_Pulse.txt",
                        OverwritePrompt = true,
                        CheckFileExists = false,
                    };

                    var success = _dialogService.ShowSaveFileDialog(this, settings);

                    _isPulseRecordingEnabled = success == true;

                    if (_isPulseRecordingEnabled)
                    {
                        _pulseStream = new FileStream(settings.FileName, FileMode.Create);
                        _pulseWriter = new StreamWriter(_pulseStream);

                        GlobalEventAggregator.Instance.AddListener<DebugPulseEventArgs>(DebugPulseListner);
                    }
                }
                else
                {
                    GlobalEventAggregator.Instance.RemoveListener<DebugPulseEventArgs>(DebugPulseListner);

                    _pulseWriter?.Flush();
                    _pulseStream?.Seek(0, SeekOrigin.Begin);
                    _pulseStream?.Dispose();
                }

                OnPropertyChanged();
            }
        }

        private void DebugPulseListner(DebugPulseEventArgs args)
        {
            if (_pulseWriter?.BaseStream?.Length > 0)
            {
                _pulseWriter.WriteLine();
            }

            _pulseWriter?.Write($">>> Pulse length (Raw/Normalized): {args.RawLength}/{args.NormalizedLength} ");

            _pulseWriter?.Write($"{(args.Negative ? "Low" : "High")}{Environment.NewLine}");

            var line = 0;
            var row = 0;

            foreach (int sample in args.Samples)
            {
                if (!(row > 0))
                    _pulseWriter?.Write($">> {line++:D3},  ");
                row++;

                _pulseWriter?.Write($"{unchecked((ushort) sample):D6}");
                if (row == 16 /*&& (args.Samples.Length % 16) > 0*/)
                {
                    _pulseWriter?.Write(Environment.NewLine);
                    row = 0;
                }
                else
                {
                    _pulseWriter?.Write(", ");
                }
            }
        }
    }
}