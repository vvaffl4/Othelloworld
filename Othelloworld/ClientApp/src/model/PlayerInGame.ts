import Player from "./Player";

export enum Color {
	none = 0,
	white = 1,
	black = 2
}

export enum GameResult {
	undecided = 0,
	won = 1,
	lost = 2,
	draw = 3
}

export default interface PlayerInGame {
	player: Player;
	color: Color;
	isHost: boolean;
	result: GameResult;
	gameToken: string;
}