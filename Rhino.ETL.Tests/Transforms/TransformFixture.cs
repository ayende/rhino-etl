using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;

namespace Rhino.ETL.Tests.Transforms
{
	[TestFixture]
	public class TransformFixture : BaseTest
	{
		private EtlConfigurationContext configurationContext;

		[SetUp]
		public void TestInitialize()
		{
			configurationContext = BuildContext(@"Transforms\transforms_only.retl");
		}

		[Test]
		public void TransformRowFromDSL()
		{
			Transform transform = configurationContext.Transforms["NameToUpper"];
			Row row = new Row();
			row["Name"] = "ayende";
			transform.Apply(row, new Hashtable());
			Assert.AreEqual("AYENDE", row["Name"]);
		}

		[Test]
		public void TransformRowByIteratingAllCoumns()
		{
			Transform transform = configurationContext.Transforms["RemoveCommas"];
			Row row = new Row();
			row["Name"] = "Ayende, Rahien";
			row["Today"] = DateTime.Today;
			row["City"] = "Tel-Aviv";
			transform.Apply(row, new Hashtable());
			Assert.AreEqual("Ayende Rahien", row["Name"]);
			Assert.AreEqual(DateTime.Today, row["Today"]);
			Assert.AreEqual("Tel-Aviv", row["City"]);
		}

		[Test]
		public void TrimEmptyStringAndTranslateToNull()
		{
			Transform transform = configurationContext.Transforms["TrimEmptyStringToNull"];
			Row row = new Row();
			row["Name"] = "A  \t";
			row["City"] = "  ";
			row["Street"] = "\t\t";
			transform.Apply(row, new Hashtable());
			Assert.AreEqual("A  \t", row["Name"]);
			Assert.IsNull(row["City"]);
			Assert.IsNull(row["Street"]);
		}

		[Test]
		public void IntroduceSpace()
		{
			Transform transform = configurationContext.Transforms["IntroduceSpace"];
			Row row = new Row();
			row["PostalCode"] = "SW1A0AA";
			transform.Apply(row, new Hashtable());
			Assert.AreEqual("SW1A 0AA", row["PostalCode"]);
		}

		[Test]
		public void MakeTitleCase_IntroduceNewColumn()
		{
			Transform transform = configurationContext.Transforms["MakeTitleCase"];
			Row row = new Row();
			row["Name"] = "ayende";
			transform.Apply(row, new Hashtable());
			Assert.AreEqual("Ayende", row["Title"]);
		}

		[Test]
		public void MarkRowForRemoval()
		{
			Transform transform = configurationContext.Transforms["RemoveRowsWithoutId"];
			Row row = new Row();
			row["Id"] = 1;
			transform.Apply(row, new Hashtable());
			Assert.IsFalse(transform.ShouldSkipRow);
			row["Id"] = null;
			transform.Apply(row, new Hashtable());
			Assert.IsTrue(transform.ShouldSkipRow);
		}

		[Test]
		public void TranslateDate()
		{
			Transform transform = configurationContext.Transforms["TranslateDate"];
			Row row = new Row();
			row["Date"] = new DateTime(2007, 03, 04).ToString("MMM dd, yyyy");
			transform.Apply(row, new Hashtable());
			Assert.AreEqual("2007-03-04", row["Date"] );
		}

		[Test]
		public void RemoveBadDate()
		{
			Transform transform = configurationContext.Transforms["RemoveBadDate"];
			Row row = new Row();
			row["Date"] = new DateTime(2007, 03, 04).ToString("MMM dd, yyyy");
			transform.Apply(row, new Hashtable());
			Assert.IsNotNull(row["Date"]);
			row["Date"] = "nonesense";
			transform.Apply(row, new Hashtable());
			Assert.IsNull(row["Date"]);
		}

		[Test]
		public void PassParametersToTransform()
		{
			Transform transform = configurationContext.Transforms["RemoveCommasWithParemeters"];
			Row row = new Row();
			row["Name"] = "Ayende, Rahien";
			row["Today"] = DateTime.Today;
			row["City"] = "Tel, Aviv";
			Hashtable parameters = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
			parameters["ColumnsToClean"] = new string[] { "Name" };
			transform.Apply(row, parameters);
			Assert.AreEqual("Ayende Rahien", row["Name"]);
			Assert.AreEqual(DateTime.Today, row["Today"]);
			Assert.AreEqual("Tel, Aviv", row["City"]);
		}
	}
}
