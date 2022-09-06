import type Game from "./Game";

export default interface Account {
	token: string,
	username: string,
	email: string,
	password: string,
	game: Game | null
}