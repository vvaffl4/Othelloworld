import Game from "../model/Game";
import PagedList from "../model/PagedList";
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

export const changePassword = (passwordChange: { currentPassword: string, newPassword: string }, captchaToken: string, token: Token) =>
	fetch('/account/changepassword',
		{
			method: 'PUT',
			headers: {
				'Content-Type': 'application/json',
				'Accept': 'application/json',
				'Authorization': `Bearer ${token.token}`
			},
			body: JSON.stringify({ ...passwordChange, captchaToken })
		})
		.then(checkResponseStatus)
		.then(parseResultToJson)
		.then(json => json as Token);

export const forgotPassword = (account: { email: string }, captchaToken: string) =>
	fetch('/account/forgotpassword',
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

export const recoverPassword = (passwordRecovery: { newPassword: string, email: string, resetToken: string }, captchaToken: string) =>
	fetch('/account/recoverpassword',
		{
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
				'Accept': 'application/json'
			},
			body: JSON.stringify({ ...passwordRecovery, captchaToken })
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

export const getGames = (pageNumber: number, pageSize: number, token: Token) =>
	fetch(`/game/pages?pageNumber=${pageNumber}&pageSize=${pageSize}`,
		{
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${token.token}`
			}
		})
		.then(checkResponseStatus)
		.then(parseResultToJson)
		.then(json => json as PagedList<Game>);

export const searchGames = (searchValue: string, pageNumber: number, pageSize: number, token: Token) =>
	fetch(`/game/search/${searchValue}?pageNumber=${pageNumber}&pageSize=${pageSize}`,
		{
			method: 'GET',
			headers: {
				'Authorization': `Bearer ${token.token}`
			}
		})
		.then(checkResponseStatus)
		.then(parseResultToJson)
		.then(json => json as PagedList<Game>);

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


export const confirmResults = (token: Token) =>
	fetch(`/game/confirm`,
		{
			method: 'POST',
			headers: {
				'Accept': 'application/json',
				'Authorization': `Bearer ${token.token}`
			}
		})
		.then(checkResponseStatus)
		.then(parseResultToJson)
		.then(json => json as Game);

export const fetchCountries = () =>
	fetch('./countries.json')
		.then(checkResponseStatus)
		.then(parseResultToJson)
		.then(json => json as Country[]);