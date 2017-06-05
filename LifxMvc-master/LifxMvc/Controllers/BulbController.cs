using LifxMvc.Domain;
using LifxMvc.Models;
using LifxMvc.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Helpers;
using System.Web.Mvc;

namespace LifxMvc.Controllers
{
	public class BulbController : Controller
	{
		const string BULBS = "BulbController.Bulbs";

		ICacheService CacheService { get; set; }
		IDiscoveryService DiscoveryService { get; set; }
		IBulbService BulbService { get; set; }

		List<IBulb> Bulbs
		{
			get { return CacheService.GetOrSet(BULBS, this.GetBulbs); }
		}


		#region Construction
		public BulbController(ICacheService cacheSvc, IDiscoveryService discoverySvc, IBulbService bulbSvc)
		{
			this.CacheService = cacheSvc;
			this.DiscoveryService = discoverySvc;
			this.BulbService = bulbSvc;
		}

		#endregion

		class BulbComparer : IComparer<IBulb>
		{
			public int Compare(IBulb x, IBulb y)
			{
				var result = (x.Group ?? "").CompareTo((y.Group ?? ""));
				if (0 == result)
				{
					result = (x.Label ?? "").CompareTo((y.Label ?? ""));
				}

				return result;
			}
		}

		List<IBulb> GetBulbs()
		{
			List<IBulb> bulbs = new List<IBulb>();
			//using (var svc = new DiscoveryService())
			//{
				bulbs = this.DiscoveryService.DiscoverAsync(1);
				bulbs.Sort(new BulbComparer());
			//}
			return bulbs;
		}

		// GET: Bulb
		public ActionResult Index()
		{
			//var bulbs = this.Bulbs;
			//var vm = new BulbsViewModel(bulbs);
			var vm = new BulbsViewModel();
			return View(vm.Groups);
		}

		public JsonResult IndexJson()
		{
			var bulbs = this.Bulbs;
			var vm = new BulbsViewModel(bulbs);

			var result = Json(vm, JsonRequestBehavior.AllowGet);
			return result;
		}

		public ActionResult old_Index()
		{
			var bulbs = this.Bulbs;
			return View(bulbs);
		}

		public ActionResult Discover()
		{
			this.CacheService.Remove(BULBS);
			var unused = this.Bulbs;
			return RedirectToAction("Index");
		}


		public ActionResult TogglePowerAll()
		{
			var isOn = this.Bulbs.Where(x => x.IsOn).Count() > 0;

			foreach (var bulb in Bulbs)
			{
				this.BulbService.LightSetPower(bulb, !isOn);
			}
			return RedirectToAction("Index");
		}

		public ActionResult TogglePowerGroup(string name)
		{
			var bulbs = this.Bulbs.Where(x => x.Group == name);
			var isOn = bulbs.Where(x => x.IsOn).Count() > 0;
			
			foreach (var bulb in bulbs)
			{
				this.BulbService.LightSetPower(bulb, !isOn);
			}

			return RedirectToAction("Index");
		}

		public ActionResult TogglePowerBulb(int bulbId)
		{
			var bulb = this.Bulbs.FirstOrDefault(x => x.BulbId == bulbId);

			this.BulbService.LightSetPower(bulb, !bulb.IsOn);
			return RedirectToAction("Index");
		}

		public ActionResult DiscoverJson()
		{
			this.CacheService.Remove(BULBS);
			var vm = new BulbsViewModel(this.Bulbs);
			var result = Json(vm, JsonRequestBehavior.AllowGet);
			return result;
		}

		public JsonResult TogglePowerAllJson()
		{
			var isOn = this.Bulbs.Where(x => x.IsOn).Count() > 0;

			foreach (var bulb in Bulbs)
			{
				this.BulbService.LightSetPower(bulb, !isOn);
			}

			var vm = new BulbsViewModel(this.Bulbs);
			var result = Json(vm, JsonRequestBehavior.AllowGet);
			return result;
		}

		public JsonResult TogglePowerGroupJson(string name)
		{
			var bulbs = this.Bulbs.Where(x => x.Group == name);
			var isOn = bulbs.Where(x => x.IsOn).Count() > 0;

			foreach (var bulb in bulbs)
			{
				this.BulbService.LightSetPower(bulb, !isOn);
			}

			var vm = new BulbsViewModel(this.Bulbs);
			var result = Json(vm, JsonRequestBehavior.AllowGet);
			return result;
		}

		public JsonResult TogglePowerBulbJson(int bulbId)
		{
			var bulb = this.Bulbs.FirstOrDefault(x => x.BulbId == bulbId);

			this.BulbService.LightSetPower(bulb, !bulb.IsOn);

			var vm = new BulbsViewModel(this.Bulbs);
			var result = Json(vm, JsonRequestBehavior.AllowGet);
			return result;
		}
																						
		public JsonResult SetColorBulbJson(int bulbId, string color)
		{

			var bulb = this.Bulbs.FirstOrDefault(x => x.BulbId == bulbId);

			var c = this.ParseColor(color);
			this.BulbService.LightSetColor(bulb, c);

			var vm = new BulbsViewModel(this.Bulbs);
			var result = Json(vm, JsonRequestBehavior.AllowGet);
			return result;
		}

		public JsonResult GetKelvinPalette()
		{
			var palette = KelvinColor.GetPalette();
			var result = Json(palette, JsonRequestBehavior.AllowGet);
			return result;
		}

		Color ParseColor(string color)
		{
			//Parse: rgb(255, 0, 137)
			const string REGEX = @"\D+";
			var values = Regex.Split(color, REGEX).Where(x => !string.IsNullOrEmpty(x)).ToArray();

			const int EXPECTED_LENGTH = 3;
			if (EXPECTED_LENGTH != values.Length)
				throw new Exception("Error parsing color.");

			var result = Color.FromArgb(Convert.ToInt32(values[0]),
				Convert.ToInt32(values[1]),
				Convert.ToInt32(values[2]));

			return result;
		}



		[HttpPost]
		public ActionResult Index(List<IBulb> list)
		{
			return View(list);
		}

		public ActionResult GetBulbView()
		{
			var result = PartialView("bulbTableRow.cshtml");
			return result;
		}

	}
}