using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IForgot
{
	public class SettingsModel
	{
		[JsonProperty("intervalInMinutes")]
		public int IntervalInMinutes { get; set; }

		[JsonProperty("path")]
		public string Path { get; set; }

		[JsonProperty("howManyDays")]
		public int HowManyDays { get; set; }
	}
}
