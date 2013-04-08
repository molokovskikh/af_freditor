using System;

namespace FREditor
{
	public class Filter
	{
		public Filter(string name, ulong regionId, int sourceIndex, int supplierIndex)
		{
			Name = name;
			RegionId = regionId;
			SourceIndex = sourceIndex;
			SupplierIndex = supplierIndex;
		}

		public string Name { get; set; }
		public ulong RegionId { get; set; }
		public int SourceIndex { get; set; }
		public int SupplierIndex { get; set; }

		public bool CanSearch()
		{
			return !String.IsNullOrEmpty(Name)
				|| RegionId != 0 || SourceIndex != 0 || SupplierIndex != 0;
		}
	}
}