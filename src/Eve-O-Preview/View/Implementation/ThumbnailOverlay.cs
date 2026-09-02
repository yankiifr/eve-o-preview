using System;
using System.Windows.Forms;
using EveOPreview.Configuration;
using EveOPreview.Services;

namespace EveOPreview.View
{
	public partial class ThumbnailOverlay : Form
	{
		#region Private fields
		private readonly Action<object, EventArgs> _areaMouseEnterAction;
		private readonly Action<object, EventArgs> _areaMouseLeaveAction;
		private readonly Action<object, MouseEventArgs> _areaMouseDownAction;
		private readonly Action<object, MouseEventArgs> _areaMouseMoveAction;
		private readonly Action<object, MouseEventArgs> _areaMouseUpAction;
		#endregion

		public ThumbnailOverlay(Form owner,
			Action<object, EventArgs> areaMouseEnterAction,
			Action<object, EventArgs> areaMouseLeaveAction,
			Action<object, MouseEventArgs> areaMouseDownAction,
			Action<object, MouseEventArgs> areaMouseMoveAction,
			Action<object, MouseEventArgs> areaMouseUpAction)
		{
			this.Owner = owner;
			this._areaMouseEnterAction = areaMouseEnterAction;
			this._areaMouseLeaveAction = areaMouseLeaveAction;
			this._areaMouseDownAction = areaMouseDownAction;
			this._areaMouseMoveAction = areaMouseMoveAction;
			this._areaMouseUpAction = areaMouseUpAction;

			InitializeComponent();
		}

		private void OverlayArea_MouseEnter(object sender, EventArgs e)
		{
			this._areaMouseEnterAction(this, e);
		}

		private void OverlayArea_MouseLeave(object sender, EventArgs e)
		{
			this._areaMouseLeaveAction(this, e);
		}

		// The overlay is a separate window placed on top of the thumbnail one.
		// Mouse events received here have to be forwarded to the thumbnail view
		// as a whole sequence. Forwarding just a part of it (like the mouse up event only)
		// leaves the thumbnail view in an inconsistent state
		private void OverlayArea_MouseDown(object sender, MouseEventArgs e)
		{
			this._areaMouseDownAction(this, e);
		}

		private void OverlayArea_MouseMove(object sender, MouseEventArgs e)
		{
			this._areaMouseMoveAction(this, e);
		}

		private void OverlayArea_MouseUp(object sender, MouseEventArgs e)
		{
			this._areaMouseUpAction(this, e);
		}

		public void SetOverlayLabel(string label)
		{
			this.OverlayLabel.Text = label;
		}

		public void SetPropertiesOverlayLabel(int size, System.Drawing.Color c, ZoomAnchor anchor)
		{
			if (this.OverlayLabel.Font.Size != size)
			{
				this.OverlayLabel.Font = new System.Drawing.Font(this.OverlayLabel.Font.FontFamily, size);
			}
			this.OverlayLabel.ForeColor = c;

			int margin = 5;

			switch (anchor)
			{
				case ZoomAnchor.NW:
					this.OverlayLabel.Left = margin;
					this.OverlayLabel.Top = margin;
					this.OverlayLabel.TextAlign = System.Drawing.ContentAlignment.TopLeft;
					break;
                case ZoomAnchor.N:
                    this.OverlayLabel.Left = (this.Width / 2) - (this.OverlayLabel.Width / 2);
                    this.OverlayLabel.Top = margin;
                    this.OverlayLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
                    break;
                case ZoomAnchor.NE:
                    this.OverlayLabel.Left = this.Width - this.OverlayLabel.Width - margin;
                    this.OverlayLabel.Top = margin;
                    this.OverlayLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
                    break;
                case ZoomAnchor.W:
                    this.OverlayLabel.Left = margin;
                    this.OverlayLabel.Top = (this.Height / 2) - (this.OverlayLabel.Height / 2);
                    this.OverlayLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                    break;
                case ZoomAnchor.C:
                    this.OverlayLabel.Left = (this.Width / 2) - (this.OverlayLabel.Width / 2);
                    this.OverlayLabel.Top = (this.Height / 2) - (this.OverlayLabel.Height / 2);
                    this.OverlayLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    break;
                case ZoomAnchor.E:
                    this.OverlayLabel.Left = this.Width - this.OverlayLabel.Width - margin;
                    this.OverlayLabel.Top = (this.Height / 2) - (this.OverlayLabel.Height / 2);
                    this.OverlayLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                    break;
                case ZoomAnchor.SW:
                    this.OverlayLabel.Left = margin;
                    this.OverlayLabel.Top = this.Height - this.OverlayLabel.Height - margin;
                    this.OverlayLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
                    break;
                case ZoomAnchor.S:
                    this.OverlayLabel.Left = (this.Width / 2) - (this.OverlayLabel.Width / 2);
                    this.OverlayLabel.Top = this.Height - this.OverlayLabel.Height - margin;
                    this.OverlayLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
                    break;
                case ZoomAnchor.SE:
                    this.OverlayLabel.Left = this.Width - this.OverlayLabel.Width - margin;
                    this.OverlayLabel.Top = this.Height - this.OverlayLabel.Height - margin;
                    this.OverlayLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
                    break;
            }
		}

		public void EnableOverlayLabel(bool enable)
		{
			this.OverlayLabel.Visible = enable;
		}

		protected override CreateParams CreateParams
		{
			get
			{
				var Params = base.CreateParams;
				Params.ExStyle |= (int)InteropConstants.WS_EX_TOOLWINDOW;
				return Params;
			}
		}
	}
}
