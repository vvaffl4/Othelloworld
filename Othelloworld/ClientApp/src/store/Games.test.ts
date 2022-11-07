import Game, { GameStatus } from '../model/Game';
import PagedList from '../model/PagedList';
import { Color, GameResult } from '../model/PlayerInGame';
import reducer, { addGames, GamesState } from './Games';


const defaultState: GamesState = {
	currentPage: 1,
	totalPages: 0,
	pageSize: 5,
	totalCount: 0,
	items: []
}

describe("Redux Games", () => {
	test("Add games to gamelist", () => {
		const gameList: PagedList<Game> = {
			currentPage: 1,
			totalPages: 0,
			pageSize: 5,
			totalCount: 1,
			items: [
				{
					token: 'Game1Token',
					name: 'Game1',
					description: 'Game1Description',
					status: GameStatus.Staging,
					players: [{
						gameToken: 'Game1Token',
						color: Color.white,
						isHost: true,
						player: {
							username: 'Game1Player1',
							amountWon: 0,
							amountDraw: 0,
							amountLost: 0,
							countryCode: 'NL'
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
						[0, 0, 0, 0, 0, 0, 0, 0]
					],
					turns: []
				}
			]
	};

		expect(reducer(defaultState, addGames(gameList)))
			.toEqual(gameList);
	});
});