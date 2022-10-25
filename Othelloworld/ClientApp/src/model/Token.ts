import Player from "./Player";

export default interface Token {
	token: string;
	player?: Player;
	expires: string;
}