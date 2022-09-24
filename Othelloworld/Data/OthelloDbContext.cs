
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
	public class OthelloDbContext : IdentityDbContext<Account>
	{
		private DbSet<Account> Accounts { get; set; }
		public DbSet<Player> Players { get; set; }
		public DbSet<Game> Games { get; set; }
		private DbSet<PlayerInGame> PlayersInGame { get; set; }

		private DbSet<Turn> Turns { get; set; }

		public OthelloDbContext(
			DbContextOptions<OthelloDbContext> options) : base(options)
		{ }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			var gameTokens = new string[]{
				Guid.NewGuid().ToString(),
				Guid.NewGuid().ToString(),
				Guid.NewGuid().ToString()
			};

			// any unique string id
			const string ADMIN_ROLE_ID = "ad376a8f-9eab-4bb9-9fca-30b01540f445";
			const string USER_ROLE_ID = "ad376a8f-9eab-4444-9fca-30b01540f445";

			// users
			const string ADMIN_ID = "a18be9c0-aa65-4af8-bd17-00bd9344e575";
			var USER_1_ID = Guid.NewGuid().ToString();
			var USER_2_ID = Guid.NewGuid().ToString();
			var USER_3_ID = Guid.NewGuid().ToString();
			var USER_4_ID = Guid.NewGuid().ToString();


			builder.Entity<IdentityRole>().HasData(new IdentityRole[]
			{
				new IdentityRole
				{
					Id = USER_ROLE_ID,
					Name = "user",
					NormalizedName = "user"
				},
				new IdentityRole
				{
					Id = ADMIN_ROLE_ID,
					Name = "admin",
					NormalizedName = "admin"
				}
			});

			var hasher = new PasswordHasher<Account>();
			builder.Entity<Account>().HasData(new Account[] {
				new Account
				{
					Id = ADMIN_ID,
					UserName = "admin",
					NormalizedUserName = "admin",
					Email = "admin@gmail.com",
					NormalizedEmail = "admin@gmail.com",
					EmailConfirmed = true,
					PasswordHash = hasher.HashPassword(null, "Admin123!"),
					SecurityStamp = String.Empty
				},
				new Account
				{
					Id = USER_1_ID,
					UserName = "hello1",
					NormalizedUserName = "hello1",
					Email = "hello1@gmail.com",
					NormalizedEmail = "hello1@gmail.com",
					EmailConfirmed = true,
					PasswordHash = hasher.HashPassword(null, "hello1!"),
					SecurityStamp = String.Empty
				},
				new Account
				{
					Id = USER_2_ID,
					UserName = "hello2",
					NormalizedUserName = "hello2",
					Email = "hello2@gmail.com",
					NormalizedEmail = "hello2@gmail.com",
					EmailConfirmed = true,
					PasswordHash = hasher.HashPassword(null, "hello2!"),
					SecurityStamp = String.Empty
				},
				new Account
				{
					Id = USER_3_ID,
					UserName = "hello3",
					NormalizedUserName = "hello3",
					Email = "hello3@gmail.com",
					NormalizedEmail = "hello3@gmail.com",
					EmailConfirmed = true,
					PasswordHash = hasher.HashPassword(null, "hello3!"),
					SecurityStamp = String.Empty
				},
				new Account
				{
					Id = USER_4_ID,
					UserName = "hello4",
					NormalizedUserName = "hello4",
					Email = "hello4@gmail.com",
					NormalizedEmail = "hello4@gmail.com",
					EmailConfirmed = true,
					PasswordHash = hasher.HashPassword(null, "hello4!"),
					SecurityStamp = String.Empty
				}
			});

			builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>[]
			{
				new IdentityUserRole<string>
				{
					RoleId = ADMIN_ROLE_ID,
					UserId = ADMIN_ID,
				},
				new IdentityUserRole<string>
				{
					RoleId = USER_ROLE_ID,
					UserId = USER_1_ID,
				},
				new IdentityUserRole<string>
				{
					RoleId = USER_ROLE_ID,
					UserId = USER_2_ID,
				},
				new IdentityUserRole<string>
				{
					RoleId = USER_ROLE_ID,
					UserId = USER_3_ID,
				},
				new IdentityUserRole<string>
				{
					RoleId = USER_ROLE_ID,
					UserId = USER_4_ID,
				}
			});

			var players = new Player[]
			{
				new Player
				{
					Username = "admin",
					AmountWon = 2,
					AmountLost = 3,
					AmountDraw = 0,
					Country = "nl"
				}, 
				new Player
				{
					Username = "hello1",
					AmountWon = 2,
					AmountLost = 3,
					AmountDraw = 0,
					Country = "de"
				},
				new Player
				{
					Username = "hello2",
					AmountWon = 2,
					AmountLost = 3,
					AmountDraw = 0,
					Country = "gb"
				},
				new Player
				{
					Username = "hello3",
					AmountWon = 2,
					AmountLost = 3,
					AmountDraw = 0,
					Country = "nl"
				},
				new Player
				{
					Username = "hello4",
					AmountWon = 2,
					AmountLost = 3,
					AmountDraw = 0,
					Country = ""
				},
			};

			var playersInGame = new PlayerInGame[]
			{
				new PlayerInGame
				{
					Username = "hello1",
					Color = Color.white,
					GameToken = gameTokens[0],
					IsHost = true,
					Result = GameResult.undecided
				},
				new PlayerInGame
				{
					Username = "hello2",
					Color = Color.black,
					GameToken = gameTokens[0],
					IsHost = false,
					Result = GameResult.undecided
				},
				new PlayerInGame
				{
					Username = "hello3",
					Color = Color.white,
					GameToken = gameTokens[1],
					IsHost = true,
					Result = GameResult.undecided
				},
				new PlayerInGame
				{
					Username = "hello4",
					Color = Color.black,
					GameToken = gameTokens[1],
					IsHost = false,
					Result = GameResult.undecided
				}
			};

			var games = new Game[]
			{
				new Game{
					Token = gameTokens[0],
					Name = "Game",
					Description = "A description",
					Status = GameStatus.Playing,
					PlayerTurn = Color.white,
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
				},
				new Game{
					Token = gameTokens[1],
					Name = "Game1",
					Description = "A description1",
					Status = GameStatus.Playing,
					PlayerTurn = Color.black,
					Board = new IEnumerable<Color>[]
					{
						new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>(),
						new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>(),
						new int[8]{0, 0, 1, 2, 0, 0, 0, 0}.Cast<Color>(),
						new int[8]{0, 0, 0, 1, 2, 0, 0, 0}.Cast<Color>(),
						new int[8]{0, 0, 0, 2, 1, 0, 0, 0}.Cast<Color>(),
						new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>(),
						new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>(),
						new int[8]{0, 0, 0, 0, 0, 0, 0, 0}.Cast<Color>()
					}
				}
			};

			var turns = new Turn[]
			{
				new Turn
				{
					GameToken = gameTokens[1],
					Number = 1,
					X = 3,
					Y = 2,
					Color = Color.black
				},
				new Turn
				{
					GameToken = gameTokens[1],
					Number = 2,
					X = 2,
					Y = 2,
					Color = Color.white
				}
			};

			// Player
			builder.Entity<Player>(entity =>
				entity.HasOne(player => player.Account)
					.WithOne(account => account.Player)
					.HasForeignKey<Player>(player => player.Username)
					.HasPrincipalKey<Account>(account => account.UserName));
			builder.Entity<Player>().HasData(players);
			
			// Game
			builder.Entity<Game>().HasData(games);

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
			builder.Entity<PlayerInGame>().HasData(playersInGame);

			// Turn
			builder.Entity<Turn>()
				.HasKey(turn => new { turn.Number, turn.GameToken });

			builder.Entity<Turn>(entity =>
				entity.HasOne(turn => turn.Game)
					.WithMany(game => game.Turns)
					.HasForeignKey(playersInGame => playersInGame.GameToken));

			builder.Entity<Turn>().HasData(turns);
		}

		//public Task<bool> HasAnyAccounts()
		//{
		//	return Accounts.AnyAsync();
		//}

		//private Account SanitizeAccount(Account account)
		//{
		//	account.Username = account.Username.ToLower();
		//	account.Password = account.Password.ToLower();
		//	account.Email = account.Email.ToLower();
		//	return account;
		//}

		//public async Task<Account> CreateAccount(Account account)
		//{
		//	SanitizeAccount(account);

		//	await Accounts.AddAsync(account);

		//	await SaveChangesAsync();

		//	return account;
		//}

		//public Task<Account> FindAccount(string identifier)
		//{
		//	return Accounts
		//			.Where(account => account.Username == identifier || account.Email == identifier)
		//			.SingleOrDefaultAsync();
		//}

		//public Task<Account> LoginAccount(string identifier, string passwordEncrypt)
		//{
		//	return Accounts
		//			.Where(account =>
		//					(account.Username == identifier
		//					|| account.Email == identifier)
		//					&& account.Password == passwordEncrypt)
		//			.SingleOrDefaultAsync();
		//}


		//// Games

		//public Task<bool> HasAnyGames()
		//{
		//	return Games.AnyAsync();
		//}

		//internal async Task<Game[]> GetGames()
		//{
		//	return await Games.Include(game => game.Players).ToArrayAsync();
		//}

		//public Task<Game> GetGame(string token)
		//{
		//	return Games.SingleAsync(game => game.Token == token);
		//}

		//public async Task<Game> PutStone(string gameToken, string accountToken, int position)
		//{
		//	var game = Games
		//			.SingleOrDefault(game =>
		//					game.Token == gameToken);

		//	if (game == null)
		//	{
		//		game.Board[position] = 1;
		//		await SaveChangesAsync();

		//		return game;
		//	}

		//	throw new System.Exception("Game does not exist.");
		//}
	}
}
