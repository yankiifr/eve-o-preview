using EveOPreview.Configuration;
using EveOPreview.Services;
using EveOPreview.View.Implementation;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows;
using System.Windows.Forms;
using Rectangle = System.Drawing.Rectangle;

namespace EveOPreview.View
{
	public partial class ThumbnailOverlay : Form
	{
		#region Private fields
		private readonly Action<object, EventArgs> _areaMouseEnterAction;
		private readonly Action<object, EventArgs> _areaMouseLeaveAction;
		private readonly Action<object, MouseEventArgs> _areaMouseDownAction;
		private readonly Action<object, MouseEventArgs> _areaMouseUpAction;
		private readonly Action<object, MouseEventArgs> _areaMouseMoveAction;
		private bool _showOverlayText = true;
		#endregion

		public ThumbnailOverlay(Form owner,
			Action<object, EventArgs> areaMouseEnterAction,
			Action<object, EventArgs> areaMouseLeaveAction,
			Action<object, MouseEventArgs> areaMouseDownAction,
			Action<object, MouseEventArgs> areaMouseUpAction,
			Action<object, MouseEventArgs> areaMouseMoveAction
			)
		{
			this.Owner = owner;
			this._areaMouseEnterAction = areaMouseEnterAction;
			this._areaMouseLeaveAction = areaMouseLeaveAction;
			this._areaMouseDownAction = areaMouseDownAction;
			this._areaMouseUpAction = areaMouseUpAction;
			this._areaMouseMoveAction = areaMouseMoveAction;

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
		private void OverlayArea_MouseDown(object sender, MouseEventArgs e)
		{
			this._areaMouseDownAction(this, e);
		}
		private void OverlayArea_MouseUp(object sender, MouseEventArgs e)
		{
			this._areaMouseUpAction(this, e);
		}
		private void OverlayArea_MouseMove(object sender, MouseEventArgs e)
		{
			this._areaMouseMoveAction(this, e);
		}

		public void SetOverlayLabel(string label)
		{
			this.OverlayLabel.Text = label;
		}
		public void SetCycleGroupIndicator(bool displayCycleGroup, ZoomAnchor anchor)
		{
			if (displayCycleGroup)
			{
				this.CycleGroupIndicator.Visible = true;
				int margin = 2;
				int size = Math.Min(Math.Min(this.Height - margin, this.Width - margin), 40);

				this.CycleGroupIndicator.BackColor = this.OverlayAreaPictureBox.BackColor;
				this.CycleGroupIndicator.Width = size;
				this.CycleGroupIndicator.Height = size;

				this.CycleGroupIndicator.Top = 1;
				this.CycleGroupIndicator.Left = this.Width - size - 2;
				switch (anchor)
				{
					case ZoomAnchor.NW:
						this.CycleGroupIndicator.Left = margin;
						this.CycleGroupIndicator.Top = margin;
						break;
					case ZoomAnchor.N:
						this.CycleGroupIndicator.Left = (this.Width / 2) - (this.CycleGroupIndicator.Width / 2);
						this.CycleGroupIndicator.Top = margin;
						break;
					case ZoomAnchor.NE:
						this.CycleGroupIndicator.Left = this.Width - this.CycleGroupIndicator.Width - margin;
						this.CycleGroupIndicator.Top = margin;
						break;
					case ZoomAnchor.W:
						this.CycleGroupIndicator.Left = margin;
						this.CycleGroupIndicator.Top = (this.Height / 2) - (this.CycleGroupIndicator.Height / 2);
						break;
					case ZoomAnchor.C:
						this.CycleGroupIndicator.Left = (this.Width / 2) - (this.CycleGroupIndicator.Width / 2);
						this.CycleGroupIndicator.Top = (this.Height / 2) - (this.CycleGroupIndicator.Height / 2);
						break;
					case ZoomAnchor.E:
						this.CycleGroupIndicator.Left = this.Width - this.CycleGroupIndicator.Width - margin;
						this.CycleGroupIndicator.Top = (this.Height / 2) - (this.CycleGroupIndicator.Height / 2);
						break;
					case ZoomAnchor.SW:
						this.CycleGroupIndicator.Left = margin;
						this.CycleGroupIndicator.Top = this.Height - this.CycleGroupIndicator.Height - margin;
						break;
					case ZoomAnchor.S:
						this.CycleGroupIndicator.Left = (this.Width / 2) - (this.CycleGroupIndicator.Width / 2);
						this.CycleGroupIndicator.Top = this.Height - this.CycleGroupIndicator.Height - margin;
						break;
					case ZoomAnchor.SE:
						this.CycleGroupIndicator.Left = this.Width - this.CycleGroupIndicator.Width - margin;
						this.CycleGroupIndicator.Top = this.Height - this.CycleGroupIndicator.Height - margin;
						break;
				}


			}
			else
			{
				this.CycleGroupIndicator.Visible = false;
			}
		}

		public void SetPropertiesOverlayLabel(Font font, System.Drawing.Color foregroundColour, System.Drawing.Color outlineColour, float outlineSize, ZoomAnchor anchor)
		{
			if (
				this.OverlayLabel.Font.Size != font.Size ||
				this.OverlayLabel.Font.FontFamily != font.FontFamily ||
				this.OverlayLabel.Font.Italic != font.Italic ||
				this.OverlayLabel.Font.Bold != font.Bold
				)
			{
				this.OverlayLabel.Font = font;
			}
			this.OverlayLabel.ForeColor = foregroundColour;
			
			this.OverlayLabel.BorderColor = outlineColour;
			this.OverlayLabel.BorderSize = outlineSize;

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
			if (this.OverlayLabel.Top < 0) this.OverlayLabel.Top = 0;
			if (this.OverlayLabel.Left < 0) this.OverlayLabel.Left = 0;
		}

		public void EnableOverlayLabel(bool enable)
		{
			//this.OverlayLabel.Visible = enable;
			this._showOverlayText = enable;
		}
		public void EnableFakePreview(bool enable, bool resizeForHighlight, int highlightSize, Color bgColor)
		{
			bool IsLocationUpdateRequired(System.Drawing.Point currentLocation, int left, int top)
			{
				return (currentLocation.X != left) || (currentLocation.Y != top);
			}

			bool IsSizeUpdateRequired(System.Drawing.Size currentSize, int width, int height)
			{
				return (currentSize.Width != width) || (currentSize.Height != height);
			}


			if (!enable)
			{
				OverlayAreaPictureBox.BackColor = Color.Transparent;
				OverlayLabel.BackColor = Color.Transparent;
				OverlayAreaPictureBox.Dock = DockStyle.Fill;
			}
			else
			{
				OverlayAreaPictureBox.BackColor = bgColor;
				OverlayLabel.BackColor = Color.Transparent;
				OverlayAreaPictureBox.Dock = DockStyle.None;
			}

			var left = 0 + highlightSize;
			var top = 0 + highlightSize;
			if (IsLocationUpdateRequired(OverlayAreaPictureBox.Location, left, top))
			{
				OverlayAreaPictureBox.Location = new System.Drawing.Point(left, top);
			}
			var width = this.ClientSize.Width - (highlightSize * 2);
			var height = this.ClientSize.Height - (highlightSize * 2);
			if (IsSizeUpdateRequired(OverlayAreaPictureBox.Size, width, height))
			{
				OverlayAreaPictureBox.Size = new System.Drawing.Size(width, height);
			}
		}

		private void PaintDrawText(PaintEventArgs e, BorderLabel l)
		{
			var flags = TextFormatFlags.Right;
			if (l.TextAlign == ContentAlignment.TopLeft || l.TextAlign == ContentAlignment.BottomLeft || l.TextAlign == ContentAlignment.MiddleLeft) flags = TextFormatFlags.Left;
			flags = flags | TextFormatFlags.WordBreak;

			e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			e.Graphics.CompositingQuality = CompositingQuality.HighQuality;


			if ( l.ForeColor == l.BorderColor || l.BorderSize == 0 )
			{
				TextRenderer.DrawText(e.Graphics, l.Text, l.Font, new Rectangle(l.Left, l.Top, l.Width, l.Height), l.ForeColor, flags);
				return;
			}

			float fontSize = e.Graphics.DpiY * l.Font.SizeInPoints / 72;
			var drawSize = e.Graphics.MeasureString(l.Text, l.Font, new PointF(), StringFormat.GenericTypographic);
			var drawPath = new GraphicsPath();
			var drawPen = new Pen(new SolidBrush(l.BorderColor), l.BorderSize);
			var forecolorBrush = new SolidBrush(l.ForeColor);
			var point = new System.Drawing.Point();

			int margin = 6;

			if (l.AutoSize)
			{
				point.X = margin;
				point.Y = margin;
			}
			else
			{
				// Text is Left-Aligned:
				if (l.TextAlign == ContentAlignment.TopLeft ||
					l.TextAlign == ContentAlignment.MiddleLeft ||
					l.TextAlign == ContentAlignment.BottomLeft)
					point.X = margin;

				// Text is Center-Aligned
				else if (l.TextAlign == ContentAlignment.TopCenter ||
					l.TextAlign == ContentAlignment.MiddleCenter ||
					l.TextAlign == ContentAlignment.BottomCenter)
					point.X = (int)(l.Width - drawSize.Width) / 2;

				// Text is Right-Aligned
				else point.X = (int)( l.Width - (margin + drawSize.Width));

				// Text is Top-Aligned
				if (l.TextAlign == ContentAlignment.TopLeft ||
					l.TextAlign == ContentAlignment.TopCenter ||
					l.TextAlign == ContentAlignment.TopRight)
					point.Y = margin;

				// Text is Middle-Aligned
				else if (l.TextAlign == ContentAlignment.MiddleLeft ||
					l.TextAlign == ContentAlignment.MiddleCenter ||
					l.TextAlign == ContentAlignment.MiddleRight)
					point.Y = (int)((l.Height - drawSize.Height) / 2);

				// Text is Bottom-Aligned
				else point.Y = (int) (l.Height - (margin + drawSize.Height));
			}
			point.X += l.Location.X;
			point.Y += l.Location.Y;

			drawPath.Reset();
			drawPath.AddString(l.Text, l.Font.FontFamily, (int)l.Font.Style, fontSize,point, StringFormat.GenericTypographic);
			//drawPath.AddString(l.Text, l.Font.FontFamily, (int)l.Font.Style, fontSize, new Rectangle(l.Left, l.Top, l.Width, l.Height), StringFormat.GenericTypographic);

			// And finally, using our pen, all we have to do now
			//  is draw our graphics path to the screen. Voila!
			e.Graphics.FillPath(forecolorBrush, drawPath);
			e.Graphics.DrawPath(drawPen, drawPath);

			drawPath.Dispose();
			drawPen.Dispose();
			forecolorBrush.Dispose();

		}

		private void OverlayAreaPictureBox_Paint(object sender, PaintEventArgs e)
		{
			if (this._showOverlayText) PaintDrawText(e, OverlayLabel);
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
