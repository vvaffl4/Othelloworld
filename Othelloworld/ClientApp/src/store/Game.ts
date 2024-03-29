﻿import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { Log } from 'oidc-react';
import type { ApiRequest } from '.';
import { ErrorResponse } from '../api';
import Game, { GameStatus } from '../model/Game';
import Message from '../model/Message';
import PlayerInGame, { Color, GameResult } from '../model/PlayerInGame';
import Turn from '../model/Turn';
import { allDirections, checkIfValid, createPlaceholderMap, createTurnBoards } from '../services/BoardService';

export type CameraMode = 'perspective' | 'orthographic';

export interface Controls {
	mode: CameraMode
}

export interface Result {
	winner: string;
	loser: string;
	datetime: string;
}

export interface HistoryItem {
	type: 'history' | 'chat' | 'result',
	item: Turn | Message | Result
}

// Define a type for the slice state
export interface GameState {
	hasGame: boolean;
	players?: [PlayerInGame, PlayerInGame?];
	player?: Color,
	turn?: Color,
	turns: Turn[],
	status: GameStatus,
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
	status: GameStatus.Staging,
	history: [],
	step: 0,
	boards: [[
		[0, 0, 0, 0, 0, 0, 0, 0],
		[0, 0, 0, 0, 0, 0, 0, 0],
		[0, 0, 0, 0, 0, 0, 0, 0],
		[0, 0, 0, 2, 1, 0, 0, 0],
		[0, 0, 0, 1, 2, 0, 0, 0],
		[0, 0, 0, 0, 0, 0, 0, 0],
		[0, 0, 0, 0, 0, 0, 0, 0],
		[0, 0, 0, 0, 0, 0, 0, 0],
	]],
	placeholders: [],
	controls: {
		mode: 'perspective'
	}
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

			const color = (2 - state.step % 2) as Color;

			state.placeholders = createPlaceholderMap(state.boards[state.step], color);
			state.turn = color;
		},
		setGame: (state, action: PayloadAction<Game>) => {
			const messages = [
				{ username: 'hello3', datetime: new Date().toISOString(), text: 'hello' },
				{ username: 'hello3', datetime: new Date().toISOString(), text: 'hi' },
				{ username: 'hello3', datetime: new Date().toISOString(), text: 'gl hf' },
			]

			console.log(action.payload)

			state.turn = action.payload.playerTurn as Color;

			if (action.payload.turns) {
				state.turns = action.payload.turns
			} else {
				state.turns = [];
			}
			state.status = action.payload.status;

			var results: Result[] = [];
			if (action.payload.status === GameStatus.Finished) {
				results.push({
					winner: action.payload.players.find(pig => pig!.result === GameResult.won)!.player.username,
					loser: action.payload.players.find(pig => pig!.result === GameResult.lost)!.player.username,
					datetime: new Date().toISOString()
				});
			}

			state.history = [
				...messages.map(message => ({
					type: 'chat',
					item: message
				}) as HistoryItem),
				...state.turns.map(turn => ({
					type: 'history',
					item: turn
				}) as HistoryItem),
				...results.map(result => ({
					type: 'result',
					item: result
				}) as HistoryItem)
			].sort((a, b) => new Date(a.item.datetime).getTime() - new Date(b.item.datetime).getTime());

			state.step = state.turns.length;
			state.boards = createTurnBoards(state.turns);
			//[
			//	...state.boards,
			//	action.payload.board
			//];
			state.players = action.payload.players;

			state.placeholders = createPlaceholderMap(state.boards[state.step], action.payload.playerTurn);
			state.hasGame = true;


			//console.log(createTurnBoards(action.payload.turns));
		},
		updateGame: (state, action: PayloadAction<Game>) => {

			if (state.turns.length !== action.payload.turns.length) {
				state.turn = action.payload.playerTurn as Color;
				state.turns = action.payload.turns;
				state.history = [
					//...messages.map(message => ({
					//	type: 'chat',
					//	item: message
					//}) as HistoryItem),
					...action.payload.turns.map(turn => ({
						type: 'history',
						item: turn
					}) as HistoryItem)
				].sort((a, b) => new Date(a.item.datetime).getTime() - new Date(b.item.datetime).getTime());

				state.step = action.payload.turns.length;
				state.boards = createTurnBoards(action.payload.turns);
				state.players = action.payload.players;

				state.placeholders = createPlaceholderMap(state.boards[state.step], action.payload.playerTurn);
				state.hasGame = true;
			}
		},
		dropGame: (state) => {
			state.hasGame = false;
			state.players = undefined;
			state.player = 1;
			state.turn = 1;
			state.turns = [];
			state.status = GameStatus.Staging;
			state.history = [];
			state.step = 0;
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
			state.placeholders = [];
			state.controls = {
				mode: 'perspective'
			};
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

export const { startGame, setBoard, setStep, setGame, dropGame, toggleCameraMode } = gameSlice.actions

export default gameSlice.reducer