using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IForgot
{
	class Program
	{
		private static SettingsModel settings;
		private static bool running;

		static void Main(string[] args)
		{
			try
			{
				Console.WriteLine(Strings.CreditsApp);
				Console.WriteLine(Strings.CreditsIcon);
				Console.WriteLine(Strings.Buttons);
				Console.WriteLine();
				if (!Init())
				{
					throw new Exception(Strings.SettingsFileCorrupt);
				}
				Shooter shooter = new Shooter();
				Action shoot = () =>
				{
					string filePath = shooter.Shoot(settings.Path);
					Console.WriteLine(Strings.NewScreenshotTaken, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), filePath);
				};
				Action start = () =>
				{
					Task.Run(() =>
					{
						running = true;
						while (running)
						{
							try
							{
								shoot();
								Thread.Sleep(10000);
							}
							catch (Exception e)
							{
								Console.WriteLine(Strings.SomethingWentWrong, e.Message);
								Thread.Sleep(10000);
							}
						}
					});
				};
				start();
				while (true)
				{
					var key = Console.ReadKey();
					if(key.Key == ConsoleKey.Spacebar)
					{
						Console.WriteLine(Strings.ShootScreenshotIn3Seconds);
						Thread.Sleep(3000);
						shoot();
					}
					else if(key.Key == ConsoleKey.Delete)
					{
						running = false;
						File.Delete(Strings.SettingsFileName);
						if (!Init())
						{
							throw new Exception(Strings.SettingsFileCorrupt);
						}
						start();
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(Strings.SomethingWentWrong, e.Message);
			}
			finally
			{
				Console.ReadKey();
			}
		}

		private static bool Init()
		{
			if (!File.Exists(Strings.SettingsFileName))
			{
				settings = new SettingsModel();
				bool valid = false;
				while (!valid)
				{
					Console.WriteLine(Strings.ProvideInterval);
					Console.Write("> ");
					string intervalString = Console.ReadLine();
					int interval = 0;
					if (int.TryParse(intervalString, out interval))
					{
						valid = true;
						settings.IntervalInSeconds = interval;
					}
				}
				valid = false;

				while (!valid)
				{
					Console.WriteLine(Strings.ProvidePath);
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
					Console.WriteLine(Strings.HowManyDays);
					Console.Write("> ");
					string howManyDaysString = Console.ReadLine();
					int howManyDays = 0;
					if (int.TryParse(howManyDaysString, out howManyDays))
					{
						valid = true;
						settings.HowManyDays = howManyDays;
					}
				}

				File.WriteAllText(Strings.SettingsFileName, JsonConvert.SerializeObject(settings));
				return true;
			}

			settings = JsonConvert.DeserializeObject<SettingsModel>(File.ReadAllText(Strings.SettingsFileName));
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
				string rawDate = parts[parts.Length - 1].Replace("screen", string.Empty).Replace(".jpg", string.Empty);
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
				Console.WriteLine(Strings.DeleteOldScreenshots, deleteCount);
			}
		}
	}
}
