import PlayerInGame from "./PlayerInGame";
import Turn from "./Turn";

export enum GameStatus {
	Staging = 0,
	Playing = 1,
	Finished = 2
}

export default interface Game {
	token: string;
	name: string;
	status: GameStatus;
	description: string;
	players: [PlayerInGame, PlayerInGame?];
	board: number[][];
	playerTurn: number;
	turns: Turn[];
}