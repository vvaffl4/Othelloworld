import Player from "./Player";

export enum Color {
	none = 0,
	white = 1,
	black = 2
}

export default interface PlayerInGame {
	username: string;
	player: Player;
	color: Color;
	isHost: boolean;
	gameToken: string;
}