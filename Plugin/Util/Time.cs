using System;

using NBM.Plugin.Settings;

// 11/6/03

namespace NBM.Plugin
{
	/// <summary>
	/// Contains time formatting methods
	/// </summary>
	public class Time
	{
		private Time()
		{
		}


		/// <summary>
		/// Returns the formatted time according to the timestamp format passed
		/// </summary>
		/// <returns></returns>
		public static string GetFormattedTime()
		{
			DateTime now = DateTime.Now;

			string format = GlobalSettings.Instance().TimeStampFormat;
			string result = string.Empty;

			bool expectKeyLetter = false;

			for (int i=0; i<format.Length; ++i)
			{
				if (!expectKeyLetter)
				{
					if (format[i] == '%')
						expectKeyLetter = true;
					else
						result += format[i];
				}
				else
				{
					switch (format[i])
					{
						case '%':
							result += '%';
							break;
						case 'H':
							result += now.Hour.ToString("00");
							break;
						case 'h':
							result += (now.Hour % 12).ToString("00");
							break;
						case 'm':
							result += now.Minute.ToString("00");
							break;
						case 's':
							result += now.Second.ToString("00");
							break;
						case 'D':
							result += now.Day.ToString();
							break;
						case 'd':
							result += now.DayOfWeek.ToString();
							break;
						case 'E':
							result += ((int)now.DayOfWeek).ToString();
							break;
						case 'M':
							result += now.Month.ToString();
							break;
						case 'N':
							result += ConvertMonthNumberToName( now.Month );
							break;
						case 'Y':
							result += now.Year.ToString();
							break;
						default:
							result += format[i];
							break;
					}

					expectKeyLetter = false;
				}
			}

			return result;
		}


		/// <summary>
		/// Returns the day name according to the corresponding integer passed
		/// </summary>
		/// <param name="num">Number of day - must be between zero and six (zero = sunday)</param>
		/// <returns></returns>
		public static string ConvertDayNumberToName(int num)
		{
			switch (num)
			{
				default:
					throw new System.ArgumentOutOfRangeException("num", num, "Argument num must be between zero and six");

				case 0:
					return "Sunday";
				case 1:
					return "Monday";
				case 2:
					return "Tuesday";
				case 3:
					return "Wednesday";
				case 4:
					return "Thursday";
				case 5:
					return "Friday";
				case 6:
					return "Saturday";
			}
		}


		/// <summary>
		/// Converts the month number to the corresponding name
		/// </summary>
		/// <param name="num">Must be between 1 and 12. 1 = january</param>
		/// <returns></returns>
		public static string ConvertMonthNumberToName(int num)
		{
			switch (num)
			{
				default:
					throw new System.ArgumentOutOfRangeException("num", num, "Argument num must be between one and twelve");

				case 1:
					return "January";
				case 2:
					return "February";
				case 3:
					return "March";
				case 4:
					return "April";
				case 5:
					return "May";
				case 6:
					return "June";
				case 7:
					return "July";
				case 8:
					return "August";
				case 9:
					return "September";
				case 10:
					return "October";
				case 11:
					return "November";
				case 12:
					return "December";
			}
		}
	}
}
