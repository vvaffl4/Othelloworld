import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import type { ApiRequest } from '.';
import Game from '../model/Game';
import PlayerInGame, { Color } from '../model/PlayerInGame';

type PlayerType = 1 | 2;

// Define a type for the slice state
interface GameState {
	token?: string;
	players?: [PlayerInGame, PlayerInGame];
	player?: PlayerType,
	turn?: PlayerType,
	board: number[][],
	placeholders: [number, number][]
}

// Define the initial state using that type
const initialState: GameState = {
	players: undefined,
	player: 1,
	turn: 1,
	board: [],
	placeholders: []
}

const allDirections: [number, number][] = [
	[-1, -1], [0, -1], [1, -1],
	[-1, 0], [1, 0],
	[-1, 1], [0, 1], [1, 1]
];

const checkIfValid = (board: number[][], position: [number, number], directions: [number, number][], turn: number) => {
	//const directions = [
	//	[-1, -1], [0, -1], [1, -1],
	//	[-1,  0],					 [1,  0],
	//	[-1,  1], [0,  1], [1,  1]
	//];

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
			yIndex = index * direction[1] + position[1])
		{			
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

const createPlaceholderMap = (board: number[][], color: Color) => {
	const directions: [number, number][] = [
		[-1, -1], [0, -1], [1, -1],
		[-1,  0],		   [1,  0],
		[-1,  1], [0,  1], [1,  1]
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
	return board.reduce<[number, number][]>((state, row, yIndex) => [
		...state,
		// Every Column
		...row.reduce<[number, number][]>((state, cell, xIndex) => {
			if (cell !== opponent) return state;

			return [
				...state,
				...directions.reduce<[number, number][]>((state, direction) => {
				const [locX, locY]: [number, number] = [xIndex + direction[0], yIndex + direction[1]];

				if (locX < 0 || locY < 0 || locX > 7 || locY > 7
					|| placeHolderMap[locY][locX] !== 0
					|| board[locY][locX] !== 0) return state;

					console.log(locX, locY);

					const result = checkIfValid(
						board,
						[locX, locY],
						[[xIndex - locX, yIndex - locY]],
						color - 1);

				if (result.valid) {
					placeHolderMap[locY][locX] = 1;
					return [
						...state,
						[locX, locY]
					]
				}

				return state;
				}, [])
			]
		}, [])
	], []);
}

export const gameSlice = createSlice({
	name: 'game',
	initialState,
	reducers: {
		startGame: (state, action: PayloadAction<1 | 2>) => {
			state.player = action.payload;
			state.turn = action.payload;
			state.board = [
				[0, 0, 0, 0, 0, 0, 0, 0],
				[0, 0, 0, 0, 0, 0, 0, 0],
				[0, 0, 0, 0, 0, 0, 0, 0],
				[0, 0, 0, 2, 1, 0, 0, 0],
				[0, 0, 0, 1, 2, 0, 0, 0],
				[0, 0, 0, 0, 0, 0, 0, 0],
				[0, 0, 0, 0, 0, 0, 0, 0],
				[0, 0, 0, 0, 0, 0, 0, 0],
			];
		},
		setBoard: (state, action: PayloadAction<number[][]>) => {
			state.board = action.payload;
		},
		setGame: (state, action: PayloadAction<Game>) => {
			state.token = action.payload.token;
			state.board = action.payload.board;
			state.players = action.payload.players;

			state.placeholders = createPlaceholderMap(state.board, Color.white);
		},
		putStone: (state, { payload: [locX, locY] }: PayloadAction<[number, number]>) => {
			if (locX < 0
				|| locY < 0
				|| locX > 7
				|| locY > 7) return state;

			const checkResult = checkIfValid(state.board, [locX, locY], allDirections, state.player! - 1);

			if (!checkResult.valid) return state;

			checkResult.paths.forEach(path => {
				path.forEach(position => {
					state.board[position[1]][position[0]] = state.player!;
				});
			});
		}
	},
})

export const createGame = (game: Pick<Game, 'name' | 'description'>): ApiRequest =>
	async (dispatch, getState, { createGame }) =>
		createGame(getState().auth, game)
			.then(game => dispatch(setGame(game)));

//export const joinGame = (token: string): ApiRequest =>


export const fetchGame = (gameToken: string): ApiRequest =>
	async (dispatch, getState, { getGame }) =>
		getGame(getState().auth, gameToken)
			.then(game => dispatch(setGame(game)))
			.catch(console.error);

export const putStone = (gameToken: string, position: [number, number]): ApiRequest =>
	async (dispatch, getState, { putStone }) =>
		putStone(getState().auth, position)
			.then(game => dispatch(setGame(game)))
			.catch(console.error);

export const { startGame, setBoard, setGame } = gameSlice.actions

export default gameSlice.reducer