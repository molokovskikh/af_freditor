using System;

namespace FREditor
{
	public class Filter
	{
		public Filter(string name, ulong regionId)
		{
			Name = name;
			RegionId = regionId;
		}

		public string Name { get; set; }
		public ulong RegionId { get; set; }

		public bool CanSearch()
		{
			return !String.IsNullOrEmpty(Name)
				|| RegionId != 0;
		}
	}
}