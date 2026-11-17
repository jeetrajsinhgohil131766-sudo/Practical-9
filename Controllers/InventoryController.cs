
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class InventoryController : Controller
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["InventoryDB"].ConnectionString;

        public ActionResult Index()
        {
            List<InventoryItem> items = new List<InventoryItem>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM InventoryItems ORDER BY ItemId DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            items.Add(new InventoryItem
                            {
                                ItemId = Convert.ToInt32(dr["ItemId"]),
                                ItemName = dr["ItemName"].ToString(),
                                Category = dr["Category"].ToString(),
                                Quantity = Convert.ToInt32(dr["Quantity"]),
                                Price = Convert.ToDecimal(dr["Price"]),
                                Supplier = dr["Supplier"].ToString()
                            });
                        }
                    }
                }
            }

            return View(items);
        }

        public ActionResult Details(int id)
        {
            InventoryItem item = GetItem(id);

            if (item == null)
                return HttpNotFound();

            return View(item);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InventoryItem item)
        {
            if (!ModelState.IsValid)
                return View(item);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO InventoryItems
                    (ItemName, Category, Quantity, Price, Supplier)
                    VALUES
                    (@ItemName, @Category, @Quantity, @Price, @Supplier)";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    AddParameters(cmd, item);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            InventoryItem item = GetItem(id);

            if (item == null)
                return HttpNotFound();

            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InventoryItem item)
        {
            if (!ModelState.IsValid)
                return View(item);

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"UPDATE InventoryItems SET
                    ItemName = @ItemName,
                    Category = @Category,
                    Quantity = @Quantity,
                    Price = @Price,
                    Supplier = @Supplier
                    WHERE ItemId = @ItemId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    AddParameters(cmd, item);
                    cmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = item.ItemId;

                    con.Open();

                    if (cmd.ExecuteNonQuery() == 0)
                        return HttpNotFound();
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            InventoryItem item = GetItem(id);

            if (item == null)
                return HttpNotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM InventoryItems WHERE ItemId = @ItemId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = id;
                    con.Open();

                    if (cmd.ExecuteNonQuery() == 0)
                        return HttpNotFound();
                }
            }

            return RedirectToAction("Index");
        }

        private InventoryItem GetItem(int id)
        {
            InventoryItem item = null;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM InventoryItems WHERE ItemId = @ItemId";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@ItemId", SqlDbType.Int).Value = id;
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            item = new InventoryItem
                            {
                                ItemId = Convert.ToInt32(dr["ItemId"]),
                                ItemName = dr["ItemName"].ToString(),
                                Category = dr["Category"].ToString(),
                                Quantity = Convert.ToInt32(dr["Quantity"]),
                                Price = Convert.ToDecimal(dr["Price"]),
                                Supplier = dr["Supplier"].ToString()
                            };
                        }
                    }
                }
            }

            return item;
        }

        private void AddParameters(SqlCommand cmd, InventoryItem item)
        {
            cmd.Parameters.Add("@ItemName", SqlDbType.NVarChar, 100).Value = item.ItemName;
            cmd.Parameters.Add("@Category", SqlDbType.NVarChar, 50).Value = item.Category;
            cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = item.Quantity;

            SqlParameter price = cmd.Parameters.Add("@Price", SqlDbType.Decimal);
            price.Precision = 10;
            price.Scale = 2;
            price.Value = item.Price;

            cmd.Parameters.Add("@Supplier", SqlDbType.NVarChar, 100).Value = item.Supplier;
        }
    }
}
