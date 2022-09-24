import { Color } from "./PlayerInGame";

export default interface Turn {
	number: number;
	x: number;
	y: number;
	color: Color;
	datetime: string;
}