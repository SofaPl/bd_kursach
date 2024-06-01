using Back.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;


        [Route("api/[controller]")]
        [ApiController]

            public class ItemsController : ControllerBase
            {
                private readonly string _connectionString;

                public ItemsController(IConfiguration configuration)
                {
                    _connectionString = configuration.GetConnectionString("DefaultConnection");
                }

                // GET: api/Items
                [HttpGet]
                public async Task<ActionResult<IEnumerable<ItemCard>>> GetItems()
                {
                    var items = new List<ItemCard>();

                    using (var conn = new NpgsqlConnection(_connectionString))
                    {
                        await conn.OpenAsync();

                        using (var cmd = new NpgsqlCommand("SELECT * FROM item_card", conn))
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                items.Add(new ItemCard
                                {
                                    ProductId = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Price = reader.GetDecimal(2),
                                    Description = reader.GetString(3),
                                    Category = reader.GetString(4),
                                    ImageUrl = reader.GetString(5)
                                });
                            }
                        }
                    }

                    return items;
                }

                // GET: api/Items/5
                [HttpGet("{productId}")]
                public async Task<ActionResult<ItemCard>> GetItem(int productId)
                {
                    ItemCard item = null;

                    using (var conn = new NpgsqlConnection(_connectionString))
                    {
                        await conn.OpenAsync();

                        using (var cmd = new NpgsqlCommand("SELECT * FROM item_card WHERE product_id = @productId", conn))
                        {
                            cmd.Parameters.AddWithValue("productId", productId);

                            using (var reader = await cmd.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    item = new ItemCard
                                    {
                                        ProductId = reader.GetInt32(0),
                                        Title = reader.GetString(1),
                                        Price = reader.GetDecimal(2),
                                        Description = reader.GetString(3),
                                        Category = reader.GetString(4),
                                        ImageUrl = reader.GetString(5)
                                    };
                                }
                            }
                        }
                    }



            if (item == null)
                    {
                        return NotFound();
                    }

                    return item;
                }

                // POST: api/Items
                [HttpPost]
                public async Task<ActionResult<ItemCard>> PostItem(ItemCard item)
                {
                    using (var conn = new NpgsqlConnection(_connectionString))
                    {
                        await conn.OpenAsync();

                        using (var cmd = new NpgsqlCommand("INSERT INTO item_card (title, price, description, category, image_url) VALUES (@title, @price, @description, @category, @imageUrl) RETURNING product_id", conn))
                        {
                            cmd.Parameters.AddWithValue("title", item.Title);
                            cmd.Parameters.AddWithValue("price", item.Price);
                            cmd.Parameters.AddWithValue("description", item.Description);
                            cmd.Parameters.AddWithValue("category", item.Category);
                            cmd.Parameters.AddWithValue("imageUrl", item.ImageUrl);

                            item.ProductId = (int)await cmd.ExecuteScalarAsync();
                        }
                    }

                    return CreatedAtAction(nameof(GetItem), new { productId = item.ProductId }, item);
                }

    // GET: api/Clients/5
    [HttpGet("{userId}")]
    public async Task<ActionResult<Client>> GetUser(int userId)
    {
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            using (var cmd = new NpgsqlCommand("SELECT * FROM client WHERE user_id = @userId", conn))
            {
                cmd.Parameters.AddWithValue("userId", userId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var client = new Client
                        {
                            UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
                            Username = reader.GetString(reader.GetOrdinal("username")),
                            Email = reader.GetString(reader.GetOrdinal("email")),
                            Password = reader.GetString(reader.GetOrdinal("password")),
                            ShoppingCart = reader.IsDBNull(reader.GetOrdinal("shoppingcart")) ? new List<int>() : reader.GetFieldValue<int[]>(reader.GetOrdinal("shoppingcart")).ToList()
                        };
                        return client;
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
        }
    }
    // POST: api/Clients/register
    [HttpPost("register")]
    public async Task<ActionResult<Client>> RegisterUser(Client newUser)
    {
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            using (var cmd = new NpgsqlCommand("INSERT INTO client (username, email, password, shoppingcart) VALUES (@username, @email, @password, @shoppingcart) RETURNING user_id", conn))
            {
                cmd.Parameters.AddWithValue("username", newUser.Username);
                cmd.Parameters.AddWithValue("email", newUser.Email);
                cmd.Parameters.AddWithValue("password", newUser.Password);
                cmd.Parameters.AddWithValue("shoppingcart", new int[0]); // New users start with an empty shopping cart

                newUser.UserId = (int)await cmd.ExecuteScalarAsync();
            }
        }

        return CreatedAtAction(nameof(GetUser), new { userId = newUser.UserId }, newUser);
    }
    // POST: api/Clients/login
    [HttpPost("login")]
    public async Task<ActionResult<Client>> Login(Client loginUser)
    {
        Client user = null;

        using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            using (var cmd = new NpgsqlCommand("SELECT * FROM client WHERE username = @username AND email = @email AND password = @password", conn))
            {
                cmd.Parameters.AddWithValue("username", loginUser.Username);
                cmd.Parameters.AddWithValue("email", loginUser.Email);
                cmd.Parameters.AddWithValue("password", loginUser.Password);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        user = new Client
                        {
                            UserId = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            Email = reader.GetString(2),
                            Password = reader.GetString(3),
                            ShoppingCart = reader.GetFieldValue<int[]>(4).ToList()
                        };
                    }
                }
            }
        }

        if (user == null)
        {
            return Unauthorized();
        }

        return user;
    }
    [HttpGet("user/{userId}/shoppingcart")]
    public async Task<ActionResult<IEnumerable<ItemCard>>> GetShoppingCart(int userId)
    {
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            // Получаем пользователя по userId
            var user = await GetUser(userId);
            if (user.Value == null)
            {
                return NotFound();
            }

            // Получаем товары по ID из корзины пользователя
            var productIds = user.Value.ShoppingCart;
            var products = new List<ItemCard>();

            foreach (var productId in productIds)
            {
                using (var cmd = new NpgsqlCommand("SELECT * FROM item_card WHERE product_id = @productId", conn))
                {
                    cmd.Parameters.AddWithValue("productId", productId);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var product = new ItemCard
                            {
                                ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                                Title = reader.GetString(reader.GetOrdinal("title")),
                                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                                Description = reader.GetString(reader.GetOrdinal("description")),
                                Category = reader.GetString(reader.GetOrdinal("category")),
                                ImageUrl = reader.GetString(reader.GetOrdinal("image_url"))
                            };
                            products.Add(product);
                        }
                    }
                }
            }

            return products;
        }
    }

    // POST: api/Clients/{userId}/shoppingcart
    [HttpPost("user/{userId}/shoppingcart")]
    public async Task<IActionResult> AddToShoppingCart(int userId, [FromBody] ProductIdRequest request)
    {
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            // Получаем пользователя по userId
            var user = await GetUser(userId);
            if (user.Value == null)
            {
                return NotFound();
            }

            // Получаем текущий массив shoppingcart и добавляем новый productId
            var shoppingCart = user.Value.ShoppingCart.ToList();
            shoppingCart.Add(request.ProductId);

            // Обновляем поле shoppingcart в базе данных
            using (var cmd = new NpgsqlCommand("UPDATE client SET shoppingcart = @shoppingcart WHERE user_id = @userId", conn))
            {
                cmd.Parameters.AddWithValue("shoppingcart", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer, shoppingCart.ToArray());
                cmd.Parameters.AddWithValue("userId", userId);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        return NoContent();
    }

    public class ProductIdRequest
    {
        public int ProductId { get; set; }
    }
    // POST: api/Clients/savecard
    [HttpPost("savecard")]
    public async Task<ActionResult<BankCard>> SaveBankCard(BankCard newCard)
    {
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            using (var cmd = new NpgsqlCommand("INSERT INTO bank_card (user_id, card_number, cardholder_name, expiry_date, cvv) VALUES (@userId, @cardNumber, @cardholderName, @expiryDate, @cvv) RETURNING user_id", conn))
            {
                cmd.Parameters.AddWithValue("userId", newCard.UserId);
                cmd.Parameters.AddWithValue("cardNumber", newCard.CardNumber);
                cmd.Parameters.AddWithValue("cardholderName", newCard.CardholderName);
                cmd.Parameters.AddWithValue("expiryDate", newCard.ExpiryDate);
                cmd.Parameters.AddWithValue("cvv", newCard.CVV);

                newCard.BankCardId = (int)await cmd.ExecuteScalarAsync();
            }
        }

        return CreatedAtAction(nameof(GetUser), new { userId = newCard.UserId }, newCard);
    }
}
