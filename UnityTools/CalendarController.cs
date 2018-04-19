using System;
using System.Collections.Generic;
using System.Globalization;

namespace UnityTools {

    public class CalendarController {

		#region Data_Class
		/// <summary>
		/// Class which used to store the data of day.
		/// </summary>
		public class DayData {

			/// <summary>
			/// Day number.
			/// </summary>
			public int day;
			/// <summary>
			/// Day of week.
			/// </summary>
			public DayOfWeek dayOfWeek;

			/// <summary>
			/// Get the day of week in numeric form.
			/// </summary>
			public int getWeekDay() {

				switch (dayOfWeek) {
				case DayOfWeek.Monday:
					return 1;
				case DayOfWeek.Tuesday:
					return 2;
				case DayOfWeek.Wednesday:
					return 3;
				case DayOfWeek.Thursday:
					return 4;
				case DayOfWeek.Friday:
					return 5;
				case DayOfWeek.Saturday:
					return 6;
				case DayOfWeek.Sunday:
					return 7;
				default:
					return -1;
				}
	
			}

		}
		#endregion

		#region Fields_And_Properties
		private DateTime _date;
		private Calendar calendar;

		/// <summary>
		/// List which used to store the data of days from the month component.
		/// </summary>
		public List<DayData> days {
			get;
			private set;
		}
		#endregion

		#region Functions
		/// <summary>
		/// Initialize the calendar controller instance.
		/// </summary>
		public void init() {

			DateTime today = DateTime.Today;
			_date = new DateTime (today.Year, today.Month, today.Day, new GregorianCalendar ());
			calendar = CultureInfo.InvariantCulture.Calendar;
			days = new List<DayData> ();
			loadMonth ();

		}

		/// <summary>
		/// Get the day of week component in numeric form.
		/// </summary>
		public int getWeekDay() {

			switch (_date.DayOfWeek) {
			case DayOfWeek.Monday:
				return 1;
			case DayOfWeek.Tuesday:
				return 2;
			case DayOfWeek.Wednesday:
				return 3;
			case DayOfWeek.Thursday:
				return 4;
			case DayOfWeek.Friday:
				return 5;
			case DayOfWeek.Saturday:
				return 6;
			case DayOfWeek.Sunday:
				return 7;
			default:
				return -1;
			}

		}

		/// <summary>
		/// Gets the day component.
		/// </summary>
		public int getDay() {

			return _date.Day;

		}

		/// <summary>
		/// Add the month component.
		/// </summary>
		public void addMonth(int month) {

			_date = calendar.AddMonths (_date, month);
			loadMonth ();

		}

		/// <summary>
		/// Subtract the month component.
		/// </summary>
		public void subtractMonth(int month) {

			_date = calendar.AddMonths (_date, -1 * month);
			loadMonth ();

		}

		/// <summary>
		/// Sets the month component.
		/// </summary>
		public void setMonth(int month, int year) {

			if (_date.Year == year) {
				_date = calendar.AddMonths (_date, month - _date.Month);
			} else if (_date.Year < year) {
				_date = calendar.AddMonths (_date, month - _date.Month + 12 * (year - _date.Year));
			} else {
				_date = calendar.AddMonths (_date, month - _date.Month - 12 * (year - _date.Year));
			}
			loadMonth ();

		}

		/// <summary>
		/// Get the month component.
		/// </summary>
		public int getMonth() {

			return _date.Month;

		}

		/// <summary>
		/// Get the month component in string.
		/// </summary>
		public string getMonthInString(bool shortForm = false) {

			switch (_date.Month) {
			case 1:
				return shortForm ? "Jan" : "January";
			case 2:
				return shortForm ? "Feb" : "February";
			case 3:
				return shortForm ? "Mar" : "March";
			case 4:
				return shortForm ? "Apr" : "April";
			case 5:
				return "May";
			case 6:
				return shortForm ? "Jun" : "June";
			case 7:
				return shortForm ? "Jul" : "July";
			case 8:
				return shortForm ? "Aug" : "Augest";
			case 9:
				return shortForm ? "Sep" : "September";
			case 10:
				return shortForm ? "Oct" : "October";
			case 11:
				return shortForm ? "Nov" : "November";
			case 12:
				return shortForm ? "Dec" : "December";
			default:
				return null;
			}

		}

		/// <summary>
		/// Get the year component.
		/// </summary>
		public int getYear() {

			return _date.Year;

		}

		private void loadMonth() {

			int numOfDays = calendar.GetDaysInMonth (_date.Year, _date.Month);
			if (days.Count > 0) {
				days.Clear ();
			}
			for (int i = 1; i <= numOfDays; i++) {
				DayData dayData = new DayData ();
				dayData.day = i;
				dayData.dayOfWeek = new DateTime (_date.Year, _date.Month, i).DayOfWeek;
				days.Add (dayData);
			}

		}
		#endregion

	}

}