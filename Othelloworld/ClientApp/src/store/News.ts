import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { Table } from "./Games";

interface NewsItem {
	title: string;
	image: string;
	content: string;
	timestamp: string;
}

interface News extends Table<number, NewsItem>{
}

const initialState: NewsItem[] = [
	{
		title: "How to Play",
		image: "How to Play",
		content: "For the first time we all wonder what Reversi is. To introduce you to the game, here is a little \"How to\" on the game.",
		timestamp: new Date().toISOString()
	},
	{
		title: "Stategies",
		image: "/img/strategies.png",
		content: "There are multiple strategies and different takes on the game. Learn here how you can best your opponent.",
		timestamp: new Date().toISOString()
	},
	{
		title: "Analyse",
		image: "/img/analyse.png",
		content: "When it's a loss or even a win, in order to improve, don't forget to analyse each step on the way to the result. ",
		timestamp: new Date().toISOString()
	}
];


export const newsSlice = createSlice({
	name: 'game',
	initialState,
	reducers: {
		addNewsItems: (state, action: PayloadAction<NewsItem[]>) => {
			state = [
				...state,
				...action.payload
			];
		},
	}
});

export const { addNewsItems } = newsSlice.actions

export default newsSlice.reducer