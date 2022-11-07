import { Color } from "../model/PlayerInGame";
import Turn from "../model/Turn";

export const allDirections: [number, number][] = [
	[-1, -1], [0, -1], [1, -1],
	[-1, 0], [1, 0],
	[-1, 1], [0, 1], [1, 1]
];

export function checkIfValid(board: number[][], position: [number, number], directions: [number, number][], turn: number) {
	if (board[position[1]][position[0]] !== 0) return { valid: false, paths: [] };

	return directions.reduce((result, direction) => {
		let hasOpponent = false;
		const path: [number, number][] = [];

		for (let index = 1,
			xIndex = index * direction[0] + position[0],
			yIndex = index * direction[1] + position[1];

			xIndex > -1 && yIndex > -1 && xIndex < 8 && yIndex < 8;

			++index,
			xIndex = index * direction[0] + position[0],
			yIndex = index * direction[1] + position[1]) {
			if (board[yIndex][xIndex] === turn + 1 && hasOpponent) {
				return {
					valid: true,
					paths: [
						...result.paths,
						path
					]
				};
			} else if (board[yIndex][xIndex] === 1 - turn + 1) {
				hasOpponent = true;
				path.push([xIndex, yIndex]);
			} else {
				break;
			}
		}

		return result;
	}, {
		valid: false,
		paths: [[position]]
	});
}

export function createPlaceholderMap (board: number[][], color: Color) {
	const directions: [number, number][] = [
		[-1, -1], [0, -1], [1, -1],
		[-1, 0], [1, 0],
		[-1, 1], [0, 1], [1, 1]
	];
	const opponent = Number(color == Color.white ? Color.black : Color.white);
	const placeHolderMap = [
		[0, 0, 0, 0, 0, 0, 0, 0],
		[0, 0, 0, 0, 0, 0, 0, 0],
		[0, 0, 0, 0, 0, 0, 0, 0],
		[0, 0, 0, 0, 0, 0, 0, 0],
		[0, 0, 0, 0, 0, 0, 0, 0],
		[0, 0, 0, 0, 0, 0, 0, 0],
		[0, 0, 0, 0, 0, 0, 0, 0],
		[0, 0, 0, 0, 0, 0, 0, 0]
	];

	// Every Row
	return board.reduce<[number, number][]>((state, row, yIndex) =>
		// Every Column
		row.reduce((state, cell, xIndex) =>
			(cell !== opponent)
				? state
				: directions.reduce((state, direction) => {
					const [locX, locY]: [number, number] = [xIndex + direction[0], yIndex + direction[1]];

					if (locX < 0 || locY < 0 || locX > 7 || locY > 7
						|| placeHolderMap[locY][locX] !== 0
						|| board[locY][locX] !== 0) return state;

					const result = checkIfValid(
						board,
						[locX, locY],
						[[xIndex - locX, yIndex - locY]],
						color - 1
					);

					if (result.valid) {
						placeHolderMap[locY][locX] = 1;
						return [
							...state,
							[locX, locY]
						]
					}

					return state;
				}, state)
			, state)
		, []);
}

export function createTurnBoards (turns: Turn[]) {
	const defaultBoard = [
		[0, 0, 0, 0, 0, 0, 0, 0],
		[0, 0, 0, 0, 0, 0, 0, 0],
		[0, 0, 0, 0, 0, 0, 0, 0],
		[0, 0, 0, 1, 2, 0, 0, 0],
		[0, 0, 0, 2, 1, 0, 0, 0],
		[0, 0, 0, 0, 0, 0, 0, 0],
		[0, 0, 0, 0, 0, 0, 0, 0],
		[0, 0, 0, 0, 0, 0, 0, 0],
	];

	return turns.reduce((state, turn, index) => {
		const result = checkIfValid(state[index], [turn.x, turn.y], allDirections, turn.color - 1);

		if (!result.valid) return state;

		const newBoard = state[index].map(row => [...row]);

		result.paths.forEach(path => {
			path.forEach(position => {
				newBoard[position[1]][position[0]] = turn.color;
			});
		});

		return [
			...state,
			newBoard
		];
	}, [defaultBoard])
}

export const countColors = (board: number[][]) => board.reduce((state, value) =>
	value.reduce((state, value) => [
		state[Color.none] + Number(value == Color.none),
		state[Color.white] + Number(value == Color.white),
		state[Color.black] + Number(value == Color.black)
	], state) //[0, 0, 0])
		//.map((count, index) => count + state[index])
	, [0, 0, 0]);