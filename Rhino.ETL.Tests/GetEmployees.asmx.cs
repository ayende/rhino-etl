using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Services;
using System.Data.SqlClient;

namespace Rhino.ETL.Tests
{
	/// <summary>
	/// Summary description for GetEmployees
	/// </summary>
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	public class GetEmployeesWebService : WebService
	{
		[WebMethod]
		public Employee[] GetEmployees(string companyName)
		{
			List<Employee> employees = new List<Employee>();
			employees.Add(new Employee("ALFKI", "Wrong id"));
			employees.Add(new Employee("BUMP", "Goose"));
			employees.Add(new Employee("Lump", "Of Rock"));
			return employees.ToArray();
		}
	}

	public class Employee
	{
		private string id;
		private string name;

		public Employee()
		{

		}

		public Employee(string id, string name)
		{
			this.id = id;
			this.name = name;
		}

		public string Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}
