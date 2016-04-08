using System;

namespace BAModel
{
	public class PublishFile
	{
		public string FileName { get; set; }
		public string FilePath { get; set; }
		public bool FileIsLocal { get; set; }

		private bool _forceContentTypeOther = false;
		public bool ForceContentTypeOther
		{
			get
			{
				return _forceContentTypeOther;
			}
			set
			{
				_forceContentTypeOther = value;
			}
		}
	}
}

