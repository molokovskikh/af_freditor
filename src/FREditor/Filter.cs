using System;

namespace FREditor
{
	public class Filter
	{
		public Filter(string name, ulong regionId, int segment)
		{
			Name = name;
			RegionId = regionId;
			Segment = segment;
		}

		public string Name { get; set; }
		public ulong RegionId { get; set; }
		public int Segment { get; set; }

		public bool CanSearch()
		{
			return !String.IsNullOrEmpty(Name)
				|| RegionId != 0
				|| Segment != -1;
		}
	}
}