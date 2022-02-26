#region Copyright (c) 2000-2021 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2021 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2021 Developer Express Inc.

namespace Xenial.Framework.LabelEditors.Win.Model.Core
{
	partial class XenialCssEditorForm
	{
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.buttonsPanel = new DevExpress.XtraEditors.PanelControl();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.cssCodeViewer = new DevExpress.XtraEditors.Internal.CssCodeViewer();
			((System.ComponentModel.ISupportInitialize)(this.buttonsPanel)).BeginInit();
			this.buttonsPanel.SuspendLayout();
			this.cssCodeViewer.SuspendLayout();
			this.SuspendLayout();
			this.buttonsPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.buttonsPanel.Controls.Add(this.btnOk);
			this.buttonsPanel.Controls.Add(this.btnCancel);
			this.buttonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.buttonsPanel.Location = new System.Drawing.Point(10, 663);
			this.buttonsPanel.Name = "buttonsPanel";
			this.buttonsPanel.Padding = new System.Windows.Forms.Padding(24, 0, 0, 0);
			this.buttonsPanel.Size = new System.Drawing.Size(888, 35);
			this.buttonsPanel.TabIndex = 2;
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(729, 7);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 0;
			this.btnOk.Text = "OK";
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(809, 7);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.cssCodeViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cssCodeViewer.Location = new System.Drawing.Point(10, 10);
			this.cssCodeViewer.Name = "htmlCodeViewer1";
			this.cssCodeViewer.Padding = new System.Windows.Forms.Padding(10);
			this.cssCodeViewer.Size = new System.Drawing.Size(888, 653);
			this.cssCodeViewer.TabIndex = 2;
			this.cssCodeViewer.ReadOnly = false;
			this.ClientSize = new System.Drawing.Size(908, 752);
			this.Controls.Add(this.cssCodeViewer);
			this.Controls.Add(this.buttonsPanel);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(500, 400);
			this.Name = "ImageEditorForm";
			this.Padding = new System.Windows.Forms.Padding(10);
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Image Picker";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			((System.ComponentModel.ISupportInitialize)(this.buttonsPanel)).EndInit();
			this.buttonsPanel.ResumeLayout(false);
			this.cssCodeViewer.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.Internal.CssCodeViewer cssCodeViewer;
		private DevExpress.XtraEditors.PanelControl buttonsPanel;
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraEditors.SimpleButton btnOk;
	}
}
