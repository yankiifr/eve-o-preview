using System;

namespace EveOPreview.Services.Implementation
{
	sealed class ProcessInfo : IProcessInfo
	{
		public ProcessInfo(IntPtr handle, string title, int id)
		{
			this.Handle = handle;
			this.Title = title;
			this.Id = id;
		}

		public IntPtr Handle { get; }
		public string Title { get; }
		public int Id { get; }
	}
}