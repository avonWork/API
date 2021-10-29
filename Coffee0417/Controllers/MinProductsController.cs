using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using Coffee0417.Models;
using Coffee0417.Utils;
using static Coffee0417.Utils.Enum;
using static Coffee0417.Utils.Myall;

namespace Coffee0417.Controllers
{
    [EnableCors("*", "*", "*")]
    public class MinProductsController : ApiController
    {
        private Model1 db = new Model1();

        // GET: api/MinProducts/Get
        //得到所有產品
        //暫時用不到
        public IHttpActionResult Get()
        {
            return Ok(db.Products);
        }

        // GET: api/MinProducts/GetItem
        //得到全部產品清單列表
        //ok
        public IHttpActionResult GetItem()
        {
            var item = db.Products.Select(x => new
            {
                x.Id,
                x.ProductName,
                x.ProductImg,
                x.ProducPrice
            }).ToList();
            var count = item.Count();
            return Ok(new
            {
                check = "ok",
                message = message.取得產品列表,
                item,
                Counts = count
            });
        }

        // GET: api/MinProducts/GetItemPage
        //api/MinProducts/GetItemPage?pageCnt=2&pageRows=5
        //分頁功能---得到全部產品清單列表(頁碼pageCnt,筆數pageRows )
        //ok
        public IHttpActionResult GetItemPage(int pageCnt, int pageRows)
        {
            var item = db.Products.Select(x => new
            {
                x.Id,
                x.ProductName,
                x.ProductImg,
                x.ProducPrice
            }).ToList().Skip((pageCnt - 1) * pageRows).Take(pageRows);
            var count = db.Products.Count();
            return Ok(new
            {
                check = "ok",
                message = message.取得產品列表,
                item,
                Counts = count
            });
        }

        // GET: api/ MinProducts/GetIndex
        //P1首頁---六張隨機推薦商品圖
        //ok
        /// <summary>
        ///隨機產生推薦商品前六條
        /// </summary>
        /// <returns></returns>
        public IHttpActionResult GetIndex()
        {
            //隨機產生推薦商品前六條
            var recommend = db.Products.OrderBy(_ => Guid.NewGuid()).Take(6).Select(x => new
            {
                x.Id,
                x.ProductImg,
                x.ProductName,
                x.ProducPrice
            }).ToList();
            return Ok(new
            {
                check = "ok",
                message = message.取得產品列表,
                recommend
            });
        }

        // GET: api/MinProducts/Get/25
        //透過id得到一筆產品內容
        //ok
        public IHttpActionResult Get(int id)
        {
            MinProduct pro = db.Products.FirstOrDefault(x => x.Id == id);
            if (pro == null)
            {
                return Ok(new
                {
                    check = "no",
                    message = message.產品id錯誤
                });
            }
            return Ok(new
            {
                check = "ok",
                message = message.取得產品內容,
                pro.Id,
                pro.ProductImg,
                pro.ProductName,
                pro.ProducPrice,
                pro.ProductDescription,
                pro.ProductContent
            });
        }

        // GET: api/MinProducts/SearchPro
        //api/MinProducts/SearchPro?name=巴拿馬
        //ok
        //透過產品名稱搜尋商品
        [HttpGet]
        public IHttpActionResult SearchPro(string name)
        {
            var prodatalist = db.Products.Where(x => x.ProductName.Contains(name)).Select(z => new
            {
                z.Id,
                z.ProductImg,
                z.ProductName,
                z.ProducPrice
            }).ToList();
            var count = prodatalist.Count();
            if ((prodatalist != null) && (!prodatalist.Any()))
            {
                return Ok(new
                {
                    check = "no",
                    message = message.產品搜尋無資料
                });
            }
            return Ok(new
            {
                check = "ok",
                message = message.取得產品列表,
                prodatalist,
                Counts = count
            });
        }

        // GET: api/MinProducts/SearchProPage
        //api/MinProducts/SearchProPage?name=巴拿馬&pageCnt=2&pageRows=5
        //ok
        //分頁功能---透過產品名稱搜尋商品(頁碼pageCnt,筆數pageRows )
        [HttpGet]
        public IHttpActionResult SearchProPage(string name, int pageCnt, int pageRows)
        {
            var prodatalist = db.Products.Where(x => x.ProductName.Contains(name)).Select(z => new
            {
                z.Id,
                z.ProductImg,
                z.ProductName,
                z.ProducPrice
            }).ToList().Skip((pageCnt - 1) * pageRows).Take(pageRows);
            var count = db.Products.Where(x => x.ProductName.Contains(name)).Count();
            if ((prodatalist != null) && (!prodatalist.Any()))
            {
                return Ok(new
                {
                    check = "no",
                    message = message.產品搜尋無資料
                });
            }
            return Ok(new
            {
                check = "ok",
                message = message.取得產品列表,
                prodatalist,
                Counts = count
            });
        }

        // GET: api/MinProducts/MenuClass
        //MinProducts/MenuClass?classid=2001
        //菜單產品分類-分類編號
        [HttpGet]
        public IHttpActionResult MenuClass(string classid)
        {
            var orderProducts = db.Products.Where(x => x.Productlabel == classid).Select(z => new
            {
                z.Id,
                z.ProductImg,
                z.ProductName,
                z.ProducPrice,
                z.ProductClass
            }).ToList();
            var count = orderProducts.Count();
            return Ok(new
            {
                check = "ok",
                message = message.取得產品列表,
                orderProducts,
                Counts = count
            });
        }

        // GET: api/MinProducts/MenuClassPage
        //api/MinProducts/MenuClassPage?classid=2001&pageCnt=2&pageRows=5
        //ok
        //分頁功能---菜單產品分類-分類編號(頁碼pageCnt,筆數pageRows )
        [HttpGet]
        public IHttpActionResult MenuClassPage(string classid, int pageCnt, int pageRows)
        {
            var orderProducts = db.Products.Where(x => x.Productlabel == classid).Select(z => new
            {
                z.Id,
                z.ProductImg,
                z.ProductName,
                z.ProducPrice,
                z.ProductClass
            }).ToList().Skip((pageCnt - 1) * pageRows).Take(pageRows);
            var count = db.Products.Where(x => x.Productlabel == classid).Count();
            return Ok(new
            {
                check = "ok",
                message = message.取得產品列表,
                orderProducts,
                Counts = count
            });
        }

        // GET: api/MinProducts/MenuClassLow
        //https://localhost:44304/api/MinProducts/MenuClassLow?classid=1001
        //菜單產品分類(價格排序低到高)--分類編號
        [HttpGet]
        public IHttpActionResult MenuClassLow(string classid)
        {
            var orderProducts = db.Products.Where(x => x.Productlabel == classid).OrderBy(y => y.ProducPrice).Select(z => new
            {
                z.Id,
                z.ProductImg,
                z.ProductName,
                z.ProducPrice,
                z.ProductClass
            }).ToList();
            var count = orderProducts.Count();
            return Ok(new
            {
                check = "ok",
                message = message.取得產品列表,
                orderProducts,
                Counts = count
            });
        }

        // GET: api/MinProducts/MenuClassLowPage
        //api/MinProducts/MenuClassLowPage?classid=1001&pageCnt=2&pageRows=5
        //分頁功能---菜單產品分類(價格排序低到高)--分類編號(頁碼pageCnt,筆數pageRows )
        [HttpGet]
        public IHttpActionResult MenuClassLowPage(string classid, int pageCnt, int pageRows)
        {
            var orderProducts = db.Products.Where(x => x.Productlabel == classid).OrderBy(y => y.ProducPrice).Select(z => new
            {
                z.Id,
                z.ProductImg,
                z.ProductName,
                z.ProducPrice,
                z.ProductClass
            }).ToList().Skip((pageCnt - 1) * pageRows).Take(pageRows);
            var count = db.Products.Where(x => x.Productlabel == classid).Count();
            return Ok(new
            {
                check = "ok",
                message = message.取得產品列表,
                orderProducts,
                Counts = count
            });
        }

        // GET: api/MinProducts/MenuClassHigh
        //https://localhost:44304/api/MinProducts/MenuClassHigh?classid=1001
        //菜單產品分類(價格排序高到低)--分類編號
        [HttpGet]
        public IHttpActionResult MenuClassHigh(string classid)
        {
            var orderProducts = db.Products.Where(x => x.Productlabel == classid).OrderByDescending(y => y.ProducPrice).Select(z => new
            {
                z.Id,
                z.ProductImg,
                z.ProductName,
                z.ProducPrice,
                z.ProductClass
            }).ToList();
            var count = orderProducts.Count();
            return Ok(new
            {
                check = "ok",
                message = message.取得產品列表,
                orderProducts,
                Counts = count
            });
        }

        // GET: api/MinProducts/MenuClassHighPage
        //api/MinProducts/MenuClassHighPage?classid=1001&pageCnt=2&pageRows=5
        //分頁功能---菜單產品分類(價格排序高到低)--分類編號(頁碼pageCnt,筆數pageRows )
        [HttpGet]
        public IHttpActionResult MenuClassHighPage(string classid, int pageCnt, int pageRows)
        {
            var orderProducts = db.Products.Where(x => x.Productlabel == classid).OrderByDescending(y => y.ProducPrice).Select(z => new
            {
                z.Id,
                z.ProductImg,
                z.ProductName,
                z.ProducPrice,
                z.ProductClass
            }).ToList().Skip((pageCnt - 1) * pageRows).Take(pageRows);
            var count = db.Products.Where(x => x.Productlabel == classid).Count();
            return Ok(new
            {
                check = "ok",
                message = message.取得產品列表,
                orderProducts,
                Counts = count
            });
        }

        // GET: api/MinProducts/Low
        //菜單分類渲染產品(排序低到高)--名稱
        //暫時用不到
        [HttpGet]
        public IHttpActionResult Low(string name)
        {
            var orderProducts = db.Products.Where(x => x.ProductClass.Contains(name)).OrderBy(y => y.ProducPrice).Select(z => new
            {
                z.Id,
                z.ProductImg,
                z.ProductName,
                z.ProducPrice,
                z.ProductClass
            }).ToList();
            return Ok(orderProducts);
        }

        // GET: api/MinProducts/High
        //菜單分類渲染產品(排序高到低)--名稱
        //暫時用不到
        [HttpGet]
        public IHttpActionResult High(string name)
        {
            var orderProducts = db.Products.Where(x => x.ProductClass.Contains(name)).OrderByDescending(y => y.ProducPrice).Select(z => new
            {
                z.Id,
                z.ProductImg,
                z.ProductName,
                z.ProducPrice,
                z.ProductClass
            }).ToList();
            return Ok(orderProducts);
        }

        // PUT: api/MinProducts/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMinProduct(int id, MinProduct minProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != minProduct.Id)
            {
                return BadRequest();
            }

            db.Entry(minProduct).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MinProductExists(id))
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

        // POST: api/MinProducts
        [ResponseType(typeof(MinProduct))]
        public IHttpActionResult PostMinProduct(MinProduct minProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Products.Add(minProduct);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = minProduct.Id }, minProduct);
        }

        // DELETE: api/MinProducts/5
        [ResponseType(typeof(MinProduct))]
        public IHttpActionResult DeleteMinProduct(int id)
        {
            MinProduct minProduct = db.Products.Find(id);
            if (minProduct == null)
            {
                return NotFound();
            }

            db.Products.Remove(minProduct);
            db.SaveChanges();

            return Ok(minProduct);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MinProductExists(int id)
        {
            return db.Products.Count(e => e.Id == id) > 0;
        }
    }
}