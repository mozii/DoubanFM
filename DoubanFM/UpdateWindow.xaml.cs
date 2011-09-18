﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Xml.Serialization;
using DoubanFM.Core;
using System.Diagnostics;

namespace DoubanFM
{
	/// <summary>
	/// UpdateWindow.xaml 的交互逻辑
	/// </summary>
	public partial class UpdateWindow : Window
	{
		/// <summary>
		/// 更新器
		/// </summary>
		public Updater Updater { get; private set; }

		public UpdateWindow(DoubanFMWindow owner, Updater updater = null)
		{
			InitializeComponent();
			
			Owner = owner;
			System.Windows.Data.Binding binding = new System.Windows.Data.Binding("Background");
			binding.Source = owner.MainPanel;
			MainPanel.SetBinding(UpdateWindow.BackgroundProperty, binding);
			AllowsTransparency = owner.AllowsTransparency;
			MainPanel.Margin = owner.MainPanel.Margin;

			Updater = updater;
			if (updater == null) Updater = new Updater((FindResource("Player") as Player).Settings);
			ShowRightPanel();
			Updater.StateChanged += new EventHandler((o, e) =>
			{
				ShowRightPanel();
			});
			Updater.ProgressChanged += new System.Net.DownloadProgressChangedEventHandler((o, e) =>
			{
				DownloadProgress.Value = e.ProgressPercentage;
			});
			if (Updater.Now == Core.Updater.State.UnStarted) Updater.Start();
		}
		/// <summary>
		/// 显示正确的面板
		/// </summary>
		void ShowRightPanel()
		{
			switch (Updater.Now)
			{
				case Core.Updater.State.UnStarted:
					ShowPanel(null);
					break;
				case Core.Updater.State.Checking:
					ShowPanel(CheckUpdatePanel);
					break;
				case Core.Updater.State.CheckFailed:
					CheckError.Text = null;
					Exception error = Updater.LastError;
					while (error != null)
					{
						CheckError.Text += error.Message + "\n";
						error = error.InnerException;
					}
					ShowPanel(CheckUpdateFailedPanel);
					break;
				case Core.Updater.State.HasNewVersion:
					VersionName.Content = Updater.NewVersionName;
					PublishTime.Content = Updater.NewVersionPublishTime;
					StringBuilder sb = new StringBuilder();
					foreach (var product in Updater.NewerProducts)
					{
						sb.AppendLine(product.VersionName + " (" + product.PublishTime + " )");
						sb.AppendLine(product.UpdateDetail);
						sb.AppendLine();
					}
					UpdateDetail.Text = sb.ToString();
					ShowPanel(HasNewVersionPanel);
					break;
				case Core.Updater.State.NoNewVersion:
					ShowPanel(NoNewVersionPanel);
					break;
				case Core.Updater.State.Downloading:
					ShowPanel(DownloadingPanel);
					break;
				case Core.Updater.State.DownloadFailed:
					DownloadError.Text = null;
					Exception error2 = Updater.LastError;
					while (error2 != null)
					{
						DownloadError.Text += error2.Message + "\n";
						error2 = error2.InnerException;
					}
					ShowPanel(DownloadFailedPanel);
					break;
				case Core.Updater.State.DownloadCompleted:
					ShowPanel(DownloadCompletedPanel);
					break;
				case Core.Updater.State.Canneled:
					this.Close();
					break;
			}
		}
		/// <summary>
		/// 在特定阶段显示特定面板
		/// </summary>
		void ShowPanel(Panel panel)
		{
			foreach (var p in ContentPanel.Children)
			{
				Panel pp = p as Panel;
				if (pp != null)
				{
					if (panel == pp) pp.Visibility = Visibility.Visible;
					else pp.Visibility = Visibility.Collapsed;
				}
			}
		}

		private void window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// 在此处添加事件处理程序实现。
			this.DragMove();
		}

		private void window_Closed(object sender, EventArgs e)
		{
			if (Updater.Now != Core.Updater.State.DownloadCompleted)
				Updater.Cancel();
			Updater.Dispose();
		}

		private void Close(object sender, System.Windows.RoutedEventArgs e)
		{
			// 在此处添加事件处理程序实现。
			this.Close();
		}

		private void ReCheck(object sender, System.Windows.RoutedEventArgs e)
		{
			// 在此处添加事件处理程序实现。
			Updater.ReCheck();
		}

		private void AutoDownload(object sender, System.Windows.RoutedEventArgs e)
		{
			// 在此处添加事件处理程序实现。
			Updater.Download();
		}

		private void ManualDownload(object sender, System.Windows.RoutedEventArgs e)
		{
			// 在此处添加事件处理程序实现。
			Process.Start(Updater.DownloadLink);
		}

		private void HomePage(object sender, System.Windows.RoutedEventArgs e)
		{
			// 在此处添加事件处理程序实现。
			Process.Start("http://www.kfstorm.com/doubanfm");
		}

		private void ReDownload(object sender, System.Windows.RoutedEventArgs e)
		{
			// 在此处添加事件处理程序实现。
			Updater.Download();
		}

		private void window_Activated(object sender, EventArgs e)
		{
			GradientStopCollection active = (GradientStopCollection)FindResource("ActiveShadowGradientStops");
			GradientStopCollection now = (GradientStopCollection)FindResource("ShadowGradientStops");
			now.Clear();
			foreach (GradientStop g in active)
				now.Add(g);
		}

		private void window_Deactivated(object sender, EventArgs e)
		{
			GradientStopCollection inactive = (GradientStopCollection)FindResource("InactiveShadowGradientStops");
			GradientStopCollection now = (GradientStopCollection)FindResource("ShadowGradientStops");
			now.Clear();
			foreach (GradientStop g in inactive)
				now.Add(g);
		}

		private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Owner.Activate();
		}

	}

}