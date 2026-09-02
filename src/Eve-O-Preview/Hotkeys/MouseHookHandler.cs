using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace EveOPreview.UI.Hotkeys
{
	// Mouse buttons cannot be registered as system-wide hotkeys - the RegisterHotKey API
	// handles keyboard input only. A low level mouse hook is used instead.
	// A single hook is shared by every registered binding.
	sealed class MouseHookHandler : IDisposable
	{
		#region Native
		private const int WH_MOUSE_LL = 14;
		private const int HC_ACTION = 0;

		private const int WM_MBUTTONDOWN = 0x0207;
		private const int WM_XBUTTONDOWN = 0x020B;

		private const int XBUTTON1 = 1;
		private const int XBUTTON2 = 2;

		private delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook, HookProc callback, IntPtr moduleHandle, uint threadId);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool UnhookWindowsHookEx(IntPtr hookHandle);

		[DllImport("user32.dll")]
		private static extern IntPtr CallNextHookEx(IntPtr hookHandle, int code, IntPtr wParam, IntPtr lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string moduleName);

		[StructLayout(LayoutKind.Sequential)]
		private struct MSLLHOOKSTRUCT
		{
			public int X;
			public int Y;
			public int MouseData;
			public int Flags;
			public int Time;
			public IntPtr ExtraInfo;
		}
		#endregion

		#region Private fields
		private static MouseHookHandler _instance;

		private readonly Dictionary<MouseButton, List<Func<bool>>> _handlers;

		// The delegate has to be stored in a field. Otherwise it is collected
		// by the GC while the unmanaged side still holds a pointer to it
		private readonly HookProc _callback;

		private IntPtr _hookHandle;
		#endregion

		private MouseHookHandler()
		{
			this._handlers = new Dictionary<MouseButton, List<Func<bool>>>();
			this._callback = this.HookCallback;
			this._hookHandle = IntPtr.Zero;
		}

		public static MouseHookHandler Instance => MouseHookHandler._instance ?? (MouseHookHandler._instance = new MouseHookHandler());

		// Returns the button described by the provided string, or MouseButton.None
		// if the string does not name a mouse button at all
		public static MouseButton ParseButton(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return MouseButton.None;
			}

			switch (value.Trim().Replace(" ", "").ToUpperInvariant())
			{
				case "XBUTTON1":
				case "MOUSE4":
				case "MOUSEBUTTON4":
					return MouseButton.XButton1;
				case "XBUTTON2":
				case "MOUSE5":
				case "MOUSEBUTTON5":
					return MouseButton.XButton2;
				case "MIDDLE":
				case "MBUTTON":
				case "MOUSE3":
				case "MOUSEBUTTON3":
					return MouseButton.Middle;
				default:
					return MouseButton.None;
			}
		}

		// The handler returns true when it has consumed the button press.
		// A consumed press is not passed to the application under the mouse cursor
		public void Register(MouseButton button, Func<bool> handler)
		{
			if ((button == MouseButton.None) || (handler == null))
			{
				return;
			}

			if (!this._handlers.TryGetValue(button, out List<Func<bool>> handlers))
			{
				handlers = new List<Func<bool>>();
				this._handlers[button] = handlers;
			}

			handlers.Add(handler);

			this.InstallHook();
		}

		public void UnregisterAll()
		{
			this._handlers.Clear();
			this.RemoveHook();
		}

		public void Dispose()
		{
			this.RemoveHook();
			GC.SuppressFinalize(this);
		}

		~MouseHookHandler()
		{
			this.RemoveHook();
		}

		private void InstallHook()
		{
			if (this._hookHandle != IntPtr.Zero)
			{
				return;
			}

			this._hookHandle = MouseHookHandler.SetWindowsHookEx(MouseHookHandler.WH_MOUSE_LL, this._callback, MouseHookHandler.GetModuleHandle(null), 0);
		}

		private void RemoveHook()
		{
			if (this._hookHandle == IntPtr.Zero)
			{
				return;
			}

			MouseHookHandler.UnhookWindowsHookEx(this._hookHandle);
			this._hookHandle = IntPtr.Zero;
		}

		private IntPtr HookCallback(int code, IntPtr wParam, IntPtr lParam)
		{
			// This callback is invoked for every single mouse event of the system.
			// It has to return as fast as possible - Windows silently drops hooks
			// that take too long to answer
			if (code != MouseHookHandler.HC_ACTION)
			{
				return MouseHookHandler.CallNextHookEx(this._hookHandle, code, wParam, lParam);
			}

			MouseButton button = MouseHookHandler.GetPressedButton(wParam.ToInt32(), lParam);

			if ((button != MouseButton.None) && this._handlers.TryGetValue(button, out List<Func<bool>> handlers))
			{
				bool isHandled = false;

				foreach (Func<bool> handler in handlers)
				{
					try
					{
						isHandled |= handler();
					}
					catch (Exception)
					{
						// A failing handler should never break the mouse input of the whole system
					}
				}

				if (isHandled)
				{
					return (IntPtr)1;
				}
			}

			return MouseHookHandler.CallNextHookEx(this._hookHandle, code, wParam, lParam);
		}

		private static MouseButton GetPressedButton(int message, IntPtr lParam)
		{
			if (message == MouseHookHandler.WM_MBUTTONDOWN)
			{
				return MouseButton.Middle;
			}

			if (message != MouseHookHandler.WM_XBUTTONDOWN)
			{
				return MouseButton.None;
			}

			MSLLHOOKSTRUCT data = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);

			// The pressed X button is stored in the high order word of the MouseData field
			switch ((data.MouseData >> 16) & 0xFFFF)
			{
				case MouseHookHandler.XBUTTON1:
					return MouseButton.XButton1;
				case MouseHookHandler.XBUTTON2:
					return MouseButton.XButton2;
				default:
					return MouseButton.None;
			}
		}
	}

	enum MouseButton
	{
		None = 0,
		Middle,
		XButton1,
		XButton2
	}
}
