using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiCar.Model;

namespace WebApiCar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {

        static string conn = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=CarDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        
        public static List<Car> carList = new List<Car>()
        {
            new Car(){Id = 1,Model="x3",Vendor="Tesla", Price=400000},
            new Car(){Id = 2,Model="x2",Vendor="Tesla", Price=600000},
            new Car(){Id = 3,Model="x1",Vendor="Tesla", Price=800000},
            new Car(){Id = 4,Model="x0",Vendor="Tesla", Price=1400000},
        };
        

        /// <summary>
        /// Method for get all the cars from the static list
        /// </summary>
        /// <returns>List of cars</returns>
        // GET: api/Cars
        [HttpGet]
        public IEnumerable<Car> Get()
        {
            var carList = new List<Car>();

            string selectall = "select id, vendor, model, price from Car";

            using (SqlConnection databaseConnection = new SqlConnection(conn))
            {
                using (SqlCommand selectCommand = new SqlCommand(selectall, databaseConnection))
                {
                    databaseConnection.Open();

                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string vendor = reader.GetString(1);
                            string model = reader.GetString(2);
                            int price = reader.GetInt32(3);

                            carList.Add(new Car(id,vendor,model,price));

                        }

                    }
                }


            }

            return carList;
        }

        // GET: api/Cars/5
        [HttpGet("{id}", Name = "GetById")]
        public Car Get(int id)
        {
            string selectById = "select vendor, model, price from Car where id = @id";

            using (SqlConnection databaseConnection = new SqlConnection(conn))
            {
                using (SqlCommand selectCommand = new SqlCommand(selectById, databaseConnection))
                {
                    selectCommand.Parameters.AddWithValue("@id", id);
                    databaseConnection.Open();

                    using (SqlDataReader reader = selectCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string vendor = reader.GetString(0);
                            string model = reader.GetString(1);
                            int price = reader.GetInt32(2);

                            return new Car(id, vendor, model, price);

                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Post a new car to the static list
        /// </summary>
        /// <param name="value"></param>
        // POST: api/Cars
        [HttpPost]
        public void Post([FromBody] Car value)
        {
            string insertSql = "insert into car(id, model, vendor, price) values (@id, @model, @vendor, @price)";

            using (SqlConnection databaseConnection = new SqlConnection(conn))
            {
                databaseConnection.Open();
                using (SqlCommand insertCommand = new SqlCommand(insertSql, databaseConnection))
                {
                    insertCommand.Parameters.AddWithValue("@id", value.Id);
                    insertCommand.Parameters.AddWithValue("@model", value.Model);
                    insertCommand.Parameters.AddWithValue("@vendor", value.Vendor);
                    insertCommand.Parameters.AddWithValue("@price", value.Price);

                    //ExecuteNonQuery shows the number of rows affected
                    int rowsaffected = insertCommand.ExecuteNonQuery();
                    Console.WriteLine($"rows affected: {rowsaffected}");
                }


            }


            Car newcar = new Car() { Id = GetId(), Model = value.Model, Vendor = value.Vendor, Price = value.Price };
            carList.Add(newcar);
        }

        // PUT: api/Cars/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Car value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            carList.Remove(Get(id));
        }

       int GetId()
        {
            int max = carList.Max(x => x.Id);
            return max+1;
        }

    }
}
