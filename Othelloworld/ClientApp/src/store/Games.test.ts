import Game, { GameStatus } from '../model/Game';
import { Color, GameResult } from '../model/PlayerInGame';
import reducer, { addGames, GamesState } from './Games';

const defaultState: GamesState = {
	byId: {},
	allIds: []
}

describe("Redux Games", () => {
	test("Add games to gamelist", () => {
		const gameList: Game[] = [
			{
				token: 'Game1Token',
				name: 'Game1',
				description: 'Game1Description',
				status: GameStatus.Staging,
				players: [{
					gameToken: 'Game1Token',
					username: 'Game1Player1',
					color: Color.white,
					isHost: true,
					player: {
						username: 'Game1Player1',
						amountWon: 0,
						amountDraw: 0,
						amountLost: 0
					},
					result: GameResult.undecided
				}, undefined],
				playerTurn: Color.black,
				board: [
					[0, 0, 0, 0, 0, 0, 0, 0],
					[0, 0, 0, 0, 0, 0, 0, 0],
					[0, 0, 0, 0, 0, 0, 0, 0],
					[0, 0, 0, 2, 1, 0, 0, 0],
					[0, 0, 0, 1, 2, 0, 0, 0],
					[0, 0, 0, 0, 0, 0, 0, 0],
					[0, 0, 0, 0, 0, 0, 0, 0],
					[0, 0, 0, 0, 0, 0, 0, 0],
				],
				turns: []

			}
		];

		expect(reducer(defaultState, addGames(gameList)))
			.toEqual({
				byId: { [gameList[0].token]: gameList[0] },
				allIds: [ gameList[0].token ]
			});
	});
});