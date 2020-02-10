using InMemoryDatabase.Controllers;
using InMemoryDatabase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace InMemoryDatabase.Tests
{
    [TestClass]
    public class LibraryControllerTests
    {
        private LibraryController _controller;
        private ILibraryRepository _repository;

        private void Populate(LibraryContext context)
        {
            var bronte = new Author { Name = "Charlotte Bronte"};

            var eyre = new Book { Title = "Jane Eyre", Author = bronte, Year = 1847 };

            context.Add(eyre);

            context.SaveChanges();
        }

        private ILibraryRepository GetInMemoryRepository()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                             .UseInMemoryDatabase(databaseName: "MockDB")
                             .Options;

            var initContext = new LibraryContext(options);

            initContext.Database.EnsureDeleted();

            Populate(initContext);

            var testContext = new LibraryContext(options);

            var repository = new DbRepository(testContext);

            return repository;
        }

        [TestInitialize]
        public void Setup()
        {
            _repository = GetInMemoryRepository(); 
            _controller = new LibraryController(_repository);
        }

        [TestMethod]
        public void IndexBooks()
        {
            var result = _controller.Index() as ViewResult;
            var model = result.Model as IEnumerable<Book>;

            Assert.AreEqual(1, model.Count());
            Assert.AreEqual("Jane Eyre", model.First().Title);
        }

        [TestMethod]
        public void IndexAuthors()
        {
            var result = _controller.Index() as ViewResult;
            var model = result.Model as IEnumerable<Book>;

            Assert.AreEqual("Charlotte Bronte", model.First().Author.Name);
        }
    }
}
