
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Othelloworld.Data.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Othelloworld.Data
{
	public class OthelloDbContext : IdentityDbContext<
		Account,
		AccountRole, 
		string
	>	{
		//private DbSet<Account> Accounts { get; set; }
		public DbSet<Player> Players { get; set; }
		public DbSet<Game> Games { get; set; }
		private DbSet<PlayerInGame> PlayersInGame { get; set; }
		private DbSet<Turn> Turns { get; set; }

		public DbSet<Country> Countries { get; set; }

		public OthelloDbContext(
			DbContextOptions<OthelloDbContext> options) : base(options)
		{ }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			CountrySeeder.Add(builder);

			// Count of Users and Games
			var USER_COUNT = 100;
			var GAME_COUNT = 50;

			// Unique Ids for 20 users
			var USER_IDS = Enumerable.Range(0, USER_COUNT)
				.Select(index => Guid.NewGuid().ToString())
				.ToList();

			// Unique Ids for 10 games
			var GAME_TOKENS = Enumerable.Range(0, GAME_COUNT)
				.Select(index => Guid.NewGuid().ToString())
				.ToList();

			// any unique string id
			var USER_ROLE_ID = Guid.NewGuid().ToString();
			var MODERATOR_ROLE_ID = Guid.NewGuid().ToString();
			var ADMIN_ROLE_ID = Guid.NewGuid().ToString();

			// users
			var ADMIN_ID = Guid.NewGuid().ToString();

			// Create Roles
			builder.Entity<AccountRole>().HasData(new AccountRole[]
			{
				new AccountRole
				{
					Id = USER_ROLE_ID,
					Name = "user",
					NormalizedName = "user"
				},
				new AccountRole
				{
					Id = MODERATOR_ROLE_ID,
					Name = "mod",
					NormalizedName = "mod"
				},
				new AccountRole
				{
					Id = ADMIN_ROLE_ID,
					Name = "admin",
					NormalizedName = "admin"
				}
			});

			// Add 20 users
			var hasher = new PasswordHasher<Account>();

			builder.Entity<Account>().HasData(new Account
			{
				Id = ADMIN_ID,
				UserName = "admin",
				NormalizedUserName = "admin",
				Email = "admin@gmail.com",
				NormalizedEmail = "admin@gmail.com",
				EmailConfirmed = true,
				PasswordHash = hasher.HashPassword(null, "Administrator123!"),
				SecurityStamp = Guid.NewGuid().ToString("D")
			});

			builder.Entity<Account>().HasData(USER_IDS.Select((guid, index) => new Account
			{
				Id = guid,
				UserName = $"hello{index}",
				NormalizedUserName = $"hello{index}",
				Email = $"hello{index}@gmail.com",
				NormalizedEmail = $"hello{index}@gmail.com",
				EmailConfirmed = true,
				PasswordHash = hasher.HashPassword(null, $"HelloWorld{index}!"),
				SecurityStamp = Guid.NewGuid().ToString("D")
			}));

			builder.Entity<IdentityUserRole<string>>().HasData(USER_IDS.Select(guid => new IdentityUserRole<string>
			{
				RoleId = USER_ROLE_ID,
				UserId = guid,
			}));

			// Add roles to users
			builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
			{
				RoleId = ADMIN_ROLE_ID,
				UserId = ADMIN_ID,
			});

			// Add players to all users
			var random = new Random();
			builder.Entity<Player>().HasData(new Player
			{
				Username = "admin",
				AmountWon = random.Next(6),
				AmountLost = random.Next(6),
				AmountDraw = random.Next(3),
				CountryCode = "NL"
			});

			builder.Entity<Player>().HasData(USER_IDS.Select((_, index) => new Player
			{
				Username = $"hello{index}",
				AmountWon = random.Next(6),
				AmountLost = random.Next(6),
				AmountDraw = random.Next(3),
				CountryCode = "NL"
			}));

			// Add all games
			builder.Entity<Game>().HasData(GAME_TOKENS.Select((guid, index) => new Game
			{
				Token = guid,
				Name = $"Game {index}",
				Description = $"A description {index}",
				Status = GameStatus.Staging,
				PlayerTurn = Color.black,
				Board = new IEnumerable<Color>[]
					{
						new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>(),
						new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>(),
						new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>(),
						new int[8]{0, 0, 0, 1, 2, 0, 0, 0}.Cast<Color>(),
						new int[8]{0, 0, 0, 2, 1, 0, 0, 0}.Cast<Color>(),
						new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>(),
						new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>(),
						new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>()
					}
			}));

			// Add almost done admin game
			var ADMIN_GAME = Guid.NewGuid().ToString();

			builder.Entity<Game>().HasData(new Game
			{
				Token = ADMIN_GAME,
				Name = "Game",
				Description = "A description",
				Status = GameStatus.Playing,
				PlayerTurn = Color.white,
				Board = new IEnumerable<Color>[]
					{
						new int[8]{1, 1, 1, 1, 1, 1, 1, 1}.Cast<Color>(),
						new int[8]{1, 1, 1, 1, 1, 1, 1, 1}.Cast<Color>(),
						new int[8]{1, 1, 1, 1, 0, 1, 1, 1}.Cast<Color>(),
						new int[8]{1, 1, 1, 1, 2, 1, 1, 1}.Cast<Color>(),
						new int[8]{1, 1, 1, 2, 1, 1, 1, 1}.Cast<Color>(),
						new int[8]{1, 1, 1, 1, 1, 1, 1, 1}.Cast<Color>(),
						new int[8]{1, 1, 1, 1, 1, 1, 1, 1}.Cast<Color>(),
						new int[8]{1, 1, 1, 1, 1, 1, 1, 1}.Cast<Color>()
					},
				Turns = new List<Turn>()
			});

			// Add Admin and hello50 to admin game
			builder.Entity<PlayerInGame>().HasData(new PlayerInGame
			{
				Username = "admin",
				Color = Color.white,
				GameToken = ADMIN_GAME,
				IsHost = true,
				Result = GameResult.undecided,
				ConfirmResults = false
			});

			builder.Entity<PlayerInGame>().HasData(new PlayerInGame
			{
				Username = "hello50",
				Color = Color.black,
				GameToken = ADMIN_GAME,
				IsHost = true,
				Result = GameResult.undecided,
				ConfirmResults = false
			});

			// Add 1 player to each game
			builder.Entity<PlayerInGame>().HasData(GAME_TOKENS.Select((guid, index) => new PlayerInGame
			{
				Username = $"hello{index}",
				Color = Color.white,
				GameToken = guid,
				IsHost = true,
				Result = GameResult.undecided,
				ConfirmResults = false
			}));

			//builder.Entity<Account>().HasData(new Account[] {
			//	new Account
			//	{
			//		Id = ADMIN_ID,
			//		UserName = "admin",
			//		NormalizedUserName = "admin",
			//		Email = "admin@gmail.com",
			//		NormalizedEmail = "admin@gmail.com",
			//		EmailConfirmed = true,
			//		PasswordHash = hasher.HashPassword(null, "Administrator123!"),
			//		SecurityStamp = String.Empty
			//	},
			//	new Account
			//	{
			//		Id = MOD_1_ID,
			//		UserName = "mod",
			//		NormalizedUserName = "mod",
			//		Email = "mod@gmail.com",
			//		NormalizedEmail = "mod@gmail.com",
			//		EmailConfirmed = true,
			//		PasswordHash = hasher.HashPassword(null, "Moderator123!"),
			//		SecurityStamp = String.Empty
			//	},
			//	new Account
			//	{
			//		Id = USER_1_ID,
			//		UserName = "hello1",
			//		NormalizedUserName = "hello1",
			//		Email = "hello1@gmail.com",
			//		NormalizedEmail = "hello1@gmail.com",
			//		EmailConfirmed = true,
			//		PasswordHash = hasher.HashPassword(null, "HelloWorld1!"),
			//		SecurityStamp = String.Empty
			//	},
			//	new Account
			//	{
			//		Id = USER_2_ID,
			//		UserName = "hello2",
			//		NormalizedUserName = "hello2",
			//		Email = "hello2@gmail.com",
			//		NormalizedEmail = "hello2@gmail.com",
			//		EmailConfirmed = true,
			//		PasswordHash = hasher.HashPassword(null, "HelloWorld2!"),
			//		SecurityStamp = String.Empty
			//	},
			//	new Account
			//	{
			//		Id = USER_3_ID,
			//		UserName = "hello3",
			//		NormalizedUserName = "hello3",
			//		Email = "hello3@gmail.com",
			//		NormalizedEmail = "hello3@gmail.com",
			//		EmailConfirmed = true,
			//		PasswordHash = hasher.HashPassword(null, "HelloWorld3!"),
			//		SecurityStamp = String.Empty
			//	},
			//	new Account
			//	{
			//		Id = USER_4_ID,
			//		UserName = "hello4",
			//		NormalizedUserName = "hello4",
			//		Email = "hello4@gmail.com",
			//		NormalizedEmail = "hello4@gmail.com",
			//		EmailConfirmed = true,
			//		PasswordHash = hasher.HashPassword(null, "HelloWorld4!"),
			//		SecurityStamp = String.Empty
			//	}
			//});

			//builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>[]
			//{
			//	new IdentityUserRole<string>
			//	{
			//		RoleId = ADMIN_ROLE_ID,
			//		UserId = ADMIN_ID,
			//	},
			//	new IdentityUserRole<string>
			//	{
			//		RoleId = MODERATOR_ROLE_ID,
			//		UserId = MOD_1_ID,
			//	},
			//	new IdentityUserRole<string>
			//	{
			//		RoleId = USER_ROLE_ID,
			//		UserId = USER_1_ID,
			//	},
			//	new IdentityUserRole<string>
			//	{
			//		RoleId = USER_ROLE_ID,
			//		UserId = USER_2_ID,
			//	},
			//	new IdentityUserRole<string>
			//	{
			//		RoleId = USER_ROLE_ID,
			//		UserId = USER_3_ID,
			//	},
			//	new IdentityUserRole<string>
			//	{
			//		RoleId = USER_ROLE_ID,
			//		UserId = USER_4_ID,
			//	}
			//});

			//var players = new Player[]
			//{
			//	new Player
			//	{
			//		Username = "admin",
			//		AmountWon = 2,
			//		AmountLost = 3,
			//		AmountDraw = 0,
			//		CountryCode = "NL"
			//	},
			//	new Player
			//	{
			//		Username = "mod",
			//		AmountWon = 1,
			//		AmountLost = 3,
			//		AmountDraw = 1,
			//		CountryCode = "NL"
			//	},
			//	new Player
			//	{
			//		Username = "hello1",
			//		AmountWon = 2,
			//		AmountLost = 3,
			//		AmountDraw = 0,
			//		CountryCode = "DE"
			//	},
			//	new Player
			//	{
			//		Username = "hello2",
			//		AmountWon = 2,
			//		AmountLost = 3,
			//		AmountDraw = 0,
			//		CountryCode = "GB"
			//	},
			//	new Player
			//	{
			//		Username = "hello3",
			//		AmountWon = 2,
			//		AmountLost = 3,
			//		AmountDraw = 0,
			//		CountryCode = "NL"
			//	},
			//	new Player
			//	{
			//		Username = "hello4",
			//		AmountWon = 2,
			//		AmountLost = 3,
			//		AmountDraw = 0,
			//		CountryCode = "EG"
			//	},
			//};

			//var playersInGame = new PlayerInGame[]
			//{
			//	new PlayerInGame
			//	{
			//		Username = "hello1",
			//		Color = Color.white,
			//		GameToken = gameTokens[2],
			//		IsHost = true,
			//		Result = GameResult.undecided
			//	},
			//	new PlayerInGame
			//	{
			//		Username = "hello2",
			//		Color = Color.black,
			//		GameToken = gameTokens[2],
			//		IsHost = false,
			//		Result = GameResult.undecided
			//	},
			//	new PlayerInGame
			//	{
			//		Username = "hello3",
			//		Color = Color.white,
			//		GameToken = gameTokens[1],
			//		IsHost = true,
			//		Result = GameResult.undecided
			//	},
			//	new PlayerInGame
			//	{
			//		Username = "hello4",
			//		Color = Color.black,
			//		GameToken = gameTokens[1],
			//		IsHost = false,
			//		Result = GameResult.undecided
			//	}
			//};

			//var games = new Game[]
			//{
			//	//new Game{
			//	//	Token = gameTokens[0],
			//	//	Name = "Game",
			//	//	Description = "A description",
			//	//	Status = GameStatus.Playing,
			//	//	PlayerTurn = Color.white,
			//	//	Board = new IEnumerable<Color>[]
			//	//	{
			//	//		new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>(),
			//	//		new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>(),
			//	//		new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>(),
			//	//		new int[8]{0, 0, 0, 1, 2, 0, 0, 0}.Cast<Color>(),
			//	//		new int[8]{0, 0, 0, 2, 1, 0, 0, 0}.Cast<Color>(),
			//	//		new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>(),
			//	//		new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>(),
			//	//		new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>()
			//	//	}
			//	//},
			//	new Game{
			//		Token = gameTokens[2],
			//		Name = "Game",
			//		Description = "A description",
			//		Status = GameStatus.Playing,
			//		PlayerTurn = Color.white,
			//		Board = new IEnumerable<Color>[]
			//		{
			//			new int[8]{1, 1, 1, 1, 1, 1, 1, 1}.Cast<Color>(),
			//			new int[8]{1, 1, 1, 1, 1, 1, 1, 1}.Cast<Color>(),
			//			new int[8]{1, 1, 1, 1, 0, 1, 1, 1}.Cast<Color>(),
			//			new int[8]{1, 1, 1, 1, 2, 1, 1, 1}.Cast<Color>(),
			//			new int[8]{1, 1, 1, 2, 1, 1, 1, 1}.Cast<Color>(),
			//			new int[8]{1, 1, 1, 1, 1, 1, 1, 1}.Cast<Color>(),
			//			new int[8]{1, 1, 1, 1, 1, 1, 1, 1}.Cast<Color>(),
			//			new int[8]{1, 1, 1, 1, 1, 1, 1, 1}.Cast<Color>()
			//		}
			//	},
			//	new Game{
			//		Token = gameTokens[1],
			//		Name = "Game1",
			//		Description = "A description1",
			//		Status = GameStatus.Playing,
			//		PlayerTurn = Color.black,
			//		Board = new IEnumerable<Color>[]
			//		{
			//			new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>(),
			//			new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>(),
			//			new int[8]{0, 0, 1, 2, 0, 0, 0, 0}.Cast<Color>(),
			//			new int[8]{0, 0, 0, 1, 2, 0, 0, 0}.Cast<Color>(),
			//			new int[8]{0, 0, 0, 2, 1, 0, 0, 0}.Cast<Color>(),
			//			new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>(),
			//			new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>(),
			//			new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>()
			//		}
			//	}
			//};

			//var turns = new Turn[]
			//{
			//	new Turn
			//	{
			//		GameToken = gameTokens[1],
			//		Number = 1,
			//		X = 3,
			//		Y = 2,
			//		Color = Color.black
			//	},
			//	new Turn
			//	{
			//		GameToken = gameTokens[1],
			//		Number = 2,
			//		X = 2,
			//		Y = 2,
			//		Color = Color.white
			//	},
			//	new Turn
			//	{
			//		GameToken = gameTokens[2],
			//		Number = 1,
			//		X = 0,
			//		Y = 0,
			//		Color = Color.black
			//	}
			//};

			// Player
			builder.Entity<Player>(entity =>
			{
				entity.HasOne(player => player.Account)
					.WithOne(account => account.Player)
					.HasForeignKey<Player>(player => player.Username)
					.HasPrincipalKey<Account>(account => account.UserName);
			});
			//builder.Entity<Player>().HasData(players);

			// Game
			//builder.Entity<Game>().HasData(games);

			// PlayerInGame
			builder.Entity<PlayerInGame>()
				.HasKey(playerInGame => new { playerInGame.Username, playerInGame.GameToken });

			builder.Entity<PlayerInGame>(entity =>
				entity.HasOne(playersInGame => playersInGame.Game)
					.WithMany(game => game.Players)
					.HasForeignKey(playersInGame => playersInGame.GameToken));

			builder.Entity<PlayerInGame>(entity =>
				entity.HasOne(playerInGame => playerInGame.Player)
					.WithMany(player => player.PlayerInGame));

			//.HasForeignKey<PlayerInGame>(playerInGame => playerInGame.Username)
			//.HasPrincipalKey<Player>(player => player.Username));
			//builder.Entity<PlayerInGame>().HasData(playersInGame);

			// Turn
			builder.Entity<Turn>()
				.HasKey(turn => new { turn.Number, turn.GameToken });

			builder.Entity<Turn>(entity =>
				entity.HasOne(turn => turn.Game)
					.WithMany(game => game.Turns)
					.HasForeignKey(playersInGame => playersInGame.GameToken));

			//builder.Entity<Turn>().HasData(turns);
		}
	}
}
