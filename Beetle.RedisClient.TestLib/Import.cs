using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Beetle.Redis;
namespace Beetle.RedisClient.TestLib
{
    [TestClass]
    public  class ImportData
    {
        [TestMethod]
        public void GetEmployee()
        {
            RedisKey rk = "emp_1".Protobuf();
            rk.Get<Employee>();
        }

        [TestMethod]
        public void Import()
        {
           
                Model.DataSet1.OrdersDataTable odt = new Model.DataSet1.OrdersDataTable();
                odt.ReadXml("Orders.xml");
                ProtobufList<Order> orders = "Orders";
                orders.Clear();
                foreach (Model.DataSet1.OrdersRow order in odt.Rows)
                {
                    Order item = new Order();
                    item.CustomerID = order.CustomerID;
                    item.EmployeeID = order.EmployeeID;
                    item.Freight = (double)order.Freight;
                    item.OrderDate = order.OrderDate;
                    item.OrderID = order.OrderID;
                    item.RequiredDate = order.RequiredDate;
                    item.ShipAddress = order.ShipAddress;
                    item.ShipCity = order.ShipCity;
                    item.ShipCountry = order.ShipCountry;
                    item.ShipName = order.ShipName;
                    item.ShippedDate = order.ShippedDate;
                    item.ShipPostalCode = order.ShipPostalCode;
                    item.ShipVia = order.ShipVia;
                    ("order_" + item.OrderID).SetProtobuf(item);
                    orders.Add(item);
                   
                }
          
           
                Model.DataSet1.EmployeesDataTable edt = new Model.DataSet1.EmployeesDataTable();
                edt.ReadXml("Employees.xml");
                foreach (Model.DataSet1.EmployeesRow erow in edt.Rows)
                {
                    Employee emp = new Employee();
                    emp.EmployeeID = erow.EmployeeID;
                    emp.Address = erow.Address;
                    emp.BirthDate = erow.BirthDate;
                    emp.City = erow.City;
                    emp.Country = erow.Country;
                    emp.Extension = erow.Extension;
                    emp.FirstName = erow.FirstName;
                    emp.HireDate = erow.HireDate;
                    emp.HomePhone = erow.HomePhone;
                    emp.LastName = erow.LastName;
                    emp.Notes = erow.Notes;
                    emp.PostalCode = erow.PostalCode;
                    emp.Region = erow.Region;
                    emp.Title = erow.Title;
                    emp.TitleOfCourtesy = erow.TitleOfCourtesy;
                    ("emp_" + emp.EmployeeID).SetProtobuf(emp);
                   
                }
         
           
                Model.DataSet1.CustomersDataTable cdt = new Model.DataSet1.CustomersDataTable();
                cdt.ReadXml("customers.xml");
                foreach (Model.DataSet1.CustomersRow crow in cdt.Rows)
                {
                    Customer cust = new Customer();
                    cust.Address = crow.Address;
                    cust.City = crow.City;
                    cust.CompanyName = crow.CompanyName;
                    cust.ContactName = crow.ContactName;
                    cust.ContactTitle = crow.ContactTitle;
                    cust.Country = crow.Country;
                    cust.CustomerID = crow.CustomerID;
                    cust.Fax = crow.Fax;
                    cust.Phone = crow.Phone;
                    cust.PostalCode = crow.PostalCode;
                    ("cust_" + cust.CustomerID).SetProtobuf(cust);
                   
                }
           

        }
    }
}
