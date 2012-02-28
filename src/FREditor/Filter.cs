using System;

namespace FREditor
{
	public class Filter
	{
		public Filter(string name, ulong regionId, int sourceIndex)
		{
			Name = name;
			RegionId = regionId;
			SourceIndex = sourceIndex;
		}

		public string Name { get; set; }
		public ulong RegionId { get; set; }
		public int SourceIndex { get; set; }

		public bool CanSearch()
		{
			return !String.IsNullOrEmpty(Name)
				|| RegionId != 0 || SourceIndex != 0;
		}
	}
}