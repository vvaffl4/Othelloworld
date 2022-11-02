import Game from "../model/Game";
import Player from "../model/Player";
import Token from "../model/Token";
import { Country } from "../store/World";

export interface AccountDTO {
	username: string;
	email: string;
	password: string;
	country: string;
}

export interface LoginDTO {
	email: string;
	password: string;
}

export type ErrorResponse = (result: Response) => void;

const checkResponseStatus = (result: Response) =>
	result.status > 199 && result.status < 400
		? result
		: (() => { throw result })();

const parseResultToJson = ((result: Response) => result.json());

export const register = (account: AccountDTO, captchaToken: string) =>
	fetch('/account/register',
		{
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
				'Accept': 'application/json'
			},
			body: JSON.stringify({ ...account, captchaToken })
		})
		.then(checkResponseStatus)
		.then(parseResultToJson)
		.then(json => json as Token);

export const login = (credentials: LoginDTO) =>
	fetch('/account/login',
		{
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
				'Accept': 'application/json'
			},
			body: JSON.stringify(credentials)
		})
		.then(checkResponseStatus)
		.then(parseResultToJson)
		.then(json => json as Token);

export const logout = (token: Token) =>
	fetch('/account/logout',
		{
			method: 'POST',
			headers: {
				'Authorization': `Bearer ${token.token}`
			}
		})
		.then(checkResponseStatus);

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
		.then(checkResponseStatus)
		.then(parseResultToJson)
		.then(json => json as Game);

export const joinGame = (token: Token, gameToken: string) =>
	fetch('/game/join',
		{
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
				'Accept': 'application/json',
				'Authorization': `Bearer ${token.token}`
			},
			body: JSON.stringify({ token: gameToken })
		})
		.then(checkResponseStatus)
		.then(parseResultToJson)
		.then(json => json as Game)

export const getGames = (token: Token) =>
	fetch(`/game/pages?pageNumber=1&pageSize=3`,
		{
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${token.token}`
			}
		})
		.then(checkResponseStatus)
		.then(parseResultToJson)
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
		.then(checkResponseStatus)
		.then(parseResultToJson)
		.then(json => json as Game);

export const getGameHistory = (token: Token, username: string) =>
	fetch(`/game/history/${username}`,
		{
			method: 'GET',
			headers: {
				'Accept': 'application/json',
				'Authorization': `Bearer ${token.token}`
			}
		})
		.then(checkResponseStatus)
		.then(parseResultToJson)
		.then(json => json as Game[]);

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
		.then(checkResponseStatus)
		.then(parseResultToJson)
		.then(json => json as Game);

export const passTurn = (token: Token) =>
	fetch('/game/pass',
		{
			method: 'PUT',
			headers: {
				'Accept': 'application/json',
				'Authorization': `Bearer ${token.token}`
			}
		})
		.then(checkResponseStatus)
		.then(parseResultToJson)
		.then(json => json as Game);

export const giveUp = (token: Token) =>
	fetch('/game/giveup',
		{
			method: 'PUT',
			headers: {
				'Accept': 'application/json',
				'Authorization': `Bearer ${token.token}`
			}
		})
		.then(checkResponseStatus)
		.then(parseResultToJson)
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
		.then(checkResponseStatus)
		.then(parseResultToJson)
		.then(json => json as Player);

export const fetchCountries = () =>
	fetch('./countries.geojson')
		.then(checkResponseStatus)
		.then(parseResultToJson)
		.then(json => json as Country[]);