using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ProdcutSite.Controllers;
using ProdcutSite.HtmlHelpers;
using ProdcutSite.Models;
using ProductSite.Domain.Abstract;
using ProductSite.Domain.Entities;


namespace ProductSite.UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Can_Paginate()
        {
            //Arrange 
            // - create the mock reposortry
            Mock<IProductsRepository> mock  = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1"},
                    new Product {ProductID = 2, Name = "P2"},
                    new Product {ProductID = 3, Name = "P3"},
                    new Product {ProductID = 4, Name = "P4"},
                    new Product {ProductID = 5, Name = "P5"}
                }.AsQueryable());

            // create a controller and make the page size 3 items        
            ProductController controller = new ProductController(mock.Object);        
            controller.PageSize = 3;

            // Action 
        
            IEnumerable<Product> result = (IEnumerable<Product>)controller.List(null ,2).Model;        
            // Assert        
            Product[] prodArray = result.ToArray();         
            Assert.IsTrue(prodArray.Length == 2);        
            Assert.AreEqual(prodArray[0].Name, "P4");         
            Assert.AreEqual(prodArray[1].Name, "P5"); 
        }

        [TestMethod] public void Can_Generate_Page_Links()
        {     
            // Arrange - define an HTML helper - we need to do this     
            // in order to apply the extension method    
            HtmlHelper myHelper = null;     
            // Arrange - create PagingInfo data     
            PagingInfo pagingInfo = new PagingInfo
                {
                    CurrentPages = 2 ,
                    TotalItems = 28, 
                    ItemPerPages = 10                            
                };  
            // Arrange - set up the delegate using a lambda expression     
            Func<int, string> pageUrlDelegate = i => "Page" + i;     
            // Act     
            MvcHtmlString result = myHelper.PageLinks(pagingInfo, pageUrlDelegate);     
            // Assert    
            Assert.AreEqual(result.ToString(), @"<a href=""Page1"">1</a><a class=""selected"" href=""Page2"">2</a><a href=""Page3"">3</a>"); 
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model()
        {
            // Arrange     
            // - create the mock repository    
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1"},
                    new Product {ProductID = 2, Name = "P2"},
                    new Product {ProductID = 3, Name = "P3"},
                    new Product {ProductID = 4, Name = "P4"},
                    new Product {ProductID = 5, Name = "P5"}
                }.AsQueryable());
            // Arrange - create a controller and make the page size 3 items    
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;
            // Action    
            ProductsListViewModel result = (ProductsListViewModel) controller.List(null ,2).Model;
            // Assert     
            PagingInfo pageInfo = result.PagingInfo;
            Assert.AreEqual(pageInfo.CurrentPages, 2);
            Assert.AreEqual(pageInfo.ItemPerPages, 3);
            Assert.AreEqual(pageInfo.TotalItems, 5);
            Assert.AreEqual(pageInfo.TotalPages, 2);
        }

        [TestMethod]
        public void Can_Filter_Products()
        {
            // Arrange     
            // - create the mock repository     

            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1", Catagory = "Cat1"},
                    new Product {ProductID = 2, Name = "P2", Catagory = "Cat2"},
                    new Product {ProductID = 3, Name = "P3", Catagory = "Cat1"},
                    new Product {ProductID = 4, Name = "P4", Catagory = "Cat2"},
                    new Product
                        {
                            ProductID = 5,
                            Name = "P5",
                            Catagory = "Cat3"
                        }
                }.AsQueryable());
            // Arrange - create a controller and make the page size 3 items     
            ProductController controller = new ProductController(mock.Object);
            controller.PageSize = 3;
            // Action     
            Product[] result = ((ProductsListViewModel) controller.List("Cat2", 1).Model)
                .Products.ToArray();
            // Assert     
            Assert.AreEqual(result.Length, 2);
            Assert.IsTrue(result[0].Name == "P2" && result[0].Catagory == "Cat2");
            Assert.IsTrue(result[1].Name == "P4" && result[1].Catagory == "Cat2");
        }

        [TestMethod]
        public void Can_Create_Categories()
        {
            // Arrange     
            // - create the mock repository     
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products)
                .Returns(new Product[]
                        {
                            new Product {ProductID = 1, Name = "P1", Catagory = "Apples"},
                            new Product {ProductID = 2, Name = "P2", Catagory = "Apples"},
                            new Product {ProductID = 3, Name = "P3", Catagory = "Plums"},
                            new Product {ProductID = 4, Name = "P4", Catagory = "Oranges"},
                        }.AsQueryable());
            // Arrange - create the controller    
            NavController target = new NavController(mock.Object);
            // Act = get the set of categories      
            string[] results = ((IEnumerable<string>) target.Menu().Model).ToArray();
            // Assert     Assert.AreEqual(results.Length, 3); 
            Assert.AreEqual(results[0], "Apples");
            Assert.AreEqual(results[1], "Oranges");
            Assert.AreEqual(results[2], "Plums");
        }


        [TestMethod]
        public void Indicates_Selected_Category()
        {
            // Arrange     
            // - create the mock repository     
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1", Catagory = "Apples"},
                    new Product {ProductID = 4, Name = "P2", Catagory = "Oranges"},

                }.AsQueryable());
            // Arrange - create the controller      
            NavController target = new NavController(mock.Object);
            // Arrange - define the category to selected     
            string categoryToSelect = "Apples";
            // Action     
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;
            // Assert     
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod] 
        public void Generate_Category_Specific_Product_Count() 
        {     
            // Arrange    
            // - create the mock repository    
            Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
            mock.Setup(m => m.Products).Returns(new Product[]
                {
                    new Product {ProductID = 1, Name = "P1", Catagory = "Cat1"}, 
                    new Product {ProductID = 2, Name = "P2", Catagory = "Cat2"},         
                    new Product {ProductID = 3, Name = "P3", Catagory = "Cat1"},         
                    new Product {ProductID = 4, Name = "P4", Catagory = "Cat2"},         
                    new Product {ProductID = 5, Name = "P5", Catagory = "Cat3"}
                }.AsQueryable()); 
              // Arrange - create a controller and make the page size 3 items    
            ProductController target = new ProductController(mock.Object);    
            target.PageSize = 3;    
            // Action - test the product counts for different categories     
            int res1 = ((ProductsListViewModel)target.List("Cat1").Model).PagingInfo.TotalItems;     
            int res2 = ((ProductsListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems;     
            int res3 = ((ProductsListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems;     
            int resAll = ((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems;     
            // Assert    
            Assert.AreEqual(res1, 2);    
            Assert.AreEqual(res2, 2);    
            Assert.AreEqual(res3, 1);    
            Assert.AreEqual(resAll, 5); 
        }

        [TestMethod]
        public void Can_Add_New_Lines()
        {
            // Arrange - create some test products
            Product p1 = new Product {ProductID = 1, Name = "P1"};
            Product p2 = new Product { ProductID = 2, Name = "P2" };    
            // Arrange - create a new cart     
            Cart target = new Cart(); 
             // Act     
            target.AddItem(p1, 1);     
            target.AddItem(p2, 1);     
            CartLine[] results = target.Lines.ToArray();     
            // Assert     
            Assert.AreEqual(results.Length, 2);    
            Assert.AreEqual(results[0].Product, p1);     
            Assert.AreEqual(results[1].Product, p2);    
        }

        [TestMethod]
        public void Can_Remove_Line()
        {
            // Arrange - create some test products    
            Product p1 = new Product { ProductID = 1, Name = "P1" };
            Product p2 = new Product { ProductID = 2, Name = "P2" }; 
            Product p3 = new Product { ProductID = 3, Name = "P3" }; 
            // Arrange - create a new cart    
            Cart target = new Cart();    
            // Arrange - add some products to the cart   
            target.AddItem(p1, 1);    
            target.AddItem(p2, 3);    
            target.AddItem(p3, 5); 
            target.AddItem(p2, 1);    
            // Act    
            target.RemoveLine(p2);   
            // Assert    
            Assert.AreEqual(target.Lines.Count(c => c.Product == p2), 0); 
            Assert.AreEqual(target.Lines.Count(), 2); 
        }

        [TestMethod]
        public void Can_Clear_Contents()
        {
            // Arrange - create some test products
            Product p1 = new Product { ProductID = 1, Name = "P1", Price = 100M };
            Product p2 = new Product { ProductID = 2, Name = "P2", Price = 50M }; 
            // Arrange - create a new cart     
            Cart target = new Cart();    
            // Arrange - add some items    
            target.AddItem(p1, 1);  
            target.AddItem(p2, 1);   
            // Act - reset the cart    
            target.Clear();     
            // Assert     
            Assert.AreEqual(target.Lines.Count(), 0); 
        }


    }
}
