namespace Rhino.ETL.Tests
{
	using System;
	using Engine;
	using MbUnit.Framework;

	[TestFixture]
	public class RowFixture
	{
		[Test]
		public void CanGetValuesFromFields()
		{
			Row row = Row.FromObject(new Customer());
			Assert.AreEqual(5, row["Id"]);
		}

		[Test]
		public void CanGetValuesFromProperties()
		{
			Row row = Row.FromObject(new Customer());
			Assert.AreEqual("ayende", row["Name"]);
		}

		[Test]
		public void GetGetValueFromProtectedField()
		{
			Row row = Row.FromObject(new Customer());
			Assert.AreEqual(DateTime.Today, row["Time"]);
		}

		[Test]
		public void CanGetValueFromProtectedProperty()
		{
			Row row = Row.FromObject(new Customer());
			Assert.AreEqual(15d, row["Salary"]);
		}
	}

	public class Customer
	{
		protected DateTime Time = DateTime.Today;

		private readonly decimal salary = 15;

		protected decimal Salary
		{
			get { return salary; }
		}

		public int Id = 5;
		private string name = "ayende";

		public string Name
		{
			get { return name; }
		}
	}
}