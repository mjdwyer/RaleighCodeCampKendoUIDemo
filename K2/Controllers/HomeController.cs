using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using K2.Models;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Data.Objects.SqlClient;
using Kendo.Mvc.Infrastructure;



namespace K4.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.Message = "Welcome to ASP.NET MVC!";

			return View();
		}

		public ActionResult Grid1()
		{
			return View();
		}

		public ActionResult Grid2()
		{
			return View();
		}
        
		public JsonResult LoadGrid2([DataSourceRequest] DataSourceRequest request)
		{
			DataSourceResult result = new DataSourceResult();

			using (var northwind = new NorthWindEntities1())
			{

				IQueryable<ProductViewModel> pvm = from p in northwind.Products
												   orderby p.ProductID
												   join c in northwind.Categories on p.CategoryID equals c.CategoryID
												   join s in northwind.Suppliers on p.SupplierID equals s.SupplierID
												   select new ProductViewModel
												   {
													   ProductID = p.ProductID,
													   ProductName = p.ProductName,
													   CompanyName = s.CompanyName,
													   CategoryName = c.CategoryName,
													   QuantityPerUnit = p.QuantityPerUnit,
													   UnitPrice = p.UnitPrice,
													   UnitsInStock = p.UnitsInStock,
													   UnitsOnOrder = p.UnitsOnOrder,
													   ReorderLevel = p.ReorderLevel,
													   Discontinued = p.Discontinued
												   };

				result = (pvm as IQueryable).ToDataSourceResult(request);
				return Json(result, JsonRequestBehavior.AllowGet);

			}


		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult EditingPopup_Update([DataSourceRequest] DataSourceRequest request, ProductViewModel product)
		{
			if (product != null && ModelState.IsValid)
			{

				using (var northwind = new NorthWindEntities1())
				{

					Product target = northwind.Products.SingleOrDefault(p => p.ProductID == product.ProductID);
					if (target != null)
					{
						target.ReorderLevel = product.ReorderLevel;
						target.UnitPrice = product.UnitPrice;
						target.UnitsInStock = product.UnitsInStock;
						target.UnitsOnOrder = product.UnitsOnOrder;
						target.Discontinued = product.Discontinued;
						northwind.SaveChanges();
					}
				}
			}

			return Json(new[] { product }.ToDataSourceResult(request, ModelState));
		}

		public ActionResult Chart2()
		{
			ChartViewModel cvm = new ChartViewModel();

			DataSourceRequest request = new DataSourceRequest();
			request.Aggregates = GridDescriptorSerializer.Deserialize<Kendo.Mvc.AggregateDescriptor>("UnitsInStock-sum");
			request.Groups = GridDescriptorSerializer.Deserialize<Kendo.Mvc.GroupDescriptor>("CategoryName-asc");

			using (var northwind = new NorthWindEntities1())
			{

				IQueryable<ProductViewModel> pvm = from p in northwind.Products
												   orderby p.ProductID
												   join c in northwind.Categories on p.CategoryID equals c.CategoryID
												   join s in northwind.Suppliers on p.SupplierID equals s.SupplierID
												   select new ProductViewModel
												   {
													   ProductID = p.ProductID,
													   ProductName = p.ProductName,
													   CompanyName = s.CompanyName,
													   CategoryName = c.CategoryName,
													   QuantityPerUnit = p.QuantityPerUnit,
													   UnitPrice = p.UnitPrice,
													   UnitsInStock = p.UnitsInStock,
													   UnitsOnOrder = p.UnitsOnOrder,
													   ReorderLevel = p.ReorderLevel,
													   Discontinued = p.Discontinued
												   };

				DataSourceResult data = (pvm as IQueryable).ToDataSourceResult(request);

                var pieSections = (data.Data as List<Kendo.Mvc.Infrastructure.AggregateFunctionsGroup>).Select(p => new { Key = p.Key, Value = (int)((p.Aggregates["UnitsInStock"] as Dictionary<string, object>)["Sum"]) });

                var series = pieSections.Select(p => new ChartDataItem { category = p.Key.ToString(), value = p.Value });

                cvm.series = series.ToList();

			}

			return View(cvm);
		}

		public JsonResult LoadGrid1()
		{

			var take = string.IsNullOrEmpty(Request.QueryString["take"]) ? 1000 : Convert.ToInt32(Request.QueryString["take"]);
			var skip = string.IsNullOrEmpty(Request.QueryString["skip"]) ? 1000 : Convert.ToInt32(Request.QueryString["skip"]);

			var northwind = new NorthWindEntities1();

			var totalRecs = northwind.Orders.Count();

			var dataJson = new
			{
				total = totalRecs,
				data = northwind.Orders.Select(o => new OrderViewModel
						{
							OrderID = o.OrderID,
							Freight = o.Freight,
							ShipName = o.ShipName,
							OrderDate = o.OrderDate,
							ShipCity = o.ShipCity
						}).OrderBy(o => o.OrderID).Skip(skip).Take(take).ToArray()
			};

			return this.Json(dataJson, JsonRequestBehavior.AllowGet);
		}

	}
}
