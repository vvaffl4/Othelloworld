import PlayerInGame from "./PlayerInGame";

export default interface Game {
	token: string;
	name: string;
	description: string;
	players: [PlayerInGame, PlayerInGame];
	board: number[][];
	playerTurn: number;
}