using bookDemo.Data;
using bookDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace bookDemo.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BooksController : ControllerBase
	{
		[HttpGet]
		public IActionResult GetAllBooks()
		{
			var books = ApplicationContext.Books;
			return Ok(books);
		}

		[HttpGet("{id:int}")]
		public IActionResult GetOneBook(int id)
		{
			var book = ApplicationContext.Books.Where(p => p.Id == id).FirstOrDefault();
			if(book is null)
				return NotFound();	

			return Ok(book);
		}

		[HttpPost]
		public IActionResult CreateOneBook([FromBody] Book book)
		{
			try
			{
				if (book is null)
					return BadRequest();

				ApplicationContext.Books.Add(book);
				return StatusCode(201, book);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpPut("{id:int}")]
		public IActionResult UpdateOneBook([FromRoute(Name ="id")]int id,
			[FromBody]Book book)
		{
			//check book?
			var entity = ApplicationContext.Books.Find(p => p.Id == id);
			if (entity is null)
				return NotFound();

			if(id != book.Id)
				return BadRequest();

			ApplicationContext.Books.Remove(entity);
			book.Id = entity.Id;
			ApplicationContext.Books.Add(book);
			return Ok(book);
		}


		[HttpDelete]
		public IActionResult DeleteAllBooks()
		{
			ApplicationContext.Books.Clear();
			return NoContent(); // 204
		}

		[HttpDelete("{id:int}")]
		public IActionResult DeleteOneBook([FromRoute(Name ="id")] int id)
		{
			var entity = ApplicationContext.Books.Find(p => p.Id == id);

			if(entity is null)
				return NotFound(new
				{
					statusCode = 404,
					message = $"Book with id:{id} could not found."
				}); // 404

			ApplicationContext.Books.Remove(entity);
			return NoContent();
		}


		[HttpPatch("{id:int}")]
		public IActionResult PartiallyUpdateOneBook([FromRoute(Name ="id")]int id,
			[FromBody] JsonPatchDocument<Book> bookPatch)
		{
			var entity = ApplicationContext.Books.Find(p => p.Id == id);
			if (entity is null)
				return NotFound();// 404

			bookPatch.ApplyTo(entity);
			return NoContent();// 204
		}
	}
}
