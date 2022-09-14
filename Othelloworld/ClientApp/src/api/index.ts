import Game from "../model/Game";
import Player from "../model/Player";
import Token from "../model/Token";
import { Country } from "../store/World";

export const register = (username: string, email: string, password: string) =>
	fetch('/account/register',
		{
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
				'Accept': 'application/json'
			},
			body: JSON.stringify({ username, email, password })
		})
		.then(result => result.json())
		.then(json => json as Token);

export const login = (email: string, password: string) =>
	fetch('/account/login',
		{
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
				'Accept': 'application/json'
			},
			body: JSON.stringify({ email, password })
		})
		.then(result => result.json())
		.then(json => json as Token);

	// Games
export const createGame = (token: Token, game: Pick<Game, 'name' | 'description'>) =>
	fetch('/game',
		{
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
				'Accept': 'application/json',
				'Authorization': `Bearer ${token.token}`
			},
			body: JSON.stringify(game)
		})
		.then(result => result.json())
		.then(json => json as Game);

export const getGames = (token: Token) =>
	fetch(`/game/pages?pageNumber=1&pageSize=3`,
		{
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${token.token}`
			}
		})
		.then(result => result.json())
		.then(json => json as Game[]);

export const getGame = (token: Token) =>
	fetch(`/game`,
		{
			method: 'GET',
			headers: {
				'Accept': 'application/json',
				'Authorization': `Bearer ${token.token}`
			}
		})
		.then(result => result.json())
		.then(json => json as Game);

export const putStone = (token: Token, position: [number, number]) =>
	fetch(`/game`,
		{
			method: 'PUT',
			headers: {
				'Content-Type': 'application/json',
				'Accept': 'application/json',
				'Authorization': `Bearer ${token.token}`
			},
			body: JSON.stringify(position)
		})
		.then(result => {
			if (result.status !== 200) throw new Error(result.statusText)
			return result.json();
		})
		.then(json => json as Game);

export const getPlayer = (token: Token, username: string) =>
	fetch(`/player/${username}`,
		{
			method: 'GET',
			headers: {
				'Accept': 'application/json',
				'Authorization': `Bearer ${token.token}`
			}
		})
		.then(result => {
			if (result.status !== 200) throw new Error(result.statusText)
			return result.json();
		})
		.then(json => json as Player);

export const fetchCountries = () =>
	fetch('./countries.geojson')
		.then(result => result.json())
		.then(json => json as Country[]);