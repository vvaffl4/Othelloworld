import reducer, { GameState, setBoard } from './Game';

// Define the initial state using that type
const defaultState: GameState = {
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
};


describe('thingy', () => {
	test('should return the initial state', () => {
		const testBoard = [
			[0, 0, 0, 0, 0, 0, 0, 0],
			[0, 0, 0, 0, 0, 0, 0, 0],
			[0, 0, 2, 1, 0, 0, 0, 0],
			[0, 0, 0, 2, 1, 0, 0, 0],
			[0, 0, 0, 1, 2, 0, 0, 0],
			[0, 0, 0, 0, 0, 0, 0, 0],
			[0, 0, 0, 0, 0, 0, 0, 0],
			[0, 0, 0, 0, 0, 0, 0, 0],
		];

		expect(reducer(defaultState, setBoard(testBoard))).toEqual(
			{
				...defaultState,
				boards: [testBoard]
			}
		);
	})
});