﻿using Reclaimer.Models;
using Reclaimer.Plugins;
using Studio.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Reclaimer.Controls
{
    /// <summary>
    /// Interaction logic for OutputViewer.xaml
    /// </summary>
    public partial class OutputViewer
    {
        #region Dependency Properties
        public static readonly DependencyProperty WordWrapEnabledProperty =
            DependencyProperty.Register(nameof(WordWrapEnabled), typeof(bool), typeof(OutputViewer), new PropertyMetadata(false, WordWrapEnabledChanged));

        public bool WordWrapEnabled
        {
            get => (bool)GetValue(WordWrapEnabledProperty);
            set => SetValue(WordWrapEnabledProperty, value);
        }

        public static void WordWrapEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as OutputViewer;
            control.txtOutput.TextWrapping = control.WordWrapEnabled ? TextWrapping.Wrap : TextWrapping.NoWrap;
        }
        #endregion

        private static TabModel instance;
        public static TabModel Instance
        {
            get
            {
                if (instance == null)
                {
                    var viewer = new OutputViewer();
                    instance = new TabModel(viewer, TabItemType.Tool);
                    instance.Header = instance.ToolTip = "Output";
                }

                return instance;
            }
        }

        public ObservableCollection<Tuple<string, string>> LoadedPlugins { get; }

        public OutputViewer()
        {
            LoadedPlugins = new ObservableCollection<Tuple<string, string>>();

            var list = Substrate.AllPlugins.Select(p => Tuple.Create(p.Key, p.Name)).ToList();
            LoadedPlugins.Add(Tuple.Create(string.Empty, "All Plugins"));

            foreach (var p in list.OrderBy(t => t.Item2))
                LoadedPlugins.Add(p);

            InitializeComponent();
            DataContext = this;

            cmbPlugins.SelectedIndex = 0;
        }

        #region Event Handlers
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cmbPlugins_SelectionChanged(null, null);

            Substrate.Log += Substrate_Log;
            Substrate.EmptyLog += Substrate_EmptyLog;
            txtOutput.CaretIndex = txtOutput.Text.Length;
            txtOutput.ScrollToEnd();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Substrate.Log -= Substrate_Log;
            Substrate.EmptyLog -= Substrate_EmptyLog;
        }

        private void cmbPlugins_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var key = cmbPlugins.SelectedValue as string;

            var selection = key == string.Empty
                ? Substrate.AllPlugins
                : new[] { Substrate.GetPlugin(key) };

            var output = selection.SelectMany(p => p.logEntries)
                .OrderBy(p => p.Timestamp)
                .Select(p => p.Message);

            txtOutput.Clear();
            if (output.Any())
                txtOutput.AppendText(string.Join(Environment.NewLine, output) + Environment.NewLine);
            txtOutput.CaretIndex = txtOutput.Text.Length;
            txtOutput.ScrollToEnd();
        }

        private void btnClearAll_Click(object sender, RoutedEventArgs e)
        {
            var key = cmbPlugins.SelectedValue as string;

            var selection = key == string.Empty
                ? Substrate.AllPlugins
                : new[] { Substrate.GetPlugin(key) };

            foreach (var p in selection)
                p.ClearLog();

            cmbPlugins_SelectionChanged(null, null);
        }

        private async void Substrate_Log(object sender, LogEventArgs e)
        {
            try
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    var key = cmbPlugins.SelectedValue as string;

                    if (key != string.Empty && key != e.Source.Key)
                        return;

                    txtOutput.AppendText(e.Entry.Message + Environment.NewLine);

                    if (txtOutput.CaretIndex == txtOutput.Text.Length)
                        txtOutput.ScrollToEnd();
                });
            }
            catch (TaskCanceledException) { }
        }

        private async void Substrate_EmptyLog(object sender, LogEventArgs e)
        {
            try
            {
                await Dispatcher.InvokeAsync(() =>
                {
                    var key = cmbPlugins.SelectedValue as string;

                    if (key != string.Empty && key != e.Source.Key)
                        return;

                    cmbPlugins_SelectionChanged(null, null);
                });
            }
            catch (TaskCanceledException) { }
        }
        #endregion
    }
}
