using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IForgot
{
	class Program
	{
		private static SettingsModel settings;

		static void Main(string[] args)
		{
			try
			{
				Console.WriteLine(Strings.CreditsApp);
				Console.WriteLine(Strings.CreditsIcon);
				Console.WriteLine();
				if (!Init())
				{
					throw new Exception("The settings.json file is corrupt. Please delete it.");
				}
				Shooter shooter = new Shooter();
				while (true)
				{
					try
					{
						Thread.Sleep(settings.IntervalInMinutes * 60 * 1000);
						string filePath = shooter.Shoot(settings.Path);
						Console.WriteLine("New screenshot taken at {0}: {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), filePath);
					}
					catch (Exception e)
					{
						Console.WriteLine("Something went wrong: {0}", e.Message);
						Thread.Sleep(10000);
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Something went wrong: {0}", e.Message);
			}
			finally
			{
				Console.ReadKey();
			}
		}

		private static bool Init()
		{
			if (!File.Exists("settings.json"))
			{
				settings = new SettingsModel();
				bool valid = false;
				while (!valid)
				{
					Console.WriteLine("Please provide the screenshot interval (in minutes):");
					Console.Write("> ");
					string intervalString = Console.ReadLine();
					int interval = 0;
					if (int.TryParse(intervalString, out interval))
					{
						valid = true;
						settings.IntervalInMinutes = interval;
					}
				}
				valid = false;

				while (!valid)
				{
					Console.WriteLine("Please provide the path to where the screenshots should be stored:");
					Console.Write("> ");
					string path = Console.ReadLine();
					if (Directory.Exists(path))
					{
						valid = true;
						settings.Path = path;
					}
				}
				valid = false;

				while (!valid)
				{
					Console.WriteLine("How many days should the screenshots be saved?");
					Console.Write("> ");
					string howManyDaysString = Console.ReadLine();
					int howManyDays = 0;
					if (int.TryParse(howManyDaysString, out howManyDays))
					{
						valid = true;
						settings.HowManyDays = howManyDays;
					}
				}

				File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings));
				return true;
			}

			settings = JsonConvert.DeserializeObject<SettingsModel>(File.ReadAllText("settings.json"));
			if (settings == null)
			{
				return false;
			}

			Cleanup();

			return true;
		}

		private static void Cleanup()
		{
			string[] files = Directory.GetFiles(settings.Path);
			int deleteCount = 0;
			foreach (string file in files)
			{
				string[] parts = file.Split(new char[] { Path.DirectorySeparatorChar });
				string rawDate = parts[parts.Length - 1].Replace("screen", string.Empty).Replace(".png", string.Empty);
				DateTime screenShotTime = DateTime.MinValue;
				DateTime.TryParseExact(rawDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out screenShotTime);
				if (screenShotTime != DateTime.MinValue && DateTime.Now.AddDays(-settings.HowManyDays) > screenShotTime)
				{
					File.Delete(file);
					deleteCount++;
				}
			}
			if (deleteCount > 0)
			{
				Console.WriteLine("Deleted {0} old screenshots.", deleteCount);
			}
		}
	}
}
