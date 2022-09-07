import Game from "../model/Game";
import Token from "../model/Token";
import { Country } from "../store/World";

const Api = {
	register: (username: string, email: string, password: string) =>
		fetch('/Account/register',
			{
				method: 'POST',
				headers: {
					'Content-Type': 'application/json',
					'Accept': 'application/json'
				},
				body: JSON.stringify({ username, email, password })
			})
			.then(result => result.json())
			.then(json => json as Token),

	login: (email: string, password: string) =>
		fetch('/Account/login',
			{
				method: 'POST',
				headers: {
					'Content-Type': 'application/json',
					'Accept': 'application/json'
				},
				body: JSON.stringify({ email, password })
			})
			.then(result => result.json())
			.then(json => json as Token),

	// Games
	createGame: (token: Token, game: Pick<Game, 'name' | 'description'>) =>
		fetch('/Game',
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
			.then(json => json as Game),

	getGames: (token: Token) => fetch(`/Game?pageNumber=1&pageSize=3`, {
		method: 'GET',
		headers: {
			'Authorization': `Bearer ${token.token}`
		}
	})
		.then(result => result.json())
		.then(json => json as Game[]),

	getGame: (token: Token, gameToken: string) => fetch(`/Game/${gameToken}`,
		{
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${token.token}`
			}
		})
		.then(result => result.json())
		.then(json => json as Game),

	putStone: (token: Token, position: [number, number]) =>
		fetch(`/Game/test/AnAccountToken`,
		{
			method: 'PUT',
			headers: {
				'Content-Type': 'application/json',
				'Authorization': `Bearer ${token.token}`
			},
			body: JSON.stringify(position)
		})
			.then(result => {
				if (result.status !== 200) throw new Error(result.statusText)
				return result.json();
			})
			.then(json => json as Game),	

	fetchCountries: () => fetch('./countries.geojson')
    .then(result => result.json())
		.then(json => json as Country[])
}

export type ApiType = typeof Api;
export default Api;