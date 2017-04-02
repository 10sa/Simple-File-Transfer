﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SimFTP.Net.Server;

namespace SimFTPV.Forms
{
	public partial class ReceiveConfigs : Form
	{
		Server server;
		public ReceiveConfigs(ref Server cfg)
		{
			InitializeComponent();
			server = cfg;
		}

		private void ReceiveConfigs_Load(object sender, EventArgs e)
		{
			RefreshConfigList();
			RefreshAccountList();
		}

		private void RefreshConfigList()
		{
			foreach(var config in server.config.ConfigTable)
			{
				ListViewItem items = new ListViewItem(config.Key);
				items.SubItems.Add(config.Value);

				listView1.Items.Add(items);
			}

			foreach(ListViewItem item in listView1.Items)
			{
				if(item.SubItems[1].Text == bool.TrueString)
					item.Checked = true;
				else
					item.Checked = false;
			}
			listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		private void RefreshAccountList()
		{
			foreach(var config in server.accountConfig.ConfigTable)
			{
				ListViewItem items = new ListViewItem(config.Key);
				items.SubItems.Add(config.Value);

				listView2.Items.Add(items);
			}

			listView2.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		private void listView1_DoubleClick(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("사용자를 삭제하시겠습니까?", "사용자 삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				foreach(ListViewItem item in listView2.SelectedItems)
				{
					listView2.Items.Remove(item);
					server.accountConfig.RemoveAccount(item.SubItems[0].Text);
				}
			}

			RefreshAccountList();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			AddAcount accountAddHandler = new AddAcount(ref this.server.accountConfig);
			accountAddHandler.ShowDialog();

			RefreshAccountList();
		}

		private void listView1_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if(e.NewValue == CheckState.Checked)
				listView1.Items[e.Index].SubItems[1].Text = bool.TrueString;
			else
				listView1.Items[e.Index].SubItems[1].Text = bool.FalseString;

			server.config.SetConfigTable(listView1.Items[e.Index].SubItems[0].Text, listView1.Items[e.Index].SubItems[1].Text);
		}
	}
}