using LifxMvc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Net;
using System.Runtime.Serialization;

namespace LifxMvc.Models
{
	public class BulbsViewModel
	{
		public List<GroupViewModel> Groups { get; private set; }

		public BulbsViewModel()
		{
			this.Groups = new List<GroupViewModel>();
		}
		public BulbsViewModel(IEnumerable<IBulb> bulbs) : this()
		{
			var groupNames = (
				from bulb in bulbs
				select bulb.Group).Distinct();

			foreach (var groupName in groupNames)
			{
				var groupBulbs = bulbs.Where(x => x.Group == groupName).ToList();
				this.Groups.Add(new GroupViewModel(groupName, groupBulbs));
			}

		}

	}

	public class GroupViewModel
	{
		public string Name { get; private set; }
		public List<BulbViewModel> Bulbs { get; private set; }

		public GroupViewModel()
		{
			this.Bulbs = new List<BulbViewModel>();

		}
		public GroupViewModel(string name, List<IBulb> bulbs) : this()
		{
			this.Name = name;
			foreach (var bulb in bulbs)
			{
				this.Bulbs.Add(new BulbViewModel(bulb));
			}

			this.Bulbs.OrderBy(x => x.Label);
		}
	}

	[Serializable]
	public class BulbViewModel 
	{
		public IHSBK HSBK { get; set; }

		public string ColorString { get; set; }
		public string IPEndPoint { get; set; }
		public int BulbId { get; private set; }
		public string Group { get; set; }
		public UInt16 Hue { get; set; }
		public UInt16 Saturation { get; set; }
		public UInt16 Brightness { get; set; }
		public UInt16 Kelvin { get; set; }
		public bool IsOn { get; set; }
		public bool IsColor { get; set; }
		public string Label { get; set; }

		public BulbViewModel(IBulb bulb)
		{
			this.BulbId = bulb.BulbId;
			this.Brightness = bulb.Brightness;

			this.Group = bulb.Group;
			this.Group = bulb.Product.ToString();
			this.Hue = bulb.Hue;
			this.IsOn = bulb.IsOn;
			this.Kelvin = bulb.Kelvin;
			this.Label = bulb.Label;
			this.Saturation = bulb.Saturation;
			uint TxCount = bulb.TxCount;
			this.IPEndPoint = bulb.IPEndPoint.Address.ToString();
			this.HSBK = bulb.HSBK;
			this.ColorString = string.Format("rgb({0},{1},{2})", bulb.Color.R, bulb.Color.G, bulb.Color.B);
			this.IsColor = bulb.IsColor;
			

		}
		[OnSerializing]
		void OnSerializing(StreamingContext ctx)
		{
		}

	}//class


}//ns