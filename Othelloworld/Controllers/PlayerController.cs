﻿using Duende.IdentityServer.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Othelloworld.Data.Models;
using Othelloworld.Data.Repos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Othelloworld.Controllers
{
	[Authorize(Roles = "user, mod, admin")]
	[ApiController]
	[Route("[controller]")]
	public class PlayerController : Controller
	{
		private readonly IPlayerRepository _playerRepository;

		public PlayerController(IPlayerRepository playerRepository)
		{
			_playerRepository = playerRepository;
		}
		[HttpGet("{username}")]
		public async Task<ActionResult<Player>> GetAsync(string username)
		{
			if (!ModelState.IsValid || username.IsNullOrEmpty()) return BadRequest("Provided params are incorrect");

			Player player = await _playerRepository.FindByCondition(player => player.Username == username)
				.FirstAsync();

			return Ok(player);
		}

		[HttpGet("countries")]
		public ActionResult<IEnumerable<Country>> GetCountries()
		{
			IEnumerable<Country> countries = _playerRepository.GetCountries();

			return Ok(countries);
		}
	}
}
