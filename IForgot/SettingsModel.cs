using Newtonsoft.Json;

namespace IForgot
{
	public class SettingsModel
	{
		[JsonProperty("intervalInSeconds")]
		public int IntervalInSeconds { get; set; }

		[JsonProperty("path")]
		public string Path { get; set; }

		[JsonProperty("howManyDays")]
		public int HowManyDays { get; set; }
	}
}
