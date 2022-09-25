﻿import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { Log } from 'oidc-react';
import type { ApiRequest } from '.';
import { ErrorResponse } from '../api';
import Game from '../model/Game';
import Message from '../model/Message';
import PlayerInGame, { Color } from '../model/PlayerInGame';
import Turn from '../model/Turn';

type PlayerType = 1 | 2;
type CameraMode = 'perspective' | 'orthographic';

interface Controls {
	mode: CameraMode
}

export interface HistoryItem {
	type: 'history' | 'chat',
	item: Turn | Message
}


// Define a type for the slice state
interface GameState {
	hasGame: boolean;
	players?: [PlayerInGame, PlayerInGame];
	player?: PlayerType,
	turn?: PlayerType,
	turns: Turn[],
	history: HistoryItem[],
	step: number,
	boards: number[][][],
	placeholders: [number, number][]
	controls: Controls
}

// Define the initial state using that type
const initialState: GameState = {
	hasGame: false,
	players: undefined,
	player: 1,
	turn: 1,
	turns: [],
	history: [],
	step: 0,
	boards: [],
	placeholders: [],
	controls: {
		mode: 'perspective'
	}
}

const allDirections: [number, number][] = [
	[-1, -1], [0, -1], [1, -1],
	[-1, 0], [1, 0],
	[-1, 1], [0, 1], [1, 1]
];

const checkIfValid = (board: number[][], position: [number, number], directions: [number, number][], turn: number) => {
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
						color - 1);

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

const createTurnBoards = (turns: Turn[]) => {
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

		console.log(turn);

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

export const gameSlice = createSlice({
	name: 'game',
	initialState,
	reducers: {
		startGame: (state, action: PayloadAction<1 | 2>) => {
			state.player = action.payload;
			state.turn = action.payload;
			state.boards = [[
				[0, 0, 0, 0, 0, 0, 0, 0],
				[0, 0, 0, 0, 0, 0, 0, 0],
				[0, 0, 0, 0, 0, 0, 0, 0],
				[0, 0, 0, 2, 1, 0, 0, 0],
				[0, 0, 0, 1, 2, 0, 0, 0],
				[0, 0, 0, 0, 0, 0, 0, 0],
				[0, 0, 0, 0, 0, 0, 0, 0],
				[0, 0, 0, 0, 0, 0, 0, 0],
			]];
		},
		setBoard: (state, action: PayloadAction<number[][]>) => {
			state.boards = [action.payload];
		},
		setStep: (state, action: PayloadAction<number>) => {
			state.step = action.payload;
			state.placeholders = createPlaceholderMap(state.boards[state.step], state.turns[state.step].color);
			state.turn = state.turns[state.step].color as PlayerType;
		},
		setGame: (state, action: PayloadAction<Game>) => {
			const messages = [
				{ username: 'hello3', datetime: new Date().toISOString(), text: 'hello' },
				{ username: 'hello3', datetime: new Date().toISOString(), text: 'hi' },
				{ username: 'hello3', datetime: new Date().toISOString(), text: 'gl hf' },
			]

			state.hasGame = true;
			state.turn = action.payload.playerTurn as PlayerType;
			state.turns = action.payload.turns;
			state.history = [
				...messages.map(message => ({
					type: 'chat',
					item: message
				}) as HistoryItem),
				...action.payload.turns.map(turn => ({
					type: 'history',
					item: turn
				}) as HistoryItem)
			].sort((a, b) => new Date(a.item.datetime).getTime() - new Date(b.item.datetime).getTime());

			state.step = action.payload.turns.length;
			state.boards = createTurnBoards(action.payload.turns);
			state.players = action.payload.players;

			state.placeholders = createPlaceholderMap(state.boards[state.step], action.payload.playerTurn);

			//console.log(createTurnBoards(action.payload.turns));
		},
		putStone: (state, { payload: [locX, locY] }: PayloadAction<[number, number]>) => {
			if (locX < 0
				|| locY < 0
				|| locX > 7
				|| locY > 7) return state;

			const checkResult = checkIfValid(state.boards[state.step], [locX, locY], allDirections, state.player! - 1);

			if (!checkResult.valid) return state;

			checkResult.paths.forEach(path => {
				path.forEach(position => {
					state.boards[state.step][position[1]][position[0]] = state.player!;
				});
			});
		},
		toggleCameraMode: (state) => {
			state.controls.mode = state.controls.mode === 'perspective'
				? 'orthographic'
				: 'perspective';
		}
	}
});

export const createGame = (game: Pick<Game, 'name' | 'description'>, errorCallback: ErrorResponse = console.error): ApiRequest =>
	async (dispatch, getState, { createGame }) =>
		createGame(getState().auth, game)
			.then(game => dispatch(setGame(game)))
			.catch(errorCallback);

export const joinGame = (token: string): ApiRequest =>
	async (dispatch, getState, { joinGame }) =>
		joinGame(getState().auth, token)
			.then(game => dispatch(setGame(game)))
			.catch(console.error);

export const fetchGame = (errorCallback: ErrorResponse = console.error): ApiRequest =>
	async (dispatch, getState, { getGame }) =>
		getGame(getState().auth)
			.then(game => dispatch(setGame(game)))
			.catch(errorCallback);

export const putStone = (position: [number, number]): ApiRequest =>
	async (dispatch, getState, { putStone }) =>
		putStone(getState().auth, position)
			.then(game => dispatch(setGame(game)))
			.catch(console.error);

export const passTurn = (): ApiRequest =>
	async (dispatch, getState, { passTurn }) =>
		passTurn(getState().auth)
			.then(game => dispatch(setGame(game)))
			.catch(console.log);

export const giveUp = (): ApiRequest =>
	async (dispatch, getState, { giveUp }) =>
		giveUp(getState().auth)
			.then(game => dispatch(setGame(game)))
			.catch(console.error);

export const { startGame, setBoard, setStep, setGame, toggleCameraMode } = gameSlice.actions

export default gameSlice.reducer