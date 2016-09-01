using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using RedisExercises.WebApi.Models;
using StackExchange.Redis;
using System.Configuration;
using Newtonsoft.Json;

namespace RedisExercises.WebApi.Controllers
{
    public class ProductsController : ApiController
    {
        private RedisExercisesWebApiContext db = new RedisExercisesWebApiContext();

        IDatabase cache = Connection.GetDatabase();

        // GET: api/Products
        public IHttpActionResult GetProducts()
        {

            // App Usage
            var itemsKey = "stojancho-products";

            string itemsValue = cache.StringGet(itemsKey);
            if (itemsValue == null)
            {
                // Get From Persistent storage and store in Cache.
                itemsValue = GetFromPersistentStorage(itemsKey);
                cache.StringSet(itemsKey, itemsValue);
            }
            return Ok(itemsValue);
        }

        // GET: api/Products/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult GetProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // PUT: api/Products/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutProduct(int id, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != product.ID)
            {
                return BadRequest();
            }

            db.Entry(product).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Products
        [ResponseType(typeof(Product))]
        public IHttpActionResult PostProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Products.Add(product);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = product.ID }, product);
        }

        // DELETE: api/Products/5
        [ResponseType(typeof(Product))]
        public IHttpActionResult DeleteProduct(int id)
        {
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.ID == id) > 0;
        }

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["RedisCacheName"] + ",abortConnect=false,ssl=true,password=" + ConfigurationManager.AppSettings["RedisCachePassword"]);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        private string GetFromPersistentStorage(string itemKey)
        {
            var result = db.Products;
            return JsonConvert.SerializeObject(result);
        }
    }
}